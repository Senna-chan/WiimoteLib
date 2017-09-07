using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.Helpers
{
    public static class ExtensionMethods
    {
        public static int Map(this int value, int fromSource, int toSource, int fromTarget, int toTarget)
        {
            return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
        }
    }
}
