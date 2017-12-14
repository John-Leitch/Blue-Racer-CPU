using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class QueueExtension
    {
        public static void EnqueueRange<T>(this Queue<T> queue, IEnumerable<T> items)
        {
            foreach (var i in items)
            {
                queue.Enqueue(i);
            }
        }
    }
}
