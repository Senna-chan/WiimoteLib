namespace WiiMoteLibUWP.DataTypes.Enums
{
    /// <summary>
	/// The mode of data reported for the IR sensor
	/// </summary>
	public enum IRMode : byte
    {
        /// <summary>
        /// IR sensor off
        /// </summary>
        Off = 0x00,
        /// <summary>
        /// Basic mode
        /// </summary>
        Basic = 0x01,   // 10 bytes
                        /// <summary>
                        /// Extended mode
                        /// </summary>
        Extended = 0x03,    // 12 bytes
                            /// <summary>
                            /// Full mode (unsupported)
                            /// </summary>
        Full = 0x05,    // 16 bytes * 2 (format unknown)
    };
}
