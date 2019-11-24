using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RedisCache.Helpers
{
    public class EncodingHelper
    {
        public static string GetStringFromByteArray(byte[] input)
        {
            return Encoding.UTF8.GetString(input).Replace("\"", "");
        }
    }
}
