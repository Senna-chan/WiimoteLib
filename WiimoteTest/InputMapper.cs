using Microsoft.CSharp;
using ScpDriverInterface;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WiimoteLib;
using WiimoteLib.DataTypes;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Helpers;
using WiimoteTest.Properties;
using WindowsInput;

namespace WiimoteTest
{
	public partial class InputMapper : Form
	{
		private Wiimote mWiimote;
		public InputMapper(Wiimote wiimote)
		{
			InitializeComponent();
			mWiimote = wiimote;
			//try
			//{
			//	_scpBus = new ScpBus();
			//	_scpBus.PlugIn(1);
			//}
			//catch (Exception ex)
			//{
			//	MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			//	Environment.Exit(1);
			//}
			//mWiimote.WiimoteChanged += UpdateState;
		}

		//private void UpdateState(object sender, WiimoteChangedEventArgs e)
		//{
		//	UpdateWiimoteChanged(e);
		//}


		private void btnRun_Click(object sender, EventArgs e)
		{
			String code = parseCode();
			try
			{
				Compile(code, true);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private void btnCompile_Click(object sender, EventArgs e)
		{
			String code = parseCode();
			try
			{
				Compile(code, false);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.ToString());
			}
		}

		private string parseCode()
		{
			List<String> variables = new List<string>();
			String userCode = txtCode.Text;
			// Code changing
			userCode = userCode.Replace("log(", "Console.WriteLine(");
			userCode = userCode.Replace("Wiimote.Button.", "Wiimote.ButtonState.");
			userCode = userCode.Replace("Keyboard.Press", "//inputSimulator.Keyboard.KeyPress");
			userCode = userCode.Replace("Keyboard.Down", "//inputSimulator.Keyboard.KeyDown");
			userCode = userCode.Replace("Keyboard.Up", "//inputSimulator.Keyboard.KeyUp");
			
			String code = Resources.InputMapperTemplate.Replace("//USERCODE", userCode).Replace("//GLOBALS", String.Join("\r\n", variables));
			File.WriteAllText("./tempfiles/script.cs", code);
			return code;
		}

		//private void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
		//{
		//	WiimoteState ws = args.WiimoteState;

		//	_controller.Buttons = X360Buttons.None; // Clear buttons

		//	if (ws.ButtonState.A) _controller.Buttons ^= X360Buttons.A;
		//	if (ws.ButtonState.B) _controller.Buttons ^= X360Buttons.B;
		//	if (ws.ButtonState.Right) _controller.Buttons ^= X360Buttons.Up;
		//	if (ws.ButtonState.Left) _controller.Buttons ^= X360Buttons.Down;
		//	if (ws.ButtonState.Up) _controller.Buttons ^= X360Buttons.Left;
		//	if (ws.ButtonState.Down) _controller.Buttons ^= X360Buttons.Right;
		//	if (ws.ButtonState.One) _controller.Buttons ^= X360Buttons.X;
		//	if (ws.ButtonState.Two) _controller.Buttons ^= X360Buttons.Y;
		//	if (ws.ButtonState.Plus) _controller.Buttons ^= X360Buttons.Start;
		//	if (ws.ButtonState.Home) _controller.Buttons ^= X360Buttons.Logo;

		//	switch (ws.ExtensionType)
		//	{
		//		case ExtensionType.Nunchuk:
		//			_controller.LeftStickX = (short)(ws.NunchukState.Joystick.X * 32767 * 2).Constrain(-32768, 32767);
		//			_controller.LeftStickY = (short)(ws.NunchukState.Joystick.Y * 32767 * 2).Constrain(-32768, 32767);
		//			_controller.LeftTrigger = (byte)(ws.NunchukState.C ? 255 : 0);
		//			_controller.RightTrigger = (byte)(ws.NunchukState.Z ? 255 : 0);
		//			break;
		//	}

		//	//lblAccel.Text = ws.AccelState.Values.ToString();
		//	//lblAccelImu.Text = ws.AccelState.IMUState.ToString();

		//	//if (ws.AccelState.Values.X < minWiimoteAccel.X)
		//	//{
		//	//	minWiimoteAccel.X = ws.AccelState.Values.X;
		//	//}
		//	//if (ws.AccelState.Values.Y < minWiimoteAccel.Y)
		//	//{
		//	//	minWiimoteAccel.Y = ws.AccelState.Values.Y;
		//	//}
		//	//if (ws.AccelState.Values.Z < minWiimoteAccel.Z)
		//	//{
		//	//	minWiimoteAccel.Z = ws.AccelState.Values.Z;
		//	//}
		//	//if (ws.AccelState.Values.X > maxWiimoteAccel.X)
		//	//{
		//	//	maxWiimoteAccel.X = ws.AccelState.Values.X;
		//	//}
		//	//if (ws.AccelState.Values.Y > maxWiimoteAccel.Y)
		//	//{
		//	//	maxWiimoteAccel.Y = ws.AccelState.Values.Y;
		//	//}
		//	//if (ws.AccelState.Values.Z > maxWiimoteAccel.Z)
		//	//{
		//	//	maxWiimoteAccel.Z = ws.AccelState.Values.Z;
		//	//}

		//	//lblAccelMinMax.Text = "";
		//	//lblAccelMinMax.Text += minWiimoteAccel.ToString();
		//	//lblAccelMinMax.Text += Environment.NewLine;
		//	//lblAccelMinMax.Text += maxWiimoteAccel.ToString();

		//	//chkLED1.Checked = ws.LEDState.LED1;
		//	//chkLED2.Checked = ws.LEDState.LED2;
		//	//chkLED3.Checked = ws.LEDState.LED3;
		//	//chkLED4.Checked = ws.LEDState.LED4;

		//	//chkSpeakerEnabled.Checked = ws.Speaker.Enabled;
		//	//chkSpeakerMuted.Checked = ws.Speaker.Muted;
		//	//lblSpeakerFrequency.Text = ws.Speaker.Frequency + " Hz";
		//	//lblSpeakerVolume.Text = ws.Speaker.Volume + "%";
		//	//if (ws.CurrentSample != null && ws.Speaker.Enabled)
		//	//{
		//	//	lblSpeakerSample.Text = ws.CurrentSample.AudioName;
		//	//}
		//	//else if (ws.CurrentSample == null && ws.Speaker.Frequency != SpeakerFreq.FREQ_NONE && ws.Speaker.Enabled)
		//	//{
		//	//	lblSpeakerSample.Text = "SquareWave";
		//	//}
		//	//else
		//	//{
		//	//	lblSpeakerSample.Text = "Not playing";
		//	//}
		//	//if (chkRawBuff.Checked)
		//	//{
		//	//	if (ws.RawBuff != null)
		//	//	{
		//	//		foreach (var rawBuffByte in ws.RawBuff)
		//	//		{
		//	//			lblRawBuff.Text += $"b{Convert.ToString(rawBuffByte, 2).PadLeft(8, '0')} ";
		//	//		}
		//	//	}
		//	//}
		//	//switch (ws.ExtensionType)
		//	//{
		//	//	case ExtensionType.None:
		//	//		break;
		//	//	case ExtensionType.Unknown:
		//	//		lblRawBuff.Text = "";
		//	//		if (ws.RawBuff != null)
		//	//		{
		//	//			foreach (var rawBuffByte in ws.RawBuff)
		//	//			{
		//	//				lblRawBuff.Text += $"b{Convert.ToString(rawBuffByte, 2).PadLeft(8, '0')} ";
		//	//			}
		//	//		}
		//	//		break;
		//	//	case ExtensionType.Nunchuk:
		//	//		lblChuk.Text = ws.NunchukState.AccelState.Values.ToString();
		//	//		lblChukJoy.Text = ws.NunchukState.Joystick.ToString();
		//	//		lblNunchuckAccelImu.Text = ws.NunchukState.AccelState.IMUState.ToString();
		//	//		chkC.Checked = ws.NunchukState.C;
		//	//		chkZ.Checked = ws.NunchukState.Z;
		//	//		if (ws.NunchukState.AccelState.Values.X < minNunchukAccel.X)
		//	//		{
		//	//			minNunchukAccel.X = ws.NunchukState.AccelState.Values.X;
		//	//		}
		//	//		if (ws.NunchukState.AccelState.Values.Y < minNunchukAccel.Y)
		//	//		{
		//	//			minNunchukAccel.Y = ws.NunchukState.AccelState.Values.Y;
		//	//		}
		//	//		if (ws.NunchukState.AccelState.Values.Z < minNunchukAccel.Z)
		//	//		{
		//	//			minNunchukAccel.Z = ws.NunchukState.AccelState.Values.Z;
		//	//		}
		//	//		if (ws.NunchukState.AccelState.Values.X > maxNunchukAccel.X)
		//	//		{
		//	//			maxNunchukAccel.X = ws.NunchukState.AccelState.Values.X;
		//	//		}
		//	//		if (ws.NunchukState.AccelState.Values.Y > maxNunchukAccel.Y)
		//	//		{
		//	//			maxNunchukAccel.Y = ws.NunchukState.AccelState.Values.Y;
		//	//		}
		//	//		if (ws.NunchukState.AccelState.Values.Z > maxNunchukAccel.Z)
		//	//		{
		//	//			maxNunchukAccel.Z = ws.NunchukState.AccelState.Values.Z;
		//	//		}

		//	//		_controller.LeftStickX = (short)(ws.NunchukState.Joystick.X * 32767 * 2).Constrain(-32768, 32767);
		//	//		_controller.LeftStickY = (short)(ws.NunchukState.Joystick.Y * 32767 * 2).Constrain(-32768, 32767);

		//	//		lblNunchukAccelMinMax.Text = "";
		//	//		lblNunchukAccelMinMax.Text += minNunchukAccel.ToString();
		//	//		lblNunchukAccelMinMax.Text += Environment.NewLine;
		//	//		lblNunchukAccelMinMax.Text += maxNunchukAccel.ToString();
		//	//		break;
		//	//	case ExtensionType.ClassicController:
		//	//		clbCCButtons.SetItemChecked(0, ws.ClassicControllerState.ButtonState.A);
		//	//		clbCCButtons.SetItemChecked(1, ws.ClassicControllerState.ButtonState.B);
		//	//		clbCCButtons.SetItemChecked(2, ws.ClassicControllerState.ButtonState.X);
		//	//		clbCCButtons.SetItemChecked(3, ws.ClassicControllerState.ButtonState.Y);
		//	//		clbCCButtons.SetItemChecked(4, ws.ClassicControllerState.ButtonState.Minus);
		//	//		clbCCButtons.SetItemChecked(5, ws.ClassicControllerState.ButtonState.Home);
		//	//		clbCCButtons.SetItemChecked(6, ws.ClassicControllerState.ButtonState.Plus);
		//	//		clbCCButtons.SetItemChecked(7, ws.ClassicControllerState.ButtonState.Up);
		//	//		clbCCButtons.SetItemChecked(8, ws.ClassicControllerState.ButtonState.Down);
		//	//		clbCCButtons.SetItemChecked(9, ws.ClassicControllerState.ButtonState.Left);
		//	//		clbCCButtons.SetItemChecked(10, ws.ClassicControllerState.ButtonState.Right);
		//	//		clbCCButtons.SetItemChecked(11, ws.ClassicControllerState.ButtonState.ZL);
		//	//		clbCCButtons.SetItemChecked(12, ws.ClassicControllerState.ButtonState.ZR);
		//	//		clbCCButtons.SetItemChecked(13, ws.ClassicControllerState.ButtonState.TriggerL);
		//	//		clbCCButtons.SetItemChecked(14, ws.ClassicControllerState.ButtonState.TriggerR);

		//	//		lblCCJoy1.Text = ws.ClassicControllerState.JoystickL.ToString();
		//	//		lblCCJoy2.Text = ws.ClassicControllerState.JoystickR.ToString();

		//	//		lblTriggerL.Text = ws.ClassicControllerState.TriggerL.ToString();
		//	//		lblTriggerR.Text = ws.ClassicControllerState.TriggerR.ToString();
		//	//		break;

		//	//	case ExtensionType.Guitar:
		//	//		clbGuitarButtons.SetItemChecked(0, ws.GuitarState.FretButtonState.Green);
		//	//		clbGuitarButtons.SetItemChecked(1, ws.GuitarState.FretButtonState.Red);
		//	//		clbGuitarButtons.SetItemChecked(2, ws.GuitarState.FretButtonState.Yellow);
		//	//		clbGuitarButtons.SetItemChecked(3, ws.GuitarState.FretButtonState.Blue);
		//	//		clbGuitarButtons.SetItemChecked(4, ws.GuitarState.FretButtonState.Orange);
		//	//		clbGuitarButtons.SetItemChecked(5, ws.GuitarState.ButtonState.Minus);
		//	//		clbGuitarButtons.SetItemChecked(6, ws.GuitarState.ButtonState.Plus);
		//	//		clbGuitarButtons.SetItemChecked(7, ws.GuitarState.ButtonState.StrumUp);
		//	//		clbGuitarButtons.SetItemChecked(8, ws.GuitarState.ButtonState.StrumDown);

		//	//		clbTouchbar.SetItemChecked(0, ws.GuitarState.TouchbarState.Green);
		//	//		clbTouchbar.SetItemChecked(1, ws.GuitarState.TouchbarState.Red);
		//	//		clbTouchbar.SetItemChecked(2, ws.GuitarState.TouchbarState.Yellow);
		//	//		clbTouchbar.SetItemChecked(3, ws.GuitarState.TouchbarState.Blue);
		//	//		clbTouchbar.SetItemChecked(4, ws.GuitarState.TouchbarState.Orange);

		//	//		lblGuitarJoy.Text = ws.GuitarState.Joystick.ToString();
		//	//		lblGuitarWhammy.Text = ws.GuitarState.WhammyBar.ToString();
		//	//		lblGuitarType.Text = ws.GuitarState.GuitarType.ToString();
		//	//		break;

		//	//	case ExtensionType.Drums:
		//	//		clbDrums.SetItemChecked(0, ws.DrumsState.Red);
		//	//		clbDrums.SetItemChecked(1, ws.DrumsState.Blue);
		//	//		clbDrums.SetItemChecked(2, ws.DrumsState.Green);
		//	//		clbDrums.SetItemChecked(3, ws.DrumsState.Yellow);
		//	//		clbDrums.SetItemChecked(4, ws.DrumsState.Orange);
		//	//		clbDrums.SetItemChecked(5, ws.DrumsState.Pedal);
		//	//		clbDrums.SetItemChecked(6, ws.DrumsState.Minus);
		//	//		clbDrums.SetItemChecked(7, ws.DrumsState.Plus);

		//	//		lbDrumVelocity.Items.Clear();
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.RedVelocity);
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.BlueVelocity);
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.GreenVelocity);
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.YellowVelocity);
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.OrangeVelocity);
		//	//		lbDrumVelocity.Items.Add(ws.DrumsState.PedalVelocity);

		//	//		lblDrumJoy.Text = ws.DrumsState.Joystick.ToString();
		//	//		break;

		//	//	case ExtensionType.BalanceBoard:
		//	//		if (chkLbs.Checked)
		//	//		{
		//	//			lblBBTL.Text = ws.BalanceBoardState.SensorValuesLb.TopLeft.ToString();
		//	//			lblBBTR.Text = ws.BalanceBoardState.SensorValuesLb.TopRight.ToString();
		//	//			lblBBBL.Text = ws.BalanceBoardState.SensorValuesLb.BottomLeft.ToString();
		//	//			lblBBBR.Text = ws.BalanceBoardState.SensorValuesLb.BottomRight.ToString();
		//	//			lblBBTotal.Text = ws.BalanceBoardState.WeightLb.ToString();
		//	//		}
		//	//		else
		//	//		{
		//	//			lblBBTL.Text = ws.BalanceBoardState.SensorValuesKg.TopLeft.ToString();
		//	//			lblBBTR.Text = ws.BalanceBoardState.SensorValuesKg.TopRight.ToString();
		//	//			lblBBBL.Text = ws.BalanceBoardState.SensorValuesKg.BottomLeft.ToString();
		//	//			lblBBBR.Text = ws.BalanceBoardState.SensorValuesKg.BottomRight.ToString();
		//	//			lblBBTotal.Text = ws.BalanceBoardState.WeightKg.ToString();
		//	//		}
		//	//		lblCOG.Text = ws.BalanceBoardState.CenterOfGravity.ToString();
		//	//		break;
		//	//	case ExtensionType.MotionPlus:

		//	//		lblMPRawPitch.Text = ws.MotionPlusState.GyroRaw.X.ToString();
		//	//		lblMPRawRoll.Text = ws.MotionPlusState.GyroRaw.Y.ToString();
		//	//		lblMPRawYaw.Text = ws.MotionPlusState.GyroRaw.Z.ToString();

		//	//		chcMPYawSlow.Checked = ws.MotionPlusState.SlowYaw;
		//	//		chcMPRollSlow.Checked = ws.MotionPlusState.SlowRoll;
		//	//		chcMPPitchSlow.Checked = ws.MotionPlusState.SlowPitch;

		//	//		lblMPPitch.Text = ws.MotionPlusState.Gyro.X.ToString("000.00");
		//	//		lblMPRoll.Text = ws.MotionPlusState.Gyro.Y.ToString("000.00");
		//	//		lblMPYaw.Text = ws.MotionPlusState.Gyro.Z.ToString("000.00");

		//	//		lblMotionPlusImu.Text = ws.MotionPlusState.IMUState.ToString();
		//	//		break;
		//	//	case ExtensionType.UDraw:
		//	//		g2.Clear(Color.Black);
		//	//		if (ws.TabletState.PressureType != TabletPressure.NotPressed)
		//	//		{
		//	//			g2.DrawEllipse(new Pen(Color.DarkBlue), ws.TabletState.Position.X / 6, ws.TabletState.Position.Y / 6, ws.TabletState.PenPressure / 4, ws.TabletState.PenPressure / 4);
		//	//		}
		//	//		pbTablet.Image = b2;
		//	//		chkPenPress.Checked = ws.TabletState.Point;
		//	//		chkPenUp.Checked = ws.TabletState.ButtonUp;
		//	//		chkPenDown.Checked = ws.TabletState.ButtonDown;
		//	//		lblTabletBox.Text = ws.TabletState.BoxPosition.ToString();
		//	//		lblTabletRaw.Text = ws.TabletState.RawPosition.ToString();
		//	//		lblPenPressure.Text = ws.TabletState.PenPressure.ToString();
		//	//		lblPenPosition.Text = ws.TabletState.Position.ToString();
		//	//		break;
		//	//}

		//	//g.Clear(Color.Black);

		//	//UpdateIR(ws.IRState.IRSensors[0], lblIR1, lblIR1Raw, chkFound1, Color.Red);
		//	//UpdateIR(ws.IRState.IRSensors[1], lblIR2, lblIR2Raw, chkFound2, Color.Blue);
		//	//UpdateIR(ws.IRState.IRSensors[2], lblIR3, lblIR3Raw, chkFound3, Color.Yellow);
		//	//UpdateIR(ws.IRState.IRSensors[3], lblIR4, lblIR4Raw, chkFound4, Color.Orange);

		//	//if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found)
		//	//	g.DrawEllipse(new Pen(Color.Green), (int)(ws.IRState.RawMidpoint.X / 4), (int)(ws.IRState.RawMidpoint.Y / 4), 5, 5);

		//	//pbIR.Image = b;

		//	//pbBattery.Value = (ws.Battery > 0xc8 ? 0xc8 : (int)ws.Battery);
		//	//lblBattery.Text = ws.Battery.ToString();
		//	//lblDevicePath.Text = "Device Path: " + mWiimote.HIDDevicePath;


		//	_scpBus.Report(1, _controller.GetReport(), _outputReport); // Send controller report to the bus
		//}

		//private void UpdateIR(IRSensor irSensor, Label lblNorm, Label lblRaw, CheckBox chkFound, Color color)
		//{
		//	//chkFound.Checked = irSensor.Found;

		//	//if (irSensor.Found)
		//	//{
		//	//	lblNorm.Text = irSensor.Position.ToString() + ", " + irSensor.Size;
		//	//	lblRaw.Text = irSensor.RawPosition.ToString();
		//	//	g.DrawEllipse(new Pen(color), (int)(irSensor.RawPosition.X / 4), (int)(irSensor.RawPosition.Y / 4),
		//	//				 irSensor.Size + 1, irSensor.Size + 1);
		//	//}
		//}


		void Compile(string code, bool andRun)
		{

			CompilerParameters CompilerParams = new CompilerParameters();
			string outputDirectory = Directory.GetCurrentDirectory();

			CompilerParams.GenerateInMemory = true;
			CompilerParams.TreatWarningsAsErrors = false;
			CompilerParams.GenerateExecutable = false;
			CompilerParams.CompilerOptions = "/optimize";

			string[] references = { "System.dll", "System.Data.dll" };
			CompilerParams.ReferencedAssemblies.AddRange(references);
			CompilerParams.ReferencedAssemblies.AddRange(Assembly.GetExecutingAssembly().GetReferencedAssemblies().Select(a => a.Name + ".dll").ToArray());

			CSharpCodeProvider provider = new CSharpCodeProvider();
			CompilerResults compile = provider.CompileAssemblyFromSource(CompilerParams, code);

			

			if (compile.Errors.HasErrors)
			{
				StringBuilder text = new StringBuilder();
				text.AppendLine("Compile error: ");
				foreach (CompilerError ce in compile.Errors)
				{
					text.AppendLine(ce.ToString());
				}
				txtCompileOutput.Text = text.ToString();
			}
			else
			{
				txtCompileOutput.Text = "Compile success";
				if (!andRun) return;
				Module module = compile.CompiledAssembly.GetModules()[0];
				Type mt = null;
				ConstructorInfo constructorInfo = null;
				if (module != null)
				{
					mt = module.GetType("WiimoteTest.InputMapperExecutor");
				}

				if (mt != null)
				{
					constructorInfo = mt.GetConstructors()[0];
				}

				if (constructorInfo != null)
				{
					constructorInfo.Invoke(new[] { mWiimote});
				}
			}
		}
	}
}
