using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class CpuContext
    {
        public uint InstructionAddress { get; set; }

        public uint ProgramCounter { get; set; }

        public InstructionOpcode Opcode { get; set; }

        public uint Operands { get; set; }

        public bool Error { get; set; }

        public CpuErrorCode ErrorCode { get; set; }

        public CpuContext()
        {

        }

        public CpuContext(
            uint instructionAddress,
            uint programCounter,
            InstructionOpcode opcode,
            uint operands,
            bool error,
            CpuErrorCode errorCode)
        {
            InstructionAddress = instructionAddress;
            ProgramCounter = programCounter;
            Opcode = opcode;
            Operands = operands;
            Error = error;
            ErrorCode = errorCode;
        }

        public CpuContext(byte[] buffer)
            : this(
                BitConverter.ToUInt32(buffer, 0),
                BitConverter.ToUInt32(buffer, 4),
                (InstructionOpcode)BitConverter.ToUInt32(buffer, 8),
                BitConverter.ToUInt32(buffer, 12),
                BitConverter.ToUInt32(buffer, 16) == 1,
                (CpuErrorCode)BitConverter.ToUInt32(buffer, 20))
        {
        }

        //public override string ToString()
        //{

        //}
    }
}
