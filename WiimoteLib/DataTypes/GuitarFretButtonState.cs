using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
    /// Current fret button state of the Guitar controller
    /// </summary>
    public struct GuitarFretButtonState
    {
        /// <summary>
        /// Fret buttons
        /// </summary>
        public bool Green, Red, Yellow, Blue, Orange;
    }
}
