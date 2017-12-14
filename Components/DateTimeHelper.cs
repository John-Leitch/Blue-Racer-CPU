using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class DateTimeHelper
    {
        private static DateTime _epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static DateTime FromEpoch(uint time)
        {
            return _epoch.AddSeconds(time);
        }

        public static uint ToEpoch(this DateTime dateTime)
        {
            return (uint)(dateTime - _epoch).TotalSeconds;
        }
    }
}
