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
using WiiInputMapper.HIDEmulators;

namespace WiiInputMapper.Scripting
{
	public class PreCompiledScriptV1
    {
		private Wiimote mWiimote = new Wiimote();
		private ScpBus _scpBus;
		private X360Controller _controller = new X360Controller();
		private byte[] _outputReport = new byte[8];
		private XBox XBox = new XBox();
		private Keyboard Keyboard;
		private Mouse Mouse;
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
        public PreCompiledScriptV1()
		{
			requiredExtension = ExtensionType.UDraw;
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
            if (mWiimote.WiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                mWiimote.SetReportType(InputReport.IRExtensionAccel, IRSensitivity.Maximum, true);
            if (initvarshower)
			{
				varShower.Show();
			}
            mWiimote.SetLEDs(true, false, false, false);
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

		ExtensionType requiredExtension = ExtensionType.None;
		//region userblock
		
		static int currentScreenIndex = 0;
		Screen currentScreen = Screen.AllScreens[currentScreenIndex];

        private void RunCodeLoop(WiimoteState Wiimote)
		{
			varShower.ShowVar("Wiimote.Tablet.Position", Wiimote.Tablet.Position);
			varShower.ShowVar("Wiimote.Tablet.Point", Wiimote.Tablet.Point);
			varShower.ShowVar("Wiimote.Tablet.ButtonDown", Wiimote.Tablet.ButtonDown);
			varShower.ShowVar("Wiimote.Tablet.ButtonUp", Wiimote.Tablet.ButtonUp);
            varShower.ShowVar("MousePosition", Mouse.GetCursorPosition());
            varShower.ShowVar("CurrentMonitor", currentScreenIndex);

            if (Wiimote.Tablet.Point){
                Mouse.MouseLeft(Wiimote.Tablet.ButtonDown);
				Mouse.MouseRight(Wiimote.Tablet.ButtonUp);
                int monX = (int)(Wiimote.Tablet.Position.X.Map(0,1860, 0, currentScreen.Bounds.Width) - currentScreen.Bounds.Width) + currentScreen.Bounds.Width; //convert to absolute coordinates
                int monY = (int)(Wiimote.Tablet.Position.Y.Map(0,1340, 0, currentScreen.Bounds.Height) - currentScreen.Bounds.Height) + currentScreen.Bounds.Height;
                int x = monX + currentScreen.Bounds.X;
				int y = monY + currentScreen.Bounds.Y;
                //Console.WriteLine($"monX {monX}/monY {monY}, x {x}/y {y}");
                Mouse.MoveAbs(x, y);
				//mouseOps.MoveAbs(monX, monY);
			}
			else
            {
                Mouse.MouseLeft(false);
                Mouse.MouseRight(false);
            }
			mWiimote.OnPressedReleased("Left", () => { }, ()=> {
				if(currentScreenIndex != 0)
                {
                    currentScreenIndex--;
                    currentScreen = Screen.AllScreens[currentScreenIndex];
                    mWiimote.SetLEDs(currentScreenIndex + 1);
                }
			});
			Mouse
			mWiimote.OnPressedReleased("Right", () => { }, ()=> {
				if(!(currentScreenIndex >= Screen.AllScreens.Length - 1))
                {
                    currentScreenIndex++;
                    currentScreen = Screen.AllScreens[currentScreenIndex];
					mWiimote.SetLEDs(currentScreenIndex + 1);
                }
			});
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
            return @"UNSUPPORTED. Running complex script 1";
        }
	}
}