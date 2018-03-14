using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Extant;
using Extant.Networking;

using SharedComponents.Global;

using InstanceServer.Control;
using SharedComponents.Global.Game;
using SharedComponents.Global.Game.Character;

namespace InstanceServer.World.Map.Character
{
    public partial class Characters
    {
        public class Player : GameCharacter
        {
            private NetConnection connection = null;
            private PlayerInfo info;
            private Int32 passwordToken;

            public Player(PlayerInfo info, CharacterVisualLayout visualLayout, Int32 passwordToken)
                : base("Player:" + info.Name, visualLayout)
            {
                this.info = info;
                this.passwordToken = passwordToken;

                Log.Log("Player created.");
            }

            public override string ToString()
            {
                return info.Name;
            }

            public override void Tick(float frameDiff)
            {
                base.Tick(frameDiff);

                
            }

            public override void Inform_AddCharacterInView(GameCharacter newChar)
            {
                this.SendPacket(new ClientToInstancePackets.Character_Add_c(newChar.Id, CharacterType.Player, info.Name));
                this.SendPacket(new ClientToInstancePackets.Character_Position_c(newChar.Id, newChar.Position.x, newChar.Position.y));
                this.SendPacket(new ClientToInstancePackets.Character_UpdateStats_c(newChar.Id, newChar.Stats));
            }

            public override void Inform_CharacterMovePoint(GameCharacter charFrom, MovePoint mp)
            {
                this.SendPacket(new ClientToInstancePackets.Character_Movement_c(charFrom.Id, mp));
            }

            public override void Inform_RemoveCharacterInView(GameCharacter oldChar)
            {
                this.SendPacket(new ClientToInstancePackets.Character_Remove_c(oldChar.Id));
            }

            public override void Inform_CharacterTeleport(GameCharacter charFrom, Position2D pos)
            {
                this.SendPacket(new ClientToInstancePackets.Character_Position_c(charFrom.Id, pos.x, pos.y));
            }

            /// <summary>
            /// Sets or replaces a client connect associated with this player.
            /// </summary>
            /// <param name="c">Client to set/replace.</param>
            public void SetClient(NetConnection con)
            {
                if (IsConnected)
                {
                    Log.Log("Player connected while already being connected.");
                    connection.Dispose();
                }

                connection = con;
            }

            public void DisconnectClient()
            {
                if (connection != null)
                    connection.Dispose();
            }

            public bool IsConnected
            {
                get
                {
                    return (connection != null) && (connection.State == NetConnection.NetworkState.Active);
                }
            }

            public void SendPacket(Packet p)
            {
                if (IsConnected)
                {
                    connection.SendPacket(p);
                }
            }

            public bool DistributePacket(IPacketDistributor distributor)
            {
                if (IsConnected)
                    return connection.DistributePacket(distributor);
                else
                    return false;
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