namespace WiiMoteLibUWP.DataTypes.Enums
{
    /// <summary>
	/// The report format in which the Wiimote should return data
	/// </summary>
	public enum InputReport : byte
    {
        /// <summary>
        /// Status report
        /// </summary>
        Status = 0x20,
        /// <summary>
        /// Read data from memory location
        /// </summary>
        ReadData = 0x21,
        /// <summary>
        /// Register write complete
        /// </summary>
        OutputReportAck = 0x22,
        /// <summary>
        /// Button data only
        /// </summary>
        Buttons = 0x30,
        /// <summary>
        /// Button and accelerometer data
        /// </summary>
        ButtonsAccel = 0x31,
        /// <summary>
        /// IR sensor and accelerometer data
        /// </summary>
        IRAccel = 0x33,
        /// <summary>
        /// Button and extension controller data
        /// </summary>
        ButtonsExtension = 0x34,
        /// <summary>
        /// Extension and accelerometer data
        /// </summary>
        ExtensionAccel = 0x35,
        /// <summary>
        /// IR sensor, extension controller and accelerometer data
        /// </summary>
        IRExtensionAccel = 0x37,
    };
}
