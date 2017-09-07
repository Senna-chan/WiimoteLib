using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Devices.HumanInterfaceDevice;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace WiiPairUWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MainViewModel viewModel;
        byte[] password = new byte[6];
        public MainPage()
        {
            this.InitializeComponent();
            viewModel = (MainViewModel)this.DataContext;
            ConnectToWiimote();
        }

        public async void ConnectToWiimote()
        {
            var found = false;
            /*BluetoothAdapter bluetoothAdapter = await Bluetooth.GetDefaultAsync();
            do
            {
                if (bluetoothAdapter == null)
                {
                    viewModel.ConsoleLog = "Please connect a bluetooth adapter.\r\n";
                    await Task.Delay(1000);
                }
                bluetoothAdapter = await BluetoothAdapter.GetDefaultAsync();
            } while (bluetoothAdapter == null);
            var bluetoothAddress = BitConverter.GetBytes(bluetoothAdapter.BluetoothAddress);
            
            password[0] = bluetoothAddress[0];
            password[1] = bluetoothAddress[1];
            password[2] = bluetoothAddress[2];
            password[3] = bluetoothAddress[3];
            password[4] = bluetoothAddress[4];
            password[5] = bluetoothAddress[5];
*/

            //            int i = 0;
            //            char[] g = new char[6];
            //            foreach (var pSection in password)
            //            {
            //                g[i] = (char) pSection;
            //                i++;
            //            }
            //            var h = g.ToString();
            //var selector = BluetoothDevice.GetDeviceSelector();
            // Checks for synced/paired wiidevices
            var selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
            ConsoleWriteLine("Checking for paired wii devices.");
            var devices = await DeviceInformation.FindAllAsync(selector);
            {
                if (devices.Count != 0)
                {
                    foreach (var device in devices)
                    {
                        if (device.Name.Contains("Nintendo"))
                        {
                            var result = await device.Pairing.UnpairAsync();
                            if (result.Status != DeviceUnpairingResultStatus.Unpaired)
                            {
                                ConsoleWriteLine("There was a error unpairing a wii device");
                            }
                            else
                            {
                                ConsoleWriteLine("Succesfully unpaired a wii device");
                            }
                        }
                    }
                }
            }
            // Checks for new devices
            selector = BluetoothDevice.GetDeviceSelectorFromPairingState(false);
            while (!found)
            {
                ConsoleWriteLine("Searching for devices");
                devices = await DeviceInformation.FindAllAsync(selector);
                if (devices.Count != 0)
                {
                    ConsoleWriteLine($"Found {devices.Count} devices.");
                    foreach (var device in devices)
                    {
                        if (device.Name.Contains("Nintendo"))
                        {
                            ConsoleWriteLine("One device is a Wiimote.");
                            device.Pairing.Custom.PairingRequested += Custom_PairingRequested;
                            var pairingResult = await device.Pairing.Custom.PairAsync(DevicePairingKinds.ProvidePin);
                            if (pairingResult.Status != DevicePairingResultStatus.Paired)
                            {
                                ConsoleWriteLine($"Error: {pairingResult.Status}");
                            }
                            else
                            {
                                var hidSelector = HidDevice.GetDeviceSelector(0x01, 0x05);
                                await Task.Delay(500);
                                var hidDevices = await DeviceInformation.FindAllAsync(hidSelector);

                                if (hidDevices.Count > 0)
                                {
                                    HidDevice hidDevice = await HidDevice.FromIdAsync(hidDevices.ElementAt(0).Id,
                                                       FileAccessMode.Read);
                                    found = true;

                                }
                                else
                                {
                                    ConsoleWriteLine("Didn't find a wiimote with HID");
                                }
                            }
                        }
                    }
                }
                else
                {
                    ConsoleWriteLine("Nothing Found.");
                }
                await Task.Delay(1000);
            }
        }

        private void BlDevice_ConnectionStatusChanged(BluetoothDevice sender, object args)
        {
            if (sender.Name == "Nintendo")
            {
                ConsoleWriteLine("Aaaaaah. Wiiremote is disconnected");
            }
        }

        private void Custom_PairingRequested(DeviceInformationCustomPairing sender, DevicePairingRequestedEventArgs args)
        {
            args.Accept();
        }

        private void ConsoleWriteLine(string text, bool newLine = true)
        {
            viewModel.ConsoleLog += $"{text}";
            if(newLine) viewModel.ConsoleLog += "\r\n";
        }
    }
}
