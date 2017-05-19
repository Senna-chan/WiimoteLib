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
	public struct MotionPlusState
    {
        /// <summary>
        /// Raw gyro values
        /// </summary>
        public Point3 GyroRaw;
        /// <summary>
        /// Gyro offset for calibration
        /// </summary>
        public Point3 Offset;
        /// <summary>
        /// Gyro values in deg/s
        /// </summary>
        public Point3F Gyro;
        /// <summary>
        /// Slow pitch movement
        /// </summary>
        public bool SlowPitch;
        /// <summary>
        /// Slow roll movement
        /// </summary>
        public bool SlowRoll;
        /// <summary>
        /// Slow yaw movement
        /// </summary>
        public bool SlowYaw;
        /// <summary>
        /// Passthroughmode enabled or not
        /// </summary>
        public bool PassThroughMode;
        

    }
}
