using ScpDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WiiInputMapper.Scripting;

namespace WiiInputMapper.Template
{
	public class XBox
	{
		public bool Up = false;
		public bool Down = false;
		public bool Left = false;
		public bool Right = false;
		public bool Start = false;
		public bool Back = false;
		public bool LeftStick = false;
		public bool RightStick = false;
		public bool LeftBumper = false;
		public bool RightBumper = false;
		public bool Logo = false;
		public bool A = false;
		public bool B = false;
		public bool X = false;
		public bool Y = false;

		public byte LeftTrigger = 0;
		public byte RightTrigger = 0;
		public short LeftStickX = 0;
		public short LeftStickY = 0;
		public short RightStickX = 0;
		public short RightStickY = 0;


		public void populateController(ref X360Controller controller)
		{
			controller.Buttons = X360Buttons.None; // Clear buttons
			controller.LeftTrigger = LeftTrigger;
			controller.RightTrigger = RightTrigger;
			controller.LeftStickX = LeftStickX;
			controller.LeftStickY = LeftStickY;
			controller.RightStickX = RightStickX;
			controller.RightStickY = RightStickY;
			if (Up) controller.Buttons ^= X360Buttons.Up;
			if (Down) controller.Buttons ^= X360Buttons.Down;
			if (Left) controller.Buttons ^= X360Buttons.Left;
			if (Right) controller.Buttons ^= X360Buttons.Right;
			if (Start) controller.Buttons ^= X360Buttons.Start;
			if (Back) controller.Buttons ^= X360Buttons.Back;
			if (LeftStick) controller.Buttons ^= X360Buttons.LeftStick;
			if (RightStick) controller.Buttons ^= X360Buttons.RightStick;
			if (LeftBumper) controller.Buttons ^= X360Buttons.LeftBumper;
			if (RightBumper) controller.Buttons ^= X360Buttons.RightBumper;
			if (Logo) controller.Buttons ^= X360Buttons.Logo;
			if (A) controller.Buttons ^= X360Buttons.A;
			if (B) controller.Buttons ^= X360Buttons.B;
			if (X) controller.Buttons ^= X360Buttons.X;
			if (Y) controller.Buttons ^= X360Buttons.Y;
		}
	}
}
