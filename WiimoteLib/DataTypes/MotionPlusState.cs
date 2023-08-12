using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.CalibrationInfo;

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
        /// Calibration values for fast mode
        /// </summary>
        public MotionPlusCalibrationInfo FastCalibration;
        /// <summary>
        /// Calibration values for slow mode
        /// </summary>
        public MotionPlusCalibrationInfo SlowCalibration;
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
        /// <summary>
        /// IMU State for fusion stuff
        /// </summary>
        public IMUState IMU;
        /// <summary>
        /// This holds the time between samples(For the fusion code)
        /// </summary>
        public long LastMillis;
    }
}
