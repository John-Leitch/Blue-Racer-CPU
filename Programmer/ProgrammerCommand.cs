using System;
using Microsoft.SPOT;

namespace Programmer
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
