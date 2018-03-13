using System;
using System.Collections.Generic;
using System.Net;

using SharedComponents.Server;

using Extant.Networking;

namespace MasterServer.Links
{
    public class ForwardServerLink
    {
        public event StateChange OnStateChange;
        public delegate void StateChange(ForwardServerLink fwdServer);

        public IPEndPoint RemoteEndPoint
        { get; private set; }

        private NetConnection connection;
        private bool _connectedState;

        public ForwardServerLink(IPEndPoint remoteEndPoint)
        {
            this.RemoteEndPoint = remoteEndPoint;

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

        public Packet GetPacket()
        {
            if (ConnectedState == true)
            {
                return connection.GetPacket();
            }
            else
                return null;
        }

        public void SendPacket(Packet p)
        {
            if (ConnectedState == true)
            {
                connection.SendPacket(p);
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
    }
}
