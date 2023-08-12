using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current state of the IR camera
	/// </summary>
	public struct IRState
    {
        /// <summary>
        /// Current mode of IR sensor data
        /// </summary>
        public IRMode Mode;
        /// <summary>
        /// Current state of IR sensors
        /// </summary>
        public IRSensor[] IRSensors;
        /// <summary>
        /// Normalized Sensor made of data from IR sensors 1 and 2 only.
        /// </summary>
        public IRSensor Midpoint;

        /// <summary>
        /// Current sensitivity of IR sensor
        /// </summary>
        public IRSensitivity Sensitivity;
    }
}
