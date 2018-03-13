#define DEBUG_PACKETS

using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

using Extant;
using Extant.Networking;

namespace SharedComponents.ServerToServer
{
    public static class WorldToMasterPackets
    {
        /// <summary>
        /// Thrown when a packet is instructed to create a send buffer.
        /// This usually means the packet is being sent over a network.
        /// </summary>
        public static event PacketActionHandler PacketCreatedBuffer;
        /// <summary>
        /// Thrown when a packet is read from the buffer.
        /// This usually means the packet is being received over a network.
        /// </summary>
        public static event PacketActionHandler PacketReadFromBuffer;
        public delegate void PacketActionHandler(object sender, Packet packet);

        public enum PacketType
        {
            Null,

            Ping_mw,

            Authenticate_Attempt_m,
            Authenticate_Response_w
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
                            case (PacketType.Ping_mw):
                                returnPacket = Ping_mw.ReadPacket(ref buffer);
                                break;

                            default:
                                throw new Packet.InvalidPacketRead("Invalid packet header: " + ((PacketType)packetType).ToString());
                        }

                        if (Packet.TakeByte(ref buffer) == Packet.END_PACKET)
                        {
                            if (PacketCreatedBuffer != null)
                                PacketCreatedBuffer(null, returnPacket);
                            return returnPacket;
                        }
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
        public class Ping_mw : Packet
        {
            public enum PingCode
            {
                ContinueConnection
            }

            public PingCode code;

            public Ping_mw(PingCode code)
                : base((Int32)PacketType.Ping_mw)
            {
                this.code = code;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)code));
                }
                buffer.Add(END_PACKET);

                if (PacketCreatedBuffer != null)
                    PacketCreatedBuffer(null, this);
                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                PingCode code = (PingCode)TakeInt32(ref buffer);

                return new Ping_mw(code);
            }
        }

        /// <summary>
        /// Authentication to send to main server.
        /// </summary>
        public class Authenticate_Attempt_m : Packet
        {
            public int serverId;
            public int buildNumber;

            public Authenticate_Attempt_m(int serverId, int buildNumber)
                : base((Int32)WorldToMasterPackets.PacketType.Authenticate_Attempt_m)
            {
                this.serverId = serverId;
                this.buildNumber = buildNumber;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32(buildNumber));
                }
                buffer.Add(END_PACKET);

                if (PacketCreatedBuffer != null)
                    PacketCreatedBuffer(null, this);
                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                int serverId = TakeInt32(ref buffer);
                int buildNumber = TakeInt32(ref buffer);

                return new Authenticate_Attempt_m(serverId, buildNumber);
            }
        }

        /// <summary>
        /// Authentication response from the main server.
        /// </summary>
        public class Authenticate_Response_w : Packet
        {
            public enum ResponseCode
            {
                Success,
                InvalidBuild,
                InvalidServerId
            }

            public ResponseCode responseCode;

            public Authenticate_Response_w(ResponseCode responseCode)
                : base((Int32)WorldToMasterPackets.PacketType.Authenticate_Response_w)
            {
                this.responseCode = responseCode;
            }

            public override Byte[] CreateSendBuffer()
            {
                List<Byte> buffer = new List<Byte>();
                {
                    buffer.AddRange(GetBytes_Int32((Int32)type));

                    buffer.AddRange(GetBytes_Int32((Int32)responseCode));
                }
                buffer.Add(END_PACKET);

                if (PacketCreatedBuffer != null)
                    PacketCreatedBuffer(null, this);
                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                ResponseCode someData = (ResponseCode)TakeInt32(ref buffer);

                return new Authenticate_Response_w(someData);
            }
        }
    }
}