using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class BigEndianBitConverter
    {
        public static ushort ToUInt16(byte[] bytes, int offset)
        {
            return (ushort)((bytes[offset] << 8) | bytes[offset + 1]);
        }

        public static uint ToUInt32(byte[] bytes, int offset)
        {
            return
                ((uint)bytes[offset] << 24) |
                ((uint)bytes[offset + 1] << 16) |
                ((uint)bytes[offset + 2] << 8) |
                (uint)bytes[offset + 3];
        }

        public static byte ToHighNibble(byte[] bytes, int offset)
        {
            return (byte)(bytes[offset] >> 4);
        }

        public static byte ToLowNibble(byte[] bytes, int offset)
        {
            return (byte)(bytes[offset] & 0x0F);
        }
    }
}
