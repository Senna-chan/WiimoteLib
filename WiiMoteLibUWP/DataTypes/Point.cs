namespace WiiMoteLibUWP.DataTypes
{
	/// <summary>
	/// Point structure for int 2D positions (X, Y)
	/// </summary>	
	public struct Point
	{
		/// <summary>
		/// X, Y coordinates of this point
		/// </summary>
		public int X, Y;

		/// <summary>
		/// Convert to human-readable string
		/// </summary>
		/// <returns>A string that represents the point.</returns>
		public override string ToString()
		{
			return string.Format("{{X={0}, Y={1}}}", X, Y);
		}
	}
}
