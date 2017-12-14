using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class Entropy
    {
        public static double CalculateByteEntropy(byte[] buffer)
        {
            return buffer
                .GroupBy(x => x)
                .Select(x => (double)x.Count() / buffer.Length)
                .Select(x => (x * (Math.Log(x) / Math.Log(2))) * -1)
                .Sum();
        }
    }
}
