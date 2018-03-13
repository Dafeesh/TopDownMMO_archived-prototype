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
    public class NetConnection : ThreadRun
    {
        private const int RECEIVETIMEOUT_INFINITY = 0;

        private NetworkState state;
        private IPEndPoint remoteEndPoint;
        private readonly Stopwatch connectTimeoutTimer = new Stopwatch();
        private readonly Int32 connectTimeout;
        private readonly Stopwatch receiveTimeoutTimer = new Stopwatch();
        private readonly Int32 receiveTimeout;
        private TcpClient tcpClient;
        private IAsyncResult connectResult;

        private NetworkStream stream;
        private object stream_lock = new object();
        private List<Byte> receiveBuffer = new List<byte>();
        private object receiveBuffer_lock = new object();
        private Byte[] receiveBuffer_temp = new Byte[1024];

        private ByteRecord byteLog_out = new ByteRecord();
        private ByteRecord byteLog_in = new ByteRecord();

        /// <summary>
        /// Used if connection is not already established.
        /// </summary>
        public NetConnection(IPEndPoint remoteEndPoint, Int32 connectTimeout, Int32 receiveTimeout = RECEIVETIMEOUT_INFINITY)
            : base("NetConnection-c")
        {
            this.state = NetworkState.Waiting;
            this.remoteEndPoint = remoteEndPoint;
            this.connectTimeout = connectTimeout;

            this.tcpClient = new TcpClient();
            this.receiveTimeout = receiveTimeout;
        }

        /// <summary>
        /// If connection is already established.
        /// </summary>
        public NetConnection(TcpClient tcpClient, Int32 receiveTimeout = RECEIVETIMEOUT_INFINITY)
            : base("NetConnection")
        {
            state = NetworkState.Connected;
            this.tcpClient = tcpClient;
            this.receiveTimeout = receiveTimeout;
            this.stream = tcpClient.GetStream();
            this.remoteEndPoint = (IPEndPoint)tcpClient.Client.RemoteEndPoint;
            this.connectTimeout = 0;

            BeginReceive(this.tcpClient.Client);
        }

        protected override void Begin()
        {
            if (state == NetworkState.Waiting)
            {
                // Start connection attempt
                try
                {
                    Log.Log("Attempting to connect to " + remoteEndPoint.Address.ToString() + "/" + remoteEndPoint.Port);
                    connectResult = this.tcpClient.BeginConnect(remoteEndPoint.Address.ToString(), remoteEndPoint.Port, new AsyncCallback(ConnectCallback), null);
                    connectTimeoutTimer.Start();
                    state = NetworkState.Connecting;
                }
                catch (Exception e)
                {
                    this.Stop("Exception while trying to start connecting.\n" + e.ToString());
                }
            }
        }

        protected override void RunLoop()
        {
            switch (state)
            {
                case (NetworkState.Connecting):
                    {
                        if (connectTimeoutTimer.ElapsedMilliseconds > connectTimeout)
                        {
                            this.Stop("Connect timed out.");
                        }
                        break;
                    }
                case (NetworkState.Connected):
                    {
                        if (receiveTimeout != RECEIVETIMEOUT_INFINITY)
                        {
                            if (receiveTimeoutTimer.ElapsedMilliseconds > receiveTimeout)
                                this.Stop("Receive timed out.");
                            else if (!tcpClient.Connected)
                                this.Stop("Disconnected.");
                        }
                        break;
                    }
                case (NetworkState.Closed):
                    {
                        this.Stop("Closed.");
                        break;
                    }
                default:
                    {
                        Log.Log("Invalid state: " + state.ToString());
                        throw new SocketException((int)SocketError.OperationNotSupported);
                    }
            }
        }

        protected override void Finish(bool success)
        {
            if (tcpClient != null)
            {
                tcpClient.Close();
            }
            connectTimeoutTimer.Stop();
            state = NetworkState.Closed;
            Log.Log("Finished.");
        }

        private void BeginReceive(Socket client)
        {
            client.BeginReceive(receiveBuffer_temp, 0, receiveBuffer_temp.Length, SocketFlags.None, ReceiveCallback, client);
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            if (!this.IsStopped)
            {
                if (tcpClient.Connected)
                {
                    Log.Log("ConnectCallback, connected!");

                    this.stream = tcpClient.GetStream();
                    state = NetworkState.Connected;

                    BeginReceive(tcpClient.Client);
                }
                else
                {
                    this.Stop("Callback, no connection.");
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int numBytes = client.EndReceive(ar);
                if (numBytes == 0)
                {
                    this.Stop("Receive callback: lost connection.");
                }
                else
                {
                    receiveTimeoutTimer.Reset();
                    receiveTimeoutTimer.Start();

                    lock (receiveBuffer_lock)
                    {
                        receiveBuffer.AddRange(receiveBuffer_temp.Take(numBytes));
                    }

                    Log.Log("Received bytes- " + numBytes);
                    byteLog_in.Bytes += numBytes;

                    BeginReceive(ar.AsyncState as Socket);
                }
            }
            catch (Exception e)
            {
                this.Stop("ReceiveCallback exception: " + e.ToString());
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
                    this.Stop("Invalid packet read: " + e.ToString());
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
                        stream.Write(d, 0, d.Length);
                        stream.Flush();
                    }

                    byteLog_out.Bytes += d.Length;
                }
            }
            catch (Exception e)
            {
                Log.Log("Exception while while sending packet: " + e.Message);
            }
        }

        /// <summary>
        /// Gets and sets the time the connection will wait for a packet. (ms)
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

        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (tcpClient != null)
                    return (IPEndPoint)tcpClient.Client.RemoteEndPoint;
                else
                    return null;
            }
        }

        public enum NetworkState
        {
            Waiting, //Waiting to start a connection. 
            Connecting, //Attempting to connect.
            Connected, //Successfully connected.
            Closed //Socket closed.
        }
    }
}
