using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
    /// Point structure for floating point IMU positions (Roll, Pitch, Yaw)
    /// </summary>	
    public struct IMUState
    {
        /// <summary>
        /// Roll, Pitch, Yaw positions
        /// </summary>
        public float Roll, Pitch, Yaw;

        /// <summary>
        /// Convert to human-readable string
        /// </summary>
        /// <returns>A string that represents the point</returns>
        public override string ToString()
        {
            return string.Format("{{Roll={0}, Pitch={1}, Yaw={2}}}", Roll, Pitch, Yaw);
        }

    }
}
