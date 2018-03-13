using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MasterServer.Links;

using SharedComponents.Server;
using SharedComponents.Global;

using Extant;
using Extant.Networking;

namespace ForwardServer.Links
{
    public class ClientLink : ThreadRun
    {
        public enum ClientLinkState
        {
            Waiting,
            Authorizing,
            Active,
            Closed
        }

        private static UInt32 clientIDIterator = 0;
        private static object clientIDIterator_lock = new object();

        public UInt32 ClientID
        { get; private set; }
        public ClientLinkState State
        { get; private set; }
        public String Username
        { get; private set; }
        public String Password
        { get; private set; }

        private MasterServerLink masterServerLink;
        private NetConnection connection;

        public ClientLink(MasterServerLink masterServerLink, NetConnection connection)
            : base("Client-" + connection.RemoteEndPoint.ToString())
        {
            this.masterServerLink = masterServerLink;
            this.connection = connection;

            lock (clientIDIterator_lock)
            {
                ClientID = ++clientIDIterator;
            }
        }

        protected override void Begin()
        {
            masterServerLink.OnStateChange += MasterServerConnectionChanged;
            Log.Log("Start.");
        }

        protected override void RunLoop()
        {
            HandleMasterPackets();
            HandleClientPackets();
        }

        protected override void Finish(bool success)
        {
            masterServerLink.OnStateChange -= MasterServerConnectionChanged;
            connection.SendPacket(new ClientToForwardPackets.ErrorCode_c(ClientToForwardPackets.ErrorCode_c.ErrorCode.LostConnectionToMain));
            connection.Stop("ClientLink finished.");
            Log.Log("Finished.");
        }

        private void MasterServerConnectionChanged(MasterServerLink msl)
        {
            if (msl.ConnectedState == false)
            {
                this.Stop("Lost connection to MasterServer.");
            }
        }

        private void HandleMasterPackets()
        {
            Packet packet = null;
            while ((packet = masterServerLink.GetPacket()) != null)
            {
                switch (State)
                {
                    case ClientLinkState.Authorizing:
                        {
                            if (packet is ForwardToMasterPackets.AccountAuthorize_Response_f)
                            {
                                var p = packet as ForwardToMasterPackets.AccountAuthorize_Response_f;

                                if (p.Success)
                                {
                                    //Send client success
                                    connection.SendPacket(new ClientToForwardPackets.AccountAuthorize_Response_c(true));

                                    State = ClientLinkState.Active;
                                }
                                else
                                {
                                    //Send client wrong login
                                    connection.SendPacket(new ClientToForwardPackets.AccountAuthorize_Response_c(false));

                                    State = ClientLinkState.Closed;
                                    this.Stop("Invalid login from client: " + Username);
                                }
                            }
                            else
                            {
                                //Send client internal error
                                connection.SendPacket(new ClientToForwardPackets.ErrorCode_c(ClientToForwardPackets.ErrorCode_c.ErrorCode.InternalError));

                                Log.Log("Received wrong packet from Master. Expected Auth Response but got: " + packet.Type);
                                State = ClientLinkState.Closed;
                                this.Stop("Invalid packet from Master.");
                            }
                        }
                        break;

                    case ClientLinkState.Active:
                        {

                        }
                        break;

                    default:
                        {
                            //Send client internal error
                            connection.SendPacket(new ClientToForwardPackets.ErrorCode_c(ClientToForwardPackets.ErrorCode_c.ErrorCode.InternalError));

                            State = ClientLinkState.Closed;
                            this.Stop("Invalid state when interpretting packet from Master.");
                        }
                        break;
                }
            }
        }

        private void HandleClientPackets()
        {
            Packet packet = null;
            while ((packet = connection.GetPacket()) != null)
            {
                switch (State)
                {
                    case ClientLinkState.Waiting:
                        {
                            if (packet is ClientToForwardPackets.AccountAuthorize_Attempt_f)
                            {
                                var p = packet as ClientToForwardPackets.AccountAuthorize_Attempt_f;

                                //Send master server attempt at login
                                masterServerLink.SendPacket(ClientID, new ForwardToMasterPackets.AccountAuthorize_Attempt_m(p.Username, p.Password));

                                State = ClientLinkState.Authorizing;
                            }
                            else
                            {
                                //Send client invalid packet
                                connection.SendPacket(new ClientToForwardPackets.ErrorCode_c(ClientToForwardPackets.ErrorCode_c.ErrorCode.InvalidPacket));

                                State = ClientLinkState.Closed;
                                this.Stop("Client sent invalid packet.");
                            }
                        }
                        break;

                    case ClientLinkState.Active:
                        {

                        }
                        break;

                    default:
                        {
                            //Send client invalid packet
                            connection.SendPacket(new ClientToForwardPackets.ErrorCode_c(ClientToForwardPackets.ErrorCode_c.ErrorCode.InvalidPacket));

                            State = ClientLinkState.Closed;
                            this.Stop("Invalid packet during state from Client.");
                        }
                        break;
                }
            }
        }
    }
}
