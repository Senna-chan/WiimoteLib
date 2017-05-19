using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current state of the Nunchuk extension
	/// </summary>	
	public struct NunchukState
    {
        /// <summary>
        /// Calibration data for Nunchuk extension
        /// </summary>
        public NunchukCalibrationInfo CalibrationInfo;
        /// <summary>
        /// State of accelerometers
        /// </summary>
        public AccelState AccelState;
        /// <summary>
        /// Raw joystick position before normalization.  Values range between 0 and 255.
        /// </summary>
        public Point RawJoystick;
        /// <summary>
        /// Normalized joystick position.  Values range between -0.5 and 0.5
        /// </summary>
        public PointF Joystick;
        /// <summary>
        /// Digital button on Nunchuk extension
        /// </summary>
        public bool C, Z;
    }
}
