using Extant;
using Extant.Networking;
using SharedComponents.ServerToServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MasterServer.Host
{
    public class AuthenticatingClient : ThreadRun
    {
        public AuthenticationStep State
        { get; private set; }
        public Int32 Authenticate_ServerId
        { get; private set; }
        public Int32 Authenticate_Build
        { get; private set; }
        public IPEndPoint IPEndPoint
        { get; private set; }

        private NetConnection client;

        public AuthenticatingClient(NetConnection client)
            : base("AuthenticatingClient")
        {
            this.client = client;
            this.IPEndPoint = client.RemoteEndPoint;
            State = AuthenticationStep.Waiting;
        }

        protected override void RunLoop()
        {
            if (client.State != NetConnection.NetworkState.Closed)
            {
                if (State != AuthenticationStep.Success)
                {
                    Packet packet = client.GetPacket();
                    if (packet != null)
                    {
                        if ((WorldToMasterPackets.PacketType)packet.Type == WorldToMasterPackets.PacketType.Authenticate_Attempt_m)
                        {
                            WorldToMasterPackets.Authenticate_Attempt_m pp = packet as WorldToMasterPackets.Authenticate_Attempt_m;

                            Authenticate_ServerId = pp.serverId;
                            Authenticate_Build = pp.buildNumber;

                            State = AuthenticationStep.Success;
                        }
                        else
                        {
                            State = AuthenticationStep.Failed;
                            this.Stop("Received wrong packet: " + ((WorldToMasterPackets.PacketType)packet.Type).ToString());
                        }
                    }
                }
            }
            else
            {
                State = AuthenticationStep.Failed;
                this.Stop("Disconnected.");
            }
        }

        public NetConnection TakeNetConnection()
        {
            ThreadTask<Object> task = new ThreadTask<Object>((ThreadTaskParams parameters) =>
            {
                NetConnection toReturn = null;

                toReturn = client;
                client = null;
                this.Stop("NetConnection was taken.");

                return toReturn;
            }, null);

            this.Invoke(task);
            return task.Result as NetConnection;
        }

        public void SendPacket(Packet p)
        {
            client.SendPacket(p);
        }

        public enum AuthenticationStep
        {
            Waiting,
            Success,
            Failed
        }
    }
}
