using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Extant;
using Extant.Networking;

namespace SharedComponents.Global
{
    public static class ClientToForwardPackets
    {
        public enum PacketType
        {
            Null,

            ErrorCode_c,

            AccountAuthorize_Attempt_f,
            AccountAuthorize_Response_c
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
                            case (PacketType.ErrorCode_c):
                                returnPacket = ErrorCode_c.ReadPacket(ref buffer);
                                break;

                            case (PacketType.AccountAuthorize_Attempt_f):
                                returnPacket = AccountAuthorize_Attempt_f.ReadPacket(ref buffer);
                                break;

                            case (PacketType.AccountAuthorize_Response_c):
                                returnPacket = AccountAuthorize_Response_c.ReadPacket(ref buffer);
                                break;

                            default:
                                DebugLogger.Global.Log("Invalid packet header: " + packetType + "/" + ((PacketType)packetType).ToString());
                                throw new Packet.InvalidPacketRead("Invalid packet header: " + ((PacketType)packetType).ToString());
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                            return returnPacket;
                        else
                            throw new Packet.InvalidPacketRead("Last byte of packet was not END_PACKET byte.");
                    }
                    catch (ArgumentOutOfRangeException)// e) //Not enough data yet to make a full packet.
                    {
                        //DebugLogger.Global.Log("Packet not large enough yet." + e.ToString());
                        buffer = backupBuffer.ToList();
                        return null;
                    }
                }
            }
            return null;
        }

        /// <summary>
        /// Error code to send to client about the connection.
        /// </summary>
        public class ErrorCode_c : Packet
        {
            public enum ErrorCode
            {
                LostConnectionToMain,
                InternalError,
                InvalidPacket
            }

            public ErrorCode error;

            public ErrorCode_c(ErrorCode error)
                : base((Int32)ClientToForwardPackets.PacketType.ErrorCode_c)
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

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                int error = TakeInt32(ref buffer);

                return new ErrorCode_c((ErrorCode)error);
            }
        }


        /// <summary>
        /// Ping to the client.
        /// </summary>
        public class AccountAuthorize_Attempt_f : Packet
        {
            public String Username;
            public String Password;

            public AccountAuthorize_Attempt_f(String username, String password)
                : base((Int32)PacketType.AccountAuthorize_Attempt_f)
            {
                this.Username = username;
                this.Password = password;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_String_Unicode(Username));
                    buffer.AddRange(GetBytes_String_Unicode(Password));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                String username = TakeString(ref buffer);
                String password = TakeString(ref buffer);

                return new AccountAuthorize_Attempt_f(username, password);
            }
        }

        /// <summary>
        /// Authorization response from Master.
        /// </summary>
        public class AccountAuthorize_Response_c : Packet
        {
            public bool Success;

            public AccountAuthorize_Response_c(bool success)
                : base((Int32)PacketType.AccountAuthorize_Response_c)
            {
                this.Success = success;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.Add((Success) ? (BYTE_TRUE) : (BYTE_FALSE));
                }
                buffer.Add(END_PACKET);

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                bool success = (TakeByte(ref buffer) == BYTE_TRUE);

                return new AccountAuthorize_Response_c(success);
            }
        }

    }
}
