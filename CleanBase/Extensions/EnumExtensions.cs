using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanBase.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum ParseEnum<TEnum>(this string data) where TEnum:struct
        {
            Enum.TryParse(data, true, out TEnum cachingProvider);
            return cachingProvider;
        }
    }
}
