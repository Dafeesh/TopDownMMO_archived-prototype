using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using WorldServer.Networking;

using SharedComponents.GameProperties;

namespace WorldServer.World
{
    public class Player
    {
        private ClientConnection client;

        private String username;
        private Int32 password;
        private bool newlyConnected;

        public Player(PlayerInfo info, ClientConnection con)
        {
            this.username = info.Username;
            this.password = info.Password;

            SetClient(con);
            DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Player joined: " + this.username);
        }

        /// <summary>
        /// Sets the client for the player.
        /// </summary>
        /// <param name="c">The client to be assigned.</param>
        public void SetClient(ClientConnection c)
        {
            if (client != null)
            {
                if (client.IsConnectedAndVerified)
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Player connected while already being connected: " + this.username);
                else
                    DebugLogger.GlobalDebug.Log(DebugLogger.LogType.Networking, "Player reconnected: " + this.username);
                client.Stop();
            }
            client = c;
            newlyConnected = true;
        }

        /// <summary>
        /// Disconnects the client connected to this player.
        /// </summary>
        public void Disconnect()
        {
            if (client != null)
            {
                client.Stop();
                client = null;
            }
        }

        /** TODO: Should process commands, not forward Packets.
         * 
        /// <summary>
        /// Returns the oldest packet received if there is an active connection. Else, it returns null.
        /// </summary>
        /// <returns>The oldest packet received.</returns>
        public Packet GetPacket()
        {
            if (client != null)
            {
                return client.GetPacket();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Sends a packet to the Client if there is an active connection.
        /// </summary>
        /// <param name="p">The packet to be sent.</param>
        public void SendPacket(Packet p)
        {
            if (client != null)
            {
                client.SendPacket(p);
            }
        }

        /// <summary>
        /// Gets the value for if this is a newly connected player that needs game data.
        /// </summary>
        public Boolean IsNewlyConnected
        {
            get
            {
                return newlyConnected;
            }
        }
         * */

        public String Username
        {
            get
            {
                return username;
            }
        }

        public Int32 Password
        {
            get
            {
                return password;
            }
        }
    }
}
