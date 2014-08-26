using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

namespace Processor
{
    [Flags]
    public enum MemoryAccessFlag : uint
    {
        None = 0x0,
        Read = 0x1,
        Write = 0x2,
        Execute = 0x4,

        //Read = 0x20000000,
        //Write = 0x40000000,
        //Execute = 0x80000000,

    }
}
