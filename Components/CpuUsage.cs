using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public class CpuUsage
    {
        private TimeSpan _lastTime;

        private DateTime _lastRefresh;

        public Process Process { get; private set; }

        public double Value { get; private set; }

        public CpuUsage(Process process)
        {
            Process = process;
        }

        public double NextValue()
        {
            double usage = 0;
            var now = DateTime.Now;

            if (_lastTime.TotalMilliseconds != 0)
            {
                var delta = (now - _lastRefresh).Ticks;
                var cpuDelta = (Process.TotalProcessorTime - _lastTime).Ticks;
                usage = (float)cpuDelta / delta / Environment.ProcessorCount * 100;
            }

            _lastTime = Process.TotalProcessorTime;
            _lastRefresh = now;

            return Value = usage;
        }
    }
}
