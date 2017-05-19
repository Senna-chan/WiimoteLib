namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// The 4 sensors on the Balance Board (short values)
	/// </summary>
	public struct BalanceBoardSensors
    {
        /// <summary>
        /// Sensor at top right
        /// </summary>
        public short TopRight;
        /// <summary>
        /// Sensor at top left
        /// </summary>
        public short TopLeft;
        /// <summary>
        /// Sensor at bottom right
        /// </summary>
        public short BottomRight;
        /// <summary>
        /// Sensor at bottom left
        /// </summary>
        public short BottomLeft;
    }
}
