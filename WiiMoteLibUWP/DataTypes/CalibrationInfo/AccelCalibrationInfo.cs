namespace WiiMoteLibUWP.DataTypes.CalibrationInfo
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
