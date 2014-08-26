using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class ModelSimSettings
    {
        public string WorkingPath { get; set; }

        public string[] VerilogFiles { get; set; }

        public string TestModule { get; set; }

        public string[] Signals { get; set; }

        public int RunTime { get; set; }

        public string Output { get; set; }

        public string CreateScript()
        {
            var sb = new StringBuilder();

            sb.AppendFormat("cd {0}\r\n", WorkingPath);
            sb.Append("project open Md5\r\n");
            sb.Append(VerilogFiles.Select(x => string.Format("vlog {0}\r\n", x)).Join());
            sb.AppendFormat("vsim {0}\r\n", TestModule);
            sb.Append(Signals.Select(x => string.Format("add list {0}\r\n", x)).Join());
            sb.AppendFormat("run {0}\r\n", RunTime);
            sb.AppendFormat("write list {0}\r\n", Output);
            sb.Append("quit -force\r\n");

            return sb.ToString();
        }
    }
}
