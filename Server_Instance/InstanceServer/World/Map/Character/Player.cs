using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using SharedComponents.Global;
using SharedComponents.Global.GameProperties;

using InstanceServer.Control;

namespace InstanceServer.World.Map.Character
{
    public partial class Characters
    {
        public class Player : GameCharacter
        {
            private ClientAuthConnection client = null;
            private PlayerInfo info;
            private Int32 passwordToken;

            public Player(PlayerInfo info, CharacterLayout layout, Int32 passwordToken)
                : base("Player:" + info.Name, layout)
            {
                this.info = info;
                this.passwordToken = passwordToken;

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
                    }
                }
            }

            public override void Inform_AddCharacterInView(GameCharacter newChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Add_c(newChar.Id, newChar.Layout, CharacterType.Player));
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(newChar.Id, newChar.Position.x, newChar.Position.y));
                this.SendPacket(new ClientToWorldPackets.Character_UpdateStats_c(newChar.Id, newChar.Stats));
            }

            public override void Inform_CharacterMovePoint(GameCharacter charFrom, MovePoint mp)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Movement_c(charFrom.Id, mp));
            }

            public override void Inform_RemoveCharacterInView(GameCharacter oldChar)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Remove_c(oldChar.Id));
            }

            public override void Inform_CharacterTeleport(GameCharacter charFrom, Position2D pos)
            {
                this.SendPacket(new ClientToWorldPackets.Character_Position_c(charFrom.Id, pos.x, pos.y));
            }

            /// <summary>
            /// Sets or replaces a client connect associated with this player.
            /// </summary>
            /// <param name="c">Client to set/replace.</param>
            public void SetClient(ClientAuthConnection c)
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

            public PlayerInfo Info
            {
                get
                {
                    return info;
                }
            }

            public Int32 PasswordToken
            {
                get
                {
                    return passwordToken;
                }
            }
        }
    }
}