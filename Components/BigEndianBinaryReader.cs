using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class BigEndianBinaryReader : BinaryReader
    {
        public override decimal ReadDecimal()
        {
            throw new NotImplementedException();
        }

        public override double ReadDouble()
        {
            throw new NotImplementedException();
        }

        public override short ReadInt16()
        {
            var b = ReadBytes(2);
            //var asdf = BitConverter.ToInt16(b.Reverse().ToArray(), 0);
            
            return (short)(((short)b[0] << 8) + (short)b[1]);
        }

        public override int ReadInt32()
        {
            throw new NotImplementedException();
        }

        public override long ReadInt64()
        {
            throw new NotImplementedException();
        }

        public override sbyte ReadSByte()
        {
            throw new NotImplementedException();
        }

        public override float ReadSingle()
        {
            throw new NotImplementedException();
        }

        public override string ReadString()
        {
            throw new NotImplementedException();
        }

        public override ushort ReadUInt16()
        {
            var b = ReadBytes(2);
            return (ushort)(((uint)b[0] << 8) + (uint)b[1]);
        }

        public override uint ReadUInt32()
        {
            var b = ReadBytes(4);

            if (b.Length < 4)
            {
                return 0;
            }

            return ((uint)b[0] << 24) +
                ((uint)b[1] << 16) +
                ((uint)b[2] << 8) +
                ((uint)b[3]);
        }

        public override ulong ReadUInt64()
        {
            var b = ReadBytes(8);

            if (b.Length < 8)
            {
                return 0;
            }

            return
                ((uint)b[0] << (8 * 7)) |
                ((uint)b[1] << (8 * 6)) |
                ((uint)b[2] << (8 * 5)) |
                ((uint)b[3] << (8 * 4)) |
                ((uint)b[4] << (8 * 3)) |
                ((uint)b[5] << (8 * 2)) |
                ((uint)b[6] << (8 * 1)) |
                ((uint)b[7] << (8 * 0));
        }

        public BigEndianBinaryReader(Stream input)
            : base(input)
        {
        }

        public BigEndianBinaryReader(Stream input, Encoding encoding)
            : base(input, encoding)
        {
        }

        public BigEndianBinaryReader(Stream input, Encoding encoding, bool leaveOpen)
            : base(input, encoding, leaveOpen)
        {
        }
    }
}
