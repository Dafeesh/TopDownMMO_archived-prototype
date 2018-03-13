using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer.Networking;
using GameServer.Shared;
using Extant;

namespace GameServer.HostGame
{
    public class Player
    {
        private Client client;
        private Boolean newlyConnected;

        private Int32 modelNumber;
        private String username;
        private Int32 password;
        private String clan;
        private Int32 level;

        private TeamColor teamColor;

        public Player(String name, Int32 password, String clan, Int32 level, Int32 modelNumber, TeamColor teamColor)
        {
            this.username = name;
            this.password = password;
            this.clan = clan;
            this.level = level;
            this.modelNumber = modelNumber;
            this.teamColor = teamColor;
        }

        /// <summary>
        /// Sets the client for the player.
        /// </summary>
        /// <param name="c">The client to be assigned.</param>
        public void SetClient(Client c)
        {
            if (client != null)
            {
                DebugLogger.GlobalDebug.LogCatch("Player connected while already being connected: " + this.username);
                client.Stop();
            }
            client = c;
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
        /// Set and gets the value for if this is a newly connected player that needs game data.
        /// </summary>
        public Boolean IsNewlyConnected
        {
            set
            {
                newlyConnected = value;
            }
            get
            {
                return newlyConnected;
            }
        }

        public Int32 ModelNumber
        {
            get
            {
                return modelNumber;
            }
        }
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
        public String Clan
        {
            get
            {
                return clan;
            }
        }
        public Int32 Level
        {
            get
            {
                return level;
            }
        }
        public TeamColor TeamColor
        {
            get
            {
                return teamColor;
            }
        }
    }
}
