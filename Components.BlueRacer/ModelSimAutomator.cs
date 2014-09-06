using System;
using System.Collections.Generic;
using System.Configuration;
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
            Exe = ModelSimAutomatorSettings.GetExePath();
        }

        public string Execute(ModelSimSettings settings)
        {
            var originalDir = Directory.GetCurrentDirectory();
            string doFile = null, outputFile = null;

            try
            {
                Directory.SetCurrentDirectory(Path.GetDirectoryName(Exe));
                doFile = PathHelper.UseForwardSlashes(Path.GetFullPath(Path.GetRandomFileName() + ".do"));
                outputFile = PathHelper.UseForwardSlashes(Path.GetFullPath(Path.GetRandomFileName() + ".txt"));
                settings.Output = outputFile;
                var script = settings.CreateScript();
                File.WriteAllText(doFile, script);
                var p = Process.Start(Exe, string.Format("-do \"{0}\"", doFile));
                p.WaitForExit();

                return File.ReadAllText(PathHelper.UseForwardSlashes(outputFile));
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
        }
    }
}
