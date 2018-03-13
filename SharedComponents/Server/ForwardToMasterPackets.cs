using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

using Extant;
using Extant.Networking;

namespace SharedComponents.Server
{
    public static class ForwardToMasterPackets
    {
        public enum PacketType
        {
            Null,

            AccountAuthorize_Attempt_m,
            AccountAuthorize_Response_f
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
                            case (PacketType.AccountAuthorize_Attempt_m):
                                returnPacket = AccountAuthorize_Attempt_m.ReadPacket(ref buffer);
                                break;

                            case (PacketType.AccountAuthorize_Response_f):
                                returnPacket = AccountAuthorize_Response_f.ReadPacket(ref buffer);
                                break;

                            default:
                                throw new Packet.InvalidPacketRead("Invalid packet header: " + ((PacketType)packetType).ToString());
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                            return returnPacket;
                        else
                            throw new Packet.InvalidPacketRead("Last byte of packet was not END_PACKET byte.");
                    }
                    catch (ArgumentOutOfRangeException) //Not enough data yet to make a full packet.
                    {
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
        public class AccountAuthorize_Attempt_m : Packet
        {
            public UInt32 ClientID;
            public String Username;
            public String Password;

            public AccountAuthorize_Attempt_m(UInt32 clientID, String username, String password)
                : base((Int32)PacketType.AccountAuthorize_Attempt_m)
            {
                this.ClientID = clientID;
                this.Username = username;
                this.Password = password;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_UInt32(ClientID));
                    buffer.AddRange(GetBytes_String_Unicode(Username));
                    buffer.AddRange(GetBytes_String_Unicode(Password));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                UInt32 clientID = TakeUInt32(ref buffer);
                String username = TakeString(ref buffer);
                String password = TakeString(ref buffer);

                return new AccountAuthorize_Attempt_m(clientID, username, password);
            }
        }

        /// <summary>
        /// Authorization response from Master.
        /// </summary>
        public class AccountAuthorize_Response_f : Packet
        {
            public UInt32 ClientID;
            public bool Success;

            public AccountAuthorize_Response_f(UInt32 clientID, bool success)
                : base((Int32)PacketType.AccountAuthorize_Response_f)
            {
                this.ClientID = clientID;
                this.Success = success;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_UInt32(ClientID));
                    buffer.Add((Success)?(BYTE_TRUE):(BYTE_FALSE));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                UInt32 clientID = TakeUInt32(ref buffer);
                bool success = (TakeByte(ref buffer) == BYTE_TRUE);

                return new AccountAuthorize_Response_f(clientID, success);
            }
        }
    }
}