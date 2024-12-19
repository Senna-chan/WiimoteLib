using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes.Enums
{
    internal enum WiimoteRegister : int
    {
       IR = 0x04b00030,
       IR_SENSITIVITY_1 = 0x04b00000,
       IR_SENSITIVITY_2 = 0x04b0001a,
       IR_MODE = 0x04b00033,

       EXTENSION_INIT_1 = 0x04a400f0,
       EXTENSION_INIT_2 = 0x04a400fb,
       EXTENSION_EXT_INIT_1 = 0x04a600f0,
       EXTENSION_EXT_INIT_2 = 0x04a600fb,
       EXTENSION_TYPE = 0x04a400fa,
       EXTENSION_CALIBRATION = 0x04a40020,
       MOTIONPLUS_DETECT = 0x04a600fa,
       MOTIONPLUS_INIT = 0x04a600f0,
       MOTIONPLUS_ENABLE = 0x04a600fe,
       MOTIONPLUS_DISABLE = 0x04a400f0,
    }
}
