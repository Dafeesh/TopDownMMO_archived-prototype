//#define DEBUG_PACKETS

using System;
using System.Linq;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using Extant;
using Extant.Networking;

using SharedComponents.Global.GameProperties;

namespace SharedComponents.Global
{
    public static class ClientToInstancePackets
    {
        public enum PacketType
        {
            Null,

            Error_c,

            Map_Reset_c,
            Map_TerrainBlock_c,

            Verify_Details_i,
            Verify_Result_c,

            Player_Info_c,
            Player_SetControl_c,
            Player_MovementRequest_i,

            Character_Add_c,
            Character_Remove_c,
            Character_Position_c,
            Character_Movement_c,
            Character_UpdateStats_c
        }

        public class Distribution : IPacketDistributor
        {
            public Delegate_PacketDistribute<Error_c> out_Error_c = null;

            public Delegate_PacketDistribute<Map_Reset_c> out_Map_Reset_c = null;
            public Delegate_PacketDistribute<Map_TerrainBlock_c> out_Map_TerrainBlock_c = null;

            public Delegate_PacketDistribute<Verify_Details_i> out_Verify_Details_i = null;
            public Delegate_PacketDistribute<Verify_Result_c> out_Verify_Result_c = null;

            public Delegate_PacketDistribute<Player_Info_c> out_Player_Info_c = null;
            public Delegate_PacketDistribute<Player_SetControl_c> out_Player_SetControl_c = null;
            public Delegate_PacketDistribute<Player_MovementRequest_i> out_Player_MovementRequest_i = null;

            public Delegate_PacketDistribute<Character_Add_c> out_Character_Add_c = null;
            public Delegate_PacketDistribute<Character_Remove_c> out_Character_Remove_c = null;
            public Delegate_PacketDistribute<Character_Position_c> out_Character_Position_c = null;
            public Delegate_PacketDistribute<Character_Movement_c> out_Character_Movement_c = null;
            public Delegate_PacketDistribute<Character_UpdateStats_c> out_Character_UpdateStats_c = null;

            public void Dispose()
            {
                out_Error_c = null;

                out_Map_Reset_c = null;
                out_Map_TerrainBlock_c = null;

                out_Verify_Details_i = null;
                out_Verify_Result_c = null;

                out_Player_Info_c = null;
                out_Player_SetControl_c = null;
                out_Player_MovementRequest_i = null;

                out_Character_Add_c = null;
                out_Character_Remove_c = null;
                out_Character_Position_c = null;
                out_Character_Movement_c = null;
                out_Character_UpdateStats_c = null;
            }

            public bool DistributePacket(ref List<byte> buffer)
            {
                if (buffer != null &&
                    buffer.Count > sizeof(Int32))
                {
                    Byte[] backupBuffer = buffer.ToArray();
                    try
                    {
                        Int32 packetType = Packet.TakeInt32(ref buffer);

                        switch ((PacketType)packetType)
                        {
                            case (PacketType.Error_c):
                                if (out_Error_c != null)
                                    out_Error_c(Error_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Map_Reset_c):
                                if (out_Map_Reset_c != null)
                                    out_Map_Reset_c(Map_Reset_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Map_TerrainBlock_c):
                                if (out_Map_TerrainBlock_c != null)
                                    out_Map_TerrainBlock_c(Map_TerrainBlock_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Verify_Details_i):
                                if (out_Verify_Details_i != null)
                                    out_Verify_Details_i(Verify_Details_i.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Verify_Result_c):
                                if (out_Verify_Result_c != null)
                                    out_Verify_Result_c(Verify_Result_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Player_Info_c):
                                if (out_Player_Info_c != null)
                                    out_Player_Info_c(Player_Info_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Player_SetControl_c):
                                if (out_Player_SetControl_c != null)
                                    out_Player_SetControl_c(Player_SetControl_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Player_MovementRequest_i):
                                if (out_Player_MovementRequest_i != null)
                                    out_Player_MovementRequest_i(Player_MovementRequest_i.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Character_Add_c):
                                if (out_Character_Add_c != null)
                                    out_Character_Add_c(Character_Add_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Character_Remove_c):
                                if (out_Character_Remove_c != null)
                                    out_Character_Remove_c(Character_Remove_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Character_Movement_c):
                                if (out_Character_Movement_c != null)
                                    out_Character_Movement_c(Character_Movement_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Character_Position_c):
                                if (out_Character_Position_c != null)
                                    out_Character_Position_c(Character_Position_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Character_UpdateStats_c):
                                if (out_Character_UpdateStats_c != null)
                                    out_Character_UpdateStats_c(Character_UpdateStats_c.ReadPacket(ref buffer));
                                break;

                            default:
                                DebugLogger.Global.Log("Invalid packet header: " + packetType + "/" + ((PacketType)packetType).ToString());
                                throw new Packet.InvalidPacketRead("Invalid packet header: " + ((PacketType)packetType).ToString());
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                            return true;
                        else
                            throw new Packet.InvalidPacketRead("Last byte of packet was not END_PACKET byte.");
                    }
                    catch (ArgumentOutOfRangeException)// e) //Not enough data yet to make a full packet.
                    {
                        //DebugLogger.Global.Log("Packet not large enough yet." + e.ToString());
                        buffer = backupBuffer.ToList();
                        return false;
                    }
                }
                else
                    return false;
            }
        }

        /// <summary>
        /// Inform the client of an error.
        /// </summary>
        public class Error_c : Packet
        {
            public enum ErrorCode
            {
                InvalidPacket
            }

            public ErrorCode error;

            public Error_c(ErrorCode error)
                : base((Int32)PacketType.Error_c)
            {
                this.error = error;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)error));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Error_c ReadPacket(ref List<byte> buffer)
            {
                ErrorCode error = (ErrorCode)TakeInt32(ref buffer);

                return new Error_c(error);
            }
        }

        /// <summary>
        /// Inform the client to reset the map.
        /// </summary>
        public class Map_Reset_c : Packet
        {
            public int newNumBlocksX;
            public int newNumBlocksY;

            public Map_Reset_c(int newNumBlocksX, int newNumBlocksY)
                : base((Int32)PacketType.Map_Reset_c)
            {
                this.newNumBlocksX = newNumBlocksX;
                this.newNumBlocksY = newNumBlocksY;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(newNumBlocksX));
                    buffer.AddRange(GetBytes_Int32(newNumBlocksY));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Map_Reset_c ReadPacket(ref List<byte> buffer)
            {
                Int32 numBlocksX = TakeInt32(ref buffer);
                Int32 numBlocksY = TakeInt32(ref buffer);

                return new Map_Reset_c(numBlocksX, numBlocksY);
            }
        }

        /// <summary>
        /// A piece of the terrain for the map the player is in.
        /// </summary>
        public class Map_TerrainBlock_c : Packet
        {
            public int blockX;
            public int blockY;
            public Single[,] heightMap;

            public Map_TerrainBlock_c(int blockX, int blockY, Single[,] heightMap)
                : base((Int32)PacketType.Map_TerrainBlock_c)
            {
                this.blockX = blockX;
                this.blockY = blockY;
                this.heightMap = heightMap;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(blockX));
                    buffer.AddRange(GetBytes_Int32(blockY));
                    for (int i = 0; i < MapDefaults.TERRAINBLOCK_WIDTH; i++)
                    {
                        for (int j = 0; j < MapDefaults.TERRAINBLOCK_WIDTH; j++)
                        {
                            buffer.AddRange(GetBytes_Single(heightMap[i, j]));
                        }
                    }
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Map_TerrainBlock_c ReadPacket(ref List<byte> buffer)
            {
                int blockX = TakeInt32(ref buffer);
                int blockY = TakeInt32(ref buffer);

                Single[,] heightMap = new Single[MapDefaults.TERRAINBLOCK_WIDTH, MapDefaults.TERRAINBLOCK_WIDTH];
                for (int i = 0; i < MapDefaults.TERRAINBLOCK_WIDTH; i++)
                {
                    for (int j = 0; j < MapDefaults.TERRAINBLOCK_WIDTH; j++)
                    {
                        //DebugLogger.Global.Log("[" + i + "," + j + "]");
                        heightMap[i, j] = TakeSingle(ref buffer);
                    }
                }

                return new Map_TerrainBlock_c(blockX, blockY, heightMap);
            }
        }


        /// <summary>
        /// Informs the player which character it is controlling.
        /// </summary>
        public class Player_SetControl_c : Packet
        {
            public Int32 charId;

            public Player_SetControl_c(Int32 charId)
                : base((Int32)PacketType.Player_SetControl_c)
            {
                this.charId = charId;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Player_SetControl_c ReadPacket(ref List<byte> buffer)
            {
                Int32 id = TakeInt32(ref buffer);

                return new Player_SetControl_c(id);
            }
        }

        /// <summary>
        /// Character is requesting to move.
        /// </summary>
        public class Player_MovementRequest_i : Packet
        {
            public Single posx, posy;

            public Player_MovementRequest_i(Single posx, Single posy)
                : base((Int32)PacketType.Player_MovementRequest_i)
            {
                this.posx = posx;
                this.posy = posy;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Single(posx));
                    buffer.AddRange(GetBytes_Single(posy));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Player_MovementRequest_i ReadPacket(ref List<byte> buffer)
            {
                Single posx = TakeSingle(ref buffer);
                Single posy = TakeSingle(ref buffer);

                return new Player_MovementRequest_i(posx, posy);
            }
        }

        /// <summary>
        /// Inform a player of a new character.
        /// </summary>
        public class Character_Add_c : Packet
        {
            public Int32 charId;
            public CharacterType charType;
            public string charName;

            public Character_Add_c(Int32 charId, CharacterType charType, string charName)
                : base((Int32)PacketType.Character_Add_c)
            {
                this.charId = charId;
                this.charType = charType;
                this.charName = charName;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)charType));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Int32((Int32)charType));
                    buffer.AddRange(GetBytes_String_Unicode(charName));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Character_Add_c ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                CharacterType type = (CharacterType)TakeInt32(ref buffer);
                string name = TakeString(ref buffer);

                return new Character_Add_c(charId, type, name);
            }
        }

        /// <summary>
        /// Inform a player to delete a character.
        /// </summary>
        public class Character_Remove_c : Packet
        {
            public Int32 charId;

            public Character_Remove_c(Int32 charId)
                : base((Int32)PacketType.Character_Remove_c)
            {
                this.charId = charId;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Character_Remove_c ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);

                return new Character_Remove_c(charId);
            }
        }

        /// <summary>
        /// Character has a change in movement.
        /// </summary>
        public class Character_Position_c : Packet
        {
            public Int32 charId;
            public Single newx, newy;

            public Character_Position_c(Int32 charId,
                                        Single newx, Single newy)
                : base((Int32)PacketType.Character_Position_c)
            {
                this.charId = charId;
                this.newx = newx;
                this.newy = newy;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Single(newx));
                    buffer.AddRange(GetBytes_Single(newy));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Character_Position_c ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Single newx = TakeSingle(ref buffer);
                Single newy = TakeSingle(ref buffer);

                return new Character_Position_c(charId,
                                                newx, newy);
            }
        }

        /// <summary>
        /// Character has a change in movement.
        /// </summary>
        public class Character_Movement_c : Packet
        {
            public Int32 charId;
            public MovePoint movePoint;

            public Character_Movement_c(Int32 charId,
                                        MovePoint movePoint)
                : base((Int32)PacketType.Character_Movement_c)
            {
                this.charId = charId;
                this.movePoint = movePoint;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Single(movePoint.start.x));
                    buffer.AddRange(GetBytes_Single(movePoint.start.y));
                    buffer.AddRange(GetBytes_Single(movePoint.end.x));
                    buffer.AddRange(GetBytes_Single(movePoint.end.y));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Character_Movement_c ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Single startx = TakeSingle(ref buffer);
                Single starty = TakeSingle(ref buffer);
                Single newx = TakeSingle(ref buffer);
                Single newy = TakeSingle(ref buffer);

                return new Character_Movement_c(charId, new MovePoint(new Position2D(startx, starty), new Position2D(newx, newy)));
            }
        }

        /// <summary>
        /// Character has a change in stats.
        /// </summary>
        public class Character_UpdateStats_c : Packet
        {
            public Int32 charId;
            public CharacterStats stats;

            public Character_UpdateStats_c(Int32 charId,
                                           CharacterStats stats)
                : base((Int32)PacketType.Character_UpdateStats_c)
            {
                this.charId = charId;
                this.stats = stats;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Single(stats.MoveSpeed));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Character_UpdateStats_c ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Single moveSpeed = TakeSingle(ref buffer);

                return new Character_UpdateStats_c(charId, new CharacterStats() { MoveSpeed = moveSpeed });
            }
        }

        /// <summary>
        /// Updates the player's info.
        /// </summary>
        public class Player_Info_c : Packet
        {
            public String username;
            public Int32 level;

            public Player_Info_c(String username, Int32 level)
                : base((Int32)PacketType.Player_Info_c)
            {
                this.username = username;
                this.level = level;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(username));
                    buffer.AddRange(GetBytes_Int32(level));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Player_Info_c ReadPacket(ref List<byte> buffer)
            {
                String username = TakeString(ref buffer);
                Int32 level = TakeInt32(ref buffer);

                return new Player_Info_c(username, level);
            }
        }

        /// <summary>
        /// Client's attempt to verify credentials.
        /// </summary>
        public class Verify_Details_i : Packet
        {
            public Int32 build;
            public String username;
            public Int32 password;

            public Verify_Details_i(Int32 build, String username, Int32 password)
                : base((Int32)PacketType.Verify_Details_i)
            {
                this.build = build;
                this.username = username;
                this.password = password;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(build));
                    buffer.AddRange(GetBytes_String_Unicode(username));
                    buffer.AddRange(GetBytes_Int32(password));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Verify_Details_i ReadPacket(ref List<byte> buffer)
            {
                Int32 build = TakeInt32(ref buffer);
                String username = TakeString(ref buffer);
                Int32 password = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Verify_Details_g - " + username + "/" + password + "/" + build);
#endif

                return new Verify_Details_i(build, username, password);
            }
        }

        /// <summary>
        /// GameServer's response to a verification attempt.
        /// </summary>
        public class Verify_Result_c : Packet
        {
            public enum VerifyReturnCode
            {
                DoesNotExist,
                IncorrectPassword,
                IncorrectVersion,
                AlreadyConnected,
                Success
            }

            public VerifyReturnCode returnCode;

            public Verify_Result_c(VerifyReturnCode errorCode)
                : base((Int32)PacketType.Verify_Result_c)
            {
                this.returnCode = errorCode;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)returnCode));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Verify_Result_c ReadPacket(ref List<byte> buffer)
            {
                VerifyReturnCode errorCode = (VerifyReturnCode)TakeInt32(ref buffer);

                return new Verify_Result_c(errorCode);
            }
        }
    }
}