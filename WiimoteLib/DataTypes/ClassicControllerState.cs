using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
	/// <summary>
	/// Current state of the Classic Controller
	/// </summary>
	public struct ClassicControllerState
	{
		/// <summary>
		/// Calibration data for Classic Controller extension
		/// </summary>
		public ClassicControllerCalibrationInfo CalibrationInfo;
		/// <summary>
		/// Current button state
		/// </summary>
		public ClassicControllerButtonState ButtonState;
		/// <summary>
		/// Raw value of left joystick.  Values range between 0 - 255.
		/// </summary>
		public Point RawJoystickL;
		/// <summary>
		/// Raw value of right joystick.  Values range between 0 - 255.
		/// </summary>
		public Point RawJoystickR;
		/// <summary>
		/// Normalized value of left joystick.  Values range between -0.5 - 0.5
		/// </summary>
		public PointF JoystickL;
		/// <summary>
		/// Normalized value of right joystick.  Values range between -0.5 - 0.5
		/// </summary>
		public PointF JoystickR;
		/// <summary>
		/// Raw value of analog trigger.  Values range between 0 - 255.
		/// </summary>
		public byte RawTriggerL, RawTriggerR;
		/// <summary>
		/// Normalized value of analog trigger.  Values range between 0.0 - 1.0.
		/// </summary>
		public float TriggerL, TriggerR;
	}
}
