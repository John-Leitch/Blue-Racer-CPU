using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class CpuProgrammer
    {
        private CpuConnection _connection;

        public CpuProgrammer(CpuConnection connection)
        {
            _connection = connection;
        }

        public void SetPageFlags(uint pageNumber, MemoryAccessFlag flags)
        {
            _connection.WriteCommand(ProgrammerCommand.SetPageFlags, pageNumber);
            _connection.Write(new[] { (byte)flags });
            _connection.ExpectCommand(ProgrammerCommand.SetPageFlags);
        }

        public MemoryAccessFlag GetPageFlags(uint pageNumber)
        {
            _connection.WriteCommand(ProgrammerCommand.GetPageFlags, pageNumber);
            var f = (MemoryAccessFlag)_connection.Read(1)[0];
            _connection.ExpectCommand(ProgrammerCommand.GetPageFlags);

            return f;
        }

        public void Program(string binFile)
        {
            var bytes = File.ReadAllBytes(binFile);
            var remainder = bytes.Length % 4;

            if (remainder != 0)
            {
                var padding = 4 - remainder;
                Array.Resize(ref bytes, bytes.Length + padding);
            }

            var pageCount = (uint)Math.Ceiling((double)bytes.Length / 0x100);

            var dbg = new CpuDebugger(_connection);
            dbg.Break();

            for (uint i = 0; i < bytes.Length; i += 4)
            {
                var data = BigEndianBitConverter.ToUInt32(bytes, (int)i);
                dbg.Write(i, data);
            }

            for (uint i = 0; i < pageCount; i++)
            {
                SetPageFlags(i, MemoryAccessFlag.Read | MemoryAccessFlag.Execute);
                //System.Threading.Thread.Sleep(100);
            }

            for (uint i = pageCount; i < 0xe0; i++)
            {
                SetPageFlags(i, MemoryAccessFlag.Read | MemoryAccessFlag.Write);
            }

            dbg.Restart();

            var context = dbg.GetContext();

            if (context.Error)
            {
                throw new InvalidOperationException(context.ErrorCode.ToString());
            }

            //while (true)
            //{

            //}
        }
    }
}
