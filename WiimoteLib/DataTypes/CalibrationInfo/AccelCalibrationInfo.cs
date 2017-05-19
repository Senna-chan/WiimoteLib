using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Accelerometer calibration information
	/// </summary>
	public struct AccelCalibrationInfo
    {
        /// <summary>
        /// Zero point of accelerometer
        /// </summary>
        public byte X0, Y0, Z0;
        /// <summary>
        /// Gravity at rest of accelerometer
        /// </summary>
        public byte XG, YG, ZG;
    }
}
