using Components.BlueRacer;
using Components.ConsolePlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuShell
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 1)
            {
                Cli.WriteLine("CpuShell [host]");
                Environment.Exit(1);
            }

            var console = new CpuConsole();
            console.Connect(args[0]);

            while (true)
            {
                try
                {
                    console.ReadCommand();
                }
                catch (Exception e)
                {
                    Cli.WriteErrorMessage("Error reading command: {0}", e.Message);
                }
            }
        }
    }
}
