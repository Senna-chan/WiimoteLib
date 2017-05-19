namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// Current state of the accelerometers
	/// </summary>
	public struct AccelState
    {
        /// <summary>
        /// Raw accelerometer data.
        /// <remarks>Values range between 0 - 255</remarks>
        /// </summary>
        public Point3 RawValues;
        /// <summary>
        /// Normalized accelerometer data.  Values range between 0 - ?, but values > 3 and &lt; -3 are inaccurate.
        /// </summary>
        public Point3F Values;
    }
}
