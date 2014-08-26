using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace Components
{
    public static class ByteHelper
    {
        public static byte[] Parse(string binaryData)
        {
            binaryData = Regex.Replace(binaryData, @"[\s:]", "");

            if ((binaryData.Length & 1) != 0)
            {
                throw new ArgumentException();
            }

            var bytes = new byte[binaryData.Length / 2];

            for (int i = 0; i < binaryData.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(binaryData.Substring(i, 2), 16);
            }

            return bytes;
        }

        public static TStruct BytesToStruct<TStruct>(byte[] bytes)
        {
            var h = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            var s = (TStruct)Marshal.PtrToStructure(h.AddrOfPinnedObject(), typeof(TStruct));
            h.Free();
            return s;
        }

        public static uint SwapUInt32(uint x)
        {
            return ((x >> 24) & 0x000000FF) |
                ((x >> 8) & 0x0000FF00) |
                ((x << 8) & 0x00FF0000) |
                ((x << 24) & 0xFF000000);
        }

        public static ushort SwapUInt16(ushort x)
        {
            return (ushort)
                (((x >> 8) & 0x00FF) |
                ((x << 8) & 0xFF00));
        }

        public static string ToHex(byte[] b)
        {
            return string.Join("", b.Select(x => string.Format("{0:X2}", x)));
        }
    }
}
