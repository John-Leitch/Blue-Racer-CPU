using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class VerilogDocument
    {
        private StringBuilder _document = new StringBuilder();

        private string CreateDefine(string name, string value)
        {
            return string.Format("`define {0} {1}", name, value);
        }

        public void AddDefine8(string name, byte b)
        {
            _document.AppendLine(string.Format("`define {0} 8'h{1:X2}", name, b));
        }

        public void AddDefine16(string name, ushort b)
        {
            _document.AppendLine(string.Format("`define {0} 16'h{1:X4}", name, b));
        }

        public void AddDefine32(string name, uint b)
        {
            _document.AppendLine(string.Format("`define {0} 32'h{1:X8}", name, b));
        }

        public void AddDefineString(string name, string body)
        {
            var value = body.Contains("\r") || body.Contains("\n") ?
                " \\\r\n" + body.Replace("\r\n", "\n").Replace('\r', '\n').Replace("\n", " \\\r\n") :
                body;

            _document.AppendLine(CreateDefine(name, value + "\r\n"));
        }

        public void AddLine()
        {
            _document.AppendLine();
        }

        public override string ToString()
        {
            return _document.ToString();
        }
    }
}
