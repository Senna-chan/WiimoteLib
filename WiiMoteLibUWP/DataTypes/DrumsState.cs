namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// Current state of the Drums controller
	/// </summary>
	public struct DrumsState
    {
        /// <summary>
        /// Drum pads
        /// </summary>
        public bool Red, Green, Blue, Orange, Yellow, Pedal;
        /// <summary>
        /// Speed at which the pad is hit.  Values range from 0 (very hard) to 6 (very soft)
        /// </summary>
        public int RedVelocity, GreenVelocity, BlueVelocity, OrangeVelocity, YellowVelocity, PedalVelocity;
        /// <summary>
        /// Other buttons
        /// </summary>
        public bool Plus, Minus;
        /// <summary>
        /// Raw value of analong joystick.  Values range from 0 - 15
        /// </summary>
        public Point RawJoystick;
        /// <summary>
        /// Normalized value of analog joystick.  Values range from 0.0 - 1.0
        /// </summary>
        public PointF Joystick;
    }
}
