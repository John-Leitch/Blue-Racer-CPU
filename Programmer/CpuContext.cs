using System;
using Microsoft.SPOT;
using System.Net.Sockets;

namespace Programmer
{
    public class CpuContext
    {
        public uint InstructionAddress { get; set; }

        public uint ProgramCounter { get; set; }

        public uint Opcode { get; set; }

        public uint Operands { get; set; }

        public uint Error { get; set; }

        public uint ErrorCode { get; set; }

        public CpuContext()
        {
        }

        public CpuContext(
            uint instructionAddress,
            uint programCounter,
            uint opcode,
            uint operands,
            uint error,
            uint errorCode)
        {
            InstructionAddress = instructionAddress;
            ProgramCounter = programCounter;
            Opcode = opcode;
            Operands = operands;
            Error = error;
            ErrorCode = errorCode;
        }

        public void WriteTo(Socket socket)
        {
            foreach (var x in new[]
            {
                InstructionAddress,
                ProgramCounter,
                Opcode,
                Operands,
                Error,
                ErrorCode,
            })
            {
                socket.Send(BitConverter.GetBytes(x));
            }
        }
    }
}
