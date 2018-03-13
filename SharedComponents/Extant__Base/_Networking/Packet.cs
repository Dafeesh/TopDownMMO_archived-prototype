using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Extant._Networking
{
    public abstract class NetPacket
    {
        private const Byte BYTE_ENDPACKET = (Byte)23; //End of trans. block
 
        private Int32 tag;
        protected NetPacketItem[] items;
 
        public NetPacket(Int32 tag, NetPacketItem[] items)
        {
            this.tag = tag;
            this.items = items;
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
        public static TypeCode[] SupportedTypes = { TypeCode.Byte, TypeCode.Int32 };
 
        public Type type;
        public object value;
 
        public NetPacketItem(Type type, object value)
        {
            if (SupportedTypes.Contains(Type.GetTypeCode(type)) == false)
            {
                throw new InvalidOperationException("NetPacket does not support object type '" + type.ToString() + "'.");
            }
 
            this.type = type;
            this.value = value;
        }

        public Type Type
        {
            get
            {
                return type;
            }
        }

        public object Value
        {
            get
            {
                return value;
            }
        }
    }
}