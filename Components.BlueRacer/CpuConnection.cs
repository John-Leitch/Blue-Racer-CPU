using Components.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public sealed class CpuConnection : IDisposable
    {
        private NetworkStream _stream;

        public bool IsConnected { get; private set; }

        public void Open(string host)
        {
            ExceptionHelper.InvalidOperationIf(IsConnected);
            IsConnected = true;
            _stream = TcpHelper.GetStream(host, 5230);
        }

        public void Disconnect()
        {
            ExceptionHelper.InvalidOperationIf(!IsConnected);
            IsConnected = false;
            _stream.Dispose();
            _stream = null;
        }

        public void Dispose()
        {
            ExceptionHelper.InvalidOperationIf(IsConnected);
        }

        public void Write(byte[] buffer)
        {
            _stream.Write(buffer);
        }

        public byte[] Read(int bufferLength)
        {
            return _stream.ReadAll(bufferLength);
        }

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

        public static CpuConnection Create(string host)
        {
            var connection = new CpuConnection();
            connection.Open(host);

            return connection;
        }
    }
}
