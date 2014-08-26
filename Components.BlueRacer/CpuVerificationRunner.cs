using Components.ConsolePlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class CpuVerificationRunner
    {
        public CpuVerifier Verifier { get; set; }

        public string TestPath { get; set; }

        private void Run(string asmFile)
        {
            Cli.Write("Running ~Cyan~{0}~R~... ", Path.GetFileName(asmFile));
            CpuVerifier verifier = new InstructionStreamVerifier();
            verifier = new CpuSoftwareVerifier();
            var asm = File.ReadAllText(asmFile);
            var result = verifier.Verify(asm);

            if (result.Passed)
            {
                Cli.WriteLine("~Green~Passed~R~");
            }
            else
            {
                Cli.WriteLine("~Red~Failed~R~");
                Cli.Dump(result);
            }
        }

        public void Run()
        {
            Cli.WriteHeader(Verifier.GetName(), "~White~~|Blue~");
            var p = PathHelper.GetExecutingPath(TestPath);
            var files = Directory.GetFiles(p, "*.alx", SearchOption.AllDirectories);

            foreach (var f in files)
            {
                Run(f);
            }
        }
    }
}
