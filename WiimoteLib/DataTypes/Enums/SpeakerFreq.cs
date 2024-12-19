using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
// ReSharper disable InconsistentNaming
#pragma warning disable 1591

namespace WiimoteLib.DataTypes.Enums
{
    /// <summary>
    /// Enum that holds speaker frequencies that are tuned
    /// </summary>
    public enum SpeakerFreq
    {
        // (keep in sync with FreqLookup in wiimote.cpp)
        FREQ_NONE = 0,
        // my PC can't keep up with these using bUseHIDwrite, so I haven't
        //  been able to tune them yet
        FREQ_4200HZ = 4200,
        FREQ_3920HZ = 3920,
        FREQ_3640HZ = 3640,
        FREQ_3360HZ = 3360,
        // these were tuned until the square-wave was glitch-free on my remote -
        //  may not be exactly right
        FREQ_3130HZ = 3130,    
        FREQ_2940HZ = 2940,    
        FREQ_2760HZ = 2760,    
        FREQ_2610HZ = 2610,    
        FREQ_2470HZ = 2470,
        FREQ_4410HZ = 4410
    };
}
