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
        public const string ExeKey = "ModelSim", WorkingPathKey = "WorkingPath";

        public static string GetExePath()
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(ExeKey) ? 
                Path.GetFullPath(ConfigurationManager.AppSettings[ExeKey]) : 
                null;
        }

        public static string GetWorkingPath()
        {
            return ConfigurationManager.AppSettings.AllKeys.Contains(WorkingPathKey) ?
                Path.GetFullPath(ConfigurationManager.AppSettings[WorkingPathKey]) : 
                null;
        }
    }
}
