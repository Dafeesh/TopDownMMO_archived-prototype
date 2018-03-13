﻿#define LOG_DEBUG

using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace Extant.Networking
{
    public class NetConnection : ThreadRun
    {
        private NetworkState state;
        private IPEndPoint remoteEndPoint;
        private readonly Stopwatch connectTimeoutTimer = new Stopwatch();
        private readonly Int32 connectTimeout;
        private readonly Stopwatch receiveTimeoutTimer = new Stopwatch();
        private readonly Int32 receiveTimeout;
        private TcpClient tcpClient;
        private IAsyncResult connectResult;

        private NetworkStream stream;
        private List<Byte> receiveBuffer = new List<byte>();
        private Byte[] receiveBuffer_temp = new Byte[1024];

        private Queue<Packet> packets = new Queue<Packet>();
        private object packets_lock = new object();
        private int packets_count_out = 0;
        private int packets_count_in = 0;

        public delegate Packet ReadBufferFunc(ref List<Byte> bytes);
        private ReadBufferFunc ReadBuffer;

        /// <summary>
        /// Used if connection is not already established.
        /// </summary>
        /// <param name="remoteEndPoint">Endpoint to connect to.</param>
        /// <param name="connectTimeout">How long connecting should try. (mSec)</param>
        /// <param name="readTimeout">How long it should wait for a packet. (mSec)</param>
        public NetConnection(ReadBufferFunc func, IPEndPoint remoteEndPoint, Int32 connectTimeout, Int32 receiveTimeout)
            : base("NetConnection-c")
        {
            ReadBuffer = func;

            this.state = NetworkState.Waiting;
            this.remoteEndPoint = remoteEndPoint;
            this.connectTimeout = connectTimeout;

            this.tcpClient = new TcpClient();
            this.receiveTimeout = receiveTimeout;
        }

        /// <summary>
        /// If connection is already established.
        /// </summary>
        public NetConnection(ReadBufferFunc func, TcpClient tcpClient, Int32 receiveTimeout)
            : base("NetConnection")
        {
            ReadBuffer = func;

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
                    //DebugLogger.GlobalDebug.LogNetworking("NetConnection: Attempting to connect to " + remoteEndPoint.Address.ToString() + "/" + remoteEndPoint.Port);
                    connectResult = this.tcpClient.BeginConnect(remoteEndPoint.Address.ToString(), remoteEndPoint.Port, new AsyncCallback(ConnectCallback), null);
                    connectTimeoutTimer.Start();
                    state = NetworkState.Connecting;
                }
                catch (Exception e)
                {
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "NetConnection: Exception while trying to start connecting.\n" + e.ToString());
                    this.Stop();
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
#if LOG_DEBUG
                            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: Connect timed out.");
#endif
                            this.Stop();
                        }
                        break;
                    }
                case (NetworkState.Connected):
                    {
                        if (receiveTimeoutTimer.ElapsedMilliseconds > receiveTimeout || !tcpClient.Connected)
                        {
#if LOG_DEBUG
                            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: Receive timed out.");
#endif
                            this.Stop();
                        }
                        break;
                    }
                case (NetworkState.Closed):
                    {
                        this.Stop();
                        break;
                    }
                default:
                    {
                        DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Fatal, "NetConnection: Invalid state: " + state.ToString());
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
#if LOG_DEBUG
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: ConnectCallback, connected!");
#endif
                    this.stream = tcpClient.GetStream();
                    state = NetworkState.Connected;

                    receiveTimeoutTimer.Reset();
                    receiveTimeoutTimer.Start();
                    BeginReceive(tcpClient.Client);
                }
                else
                {
#if LOG_DEBUG
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: ConnectCallback, no connection.");
#endif
                    this.Stop();
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            //DebugLogger.GlobalDebug.Log("RECEIVED: " + DateTime.Now.ToString());
            try
            {
                Socket client = (Socket)ar.AsyncState;

                int numBytes = client.EndReceive(ar);
                if (numBytes == 0)
                {
#if LOG_DEBUG
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: Client disconnected.");
#endif
                    this.Stop();
                }
                else
                {
                    receiveBuffer.AddRange(receiveBuffer_temp.Take(numBytes));
                    InterpretBuffer(receiveBuffer);
                    
#if LOG_DEBUG
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "NetConnection: Received bytes- " + numBytes);
#endif
                    ByteRecord.GlobalByteRecord_In.Bytes += numBytes;

                    BeginReceive(ar.AsyncState as Socket);
                    //BeginReceive(tcpClient);
                }
            }
            catch (Exception e)
            {
                DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Catch, "NetConnection: ReceiveCallback exception ->\n" + e.ToString() + "\n--");
                this.Stop();
            }
        }

        private void InterpretBuffer(List<byte> buffer)
        {
            if (buffer.Count > 0)
            {
                Packet p = null;
                while ((p = ReadBuffer(ref buffer)) != null)
                {
                    lock (packets_lock)
                    {
                        packets.Enqueue(p);
                    }
                }
            }
        }

        public Packet GetPacket()
        {
            Packet p = null;
            lock (packets_lock)
            {
                if (packets.Count > 0)
                {
                    p = packets.Dequeue();
                    packets_count_in++;
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

                packets_count_out++;
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
            Waiting, //Waiting to start a connection. 
            Connecting, //Attempting to connect.
            Connected, //Successfully connected.
            Closed //Socket closed.
        }
    }
}
