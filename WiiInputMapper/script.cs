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
using WindowsInput.Events;

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
		private bool usingIR = false; // If true than we will transform some IR values to be better suited with what we want to do most of the time
		private VarShower varShower = new VarShower();
		private WiimoteFunctions wiimoteFunctions;
		private DataParser dataParser = new DataParser();
		Point IRDifference;
		Point lastIRPosition;
		public InputMapperTemplate()
		{
			initvarshower = true;

			Mouse = new Mouse();
			Keyboard = new Keyboard();

			wiimoteFunctions = new WiimoteFunctions(mWiimote);
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
				return;
			}
			mWiimote.SetReportType(InputReport.IRAccel, true);
			mWiimote.SetLEDs(true, true, true, true);
			if (initvarshower)
			{
				varShower.Show();
			}
		}

		public void StopScript()
		{
			mWiimote.SetRumble(false);
			mWiimote.MuteSpeaker(true);
			mWiimote.Disconnect();
			if (_scpBus != null) _scpBus.Unplug(1);
			varShower.Close();
			Mouse.Stop();
			Keyboard.Stop();
		}


		//region userblock
		
		float mousespeed = 1.0f;


		private void CodeExecutor(WiimoteState Wiimote)
		{
						varShower.ShowVar("Wiimote.Nunchuk.Accel.IMU.Pitch", Wiimote.Nunchuk.Accel.IMU.Pitch);
			varShower.ShowVar("Wiimote.Nunchuk.Accel.IMU.Roll", Wiimote.Nunchuk.Accel.IMU.Roll);
			Keyboard.Down(KeyCode.Shift, DataValidater.ValidatePitch(Wiimote.Nunchuk.Accel.IMU.Pitch, Wiimote.Nunchuk.Accel.IMU.Roll, -40, '<'));
			Keyboard.Up(KeyCode.Shift, DataValidater.ValidatePitch(Wiimote.Nunchuk.Accel.IMU.Pitch, Wiimote.Nunchuk.Accel.IMU.Roll, 40, '>'));
			Keyboard.Press(KeyCode.R, Wiimote.Nunchuk.Accel.IMU.Roll < -40);

		}
		//endregion  

		private void UpdateState(object sender, WiimoteChangedEventArgs e)
		{
			if (usingIR)
			{
				/// Mapping to -value +value
				e.WiimoteState.IR.Midpoint.RawPosition.X -= 512; // Map 0-1023 to -512 - 512
				e.WiimoteState.IR.Midpoint.RawPosition.Y -= 384;
				e.WiimoteState.IR.Midpoint.Position.X -= 0.5f;
				e.WiimoteState.IR.Midpoint.Position.Y -= 0.5f;

				// Making smaller numbers
				e.WiimoteState.IR.Midpoint.RawPosition.X /= 10;
				e.WiimoteState.IR.Midpoint.RawPosition.Y /= 10;
				e.WiimoteState.IR.Midpoint.Position.X = Math.Round(e.WiimoteState.IR.Midpoint.Position.X, 3);
				e.WiimoteState.IR.Midpoint.Position.Y = Math.Round(e.WiimoteState.IR.Midpoint.Position.Y, 3);

				e.WiimoteState.IR.Midpoint.RawPosition.X	*= -1;
				e.WiimoteState.IR.Midpoint.RawPosition.Y	*= -1;
				e.WiimoteState.IR.Midpoint.Position.X		*= -1;
				e.WiimoteState.IR.Midpoint.Position.Y		*= -1;
				wiimoteFunctions.IRRumble(e.WiimoteState.IR.Midpoint.ValidPosition);
				//if (e.WiimoteState.IR.Midpoint.ValidPosition)
				//{
				//	dataParser.AddData("irrawX", e.WiimoteState.IR.Midpoint.RawPosition.X);
				//	dataParser.AddData("irrawY", e.WiimoteState.IR.Midpoint.RawPosition.Y);
				//	dataParser.AddData("irX", e.WiimoteState.IR.Midpoint.Position.X);
				//	dataParser.AddData("irY", e.WiimoteState.IR.Midpoint.Position.Y);
				//}
				//e.WiimoteState.IR.Midpoint.RawPosition.X	= (int)dataParser.GetData("irrawX");
				//e.WiimoteState.IR.Midpoint.RawPosition.Y	= (int)dataParser.GetData("irrawY");
				//e.WiimoteState.IR.Midpoint.Position.X		= dataParser.GetData("irX");
				//e.WiimoteState.IR.Midpoint.Position.Y		= dataParser.GetData("irY");
				IRDifference.X = lastIRPosition.X - e.WiimoteState.IR.Midpoint.RawPosition.X;
				IRDifference.Y = lastIRPosition.Y - e.WiimoteState.IR.Midpoint.RawPosition.Y;
				lastIRPosition = e.WiimoteState.IR.Midpoint.RawPosition;
			}

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

		public string GetSourceCode()
		{
			return @"
show(Wiimote.Nunchuk.Accel.Pitch )
show(Wiimote.Nunchuk.Accel.Roll )
Keyboard.KeyDown.Shift = Wiimote.Nunchuk.Accel.Pitch <  -40;
Keyboard.KeyUp.Shift = Wiimote.Nunchuk.Accel.Pitch >  40;
Keyboard.R = Wiimote.Nunchuk.Accel.Roll < -40;
";
		}
	}
}