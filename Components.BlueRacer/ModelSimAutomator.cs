using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public class ModelSimAutomator
    {
        public string Exe { get; set; }

        public ModelSimAutomator()
        {
            Exe = @"C:\altera\13.1\modelsim_ase\win32aloem\vsim.exe";
        }

        public string Execute(ModelSimSettings settings)
        {
            var originalDir = Directory.GetCurrentDirectory();
            string doFile = null, outputFile = null;

            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Exe));

                doFile = Path
                    .GetFullPath(Path.GetRandomFileName() + ".do")
                    .Replace('\\', '/');

                outputFile = Path
                    .GetFullPath(Path.GetRandomFileName() + ".txt")
                    .Replace('\\', '/');

                settings.Output = outputFile;

                var script = settings.CreateScript();
                File.WriteAllText(doFile, script);

                var p = Process.Start(Exe, string.Format("-do \"{0}\"", doFile));
                p.WaitForExit();
                return File.ReadAllText(outputFile.Replace('/', '\\'));
            }
            finally
            {
                foreach (var f in new [] { doFile, outputFile })
                {
                    if (f != null && File.Exists(f))
                    {
                        try
                        {
                            File.Delete(f);
                        }
                        catch { }
                    }
                }

                Directory.SetCurrentDirectory(originalDir);
            }
            //var list = ModelSimList.Parse(File.ReadAllText(outputFile.Replace('/', '\\')));
            
            //return list;
        }
    }
}
