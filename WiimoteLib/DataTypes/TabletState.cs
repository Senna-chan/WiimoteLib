using WiimoteLib.DataTypes.Enums;

namespace WiimoteLib.DataTypes
{
    /// <summary>
    /// State of a UDrawTablet
    /// </summary>
    public struct TabletState
    {
        /// <summary>
        /// Position on the tablet x 1860 y 1340 Upper left = 0,0
        /// </summary>
        public Point Position;

        /// <summary>
        /// Position of the box the pen is in
        /// </summary>
        public Point BoxPosition;

        /// <summary>
        /// Position of the pen in a box
        /// </summary>
        public Point RawPosition;
        /// <summary>
        /// Pen pressure
        /// </summary>
        public ushort PenPressure;

        /// <summary>
        /// Type of pressure on the tablet
        /// </summary>
        public TabletPressure PressureType;


        /// <summary>
        /// Tablet buttons
        /// </summary>
        public bool Point, ButtonUp, ButtonDown;
        /// <summary>
        /// Old Tablet Buttons
        /// </summary>
        public bool OldPoint, OldButtonUp, OldButtonDown;
    }
}