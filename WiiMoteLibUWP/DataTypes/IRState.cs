using WiiMoteLibUWP.DataTypes.Enums;

namespace WiiMoteLibUWP.DataTypes
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
        /// Raw midpoint of IR sensors 1 and 2 only.  Values range between 0 - 1023, 0 - 767
        /// </summary>
        public Point RawMidpoint;
        /// <summary>
        /// Normalized midpoint of IR sensors 1 and 2 only.  Values range between 0.0 - 1.0
        /// </summary>
        public PointF Midpoint;
    }
}
