using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;

namespace WiiPair
{
    internal class Program
    {
        private static bool pairViaSync = true;
        private static byte[] createPassword(BluetoothAddress address)
        {
            byte[] password = new byte[6];
            byte[] addrBytes = address.ToByteArray();
            password[0] = addrBytes[5];
            password[1] = addrBytes[4];
            password[2] = addrBytes[3];
            password[3] = addrBytes[2];
            password[4] = addrBytes[1];
            password[5] = addrBytes[0];
            return password;
        }
        static void Main(string[] args)
        {
            BluetoothClient client = new BluetoothClient();
            BluetoothRadio radio = BluetoothRadio.Default;
            var hostAddressObj = radio.LocalAddress;
            byte[] wiimotePassword = createPassword(hostAddressObj);
            BluetoothDeviceInfo device = null;
            do
            {
                var discoveredDevices = client.DiscoverDevices();
                foreach (var dev in discoveredDevices)
                {
                    Console.WriteLine($"Found device {dev.DeviceName} with address {dev.DeviceAddress.ToString()}");
                    if (dev.DeviceName.Contains("Nintendo"))
                    {
                        device = dev;
                        break;
                    }
                }
            } while (device == null);

            bool wiimoteReady = device.Authenticated;

            if (!device.Authenticated)
            {
                if(!pairViaSync)
                {
                    wiimotePassword = createPassword(device.DeviceAddress);
                }
                string passwordStr = Encoding.Default.GetString(wiimotePassword);
                bool result = BluetoothSecurity.PairRequest(device.DeviceAddress, passwordStr, false);
                Console.WriteLine($"Paired {result}");
                wiimoteReady = result;
            }
            if (!wiimoteReady)
            {
                Console.WriteLine("Failed to connect to the wiimote");
                Environment.Exit(1);
            }
            device.Refresh();
            System.Diagnostics.Debug.WriteLine(device.Authenticated);

            client.Connect(device.DeviceAddress, BluetoothService.HumanInterfaceDevice);

            client.Close();
        }
    }
}
