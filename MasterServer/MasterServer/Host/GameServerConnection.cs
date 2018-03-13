using System;
using System.Net.Sockets;

using Extant;
using Extant.Networking;
using SharedComponents.ServerToServer;

namespace MasterServer.Host
{
    class GameServerConnection : ThreadRun
    {
        private string name;
        private NetConnection connection;

        public GameServerConnection(String name, TcpClient client)
            : base("GSConnection-" + name)
        {
            this.name = name;
            this.connection = new NetConnection(WorldToMasterPackets.ReadBuffer, client);
        }

        protected override void Begin()
        {
            Log.Log("Connection started.");
        }

        protected override void RunLoop()
        {
            if (connection.State != NetConnection.NetworkState.Closed)
            {
                
            }
            else
                this.Stop("Disconnected.");
        }

        protected override void Finish(bool success)
        {
            connection.Dispose();
            Log.Log("Connection ended.");
        }

        public void SendPacket(Packet p)
        {
            connection.SendPacket(p);
        }

        public Packet GetPacket()
        {
            return connection.GetPacket();
        }
    }
}
