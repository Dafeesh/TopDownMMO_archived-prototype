using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extant;
using Extant.Networking;

using SharedComponents.Global.Game;
using System.Net;
using SharedComponents.Global.Game.Character;

namespace SharedComponents.Global
{
    public static class ClientToMasterPackets
    {
        public enum PacketType
        {
            Null,

            ErrorCode_c,

            AccountAuthorize_Attempt_m,
            AccountAuthorize_Response_c,

            Menu_CharacterListItem_c,
            Menu_CharacterListItem_Select_m,

            Connection_WorldServerInfo_c
        }

        public class Distribution : IPacketDistributor
        {
            public Delegate_PacketDistribute<ErrorCode_c> out_ErrorCode_c = null;

            public Delegate_PacketDistribute<AccountAuthorize_Attempt_m> out_AccountAuthorize_Attempt_m = null;
            public Delegate_PacketDistribute<AccountAuthorize_Response_c> out_AccountAuthorize_Response_c = null;

            public Delegate_PacketDistribute<Menu_CharacterListItem_c> out_Menu_CharacterListItem_c = null;
            public Delegate_PacketDistribute<Menu_CharacterListItem_Select_m> out_Menu_CharacterListItem_Select_m = null;

            public Delegate_PacketDistribute<Connection_WorldServerInfo_c> out_Connection_WorldServerInfo_c = null;

            public void Dispose()
            {
                out_ErrorCode_c = null;

                out_AccountAuthorize_Attempt_m = null;
                out_AccountAuthorize_Response_c = null;

                out_Menu_CharacterListItem_c = null;
                out_Menu_CharacterListItem_Select_m = null;

                out_Connection_WorldServerInfo_c = null;
            }

            /// <returns>If a packet was distributed.</returns>
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
                            case (PacketType.ErrorCode_c):
                                if (out_ErrorCode_c != null)
                                    out_ErrorCode_c(ErrorCode_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.AccountAuthorize_Attempt_m):
                                if (out_AccountAuthorize_Attempt_m != null)
                                    out_AccountAuthorize_Attempt_m(AccountAuthorize_Attempt_m.ReadPacket(ref buffer));
                                break;

                            case (PacketType.AccountAuthorize_Response_c):
                                if (out_AccountAuthorize_Response_c != null)
                                    out_AccountAuthorize_Response_c(AccountAuthorize_Response_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Menu_CharacterListItem_c):
                                if (out_Menu_CharacterListItem_c != null)
                                    out_Menu_CharacterListItem_c(Menu_CharacterListItem_c.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Menu_CharacterListItem_Select_m):
                                if (out_Menu_CharacterListItem_Select_m != null)
                                    out_Menu_CharacterListItem_Select_m(Menu_CharacterListItem_Select_m.ReadPacket(ref buffer));
                                break;

                            case (PacketType.Connection_WorldServerInfo_c):
                                if (out_Connection_WorldServerInfo_c != null)
                                    out_Connection_WorldServerInfo_c(Connection_WorldServerInfo_c.ReadPacket(ref buffer));
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
                    }
                }
                return false;
            }
        }

        /// <summary>
        /// Error code to send to client about the connection.
        /// </summary>
        public class ErrorCode_c : Packet
        {
            public enum ErrorCode
            {
                InternalError,
                InvalidPacket,
                InvalidOperation
            }

            public ErrorCode error;

            public ErrorCode_c(ErrorCode error)
                : base((Int32)ClientToMasterPackets.PacketType.ErrorCode_c)
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

            public static ErrorCode_c ReadPacket(ref List<byte> buffer)
            {
                int error = TakeInt32(ref buffer);

                return new ErrorCode_c((ErrorCode)error);
            }
        }


        /// <summary>
        /// Ping to the client.
        /// </summary>
        public class AccountAuthorize_Attempt_m : Packet
        {
            public Int32 Build;
            public String Username;
            public String Password;

            public AccountAuthorize_Attempt_m(Int32 build, String username, String password)
                : base((Int32)PacketType.AccountAuthorize_Attempt_m)
            {
                this.Build = build;
                this.Username = username;
                this.Password = password;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(Build));
                    buffer.AddRange(GetBytes_String_Unicode(Username));
                    buffer.AddRange(GetBytes_String_Unicode(Password));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static AccountAuthorize_Attempt_m ReadPacket(ref List<byte> buffer)
            {
                Int32 build = TakeInt32(ref buffer);
                String username = TakeString(ref buffer);
                String password = TakeString(ref buffer);

                return new AccountAuthorize_Attempt_m(build, username, password);
            }
        }

        /// <summary>
        /// Authorization response from Master.
        /// </summary>
        public class AccountAuthorize_Response_c : Packet
        {
            public enum AuthResponse
            {
                Success,
                InvalidLogin,
                InvalidBuild
            }

            public AuthResponse Response;

            public AccountAuthorize_Response_c(AuthResponse response)
                : base((Int32)PacketType.AccountAuthorize_Response_c)
            {
                this.Response = response;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)Response));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static AccountAuthorize_Response_c ReadPacket(ref List<byte> buffer)
            {
                AuthResponse response = (AuthResponse)TakeInt32(ref buffer);

                return new AccountAuthorize_Response_c(response);
            }
        }

        /// <summary>
        /// Sends a single character to add to the list to select.
        /// </summary>
        public class Menu_CharacterListItem_c : Packet
        {
            public string Name;
            public CharacterVisualLayout VisualLayout;
            public int Level;

            public Menu_CharacterListItem_c(string name, CharacterVisualLayout vlayout, int level)
                : base((Int32)ClientToMasterPackets.PacketType.Menu_CharacterListItem_c)
            {
                this.Name = name;
                this.VisualLayout = vlayout;
                this.Level = level;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(Name));
                    buffer.AddRange(GetBytes_Int32((int)VisualLayout.Type));
                    buffer.AddRange(GetBytes_Int32(Level));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Menu_CharacterListItem_c ReadPacket(ref List<byte> buffer)
            {
                string name = TakeString(ref buffer);
                CharacterVisualLayout layout = new CharacterVisualLayout((CharacterVisualLayout.VisualType)TakeInt32(ref buffer));
                int level = TakeInt32(ref buffer);

                return new Menu_CharacterListItem_c(name, layout, level);
            }
        }

        /// <summary>
        /// Sends the selection for what character to play to the server.
        /// </summary>
        public class Menu_CharacterListItem_Select_m : Packet
        {
            public string CharName;
            public int WorldNumber;

            public Menu_CharacterListItem_Select_m(string charName, int worldNumber)
                : base((Int32)ClientToMasterPackets.PacketType.Menu_CharacterListItem_Select_m)
            {
                this.CharName = charName;
                this.WorldNumber = worldNumber;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(CharName));
                    buffer.AddRange(GetBytes_Int32(WorldNumber));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Menu_CharacterListItem_Select_m ReadPacket(ref List<byte> buffer)
            {
                string charName = TakeString(ref buffer);
                int worldNum = TakeInt32(ref buffer);

                return new Menu_CharacterListItem_Select_m(charName, worldNum);
            }
        }

        /// <summary>
        /// Informs the client to which WorldServer it should connect to.
        /// </summary>
        public class Connection_WorldServerInfo_c : Packet
        {
            public IPEndPoint EndPoint;
            public string CharName;

            public Connection_WorldServerInfo_c(IPEndPoint endPoint, string charName)
                : base((Int32)ClientToMasterPackets.PacketType.Connection_WorldServerInfo_c)
            {
                this.EndPoint = endPoint;
                this.CharName = charName;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(EndPoint.Address.ToString()));
                    buffer.AddRange(GetBytes_Int32(EndPoint.Port));
                    buffer.AddRange(GetBytes_String_Unicode(CharName));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Connection_WorldServerInfo_c ReadPacket(ref List<byte> buffer)
            {
                string ip = TakeString(ref buffer);
                int port = TakeInt32(ref buffer);
                string charName = TakeString(ref buffer);

                return new Connection_WorldServerInfo_c(new IPEndPoint(IPAddress.Parse(ip), port), charName);
            }
        }

    }
}
