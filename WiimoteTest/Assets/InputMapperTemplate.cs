using System;
using System.Data;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiimoteLib;
using WiimoteLib.DataTypes;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Helpers;
using ScpDriverInterface;
using WindowsInput;

namespace WiimoteTest
{
	public class InputMapperExecutor
	{
		private Wiimote mWiimote;
		private ScpBus _scpBus;
		public static X360Controller _controller = new X360Controller();
		private byte[] _outputReport = new byte[8];
		private XBox XBox = new XBox();
		InputSimulator inputSimulator = new InputSimulator();
		public InputMapperExecutor(Wiimote wiimote)
		{
			mWiimote = wiimote;
			try
			{
				_scpBus = new ScpBus();
				_scpBus.PlugIn(1);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(1);
			}
			mWiimote.WiimoteChanged += UpdateState;
		}
		//GLOBALS
		private void CodeExecutor(WiimoteState Wiimote)
		{
			//USERCODE
			XBox.populateController();
			_scpBus.Report(1, _controller.GetReport(), _outputReport);
		}

		private void UpdateState(object sender, WiimoteChangedEventArgs e)
		{
			CodeExecutor(e.WiimoteState);
		}
	}

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
			InputMapperExecutor._controller.Buttons = X360Buttons.None; // Clear buttons
			InputMapperExecutor._controller.LeftTrigger = LeftTrigger;
			InputMapperExecutor._controller.RightTrigger = RightTrigger;
			InputMapperExecutor._controller.LeftStickX = LeftStickX;
			InputMapperExecutor._controller.LeftStickY = LeftStickY;
			InputMapperExecutor._controller.RightStickX = RightStickX;
			InputMapperExecutor._controller.RightStickY = RightStickY;
			if (Up) InputMapperExecutor._controller.Buttons ^= X360Buttons.Up;
			if (Down) InputMapperExecutor._controller.Buttons ^= X360Buttons.	   Down			   ;
			if (Left) InputMapperExecutor._controller.Buttons ^= X360Buttons.		   Left			   ;
			if (Right) InputMapperExecutor._controller.Buttons ^= X360Buttons.		   Right		   ;
			if (Start) InputMapperExecutor._controller.Buttons ^= X360Buttons.		   Start		   ;
			if (Back) InputMapperExecutor._controller.Buttons ^= X360Buttons.		   Back			   ;
			if (LeftStick) InputMapperExecutor._controller.Buttons ^= X360Buttons.	   LeftStick	   ;
			if (RightStick) InputMapperExecutor._controller.Buttons ^= X360Buttons.	   RightStick	   ;
			if (LeftBumper) InputMapperExecutor._controller.Buttons ^= X360Buttons.	   LeftBumper	   ;
			if (RightBumper) InputMapperExecutor._controller.Buttons ^= X360Buttons.   RightBumper	   ;
			if (Logo) InputMapperExecutor._controller.Buttons ^= X360Buttons.		   Logo			   ;
			if (A) InputMapperExecutor._controller.Buttons ^= X360Buttons.			   A			   ;
			if (B) InputMapperExecutor._controller.Buttons ^= X360Buttons.			   B			   ;
			if (X) InputMapperExecutor._controller.Buttons ^= X360Buttons.			   X			   ;
			if (Y) InputMapperExecutor._controller.Buttons ^= X360Buttons.Y							   ;
		}
	}
}