//#define DEBUG_PACKETS

using System;
using System.Linq;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using Extant;
using Extant.Networking;

using SharedComponents.GameProperties;

namespace SharedComponents
{
    public static class ClientToWorldPackets
    {
        public enum PacketType
        {
            Null,

            Ping_c,

            Map_MoveTo_c,

            Verify_Details_w,
            Verify_Result_c,

            Player_Info_c,
            Player_SetControl_c,

            Character_Add_c,
            Character_Remove_c,
            Character_Position_c,
            Character_Movement_c
        }

        /// <summary>
        /// Returns a Packet that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buffer">Buffer used to try to create a packet.</param>
        public static Packet ReadBuffer(ref List<Byte> buffer)
        {
            if (buffer != null)
            {
                if (buffer.Count > sizeof(Int32))
                {
                    Byte[] backupBuffer = buffer.ToArray();
                    try
                    {
                        Packet returnPacket = null;
                        Int32 packetType = Packet.TakeInt32(ref buffer);

                        switch ((PacketType)packetType)
                        {
                            case (PacketType.Ping_c):
                                returnPacket = Ping_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Map_MoveTo_c):
                                returnPacket = Map_MoveTo_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Verify_Details_w):
                                returnPacket = Verify_Details_g.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Verify_Result_c):
                                returnPacket = Verify_Result_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_Info_c):
                                returnPacket = Player_Info_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_SetControl_c):
                                returnPacket = Player_SetControl_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Add_c):
                                returnPacket = Character_Add_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Remove_c):
                                returnPacket = Character_Remove_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Movement_c):
                                returnPacket = Character_Movement_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Position_c):
                                returnPacket = Character_Position_c.ReadPacket(ref buffer);
                                break;

                            default:
                                DebugLogger.Global.Log("Invalid packet header: " + packetType + "/" + ((PacketType)packetType).ToString());
                                throw new Packet.InvalidPacketRead();
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                            return returnPacket;
                        else
                            throw new Packet.InvalidPacketRead();
                    }
                    catch (ArgumentOutOfRangeException e) //Not enough data yet to make a full packet.
                    {
                        DebugLogger.Global.Log("Packet not large enough yet." + e.ToString());
                        buffer = backupBuffer.ToList();
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Ping to the client.
        /// </summary>
        public class Ping_c : Packet
        {
            public Ping_c()
                : base((Int32)PacketType.Ping_c, ProtocolType.Tcp)
            {
                
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Ping_c");
#endif

                return new Ping_c();
            }
        }
        
        /// <summary>
        /// Inform client of a change in the map.
        /// </summary>
        public class Map_MoveTo_c : Packet
        {
            public int mapNum;

            public Map_MoveTo_c(int mapNum)
                : base((Int32)PacketType.Map_MoveTo_c, ProtocolType.Tcp)
            {
                this.mapNum = mapNum;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(mapNum));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 mn = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Ping_c");
#endif

                return new Map_MoveTo_c(mn);
            }
        }

        /// <summary>
        /// Informs the player which character it is controlling.
        /// </summary>
        public class Player_SetControl_c : Packet
        {
            public Int32 id;

            public Player_SetControl_c(Int32 id)
                : base((Int32)PacketType.Player_SetControl_c, ProtocolType.Tcp)
            {
                this.id = id;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(id));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 id = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Player_SetControl_c - " + id);
#endif

                return new Player_SetControl_c(id);
            }
        }

        /// <summary>
        /// Inform a player of a new character.
        /// </summary>
        public class Character_Add_c : Packet
        {
            public Int32 charId;
            public CharacterType charType;
            public Int32 modelNumber;

            public Character_Add_c(Int32 charId, CharacterType charType, Int32 modelNumber)
                : base((Int32)PacketType.Character_Add_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.charType = charType;
                this.modelNumber = modelNumber;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Int32((Int32)charType));
                    buffer.AddRange(GetBytes_Int32(modelNumber));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                CharacterType charType = (CharacterType)TakeInt32(ref buffer);
                Int32 modelNumber = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Character_Add_c - " + charId + " - " + charType.ToString() + " / " + modelNumber);
#endif

                return new Character_Add_c(charId, charType, modelNumber);
            }
        }

        /// <summary>
        /// Inform a player to delete a character.
        /// </summary>
        public class Character_Remove_c : Packet
        {
            public Int32 charId;

            public Character_Remove_c(Int32 charId)
                : base((Int32)PacketType.Character_Remove_c, ProtocolType.Tcp)
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

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Character_Remove_c - " + charId);
#endif

                return new Character_Remove_c(charId);
            }

            public enum VisibilityMode
            {
                None,
                Full,
                Stealth
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
                : base((Int32)PacketType.Character_Position_c, ProtocolType.Tcp)
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

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Single newx = TakeSingle(ref buffer);
                Single newy = TakeSingle(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Character_Position_c - " + charId + ": (" + newx + "," + newy + ")");
#endif

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
            public Double x, y;
            public Double newx, newy;
            public Double speed;

            public Character_Movement_c(Int32 charId,
                                        Double x, Double y,
                                        Double newx, Double newy,
                                        Double speed)
                : base((Int32)PacketType.Character_Movement_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.x = x;
                this.y = y;
                this.newx = newx;
                this.newy = newy;
                this.speed = speed;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Double(x));
                    buffer.AddRange(GetBytes_Double(y));
                    buffer.AddRange(GetBytes_Double(newx));
                    buffer.AddRange(GetBytes_Double(newy));
                    buffer.AddRange(GetBytes_Double(speed));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Double x = TakeDouble(ref buffer);
                Double y = TakeDouble(ref buffer);
                Double newx = TakeDouble(ref buffer);
                Double newy = TakeDouble(ref buffer);
                Double speed = TakeDouble(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Character_Movement_c - " + charId + ": (" + x + "," + y + ") -> (" + newx + "," + newy + ")");
#endif

                return new Character_Movement_c(charId,
                                                x, y,
                                                newx, newy,
                                                speed);
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
                : base((Int32)PacketType.Player_Info_c, ProtocolType.Tcp)
            {
                this.username = username;
                this.level = level;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(username, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_Int32(level));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                String username = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                Int32 level = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Player_Info_c - ");
#endif

                return new Player_Info_c(username, level);
            }
        }

        /// <summary>
        /// Client's attempt to verify credentials.
        /// </summary>
        public class Verify_Details_g : Packet
        {
            public Int32 build;
            public String username;
            public Int32 password;

            public Verify_Details_g(Int32 build, String username, Int32 password)
                : base((Int32)PacketType.Verify_Details_w, ProtocolType.Tcp)
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
                    buffer.AddRange(GetBytes_String_Unicode(username, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_Int32(password));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 build = TakeInt32(ref buffer);
                String username = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                Int32 password = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Verify_Details_g - " + username + "/" + password + "/" + build);
#endif

                return new Verify_Details_g(build, username, password);
            }
        }

        /// <summary>
        /// GameServer's response to a verification attempt.
        /// </summary>
        public class Verify_Result_c : Packet
        {
            public VerifyReturnCode returnCode;

            public Verify_Result_c(VerifyReturnCode errorCode)
                : base((Int32)PacketType.Verify_Result_c, ProtocolType.Tcp)
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

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                VerifyReturnCode errorCode = (VerifyReturnCode)TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Verify_Result_c - " + errorCode.ToString());
#endif

                return new Verify_Result_c(errorCode);
            }

            public enum VerifyReturnCode
            {
                DoesNotExist,
                IncorrectPassword,
                IncorrectVersion,
                AlreadyConnected,
                Success
            }
        }
    }
}