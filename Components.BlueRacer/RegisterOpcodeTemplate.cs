using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class RegisterOpcodeTemplate
    {
        public string Format { get; set; }

        public byte StartValue { get; set; }

        public RegisterOpcodeTemplate()
        {
        }

        public RegisterOpcodeTemplate(string format, byte startValue)
        {
            Format = format;
            StartValue = startValue;
        }

        public Tuple<string, byte>[] CreateEnums(int registerCount)
        {
            return Enumerable
                .Range(StartValue, registerCount)
                .Select(x => Tuple.Create(string.Format(Format, x - StartValue), (byte)x))
                .ToArray();
        }
    }
}
