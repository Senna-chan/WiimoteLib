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
    /// Enum that holds speaker frequencies that are calibrated to a specific wiimote
    /// </summary>
    public enum SpeakerFreq
    {
        // (keep in sync with FreqLookup in wiimote.cpp)
        FREQ_NONE = 0,
        // my PC can't keep up with these using bUseHIDwrite, so I haven't
        //  been able to tune them yet
        FREQ_4200HZ = 1,
        FREQ_3920HZ = 2,
        FREQ_3640HZ = 3,
        FREQ_3360HZ = 4,
        // these were tuned until the square-wave was glitch-free on my remote -
        //  may not be exactly right
        FREQ_3130HZ = 5,    // +190
        FREQ_2940HZ = 6,    // +180
        FREQ_2760HZ = 7,    // +150
        FREQ_2610HZ = 8,    // +140
        FREQ_2470HZ = 9,
    };
}
