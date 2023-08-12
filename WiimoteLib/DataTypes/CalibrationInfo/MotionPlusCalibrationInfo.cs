using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes.CalibrationInfo
{
    public struct MotionPlusCalibrationInfo
    {
        /// <summary>
        /// Zero point of gyro at 0 deg/sec
        /// </summary>
        public short Y0, R0, P0;
        /// <summary>
        /// Scale of the gyro
        /// </summary>
        public short YS, RS, PS;
        /// <summary>
        /// Angular velocity of the above scale values in deg/sec divided by 6.
        /// e.g. A value of 45 would mean the "scale" values represent the gyro at 270 deg/sec.
        /// </summary>
        public byte ScaleReference;
    }
}
