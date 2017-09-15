namespace WiimoteLib.DataTypes.Enums
{
    /// <summary>
	/// The extension plugged into the Wiimote
	/// </summary>
	public enum ExtensionType : long
    {
        /// <summary>
        /// No extension
        /// </summary>
        None = 0x000000000000,
        /// <summary>
        /// Motion Plus
        /// </summary>
        MotionPlus = 0x0000a4200405,
        /// <summary>
        /// Nunchuk extension
        /// </summary>
        Nunchuk = 0x0000a4200000,
        /// <summary>
        /// Classic Controller extension
        /// </summary>
        ClassicController = 0x0000a4200101,
        /// <summary>
        /// Guitar controller from Guitar Hero 3/WorldTour
        /// </summary>
        Guitar = 0x0000a4200103,
        /// <summary>
        /// Drum controller from Guitar Hero: World Tour
        /// </summary>
        Drums = 0x0100a4200103,
        /// <summary>
        /// Wii Fit Balance Board controller
        /// </summary>
        BalanceBoard = 0x0000a4200402,
        /// <summary>
        /// Wii Fit Balance Board controller
        /// </summary>
        UDraw = 280378218643730,
        /// <summary>
        /// Partially inserted extension.  This is an error condition.
        /// </summary>
        ParitallyInserted = 0xffffffffffff
    };
}
