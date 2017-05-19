namespace WiiMoteLibUWP.DataTypes
{
    /// <summary>
	/// Point structure for int 3D positions (X, Y, Z)
	/// </summary>
	public struct Point3
    {
        /// <summary>
        /// X, Y, Z coordinates of this point
        /// </summary>
        public int X, Y, Z;

        /// <summary>
        /// Convert to human-readable string
        /// </summary>
        /// <returns>A string that represents the point.</returns>
        public override string ToString()
        {
            return string.Format("{{X={0}, Y={1}, Z={2}}}", X, Y, Z);
        }
    }
}
