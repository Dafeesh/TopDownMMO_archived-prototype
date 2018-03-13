using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

using Extant;
using Extant.Networking;
using GameServer;
using GameServer.Shared;

namespace GameServer.Networking
{
    public class Client : ThreadRun
    {
        private NetConnection connection;

        private Int32 verifyBuild;
        private String verifyUsername;
        private Int32 verifyPassword;
        private Stopwatch lifeTimeTimer = new Stopwatch();

        private ClientState state;

        public Client(TcpClient tcpClient)
            : base("Client")
        {
            connection = new NetConnection(tcpClient);
            connection.Start();
            state = ClientState.Varifying;
        }

        protected override void Begin()
        {
            lifeTimeTimer.Start();
        }

        protected override void RunLoop()
        {
            //Check connection
            if (connection.State == NetConnection.NetworkState.Failed)
            {
                this.Stop();
                return;
            }

            switch (state)
            {
                case (ClientState.Disconnected):
                    {
                        this.Stop();
                        break;
                    }
                case (ClientState.Varifying):
                    {
                        Packet p = null;
                        //Read Packet and see if it varification
                        if ((p = connection.GetPacket()) != null)
                        {
                            if (p.Type == Packet.PacketType.Verify_Details_g)
                            {
                                verifyBuild = (p as Packets.Verify_Details_g).build;
                                verifyUsername = (p as Packets.Verify_Details_g).username;
                                verifyPassword = (p as Packets.Verify_Details_g).password;

                                //Check if gameversion is correct
                                if (verifyBuild == GameVersion.Build)
                                {
                                    state = ClientState.Connected;
                                    connection.SendPacket(new Packets.Verify_Result_c(Packets.Verify_Result_c.VerifyReturnCode.Success));
                                }
                                else
                                {
                                    connection.SendPacket(new Packets.Verify_Result_c(Packets.Verify_Result_c.VerifyReturnCode.IncorrectVersion));
                                    this.Stop();
                                }
                            }
                            else
                            {
                                DebugLogger.GlobalDebug.LogCatch("Client received wrong packet when trying to verify: " + this.RunningID + "/" + p.Type.ToString());
                                this.Stop();
                                return;
                            }
                        }
                        break;
                    }
                    
                case (ClientState.Connected):
                    {

                        break;
                    }
            }
        }

        protected override void Finish()
        {
            DebugLogger.GlobalDebug.LogNetworking("Client disconnected. (" + this.RunningID + ")");
            connection.Stop();
            lifeTimeTimer.Stop();
            state = ClientState.Disconnected;
        }

        /// <summary>
        /// Returns if the Client is currently connected and verified.
        /// </summary>
        public Boolean IsConnected
        {
            get
            {
                return (state == ClientState.Connected);
            }
        }

        /// <summary>
        /// Returns the username received as verification.
        /// </summary>
        public String VerifyUsername
        {
            get
            {
                return verifyUsername;
            }
        }

        /// <summary>
        /// Returns the password received as verification.
        /// </summary>
        public Int32 VerifyPassword
        {
            get
            {
                return verifyPassword;
            }
        }

        /// <summary>
        /// Returns the time in milliseconds of how long the client has been active.
        /// </summary>
        public Int32 LifeTime
        {
            get
            {
                return (Int32)lifeTimeTimer.ElapsedMilliseconds;
            }
        }

        /// <summary>
        /// Dequeues the oldest received packet.
        /// </summary>
        /// <returns>The dequeued packet.</returns>
        public Packet GetPacket()
        {
            return connection.GetPacket();
        }

        /// <summary>
        /// Sends a packet to the client if it holds a connection.
        /// </summary>
        /// <param name="p">The packet being sent.</param>
        public void SendPacket(Packet p)
        {
            connection.SendPacket(p);
        }
    }

    enum ClientState
    {
        Disconnected,
        Varifying,
        Connected
    }
}
