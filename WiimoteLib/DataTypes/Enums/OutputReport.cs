using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes.Enums
{
    public enum OutputReport : byte
    {
        LEDs = 0x11,
        Type = 0x12,
        IR = 0x13,
        SpeakerEnable = 0x14,
        Status = 0x15,
        WriteMemory = 0x16,
        ReadMemory = 0x17,
        SpeakerData = 0x18,
        SpeakerMute = 0x19,
        IR2 = 0x1a,
    }
}
