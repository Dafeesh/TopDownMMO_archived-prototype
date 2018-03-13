using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

using Extant;
using Extant.Networking;

namespace SharedComponents.Server
{
    public static class InstanceToMasterPackets
    {
        public enum PacketType
        {
            Null,

            Ping_mw
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

                return buffer.ToArray();
            }

            public static Packet ReadPacket(ref List<byte> buffer)
            {
                PingCode code = (PingCode)TakeInt32(ref buffer);

                return new Ping_mw(code);
            }
        }
    }
}