using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class EnumGenerator
    {
        private StringBuilder _enums = new StringBuilder();

        private string _name;

        public EnumGenerator(string name)
        {
            _name = name;
        }

        public void Add(string name, byte value)
        {
            _enums.AppendLine(string.Format("        {0} = 0x{1:X2},", name, value));
        }

        public override string ToString()
        {
            return string.Format(
                "namespace Components.BlueRacer\r\n{{\r\n    public enum {0} : byte\r\n    {{\r\n{1}\r\n    }}\r\n}}",
                _name,
                _enums.ToString());
        }
    }
}
