using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
	/// <summary>
	/// Calibration information stored on the Nunchuk
	/// </summary>
	public struct NunchukCalibrationInfo
	{
		/// <summary>
		/// Accelerometer calibration data
		/// </summary>
		public AccelCalibrationInfo AccelCalibration;
		/// <summary>
		/// Joystick X-axis calibration
		/// </summary>
		public byte MinX, MidX, MaxX;
		/// <summary>
		/// Joystick Y-axis calibration
		/// </summary>
		public byte MinY, MidY, MaxY;
	}
}
