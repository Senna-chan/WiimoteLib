using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
	/// <summary>
	/// Point structure for floating point 2D positions (X, Y)
	/// </summary>
	public struct PointF
	{
		/// <summary>
		/// X, Y coordinates of this point
		/// </summary>
		public double X, Y;

		/// <summary>
		/// Convert to human-readable string
		/// </summary>
		/// <returns>A string that represents the point</returns>
		public override string ToString()
		{
			return string.Format("{{X={0}, Y={1}}}", X, Y);
		}

	}
}
