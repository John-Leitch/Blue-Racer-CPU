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
            string includeFile = null,
                testFile = null,
                resultFile = null;

            try
            {
                asm = "#'SoftwareTestHeader';\r\n" + asm + "\r\n#'SoftwareTestFooter';\r\n";
                var instructions = new AphidAssembler().Assemble(asm);
                includeFile = Path.GetFullPath(Path.GetRandomFileName() + ".v");
                new AphidAssembler().AssembleToVerilog(asm, includeFile);

                var testTemplate = new McuTestTemplate()
                {
                    ProgramVerilogFile = includeFile.Replace("\\", "\\\\"),
                    TestCount = GetTestCount(asm),
                    InstructionCompleteCases = GetInstructionCompleteCases(asm),
                };

                var test = testTemplate.ToString();

                testFile = Path.Combine(
                    ModelSimAutomatorSettings.GetWorkingPath(),
                    "McuTest-auto.v");

                File.WriteAllText(testFile, test);

                resultFile = Path.GetFullPath(Path.GetRandomFileName() + ".txt");

                var settings = new ModelSimSettings()
                {
                    WorkingPath = PathHelper.UseForwardSlashes(ModelSimAutomatorSettings.GetWorkingPath()),
                    VerilogFiles = new[] { PathHelper.UseForwardSlashes(testFile) },
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
                    Output = PathHelper.UseForwardSlashes(resultFile),
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
            finally
            {
                foreach (var s in new[]
                {
                    includeFile,
                    testFile,
                    resultFile
                })
                {
                    if (File.Exists(s))
                    {
                        File.Delete(s);
                    }
                }
            }
        }
    }
}
