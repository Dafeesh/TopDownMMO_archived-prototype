#define DEBUG_PACKETS

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Net.Sockets;

using Extant;
using Extant.Networking;

namespace SharedComponents
{
    public static class ClientToMainPackets
    {
        public enum PacketType
        {
            Null,

            LoginAttempt_m,
            LoginResult_c,

            Servers_add_c,
            
            //SelectServer_m,
            //SelectServerResult_c
        }

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
                            case (PacketType.LoginAttempt_m):
                                returnPacket = LoginAttempt_m.ReadPacket(ref buffer);
                                break;

                            case (PacketType.LoginResult_c):
                                returnPacket = LoginResult_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.Servers_add_c):
                                returnPacket = Servers_add_c.ReadPacket(ref buffer);
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

        public class LoginAttempt_m : Packet
        {
            public Int32 build;
            public String username;
            public String password;

            public LoginAttempt_m(Int32 build,
                                  String username,
                                  String password)
                : base((Int32)PacketType.LoginAttempt_m, ProtocolType.Tcp)
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
                    buffer.AddRange(GetBytes_String_Unicode(password, STRBYTELENGTH_25));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 build = TakeInt32(ref buffer);
                String username = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                String password = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_25));

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: LoginAttempt_m");
#endif

                return new LoginAttempt_m(build, username, password);
            }
        }

        public class LoginResult_c : Packet
        {
            public LoginResult result;

            public enum LoginResult
            {
                InvalidLogin,
                Success
            }

            public LoginResult_c(LoginResult result)
                : base((Int32)PacketType.LoginResult_c, ProtocolType.Tcp)
            {
                this.result = result;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)result));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 result = TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: LoginResult_c");
#endif

                return new LoginResult_c((LoginResult)result);
            }
        }

        public class Servers_add_c : Packet
        {
            public Int32 number;
            public String name;
            public String ip;
            public ServerPopulationState popState;

            public enum ServerPopulationState
            {
                Low,
                Medium,
                High,
                Full
            }

            public Servers_add_c(Int32 number,
                                 String name,
                                 String ip,
                                 ServerPopulationState popState)
                : base((Int32)PacketType.Servers_add_c, ProtocolType.Tcp)
            {
                this.number = number;
                this.name = name;
                this.ip = ip;
                this.popState = popState;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(number));
                    buffer.AddRange(GetBytes_String_Unicode(name, STRBYTELENGTH_12));
                    buffer.AddRange(GetBytes_String_Unicode(ip, STRBYTELENGTH_25));
                    buffer.AddRange(GetBytes_Int32((Int32)popState));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                Int32 number = TakeInt32(ref buffer);
                String name = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                String ip = new String(TakeUnicodeChars(ref buffer, STRBYTELENGTH_12));
                ServerPopulationState popState = (ServerPopulationState)TakeInt32(ref buffer);

#if DEBUG_PACKETS
                DebugLogger.Global.Log("Packet In: Servers_add_c");
#endif

                return new Servers_add_c(number, name, ip, popState);
            }
        }
    }
}
