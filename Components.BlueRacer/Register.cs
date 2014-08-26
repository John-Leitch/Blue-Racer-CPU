using Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class Register
    {
        public const int Count = 8;

        public static string CreateDeclarations()
        {
            return string.Format(
                "reg [Architecture-1:0] {0}, _programCounter = 0;",
                Enumerable
                    .Range(0, Register.Count)
                    .Select(x => string.Format("_r{0} = 0", x))
                    .Join(", "));
        }
    }
}
