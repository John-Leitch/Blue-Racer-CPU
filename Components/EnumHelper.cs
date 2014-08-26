using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Components
{
    public static class EnumHelper
    {
        public static TEnum[] GetValues<TEnum>()
        {
            return (TEnum[])Enum.GetValues(typeof(TEnum));
        }
    }
}
