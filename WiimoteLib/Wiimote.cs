//////////////////////////////////////////////////////////////////////////////////
//	Wiimote.cs
//	Managed Wiimote Library
//	Written by Brian Peek (http://www.brianpeek.com/)
//	for MSDN's Coding4Fun (http://msdn.microsoft.com/coding4fun/)
//	Visit http://blogs.msdn.com/coding4fun/archive/2007/03/14/1879033.aspx
//  and http://www.codeplex.com/WiimoteLib
//	for more information
//////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Microsoft.Win32.SafeHandles;
using System.Threading;
using System.Threading.Tasks;
using MathFloat;
using WiimoteLib.DataTypes;
using WiimoteLib.DataTypes.Enums;
using WiimoteLib.Exceptions;
using WiimoteLib.Helpers;
using ThreadState = System.Threading.ThreadState;
using Timer = System.Timers.Timer;

namespace WiimoteLib
{
    /// <summary>
    /// Implementation of Wiimote
    /// </summary>
    public class Wiimote : IDisposable
    {
        /// <summary>
        /// Event raised when Wiimote state is changed
        /// </summary>
        public event EventHandler<WiimoteChangedEventArgs> WiimoteChanged;

        /// <summary>
        /// Event raised when an extension is inserted or removed
        /// </summary>
        public event EventHandler<WiimoteExtensionChangedEventArgs> WiimoteExtensionChanged;

        // VID = Nintendo, PID = Wiimote
        private const int VID = 0x057e;
        private const int PIDO = 0x0306;
        private const int PIDN = 0x0330;

        // sure, we could find this out the hard way using HID, but trust me, it's 22
        private const int REPORT_LENGTH = 22;

        private const float M_PI = 3.14159265F;

        // Wiimote output commands
        private enum OutputReport : byte
        {
            LEDs			= 0x11,
            Type			= 0x12,
            IR				= 0x13,
            SpeakerEnable   = 0x14,
            Status			= 0x15,
            WriteMemory		= 0x16,
            ReadMemory		= 0x17,
            SpeakerData     = 0x18,
            SpeakerMute     = 0x19,
            IR2				= 0x1a,
        };

        private static readonly List<int> FreqLookup = new List<int>{0, 4200, 3920, 3640, 3360,
            3130,	2940, 2760, 2610, 2470, 4410 };

        // Wiimote registers
        private const int REGISTER_IR				= 0x04b00030;
        private const int REGISTER_IR_SENSITIVITY_1	= 0x04b00000;
        private const int REGISTER_IR_SENSITIVITY_2	= 0x04b0001a;
        private const int REGISTER_IR_MODE			= 0x04b00033;

        private const int REGISTER_EXTENSION_INIT_1			= 0x04a400f0;
        private const int REGISTER_EXTENSION_INIT_2			= 0x04a400fb;
        private const int REGISTER_EXTENSION_EXT_INIT_1     = 0x04a600f0;
        private const int REGISTER_EXTENSION_EXT_INIT_2     = 0x04a600fb;
        private const int REGISTER_EXTENSION_TYPE			= 0x04a400fa;
        private const int REGISTER_EXTENSION_CALIBRATION	= 0x04a40020;
        private const int REGISTER_MOTIONPLUS_DETECT        = 0x04a600fa;
        private const int REGISTER_MOTIONPLUS_INIT          = 0x04a600f0;
        private const int REGISTER_MOTIONPLUS_ENABLE        = 0x04a600fe;
        private const int REGISTER_MOTIONPLUS_DISABLE       = 0x04a400f0;

        // length between board sensors
        private const int BSL = 43;

        // width between board sensors
        private const int BSW = 24;

        // read/write handle to the device
        private SafeFileHandle mHandle;

        // a pretty .NET stream to read/write from/to
        private FileStream mStream;

        // report buffer
        private readonly byte[] mBuff = new byte[REPORT_LENGTH];

        // read data buffer
        private byte[] mReadBuff;

        // address to read from
        private int mAddress;

        // size of requested read
        private short mSize;

        // current state of controller
        private readonly WiimoteState mWiimoteState = new WiimoteState();

        // event for read data processing
        private readonly AutoResetEvent mReadDone = new AutoResetEvent(false);
        private readonly AutoResetEvent mWriteDone = new AutoResetEvent(false);

        // event for status report
        private readonly AutoResetEvent mStatusDone = new AutoResetEvent(false);

        // use a different method to write reports
        private bool mAltWriteMethod = false;
        private bool mAltWriteMethodWriteFile = false;

        // HID device path of this Wiimote
        private string mDevicePath = string.Empty;

        // unique ID
        private readonly Guid mID = Guid.NewGuid();

        // delegate used for enumerating found Wiimotes
        internal delegate bool WiimoteFoundDelegate(string devicePath);

        // kilograms to pounds
        private const float KG2LB = 2.20462262f;

        // Motion plus
        private const float ADC_TO_DEG_S = 8192 / 595;
        private const float FAST_MULTIPLIER = 2000 / 440;

        // Prevent MotionPlus from turning off upon init
        private bool mSuppressExtensionInit = false;

        private Mahony filter = new Mahony(100);
        private WiimoteAudioSample currentSample = null;
        private Thread SampleThread = null;
        public int FreqOverride = 0;

        private CancellationTokenSource _sampleThreadCancellationTokenSource = new CancellationTokenSource();
        private bool _stopSampleThread;


        /// <summary>
        /// Default constructor
        /// </summary>
        public Wiimote()
        {
        }

        internal Wiimote(string devicePath)
        {
            mDevicePath = devicePath;
        }

        /// <summary>
        /// Connect to the first-found Wiimote
        /// </summary>
        /// <exception cref="WiimoteNotFoundException">Wiimote not found in HID device list</exception>
        public void Connect()
        {
            if(string.IsNullOrEmpty(mDevicePath))
                FindWiimote(WiimoteFound);
            else
                OpenWiimoteDeviceHandle(mDevicePath);
        }

        internal static void FindWiimote(WiimoteFoundDelegate wiimoteFound)
        {
            int index = 0;
            bool found = false;
            Guid guid;
            SafeFileHandle mHandle;

            // get the GUID of the HID class
            HIDImports.HidD_GetHidGuid(out guid);

            // get a handle to all devices that are part of the HID class
            // Fun fact:  DIGCF_PRESENT worked on my machine just fine.  I reinstalled Vista, and now it no longer finds the Wiimote with that parameter enabled...
            IntPtr hDevInfo = HIDImports.SetupDiGetClassDevs(ref guid, null, IntPtr.Zero, HIDImports.DIGCF_DEVICEINTERFACE);// | HIDImports.DIGCF_PRESENT);

            // create a new interface data struct and initialize its size
            HIDImports.SP_DEVICE_INTERFACE_DATA diData = new HIDImports.SP_DEVICE_INTERFACE_DATA();
            diData.cbSize = Marshal.SizeOf(diData);

            // get a device interface to a single device (enumerate all devices)
            while (HIDImports.SetupDiEnumDeviceInterfaces(hDevInfo, IntPtr.Zero, ref guid, index, ref diData))
            {
                UInt32 size;

                // get the buffer size for this device detail instance (returned in the size parameter)
                HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, IntPtr.Zero, 0, out size, IntPtr.Zero);

                // create a detail struct and set its size
                HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA diDetail = new HIDImports.SP_DEVICE_INTERFACE_DETAIL_DATA();

                // yeah, yeah...well, see, on Win x86, cbSize must be 5 for some reason.  On x64, apparently 8 is what it wants.
                // someday I should figure this out.  Thanks to Paul Miller on this...
                diDetail.cbSize = (uint)(IntPtr.Size == 8 ? 8 : 5);

                // actually get the detail struct
                if (HIDImports.SetupDiGetDeviceInterfaceDetail(hDevInfo, ref diData, ref diDetail, size, out size, IntPtr.Zero))
                {
                    Debug.WriteLine(string.Format("{0}: {1} - {2}", index, diDetail.DevicePath, Marshal.GetLastWin32Error()));

                    // open a read/write handle to our device using the DevicePath returned
                    mHandle = HIDImports.CreateFile(diDetail.DevicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

                    // create an attributes struct and initialize the size
                    HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
                    attrib.Size = Marshal.SizeOf(attrib);

                    // get the attributes of the current device
                    if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
                    {
                        // if the vendor and product IDs match up
                        if (attrib.VendorID == VID && (attrib.ProductID == PIDN || attrib.ProductID == PIDO))
                        {
                            // it's a Wiimote
                            Debug.WriteLine("Found one!");
                            found = true;

                            // fire the callback function...if the callee doesn't care about more Wiimotes, break out
                            if (!wiimoteFound(diDetail.DevicePath))
                                break;
                        }
                    }
                    mHandle.Close();
                }
                else
                {
                    // failed to get the detail struct
                    throw new WiimoteException("SetupDiGetDeviceInterfaceDetail failed on index " + index);
                }

                // move to the next device
                index++;
            }

            // clean up our list
            HIDImports.SetupDiDestroyDeviceInfoList(hDevInfo);

            // if we didn't find a Wiimote, throw an exception
            if (!found)
                throw new WiimoteNotFoundException("No Wiimotes found in HID device list.");
        }

        private bool WiimoteFound(string devicePath)
        {
            mDevicePath = devicePath;

            // if we didn't find a Wiimote, throw an exception
            OpenWiimoteDeviceHandle(mDevicePath);

            return false;
        }

        private void OpenWiimoteDeviceHandle(string devicePath)
        {
            // open a read/write handle to our device using the DevicePath returned
            mHandle = HIDImports.CreateFile(devicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, HIDImports.EFileAttributes.Overlapped, IntPtr.Zero);

            // create an attributes struct and initialize the size
            HIDImports.HIDD_ATTRIBUTES attrib = new HIDImports.HIDD_ATTRIBUTES();
            attrib.Size = Marshal.SizeOf(attrib);

            // get the attributes of the current device
            if (HIDImports.HidD_GetAttributes(mHandle.DangerousGetHandle(), ref attrib))
            {
                // if the vendor and product IDs match up
                if (attrib.VendorID == VID && (attrib.ProductID == PIDN || attrib.ProductID == PIDO))
                {
                    // create a nice .NET FileStream wrapping the handle above
                    mStream = new FileStream(mHandle, FileAccess.ReadWrite, REPORT_LENGTH, true);

                    // start an async read operation on it
                    BeginAsyncRead();

                    // read the calibration info from the controller
                    try
                    {
                        ReadWiimoteCalibration();
                    }
                    catch
                    {
                        // if we fail above, try the alternate HID writes
                        mAltWriteMethod = true;
                        ReadWiimoteCalibration();
                    }

                    // force a status check to get the state of any extensions plugged in at startup
                    GetStatus();
                }
                else
                {
                    // otherwise this isn't the controller, so close up the file handle
                    mHandle.Close();
                    throw new WiimoteException("Attempted to open a non-Wiimote device.");
                }
            }
        }

        /// <summary>
        /// Disconnect from the controller and stop reading data from it
        /// </summary>
        public void Disconnect()
        {
            // close up the stream and handle
            mStream?.Close();

            mHandle?.Close();
        }

        /// <summary>
        /// MotionPlus
        /// </summary>
        public void ConnectMotionPlus()
        {
            mSuppressExtensionInit = true;
            WriteData(REGISTER_EXTENSION_EXT_INIT_1, 0x55);
            WriteData(REGISTER_EXTENSION_INIT_1, 0x55);
            if (mWiimoteState.ExtensionType == ExtensionType.Nunchuk)
            {
                WriteData(0x04a600fe, 0x05);
            } 
            else if(mWiimoteState.ExtensionType == ExtensionType.ClassicController)
            {
                WriteData(0x04a600fe, 0x07);
            }
            else
            {
                WriteData(0x04a600fe, 0x04);
            }
        }
        /// <summary>
        /// Disconnects the MotionPlus
        /// </summary>
        public void DisconnectMotionPlus()
        {
            WriteData(REGISTER_EXTENSION_INIT_1, 0x55); // Deactivate motion plus
        }
        /// <summary>
        /// Calibrade the Motion Plus
        /// </summary>
        public void CalibrateMotionPlus()
        {
            mWiimoteState.MotionPlus.Offset.X = mWiimoteState.MotionPlus.GyroRaw.X;
            mWiimoteState.MotionPlus.Offset.Y = mWiimoteState.MotionPlus.GyroRaw.Y;
            mWiimoteState.MotionPlus.Offset.Z = mWiimoteState.MotionPlus.GyroRaw.Z;

            WriteData(0x0016, 3, new byte[] { 124, 124, 125 });
            WriteData(0x0020, 3, new byte[] { 149, 151, 151 });
        }

        /// <summary>
        /// Start reading asynchronously from the controller
        /// </summary>
        private void BeginAsyncRead()
        {
            // if the stream is valid and ready
            if(mStream != null && mStream.CanRead)
            {
                // setup the read and the callback
                byte[] buff = new byte[REPORT_LENGTH];
                mStream.BeginRead(buff, 0, REPORT_LENGTH, new AsyncCallback(OnReadData), buff);
            }
        }

        /// <summary>
        /// Callback when data is ready to be processed
        /// </summary>
        /// <param name="ar">State information for the callback</param>
        private void OnReadData(IAsyncResult ar)
        {
            // grab the byte buffer
            byte[] buff = (byte[])ar.AsyncState;

            try
            {
                // end the current read
                mStream.EndRead(ar);

                // parse it
                try
                {
                    if (ParseInputReport(buff))
                    {
                        // post an event
                        WiimoteChanged?.Invoke(this, new WiimoteChangedEventArgs(mWiimoteState));
                    }
                }
                catch (WiimoteException e)
                {
                    Console.WriteLine(e.StackTrace);
                }
                // start reading again
                BeginAsyncRead();
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("OperationCanceledException");
            }
            catch (IOException e)
            {
                throw new WiimoteNotFoundException("The wiimote is not on.");
            }
        }

        /// <summary>
        /// Parse a report sent by the Wiimote
        /// </summary>
        /// <param name="buff">Data buffer to parse</param>
        /// <returns>Returns a boolean noting whether an event needs to be posted</returns>
        private bool ParseInputReport(byte[] buff)
        {
            InputReport type = (InputReport)buff[0];
            //Console.WriteLine("InputReport: " + type.ToString());
            switch (type)
            {
                case InputReport.Buttons:
                    ParseButtons(buff);
                    break;
                case InputReport.ButtonsAccel:
                    ParseButtons(buff);
                    ParseAccel(buff);
                    break;
                case InputReport.IRAccel:
                    ParseButtons(buff);
                    ParseAccel(buff);
                    ParseIR(buff);
                    break;
                case InputReport.ButtonsExtension:
                    ParseButtons(buff);
                    ParseExtension(buff, 3);
                    break;
                case InputReport.ExtensionAccel:
                    ParseButtons(buff);
                    ParseAccel(buff);
                    ParseExtension(buff, 6);
                    break;
                case InputReport.IRExtensionAccel:
                    ParseButtons(buff);
                    ParseAccel(buff);
                    ParseIR(buff);
                    ParseExtension(buff, 16);
                    break;
                case InputReport.Status:
                    ParseButtons(buff);
                    mWiimoteState.BatteryRaw = buff[6];
                    mWiimoteState.Battery = (((100.0f * 48.0f * (float)((int)buff[6] / 48.0f))) / 192.0f);

                    // get the real LED values in case the values from SetLEDs() somehow becomes out of sync, which really shouldn't be possible
                    mWiimoteState.LED.LED1 = (buff[3] & 0x10) != 0;
                    mWiimoteState.LED.LED2 = (buff[3] & 0x20) != 0;
                    mWiimoteState.LED.LED3 = (buff[3] & 0x40) != 0;
                    mWiimoteState.LED.LED4 = (buff[3] & 0x80) != 0;

                    // extension connected?
                    bool extension = (buff[3] & 0x02) != 0;
                    Console.WriteLine("Extension: " + extension);

                    if(mWiimoteState.Extension != extension)
                    {
                        mWiimoteState.Extension = extension;

                        if(extension)
                        {
                            BeginAsyncRead();
                            InitializeExtension();
                        }
                        else
                            mWiimoteState.ExtensionType = ExtensionType.None;

                        // only fire the extension changed event if we have a real extension (i.e. not a balance board)
                        if(WiimoteExtensionChanged != null && mWiimoteState.ExtensionType != ExtensionType.BalanceBoard)
                            WiimoteExtensionChanged(this, new WiimoteExtensionChangedEventArgs(mWiimoteState.ExtensionType, mWiimoteState.Extension));
                    }
                    mStatusDone.Set();
                    break;
                case InputReport.ReadData:
                    ParseButtons(buff);
                    ParseReadData(buff);
                    break;
                case InputReport.OutputReportAck:
                    Console.Write($"Ack outputReport: {(OutputReport)buff[3]} (0x{buff[3]:X}) Error Code: ");
                    if (buff[4] == 0)
                    {
                        Console.WriteLine("success");
                    }
                    else if(buff[4] == 3)
                    {
                        Console.WriteLine("error");
                    } else if (buff[4] == 4)
                    {
                        Console.WriteLine("unknown: 0x16 0x17 0x18");
                    }else if (buff[4] == 5)
                    {
                        Console.WriteLine("unknown: 0x12");
                    }else if (buff[4] == 8)
                    {
                        Console.WriteLine("unknown: 0x16");
                    }
                    else
                    {
                        Console.WriteLine(buff[4].ToString("X"));
                    }
                    mWriteDone.Set();
                    break;
                default:
                    Console.WriteLine("Unknown report type: " + type.ToString("x"));
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Handles setting up an extension when plugged in
        /// </summary>
        private void InitializeExtension()
        {
            if (!mSuppressExtensionInit)
            {
                WriteData(REGISTER_EXTENSION_INIT_1, 0x55);
                WriteData(REGISTER_EXTENSION_INIT_2, 0x00);
                mSuppressExtensionInit = false;
            }
            // start reading again
            BeginAsyncRead();

            byte[] buff = ReadData(REGISTER_EXTENSION_TYPE, 6);
            long type = ((long)buff[0] << 40) | ((long)buff[1] << 32) | ((long)buff[2]) << 24 | ((long)buff[3]) << 16 | ((long)buff[4]) << 8 | buff[5];

            Debug.Print("Extension type: ", type.ToString("x"));

            switch((ExtensionType)type)
            {
                case ExtensionType.None:
                case ExtensionType.ParitallyInserted:
                    mWiimoteState.Extension = false;
                    mWiimoteState.ExtensionType = ExtensionType.None;
                    return;
                case ExtensionType.MotionPlus:
                case ExtensionType.Nunchuk:
                case ExtensionType.ClassicController:
                case ExtensionType.Guitar:
                case ExtensionType.BalanceBoard:
                case ExtensionType.Drums:
                case ExtensionType.UDraw:
                    mWiimoteState.ExtensionType = (ExtensionType)type;
                    this.SetReportType(InputReport.ButtonsExtension, true);
                    break;
                default:
                    mWiimoteState.ExtensionType = ExtensionType.Unknown;
                    this.SetReportType(InputReport.ButtonsExtension, true);
                    Console.WriteLine("Unknown extension controller found: " + type.ToString("x"));
                    break;
            }

            switch(mWiimoteState.ExtensionType)
            {
                case ExtensionType.Nunchuk:
                    buff = ReadData(REGISTER_EXTENSION_CALIBRATION, 16);

                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.X0 = buff[0];
                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Y0 = buff[1];
                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Z0 = buff[2];
                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.XG = buff[4];
                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.YG = buff[5];
                    mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.ZG = buff[6];
                    mWiimoteState.Nunchuk.CalibrationInfo.MaxX = buff[8];
                    mWiimoteState.Nunchuk.CalibrationInfo.MinX = buff[9];
                    mWiimoteState.Nunchuk.CalibrationInfo.MidX = buff[10];
                    mWiimoteState.Nunchuk.CalibrationInfo.MaxY = buff[11];
                    mWiimoteState.Nunchuk.CalibrationInfo.MinY = buff[12];
                    mWiimoteState.Nunchuk.CalibrationInfo.MidY = buff[13];
                    break;
                case ExtensionType.ClassicController:
                    buff = ReadData(REGISTER_EXTENSION_CALIBRATION, 16);

                    mWiimoteState.ClassicController.CalibrationInfo.MaxXL = (byte)(buff[0] >> 2);
                    mWiimoteState.ClassicController.CalibrationInfo.MinXL = (byte)(buff[1] >> 2);
                    mWiimoteState.ClassicController.CalibrationInfo.MidXL = (byte)(buff[2] >> 2);
                    mWiimoteState.ClassicController.CalibrationInfo.MaxYL = (byte)(buff[3] >> 2);
                    mWiimoteState.ClassicController.CalibrationInfo.MinYL = (byte)(buff[4] >> 2);
                    mWiimoteState.ClassicController.CalibrationInfo.MidYL = (byte)(buff[5] >> 2);

                    mWiimoteState.ClassicController.CalibrationInfo.MaxXR = (byte)(buff[6] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MinXR = (byte)(buff[7] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MidXR = (byte)(buff[8] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MaxYR = (byte)(buff[9] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MinYR = (byte)(buff[10] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MidYR = (byte)(buff[11] >> 3);

                    // this doesn't seem right...
//					mWiimoteState.ClassicController.AccelCalibrationInfo.MinTriggerL = (byte)(buff[12] >> 3);
//					mWiimoteState.ClassicController.AccelCalibrationInfo.MaxTriggerL = (byte)(buff[14] >> 3);
//					mWiimoteState.ClassicController.AccelCalibrationInfo.MinTriggerR = (byte)(buff[13] >> 3);
//					mWiimoteState.ClassicController.AccelCalibrationInfo.MaxTriggerR = (byte)(buff[15] >> 3);
                    mWiimoteState.ClassicController.CalibrationInfo.MinTriggerL = 0;
                    mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerL = 31;
                    mWiimoteState.ClassicController.CalibrationInfo.MinTriggerR = 0;
                    mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerR = 31;
                    break;
                case ExtensionType.Guitar:
                case ExtensionType.Drums:
                    // there appears to be no calibration data returned by the guitar controller
                    break;
                case ExtensionType.BalanceBoard:
                    buff = ReadData(REGISTER_EXTENSION_CALIBRATION, 32);

                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.TopRight =		(short)((short)buff[4] << 8 | buff[5]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.BottomRight =	(short)((short)buff[6] << 8 | buff[7]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.TopLeft =		(short)((short)buff[8] << 8 | buff[9]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.BottomLeft =	(short)((short)buff[10] << 8 | buff[11]);

                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.TopRight =		(short)((short)buff[12] << 8 | buff[13]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.BottomRight =	(short)((short)buff[14] << 8 | buff[15]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.TopLeft =		(short)((short)buff[16] << 8 | buff[17]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.BottomLeft =	(short)((short)buff[18] << 8 | buff[19]);

                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.TopRight =		(short)((short)buff[20] << 8 | buff[21]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.BottomRight =	(short)((short)buff[22] << 8 | buff[23]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.TopLeft =		(short)((short)buff[24] << 8 | buff[25]);
                    mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.BottomLeft =	(short)((short)buff[26] << 8 | buff[27]);
                    break;
            }
        }

        /// <summary>
        /// Decrypts data sent from the extension to the Wiimote
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <returns>Byte array containing decoded data</returns>
        private byte[] DecryptBuffer(byte[] buff)
        {
            for(int i = 0; i < buff.Length; i++)
                buff[i] = (byte)(((buff[i] ^ 0x17) + 0x17) & 0xff);

            return buff;
        }

        /// <summary>
        /// Parses a standard button report into the Buttons struct
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseButtons(byte[] buff)
        {
            mWiimoteState.OldButtons = mWiimoteState.Buttons;
            mWiimoteState.Buttons.A		= (buff[2] & 0x08) != 0;
            mWiimoteState.Buttons.B		= (buff[2] & 0x04) != 0;
            mWiimoteState.Buttons.Minus	= (buff[2] & 0x10) != 0;
            mWiimoteState.Buttons.Home	= (buff[2] & 0x80) != 0;
            mWiimoteState.Buttons.Plus	= (buff[1] & 0x10) != 0;
            mWiimoteState.Buttons.One	= (buff[2] & 0x02) != 0;
            mWiimoteState.Buttons.Two	= (buff[2] & 0x01) != 0;
            mWiimoteState.Buttons.Up	= (buff[1] & 0x08) != 0;
            mWiimoteState.Buttons.Down	= (buff[1] & 0x04) != 0;
            mWiimoteState.Buttons.Left	= (buff[1] & 0x01) != 0;
            mWiimoteState.Buttons.Right	= (buff[1] & 0x02) != 0;
        }
        

        /// <summary>
        /// Parse accelerometer data
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseAccel(byte[] buff)
        {
            mWiimoteState.Accel.RawValues.X = buff[3];
            mWiimoteState.Accel.RawValues.Y = buff[4];
            mWiimoteState.Accel.RawValues.Z = buff[5];
            mWiimoteState.Accel.Values.X = (float)((float)mWiimoteState.Accel.RawValues.X - mWiimoteState.AccelCalibrationInfo.X0) / 
                                            ((float)mWiimoteState.AccelCalibrationInfo.XG - mWiimoteState.AccelCalibrationInfo.X0);
            mWiimoteState.Accel.Values.Y = (float)((float)mWiimoteState.Accel.RawValues.Y - mWiimoteState.AccelCalibrationInfo.Y0) /
                                            ((float)mWiimoteState.AccelCalibrationInfo.YG - mWiimoteState.AccelCalibrationInfo.Y0);
            mWiimoteState.Accel.Values.Z = (float)((float)mWiimoteState.Accel.RawValues.Z - mWiimoteState.AccelCalibrationInfo.Z0) /
                                            ((float)mWiimoteState.AccelCalibrationInfo.ZG - mWiimoteState.AccelCalibrationInfo.Z0);

            
            mWiimoteState.Accel.IMU.Pitch = (float)(Math.Atan2(-mWiimoteState.Accel.Values.Y, mWiimoteState.Accel.Values.Z) * 180.0) / M_PI;
            mWiimoteState.Accel.IMU.Roll = (float)(Math.Atan2(-mWiimoteState.Accel.Values.X, Math.Sqrt(mWiimoteState.Accel.Values.Y * mWiimoteState.Accel.Values.Y + mWiimoteState.Accel.Values.Z * mWiimoteState.Accel.Values.Z)) * 180.0) / M_PI;
            
            
        }

        /// <summary>
        /// Parse IR data from report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseIR(byte[] buff)
        {
            mWiimoteState.IR.IRSensors[0].RawPosition.X = buff[6] | ((buff[8] >> 4) & 0x03) << 8;
            mWiimoteState.IR.IRSensors[0].RawPosition.Y = buff[7] | ((buff[8] >> 6) & 0x03) << 8;

            switch(mWiimoteState.IR.Mode)
            {
                case IRMode.Basic:
                    mWiimoteState.IR.IRSensors[1].RawPosition.X = buff[9]  | ((buff[8] >> 0) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[1].RawPosition.Y = buff[10] | ((buff[8] >> 2) & 0x03) << 8;

                    mWiimoteState.IR.IRSensors[2].RawPosition.X = buff[11] | ((buff[13] >> 4) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[2].RawPosition.Y = buff[12] | ((buff[13] >> 6) & 0x03) << 8;

                    mWiimoteState.IR.IRSensors[3].RawPosition.X = buff[14] | ((buff[13] >> 0) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[3].RawPosition.Y = buff[15] | ((buff[13] >> 2) & 0x03) << 8;

                    mWiimoteState.IR.IRSensors[0].Size = 0x00;
                    mWiimoteState.IR.IRSensors[1].Size = 0x00;
                    mWiimoteState.IR.IRSensors[2].Size = 0x00;
                    mWiimoteState.IR.IRSensors[3].Size = 0x00;

                    mWiimoteState.IR.IRSensors[0].Found = !(buff[6] == 0xff && buff[7] == 0xff);
                    mWiimoteState.IR.IRSensors[1].Found = !(buff[9] == 0xff && buff[10] == 0xff);
                    mWiimoteState.IR.IRSensors[2].Found = !(buff[11] == 0xff && buff[12] == 0xff);
                    mWiimoteState.IR.IRSensors[3].Found = !(buff[14] == 0xff && buff[15] == 0xff);
                    break;
                case IRMode.Extended:
                    mWiimoteState.IR.IRSensors[1].RawPosition.X = buff[9]  | ((buff[11] >> 4) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[1].RawPosition.Y = buff[10] | ((buff[11] >> 6) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[2].RawPosition.X = buff[12] | ((buff[14] >> 4) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[2].RawPosition.Y = buff[13] | ((buff[14] >> 6) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[3].RawPosition.X = buff[15] | ((buff[17] >> 4) & 0x03) << 8;
                    mWiimoteState.IR.IRSensors[3].RawPosition.Y = buff[16] | ((buff[17] >> 6) & 0x03) << 8;

                    mWiimoteState.IR.IRSensors[0].Size = buff[8] & 0x0f;
                    mWiimoteState.IR.IRSensors[1].Size = buff[11] & 0x0f;
                    mWiimoteState.IR.IRSensors[2].Size = buff[14] & 0x0f;
                    mWiimoteState.IR.IRSensors[3].Size = buff[17] & 0x0f;

                    mWiimoteState.IR.IRSensors[0].Found = !(buff[6] == 0xff && buff[7] == 0xff && buff[8] == 0xff);
                    mWiimoteState.IR.IRSensors[1].Found = !(buff[9] == 0xff && buff[10] == 0xff && buff[11] == 0xff);
                    mWiimoteState.IR.IRSensors[2].Found = !(buff[12] == 0xff && buff[13] == 0xff && buff[14] == 0xff);
                    mWiimoteState.IR.IRSensors[3].Found = !(buff[15] == 0xff && buff[16] == 0xff && buff[17] == 0xff);
                    break;
            }

            mWiimoteState.IR.IRSensors[0].Position.X = (float)(mWiimoteState.IR.IRSensors[0].RawPosition.X / 1023.5f);
            mWiimoteState.IR.IRSensors[1].Position.X = (float)(mWiimoteState.IR.IRSensors[1].RawPosition.X / 1023.5f);
            mWiimoteState.IR.IRSensors[2].Position.X = (float)(mWiimoteState.IR.IRSensors[2].RawPosition.X / 1023.5f);
            mWiimoteState.IR.IRSensors[3].Position.X = (float)(mWiimoteState.IR.IRSensors[3].RawPosition.X / 1023.5f);
                           
            mWiimoteState.IR.IRSensors[0].Position.Y = (float)(mWiimoteState.IR.IRSensors[0].RawPosition.Y / 767.5f);
            mWiimoteState.IR.IRSensors[1].Position.Y = (float)(mWiimoteState.IR.IRSensors[1].RawPosition.Y / 767.5f);
            mWiimoteState.IR.IRSensors[2].Position.Y = (float)(mWiimoteState.IR.IRSensors[2].RawPosition.Y / 767.5f);
            mWiimoteState.IR.IRSensors[3].Position.Y = (float)(mWiimoteState.IR.IRSensors[3].RawPosition.Y / 767.5f);

            if(mWiimoteState.IR.IRSensors[0].Found && mWiimoteState.IR.IRSensors[1].Found)
            {
                mWiimoteState.IR.RawMidpoint.X = (mWiimoteState.IR.IRSensors[1].RawPosition.X + mWiimoteState.IR.IRSensors[0].RawPosition.X) / 2;
                mWiimoteState.IR.RawMidpoint.Y = (mWiimoteState.IR.IRSensors[1].RawPosition.Y + mWiimoteState.IR.IRSensors[0].RawPosition.Y) / 2;
        
                mWiimoteState.IR.Midpoint.X = (mWiimoteState.IR.IRSensors[1].Position.X + mWiimoteState.IR.IRSensors[0].Position.X) / 2.0f;
                mWiimoteState.IR.Midpoint.Y = (mWiimoteState.IR.IRSensors[1].Position.Y + mWiimoteState.IR.IRSensors[0].Position.Y) / 2.0f;
            }
            else
                mWiimoteState.IR.Midpoint.X = mWiimoteState.IR.Midpoint.Y = 0.0f;
        }

        /// <summary>
        /// Parse data from an extension controller
        /// </summary>
        /// <param name="buff">Data buffer</param>
        /// <param name="offset">Offset into data buffer</param>
        private void ParseExtension(byte[] buff, int offset)
        {

            mWiimoteState.RawBuff = buff;
            switch (mWiimoteState.ExtensionType)
            {
                case ExtensionType.Nunchuk:
                    mWiimoteState.Nunchuk.RawJoystick.X = buff[offset];
                    mWiimoteState.Nunchuk.RawJoystick.Y = buff[offset + 1];
                    mWiimoteState.Nunchuk.Accel.RawValues.X = buff[offset + 2];
                    mWiimoteState.Nunchuk.Accel.RawValues.Y = buff[offset + 3];
                    mWiimoteState.Nunchuk.Accel.RawValues.Z = buff[offset + 4];

                    mWiimoteState.Nunchuk.C = (buff[offset + 5] & 0x02) == 0;
                    mWiimoteState.Nunchuk.Z = (buff[offset + 5] & 0x01) == 0;

                    mWiimoteState.Nunchuk.Accel.Values.X = (float)((float)mWiimoteState.Nunchuk.Accel.RawValues.X - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.X0) / 
                                               ((float)mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.XG - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.X0);
                    mWiimoteState.Nunchuk.Accel.Values.Y = (float)((float)mWiimoteState.Nunchuk.Accel.RawValues.Y - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Y0) /
                                               ((float)mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.YG - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Y0);
                    mWiimoteState.Nunchuk.Accel.Values.Z = (float)((float)mWiimoteState.Nunchuk.Accel.RawValues.Z - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Z0) /
                                                    ((float)mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.ZG - mWiimoteState.Nunchuk.CalibrationInfo.AccelCalibration.Z0);




                    mWiimoteState.Nunchuk.Accel.IMU.Pitch = (float)(Math.Atan2(-mWiimoteState.Nunchuk.Accel.Values.Y, mWiimoteState.Nunchuk.Accel.Values.Z) * 180.0) / M_PI;
                    mWiimoteState.Nunchuk.Accel.IMU.Roll = (float)(Math.Atan2(-mWiimoteState.Nunchuk.Accel.Values.X, Math.Sqrt(mWiimoteState.Nunchuk.Accel.Values.Y * mWiimoteState.Nunchuk.Accel.Values.Y + mWiimoteState.Nunchuk.Accel.Values.Z * mWiimoteState.Nunchuk.Accel.Values.Z)) * 180.0) / M_PI;

                    if (mWiimoteState.Nunchuk.CalibrationInfo.MaxX != 0x00)
                        mWiimoteState.Nunchuk.Joystick.X = (float)(((float)mWiimoteState.Nunchuk.RawJoystick.X - mWiimoteState.Nunchuk.CalibrationInfo.MidX) / 
                                                ((float)mWiimoteState.Nunchuk.CalibrationInfo.MaxX - mWiimoteState.Nunchuk.CalibrationInfo.MinX)) * 2 * 100;

                    if(mWiimoteState.Nunchuk.CalibrationInfo.MaxY != 0x00)
                        mWiimoteState.Nunchuk.Joystick.Y = (float)(((float)mWiimoteState.Nunchuk.RawJoystick.Y - mWiimoteState.Nunchuk.CalibrationInfo.MidY) / 
                            ((float)mWiimoteState.Nunchuk.CalibrationInfo.MaxY - mWiimoteState.Nunchuk.CalibrationInfo.MinY)) * 2 * 100;

                    break;

                case ExtensionType.ClassicController:
                    mWiimoteState.ClassicController.OldButtons = mWiimoteState.ClassicController.Buttons;
                    mWiimoteState.ClassicController.RawJoystickL.X = (byte)(buff[offset] & 0x3f);
                    mWiimoteState.ClassicController.RawJoystickL.Y = (byte)(buff[offset + 1] & 0x3f);
                    mWiimoteState.ClassicController.RawJoystickR.X = (byte)((buff[offset + 2] >> 7) | (buff[offset + 1] & 0xc0) >> 5 | (buff[offset] & 0xc0) >> 3);
                    mWiimoteState.ClassicController.RawJoystickR.Y = (byte)(buff[offset + 2] & 0x1f);

                    mWiimoteState.ClassicController.RawTriggerL = (byte)(((buff[offset + 2] & 0x60) >> 2) | (buff[offset + 3] >> 5));
                    mWiimoteState.ClassicController.RawTriggerR = (byte)(buff[offset + 3] & 0x1f);

                    mWiimoteState.ClassicController.Buttons.TriggerR	= (buff[offset + 4] & 0x02) == 0;
                    mWiimoteState.ClassicController.Buttons.Plus		= (buff[offset + 4] & 0x04) == 0;
                    mWiimoteState.ClassicController.Buttons.Home		= (buff[offset + 4] & 0x08) == 0;
                    mWiimoteState.ClassicController.Buttons.Minus		= (buff[offset + 4] & 0x10) == 0;
                    mWiimoteState.ClassicController.Buttons.TriggerL	= (buff[offset + 4] & 0x20) == 0;
                    mWiimoteState.ClassicController.Buttons.Down		= (buff[offset + 4] & 0x40) == 0;
                    mWiimoteState.ClassicController.Buttons.Right		= (buff[offset + 4] & 0x80) == 0;

                    mWiimoteState.ClassicController.Buttons.Up			= (buff[offset + 5] & 0x01) == 0;
                    mWiimoteState.ClassicController.Buttons.Left		= (buff[offset + 5] & 0x02) == 0;
                    mWiimoteState.ClassicController.Buttons.ZR			= (buff[offset + 5] & 0x04) == 0;
                    mWiimoteState.ClassicController.Buttons.X			= (buff[offset + 5] & 0x08) == 0;
                    mWiimoteState.ClassicController.Buttons.A			= (buff[offset + 5] & 0x10) == 0;
                    mWiimoteState.ClassicController.Buttons.Y			= (buff[offset + 5] & 0x20) == 0;
                    mWiimoteState.ClassicController.Buttons.B			= (buff[offset + 5] & 0x40) == 0;
                    mWiimoteState.ClassicController.Buttons.ZL			= (buff[offset + 5] & 0x80) == 0;

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxXL != 0x00)
                        mWiimoteState.ClassicController.JoystickL.X = (float)((float)mWiimoteState.ClassicController.RawJoystickL.X - mWiimoteState.ClassicController.CalibrationInfo.MidXL) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxXL - mWiimoteState.ClassicController.CalibrationInfo.MinXL);

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxYL != 0x00)
                        mWiimoteState.ClassicController.JoystickL.Y = (float)((float)mWiimoteState.ClassicController.RawJoystickL.Y - mWiimoteState.ClassicController.CalibrationInfo.MidYL) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxYL - mWiimoteState.ClassicController.CalibrationInfo.MinYL);

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxXR != 0x00)
                        mWiimoteState.ClassicController.JoystickR.X = (float)((float)mWiimoteState.ClassicController.RawJoystickR.X - mWiimoteState.ClassicController.CalibrationInfo.MidXR) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxXR - mWiimoteState.ClassicController.CalibrationInfo.MinXR);

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxYR != 0x00)
                        mWiimoteState.ClassicController.JoystickR.Y = (float)((float)mWiimoteState.ClassicController.RawJoystickR.Y - mWiimoteState.ClassicController.CalibrationInfo.MidYR) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxYR - mWiimoteState.ClassicController.CalibrationInfo.MinYR);

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerL != 0x00)
                        mWiimoteState.ClassicController.TriggerL = (mWiimoteState.ClassicController.RawTriggerL) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerL - mWiimoteState.ClassicController.CalibrationInfo.MinTriggerL);

                    if(mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerR != 0x00)
                        mWiimoteState.ClassicController.TriggerR = (mWiimoteState.ClassicController.RawTriggerR) / 
                        (float)(mWiimoteState.ClassicController.CalibrationInfo.MaxTriggerR - mWiimoteState.ClassicController.CalibrationInfo.MinTriggerR);
                    break;

                case ExtensionType.Guitar:
                    mWiimoteState.Guitar.GuitarType = ((buff[offset] & 0x80) == 0) ? GuitarType.GuitarHeroWorldTour : GuitarType.GuitarHero3;

                    mWiimoteState.Guitar.Buttons.Plus		= (buff[offset + 4] & 0x04) == 0;
                    mWiimoteState.Guitar.Buttons.Minus		= (buff[offset + 4] & 0x10) == 0;
                    mWiimoteState.Guitar.Buttons.StrumDown	= (buff[offset + 4] & 0x40) == 0;

                    mWiimoteState.Guitar.Buttons.StrumUp		= (buff[offset + 5] & 0x01) == 0;
                    mWiimoteState.Guitar.FretButtons.Yellow	= (buff[offset + 5] & 0x08) == 0;
                    mWiimoteState.Guitar.FretButtons.Green		= (buff[offset + 5] & 0x10) == 0;
                    mWiimoteState.Guitar.FretButtons.Blue		= (buff[offset + 5] & 0x20) == 0;
                    mWiimoteState.Guitar.FretButtons.Red		= (buff[offset + 5] & 0x40) == 0;
                    mWiimoteState.Guitar.FretButtons.Orange	= (buff[offset + 5] & 0x80) == 0;

                    // it appears the joystick values are only 6 bits
                    mWiimoteState.Guitar.RawJoystick.X	= (buff[offset + 0] & 0x3f);
                    mWiimoteState.Guitar.RawJoystick.Y	= (buff[offset + 1] & 0x3f);

                    // and the whammy bar is only 5 bits
                    mWiimoteState.Guitar.RawWhammyBar			= (byte)(buff[offset + 3] & 0x1f);

                    mWiimoteState.Guitar.Joystick.X			= (float)(mWiimoteState.Guitar.RawJoystick.X - 0x1f) / 0x3f;	// not fully accurate, but close
                    mWiimoteState.Guitar.Joystick.Y			= (float)(mWiimoteState.Guitar.RawJoystick.Y - 0x1f) / 0x3f;	// not fully accurate, but close
                    mWiimoteState.Guitar.WhammyBar				= (float)(mWiimoteState.Guitar.RawWhammyBar) / 0x0a;	// seems like there are 10 positions?

                    mWiimoteState.Guitar.Touchbar.Yellow	= false;
                    mWiimoteState.Guitar.Touchbar.Green	= false;
                    mWiimoteState.Guitar.Touchbar.Blue	= false;
                    mWiimoteState.Guitar.Touchbar.Red		= false;
                    mWiimoteState.Guitar.Touchbar.Orange	= false;

                    switch(buff[offset + 2] & 0x1f)
                    {
                        case 0x04:
                            mWiimoteState.Guitar.Touchbar.Green = true;
                            break;
                        case 0x07:
                            mWiimoteState.Guitar.Touchbar.Green = true;
                            mWiimoteState.Guitar.Touchbar.Red = true;
                            break;
                        case 0x0a:
                            mWiimoteState.Guitar.Touchbar.Red = true;
                            break;
                        case 0x0c:
                        case 0x0d:
                            mWiimoteState.Guitar.Touchbar.Red = true;
                            mWiimoteState.Guitar.Touchbar.Yellow = true;
                            break;
                        case 0x12:
                        case 0x13:
                            mWiimoteState.Guitar.Touchbar.Yellow = true;
                            break;
                        case 0x14:
                        case 0x15:
                            mWiimoteState.Guitar.Touchbar.Yellow = true;
                            mWiimoteState.Guitar.Touchbar.Blue = true;
                            break;
                        case 0x17:
                        case 0x18:
                            mWiimoteState.Guitar.Touchbar.Blue = true;
                            break;
                        case 0x1a:
                            mWiimoteState.Guitar.Touchbar.Blue = true;
                            mWiimoteState.Guitar.Touchbar.Orange = true;
                            break;
                        case 0x1f:
                            mWiimoteState.Guitar.Touchbar.Orange = true;
                            break;
                    }
                    break;
                case ExtensionType.UDraw:
                    mWiimoteState.Tablet.OldPoint = mWiimoteState.Tablet.Point;
                    mWiimoteState.Tablet.OldButtonUp=mWiimoteState.Tablet.ButtonUp;
                    mWiimoteState.Tablet.OldButtonDown=mWiimoteState.Tablet.ButtonDown;
                    const int FIRST_BOX_START = 0x62;

                    //Ge(t the pressure state
                    if (buff[offset+2] == 0xFF)
                        mWiimoteState.Tablet.PressureType = TabletPressure.NotPressed;
                    else
                        mWiimoteState.Tablet.PressureType = TabletPressure.PenPressed;

                    //Get the pen pressure
                    mWiimoteState.Tablet.PenPressure = (ushort)(buff[3]);

                    //Get the (singular) pressure point
                    mWiimoteState.Tablet.BoxPosition.X = (buff[offset + 2] & 0x0F);
                    mWiimoteState.Tablet.BoxPosition.Y = ((buff[offset + 2] >> 4) & 0x0F);
                    mWiimoteState.Tablet.RawPosition.X = buff[offset + 0];
                    mWiimoteState.Tablet.RawPosition.Y = buff[offset + 1];
                    var x = mWiimoteState.Tablet.BoxPosition.X == 0 ? mWiimoteState.Tablet.RawPosition.X - FIRST_BOX_START : (0x100 - FIRST_BOX_START) + ((mWiimoteState.Tablet.BoxPosition.X - 1) * 0x100) + mWiimoteState.Tablet.RawPosition.X;
                    var y = mWiimoteState.Tablet.BoxPosition.Y == 0 ? mWiimoteState.Tablet.RawPosition.Y - FIRST_BOX_START : (0x100 - FIRST_BOX_START) + ((mWiimoteState.Tablet.BoxPosition.Y - 1) * 0x100) + mWiimoteState.Tablet.RawPosition.Y;
                    mWiimoteState.Tablet.Position.X = x;
                    mWiimoteState.Tablet.Position.Y = 1340 - y;
                    mWiimoteState.Tablet.Point = (buff[offset + 5] & 0x04) == 4; // For some reason this is inverted
                    mWiimoteState.Tablet.ButtonUp = (buff[offset + 5] & 0x01) == 0;
                    mWiimoteState.Tablet.ButtonDown = (buff[offset + 5] & 0x02) == 0;
                    break;
                case ExtensionType.Drums:
                    // it appears the joystick values are only 6 bits
                    mWiimoteState.Drums.RawJoystick.X	= (buff[offset + 0] & 0x3f);
                    mWiimoteState.Drums.RawJoystick.Y	= (buff[offset + 1] & 0x3f);

                    mWiimoteState.Drums.Plus			= (buff[offset + 4] & 0x04) == 0;
                    mWiimoteState.Drums.Minus			= (buff[offset + 4] & 0x10) == 0;

                    mWiimoteState.Drums.Pedal			= (buff[offset + 5] & 0x04) == 0;
                    mWiimoteState.Drums.Blue			= (buff[offset + 5] & 0x08) == 0;
                    mWiimoteState.Drums.Green			= (buff[offset + 5] & 0x10) == 0;
                    mWiimoteState.Drums.Yellow			= (buff[offset + 5] & 0x20) == 0;
                    mWiimoteState.Drums.Red			= (buff[offset + 5] & 0x40) == 0;
                    mWiimoteState.Drums.Orange			= (buff[offset + 5] & 0x80) == 0;

                    mWiimoteState.Drums.Joystick.X		= (float)(mWiimoteState.Drums.RawJoystick.X - 0x1f) / 0x3f;	// not fully accurate, but close
                    mWiimoteState.Drums.Joystick.Y		= (float)(mWiimoteState.Drums.RawJoystick.Y - 0x1f) / 0x3f;	// not fully accurate, but close

                    if((buff[offset + 2] & 0x40) == 0)
                    {
                        int pad = (buff[offset + 2] >> 1) & 0x1f;
                        int velocity = (buff[offset + 3] >> 5);

                        if(velocity != 7)
                        {
                            switch(pad)
                            {
                                case 0x1b:
                                    mWiimoteState.Drums.PedalVelocity = velocity;
                                    break;
                                case 0x19:
                                    mWiimoteState.Drums.RedVelocity = velocity;
                                    break;
                                case 0x11:
                                    mWiimoteState.Drums.YellowVelocity = velocity;
                                    break;
                                case 0x0f:
                                    mWiimoteState.Drums.BlueVelocity = velocity;
                                    break;
                                case 0x0e:
                                    mWiimoteState.Drums.OrangeVelocity = velocity;
                                    break;
                                case 0x12:
                                    mWiimoteState.Drums.GreenVelocity = velocity;
                                    break;
                            }
                        }
                    }

                    break;

                case ExtensionType.BalanceBoard:
                    mWiimoteState.BalanceBoard.SensorValuesRaw.TopRight = (short)((short)buff[offset + 0] << 8 | buff[offset + 1]);
                    mWiimoteState.BalanceBoard.SensorValuesRaw.BottomRight = (short)((short)buff[offset + 2] << 8 | buff[offset + 3]);
                    mWiimoteState.BalanceBoard.SensorValuesRaw.TopLeft = (short)((short)buff[offset + 4] << 8 | buff[offset + 5]);
                    mWiimoteState.BalanceBoard.SensorValuesRaw.BottomLeft = (short)((short)buff[offset + 6] << 8 | buff[offset + 7]);

                    mWiimoteState.BalanceBoard.SensorValuesKg.TopLeft = GetBalanceBoardSensorValue(mWiimoteState.BalanceBoard.SensorValuesRaw.TopLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.TopLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.TopLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.TopLeft);
                    mWiimoteState.BalanceBoard.SensorValuesKg.TopRight = GetBalanceBoardSensorValue(mWiimoteState.BalanceBoard.SensorValuesRaw.TopRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.TopRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.TopRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.TopRight);
                    mWiimoteState.BalanceBoard.SensorValuesKg.BottomLeft = GetBalanceBoardSensorValue(mWiimoteState.BalanceBoard.SensorValuesRaw.BottomLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.BottomLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.BottomLeft, mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.BottomLeft);
                    mWiimoteState.BalanceBoard.SensorValuesKg.BottomRight = GetBalanceBoardSensorValue(mWiimoteState.BalanceBoard.SensorValuesRaw.BottomRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg0.BottomRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg17.BottomRight, mWiimoteState.BalanceBoard.CalibrationInfo.Kg34.BottomRight);

                    mWiimoteState.BalanceBoard.SensorValuesLb.TopLeft = (mWiimoteState.BalanceBoard.SensorValuesKg.TopLeft * KG2LB);
                    mWiimoteState.BalanceBoard.SensorValuesLb.TopRight = (mWiimoteState.BalanceBoard.SensorValuesKg.TopRight * KG2LB);
                    mWiimoteState.BalanceBoard.SensorValuesLb.BottomLeft = (mWiimoteState.BalanceBoard.SensorValuesKg.BottomLeft * KG2LB);
                    mWiimoteState.BalanceBoard.SensorValuesLb.BottomRight = (mWiimoteState.BalanceBoard.SensorValuesKg.BottomRight * KG2LB);

                    mWiimoteState.BalanceBoard.WeightKg = (mWiimoteState.BalanceBoard.SensorValuesKg.TopLeft + mWiimoteState.BalanceBoard.SensorValuesKg.TopRight + mWiimoteState.BalanceBoard.SensorValuesKg.BottomLeft + mWiimoteState.BalanceBoard.SensorValuesKg.BottomRight) / 4.0f;
                    mWiimoteState.BalanceBoard.WeightLb = (mWiimoteState.BalanceBoard.SensorValuesLb.TopLeft + mWiimoteState.BalanceBoard.SensorValuesLb.TopRight + mWiimoteState.BalanceBoard.SensorValuesLb.BottomLeft + mWiimoteState.BalanceBoard.SensorValuesLb.BottomRight) / 4.0f;

                    float Kx = (mWiimoteState.BalanceBoard.SensorValuesKg.TopLeft + mWiimoteState.BalanceBoard.SensorValuesKg.BottomLeft) / (mWiimoteState.BalanceBoard.SensorValuesKg.TopRight + mWiimoteState.BalanceBoard.SensorValuesKg.BottomRight);
                    float Ky = (mWiimoteState.BalanceBoard.SensorValuesKg.TopLeft + mWiimoteState.BalanceBoard.SensorValuesKg.TopRight) / (mWiimoteState.BalanceBoard.SensorValuesKg.BottomLeft + mWiimoteState.BalanceBoard.SensorValuesKg.BottomRight);

                    mWiimoteState.BalanceBoard.CenterOfGravity.X = ((float)(Kx - 1) / (float)(Kx + 1)) * (float)(-BSL / 2);
                    mWiimoteState.BalanceBoard.CenterOfGravity.Y = ((float)(Ky - 1) / (float)(Ky + 1)) * (float)(-BSW / 2);
                    break;
                case ExtensionType.MotionPlus:

                    // Yaw
                    mWiimoteState.MotionPlus.GyroRaw.Z
                        = ((buff[offset + 3] & 0xF6) << 6) + buff[offset + 0] ;

                    // Roll
                    mWiimoteState.MotionPlus.GyroRaw.Y
                        = ((buff[offset + 4] & 0xF6) << 6) + buff[offset + 1];

                    // Pitch
                    mWiimoteState.MotionPlus.GyroRaw.X
                        = ((buff[offset + 5] & 0xF6) << 6) + buff[offset + 2];
                    if (mWiimoteState.MotionPlus.Offset.Equals(new Point3()))
                    {
                        CalibrateMotionPlus();
                    }

                    // Slow flags
                    mWiimoteState.MotionPlus.SlowYaw = ((buff[offset + 3] >> 1) & 0x01) == 1;
                    mWiimoteState.MotionPlus.SlowPitch = ((buff[offset + 3] >> 0) & 0x01) == 1;
                    mWiimoteState.MotionPlus.SlowRoll = ((buff[offset + 4] >> 1) & 0x01) == 1;


                    // Convert to deg/s
                    mWiimoteState.MotionPlus.Gyro.Z =
                            (mWiimoteState.MotionPlus.GyroRaw.Z - mWiimoteState.MotionPlus.Offset.Z)
                            / ADC_TO_DEG_S;

                    mWiimoteState.MotionPlus.Gyro.Y =
                            (mWiimoteState.MotionPlus.GyroRaw.Y - mWiimoteState.MotionPlus.Offset.Y)
                            / ADC_TO_DEG_S;

                    mWiimoteState.MotionPlus.Gyro.X =
                            (mWiimoteState.MotionPlus.GyroRaw.X - mWiimoteState.MotionPlus.Offset.X)
                            / ADC_TO_DEG_S;

                    // Slow / fast mode
                    if (!mWiimoteState.MotionPlus.SlowYaw)
                    {
                        mWiimoteState.MotionPlus.Gyro.Z *= FAST_MULTIPLIER;
                    }

                    if (!mWiimoteState.MotionPlus.SlowRoll)
                    {
                        mWiimoteState.MotionPlus.Gyro.Y *= FAST_MULTIPLIER;
                    }

                    if (!mWiimoteState.MotionPlus.SlowPitch)
                    {
                        mWiimoteState.MotionPlus.Gyro.X *= FAST_MULTIPLIER;
                    }

                    filter.UpdateImu(mWiimoteState.MotionPlus.Gyro.X, mWiimoteState.MotionPlus.Gyro.Y,
                        mWiimoteState.MotionPlus.Gyro.Z, mWiimoteState.Accel.Values.X,
                        mWiimoteState.Accel.Values.Y, mWiimoteState.Accel.Values.Z);

                    mWiimoteState.MotionPlus.IMU.Roll = filter.GetRoll();
                    mWiimoteState.MotionPlus.IMU.Pitch = filter.GetPitch();
                    mWiimoteState.MotionPlus.IMU.Yaw = filter.GetYaw();
                    break;
            }
        }

        private float GetBalanceBoardSensorValue(short sensor, short min, short mid, short max)
        {
            if(max == mid || mid == min)
                return 0;

            if(sensor < mid)
                return 68.0f * ((float)(sensor - min) / (mid - min));
            else
                return 68.0f * ((float)(sensor - mid) / (max - mid)) + 68.0f;
        }

        /// <summary>
        /// Parse data returned from a read report
        /// </summary>
        /// <param name="buff">Data buffer</param>
        private void ParseReadData(byte[] buff)
        {
            if((buff[3] & 0x08) != 0)
                throw new WiimoteException("Error reading data from Wiimote: Bytes do not exist.");

            if((buff[3] & 0x07) != 0)
                throw new WiimoteException("Error reading data from Wiimote: Attempt to read from write-only registers.");

            // get our size and offset from the report
            int size = (buff[3] >> 4) + 1;
            int offset = (buff[4] << 8 | buff[5]);

            // add it to the buffer
            Array.Copy(buff, 6, mReadBuff, offset - mAddress, size);

            // if we've read it all, set the event
            if (mAddress + mSize == offset + size)
                mReadDone.Set();
            else
            {
                Console.WriteLine("Something went wrong");
            }
        }

        /// <summary>
        /// Returns whether rumble is currently enabled.
        /// </summary>
        /// <returns>Byte indicating true (0x01) or false (0x00)</returns>
        private byte GetRumbleBit()
        {
            return (byte)(mWiimoteState.Rumble ? 0x01 : 0x00);
        }

        /// <summary>
        /// Read calibration information stored on Wiimote
        /// </summary>
        private void ReadWiimoteCalibration()
        {
            // this appears to change the report type to 0x31
            byte[] buff;
            buff = ReadData(0x0016, 7);
            if (buff[0] == 0 && buff[1] == 0 && buff[2] == 0 && buff[4] == 0 && buff[5] == 0 && buff[6] == 0)
            {
                buff = ReadData(0x0020, 7);
                if (buff[0] == 0 && buff[1] == 0 && buff[2] == 0 && buff[4] == 0 && buff[5] == 0 && buff[6] == 0)
                {
                    //Debug.Assert(false, "Something is wrong.\r\nCould not get CalibrationData.");
                    //throw new WiimoteException("Something is wrong.\r\nCould not get CalibrationData.");
                }
            }

            mWiimoteState.AccelCalibrationInfo.X0 = buff[0];
            mWiimoteState.AccelCalibrationInfo.Y0 = buff[1];
            mWiimoteState.AccelCalibrationInfo.Z0 = buff[2];
            mWiimoteState.AccelCalibrationInfo.XG = buff[4];
            mWiimoteState.AccelCalibrationInfo.YG = buff[5];
            mWiimoteState.AccelCalibrationInfo.ZG = buff[6];
        }

        /// <summary>
        /// Set Wiimote reporting mode (if using an IR report type, IR sensitivity is set to WiiLevel3)
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="continuous">Continuous data</param>
        public void SetReportType(InputReport type, bool continuous)
        {
            SetReportType(type, IRSensitivity.Maximum, continuous);
        }

        /// <summary>
        /// Set Wiimote reporting mode
        /// </summary>
        /// <param name="type">Report type</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        /// <param name="continuous">Continuous data</param>
        public void SetReportType(InputReport type, IRSensitivity irSensitivity, bool continuous)
        {
            // only 1 report type allowed for the BB
            if(mWiimoteState.ExtensionType == ExtensionType.BalanceBoard)
                type = InputReport.ButtonsExtension;

            switch(type)
            {
                case InputReport.IRAccel:
                    EnableIR(IRMode.Extended, irSensitivity);
                    break;
                case InputReport.IRExtensionAccel:
                    EnableIR(IRMode.Basic, irSensitivity);
                    break;
                default:
                    DisableIR();
                    break;
            }

            ClearReport();
            mBuff[0] = (byte)OutputReport.Type;
            mBuff[1] = (byte)((continuous ? 0x04 : 0x00) | (byte)(mWiimoteState.Rumble ? 0x01 : 0x00));
            mBuff[2] = (byte)type;

            WriteReport();
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="led1">LED 1</param>
        /// <param name="led2">LED 2</param>
        /// <param name="led3">LED 3</param>
        /// <param name="led4">LED 4</param>
        public void SetLEDs(bool led1, bool led2, bool led3, bool led4)
        {
            mWiimoteState.LED.LED1 = led1;
            mWiimoteState.LED.LED2 = led2;
            mWiimoteState.LED.LED3 = led3;
            mWiimoteState.LED.LED4 = led4;

            ClearReport();

            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] =	(byte)(
                        (led1 ? 0x10 : 0x00) |
                        (led2 ? 0x20 : 0x00) |
                        (led3 ? 0x40 : 0x00) |
                        (led4 ? 0x80 : 0x00) |
                        GetRumbleBit());

            WriteReport();
        }

        /// <summary>
        /// Set the LEDs on the Wiimote
        /// </summary>
        /// <param name="leds">The value to be lit up in base2 on the Wiimote</param>
        public void SetLEDs(int leds)
        {
            mWiimoteState.LED.LED1 = (leds & 0x01) > 0;
            mWiimoteState.LED.LED2 = (leds & 0x02) > 0;
            mWiimoteState.LED.LED3 = (leds & 0x04) > 0;
            mWiimoteState.LED.LED4 = (leds & 0x08) > 0;

            ClearReport();

            mBuff[0] = (byte)OutputReport.LEDs;
            mBuff[1] =	(byte)(
                        ((leds & 0x01) > 0 ? 0x10 : 0x00) |
                        ((leds & 0x02) > 0 ? 0x20 : 0x00) |
                        ((leds & 0x04) > 0 ? 0x40 : 0x00) |
                        ((leds & 0x08) > 0 ? 0x80 : 0x00) |
                        GetRumbleBit());

            WriteReport();
        }

        /// <summary>
        /// Toggle rumble
        /// </summary>
        /// <param name="on">On or off</param>
        public void SetRumble(bool on)
        {
            mWiimoteState.Rumble = on;

            // the LED report also handles rumble
            SetLEDs(mWiimoteState.LED.LED1, 
                    mWiimoteState.LED.LED2,
                    mWiimoteState.LED.LED3,
                    mWiimoteState.LED.LED4);
        }

        /// <summary>
        /// Retrieve the current status of the Wiimote and extensions.  Replaces GetBatteryLevel() since it was poorly named.
        /// </summary>
        public void GetStatus()
        {
            ClearReport();

            mBuff[0] = (byte)OutputReport.Status;
            mBuff[1] = GetRumbleBit();

            WriteReport();

            // signal the status report finished
            if(!mStatusDone.WaitOne(1000, false))
                throw new WiimoteException("Timed out waiting for status report");
        }

        /// <summary>
        /// Turn on the IR sensor
        /// </summary>
        /// <param name="mode">The data report mode</param>
        /// <param name="irSensitivity">IR sensitivity</param>
        private void EnableIR(IRMode mode, IRSensitivity irSensitivity)
        {
            mWiimoteState.IR.Mode = mode;

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = (byte)(0x04 | GetRumbleBit());
            WriteReport();

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = (byte)(0x04 | GetRumbleBit());
            WriteReport();

            WriteData(REGISTER_IR, 0x08);
            switch(irSensitivity)
            {
                case IRSensitivity.WiiLevel1:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x64, 0x00, 0xfe});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0xfd, 0x05});
                    break;
                case IRSensitivity.WiiLevel2:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x96, 0x00, 0xb4});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0xb3, 0x04});
                    break;
                case IRSensitivity.WiiLevel3:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xaa, 0x00, 0x64});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0x63, 0x03});
                    break;
                case IRSensitivity.WiiLevel4:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0xc8, 0x00, 0x36});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0x35, 0x03});
                    break;
                case IRSensitivity.WiiLevel5:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x07, 0x00, 0x00, 0x71, 0x01, 0x00, 0x72, 0x00, 0x20});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0x1, 0x03});
                    break;
                case IRSensitivity.Maximum:
                    WriteData(REGISTER_IR_SENSITIVITY_1, 9, new byte[] {0x02, 0x00, 0x00, 0x71, 0x01, 0x00, 0x90, 0x00, 0x41});
                    WriteData(REGISTER_IR_SENSITIVITY_2, 2, new byte[] {0x40, 0x00});
                    break;
                default:
                    throw new ArgumentOutOfRangeException("irSensitivity");
            }
            WriteData(REGISTER_IR_MODE, (byte)mode);
            WriteData(REGISTER_IR, 0x08);
        }

        /// <summary>
        /// Disable the IR sensor
        /// </summary>
        private void DisableIR()
        {
            mWiimoteState.IR.Mode = IRMode.Off;

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR;
            mBuff[1] = GetRumbleBit();
            WriteReport();

            ClearReport();
            mBuff[0] = (byte)OutputReport.IR2;
            mBuff[1] = GetRumbleBit();
            WriteReport();
        }

        /// <summary>
        /// Initialize the report data buffer
        /// </summary>
        private void ClearReport()
        {
            Array.Clear(mBuff, 0, REPORT_LENGTH);
        }

        /// <summary>
        /// Write a report to the Wiimote
        /// </summary>
        private void WriteReport()
        {
            Console.WriteLine("WriteReport: " + ((OutputReport)mBuff[0]));
            if(mAltWriteMethod)
                HIDImports.HidD_SetOutputReport(this.mHandle.DangerousGetHandle(), mBuff, (uint)mBuff.Length);
            
            else if (mStream != null)
            {
                mStream.Write(mBuff, 0, REPORT_LENGTH);
                mStream.Flush();
            }
            if(mBuff[0] == (byte)OutputReport.WriteMemory)
            {
                Console.WriteLine("Wait");
                if (!mWriteDone.WaitOne(1000, false))
                {
                    Console.WriteLine("Wait failed");
                    //throw new WiimoteException("Error writing data to Wiimote...is it connected?");
                }
            }
        }

        /// <summary>
        /// Read data or register from Wiimote
        /// </summary>
        /// <param name="address">Address to read</param>
        /// <param name="size">Length to read</param>
        /// <returns>Data buffer</returns>
        public byte[] ReadData(int address, short size)
        {
            ClearReport();

            mReadBuff = new byte[size];
            mAddress = address & 0xffff;
            mSize = size;

            mBuff[0] = (byte)OutputReport.ReadMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | GetRumbleBit());
            mBuff[2] = (byte)((address & 0x00ff0000)  >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00)  >>  8);
            mBuff[4] = (byte)(address & 0x000000ff);

            mBuff[5] = (byte)((size & 0xff00) >> 8);
            mBuff[6] = (byte)(size & 0xff);

            WriteReport();

            if(!mReadDone.WaitOne(3000, false))
                throw new WiimoteException("Error reading data from Wiimote...is it connected?");
                //Console.WriteLine("Error reading data from Wiimote...is it connected?");

            return mReadBuff;
        }

        /// <summary>
        /// Write a single byte to the Wiimote
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="data">Byte to write</param>
        public void WriteData(int address, byte data)
        {
            WriteData(address, 1, new byte[] { data });
        }

        /// <summary>
        /// Write a byte array to a specified address
        /// </summary>
        /// <param name="address">Address to write</param>
        /// <param name="size">Length of buffer</param>
        /// <param name="buff">Data buffer</param>
        
        public void WriteData(int address, byte size, byte[] buff)
        {
            ClearReport();

            mBuff[0] = (byte)OutputReport.WriteMemory;
            mBuff[1] = (byte)(((address & 0xff000000) >> 24) | GetRumbleBit());
            mBuff[2] = (byte)((address & 0x00ff0000)  >> 16);
            mBuff[3] = (byte)((address & 0x0000ff00)  >>  8);
            mBuff[4] = (byte)(address & 0x000000ff);
            mBuff[5] = size;
            Array.Copy(buff, 0, mBuff, 6, size);

            WriteReport();
        }

        /// <summary>
        /// Current Wiimote state
        /// </summary>
        public WiimoteState WiimoteState
        {
            get { return mWiimoteState; }
        }

        ///<summary>
        /// Unique identifier for this Wiimote (not persisted across application instances)
        ///</summary>
        public Guid ID
        {
            get { return mID; }
        }

        /// <summary>
        /// HID device path for this Wiimote (valid until Wiimote is disconnected)
        /// </summary>
        public string HIDDevicePath
        {
            get { return mDevicePath; }
        }
        /// <summary>
        /// Do a action when a button is pressed and when it is released.
        /// </summary>
        /// <param name="button">The name of the button that is in the class</param>
        /// <param name="pressedAction">Function to do when button is pressed</param>
        /// <param name="releasedAction">Function to do when button is released</param>
        public void OnPressedReleased(string button, Action pressedAction, Action releasedAction)
        {
            FieldInfo fieldInfo;
            fieldInfo = WiimoteState.Buttons.GetType().GetField(button, BindingFlags.Public | BindingFlags.Instance);
            var oldFieldInfo = WiimoteState.OldButtons.GetType().GetField(button, BindingFlags.Public | BindingFlags.Instance);
            if ((bool) fieldInfo.GetValue(WiimoteState.Buttons))
            {
                if (!(bool) oldFieldInfo.GetValue(WiimoteState.OldButtons)) pressedAction();
            }
            if (!(bool)fieldInfo.GetValue(WiimoteState.Buttons))
            {
                if ((bool)oldFieldInfo.GetValue(WiimoteState.OldButtons)) releasedAction();
            }
            //            if (fieldInfo == null)
            //            {
            //                WiimoteState.Nunchuk.GetType().
            //            }
        }

        #region IDisposable Members

        /// <summary>
        /// Dispose Wiimote
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose wiimote
        /// </summary>
        /// <param name="disposing">Disposing?</param>
        protected virtual void Dispose(bool disposing)
        {
            // close up our handles
            if(disposing)
                Disconnect();
        }
        #endregion

        #region Experimental speaker support(Ported from WiiYourself)

        private bool IsPlayingAudio() {
            return (WiimoteState.Speaker.Frequency != SpeakerFreq.FREQ_NONE && WiimoteState.Speaker.Volume != 0);
        }
        private bool IsPlayingSample() {
            return IsPlayingAudio() && (currentSample != null);
        }

        /// <summary>
        /// This will mute or unmute the speaker
        /// </summary>
        /// <param name="mute">true = mute. false = unmute</param>
        public void MuteSpeaker(bool mute)
        {
            if (mStream == null)
                return;

            if (WiimoteState.Speaker.Muted == mute)
                return;

            Console.WriteLine(mute ? "muting speaker." : "unmuting speaker.");

            ClearReport();
            mBuff[0] = (byte)OutputReport.SpeakerMute;
            mBuff[1] = (byte) ((mute ? 0x04 : 0x00) | GetRumbleBit());
            WriteReport();
            WiimoteState.Speaker.Muted = mute;
        }
        /// <summary>
        /// Turns the speaker on or off
        /// </summary>
        /// <param name="enable">true = on. false = off</param>
        public void EnableSpeaker(bool enable)
        {
            if (mHandle == null)
                return;

            if (WiimoteState.Speaker.Enabled == enable)
                return;

            Console.WriteLine(enable ? "enabling speaker." : "disabling speaker.");

            mBuff[0] = (byte) OutputReport.SpeakerEnable;
            mBuff[1] = (byte) ((enable ? 0x04 : 0x00) | GetRumbleBit());
            WriteReport();

            if (!enable)
            {
                WiimoteState.Speaker.Frequency = SpeakerFreq.FREQ_NONE;
                WiimoteState.Speaker.Volume = 0;
                WiimoteState.CurrentSample = new WiimoteAudioSample();
                MuteSpeaker(true);
                _sampleThreadCancellationTokenSource.Cancel(false);
            }

            WiimoteState.Speaker.Enabled = enable;
        }

        private byte[] Convert16bitMonoSamples(byte[] samples, bool _signed, SpeakerFreq freq)
	    {
        // converts 16bit mono sample data to the native 4bit format used by the Wiimote,
        //  and returns the data in a BYTE array (caller must delete[] when no
        //  longer needed):
	    if(samples == null || samples.Length == 0) return null;
        
        byte[] convertedSamples = new byte[1024];

        // ADPCM code, adapted from
        //  http://www.wiindows.org/index.php/Talk:Wiimote#Input.FOutput_Reports
        int[] index_table = {  -1,  -1,  -1,  -1,   2,   4,   6,   8,
                                        -1,  -1,  -1,  -1,   2,   4,   6,   8 };
        int[] diff_table = {   1,   3,   5,   7,   9,  11,  13,  15,
                                            -1,  -3,  -5,  -7,  -9, -11, -13,  15 };
        int[] step_scale = { 230, 230, 230, 230, 307, 409, 512, 614,
                                            230, 230, 230, 230, 307, 409, 512, 614 };
        // Encode to ADPCM, on initialization set adpcm_prev_value to 0 and adpcm_step
        //  to 127 (these variables must be preserved across reports)
        int adpcm_prev_value = 0;
        int adpcm_step = 127;

	    for(int i =0; i<samples.Length; i++)
		{
		    // convert to 16bit signed
		    int value = samples[i];// (8bit) << 8);// | samples[i]; // dither it?
		    if (!_signed)
		    {
		        value -= 32768;
		    }
		    // encode:
		    int diff = value - adpcm_prev_value;
            int encoded_val = 0;
		    if(diff< 0) {
			    encoded_val |= 8;
			    diff = -diff;
			    }
            diff = (diff << 2) / adpcm_step;
		    if (diff > 7)
		    {
		        diff = 7;
		    }
		    encoded_val |= diff;
		    adpcm_prev_value += ((adpcm_step* diff_table[encoded_val]) / 8);
		    if (adpcm_prev_value > 0x7fff)
		    {
		        adpcm_prev_value = 0x7fff;
		    }
		    if (adpcm_prev_value < -0x8000)
		    {
		        adpcm_prev_value = -0x8000;
		    }
		    adpcm_step = (adpcm_step* step_scale[encoded_val]) >> 8;
		    if (adpcm_step < 127)
		    {
		        adpcm_step = 127;
		    }
		    if (adpcm_step > 24567)
		    {
		        adpcm_step = 24567;
		    }
		    if (i % 1 == 0)
		    {
		        convertedSamples[i >> 1] |= (byte)encoded_val;
		    }
		    else
		    {
		        convertedSamples[i >> 1] |= (byte)(encoded_val << 4);
		    }
		}

	    return convertedSamples;
	    }

        /// <summary>
        /// Loads a mono 16-bit .wav audio file and parses it
        /// </summary>
        /// <param name="filepath">Full path to the file</param>
        /// <param name="frequency"> Frequency override in Hertz(Converters don't really change the headers for frequency)</param>
        /// <returns>Returns the sample that is converted for use with a wii remote</returns>
        public WiimoteAudioSample Load16bitMonoSampleWAV(string filepath, int frequency = 0)
        {
            if (filepath == String.Empty)
            {
                Console.WriteLine("No filepath given.");
                return null;
            }
            // converts unsigned 16bit mono .wav audio data to the 4bit ADPCM variant
            //  used by the Wiimote (at least the closest match so far), and returns
            //  the data in a BYTE array (caller must delete[] it when no longer needed):
            var wiimoteAudioSample = new WiimoteAudioSample();
            Console.WriteLine($"Loading '{filepath}'");
            wiimoteAudioSample.AudioName = Path.GetFileNameWithoutExtension(filepath);
            FileStream fileStream;
            try
            {
                fileStream = File.OpenRead(filepath);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (fileStream == null)
            {
                Console.WriteLine("Couldn't open {0}", filepath);
                return null;
            }
            BinaryReader reader = new BinaryReader(fileStream);
            // parse the .wav file
            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int tempsampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();
            if (channels == 0 && tempsampleRate == 0 && bitDepth == 0)
            {
                fileStream.Dispose();
                reader.Dispose();
                Debug.Assert(false, "Something is wrong with the audio file.");
            }

            if (channels != 1)
            {
                Console.WriteLine($"The file '{Path.GetFileName(filepath)}' is not in mono");
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }
            if (fmtCode != 1)
            {
                Console.WriteLine($"The file '{Path.GetFileName(filepath)}' is not a uncompressed .wav file");
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }

            if (bitDepth != 16)
            {
                Console.WriteLine($"The file '{Path.GetFileName(filepath)}' must be in 16 bit");
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            const int freqOffSet = 100; // for now
            int sampleRate;
            if (frequency == 0)
            {
                sampleRate = tempsampleRate;
            }
            else
            {
                sampleRate = frequency;
            }
            for (int index = 1; index < FreqLookup.Count; index++)
            {
                if ((sampleRate + freqOffSet) >= FreqLookup[index] &&
                   (sampleRate - freqOffSet) <= FreqLookup[index])
                {
                    wiimoteAudioSample.freq = (SpeakerFreq)index;
                    Console.WriteLine(".. using speaker freq {0}", FreqLookup[index]);
                    break;
                }
            }
            if (wiimoteAudioSample.freq == SpeakerFreq.FREQ_NONE)
            {
                Console.WriteLine($"Couldn't (loosely) match .wav samplerate {sampleRate} Hz to speaker");
            }
            var samples = reader.ReadBytes(dataSize);
            wiimoteAudioSample.samples = Convert16bitMonoSamples(samples, false, wiimoteAudioSample.freq);
            wiimoteAudioSample.length = dataSize;
            wiimoteAudioSample.Is8Bit = false;
            reader.Dispose();
            fileStream.Dispose();
            //wiimoteAudioSample.samples = reader.ReadBytes(dataSize);
            return wiimoteAudioSample;
        }


        public WiimoteAudioSample Load8bitMonoSampleWAV(string filepath, int frequency = 0)
        {
            if (filepath == String.Empty)
            {
                Console.WriteLine("No filepath given.");
                return null;
            }
            //  converts unsigned 8bit mono .wav audio data to the 4bit ADPCM variant
            //  used by the Wiimote (at least the closest match so far), and returns
            //  the data in a BYTE array (caller must delete[] it when no longer needed):
            var wiimoteAudioSample = new WiimoteAudioSample();
            Console.WriteLine($"Loading '{filepath}'");
            wiimoteAudioSample.AudioName = Path.GetFileNameWithoutExtension(filepath);
            FileStream fileStream;
            try
            {
                fileStream = File.OpenRead(filepath);
            }
            catch (FileNotFoundException ex)
            {
                throw new FileNotFoundException($"Can't find {ex.FileName} are you sure its added to the resources and not deleted.");
            }
            catch (Exception ex)
            {
                throw ex;
            }
            if (fileStream == null)
            {
                Console.WriteLine("Couldn't open {0}", filepath);
                return null;
            }
            BinaryReader reader = new BinaryReader(fileStream);
            // parse the .wav file
            int chunkID = reader.ReadInt32();
            int fileSize = reader.ReadInt32();
            int riffType = reader.ReadInt32();
            int fmtID = reader.ReadInt32();
            int fmtSize = reader.ReadInt32();
            int fmtCode = reader.ReadInt16();
            int channels = reader.ReadInt16();
            int tempsampleRate = reader.ReadInt32();
            int fmtAvgBPS = reader.ReadInt32();
            int fmtBlockAlign = reader.ReadInt16();
            int bitDepth = reader.ReadInt16();
            if (channels == 0 && tempsampleRate == 0 && bitDepth == 0)
            {
                fileStream.Dispose();
                reader.Dispose();
                Debug.Assert(false, "Something is wrong with the audio file.");
            }

            if (channels != 1)
            {
                Console.WriteLine("The file '{0}' is not in mono", filepath);
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }
            if (fmtCode != 1)
            {
                Console.WriteLine("The file '{0}' is not a uncompressed .wav file", filepath);
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }

            if (bitDepth != 8)
            {
                Console.WriteLine("The file '{0}' must be in 8 bit", filepath);
                fileStream.Dispose();
                reader.Dispose();
                return null;
            }

            if (fmtSize == 18)
            {
                // Read any extra values
                int fmtExtraSize = reader.ReadInt16();
                reader.ReadBytes(fmtExtraSize);
            }

            int dataID = reader.ReadInt32();
            int dataSize = reader.ReadInt32();
            const int epsilon = 100; // for now
            int sampleRate;
            if (frequency == 0)
            {
                sampleRate = tempsampleRate;
            }
            else
            {
                sampleRate = frequency;
            }
            for (int index = 1; index < FreqLookup.Count; index++)
            {
                if ((sampleRate + epsilon) >= FreqLookup[index] &&
                   (sampleRate - epsilon) <= FreqLookup[index])
                {
                    wiimoteAudioSample.freq = (SpeakerFreq)index;
                    Console.WriteLine(".. using speaker freq {0}", FreqLookup[index]);
                    break;
                }
            }
            if (wiimoteAudioSample.freq == SpeakerFreq.FREQ_NONE)
            {
                Console.WriteLine("Couldn't (loosely) match .wav samplerate {0} Hz to speaker", sampleRate);
            }
            var samples = reader.ReadBytes(dataSize);
            wiimoteAudioSample.samples = samples;
            wiimoteAudioSample.length = dataSize;
            wiimoteAudioSample.Is8Bit = true;
            reader.Dispose();
            fileStream.Dispose();
            //wiimoteAudioSample.samples = reader.ReadBytes(dataSize);
            return wiimoteAudioSample;
        }


        private async void SampleStreamThread()
        {
            Console.WriteLine("(starting sample thread)");
            // sends a simple square wave sample stream

            byte[] squarewave_report =
                { (byte)OutputReport.SpeakerData, 20<<3, 0xC1,0xC1,0xC1,0xC1,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,
                                   0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3, };
//            byte[] squarewave_report =
//                { (byte)OutputReport.SpeakerData, 20<<3, 0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,
//                                   0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3,0xC3, };
            byte[] sample_report = new byte[REPORT_LENGTH]
                { (byte)OutputReport.SpeakerData, 0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0 };

            bool last_playing = false;
            int frame = 0;
            long frame_start = 0;
            int total_samples = 0;
            int sample_index = 0;
            WiimoteAudioSample current_sample = null;

            // TODO: duration!!
            while (!_stopSampleThread)
            {
                bool playing = IsPlayingAudio();

                if (!playing)
                    await Task.Delay(TimeSpan.FromMilliseconds(0.10));
                else
                {
                    int freq_hz;
                    if (FreqOverride == 0)
                    {
                        freq_hz = FreqLookup[(int) WiimoteState.Speaker.Frequency];
                    }
                    else
                    {
                        freq_hz = FreqOverride;
                    }
                    float frame_ms = 1000 / (freq_hz / 40); // 20bytes = 40 samples per write
                    // has the sample just changed?
                    bool sample_changed = (current_sample != WiimoteState.CurrentSample);
                    current_sample = WiimoteState.CurrentSample;
                    
                    if (!last_playing || sample_changed)
                    {
                        frame = 0;
                        frame_start = DateTime.Now.Ticks;
                        total_samples = current_sample != null ? current_sample.length : 0;
                        sample_index = 0;
                    }

                    // are we streaming a sample?
                    if (current_sample != null)
                    {
                        if (sample_index < current_sample.length)
                        {
                            // (remember that samples are 4bit, ie. 2 per byte)
                            int samples_left = (current_sample.length - sample_index);
                            int report_samples = Math.Min(samples_left, 40);
                            // round the entries up to the nearest multiple of 2
                            int report_entries = (report_samples + 1) >> 1;

                            sample_report[1] = (byte)((report_entries << 3) |
                                                      GetRumbleBit());
                            {
                                for (int index = 0; index < report_entries; index++)
                                    sample_report[2 + index] =
                                            current_sample.samples[(sample_index >> 1) + index];

                                ClearReport();
                                Array.Copy(sample_report, mBuff, REPORT_LENGTH);
                                WriteReport();
                                sample_index += report_samples;
                            }
                        }
                        else
                        {
                            // we reached the sample end
                            WiimoteState.CurrentSample = null;
                            current_sample = null;
                            WiimoteState.Speaker.Frequency = SpeakerFreq.FREQ_NONE;
                            WiimoteState.Speaker.Volume = 0;
                        }
                    }
                    // no, a squarewave
                    else
                    {
                        squarewave_report[1] = (byte)((20 << 3) | GetRumbleBit());
                        ClearReport();
                        Array.Copy(squarewave_report, mBuff, REPORT_LENGTH);
                        WriteReport();
                    }

                    frame++;

                    // send the first two buffers immediately? (attempts to lessen startup
                    //  startup glitches by assuming we're filling a small sample
                    //  (or general input) buffer on the wiimote) - doesn't seem to help
                    //			if(frame > 2) {
                    while ((DateTime.Now.Ticks - frame_start) < (int)(frame * frame_ms))
                        await Task.Delay(TimeSpan.FromMilliseconds(0.10));
                    //				}
                }

                last_playing = playing;
            }

            Console.WriteLine("(ending sample thread)");
            MuteSpeaker(true);
            EnableSpeaker(false);
            SampleThread.Interrupt();
            if (!SampleThread.Join(2000))
            { // or an agreed resonable time
                SampleThread.Abort();
            }
            SampleThread = null;

        }
        
        private bool StartSampleThread()
        {
            if (SampleThread != null)
                return true;

            SampleThread = new Thread(SampleStreamThread);
            SampleThread.Start();
            if (SampleThread.ThreadState != ThreadState.Running)
            {
                Console.WriteLine("couldn't create sample thread!");
                MuteSpeaker(true);
                EnableSpeaker(false);
                return false;
            }
            return true;
        }

        /// <summary>
        /// This will play a squarewave at the frequency that is defined
        /// </summary>
        /// <param name="freq">Frequency Enum value</param>
        /// <param name="vol">Volume 1 to 100%</param>
        /// <returns></returns>
        public bool PlaySquareWave(SpeakerFreq freq, byte vol)
        {
            if (vol > 100)
            {
                vol = 100;
            }
            if (mHandle == null)
                return false;

            byte volume = vol;
            //byte volume = vol.Map(0, 100, 0, 64);
            // if we're already playing a sample, stop it first
            if (IsPlayingSample())
                currentSample = null;
            // if we're already playing a square wave at this freq and volume, return
            else if (IsPlayingAudio() && (WiimoteState.Speaker.Frequency == freq) &&
                                        (WiimoteState.Speaker.Volume == vol))
                return true;

            
            Console.WriteLine("playing square wave.");
            // stop playing samples
            currentSample = null;

            EnableSpeaker(true);
            MuteSpeaker(true);

            // write 0x01 to register 0xa20009 
            WriteData(0xa20009, 0x01);
            // write 0x08 to register 0xa20001 
            WriteData(0xa20001, 0x08);
            // write default sound mode (4bit ADPCM, we assume) 7-byte configuration
            //  to registers 0xa20001-0xa20008 
            byte[] frequency;
            frequency = ConvertHelpers.INT2LE(FreqOverride == 0 ? ConvertHelpers.AdpcmToWiimoteRate(FreqLookup[(int) freq], false) : ConvertHelpers.AdpcmToWiimoteRate(FreqOverride, false));
            byte[] bytes = new byte[7]{ 0x00, 0x00, frequency[0], frequency[1], volume, 0x00, 0x00 };
            WriteData(0x04a20001, (byte)bytes.Length, bytes);
            // write 0x01 to register 0xa20008 
            WriteData(0xa20008, 0x01);

            WiimoteState.Speaker.Frequency = freq;
            WiimoteState.Speaker.Volume = volume;

            MuteSpeaker(false);
            return StartSampleThread();
        }

        public void PlaySample(WiimoteAudioSample sample, int vol, SpeakerFreq freq_override = SpeakerFreq.FREQ_NONE)
	    {
            
	        if(mHandle == null || sample == null)
		        return;

	        SpeakerFreq freq;
	        if (freq_override != SpeakerFreq.FREQ_NONE)
	        {
	            freq = freq_override;
	        }
	        else
	        {
	            freq = sample.freq;
	        }

            Console.WriteLine("playing sample.");

            EnableSpeaker(true);

            MuteSpeaker(true);
	        var volume = (byte) vol.Map(0, 100, 0x00, sample.Is8Bit ? 0xff : 0x40);
            // Get the frequency in Little Endian bytes
            byte[] frequency = ConvertHelpers.INT2LE(ConvertHelpers.AdpcmToWiimoteRate(FreqLookup[(int)freq], sample.Is8Bit));

            // Write 0x01 to register 0x04a20009 
            WriteData(0x04a20009, 0x01);
            // Write 0x08 to register 0x04a20001 
            WriteData(0x04a20001, 0x08);
            // Write 7-byte configuration to registers 0x04a20001-0x04a20008 
            byte[] bytes = { 0x00, 0x00, frequency[0], frequency[1], volume, 0x00, 0x00 };

            WriteData(0x04a20001, (byte)bytes.Length, bytes);
            // + Write 0x01 to register 0x04a20008 
            WriteData(0x04a20008, 0x01);

            WiimoteState.Speaker.Frequency = freq;
            WiimoteState.Speaker.Volume = volume;
            WiimoteState.CurrentSample = sample;
            
            MuteSpeaker(false);

	        StartSampleThread();
        }

    #endregion

}
}