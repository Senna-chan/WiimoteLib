//////////////////////////////////////////////////////////////////////////////////
//	MultipleWiimoteForm.cs
//	Managed Wiimote Library Tester
//	Written by Brian Peek (http://www.brianpeek.com/)
//  for MSDN's Coding4Fun (http://msdn.microsoft.com/coding4fun/)
//	Visit http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx
//  and http://www.codeplex.com/WiimoteLib
//  for more information
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using ScpDriverInterface;
using WiimoteLib;
using WiimoteLib.DataTypes;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Helpers;

namespace WiimoteTest
{
	public partial class WiimoteInfo : UserControl
	{
		private delegate void UpdateWiimoteStateDelegate(WiimoteChangedEventArgs args);
		private delegate void UpdateExtensionChangedDelegate(WiimoteExtensionChangedEventArgs args);

		private Bitmap b = new Bitmap(256, 192, PixelFormat.Format24bppRgb);
		private Bitmap b2 = new Bitmap(256, 192, PixelFormat.Format24bppRgb);
		private Graphics g;
		private Graphics g2;
		private Wiimote mWiimote;
	    private WiimoteAudioSample catfood, sample;

	    private Point3F minWiimoteAccel;
	    private Point3F maxWiimoteAccel;
	    private Point3F minNunchukAccel;
	    private Point3F maxNunchukAccel;
		private SpeakerFreq desiredFreq = SpeakerFreq.FREQ_2610HZ;
		private byte desiredVolume = 50;
        public WiimoteInfo()
		{
			InitializeComponent();
			g = Graphics.FromImage(b);
			g2 = Graphics.FromImage(b2);
			cobSensitivity.DataSource = Enum.GetValues(typeof(IRSensitivity));
		}

		public WiimoteInfo(Wiimote wm) : this()
		{
			mWiimote = wm;
			//catfood = wm.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "Assets\\catfood.wav"), 4200);
			sample = wm.Load8bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "Assets\\sweep.wav"));
        }

		public void UpdateState(WiimoteChangedEventArgs args)
		{
			BeginInvoke(new UpdateWiimoteStateDelegate(UpdateWiimoteChanged), args);
		}

		public void UpdateExtension(WiimoteExtensionChangedEventArgs args)
		{
			BeginInvoke(new UpdateExtensionChangedDelegate(UpdateExtensionChanged), args);
		}

		private void chkLED_CheckedChanged(object sender, EventArgs e)
		{
			mWiimote.SetLEDs(chkLED1.Checked, chkLED2.Checked, chkLED3.Checked, chkLED4.Checked);
		}

		private void chkRumble_CheckedChanged(object sender, EventArgs e)
		{
			mWiimote.SetRumble(chkRumble.Checked);
		}

		private void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
		{
			WiimoteState ws = args.WiimoteState;


            clbButtons.SetItemChecked(0, ws.Buttons. A);
			clbButtons.SetItemChecked(1, ws.Buttons. B);
			clbButtons.SetItemChecked(2, ws.Buttons. Minus);
			clbButtons.SetItemChecked(3, ws.Buttons. Home);
			clbButtons.SetItemChecked(4, ws.Buttons. Plus);
			clbButtons.SetItemChecked(5, ws.Buttons. One);
			clbButtons.SetItemChecked(6, ws.Buttons. Two);
			clbButtons.SetItemChecked(7, ws.Buttons. Up);
			clbButtons.SetItemChecked(8, ws.Buttons. Down);
			clbButtons.SetItemChecked(9, ws.Buttons. Left);
			clbButtons.SetItemChecked(10, ws.Buttons.Right);


			mWiimote.OnPressedReleased("A", () => { mWiimote.PlaySample(sample, desiredVolume); }, () => { mWiimote.EnableSpeaker(false); });
            mWiimote.OnPressedReleased("B", () => { mWiimote.PlaySquareWave(desiredFreq, desiredVolume); }, () => { mWiimote.EnableSpeaker(false); });
			mWiimote.OnPressedReleased("Up", () => { }, () => { desiredVolume += 10; if (desiredVolume > 100) desiredVolume = 100; });
			mWiimote.OnPressedReleased("Down", () => { }, () => { desiredVolume -= 10; if (desiredVolume < 0) desiredVolume = 0; });
			mWiimote.OnPressedReleased("Left", () => { }, () => { desiredFreq++; if (desiredFreq > SpeakerFreq.FREQ_4410HZ) desiredFreq = SpeakerFreq.FREQ_4410HZ; });
			mWiimote.OnPressedReleased("Right", () => { }, () => { desiredFreq --; if (desiredFreq < SpeakerFreq.FREQ_4200HZ) desiredFreq = SpeakerFreq.FREQ_4200HZ; });
			lblAccel.Text = ws.Accel.Values.ToString();
		    lblAccelImu.Text = ws.Accel.IMU.ToString();

		    if (ws.Accel.Values.X < minWiimoteAccel.X)
		    {
		        minWiimoteAccel.X = ws.Accel.Values.X;
		    }
		    if (ws.Accel.Values.Y < minWiimoteAccel.Y)
		    {
		        minWiimoteAccel.Y = ws.Accel.Values.Y;
		    }
		    if (ws.Accel.Values.Z < minWiimoteAccel.Z)
		    {
		        minWiimoteAccel.Z = ws.Accel.Values.Z;
		    }
		    if (ws.Accel.Values.X > maxWiimoteAccel.X)
		    {
		        maxWiimoteAccel.X = ws.Accel.Values.X;
		    }
		    if (ws.Accel.Values.Y > maxWiimoteAccel.Y)
		    {
		        maxWiimoteAccel.Y = ws.Accel.Values.Y;
		    }
		    if (ws.Accel.Values.Z > maxWiimoteAccel.Z)
		    {
		        maxWiimoteAccel.Z = ws.Accel.Values.Z;
		    }

		    lblAccelMinMax.Text = "";
		    lblAccelMinMax.Text += minWiimoteAccel.ToString();
		    lblAccelMinMax.Text += Environment.NewLine;
		    lblAccelMinMax.Text += maxWiimoteAccel.ToString();

            chkLED1.Checked = ws.LED.LED1;
			chkLED2.Checked = ws.LED.LED2;
			chkLED3.Checked = ws.LED.LED3;
			chkLED4.Checked = ws.LED.LED4;
            
		    chkSpeakerEnabled.Checked = ws.Speaker.Enabled;
		    chkSpeakerMuted.Checked = ws.Speaker.Muted;
		    lblSpeakerFrequency.Text = ws.Speaker.Frequency + " Hz";
            lblSpeakerVolume.Text = ws.Speaker.Volume + "%";
		    if (ws.CurrentSample != null && ws.Speaker.Enabled)
		    {
		        lblSpeakerSample.Text = ws.CurrentSample.AudioName;
		    }
            else if (ws.CurrentSample == null && ws.Speaker.Frequency != SpeakerFreq.FREQ_NONE && ws.Speaker.Enabled)
		    {
		        lblSpeakerSample.Text = "SquareWave";
		    }
		    else
		    {
		        lblSpeakerSample.Text = "Not playing";
		    }
            switch (ws.ExtensionType)
			{
                case ExtensionType.None:
                case ExtensionType.Unknown:
                    break;
                case ExtensionType.Nunchuk:
					lblChuk.Text = ws.Nunchuk.Accel.Values.ToString();
					lblChukJoy.Text = ws.Nunchuk.Joystick.ToString();
                    lblNunchuckAccelImu.Text = ws.Nunchuk.Accel.IMU.ToString();
                    chkC.Checked = ws.Nunchuk.C;
					chkZ.Checked = ws.Nunchuk.Z;
                    if (ws.Nunchuk.Accel.Values.X < minNunchukAccel.X)
                    {
                        minNunchukAccel.X = ws.Nunchuk.Accel.Values.X;
                    }
                    if (ws.Nunchuk.Accel.Values.Y < minNunchukAccel.Y)
                    {
                        minNunchukAccel.Y = ws.Nunchuk.Accel.Values.Y;
                    }
                    if (ws.Nunchuk.Accel.Values.Z < minNunchukAccel.Z)
                    {
                        minNunchukAccel.Z = ws.Nunchuk.Accel.Values.Z;
                    }
                    if (ws.Nunchuk.Accel.Values.X > maxNunchukAccel.X)
                    {
                        maxNunchukAccel.X = ws.Nunchuk.Accel.Values.X;
                    }
                    if (ws.Nunchuk.Accel.Values.Y > maxNunchukAccel.Y)
                    {
                        maxNunchukAccel.Y = ws.Nunchuk.Accel.Values.Y;
                    }
                    if (ws.Nunchuk.Accel.Values.Z > maxNunchukAccel.Z)
                    {
                        maxNunchukAccel.Z = ws.Nunchuk.Accel.Values.Z;
                    }

                    //_controller.LeftStickX = (short)(ws.Nunchuk.Joystick.X * 32767 * 2).Constrain(-32768, 32767);
                    //_controller.LeftStickY = (short)(ws.Nunchuk.Joystick.Y * 32767 * 2).Constrain(-32768, 32767);

                    lblNunchukAccelMinMax.Text = "";
                    lblNunchukAccelMinMax.Text += minNunchukAccel.ToString();
                    lblNunchukAccelMinMax.Text += Environment.NewLine;
                    lblNunchukAccelMinMax.Text += maxNunchukAccel.ToString();
                    break;
				case ExtensionType.ClassicController:
					clbCCButtons.SetItemChecked(0, ws.ClassicController.Buttons.A);
					clbCCButtons.SetItemChecked(1, ws.ClassicController.Buttons.B);
					clbCCButtons.SetItemChecked(2, ws.ClassicController.Buttons.X);
					clbCCButtons.SetItemChecked(3, ws.ClassicController.Buttons.Y);
					clbCCButtons.SetItemChecked(4, ws.ClassicController.Buttons.Minus);
					clbCCButtons.SetItemChecked(5, ws.ClassicController.Buttons.Home);
					clbCCButtons.SetItemChecked(6, ws.ClassicController.Buttons.Plus);
					clbCCButtons.SetItemChecked(7, ws.ClassicController.Buttons.Up);
					clbCCButtons.SetItemChecked(8, ws.ClassicController.Buttons.Down);
					clbCCButtons.SetItemChecked(9, ws.ClassicController.Buttons.Left);
					clbCCButtons.SetItemChecked(10, ws.ClassicController.Buttons.Right);
					clbCCButtons.SetItemChecked(11, ws.ClassicController.Buttons.ZL);
					clbCCButtons.SetItemChecked(12, ws.ClassicController.Buttons.ZR);
					clbCCButtons.SetItemChecked(13, ws.ClassicController.Buttons.TriggerL);
					clbCCButtons.SetItemChecked(14, ws.ClassicController.Buttons.TriggerR);

					lblCCJoy1.Text = ws.ClassicController.JoystickL.ToString();
					lblCCJoy2.Text = ws.ClassicController.JoystickR.ToString();

					lblTriggerL.Text = ws.ClassicController.TriggerL.ToString();
					lblTriggerR.Text = ws.ClassicController.TriggerR.ToString();
					break;

				case ExtensionType.Guitar:
				    clbGuitarButtons.SetItemChecked(0, ws.Guitar.FretButtons.Green);
				    clbGuitarButtons.SetItemChecked(1, ws.Guitar.FretButtons.Red);
				    clbGuitarButtons.SetItemChecked(2, ws.Guitar.FretButtons.Yellow);
				    clbGuitarButtons.SetItemChecked(3, ws.Guitar.FretButtons.Blue);
				    clbGuitarButtons.SetItemChecked(4, ws.Guitar.FretButtons.Orange);
				    clbGuitarButtons.SetItemChecked(5, ws.Guitar.Buttons.Minus);
				    clbGuitarButtons.SetItemChecked(6, ws.Guitar.Buttons.Plus);
				    clbGuitarButtons.SetItemChecked(7, ws.Guitar.Buttons.StrumUp);
				    clbGuitarButtons.SetItemChecked(8, ws.Guitar.Buttons.StrumDown);

					clbTouchbar.SetItemChecked(0, ws.Guitar.Touchbar.Green);
					clbTouchbar.SetItemChecked(1, ws.Guitar.Touchbar.Red);
					clbTouchbar.SetItemChecked(2, ws.Guitar.Touchbar.Yellow);
					clbTouchbar.SetItemChecked(3, ws.Guitar.Touchbar.Blue);
					clbTouchbar.SetItemChecked(4, ws.Guitar.Touchbar.Orange);

					lblGuitarJoy.Text = ws.Guitar.Joystick.ToString();
					lblGuitarWhammy.Text = ws.Guitar.WhammyBar.ToString();
					lblGuitarType.Text = ws.Guitar.GuitarType.ToString();
				    break;

				case ExtensionType.Drums:
					clbDrums.SetItemChecked(0, ws.Drums.Red);
					clbDrums.SetItemChecked(1, ws.Drums.Blue);
					clbDrums.SetItemChecked(2, ws.Drums.Green);
					clbDrums.SetItemChecked(3, ws.Drums.Yellow);
					clbDrums.SetItemChecked(4, ws.Drums.Orange);
					clbDrums.SetItemChecked(5, ws.Drums.Pedal);
					clbDrums.SetItemChecked(6, ws.Drums.Minus);
					clbDrums.SetItemChecked(7, ws.Drums.Plus);

					lbDrumVelocity.Items.Clear();
					lbDrumVelocity.Items.Add(ws.Drums.RedVelocity);
					lbDrumVelocity.Items.Add(ws.Drums.BlueVelocity);
					lbDrumVelocity.Items.Add(ws.Drums.GreenVelocity);
					lbDrumVelocity.Items.Add(ws.Drums.YellowVelocity);
					lbDrumVelocity.Items.Add(ws.Drums.OrangeVelocity);
					lbDrumVelocity.Items.Add(ws.Drums.PedalVelocity);

					lblDrumJoy.Text = ws.Drums.Joystick.ToString();
					break;

				case ExtensionType.BalanceBoard:
					if(chkLbs.Checked)
					{
						lblBBTL.Text = ws.BalanceBoard.SensorValuesLb.TopLeft.ToString();
						lblBBTR.Text = ws.BalanceBoard.SensorValuesLb.TopRight.ToString();
						lblBBBL.Text = ws.BalanceBoard.SensorValuesLb.BottomLeft.ToString();
						lblBBBR.Text = ws.BalanceBoard.SensorValuesLb.BottomRight.ToString();
						lblBBTotal.Text = ws.BalanceBoard.WeightLb.ToString();
					}
					else
					{
						lblBBTL.Text = ws.BalanceBoard.SensorValuesKg.TopLeft.ToString();
						lblBBTR.Text = ws.BalanceBoard.SensorValuesKg.TopRight.ToString();
						lblBBBL.Text = ws.BalanceBoard.SensorValuesKg.BottomLeft.ToString();
						lblBBBR.Text = ws.BalanceBoard.SensorValuesKg.BottomRight.ToString();
						lblBBTotal.Text = ws.BalanceBoard.WeightKg.ToString();
					}
					lblCOG.Text = ws.BalanceBoard.CenterOfGravity.ToString();
					break;
                case ExtensionType.MotionPlus:

                    lblMPRawPitch.Text = ws.MotionPlus.GyroRaw.X.ToString();
                    lblMPRawRoll.Text  = ws.MotionPlus.GyroRaw.Y.ToString();
                    lblMPRawYaw.Text   = ws.MotionPlus.GyroRaw.Z.ToString();

                    chcMPYawSlow.Checked = ws.MotionPlus.SlowYaw;
                    chcMPRollSlow.Checked = ws.MotionPlus.SlowRoll;
                    chcMPPitchSlow.Checked = ws.MotionPlus.SlowPitch;

                    lblMPPitch.Text = ws.MotionPlus.Gyro.X.ToString("000.00");
                    lblMPRoll.Text  = ws.MotionPlus.Gyro.Y.ToString("000.00");
                    lblMPYaw.Text   = ws.MotionPlus.Gyro.Z.ToString("000.00");

                    lblMotionPlusImu.Text = ws.MotionPlus.IMU.ToString();
                    break;
                case ExtensionType.UDraw:
                    g2.Clear(Color.Black);
                    if (ws.Tablet.PressureType != TabletPressure.NotPressed)
                    {
                        g2.DrawEllipse(new Pen(Color.DarkBlue), ws.Tablet.Position.X / 6, ws.Tablet.Position.Y / 6, ws.Tablet.PenPressure / 4, ws.Tablet.PenPressure / 4);
                    }
                    pbTablet.Image = b2;
                    chkPenPress.Checked = ws.Tablet.Point;
                    chkPenUp.Checked = ws.Tablet.ButtonUp;
                    chkPenDown.Checked = ws.Tablet.ButtonDown;
                    lblTabletBox.Text = ws.Tablet.BoxPosition.ToString();
                    lblTabletRaw.Text = ws.Tablet.RawPosition.ToString();
                    lblPenPressure.Text = ws.Tablet.PenPressure.ToString();
                    lblPenPosition.Text = ws.Tablet.Position.ToString();
                    break;
			}

			g.Clear(Color.Black);
			lblIRMode.Text = ws.IR.Mode.ToString();
			UpdateIR(ws.IR.IRSensors[0], lblIR1, lblIR1Raw, chkFound1, Color.Red);
			UpdateIR(ws.IR.IRSensors[1], lblIR2, lblIR2Raw, chkFound2, Color.Blue);
			UpdateIR(ws.IR.IRSensors[2], lblIR3, lblIR3Raw, chkFound3, Color.Yellow);
			UpdateIR(ws.IR.IRSensors[3], lblIR4, lblIR4Raw, chkFound4, Color.Orange);
			UpdateIR(ws.IR.Midpoint, lblIR12A, lblIR12ARaw, chbFound12A, Color.Green);
			if ((IRSensitivity)cobSensitivity.SelectedValue != ws.IR.Sensitivity && !cobSensitivity.Focused) 
				cobSensitivity.SelectedItem = ws.IR.Sensitivity;
			//if(ws.IR.IRSensors[0].Found && ws.IR.IRSensors[1].Found)
			//	g.DrawEllipse(new Pen(Color.Green), (int)(ws.IR.RawMidpoint.X / 4), (int)(ws.IR.RawMidpoint.Y / 4), 5, 5);

			pbIR.Image = b;

			pbBattery.Value = (ws.Battery > 0xc8 ? 0xc8 : (int)ws.Battery);
			lblBattery.Text = ws.Battery.ToString();
			lblDevicePath.Text = "Device Path: " + mWiimote.HIDDevicePath;

        }

	    private void UpdateIR(IRSensor irSensor, Label lblNorm, Label lblRaw, CheckBox chkFound, Color color)
		{
			chkFound.Checked = irSensor.Found;
			chkFound.ForeColor = irSensor.ValidPosition ? Color.Green : Color.Red;
			if(irSensor.Found)
			{
				lblNorm.Text = irSensor.Position.ToString() + ", " + irSensor.Size;
				lblRaw.Text = irSensor.RawPosition.ToString();
				g.DrawEllipse(new Pen(color), (int)(irSensor.RawPosition.X / 4), (int)(irSensor.RawPosition.Y / 4),
							 irSensor.Size+1, irSensor.Size+1);
			}
		}

		private void UpdateExtensionChanged(WiimoteExtensionChangedEventArgs args)
		{
			chkExtension.Text = args.ExtensionType.ToString();
			chkExtension.Checked = args.Inserted;
		}

		public Wiimote Wiimote
		{
			set { mWiimote = value; }
		}

        private void btnMPConnect_Click(object sender, EventArgs e)
        {
            if (mWiimote.WiimoteState.ExtensionType == ExtensionType.MotionPlus)
            {
                mWiimote.DisconnectMotionPlus();
                btnMPConnect.Text = "Connect";
            } else
            {
                mWiimote.ConnectMotionPlus();
                btnMPConnect.Text = "Disconnect"; 
            }
            
        }

        private void tbFreqOverride_TextChanged(object sender, EventArgs e)
        {
            var input = ((TextBox) sender).Text;
            mWiimote.FreqOverride = input == string.Empty ? 0 : int.Parse(input);
        }


		private void cobSensitivity_SelectionChangeCommitted(object sender, EventArgs e)
		{
			mWiimote.SetReportType(mWiimote.WiimoteState.Extension ? InputReport.IRExtensionAccel : InputReport.IRAccel, (IRSensitivity)cobSensitivity.SelectedItem, true);
		}

		private void lblMPCallibrate_Click(object sender, EventArgs e)
        {
            mWiimote.CalibrateMotionPlus();
        }
    }
}
