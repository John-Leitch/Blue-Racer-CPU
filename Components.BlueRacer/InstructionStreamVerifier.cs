using Components.Aphid.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class InstructionStreamVerifier : CpuVerifier
    {
        public override string GetName()
        {
            return "Instruction Stream Verifier";
        }

        private string CreateTestCase(int number, string statement, bool isLast)
        {
            var lastStatement = isLast ? "\r\n        testComplete <= 1;" : "";

            var template = @"{0}{1}";

            return string.Format(template, statement, lastStatement);
        }

        private string CreateInstructionCheck(int number, byte[] instruction)
        {
            return string.Format(
                "test{0} <= instructionBuffer == 40'h{1};",
                number,
                ByteHelper.ToHex(instruction));
        }

        private string CreateInstructionCheckTestCase(int number, byte[] instruction, bool isLast)
        {
            return CreateTestCase(number, CreateInstructionCheck(number, instruction), isLast);
        }

        protected override int GetTestCount(string asm)
        {
            return new AphidAssembler().Assemble(asm).Count;
        }

        protected override Dictionary<int, string> GetInstructionCompleteCases(string asm)
        {
            var instructions = new AphidAssembler().Assemble(asm);

            var testCases = instructions
                .Select((x, i) => new
                {
                    Key = i,
                    Value = CreateInstructionCheckTestCase(i, x, i == instructions.Count - 1).Trim()
                })
                .ToDictionary(x => x.Key, x => x.Value);

            return testCases;
        }

        protected override VerificationResult CreateResult(
            string asm, 
            string list, 
            Dictionary<int, string> testValues, 
            CpuErrorCode error)
        {
            var ast = AphidParser.Parse(asm);

            if (testValues.Any(x => x.Value == "x"))
            {
                var r = testValues.Last(x => x.Value == "1");

                var exp = ast[r.Key];

                return new VerificationResult(
                    false,
                    string.Format("Instruction hung: {0}", exp),
                    list,
                    error);
            }
            
            return new VerificationResult(testValues.All(x => x.Value == "1"), list, error);
        }        
    }
}
