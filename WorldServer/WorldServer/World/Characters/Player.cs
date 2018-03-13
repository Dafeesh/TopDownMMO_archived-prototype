using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;
using SharedComponents;
using SharedComponents.GameProperties;

using WorldServer.Control;
using WorldServer.Networking;

namespace WorldServer.World
{
    public partial class Characters
    {
        public class Player : Character
        {
            private ClientConnection client;
            private Queue<Packet> receivePackets = new Queue<Packet>();
            private PlayerInfo info;
            private Int32 password;
            private bool newlyConnected = false;
            private bool loggingOut = false;

            private PlayerZoneLocation location;

            public Player(Template template)
                : base("Player:" + template.Info.Name, CharacterType.Player)
            {
                this.info = template.Info;
                this.password = template.Password;
                this.location = template.Location;

                client = null;

                Log.Log("Player created.");
            }

            protected override void Dispose(bool blocking)
            {
                if (client != null)
                    client.Dispose();
            }

            public override string ToString()
            {
                return info.Name;
            }

            public override void Tick(float frameDiff)
            {
                base.Tick(frameDiff);

                if (client != null)
                {
                    if (client.IsStopped)
                    {
                        Log.Log("Player disconnected.");
                        client.Dispose();
                        client = null;
                        loggingOut = true;
                    }
                }
            }

            public override void Inform_AddCharacterInView(Character newChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Add_c(newChar.Id, CharacterType.Player, 0));
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(newChar.Id, newChar.Position.x, newChar.Position.y));
                this.SendPacket(new ClientToWorldPackets.Character_UpdateStats_c(newChar.Id, newChar.Stats));
            }

            public override void Inform_CharacterMovePoint(Character charFrom, MovePoint mp)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Movement_c(charFrom.Id, mp));
            }

            public override void Inform_RemoveCharacterInView(Character oldChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Remove_c(oldChar.Id));
            }

            public override void Inform_CharacterTeleport(Character charFrom, Position2D pos)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(charFrom.Id, pos.x, pos.y));
            }

            /// <summary>
            /// Sets or replaces a client connect associated with this player.
            /// </summary>
            /// <param name="c">Client to set/replace.</param>
            public void SetClient(ClientConnection c)
            {
                if (client != null)
                {
                    if (client.IsAlive)
                        Log.Log("Player connected while already being connected.");
                    else
                        Log.Log("Player reconnected.");
                    client.Dispose();
                }
                client = c;
                newlyConnected = true;
            }

            public void SendPacket(Packet p)
            {
                if (client != null)
                {
                    client.SendPacket(p);
                }
            }

            public Packet GetPacket()
            {
                if (client == null)
                    return null;
                else
                    return client.GetPacket();
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

            public PlayerInfo Info
            {
                get
                {
                    return info;
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

            public class Template
            {
                public Template(PlayerInfo info, Int32 password, PlayerZoneLocation location)
                {
                    this.Info = info;
                    this.Password = password;
                    this.Location = location;
                }

                public readonly PlayerInfo Info;
                public readonly int Password;
                public readonly PlayerZoneLocation Location;
            }
        }
    }
}