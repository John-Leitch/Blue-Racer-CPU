using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public abstract class CpuVerifier
    {
        public abstract string GetName();

        protected abstract int GetTestCount(string asm);

        protected abstract Dictionary<int, string> GetInstructionCompleteCases(string asm);

        protected abstract VerificationResult CreateResult(string asm, string list, Dictionary<int, string> testValues, CpuErrorCode error);

        public VerificationResult Verify(string asm)
        {
            asm = "#'SoftwareTestHeader';\r\n" + asm + "\r\n#'SoftwareTestFooter';\r\n";

            var instructions = new AphidAssembler().Assemble(asm);

            new AphidAssembler().AssembleToVerilog(asm, @"c:\temp\test.v");

            var testTemplate = new McuTestTemplate()
            {
                ProgramVerilogFile = @"c:\temp\test.v".Replace("\\", "\\\\"),
                TestCount = GetTestCount(asm),
                InstructionCompleteCases = GetInstructionCompleteCases(asm),
            };

            var test = testTemplate.ToString();
            var mcuTest = @"C:\altera\13.1\McuTest-auto.v";
            File.WriteAllText(mcuTest, test);

            var settings = new ModelSimSettings()
            {
                WorkingPath = "c:/altera/13.1",
                VerilogFiles = new[] { mcuTest.Replace('\\', '/') },
                TestModule = "McuTest2",
                Signals = Enumerable
                    .Range(0, testTemplate.TestCount)
                    .Select(x => "/McuTest2/test" + x)
                    .Concat(new[] 
                    { 
                        "/McuTest2/testComplete",
                        "/McuTest2/error", 
                        "/McuTest2/errorCode",
                    })
                    .ToArray(),
                RunTime = 200000,
                Output = "c:/temp/results.txt",
            };

            var automator = new ModelSimAutomator();
            var list = automator.Execute(settings);

            var results = list
                .SplitLines(StringSplitOptions.RemoveEmptyEntries)
                .Last()
                .Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .Skip(2);

            var testValues = results
                .Select((x, i) => new
                {
                    Key = i,
                    Value = x,
                })
                .Take(results.Count() - 2)
                .ToDictionary(x => x.Key, x => x.Value);

            var errorInfo = results.Skip(results.Count() - 2).ToArray();
            var hasError = errorInfo[0] == "1";
            var error = (CpuErrorCode)uint.Parse(errorInfo[1], System.Globalization.NumberStyles.HexNumber);

            return CreateResult(asm, list, testValues, error);
        }
    }
}
