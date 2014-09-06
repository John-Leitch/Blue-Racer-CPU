using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components.BlueRacer
{
    public static class ModelSimAutomatorSettings
    {
        public const string ExeKey = "ModelSim", 
            WorkingPathKey = "WorkingPath", 
            LpmFileKey = "LpmFile";

        private static string GetPath(string key)
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(key) ?
                Path.GetFullPath(ConfigurationManager.AppSettings[key]) :
                null;
        }

        public static string GetExePath()
        {
            return GetPath(ExeKey);
        }

        public static string GetWorkingPath()
        {
            return GetPath(WorkingPathKey);
        }

        public static string GetLpmFile()
        {
            return GetPath(LpmFileKey);
        }
    }
}
