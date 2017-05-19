using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current state of LEDs
	/// </summary>
	public struct LEDState
    {
        /// <summary>
        /// LED on the Wiimote
        /// </summary>
        public bool LED1, LED2, LED3, LED4;
    }
}
