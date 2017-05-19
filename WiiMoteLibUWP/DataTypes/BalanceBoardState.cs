using WiiMoteLibUWP.DataTypes.CalibrationInfo;

namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// Current state of the Wii Fit Balance Board controller
	/// </summary>
	public struct BalanceBoardState
    {
        /// <summary>
        /// Calibration information for the Balance Board
        /// </summary>
        public BalanceBoardCalibrationInfo CalibrationInfo;
        /// <summary>
        /// Raw values of each sensor
        /// </summary>
        public BalanceBoardSensors SensorValuesRaw;
        /// <summary>
        /// Kilograms per sensor
        /// </summary>
        public BalanceBoardSensorsF SensorValuesKg;
        /// <summary>
        /// Pounds per sensor
        /// </summary>
        public BalanceBoardSensorsF SensorValuesLb;
        /// <summary>
        /// Total kilograms on the Balance Board
        /// </summary>
        public float WeightKg;
        /// <summary>
        /// Total pounds on the Balance Board
        /// </summary>
        public float WeightLb;
        /// <summary>
        /// Center of gravity of Balance Board user
        /// </summary>
        public PointF CenterOfGravity;
    }
}
