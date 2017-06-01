using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.Helpers
{
    class ConvertHelpers
    {
        /// <summary>
        /// Converts a int to Little Endian byte array
        /// </summary>
        /// <param name="data">The int to convert</param>
        /// <returns></returns>
        public static byte[] INT2LE(int data)
        {
            byte[] b = new byte[4];
            b[0] = (byte)data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return b;
        }
        /// <summary>
        /// Converts the sample rate given to what the wiiremote excpects
        /// </summary>
        /// <param name="sampleRate">The samplerate in Hertz</param>
        /// <returns></returns>
        public static int AdpcmToWiimoteRate(int sampleRate)
        {
            return 6000000 / sampleRate;
        }

        public static int Min(int r, int g, int b)
        {
            return Math.Min(Math.Min(r, g), b);
        }
    }
}
