using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// The 4 sensors on the Balance Board (float values)
	/// </summary>
	public struct BalanceBoardSensorsF
    {
        /// <summary>
        /// Sensor at top right
        /// </summary>
        public float TopRight;
        /// <summary>
        /// Sensor at top left
        /// </summary>
        public float TopLeft;
        /// <summary>
        /// Sensor at bottom right
        /// </summary>
        public float BottomRight;
        /// <summary>
        /// Sensor at bottom left
        /// </summary>
        public float BottomLeft;
    }
}
