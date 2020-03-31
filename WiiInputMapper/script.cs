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
using WindowsInput.Native;
using System.Collections.Generic;
using WiimoteLib.Exceptions;
using WiiInputMapper.Template;

namespace WiiInputMapper
{
	public class InputMapperTemplate
	{
		private Wiimote mWiimote = new Wiimote();
		private ScpBus _scpBus;
		public static X360Controller _controller = new X360Controller();
		private byte[] _outputReport = new byte[8];
		private XBox XBox = new XBox();
		private Mouse Mouse;
		private Keyboard Keyboard;
		private bool initvarshower = false; // This is now for showing vars
		private bool initscp = false; // If true than we will connect the scp bus. If not then we do not need to init it
		private VarShower varShower = new VarShower();
		public InputMapperTemplate()
		{
			InputSimulator inputSimulator = new InputSimulator();
			Mouse = new Mouse(inputSimulator);
			Keyboard = new Keyboard(inputSimulator);

			mWiimote.WiimoteChanged += UpdateState;
			mWiimote.WiimoteExtensionChanged += ExtensionChanged;
			try
			{
				if (initscp)
				{
					_scpBus = new ScpBus();
					_scpBus.PlugIn(1);
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				Environment.Exit(1);
			}
			try
			{
				Console.WriteLine("Connecting to a wiiremote");
				mWiimote.Connect();
			}
			catch (WiimoteNotFoundException)
			{
				MessageBox.Show("There is no wiimote found");
			}
			mWiimote.SetReportType(InputReport.IRAccel, true);
			mWiimote.SetLEDs(true, true, true, true);
			initvarshower = true;

			if (initvarshower)
			{
				varShower.Show();
			}
		}

		public void StopScript()
		{
			mWiimote.Disconnect();
			if(_scpBus != null) _scpBus.Unplug(1);
			varShower.Close();
			Mouse.Stop();
			Keyboard.Stop();
		}


		//region userblock
		double mousespeed = 0.1;

		private void CodeExecutor(WiimoteState Wiimote)
		{
			varShower.ShowVar("mousespeed", mousespeed);
			varShower.ShowVar("Wiimote.Nunchuk.Z", Wiimote.Nunchuk.Z);
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.X", Wiimote.Nunchuk.Joystick.X);
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.Y", Wiimote.Nunchuk.Joystick.Y);
			
			if (Wiimote.Nunchuk.Z) { 
			
				Mouse.MoveRel(Wiimote.Nunchuk.Joystick.X * mousespeed, Wiimote.Nunchuk.Joystick.Y * mousespeed);
			}
			
			if(Wiimote.Buttons.Plus != Wiimote.OldButtons.Plus){
				if(mousespeed < 1.5) mousespeed += 0.1;
			
			}
			
			if(Wiimote.Buttons.Minus != Wiimote.OldButtons.Minus){
				if(mousespeed > 0.1) mousespeed -= 0.1;
			}

		}
		//endregion  

		private void UpdateState(object sender, WiimoteChangedEventArgs e)
		{
			CodeExecutor(e.WiimoteState);
			XBox.populateController();
			if (_scpBus != null) _scpBus.Report(1, _controller.GetReport(), _outputReport);
		}


		private void ExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
		{
			if (args.Inserted)
				mWiimote.SetReportType(InputReport.IRExtensionAccel, true);
			else
				mWiimote.SetReportType(InputReport.IRAccel, true);
		}
	}
}