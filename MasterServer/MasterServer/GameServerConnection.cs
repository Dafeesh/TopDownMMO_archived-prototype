using System;
using System.Net.Sockets;

using Extant;
using Extant.Networking;
using SharedComponents.ServerToServer;

namespace MasterServer
{
    class GameServerConnection : ThreadRun
    {
        private NetConnection connection;

        public GameServerConnection(TcpClient client)
            : base("GameServerConnection")
        {
            connection = new NetConnection(WorldToMasterPackets.ReadBuffer, client);
        }
    }
}
