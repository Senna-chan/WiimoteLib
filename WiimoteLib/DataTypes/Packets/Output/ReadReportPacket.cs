using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Packets;

namespace WiimoteLib.DataTypes.Packets.Output
{
    internal class ReadReportPacket : BaseOutputPacket
    {
        public ReadReportPacket()
        {
            OutputReport = Enums.OutputReport.ReadMemory;
        }
        /*
            mBuff[0] = (byte)OutputReport.ReadMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | GetRumbleBit());
            mBuff[2] = (byte)((address & 0x00ff0000)  >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00)  >>  8);
            mBuff[4] = (byte)(address & 0x000000ff);

            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);
         */

        public int Address
        {
            get
            {
                int addr = 0;
                addr |= data[1] << 24;
                addr |= data[2] << 16;
                addr |= data[3] << 8;
                addr |= data[4];
                return addr;
            }
            set
            {
                data[1] |= (byte)(((value & 0xff000000) >> 24));
                data[2] = (byte)((value & 0x00ff0000) >> 16);
                data[3] = (byte)((value & 0x0000ff00) >> 8);
                data[4] = (byte)(value & 0x000000ff);
            }
        }
        public short Size
        {
            //mBuff [5] = (byte) ((size & 0xff00) >> 8);
            //mBuff[6] = (byte) (size & 0xff);
            get
            {
                return (short)(((short)data[5] << 8) | data[6]);
            }
            set
            {
                data[5] = (byte)((value & 0xFF00) >> 8);
                data[6] = (byte)(value & 0xFF);
            }
        }
    }
}
