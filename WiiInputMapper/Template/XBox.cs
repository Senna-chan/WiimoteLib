using ScpDriverInterface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		public void populateController()
		{
			InputMapperTemplate._controller.Buttons = X360Buttons.None; // Clear buttons
			InputMapperTemplate._controller.LeftTrigger = LeftTrigger;
			InputMapperTemplate._controller.RightTrigger = RightTrigger;
			InputMapperTemplate._controller.LeftStickX = LeftStickX;
			InputMapperTemplate._controller.LeftStickY = LeftStickY;
			InputMapperTemplate._controller.RightStickX = RightStickX;
			InputMapperTemplate._controller.RightStickY = RightStickY;
			if (Up) InputMapperTemplate._controller.Buttons ^= X360Buttons.Up;
			if (Down) InputMapperTemplate._controller.Buttons ^= X360Buttons.Down;
			if (Left) InputMapperTemplate._controller.Buttons ^= X360Buttons.Left;
			if (Right) InputMapperTemplate._controller.Buttons ^= X360Buttons.Right;
			if (Start) InputMapperTemplate._controller.Buttons ^= X360Buttons.Start;
			if (Back) InputMapperTemplate._controller.Buttons ^= X360Buttons.Back;
			if (LeftStick) InputMapperTemplate._controller.Buttons ^= X360Buttons.LeftStick;
			if (RightStick) InputMapperTemplate._controller.Buttons ^= X360Buttons.RightStick;
			if (LeftBumper) InputMapperTemplate._controller.Buttons ^= X360Buttons.LeftBumper;
			if (RightBumper) InputMapperTemplate._controller.Buttons ^= X360Buttons.RightBumper;
			if (Logo) InputMapperTemplate._controller.Buttons ^= X360Buttons.Logo;
			if (A) InputMapperTemplate._controller.Buttons ^= X360Buttons.A;
			if (B) InputMapperTemplate._controller.Buttons ^= X360Buttons.B;
			if (X) InputMapperTemplate._controller.Buttons ^= X360Buttons.X;
			if (Y) InputMapperTemplate._controller.Buttons ^= X360Buttons.Y;
		}
	}
}
