using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Extant;

namespace Extant.Networking
{
    public class NetConnection : IDisposable, ILogging
    {
        private TcpClient tcpClient;

        private NetworkState _state;
        private IPEndPoint _remoteEndPoint;
        private readonly Stopwatch lastReceiveTimer = new Stopwatch();

        private NetworkStream stream;
        private object stream_lock = new object();
        private List<Byte> receiveBuffer = new List<byte>();
        private object receiveBuffer_lock = new object();
        private Byte[] receiveBuffer_temp = new Byte[1024];

        private DebugLogger _log;
        private ByteRecord byteLog_out = new ByteRecord();
        private ByteRecord byteLog_in = new ByteRecord();
        private bool _isDisposed = false;

        /// <summary>
        /// Used if connection is not already established.
        /// </summary>
        public NetConnection(IPEndPoint remoteEndPoint)
        {
            this.Log = new DebugLogger("NetCon");

            this.RemoteEndPoint = remoteEndPoint;

            this.tcpClient = new TcpClient();
            this.State = NetworkState.Waiting_ToConnect;
        }

        /// <summary>
        /// If connection is already established.
        /// </summary>
        public NetConnection(TcpClient tcpClient)
        {
            this.Log = new DebugLogger("NetCon");

            this.RemoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;

            this.tcpClient = tcpClient;
            this.stream = tcpClient.GetStream();
            this.State = NetworkState.Waiting_Connected;
        }

        /// <summary>
        /// Start connection attempt.
        /// </summary>
        public void Start()
        {
            if (IsClosed)
                throw new InvalidOperationException("NetConnection cannot start while closed.");
            if (IsDisposed)
                throw new InvalidOperationException("NetConnection cannot start while disposed.");

            if (State == NetworkState.Waiting_ToConnect)
            {
                BeginConnect();

                State = NetworkState.Connecting;
                Log.Log("Attempting to connect to " + RemoteEndPoint.Address.ToString() + "/" + RemoteEndPoint.Port);
            }
            else if (State == NetworkState.Waiting_Connected)
            {

                BeginReceive();

                State = NetworkState.Active;
                Log.Log("Connection handling started.");
            }
            else
            {
                throw new InvalidOperationException("Invalid state while trying to start!");
            }

        }

        /// <summary>
        /// Closes an active connection.
        /// </summary>
        public void Close()
        {
            if (!IsClosed)
            {
                this.tcpClient.Close();

                this.State = NetworkState.Closed;
                Log.Log("Closed.");
            }
        }

        /// <summary>
        /// Stops and disposes an active connection.
        /// </summary>
        public void Dispose()
        {
            if (!IsClosed)
                this.Close();

            if (!IsDisposed)
            {
                Log.Log("Disposed.");
            }
        }

        private void BeginConnect()
        {
            this.tcpClient.BeginConnect(RemoteEndPoint.Address.ToString(), RemoteEndPoint.Port, new AsyncCallback(Callback_Connect), tcpClient);
        }

        private void BeginReceive()
        {
            this.tcpClient.Client.BeginReceive(receiveBuffer_temp, 0, receiveBuffer_temp.Length, SocketFlags.None, Callback_Receive, this.tcpClient.Client);
        }

        private void Callback_Connect(IAsyncResult ar)
        {
            if (!IsDisposed)
            {
                if (tcpClient.Connected)
                {
                    this.stream = tcpClient.GetStream();

                    BeginReceive();

                    this.State = NetworkState.Active;
                    Log.Log("ConnectCallback, success.");
                }
                else
                {
                    Log.Log("ConnectCallback, failed to connect.");
                }
            }
            else
            {
                Log.Log("ConnectCallback, disposed.");
            }
        }

        private void Callback_Receive(IAsyncResult ar)
        {
            if (!IsDisposed)
            {
                try
                {
                    Socket client = (Socket)ar.AsyncState;

                    int numBytes = client.EndReceive(ar);
                    if (numBytes == 0)
                    {
                        Log.Log("ReceiveCallback, lost connection.");
                        this.Close();
                    }
                    else
                    {
                        lastReceiveTimer.Reset();
                        lastReceiveTimer.Start();

                        lock (receiveBuffer_lock)
                        {
                            byteLog_in.Bytes += numBytes;
                            receiveBuffer.AddRange(receiveBuffer_temp.Take(numBytes));
                        }

                        Log.Log("ReceiveCallback, received bytes- " + numBytes);
                        BeginReceive();
                    }
                }
                catch (ObjectDisposedException)
                {
                    Log.Log("ReceiveCallback, disposed.");
                }
                catch (Exception e)
                {
                    Log.Log("ReceiveCallback exception: " + e.ToString());
                    this.Close();
                }
            }
        }

        /// <returns>If a packet was distributed.</returns>
        public bool DistributePacket(IPacketDistributor distributor)
        {
            bool sentPacket = false;
            lock (receiveBuffer_lock)
            {
                try
                {
                    sentPacket = (distributor.DistributePacket(ref receiveBuffer) == true);
                }
                catch (Packet.InvalidPacketRead e)
                {
                    Log.Log("Invalid packet read: " + e.ToString());
                    this.Close();
                }
            }
            return sentPacket;
        }

        public void SendPacket(Packet p)
        {
            try
            {
                if (tcpClient.Connected)
                {
                    Byte[] d = p.CreateSendBuffer();
                    lock (stream_lock)
                    {
                        byteLog_out.Bytes += d.Length;
                        stream.Write(d, 0, d.Length);
                        stream.Flush();
                    }

                }
            }
            catch (Exception e)
            {
                Log.Log("Exception while while sending packet: " + e.Message);
                this.Close();
            }
        }

        public NetworkState State
        {
            get
            {
                return _state;
            }

            private set
            {
                _state = value;
            }
        }

        public Int32 TimeSinceLastReceive
        {
            get
            {
                return (Int32)lastReceiveTimer.ElapsedMilliseconds;
            }
        }

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                return _remoteEndPoint;
            }

            private set
            {
                _remoteEndPoint = value;
            }
        }

        public DebugLogger Log
        {
            get
            {
                return _log;
            }

            private set
            {
                _log = value;
            }
        }

        public bool IsClosed
        {
            get
            {
                return (State == NetworkState.Closed);
            }
        }


        public bool IsDisposed
        {
            get
            {
                return _isDisposed;
            }

            private set
            {
                _isDisposed = value;
            }
        }

        public enum NetworkState
        {
            Waiting_ToConnect, //Waiting to start a connection. 
            Waiting_Connected, //Waiting with an active connection.
            Connecting, //Attempting to connect.
            Active, //Actively receiving.
            Closed //Socket closed.
        }
    }
}
