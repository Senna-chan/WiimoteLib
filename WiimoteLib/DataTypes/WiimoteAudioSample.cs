using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.DataTypes
{
    /// <summary>
    /// This is a class of the audio sample how the Wiimote wants it
    /// </summary>
    public class WiimoteAudioSample
    {
        public byte[] samples;
        public int length;
        public SpeakerFreq freq;

        /// <summary>
        /// This is to create a object
        /// </summary>
        public WiimoteAudioSample()
        {
            length = 0;
            freq = SpeakerFreq.FREQ_NONE;
        }
        /// <summary>
        /// This will make the object for the wiimote
        /// </summary>
        public WiimoteAudioSample(byte[] samples, int length, SpeakerFreq freq)
        {
            this.samples = samples;
            this.length = length;
            this.freq = freq;
        }
    };
}
