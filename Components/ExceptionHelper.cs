using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class ExceptionHelper
    {
        [DebuggerStepThrough]
        public static void InvalidOperationIf(bool condition)
        {
            if (condition)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
