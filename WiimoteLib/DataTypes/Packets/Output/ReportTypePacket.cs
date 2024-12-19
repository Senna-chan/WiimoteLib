using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Packets;

namespace WiimoteLib.DataTypes.Packets.Output
{
    internal class ReportTypePacket : OnOffPacket
    {
        /*
         * 
                    mBuff[0] = (byte)OutputReport.Type;
                    mBuff[1] = (byte)((continuous ? 0x04 : 0x00) | (byte)(mWiimoteState.Rumble ? 0x01 : 0x00));
                    mBuff[2] = (byte)type;
        */
        public ReportTypePacket() : base(Enums.OutputReport.Type) { }
        
        public InputReport type
        {
            get
            {
                return (InputReport)data[2];
            }
            set
            {
                data[2] = (byte)value;
            }
        }
    }
}
