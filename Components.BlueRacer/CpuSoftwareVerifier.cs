using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class CpuSoftwareVerifier : CpuVerifier
    {
        public override string GetName()
        {
            return "Software Verifier";
        }

        protected override int GetTestCount(string asm)
        {
            return 1;
        }

        protected override Dictionary<int, string> GetInstructionCompleteCases(string asm)
        {
            return new Dictionary<int, string>();
        }

        protected override VerificationResult CreateResult(
            string asm, 
            string list, 
            Dictionary<int, string> testValues, 
            CpuErrorCode error)
        {
            return new VerificationResult(error == CpuErrorCode.None, list, error);            
        }
    }
}
