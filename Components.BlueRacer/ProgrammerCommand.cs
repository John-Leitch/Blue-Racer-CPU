using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public enum ProgrammerCommand : byte
    {
        Break,
        Continue,
        Reset,
        Restart,
        Read,
        Write,
        GetContext,
        GetPageFlags,
        SetPageFlags,
    }
}
