using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public sealed class CpuDebugger
    {
        private CpuEthernetConnection _connection;

        public CpuDebugger(CpuEthernetConnection connection)
        {
            _connection = connection;
        }

        public CpuContext GetContext()
        {
            _connection.WriteCommand(ProgrammerCommand.GetContext);

            var bytes = new List<byte>();

            var tries = 0;

            while (bytes.Count < 24)
            {
                if (tries++ > 10)
                {
                    throw new SocketException();
                }

                var buffer = _connection.Read(24 - bytes.Count);
                bytes.AddRange(buffer);
            }

            _connection.ExpectCommand(ProgrammerCommand.GetContext);

            return new CpuContext(bytes.ToArray());
        }



        public void Break()
        {
            _connection.WriteAndExpectCommand(ProgrammerCommand.Break);
        }

        public void Continue()
        {
            _connection.WriteAndExpectCommand(ProgrammerCommand.Continue);
        }

        public void Reset()
        {
            _connection.WriteAndExpectCommand(ProgrammerCommand.Reset);
        }

        public void Restart()
        {
            _connection.WriteAndExpectCommand(ProgrammerCommand.Restart);
        }

        public uint Read(uint address)
        {
            _connection.WriteCommand(ProgrammerCommand.Read, address);
            var u = BitConverter.ToUInt32(_connection.Read(4), 0);
            _connection.ExpectCommand(ProgrammerCommand.Read);

            return u;
        }

        public void Write(uint address, uint data)
        {
            _connection.WriteCommand(ProgrammerCommand.Write, address);
            _connection.Write(BitConverter.GetBytes(data));
            _connection.ExpectCommand(ProgrammerCommand.Write);
        }
    }
}
