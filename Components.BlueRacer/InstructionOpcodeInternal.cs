using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public enum InstructionOpcodeInternal : byte
    {
        
        Push_Const = 0x01,
        
        
        SetOut0_Const = 0xa,
        SetOut1_Const = 0xb,
        SetOut2_Const = 0xc,
        SetOut3_Const = 0xd,
        //SetOut_R0 = 0xb0,
        //Read_R_R = 0xC0,
        //Write_R_R = 0xC1,
        
        Call_Const = 0xE0,
        Goto_Const = 0xE2,
        Goto_Eq = 0xE5,
        Goto_NotEq = 0xE6,

        Ext = 0xfb,
        ExtMultiStage = 0xfd,
        Error = 0xff,
    }
}
