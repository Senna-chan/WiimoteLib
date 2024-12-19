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

namespace WiiInputMapper.Scripting
{
	public class ErrorScript
	{
		private Wiimote mWiimote = new Wiimote();
		private ScpBus _scpBus;
		private X360Controller _controller = new X360Controller();
		private byte[] _outputReport = new byte[8];
		private XBox XBox = new XBox();
		private MouseOls Mouse;
		private Keyboard Keyboard;
		private bool initvarshower = false; // This is now for showing vars
		private bool initscp = false; // If true than we will connect the scp bus. If not then we do not need to init it
		private bool usingIR = false; // If true than we will transform some IR values to be better suited with what we want to do most of the time
		private VarShower varShower = new VarShower();
		private WiimoteFunctions wiimoteFunctions;
		private DataSmoother dataSmoother = new DataSmoother();
		Point IRDifference;
		Point lastIRPosition;
		private long lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds() - 10;
		private bool runBusy = false;
        ExtensionType requiredExtension = ExtensionType.None;

        public ErrorScript()
		{
			requiredExtension = ExtensionType.Nunchuk;
initvarshower = true;

			Mouse = new MouseOls();
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
			if (requiredExtension != ExtensionType.None)
			{
				mWiimote.SetReportType(InputReport.IRExtensionAccel, true);
			}
			else
            {
                mWiimote.SetReportType(InputReport.IRAccel, true);
            }
			mWiimote.SetLEDs(true, true, true, true);
			if (initvarshower)
			{
				varShower.Show();
			}
			mWiimote.GetStatus();
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
		
		float deadzone = 15f;
bool rollLeft = false;
bool allowPress = false;
float mousespeed = 1.0f;


		private void RunCodeLoop(WiimoteState Wiimote)
		{
			
			
			varShower.ShowVar("Wiimote.Buttons.A", Wiimote.Buttons.A);
			varShower.ShowVar("Wiimote.Nunchuk.Z", Wiimote.Nunchuk.Z);
			
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.X", Wiimote.Nunchuk.Joystick.X);
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.Y", Wiimote.Nunchuk.Joystick.Y);
			
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.X > deadzone", Wiimote.Nunchuk.Joystick.X > deadzone);
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.X < -deadzone", Wiimote.Nunchuk.Joystick.X < -deadzone);
			
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.Y > deadzone", Wiimote.Nunchuk.Joystick.Y > deadzone);
			varShower.ShowVar("Wiimote.Nunchuk.Joystick.Y < -deadzone", Wiimote.Nunchuk.Joystick.Y < -deadzone);
			
			varShower.ShowVar("Wiimote.Nunchuk.Accel.IMU.Pitch", Wiimote.Nunchuk.Accel.IMU.Pitch);
			varShower.ShowVar("Wiimote.Nunchuk.Accel.IMU.Roll", Wiimote.Nunchuk.Accel.IMU.Roll);
			
			varShower.ShowVar("rollLeft", rollLeft);
			varShower.ShowVar("allowPress", allowPress);
			
			if(Wiimote.Buttons.Home) {
				allowPress = true;
			} else {
				allowPress = false;
			}
			
			
			if(!rollLeft){
				if(Wiimote.Nunchuk.Accel.IMU.Roll > 45){
					rollLeft = true;
				}
			}
			else
			{
				if(Wiimote.Nunchuk.Accel.IMU.Roll < 45){
					rollLeft = false;
		if(allowPress) Keyboard.SinglePress(KeyCode.R);
				}
			}
			if(allowPress) {
				Keyboard.Press(KeyCode.W, Wiimote.Nunchuk.X > 55);
				Keyboard.Press(KeyCode.S, Wiimote.Nunchuk.X < -55);
				Keyboard.Press(KeyCode.A, Wiimote.Nunchuk.Y > 55);
				Keyboard.Press(KeyCode.D, Wiimote.Nunchuk.Y < -55);
			}
			

        }
        //endregion  



        private void RunChecker(WiimoteState Wiimote)
        {
            if (Wiimote.ExtensionType != requiredExtension)
            {
                return;
            }
			long runDiff = DateTimeOffset.Now.ToUnixTimeMilliseconds() - lastRunTime;

            if (runDiff >= 10 && !runBusy)
			{
                lastRunTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                runBusy = true;
                RunCodeLoop(Wiimote);
				runBusy = false;
				XBox.populateController(ref _controller);
				if (_scpBus != null) _scpBus.Report(1, _controller.GetReport(), _outputReport);
			} 
			else
			{
                //Console.WriteLine("To fast run. RunDiff " + runDiff + " Run busy " + runBusy);
            }
        }

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

                e.WiimoteState.IR.Midpoint.RawPosition.X *= -1;
                e.WiimoteState.IR.Midpoint.RawPosition.Y *= -1;
                e.WiimoteState.IR.Midpoint.Position.X *= -1;
                e.WiimoteState.IR.Midpoint.Position.Y *= -1;
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

            varShower.ShowVar("Extension", e.WiimoteState.ExtensionType);
            varShower.ShowVar("RequiredExtension", requiredExtension);
			RunChecker(e.WiimoteState);
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

var deadzone = 15;
var rollLeft = false;
var allowPress = false;

show(Wiimote.Buttons.A);
show(Wiimote.Nunchuk.Z);

show(Wiimote.Nunchuk.Joystick.X);
show(Wiimote.Nunchuk.Joystick.Y);

show(Wiimote.Nunchuk.Joystick.X > deadzone);
show(Wiimote.Nunchuk.Joystick.X < -deadzone);

show(Wiimote.Nunchuk.Joystick.Y > deadzone);
show(Wiimote.Nunchuk.Joystick.Y < -deadzone);

show(Wiimote.Nunchuk.Accel.Pitch);
show(Wiimote.Nunchuk.Accel.Roll);

show(rollLeft);
show(allowPress);

if(Wiimote.Buttons.Home) {
	allowPress = true;
} else {
	allowPress = false;
}


if(!rollLeft){
	if(Wiimote.Nunchuk.Accel.Roll > 45){
		rollLeft = true;
	}
}
else
{
	if(Wiimote.Nunchuk.Accel.Roll < 45){
		rollLeft = false;
rawcode
		if(allowPress) Keyboard.SinglePress(KeyCode.R);
endrawcode
	}
}
if(allowPress) {
	Keyboard.Press.W = Wiimote.Nunchuk.X > 55;
	Keyboard.Press.S = Wiimote.Nunchuk.X < -55;
	Keyboard.Press.A = Wiimote.Nunchuk.Y > 55;
	Keyboard.Press.D = Wiimote.Nunchuk.Y < -55;
}

";
		}
	}
}