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
        /// Motion Plus
        /// </summary>
        MotionPlusNunchuck = 0x0000A4200505,
        /// <summary>
        /// Motion Plus
        /// </summary>
        MotionPlusClassicController = 0x0000A4200705,
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
        /// UDraw tablet
        /// </summary>
        UDraw = 0xFF00A4200112,
        /// <summary>
        /// Partially inserted extension.  This is an error condition.
        /// </summary>
        ParitallyInserted = 0xffffffffffff,
        /// <summary>
        /// Unknown extension. This will enable raw extension input report printing
        /// </summary>
        Unknown = 0x01
    };
}
