#define DEBUG_PACKETS

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace Extant.Networking
{
    /// <summary>
    /// Packets are containers of information to be sent over the network.
    /// Names are based on this convention: (Topic)_(Action)_(Target)
    /// Targets are as follows:
    ///     c = Player Client
    ///     g = GameServer
    /// </summary>
    public abstract class Packet
    {
        public static readonly Byte EMPTY_BYTE = (Byte)0;
        public static readonly Byte END_PACKET = (Byte)23; //End of trans. block
        public static readonly Char EMPTY_CHAR = (Char)0;
        public static readonly Byte BYTE_TRUE = (Byte)1;
        public static readonly Byte BYTE_FALSE = (Byte)0;

        protected Int32 type;
        private ProtocolType protocol;

        public abstract Byte[] CreateSendBuffer();

        protected Packet(Int32 t, ProtocolType p)
        {
            type = t;
            protocol = p;
        }

        public Int32 Type
        {
            get
            {
                return type;
            }
        }

        public ProtocolType Protocol
        {
            get
            {
                return protocol;
            }
        }

        /// <summary>
        /// Returns an Int32 that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static Int32 TakeInt32(ref List<Byte> buff)
        {
            if (buff.Count < sizeof(Int32))
                throw new ArgumentOutOfRangeException("Buffer not big enough to pull Int32.");

            Int32 read = BitConverter.ToInt32(buff.ToArray(), 0);
            buff = buff.Skip(sizeof(Int32)).ToList();
            return read;
        }

        /// <summary>
        /// Returns a Single (float) that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static Single TakeSingle(ref List<Byte> buff)
        {
            if (buff.Count < sizeof(Int32))
                throw new ArgumentOutOfRangeException("Buffer not big enough to pull Single.");

            Single read = BitConverter.ToSingle(buff.ToArray(), 0);
            buff = buff.Skip(sizeof(Single)).ToList();
            return read;
        }

        /// <summary>
        /// Returns a Double that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static Double TakeDouble(ref List<Byte> buff)
        {
            if (buff.Count < sizeof(Double))
                throw new ArgumentOutOfRangeException("Buffer not big enough to pull Double.");

            Double read = BitConverter.ToDouble(buff.ToArray(), 0);
            buff = buff.Skip(sizeof(Double)).ToList();
            return read;
        }

        /// <summary>
        /// Returns a Byte that is read and removed from beginning of List.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static Byte TakeByte(ref List<Byte> buff)
        {
            if (buff.Count < sizeof(Byte))
                throw new ArgumentOutOfRangeException("Buffer not big enough to pull Byte.");

            Byte read = buff[0];
            buff = buff.Skip(sizeof(Byte)).ToList();
            return read;
        }

        /// <summary>
        /// Returns a String of Chars that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static Char[] TakeUnicodeChars(ref List<Byte> buff, int count)
        {
            if (buff.Count < count)
                throw new ArgumentOutOfRangeException("Buffer not big enough to pull Char(" + count + ").");

            Char[] arr = Encoding.Unicode.GetChars(buff.ToArray(), 0, count);
            int returnAmount = 0;
            buff = buff.Skip(count).ToList();
            foreach (Char c in arr)
            {
                if (c == EMPTY_CHAR)
                    break;
                returnAmount++;
            }
            return arr.Take(returnAmount).ToArray();
        }

        /// <summary>
        /// Returns a String of Chars that is read and removed from beginning of List of Bytes.
        /// </summary>
        /// <param name="buff">The array for data to be taken from.</param>
        public static String TakeString(ref List<Byte> buff)
        {
            int byteCount = (int)TakeByte(ref buff);
            if (byteCount <= 0)
                throw new InvalidPacketRead("TakeString cannot read from '" + byteCount + "' bytes.");

            Char[] charArr = Encoding.Unicode.GetChars(buff.ToArray(), 0, byteCount);

            buff = buff.Skip(byteCount).ToList();
            return new String(charArr);
        }

        /// <summary>
        /// Returns the Bytes that make up an Int32.
        /// </summary>
        public static Byte[] GetBytes_Int32(Int32 i)
        {
            return BitConverter.GetBytes(i);
        }

        /// <summary>
        /// Returns the Bytes that make up a Single (float).
        /// </summary>
        public static Byte[] GetBytes_Single(Single s)
        {
            return BitConverter.GetBytes(s);
        }

        /// <summary>
        /// Returns the Bytes that make up a Double.
        /// </summary>
        public static Byte[] GetBytes_Double(Double d)
        {
            return BitConverter.GetBytes(d);
        }

        public static Byte[] GetBytes_String_Unicode(String str)
        {
            Char[] chars = str.ToArray();
            List<byte> arr = new List<byte>();

            arr.Add((byte)Encoding.Unicode.GetByteCount(chars, 0, chars.Length));
            arr.AddRange(Encoding.Unicode.GetBytes(chars, 0, chars.Length));

            return arr.ToArray();
        }

        /// <summary>
        /// Thrown if a packet is not a valid set of data for the corresponding header..
        /// </summary>
        public class InvalidPacketRead : Exception
        {
            public InvalidPacketRead(String m)
                : base("Packet was found to be invalid upon reading: " + m)
            { }
        }
    }
}