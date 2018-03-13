using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Extant.Networking
{
    public abstract class NetPacket
    {
        private const Byte BYTE_ENDPACKET = (Byte)23; //End of trans. block

        private Int32 tag;
        private List<NetPacketItem> items;

        public NetPacket(Int32 tag)
        {
            this.tag = tag;
            this.items = new List<NetPacketItem>();
        }

        public byte[] CreateSendBuffer()
        {
            List<byte> buffer = new List<byte>();
            {
                foreach (NetPacketItem item in items)
                {
                    switch (Type.GetTypeCode(item.Type))
                    {
                        case (TypeCode.Byte):
                            buffer.Add((byte)item.Value);
                            break;

                        case (TypeCode.Int32):
                            buffer.AddRange(BitConverter.GetBytes((Int32)item.Value));
                            break;
                    }
                }
                buffer.Add(BYTE_ENDPACKET);
            }
            return buffer.ToArray();
        }
    }

    public class NetPacketItem
    {
        public readonly TypeCode[] SupportedTypes = { TypeCode.Byte, TypeCode.Int32 };

        public Type Type;
        public object Value;

        public NetPacketItem(Type type, object value)
        {
            if (SupportedTypes.Contains(Type.GetTypeCode(type)) == false)
            {
                throw new InvalidOperationException("NetPacket does not support object type '" + type.ToString() + "'.");
            }

            Type = type;
            Value = value;
        }
    }
}
