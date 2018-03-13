using System;
using System.Collections.Generic;
using System.Net;
using System.Diagnostics;

using SharedComponents.Server;
using SharedComponents.Global;

using Extant;
using Extant.Networking;

namespace MasterServer.Links
{
    public class InstanceServerLink : IDisposable, ILogging
    {
        private const Int32 CONNECTION_RETRYDELAY = 5000;

        public event StateChange OnStateChange;
        public delegate void StateChange(InstanceServerLink instServLink);

        public Int32 ServerId
        { get; private set; }
        public string Name
        { get; private set; }
        public IPEndPoint RemoteEndPoint
        { get; private set; }
        public IPEndPoint BroadcastEndPoint
        { get; private set; }
        public List<AccountInfo> ActiveCharacters
        { get; private set; }

        private NetConnection connection;
        private InstanceToMasterPackets.Distribution connection_distribution;
        private Stopwatch connection_retryTimer = new Stopwatch();

        private bool _isConnected = false;
        private bool _isDisposed = false;
        private DebugLogger _log;

        public InstanceServerLink(Int32 serverId,
                                  string name,
                                  IPEndPoint remoteEndPoint,
                                  IPEndPoint broadcastEndPoint)
        {
            this.Log = new DebugLogger("InstServLink-" + serverId);
            this.Log.MessageLogged += Console.WriteLine;

            this.ServerId = serverId;
            this.Name = name;
            this.RemoteEndPoint = remoteEndPoint;
            this.BroadcastEndPoint = broadcastEndPoint;
            this.ActiveCharacters = new List<AccountInfo>();

            this.connection_distribution = new InstanceToMasterPackets.Distribution()
            {
                out_Ping_im = OnReceive_Ping_im
            };

            RestartConnection();
        }

        public void Dispose()
        {
            if (!IsDisposed)
            {
                IsDisposed = true;

                connection.Dispose();
                connection_distribution.Dispose();
            }
        }

        private void RestartConnection()
        {
            if (connection != null)
                connection.Dispose();

            connection = new NetConnection(RemoteEndPoint, 5000);
            connection.Start();
        }

        public void PollConnection()
        {
            if (IsConnected)
            {
                if (connection.State == NetConnection.NetworkState.Closed)
                {
                    Log.Log("Disconnected.");
                    RestartConnection();
                    IsConnected = false;
                }
            }
            else
            {
                if (connection.State == NetConnection.NetworkState.Connected)
                {
                    Log.Log("Connected!");
                    IsConnected = true;
                }
                else if (connection.State == NetConnection.NetworkState.Closed)
                {
                    if (connection_retryTimer.IsRunning)
                    {
                        if (connection_retryTimer.ElapsedMilliseconds > CONNECTION_RETRYDELAY)
                        {
                            connection_retryTimer.Reset();
                            RestartConnection();
                        }
                    }
                    else
                    {
                        //Log.Log("Failed to connect, trying again in " + (int)(CONNECTION_RETRYDELAY/1000) + " seconds...");
                        connection_retryTimer.Start();
                    }
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
            return Name;
        }

        #region OnPacketReceive

        private void OnReceive_Ping_im(InstanceToMasterPackets.Ping_im packet)
        {
            
        }

        #endregion OnPacketReceive
    }
}
