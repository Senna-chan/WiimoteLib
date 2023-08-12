using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.Helpers
{
    /// <summary>
    /// Provides some methods that are Arduino like(I really like those functions)
    /// </summary>
    public static class ExtensionMethods
    {
        public static int Map(this int value, int inMin, int inMax, int outMin, int outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
        public static byte Map(this byte value, byte inMin, byte inMax, byte outMin, byte outMax)
        {
            return (byte) ((value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin);
        }
        public static decimal Map(this decimal value, decimal inMin, decimal inMax, decimal outMin, decimal outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }
        public static float Map(this float value, float inMin, float inMax, float outMin, float outMax)
        {
            return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
        }


        public static int Constrain(this int value, int min, int max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        public static byte Constrain(this byte value, byte min, byte max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        public static decimal Constrain(this decimal value, decimal min, decimal max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }
        public static float Constrain(this float value, float min, float max)
        {
            if (value > max) return max;
            if (value < min) return min;
            return value;
        }

    }
}
