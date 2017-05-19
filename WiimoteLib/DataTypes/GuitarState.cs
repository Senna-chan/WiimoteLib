using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current state of the Guitar controller
	/// </summary>
	public struct GuitarState
    {
        /// <summary>
        /// Guitar type
        /// </summary>
        public GuitarType GuitarType;
        /// <summary>
        /// Current button state of the Guitar
        /// </summary>
        public GuitarButtonState ButtonState;
        /// <summary>
        /// Current fret button state of the Guitar
        /// </summary>
        public GuitarFretButtonState FretButtonState;
        /// <summary>
        /// Current touchbar state of the Guitar
        /// </summary>
        public GuitarFretButtonState TouchbarState;
        /// <summary>
        /// Raw joystick position.  Values range between 0 - 63.
        /// </summary>
        public Point RawJoystick;
        /// <summary>
        /// Normalized value of joystick position.  Values range between 0.0 - 1.0.
        /// </summary>
        public PointF Joystick;
        /// <summary>
        /// Raw whammy bar position.  Values range between 0 - 10.
        /// </summary>
        public byte RawWhammyBar;
        /// <summary>
        /// Normalized value of whammy bar position.  Values range between 0.0 - 1.0.
        /// </summary>
        public float WhammyBar;
    }
}
