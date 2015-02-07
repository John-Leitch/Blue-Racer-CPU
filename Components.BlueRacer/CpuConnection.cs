using Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public abstract class CpuConnection : IDisposable
    {
        public bool IsConnected { get; protected set; }

        public abstract void Open(string connectionString);

        public abstract void Disconnect();

        public abstract void Dispose();

        public abstract void Write(byte[] buffer);

        public abstract byte[] Read(int bufferLength);

        public ProgrammerCommand ReadCommand()
        {
            return (ProgrammerCommand)Read(1)[0];
        }

        public void ExpectCommand(ProgrammerCommand command)
        {
            if (ReadCommand() != command)
            {
                throw new InvalidOperationException();
            }
        }

        public void WriteCommand(ProgrammerCommand command)
        {
            WriteCommand(command, 0);
        }

        public void WriteAndExpectCommand(ProgrammerCommand command)
        {
            WriteCommand(command);
            ExpectCommand(command);
        }

        public void WriteCommand(ProgrammerCommand command, uint operand)
        {
            Write(new byte[] { (byte)command }.Concat(BitConverter.GetBytes(operand)).ToArray());
        }
    }
}
