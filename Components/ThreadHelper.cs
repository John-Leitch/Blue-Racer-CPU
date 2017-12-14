using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Components
{
    public static class ThreadHelper
    {
        public static Thread DoWhile(Func<bool> condition, Action action, int sleepTime, ManualResetEvent finishReset)
        {
            var t = new Thread(() =>
            {
                while (condition())
                {
                    action();
                    Thread.Sleep(sleepTime);
                }

                if (finishReset != null)
                {
                    finishReset.Set();
                }
            }) { IsBackground = true };

            t.Start();

            return t;
        }

        public static Thread DoWhile(Func<bool> condition, Action action, int sleepTime)
        {
            return DoWhile(condition, action, sleepTime, null);
        }

        public static Thread DoBackground(ThreadStart action)
        {
            var t = new Thread(action) { IsBackground = true };
            t.Start();

            return t;
        }

        public static Thread Do(ThreadStart action)
        {
            var t = new Thread(action);
            t.Start();
            
            return t;
        }

        public static void For(int threads, Action action)
        {
            var resets = new List<ManualResetEvent>();

            for (int i = 0; i < threads; i++)
            {
                var r = new ManualResetEvent(false);
                resets.Add(r);

                ThreadHelper.Do(() =>
                {
                    action();
                    r.Set();
                });
            }

            foreach (var r in resets)
            {
                r.WaitOne();
            }
        }
    }
}
