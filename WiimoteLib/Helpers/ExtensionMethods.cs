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
        public static int Map(this int value, int fromSource, int toSource, int fromTarget, int toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
        public static byte Map(this byte value, byte fromSource, byte toSource, byte fromTarget, byte toTarget)
        {
            return (byte) ((value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget);
        }
        public static decimal Map(this decimal value, decimal fromSource, decimal toSource, decimal fromTarget, decimal toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
        public static float Map(this float value, float fromSource, float toSource, float fromTarget, float toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
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
