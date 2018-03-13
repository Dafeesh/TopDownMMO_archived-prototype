using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using GameServer.Networking;
using GameServer.Shared;

namespace GameServer.HostGame
{
    public class Player
    {
        private Client client;
        private Boolean newlyConnected;

        private String username;
        private String password;
        private String clan;
        private Int32 level;

        private const Int32 TEAMNUM_DEFAULT = 0;
        private Int32 teamNum;

        public Player(String name, String password, String clan, Int32 level, Int32 teamNum = TEAMNUM_DEFAULT)
        {
            this.username = name;
            this.password = password;
            this.clan = clan;
            this.level = level;
            this.teamNum = teamNum;
        }

        /// <summary>
        /// Sets the client for the player.
        /// </summary>
        /// <param name="c">The client to be assigned.</param>
        public void SetClient(Client c)
        {
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
        public Boolean NewlyConnected
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

        /// <summary>
        /// Gets the username of the player.
        /// </summary>
        public String Username
        {
            get
            {
                return username;
            }
        }

        /// <summary>
        /// Gets the password of the player.
        /// </summary>
        public String Password
        {
            get
            {
                return password;
            }
        }
    }
}
