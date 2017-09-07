using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.DataTypes
{
    /// <summary>
    /// Current state of the wiimote internal speaker (Experimental)
    /// </summary>
    public class SpeakerState
    {
        /// <summary>
        /// Speaker muted
        /// </summary>
        public bool Muted = true;
        /// <summary>
        /// Speaker on(Different then muted)
        /// </summary>
        public bool Enabled = false;
        /// <summary>
        /// Speaker volume. Min = 0x00, Max = 0x40 (We run in 4 bit ADPCM mode for better sound)
        /// </summary>
        public int Volume = 0;
        /// <summary>
        /// Speaker frequency range is 1 to 5 kHz(Need to check this)
        /// </summary>
        public SpeakerFreq Frequency = SpeakerFreq.FREQ_NONE;
    }
}
