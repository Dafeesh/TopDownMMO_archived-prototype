#define DEBUG_PACKETS

using System;
using System.Linq;
using System.Net.Sockets;
using System.Collections;
using System.Collections.Generic;

using Extant.Networking;

namespace Extant.GameServerShared
{
    public static class GameServerPackets
    {
        public enum PacketType
        {
            Null,

            Player_Add_c,
            Player_Remove_c,
            Player_SetControl_c,
            Player_Movement_g,
            Player_Score_c,
            Player_Currency_c,
            Game_Info_c,
            Game_DayPhase_c,
            Verify_Details_g,
            Verify_Result_c,

            Character_Add_c,
            Character_Remove_c,
            Character_Attributes_c,

            Character_Visibility_c,
            Character_Movement_c,
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
                            case (PacketType.Player_Add_c):
                                returnPacket = Player_Add_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_Remove_c):
                                returnPacket = Player_Remove_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Visibility_c):
                                returnPacket = Character_Visibility_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Character_Movement_c):
                                returnPacket = Character_Movement_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_Movement_g):
                                returnPacket = Player_Movement_g.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_Score_c):
                                returnPacket = Player_Score_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Player_Currency_c):
                                returnPacket = Player_Currency_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Game_Info_c):
                                returnPacket = Game_Info_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Game_DayPhase_c):
                                returnPacket = Game_DayPhase_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Verify_Details_g):
                                returnPacket = Verify_Details_g.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Verify_Result_c):
                                returnPacket = Verify_Result_c.ReadPacket(ref buffer);
                                break;

                            default:
                                DebugLogger.GlobalDebug.LogNetworking("Invalid packet header: " + packetType + "/" + ((PacketType)packetType).ToString());
                                throw new Packet.InvalidPacketRead();
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                            return returnPacket;
                        else
                            throw new Packet.InvalidPacketRead();
                    }
                    catch (ArgumentOutOfRangeException e) //Not enough data yet to make a full packet.
                    {
                        DebugLogger.GlobalDebug.LogNetworking("Packet not large enough yet." + e.ToString());
                        buffer = backupBuffer.ToList();
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Player is being introduced as in game.
        /// </summary>
        public class Player_Add_c : Packet
        {
            public String name;
            public String clan;
            public Int32 level;
            public Int32 modelNumber;

            public Player_Add_c(String name,
                                String clan,
                                Int32 level,
                                Int32 modelNumber)
                : base((Int32)PacketType.Player_Add_c, ProtocolType.Tcp)
            {
                this.name = name;
                this.clan = clan;
                this.level = level;
                this.modelNumber = modelNumber;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(name, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_String_Unicode(clan, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_Int32(level));
                    buffer.AddRange(GetBytes_Int32(modelNumber));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                String name = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                String clan = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                Int32 level = TakeInt32(ref buffer);
                Int32 modelNumber = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_Add_c - " + name + "/" + clan + "/" + level + "/" + modelNumber);
#endif

                return new Player_Add_c(name, clan, level, modelNumber);
            }
        }

        /// <summary>
        /// Player has been removed from the game.
        /// </summary>
        public class Player_Remove_c : Packet
        {
            public Int32 id;

            public Player_Remove_c(Int32 id)
                : base((Int32)PacketType.Player_Remove_c, ProtocolType.Tcp)
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
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_Remove_c - " + id);
#endif

                return new Player_Remove_c(id);
            }
        }

        /// <summary>
        /// Informs the client which player it is controlling.
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
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_SetControl_c - " + id);
#endif

                return new Player_SetControl_c(id);
            }
        }

        /// <summary>
        /// Client's update for it's position.
        /// </summary>
        public class Player_Movement_g : Packet
        {
            public Int32 timeStamp;
            public Double pos_x, pos_y;
            public Double move_x, move_y;
            public Double target_x, target_y;

            public Player_Movement_g(Int32 timeStamp,
                                     Double pos_x, Double pos_y,
                                     Double move_x, Double move_y,
                                     Double target_x, Double target_y)
                : base((Int32)PacketType.Player_Movement_g, ProtocolType.Tcp)
            {
                this.timeStamp = timeStamp;

                this.pos_x = pos_x;
                this.pos_y = pos_y;

                this.move_x = move_x;
                this.move_y = move_y;

                this.target_x = target_x;
                this.target_y = target_y;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(timeStamp));
                    buffer.AddRange(GetBytes_Double(pos_x));
                    buffer.AddRange(GetBytes_Double(pos_y));
                    buffer.AddRange(GetBytes_Double(move_x));
                    buffer.AddRange(GetBytes_Double(move_y));
                    buffer.AddRange(GetBytes_Double(target_x));
                    buffer.AddRange(GetBytes_Double(target_y));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 timeStamp = TakeInt32(ref buffer);
                Double pos_x = TakeDouble(ref buffer);
                Double pos_y = TakeDouble(ref buffer);
                Double move_x = TakeDouble(ref buffer);
                Double move_y = TakeDouble(ref buffer);
                Double target_x = TakeDouble(ref buffer);
                Double target_y = TakeDouble(ref buffer);
                Byte visilibityMode = TakeByte(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_Movement_g - (" + pos_x + "," + pos_y + ")");
#endif

                return new Player_Movement_g(timeStamp,
                                             pos_x, pos_y,
                                             move_x, move_y,
                                             target_x, target_y);
            }
        }

        /// <summary>
        /// Update to a player's score and/or team.
        /// </summary>
        public class Player_Score_c : Packet
        {
            public String username;
            public TeamColor teamColor;
            public Int32 kill, death, assist;
            public Int32 score;

            public Player_Score_c(String username, TeamColor teamColor, Int32 kill, Int32 death, Int32 assist, Int32 score)
                : base((Int32)PacketType.Player_Score_c, ProtocolType.Tcp)
            {
                this.username = username;
                this.teamColor = teamColor;
                this.kill = kill;
                this.death = death;
                this.assist = assist;
                this.score = score;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(username, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_Int32((int)teamColor));
                    buffer.AddRange(GetBytes_Int32(kill));
                    buffer.AddRange(GetBytes_Int32(death));
                    buffer.AddRange(GetBytes_Int32(assist));
                    buffer.AddRange(GetBytes_Int32(score));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                String username = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                TeamColor teamColor = (TeamColor)TakeInt32(ref buffer);
                Int32 kill = TakeInt32(ref buffer);
                Int32 death = TakeInt32(ref buffer);
                Int32 assist = TakeInt32(ref buffer);
                Int32 score = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_Score_c - " + username + "/" + teamColor.ToString() + "/" + kill + "/" + death + "/" + assist + "/" + score);
#endif

                return new Player_Score_c(username, teamColor, kill, death, assist, score);
            }
        }

        /// <summary>
        /// Update to a player's currency.
        /// </summary>
        public class Player_Currency_c : Packet
        {
            public Int32 id;
            public Int32 currency;
            public Double rate;

            public Player_Currency_c(Int32 id, Int32 currency, Double rate)
                : base((Int32)PacketType.Player_Currency_c, ProtocolType.Tcp)
            {
                this.id = id;
                this.currency = currency;
                this.rate = rate;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(id));
                    buffer.AddRange(GetBytes_Int32(currency));
                    buffer.AddRange(GetBytes_Double(rate));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 id = TakeInt32(ref buffer);
                Int32 currency = TakeInt32(ref buffer);
                Double rate = TakeDouble(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Player_Currency_c - " + id + "/" + currency + "/" + rate.ToString("F2"));
#endif

                return new Player_Currency_c(id, currency, rate);
            }
        }

        /// <summary>
        /// Inform a player of a new character.
        /// </summary>
        public class Character_Add_c : Packet
        {
            public Int32 charId;
            public Int32 modelNumber;

            public Character_Add_c(Int32 charId, Int32 modelNumber)
                : base((Int32)PacketType.Character_Add_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.modelNumber = modelNumber;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Int32(modelNumber));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Int32 modelNumber = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Character_Add_c - " + charId + "/" + modelNumber);
#endif

                return new Character_Add_c(charId, modelNumber);
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
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Character_Remove_c - " + charId);
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
        /// Update in a character's attributes.
        /// </summary>
        public class Character_Attributes_c : Packet
        {
            public Int32 charId;
            public Int32 speed;

            public Character_Attributes_c(Int32 charId, Int32 speed)
                : base((Int32)PacketType.Character_Attributes_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.speed = speed;
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
                Int32 speed = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Character_Attributes_c - " + charId);
#endif

                return new Character_Attributes_c(charId, speed);
            }

            public enum VisibilityMode
            {
                None,
                Full,
                Stealth
            }
        }

        /// <summary>
        /// Player has a different visilibity of a character.
        /// </summary>
        public class Character_Visibility_c : Packet
        {
            public Int32 charId;
            public Byte visibilityMode;

            public Character_Visibility_c(Int32 charId, Byte visibilityMode)
                : base((Int32)PacketType.Character_Visibility_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.visibilityMode = visibilityMode;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Int32(visibilityMode));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Byte visilibityMode = TakeByte(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Character_Visibility_c - " + charId + "/" + visilibityMode);
#endif

                return new Character_Visibility_c(charId, visilibityMode);
            }

            public enum VisibilityMode
            {
                None,
                Full,
                Stealth
            }
        }

        /// <summary>
        /// Player has a change in movement.
        /// </summary>
        public class Character_Movement_c : Packet
        {
            public Int32 charId;
            public Double pos_x, pos_y;
            public Double move_x, move_y;
            public Double target_x, target_y;

            public Character_Movement_c(Int32 charId,
                                        Double pos_x, Double pos_y,
                                        Double move_x, Double move_y,
                                        Double target_x, Double target_y)
                : base((Int32)PacketType.Character_Movement_c, ProtocolType.Tcp)
            {
                this.charId = charId;
                this.pos_x = pos_x;
                this.pos_y = pos_y;

                this.move_x = move_x;
                this.move_y = move_y;

                this.target_x = target_x;
                this.target_y = target_y;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(charId));
                    buffer.AddRange(GetBytes_Double(pos_x));
                    buffer.AddRange(GetBytes_Double(pos_y));
                    buffer.AddRange(GetBytes_Double(move_x));
                    buffer.AddRange(GetBytes_Double(move_y));
                    buffer.AddRange(GetBytes_Double(target_x));
                    buffer.AddRange(GetBytes_Double(target_y));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 charId = TakeInt32(ref buffer);
                Double pos_x = TakeDouble(ref buffer);
                Double pos_y = TakeDouble(ref buffer);
                Double move_x = TakeDouble(ref buffer);
                Double move_y = TakeDouble(ref buffer);
                Double target_x = TakeDouble(ref buffer);
                Double target_y = TakeDouble(ref buffer);
                Byte visilibityMode = TakeByte(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Character_Movement_c - " + charId + "/(" + pos_x + "," + pos_y + ")");
#endif

                return new Character_Movement_c(charId,
                                             pos_x, pos_y,
                                             move_x, move_y,
                                             target_x, target_y);
            }
        }

        /// <summary>
        /// Information about the game.
        /// </summary>
        public class Game_Info_c : Packet
        {
            public Int32 mapPreset;
            public Int32 gameTime;

            public Game_Info_c(Int32 mapPreset, Int32 gameTime)
                : base((Int32)PacketType.Game_Info_c, ProtocolType.Tcp)
            {
                this.mapPreset = mapPreset;
                this.gameTime = gameTime;

            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(mapPreset));
                    buffer.AddRange(GetBytes_Int32(gameTime));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 mapPreset = TakeInt32(ref buffer);
                Int32 gameTime = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Game_Info_c - " + mapPreset + "/" + gameTime);
#endif

                return new Game_Info_c(mapPreset, gameTime);
            }
        }

        /// <summary>
        /// Change in DayPhase.
        /// </summary>
        public class Game_DayPhase_c : Packet
        {
            public Int32 dayPhase;

            public Game_DayPhase_c(Int32 dayPhase)
                : base((Int32)PacketType.Game_DayPhase_c, ProtocolType.Tcp)
            {
                this.dayPhase = dayPhase;

            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(dayPhase));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 dayPhase = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Game_DayPhase_c - " + dayPhase);
#endif

                return new Game_DayPhase_c(dayPhase);
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
                : base((Int32)PacketType.Verify_Details_g, ProtocolType.Tcp)
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
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Verify_Details_g - " + username + "/" + password + "/" + build);
#endif

                return new Verify_Details_g(build, username, password);
            }
        }

        /// <summary>
        /// GameServer's response to a verification attempt.
        /// </summary>
        public class Verify_Result_c : Packet
        {
            public VerifyReturnCode errorCode;

            public Verify_Result_c(VerifyReturnCode errorCode)
                : base((Int32)PacketType.Verify_Result_c, ProtocolType.Tcp)
            {
                this.errorCode = errorCode;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)errorCode));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                VerifyReturnCode errorCode = (VerifyReturnCode)TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.GlobalDebug.LogNetworking("Packet In: Verify_Result_c - " + errorCode.ToString());
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