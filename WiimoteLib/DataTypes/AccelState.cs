using NAudio.CoreAudioApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current state of the accelerometers
	/// </summary>
	public struct AccelState
    {
        /// <summary>
        /// Raw accelerometer data.
        /// <remarks>Values range between 0 - 255</remarks>
        /// </summary>
        public Point3 RawValues;
        /// <summary>
        /// Normalized accelerometer data.  Values range between 0 - ?, but values > 3 and &lt; -3 are inaccurate.
        /// </summary>
        public Point3F Values;
        /// <summary>
        /// Roll and pitch
        /// </summary>
        public IMUState IMU;

        public override string ToString()
        {
            return IMU.ToString();
        }
    }
}
