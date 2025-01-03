using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.Helpers
{
    public static class StringExtensions
    {
        public static string ToHexString(this byte[] bytes)
        {

            StringBuilder sb = new StringBuilder(bytes.Length * 5);
            foreach (byte b in bytes)
            {
                sb.Append("0x");
                sb.Append(b.ToString("X2"));
                sb.Append(',');
            }

            return sb.ToString();
        }
        
    }
}
