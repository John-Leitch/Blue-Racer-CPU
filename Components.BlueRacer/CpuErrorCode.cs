using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public enum CpuErrorCode : uint
    {
        None = 0x00000000,

        General = 0x10000000,
        Opcode = 0x10000001,
        SubOpcode = 0x10000002,
        SubOpcode2 = 0x10000003,
        Operand = 0x10000004,

        Read = 0x20000000,
        Write = 0x20000001,
        Execute = 0x20000002,

        InvalidState = 0x30000000,
        InvalidCommand = 0x30000001,
        InvalidCommandState = 0x30000002,

        ArithmeticOperand0 = 0x40000000,
        ArithmeticOperand1 = 0x40000001,
    }
}
