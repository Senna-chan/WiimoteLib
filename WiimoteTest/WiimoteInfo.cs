﻿//////////////////////////////////////////////////////////////////////////////////
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
using WiimoteLib;
using WiimoteLib.DataTypes;
using WiimoteLib.DataTypes.Enums;

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
		public WiimoteInfo()
		{
			InitializeComponent();
			g = Graphics.FromImage(b);
			g2 = Graphics.FromImage(b2);
		}

		public WiimoteInfo(Wiimote wm) : this()
		{
			mWiimote = wm;
		    //catfood = wm.Load16bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "Assets\\catfood.wav"),4200);
		    sample = wm.Load8bitMonoSampleWAV(Path.Combine(Environment.CurrentDirectory, "Assets\\chargingpenguin.wav"));

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

			clbButtons.SetItemChecked(0, ws.ButtonState. A);
			clbButtons.SetItemChecked(1, ws.ButtonState. B);
			clbButtons.SetItemChecked(2, ws.ButtonState. Minus);
			clbButtons.SetItemChecked(3, ws.ButtonState. Home);
			clbButtons.SetItemChecked(4, ws.ButtonState. Plus);
			clbButtons.SetItemChecked(5, ws.ButtonState. One);
			clbButtons.SetItemChecked(6, ws.ButtonState. Two);
			clbButtons.SetItemChecked(7, ws.ButtonState. Up);
			clbButtons.SetItemChecked(8, ws.ButtonState. Down);
			clbButtons.SetItemChecked(9, ws.ButtonState. Left);
			clbButtons.SetItemChecked(10, ws.ButtonState.Right);



		    
		    mWiimote.OnPressedReleased("A", () => { mWiimote.PlaySample(sample, 30); }, () => { mWiimote.EnableSpeaker(false); });
            mWiimote.OnPressedReleased("B", () => { mWiimote.PlaySquareWave(SpeakerFreq.FREQ_4410HZ, 50); }, () => { mWiimote.EnableSpeaker(false); });

			lblAccel.Text = ws.AccelState.Values.ToString();

			chkLED1.Checked = ws.LEDState.LED1;
			chkLED2.Checked = ws.LEDState.LED2;
			chkLED3.Checked = ws.LEDState.LED3;
			chkLED4.Checked = ws.LEDState.LED4;
            
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

		    switch(ws.ExtensionType)
			{
				case ExtensionType.Nunchuk:
					lblChuk.Text = ws.NunchukState.AccelState.Values.ToString();
					lblChukJoy.Text = ws.NunchukState.Joystick.ToString();
					chkC.Checked = ws.NunchukState.C;
					chkZ.Checked = ws.NunchukState.Z;
					break;

				case ExtensionType.ClassicController:
					clbCCButtons.SetItemChecked(0, ws.ClassicControllerState.ButtonState.A);
					clbCCButtons.SetItemChecked(1, ws.ClassicControllerState.ButtonState.B);
					clbCCButtons.SetItemChecked(2, ws.ClassicControllerState.ButtonState.X);
					clbCCButtons.SetItemChecked(3, ws.ClassicControllerState.ButtonState.Y);
					clbCCButtons.SetItemChecked(4, ws.ClassicControllerState.ButtonState.Minus);
					clbCCButtons.SetItemChecked(5, ws.ClassicControllerState.ButtonState.Home);
					clbCCButtons.SetItemChecked(6, ws.ClassicControllerState.ButtonState.Plus);
					clbCCButtons.SetItemChecked(7, ws.ClassicControllerState.ButtonState.Up);
					clbCCButtons.SetItemChecked(8, ws.ClassicControllerState.ButtonState.Down);
					clbCCButtons.SetItemChecked(9, ws.ClassicControllerState.ButtonState.Left);
					clbCCButtons.SetItemChecked(10, ws.ClassicControllerState.ButtonState.Right);
					clbCCButtons.SetItemChecked(11, ws.ClassicControllerState.ButtonState.ZL);
					clbCCButtons.SetItemChecked(12, ws.ClassicControllerState.ButtonState.ZR);
					clbCCButtons.SetItemChecked(13, ws.ClassicControllerState.ButtonState.TriggerL);
					clbCCButtons.SetItemChecked(14, ws.ClassicControllerState.ButtonState.TriggerR);

					lblCCJoy1.Text = ws.ClassicControllerState.JoystickL.ToString();
					lblCCJoy2.Text = ws.ClassicControllerState.JoystickR.ToString();

					lblTriggerL.Text = ws.ClassicControllerState.TriggerL.ToString();
					lblTriggerR.Text = ws.ClassicControllerState.TriggerR.ToString();
					break;

				case ExtensionType.Guitar:
				    clbGuitarButtons.SetItemChecked(0, ws.GuitarState.FretButtonState.Green);
				    clbGuitarButtons.SetItemChecked(1, ws.GuitarState.FretButtonState.Red);
				    clbGuitarButtons.SetItemChecked(2, ws.GuitarState.FretButtonState.Yellow);
				    clbGuitarButtons.SetItemChecked(3, ws.GuitarState.FretButtonState.Blue);
				    clbGuitarButtons.SetItemChecked(4, ws.GuitarState.FretButtonState.Orange);
				    clbGuitarButtons.SetItemChecked(5, ws.GuitarState.ButtonState.Minus);
				    clbGuitarButtons.SetItemChecked(6, ws.GuitarState.ButtonState.Plus);
				    clbGuitarButtons.SetItemChecked(7, ws.GuitarState.ButtonState.StrumUp);
				    clbGuitarButtons.SetItemChecked(8, ws.GuitarState.ButtonState.StrumDown);

					clbTouchbar.SetItemChecked(0, ws.GuitarState.TouchbarState.Green);
					clbTouchbar.SetItemChecked(1, ws.GuitarState.TouchbarState.Red);
					clbTouchbar.SetItemChecked(2, ws.GuitarState.TouchbarState.Yellow);
					clbTouchbar.SetItemChecked(3, ws.GuitarState.TouchbarState.Blue);
					clbTouchbar.SetItemChecked(4, ws.GuitarState.TouchbarState.Orange);

					lblGuitarJoy.Text = ws.GuitarState.Joystick.ToString();
					lblGuitarWhammy.Text = ws.GuitarState.WhammyBar.ToString();
					lblGuitarType.Text = ws.GuitarState.GuitarType.ToString();
				    break;

				case ExtensionType.Drums:
					clbDrums.SetItemChecked(0, ws.DrumsState.Red);
					clbDrums.SetItemChecked(1, ws.DrumsState.Blue);
					clbDrums.SetItemChecked(2, ws.DrumsState.Green);
					clbDrums.SetItemChecked(3, ws.DrumsState.Yellow);
					clbDrums.SetItemChecked(4, ws.DrumsState.Orange);
					clbDrums.SetItemChecked(5, ws.DrumsState.Pedal);
					clbDrums.SetItemChecked(6, ws.DrumsState.Minus);
					clbDrums.SetItemChecked(7, ws.DrumsState.Plus);

					lbDrumVelocity.Items.Clear();
					lbDrumVelocity.Items.Add(ws.DrumsState.RedVelocity);
					lbDrumVelocity.Items.Add(ws.DrumsState.BlueVelocity);
					lbDrumVelocity.Items.Add(ws.DrumsState.GreenVelocity);
					lbDrumVelocity.Items.Add(ws.DrumsState.YellowVelocity);
					lbDrumVelocity.Items.Add(ws.DrumsState.OrangeVelocity);
					lbDrumVelocity.Items.Add(ws.DrumsState.PedalVelocity);

					lblDrumJoy.Text = ws.DrumsState.Joystick.ToString();
					break;

				case ExtensionType.BalanceBoard:
					if(chkLbs.Checked)
					{
						lblBBTL.Text = ws.BalanceBoardState.SensorValuesLb.TopLeft.ToString();
						lblBBTR.Text = ws.BalanceBoardState.SensorValuesLb.TopRight.ToString();
						lblBBBL.Text = ws.BalanceBoardState.SensorValuesLb.BottomLeft.ToString();
						lblBBBR.Text = ws.BalanceBoardState.SensorValuesLb.BottomRight.ToString();
						lblBBTotal.Text = ws.BalanceBoardState.WeightLb.ToString();
					}
					else
					{
						lblBBTL.Text = ws.BalanceBoardState.SensorValuesKg.TopLeft.ToString();
						lblBBTR.Text = ws.BalanceBoardState.SensorValuesKg.TopRight.ToString();
						lblBBBL.Text = ws.BalanceBoardState.SensorValuesKg.BottomLeft.ToString();
						lblBBBR.Text = ws.BalanceBoardState.SensorValuesKg.BottomRight.ToString();
						lblBBTotal.Text = ws.BalanceBoardState.WeightKg.ToString();
					}
					lblCOG.Text = ws.BalanceBoardState.CenterOfGravity.ToString();
					break;
                case ExtensionType.MotionPlus:

                    lblMPRawPitch.Text = ws.MotionPlusState.GyroRaw.X.ToString();
                    lblMPRawRoll.Text  = ws.MotionPlusState.GyroRaw.Y.ToString();
                    lblMPRawYaw.Text   = ws.MotionPlusState.GyroRaw.Z.ToString();

                    chcMPYawSlow.Checked = ws.MotionPlusState.SlowYaw;
                    chcMPRollSlow.Checked = ws.MotionPlusState.SlowRoll;
                    chcMPPitchSlow.Checked = ws.MotionPlusState.SlowPitch;

                    lblMPPitch.Text = ws.MotionPlusState.Gyro.X.ToString("000.00");
                    lblMPRoll.Text  = ws.MotionPlusState.Gyro.Y.ToString("000.00");
                    lblMPYaw.Text   = ws.MotionPlusState.Gyro.Z.ToString("000.00");
                    break;
                case ExtensionType.UDraw:
                    g2.Clear(Color.Black);
                    if (ws.TabletState.PressureType != TabletPressure.NotPressed)
                    {
                        g2.DrawEllipse(new Pen(Color.DarkBlue), ws.TabletState.Position.X / 6, ws.TabletState.Position.Y / 6, ws.TabletState.PenPressure / 4, ws.TabletState.PenPressure / 4);
                    }
                    pbTablet.Image = b2;
                    chkPenPress.Checked = ws.TabletState.Point;
                    chkPenUp.Checked = ws.TabletState.ButtonUp;
                    chkPenDown.Checked = ws.TabletState.ButtonDown;
                    lblTabletBox.Text = ws.TabletState.BoxPosition.ToString();
                    lblTabletRaw.Text = ws.TabletState.RawPosition.ToString();
                    lblPenPressure.Text = ws.TabletState.PenPressure.ToString();
                    lblPenPosition.Text = ws.TabletState.Position.ToString();
                    break;
			}

			g.Clear(Color.Black);

			UpdateIR(ws.IRState.IRSensors[0], lblIR1, lblIR1Raw, chkFound1, Color.Red);
			UpdateIR(ws.IRState.IRSensors[1], lblIR2, lblIR2Raw, chkFound2, Color.Blue);
			UpdateIR(ws.IRState.IRSensors[2], lblIR3, lblIR3Raw, chkFound3, Color.Yellow);
			UpdateIR(ws.IRState.IRSensors[3], lblIR4, lblIR4Raw, chkFound4, Color.Orange);

			if(ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found)
				g.DrawEllipse(new Pen(Color.Green), (int)(ws.IRState.RawMidpoint.X / 4), (int)(ws.IRState.RawMidpoint.Y / 4), 5, 5);

			pbIR.Image = b;

			pbBattery.Value = (ws.Battery > 0xc8 ? 0xc8 : (int)ws.Battery);
			lblBattery.Text = ws.Battery.ToString();
			lblDevicePath.Text = "Device Path: " + mWiimote.HIDDevicePath;
		}

	    private void UpdateIR(IRSensor irSensor, Label lblNorm, Label lblRaw, CheckBox chkFound, Color color)
		{
			chkFound.Checked = irSensor.Found;

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

        private void lblMPCallibrate_Click(object sender, EventArgs e)
        {
            mWiimote.CalibrateMotionPlus();
        }
    }
}
