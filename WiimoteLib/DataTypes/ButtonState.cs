using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current button state
	/// </summary>
	public struct ButtonState
    {
        /// <summary>
        /// Digital button on the Wiimote
        /// </summary>
        public bool A, B, Plus, Home, Minus, One, Two, Up, Down, Left, Right;

        
    }
}
