using System;
using System.Collections.Generic;
using System.Net;

using SharedComponents.Server;

using Extant;
using Extant.Networking;

namespace MasterServer.Links
{
    public class InstanceServerLink
    {
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
        private bool _connectedState;

        public InstanceServerLink(Int32 serverId,
                                  string name,
                                  IPEndPoint remoteEndPoint,
                                  IPEndPoint broadcastEndPoint)
        {
            this.ServerId = serverId;
            this.Name = name;
            this.RemoteEndPoint = remoteEndPoint;
            this.BroadcastEndPoint = broadcastEndPoint;
            this.ConnectedState = false;

            this.ActiveCharacters = new List<AccountInfo>();

            RestartConnection();
        }

        private void RestartConnection()
        {
            if (connection != null)
                connection.Dispose();

            connection = new NetConnection(InstanceToMasterPackets.ReadBuffer, RemoteEndPoint, 5000);
            connection.Start();
        }

        public void PollConnection()
        {
            if (ConnectedState == true)
            {
                if (connection.State == NetConnection.NetworkState.Closed)
                {
                    RestartConnection();
                    ConnectedState = false;
                }
            }
            else //if (ConnectedStatus == false)
            {
                if (connection.State == NetConnection.NetworkState.Connected)
                {
                    ConnectedState = true;
                }
            }
        }

        public bool ConnectedState
        {
            get
            {
                return _connectedState;
            }
            private set
            {
                _connectedState = value;
                if (OnStateChange != null)
                    OnStateChange(this);
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
