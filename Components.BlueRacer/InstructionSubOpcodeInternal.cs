using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public enum InstructionSubOpcodeInternal : byte
    {
        Assign = 0x01,
        
        Inc_R = 0x20,
        Dec_R = 0x21,
        
        In = 0xf0,
        Out = 0xf1,

        Eq_R_R = 0x50,
        NotEq_R_R = 0x51,
        LessThan_R_R = 0x52,
        LessThanEq_R_R = 0x53,
        GreaterThan_R_R = 0x54,
        GreaterThanEq_R_R = 0x55,
    }
}
