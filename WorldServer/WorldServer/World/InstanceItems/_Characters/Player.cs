using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;

using WorldServer.Networking;

namespace WorldServer.World.InstanceItems
{
    public partial class Characters
    {
        public class Player : Character
        {
            private ClientConnection client;
            private String username;
            private Int32 password;
            private bool newlyConnected = false;
            private bool loggingOut = false;

            private PlayerZoneLocation location = new PlayerZoneLocation();

            public Player(Info info)
                : base()
            {
                this.username = info.Username;
                this.password = info.Password;

                client = null;

                DebugLogger.Global.Log("Player created: " + username);
            }

            protected override void Dispose(bool blocking)
            {
                client.Dispose();
            }

            public override void Tick()
            {
                if (client != null)
                    if (client.IsStopped)
                    {
                        DebugLogger.Global.Log("Player disconnected: " + username);
                        client.Dispose();
                        client = null;
                        loggingOut = true;
                    }
            }

            public override void Inform_AddCharacterInView(Character newChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Add_c(newChar.Id, CharacterType.Neutral, 0));
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(newChar.Id, newChar.Position.x, newChar.Position.y));
            }

            public override void Inform_RemoveCharacterInView(Character oldChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Remove_c(oldChar.Id));
            }

            public override void Inform_CharacterTeleport(Character charFrom, Position2D pos)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(charFrom.Id, pos.x, pos.y));
            }

            public override void Inform_MoveToMap(Map m)
            {
                this.SendPacket(new ClientToWorldPackets.Map_MoveTo_c((int)m.Id));
            }

            /// <summary>
            /// Sets or replaces a client connect associated with this player.
            /// </summary>
            /// <param name="c">Client to set/replace.</param>
            public void SetClient(ClientConnection c)
            {
                if (client != null)
                {
                    if (client.IsConnectedAndVerified)
                        DebugLogger.Global.Log("Player connected while already being connected: " + this.username);
                    else
                        DebugLogger.Global.Log("Player reconnected: " + this.username);
                    client.Dispose();
                }
                client = c;
                newlyConnected = true;
            }

            /// <summary>
            /// Sends a packet to the player if a connection exists.
            /// </summary>
            /// <param name="p"></param>
            public void SendPacket(Packet p)
            {
                if (client != null)
                {
                    client.SendPacket(p);
                }
            }

            public bool IsLoggingOut
            {
                get
                {
                    return loggingOut;
                }
            }

            public bool IsNewlyConnected
            {
                get
                {
                    return newlyConnected;
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

            public PlayerZoneLocation ZoneLocation
            {
                get
                {
                    return location;
                }
            }

            public class PlayerZoneLocation
            {
                public Instances.Zone.ZoneIDs Zone { set; get; }
                public Position2D Position { set; get; }
            }

            public class Info
            {
                public Info(String username, Int32 password, PlayerZoneLocation location)
                {
                    this.Username = username;
                    this.Password = password;
                    this.Location = location;
                }

                public readonly string Username;
                public readonly int Password;
                public readonly PlayerZoneLocation Location;
            }
        }
    }
}