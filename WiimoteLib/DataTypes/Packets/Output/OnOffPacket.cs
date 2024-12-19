using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.Packets
{
    internal class OnOffPacket : BaseOutputPacket
    {
        public OnOffPacket(OutputReport reportType)
        {
            OutputReport = reportType;
        }

        public bool isOn
        {
            get
            {
                return (data[1] & 0x04) != 0;
            }
            set
            {
                data[1] |= (byte)(value ? 0x04 : 0x00);
            }
        }
    }
}
