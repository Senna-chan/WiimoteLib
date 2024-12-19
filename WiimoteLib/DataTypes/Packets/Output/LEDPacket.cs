using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using WiimoteLib.Packets;

namespace WiimoteLib.DataTypes.Packets.Output
{
    internal class LEDPacket : BaseOutputPacket
    {
        /*
         * //mBuff[0] = (byte)OutputReport.LEDs;
            //mBuff[1] =	(byte)(
            //            (led1 ? 0x10 : 0x00) |
            //            (led2 ? 0x20 : 0x00) |
            //            (led3 ? 0x40 : 0x00) |
            //            (led4 ? 0x80 : 0x00) |
            //            GetRumbleBit());
        */
        public LEDPacket()
        {
            OutputReport = Enums.OutputReport.LEDs;
        }

        public LEDState LEDState { 
            get
            {
                LEDState ledState;
                ledState.LED1 = (data[1] & 0x10) != 0;
                ledState.LED2 = (data[1] & 0x20) != 0;
                ledState.LED3 = (data[1] & 0x40) != 0;
                ledState.LED4 = (data[1] & 0x80) != 0;
                return ledState;
            }
            set
            {
                data[1] = (byte)(
                        (value.LED1 ? 0x10 : 0x00) |
                        (value.LED2 ? 0x20 : 0x00) |
                        (value.LED3 ? 0x40 : 0x00) |
                        (value.LED4 ? 0x80 : 0x00));
            }
        }
    }
}
