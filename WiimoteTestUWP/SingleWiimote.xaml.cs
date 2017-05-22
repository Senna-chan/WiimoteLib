using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;
using WiiMoteLibUWP;
using WiiMoteLibUWP.DataTypes;
using WiiMoteLibUWP.DataTypes.Enums;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WiimoteTestUWP
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SingleWiimote : Page
    {
        private WriteableBitmap b = new WriteableBitmap(256, 192);
        private Wiimote mWiimote;
        public SingleWiimote()
        {
            this.InitializeComponent();
            mWiimote = new Wiimote();
            mWiimote.Connect();
            mWiimote.WiimoteChanged += wm_WiimoteChanged;
            mWiimote.WiimoteExtensionChanged += wm_WiimoteExtensionChanged;
            mWiimote.SetReportType(InputReport.IRAccel, true);
            mWiimote.SetLEDs(false, true, true, false);
        }

        private void wm_WiimoteChanged(object sender, WiimoteChangedEventArgs args)
        {
            UpdateState(args);
        }

        private void wm_WiimoteExtensionChanged(object sender, WiimoteExtensionChangedEventArgs args)
        {
            UpdateExtension(args);

            if (args.Inserted)
                mWiimote.SetReportType(InputReport.IRExtensionAccel, true);
            else
                mWiimote.SetReportType(InputReport.IRAccel, true);
        }
        public async void UpdateState(WiimoteChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                UpdateWiimoteChanged(args);
            });
            
        }
        private void chkLED_CheckedChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            mWiimote.SetLEDs(chkLED1.IsChecked.HasValue, chkLED2.IsChecked.HasValue, chkLED3.IsChecked.HasValue, chkLED4.IsChecked.HasValue);
        }

        private void chkRumble_CheckedChanged(object sender, RoutedEventArgs routedEventArgs)
        {
            mWiimote.SetRumble(chkRumble.IsChecked.HasValue);
        }

        private void UpdateWiimoteChanged(WiimoteChangedEventArgs args)
        {
            WiimoteState ws = args.WiimoteState;

            ((CheckBox)clbButtons.Items[0]).IsChecked = ws.ButtonState.A;
            ((CheckBox)clbButtons.Items[1]).IsChecked = ws.ButtonState.B;
            ((CheckBox)clbButtons.Items[2]).IsChecked = ws.ButtonState.Minus;
            ((CheckBox)clbButtons.Items[3]).IsChecked = ws.ButtonState.Home;
            ((CheckBox)clbButtons.Items[4]).IsChecked = ws.ButtonState.Plus;
            ((CheckBox)clbButtons.Items[5]).IsChecked = ws.ButtonState.One;
            ((CheckBox)clbButtons.Items[6]).IsChecked = ws.ButtonState.Two;
            ((CheckBox)clbButtons.Items[7]).IsChecked = ws.ButtonState.Up;
            ((CheckBox)clbButtons.Items[8]).IsChecked = ws.ButtonState.Down;
            ((CheckBox)clbButtons.Items[9]).IsChecked = ws.ButtonState.Left;
            ((CheckBox)clbButtons.Items[10]).IsChecked = ws.ButtonState.Right;

            lblAccel.Text = ws.AccelState.Values.ToString();

            chkLED1.IsChecked = ws.LEDState.LED1;
            chkLED2.IsChecked = ws.LEDState.LED2;
            chkLED3.IsChecked = ws.LEDState.LED3;
            chkLED4.IsChecked = ws.LEDState.LED4;

            switch (ws.ExtensionType)
            {
                case ExtensionType.Nunchuk:
                    lblChuk.Text = ws.NunchukState.AccelState.Values.ToString();
                    lblChukJoy.Text = ws.NunchukState.Joystick.ToString();
                    chkC.IsChecked = ws.NunchukState.C;
                    chkZ.IsChecked = ws.NunchukState.Z;
                    break;

                case ExtensionType.ClassicController:
                    ((CheckBox)clbCCButtons.Items[0 ]).IsChecked = ws.ClassicControllerState.ButtonState.A;
                    ((CheckBox)clbCCButtons.Items[1 ]).IsChecked = ws.ClassicControllerState.ButtonState.B;
                    ((CheckBox)clbCCButtons.Items[2 ]).IsChecked = ws.ClassicControllerState.ButtonState.X;
                    ((CheckBox)clbCCButtons.Items[3 ]).IsChecked = ws.ClassicControllerState.ButtonState.Y;
                    ((CheckBox)clbCCButtons.Items[4 ]).IsChecked = ws.ClassicControllerState.ButtonState.Minus;
                    ((CheckBox)clbCCButtons.Items[5 ]).IsChecked = ws.ClassicControllerState.ButtonState.Home;
                    ((CheckBox)clbCCButtons.Items[6 ]).IsChecked = ws.ClassicControllerState.ButtonState.Plus;
                    ((CheckBox)clbCCButtons.Items[7 ]).IsChecked = ws.ClassicControllerState.ButtonState.Up;
                    ((CheckBox)clbCCButtons.Items[8 ]).IsChecked = ws.ClassicControllerState.ButtonState.Down;
                    ((CheckBox)clbCCButtons.Items[9 ]).IsChecked = ws.ClassicControllerState.ButtonState.Left;
                    ((CheckBox)clbCCButtons.Items[10]).IsChecked = ws.ClassicControllerState.ButtonState.Right;
                    ((CheckBox)clbCCButtons.Items[11]).IsChecked = ws.ClassicControllerState.ButtonState.ZL;
                    ((CheckBox)clbCCButtons.Items[12]).IsChecked = ws.ClassicControllerState.ButtonState.ZR;
                    ((CheckBox)clbCCButtons.Items[13]).IsChecked = ws.ClassicControllerState.ButtonState.TriggerL;
                    ((CheckBox)clbCCButtons.Items[14]).IsChecked = ws.ClassicControllerState.ButtonState.TriggerR;

                    lblCCJoy1.Text = ws.ClassicControllerState.JoystickL.ToString();
                    lblCCJoy2.Text = ws.ClassicControllerState.JoystickR.ToString();

                    lblTriggerL.Text = ws.ClassicControllerState.TriggerL.ToString();
                    lblTriggerR.Text = ws.ClassicControllerState.TriggerR.ToString();
                    break;

                case ExtensionType.Guitar:
                    ((CheckBox)clbGuitarButtons.Items[0]).IsChecked = ws.GuitarState.FretButtonState.Green;
                    ((CheckBox)clbGuitarButtons.Items[1]).IsChecked = ws.GuitarState.FretButtonState.Red;
                    ((CheckBox)clbGuitarButtons.Items[2]).IsChecked = ws.GuitarState.FretButtonState.Yellow;
                    ((CheckBox)clbGuitarButtons.Items[3]).IsChecked = ws.GuitarState.FretButtonState.Blue;
                    ((CheckBox)clbGuitarButtons.Items[4]).IsChecked = ws.GuitarState.FretButtonState.Orange;
                    ((CheckBox)clbGuitarButtons.Items[5]).IsChecked = ws.GuitarState.ButtonState.    Minus;
                    ((CheckBox)clbGuitarButtons.Items[6]).IsChecked = ws.GuitarState.ButtonState.    Plus;
                    ((CheckBox)clbGuitarButtons.Items[7]).IsChecked = ws.GuitarState.ButtonState.    StrumUp;
                    ((CheckBox)clbGuitarButtons.Items[8]).IsChecked = ws.GuitarState.ButtonState.    StrumDown;

                    ((CheckBox)clbTouchbar.Items[0]).IsChecked = ws.GuitarState.TouchbarState.Green;
                    ((CheckBox)clbTouchbar.Items[1]).IsChecked = ws.GuitarState.TouchbarState.Red;
                    ((CheckBox)clbTouchbar.Items[2]).IsChecked = ws.GuitarState.TouchbarState.Yellow;
                    ((CheckBox)clbTouchbar.Items[3]).IsChecked = ws.GuitarState.TouchbarState.Blue;
                    ((CheckBox)clbTouchbar.Items[4]).IsChecked = ws.GuitarState.TouchbarState.Orange;

                    lblGuitarJoy.Text = ws.GuitarState.Joystick.ToString();
                    lblGuitarWhammy.Text = ws.GuitarState.WhammyBar.ToString();
                    lblGuitarType.Text = ws.GuitarState.GuitarType.ToString();
                    break;

                case ExtensionType.Drums:
                    ((CheckBox)clbDrums.Items[0]).IsChecked = ws.DrumsState.Red;
                    ((CheckBox)clbDrums.Items[1]).IsChecked = ws.DrumsState.Blue;
                    ((CheckBox)clbDrums.Items[2]).IsChecked = ws.DrumsState.Green;
                    ((CheckBox)clbDrums.Items[3]).IsChecked = ws.DrumsState.Yellow;
                    ((CheckBox)clbDrums.Items[4]).IsChecked = ws.DrumsState.Orange;
                    ((CheckBox)clbDrums.Items[5]).IsChecked = ws.DrumsState.Pedal;
                    ((CheckBox)clbDrums.Items[6]).IsChecked = ws.DrumsState.Minus;
                    ((CheckBox)clbDrums.Items[7]).IsChecked = ws.DrumsState.Plus;

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
                    if (chkLbs.IsChecked.HasValue && chkLbs.IsChecked.Value)
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
                    lblMPRawRoll.Text = ws.MotionPlusState.GyroRaw.Y.ToString();
                    lblMPRawYaw.Text = ws.MotionPlusState.GyroRaw.Z.ToString();

                    chcMPYawSlow.IsChecked = ws.MotionPlusState.SlowYaw;
                    chcMPRollSlow.IsChecked = ws.MotionPlusState.SlowRoll;
                    chcMPPitchSlow.IsChecked = ws.MotionPlusState.SlowPitch;

                    lblMPPitch.Text = ws.MotionPlusState.Gyro.X.ToString("000.00");
                    lblMPRoll.Text = ws.MotionPlusState.Gyro.Y.ToString("000.00");
                    lblMPYaw.Text = ws.MotionPlusState.Gyro.Z.ToString("000.00");
                    break;
            }

            b.Clear(Color.FromArgb(255,255,255,255));

            UpdateIR(ws.IRState.IRSensors[0], lblIR1, lblIR1Raw, chkFound1, Color.FromArgb(255,255,0,0));
            UpdateIR(ws.IRState.IRSensors[1], lblIR2, lblIR2Raw, chkFound2, Color.FromArgb(255, 0, 255, 0));
            UpdateIR(ws.IRState.IRSensors[2], lblIR3, lblIR3Raw, chkFound3, Color.FromArgb(255, 255, 255, 0));
            UpdateIR(ws.IRState.IRSensors[3], lblIR4, lblIR4Raw, chkFound4, Color.FromArgb(255, 255, 127, 0));

            if (ws.IRState.IRSensors[0].Found && ws.IRState.IRSensors[1].Found)
                b.DrawEllipse((int)(ws.IRState.RawMidpoint.X / 4), (int)(ws.IRState.RawMidpoint.Y / 4), 5, 5, Color.FromArgb(255, 0, 0, 255));

            pbIR.Source = b;

            pbBattery.Value = (ws.Battery > 0xc8 ? 0xc8 : (int)ws.Battery);
            lblBattery.Text = ws.Battery.ToString();
            lblDevicePath.Text = "Device Path: " + mWiimote.HIDDevicePath;
        }

        private void UpdateIR(IRSensor irSensor, TextBlock lblNorm, TextBlock lblRaw, CheckBox chkFound, Color color)
        {
            chkFound.IsChecked = irSensor.Found;

            if (irSensor.Found)
            {
                lblNorm.Text = irSensor.Position.ToString() + ", " + irSensor.Size;
                lblRaw.Text = irSensor.RawPosition.ToString();
                b.DrawEllipse((int)(irSensor.RawPosition.X / 4), (int)(irSensor.RawPosition.Y / 4),
                             irSensor.Size + 1, irSensor.Size + 1, color);
            }
        }
        public async void UpdateExtension(WiimoteExtensionChangedEventArgs args)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => {
                UpdateExtensionChanged(args);
            });
        }

        private void UpdateExtensionChanged(WiimoteExtensionChangedEventArgs args)
        {
            chkExtension.Content = args.ExtensionType.ToString();
            chkExtension.IsChecked = args.Inserted;
        }
        private void btnMPConnect_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            if (mWiimote.WiimoteState.ExtensionType == ExtensionType.MotionPlus)
            {
                mWiimote.DisconnectMotionPlus();
                btnMPConnect.Content = "Connect";
            }
            else
            {
                mWiimote.ConnectMotionPlus();
                btnMPConnect.Content = "Disconnect";
            }

        }

        private void lblMPCallibrate_Click(object sender, RoutedEventArgs routedEventArgs)
        {
            mWiimote.CallibrateMotionPlus();
        }
    }
}
