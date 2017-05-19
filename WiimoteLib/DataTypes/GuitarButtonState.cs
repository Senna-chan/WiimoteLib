using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current button state of the Guitar controller
	/// </summary>
	public struct GuitarButtonState
    {
        /// <summary>
        /// Strum bar
        /// </summary>
        public bool StrumUp, StrumDown;
        /// <summary>
        /// Other buttons
        /// </summary>
        public bool Minus, Plus;
    }
}
