using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class InstructionMnemonic
    {
        public const string Goto = "goto",
            GotoTrue = "gotoTrue",
            GotoFalse = "gotoFalse",
            Read = "read",
            Write = "write",
            Error = "error",
            Out0 = "out0",
            Out1 = "out1",
            Out2 = "out2",
            Out3 = "out3",
            Data = "data",
            Macro = "macro",
            Push = "push",
            Pop = "pop",
            Call = "call",
            Return = "return"
            ;

        public static string[] GetAll()
        {
            return typeof(InstructionMnemonic)
                .GetFields()
                .Where(x => x.FieldType == typeof(string))
                .Select(x => (string)x.GetValue(null))
                .ToArray();
        }
    }
}
