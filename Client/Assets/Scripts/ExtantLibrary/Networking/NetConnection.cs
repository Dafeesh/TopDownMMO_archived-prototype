using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

using Extant.GameServerShared;

namespace Extant.Networking
{
    public enum NetworkState
    {
        NoConnection,
        Connected,
        Failed
    }

    public class NetConnection : ThreadRun
    {
        private NetworkState state;
        private IPEndPoint remoteEndPoint;
        private readonly Stopwatch connectTimeoutTimer = new Stopwatch();
        private readonly Int32 connectTimeout;
        private TcpClient tcpClient;
        private IAsyncResult connectResult;

        private NetworkStream stream;
        private List<Byte> receiveBuffer = new List<byte>();
        private Byte[] receiveBuffer_temp = new Byte[1024];

        private Queue<Packet> packets = new Queue<Packet>();
        private object packets_lock = new object();

        /// <summary>
        /// Used if connection is not already established.
        /// </summary>
        /// <param name="remoteEndPoint">Endpoint to connect to.</param>
        /// <param name="connectTimeout">How long connecting should try. (mSec)</param>
        /// <param name="readTimeout">How long it should wait for a packet. (mSec)</param>
        public NetConnection(IPEndPoint remoteEndPoint, Int32 connectTimeout, Int32 receiveTimeout)
            : base("NetConnection-c")
        {
            state = NetworkState.Waiting;
            this.remoteEndPoint = remoteEndPoint;
            this.connectTimeout = connectTimeout;

            this.tcpClient = new TcpClient();
            this.tcpClient.ReceiveTimeout = receiveTimeout;
        }

        /// <summary>
        /// If connection is already established.
        /// </summary>
        public NetConnection(TcpClient tcpClient)
            : base("NetConnection-s")
        {
            state = NetworkState.Connected;
            this.tcpClient = tcpClient;
            this.stream = tcpClient.GetStream();
            this.remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            this.connectTimeout = 0;
        }

        protected override void Begin()
        {
            if (state == NetworkState.Waiting)
            {
                // Start connection attempt
                try
                {
                    //DebugLogger.GlobalDebug.LogNetworking("NetConnection: Attempting to connect to " + remoteEndPoint.Address.ToString() + "/" + remoteEndPoint.Port);
                    this.connectResult = this.tcpClient.BeginConnect(remoteEndPoint.Address.ToString(), remoteEndPoint.Port, new AsyncCallback(ConnectCallback), null);
                    this.connectTimeoutTimer.Start();
                    this.state = NetworkState.Connecting;
                }
                catch (Exception e)
                {
                    DebugLogger.GlobalDebug.LogError("NetConnection: Exception while trying to start connecting.\n" + e.ToString());
                    this.Stop();
                }
            }
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            if (!this.IsStopped)
            {
                if (tcpClient.Connected)
                {
                    //DebugLogger.GlobalDebug.LogNetworking("NetConnection: ConnectCallback, connected!");
                    this.stream = tcpClient.GetStream();
                    state = NetworkState.Connected;
                }
                else
                {
                    //DebugLogger.GlobalDebug.LogNetworking("NetConnection: ConnectCallback, no connection.");
                    this.Stop();
                }
            }
        }

        protected override void RunLoop()
        {
            try
            {
                switch (state)
                {
                    case(NetworkState.Waiting):
                        {
                            DebugLogger.GlobalDebug.LogError("NetConnection: state == waiting!");
                            this.Stop();
                            break;
                        }
                    case(NetworkState.Connecting):
                        {
                            if (connectTimeoutTimer.ElapsedMilliseconds > connectTimeout)
                            {
                                //DebugLogger.GlobalDebug.LogNetworking("NetConnection: Failed to connect.");
                                this.Stop();
                            }
                            break;
                        }
                    case(NetworkState.Connected):
                        {
                            ReceiveData();
                            InterpretBuffer();
                            if (!tcpClient.Connected)
                            {
                                this.Stop();
                            }
                            break;
                        }
                    case(NetworkState.Failed):
                        {
                            this.Stop();
                            break;
                        }
                    default:
                        {
                            throw new SocketException((int)SocketError.OperationNotSupported);
                        }
                }
            }
            catch (SocketException)
            {
                //DebugLogger.GlobalDebug.LogCatch("NetConnection: Error and/or disconnected.\n" + e);
                if (tcpClient.Connected)
                    tcpClient.Client.Disconnect(true);
                this.Stop();
            }
        }

        private void ReceiveData()
        {
            try
            {
                int numBytes = stream.Read(receiveBuffer_temp, 0, receiveBuffer_temp.Length);
                if (numBytes == 0) //Disconnected
                {
                    //DebugLogger.GlobalDebug.LogCatch("NetConnection: Lost connection. (zero bytes read)");
                    this.Stop();
                }
                else //Receive data
                {
                    receiveBuffer.AddRange(receiveBuffer_temp.Take(numBytes));
                    //DebugLogger.GlobalDebug.LogNetworking("NetConnection: Received bytes- " + numBytes);
                    ByteRecord.GlobalByteRecord_In.Bytes += numBytes;
                }
            }
            catch (IOException)
            {
                //DebugLogger.GlobalDebug.LogCatch("NetConnection: Lost connection. (IOException)");
                this.Stop();
            }
        }

        private void InterpretBuffer()
        {
            if (receiveBuffer.Count > 0)
            {
                Packet p = null;
                while ((p = GameServerPackets.ReadBuffer(ref receiveBuffer)) != null)
                {
                    lock (packets_lock)
                    {
                        packets.Enqueue(p);
                    }
                }
            }
        }

        protected override void Finish()
        {
            tcpClient.Close();
            connectTimeoutTimer.Stop();
            state = NetworkState.Failed;
        }

        public Packet GetPacket()
        {
            Packet p = null;
            lock (packets_lock)
            {
                if (packets.Count > 0)
                {
                    p = packets.Dequeue();
                }
            }
            return p;
        }

        public void SendPacket(Packet p)
        {
            if (tcpClient.Connected)
            {
                Byte[] d = p.CreateSendBuffer();
                stream.Write(d, 0, d.Length);
                stream.Flush();
            }
        }

        /// <summary>
        /// Gets and sets the time the connection will wait for a packet.
        /// </summary>
        public Int32 ReceiveTimeout
        {
            get
            {
                return tcpClient.ReceiveTimeout;
            }
            set
            {
                tcpClient.ReceiveTimeout = value;
            }
        }

        public NetworkState State
        {
            get
            {
                return state;
            }
        }

        public enum NetworkState
        {
            Waiting,
            Connecting,
            Connected,
            Failed
        }
    }
}
