using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using SharedComponents.Global;
using SharedComponents.Server;

namespace MasterServer.Links
{
    public abstract class ServerLink : IDisposable, ILogging
    {
        private const uint CONNECTION_RETRYDELAY = 5000;

        public event StateChange OnStateChange;
        public delegate void StateChange(ServerLink instServLink);

        public string ServerName
        { get; private set; }
        public IPEndPoint RemoteEndPoint
        { get; private set; }
        public IPEndPoint BroadcastEndPoint
        { get; private set; }

        private NetConnection connection;
        private Stopwatch connection_retryTimer = new Stopwatch();

        private bool _isConnected = false;
        private bool _isDisposed = false;
        private DebugLogger _log;

        public ServerLink(string serverName,
                          IPEndPoint remoteEndPoint,
                          IPEndPoint broadcastEndPoint)
        {
            this.Log = new DebugLogger("ServLink-" + serverName);
            this.Log.MessageLogged += Console.WriteLine;

            this.ServerName = serverName;
            this.RemoteEndPoint = remoteEndPoint;
            this.BroadcastEndPoint = broadcastEndPoint;

            RestartConnectionAsync(0);
        }

        private async void RestartConnectionAsync(uint delay)
        {
            if (connection != null)
                connection.Close();

            if (delay > 0)
                await Task.Delay((int)delay);

            connection = new NetConnection(RemoteEndPoint);
            connection.Start();
            connection.OnStateChange += OnStateChange_Connection;
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                connection.Dispose();
            }
        }

        private void OnStateChange_Connection(NetConnection.NetworkState state)
        {
            switch (state)
            {
                case (NetConnection.NetworkState.Active):
                    {
                        if (!IsConnected)
                            IsConnected = true;
                    }
                    break;

                case (NetConnection.NetworkState.Closed):
                    {
                        if (IsConnected)
                            IsConnected = false;
                    }
                    break;
            }

            if (IsConnected)
            {
                if (state == NetConnection.NetworkState.Closed)
                {
                    Log.Log("Disconnected.");
                    RestartConnectionAsync(0);
                    IsConnected = false;
                }
            }
            else
            {
                if (state == NetConnection.NetworkState.Active)
                {
                    Log.Log("Connected!");
                    IsConnected = true;
                }
                else if (connection.State == NetConnection.NetworkState.Closed)
                {
                    //Log.Log("Failed to connect. Retrying in " + (CONNECTION_RETRYDELAY / 1000).ToString() + " seconds.");
                    RestartConnectionAsync(CONNECTION_RETRYDELAY);
                }

            }
        }

        public bool IsConnected
        {
            get
            {
                return _isConnected;
            }
            private set
            {
                if (value != _isConnected)
                {
                    _isConnected = value;
                    if (OnStateChange != null)
                        OnStateChange(this);
                }
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

        public override string ToString()
        {
            return ServerName;
        }
    }
}
