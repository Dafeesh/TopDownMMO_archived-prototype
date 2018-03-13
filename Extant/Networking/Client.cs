using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

using Extant;
using Extant.Networking;
using GameServer;
using GameServer.Shared;

namespace GameServer.Networking
{
    public class Client : ThreadRun
    {
        private NetConnection connection;

        private String verifyVersion;
        private String verifyUsername;
        private String verifyPassword;

        private ClientState state;

        private List<Byte> receiveBuffer = new List<byte>();
        private Queue<Packet> packets = new Queue<Packet>();
        private object packets_lock = new object();

        public Client(TcpClient tcpClient)
            : base("Client")
        {
            connection = new NetConnection(tcpClient);
            connection.Start();
            state = ClientState.Varifying;

            DebugLogger.GlobalDebug.LogNetworking("Client started. (" + this.RunningID + ")");
        }

        protected override void RunLoop()
        {
            if (connection.State == NetConnection.NetworkState.Failed)
            {
                DebugLogger.GlobalDebug.LogNetworking("Client disconnected: " + this.RunningID);
                this.Stop();
                return;
            }

            switch (state)
            {
                case (ClientState.Disconnected):
                    {

                        break;
                    }
                case (ClientState.Varifying):
                    {
                        //Read Packet and see if it varification
                        if (packets.Count > 0)
                        {
                            Packet varifyPacket = null;
                            lock (packets_lock)
                            {
                                varifyPacket = packets.Dequeue();
                            }

                            if (varifyPacket.Type == Packet.PacketType.VarifyInfo_s)
                            {
                                DebugLogger.GlobalDebug.LogNetworking("Client received varification!");
                                verifyVersion = (varifyPacket as VarifyInfo_s).gameVersion;
                                verifyUsername = (varifyPacket as VarifyInfo_s).username;
                                verifyPassword = (varifyPacket as VarifyInfo_s).password;
                                state = ClientState.Connected;
                            }
                            else
                            {
                                DebugLogger.GlobalDebug.LogNetworking("Client received wrong packet when trying to varify: " + this.RunningID + "/" + varifyPacket.Type.ToString());
                                this.Stop();
                                return;
                            }
                        }
                        //             |
                        //Fall through v
                        goto case (ClientState.Connected);
                    }
                    
                case (ClientState.Connected):
                    {
                        //Check connection
                        if (connection.IsStopped)
                        {
                            this.Stop();
                            state = ClientState.Disconnected;
                            break;
                        }

                        //Get data from connection
                        UpdateAndInterpretData();

                        break;
                    }
            }
        }

        private void UpdateAndInterpretData()
        {
            //Read data from connection
            IEnumerable<byte> receivedBytes;
            while ((receivedBytes = connection.TakeData()) != null)
            {
                receiveBuffer.AddRange(receivedBytes);
            }

            if (receiveBuffer.Count > 0)
            {
                //Interpret data as a Packet
                Packet newPacket = null;
                try
                {
                    newPacket = Packet.ReadBuffer(ref receiveBuffer);
                }
                catch (InvalidPacketRead e)
                {
                    DebugLogger.GlobalDebug.LogNetworking("Client received invalid packet. (" + this.RunningID + ")\n" + e.ToString());
                }

                //Add packet to queue to be grabbed via GetPacket().
                if (newPacket != null)
                {
                    lock (packets_lock)
                    {
                        DebugLogger.GlobalDebug.LogNetworking("Received packet: " + newPacket.Type.ToString());
                        packets.Enqueue(newPacket);
                    }
                }
            }
        }

        protected override void Finish()
        {
            DebugLogger.GlobalDebug.LogNetworking("Client closed. (" + this.RunningID + ")");
            state = ClientState.Disconnected;
            if (!connection.IsStopped)
                connection.Stop();
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
        public String VerifyPassword
        {
            get
            {
                return verifyPassword;
            }
        }

        /// <summary>
        /// Dequeues the oldest received packet.
        /// </summary>
        /// <returns>The dequeued packet.</returns>
        public Packet GetPacket()
        {
            Packet toReturn = null;
            if (packets.Count > 0)
            {
                lock (packets_lock)
                {
                    if (packets.Count > 0)
                    {
                        toReturn = packets.Dequeue();
                    }
                }
            }
            return toReturn;
        }

        /// <summary>
        /// Sends a packet to the client if it holds a connection.
        /// </summary>
        /// <param name="p">The packet being sent.</param>
        public void SendPacket(Packet p)
        {
            if (state == ClientState.Connected)
                connection.SendData(p.CreateSendBuffer());
        }
    }

    enum ClientState
    {
        Disconnected,
        Varifying,
        Connected
    }
}
