using Components.BlueRacer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuVerifier
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var runner in new[]
            {
                new CpuVerificationRunner()
                {
                    Verifier = new InstructionStreamVerifier(),
                    TestPath = @"InstructionSeqTests",
                },
                new CpuVerificationRunner()
                {
                    Verifier = new CpuSoftwareVerifier(),
                    TestPath = @"SoftwareTests",
                }
            })
            {
                runner.Run();
            }
        }
    }
}
