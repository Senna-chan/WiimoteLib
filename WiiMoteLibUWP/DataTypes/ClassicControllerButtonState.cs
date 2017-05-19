namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// Curernt button state of the Classic Controller
	/// </summary>
	public struct ClassicControllerButtonState
    {
        /// <summary>
        /// Digital button on the Classic Controller extension
        /// </summary>
        public bool A, B, Plus, Home, Minus, Up, Down, Left, Right, X, Y, ZL, ZR;
        /// <summary>
        /// Analog trigger - false if released, true for any pressure applied
        /// </summary>
        public bool TriggerL, TriggerR;
    }
}
