using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Diagnostics;

using Extant;
using Extant.Networking;

using SharedComponents;

namespace WorldServer.Networking
{
    public class ClientConnection : ThreadRun
    {
        private const Int32 RECEIVE_TIMEOUT = 180000;

        private NetConnection connection;
        private Stopwatch lifeTimeTimer = new Stopwatch();

        private Int32 verifyBuild;
        private String verifyUsername;
        private Int32 verifyPasswordToken;

        private ClientState state = ClientState.Null;

        public ClientConnection(TcpClient tcpClient)
            : base("Client")
        {
            connection = new NetConnection(ClientToWorldPackets.ReadBuffer, tcpClient, RECEIVE_TIMEOUT);
            //Log.MessageLogged += Console.WriteLine;
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
            if (connection.State == NetConnection.NetworkState.Closed)
            {
                this.Stop("Connection lost.");
                this.state = ClientState.Disconnected;
                return;
            }

            switch (state)
            {
                case (ClientState.Disconnected):
                    {
                        this.Stop("Disconnected.");
                        break;
                    }
                case (ClientState.Varifying):
                    {
                        Packet p = null;
                        //Read Packet and see if it varification
                        if ((p = connection.GetPacket()) != null)
                        {
                            if (p.Type == (Int32)ClientToWorldPackets.PacketType.Verify_Details_w)
                            {
                                verifyBuild = (p as ClientToWorldPackets.Verify_Details_g).build;
                                verifyUsername = (p as ClientToWorldPackets.Verify_Details_g).username;
                                verifyPasswordToken = (p as ClientToWorldPackets.Verify_Details_g).password;

                                state = ClientState.Connected;
                            }
                            else
                            {
                                this.Stop("Client sent wrong packet when trying to verify: " + p.Type.ToString());
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

        protected override void Finish(bool success)
        {
            lifeTimeTimer.Stop();

            Log.Log("Client finished.");
            connection.Stop("Client finished.");

            state = ClientState.Disconnected;
        }

        /// <summary>
        /// Returns if the Client is currently connected and received verification data.
        /// </summary>
        public Boolean IsAlive
        {
            get
            {
                return (state == ClientState.Connected);
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
        public Int32 VerifyPasswordToken
        {
            get
            {
                return verifyPasswordToken;
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
            Console.WriteLine("Packet -> " + (ClientToWorldPackets.PacketType)p.Type);
            connection.SendPacket(p);
        }
    }

    enum ClientState
    {
        Null,
        Disconnected,
        Varifying,
        Connected
    }
}
