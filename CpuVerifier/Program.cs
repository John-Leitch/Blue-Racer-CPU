using Components.BlueRacer;
using Components.ConsolePlus;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CpuVerifier
{
    class Program
    {
        static void PrintErrorAnExit(int errorCode, string format, params object[] args)
        {
            Cli.WriteLine(format, args);
            Environment.Exit(errorCode);
        }

        static void CheckSettingSet(int errorCode, string settingName, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                PrintErrorAnExit(errorCode, "\"~Cyan~{0}~R~\" not set; exiting", settingName);
            }
        }

        static void CheckFsoExists(int errorCode, string filename, Func<string, bool> fsoExists)
        {
            if (!fsoExists(filename))
            {
                PrintErrorAnExit(errorCode, "\"~Cyan~{0}~R~\" not found; exiting", filename);
            }
        }

        static void ValidateConfig()
        {
            CheckSettingSet(
                0x10,
                ModelSimAutomatorSettings.ExeKey,
                ModelSimAutomatorSettings.GetExePath());

            CheckFsoExists(
                0x11,
                ModelSimAutomatorSettings.GetExePath(),
                File.Exists);

            CheckSettingSet(
                0x20,
                ModelSimAutomatorSettings.WorkingPathKey,
                ModelSimAutomatorSettings.GetWorkingPath());

            CheckFsoExists(
                0x21,
                ModelSimAutomatorSettings.GetWorkingPath(),
                Directory.Exists);

            CheckSettingSet(
                0x30,
                ModelSimAutomatorSettings.LpmFileKey,
                ModelSimAutomatorSettings.GetLpmFile());

            CheckFsoExists(
                0x31,
                ModelSimAutomatorSettings.GetLpmFile(),
                File.Exists);
        }

        static void Main(string[] args)
        {
            ValidateConfig();

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
