using System;
using System.Collections.Generic;
using System.Net;

using SharedComponents.ServerToServer;
using Extant.Networking;

namespace MasterServer.Host
{
    public class WorldServerLink
    {
        public Int32 ServerId
        { get; private set; }
        public string Name
        { get; private set; }
        public IPEndPoint ExpectedRemoteEndPoint
        { get; private set; }
        public List<AccountInfo> ActiveCharacters
        { get; private set; }

        private NetConnection connection = null;

        public WorldServerLink(Int32 serverId, string name, IPEndPoint expectedRemoteEndPoint)
        {
            this.ServerId = serverId;
            this.Name = name;
            this.ExpectedRemoteEndPoint = expectedRemoteEndPoint;
            this.ActiveCharacters = new List<AccountInfo>();
        }

        public override string ToString()
        {
            return Name;
        }

        public void SetConnection(NetConnection connection)
        {
            if (connection != null)
            {

            }
            this.connection = connection;
        }

        public bool IsConnected
        {
            get
            {
                if (connection != null && connection.State == NetConnection.NetworkState.Connected)
                    return true;
                else
                    return false;
            }
        }
    }
}
