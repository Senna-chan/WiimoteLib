using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.Packets;

namespace WiimoteLib.DataTypes.Packets.Output
{
    internal class WriteReportPacket : BaseOutputPacket
    {
        public WriteReportPacket() {
            OutputReport = Enums.OutputReport.WriteMemory;
        }
        /*
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | GetRumbleBit());
            mBuff[2] = (byte)((address & 0x00ff0000)  >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00)  >>  8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = size;
            Array.Copy(buff, 0, mBuff, 6, size);
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
                data[2]  = (byte)((value & 0x00ff0000) >> 16);
                data[3]  = (byte)((value & 0x0000ff00) >> 8);
                data[4]  = (byte)(value & 0x000000ff);
            }
        }
        public byte Size
        {
            get
            {
                return data[5];
            }
            set
            {
                data[5] = value;
            }
        }

        public byte[] WriteData
        {
            get
            {
                if (Size == 0) return new byte[0];
                byte[] wData = new byte[Size];
                Array.Copy(data, 6, wData, 0, Size);
                return wData;
            }
            set
            {
                if (Size != 0)
                {
                    Array.Copy(value, 0, data, 6, Size);
                }
            }
        }
    }
}
