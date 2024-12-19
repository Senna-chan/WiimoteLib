using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.Packets
{
    internal class BaseOutputPacket
    {
        protected byte[] data = new byte[Wiimote.REPORT_LENGTH];
        private static bool rumble = false; // Static. All packets need this and we don't want to set it all the time

        public OutputReport OutputReport
        {
            get
            {
                return (OutputReport)data[0];
            }
            set
            {
                data[0] = (byte)value;
            }
        }

        public bool Rumble
        {
            get
            {
                if ((data[1] & 0x01) != 0)
                {
                    return true;
                }
                return false;
            }
            set
            {
                rumble = value;
                data[1] |= (byte)(rumble ? 0x01 : 0x00);
            }
        }

        internal byte[] GetData()
        {
            Rumble = rumble; // Force set the data
            return data;
        }
    }
}
