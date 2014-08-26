using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public enum CpuCommandState : byte
    {
        Waiting = 0x00,
        ReadAddress = 0x10,
        WriteAddress = 0x30,
        Write4 = 0x40,
        WaitingGetPageNumber = 0x50,
        WaitingSetPageNumber = 0x51,
        WaitingSetPageFlags = 0x52,
    }
}
