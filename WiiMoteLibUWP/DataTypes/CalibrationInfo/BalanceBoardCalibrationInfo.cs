namespace WiiMoteLibUWP.DataTypes.CalibrationInfo
{
    /// <summary>
	/// Calibration information
	/// </summary>
	public struct BalanceBoardCalibrationInfo
    {
        /// <summary>
        /// Calibration information at 0kg
        /// </summary>
        public BalanceBoardSensors Kg0;
        /// <summary>
        /// Calibration information at 17kg
        /// </summary>
        public BalanceBoardSensors Kg17;
        /// <summary>
        /// Calibration information at 34kg
        /// </summary>
        public BalanceBoardSensors Kg34;
    }
}
