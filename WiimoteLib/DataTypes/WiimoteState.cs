using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiimoteLib.DataTypes
{
    /// <summary>
	/// Current overall state of the Wiimote and all attachments
	/// </summary>
	public class WiimoteState
    {
        /// <summary>
        /// Current calibration information
        /// </summary>
        public AccelCalibrationInfo AccelCalibrationInfo;
        /// <summary>
        /// Current state of accelerometers
        /// </summary>
        public AccelState AccelState;
        /// <summary>
        /// Current state of buttons
        /// </summary>
        public ButtonState ButtonState;
        /// <summary>
        /// Current state of IR sensors
        /// </summary>
        public IRState IRState;
        /// <summary>
        /// Raw byte value of current battery level
        /// </summary>
        public byte BatteryRaw;
        /// <summary>
        /// Calculated current battery level
        /// </summary>
        public float Battery;
        /// <summary>
        /// Current state of rumble
        /// </summary>
        public bool Rumble;
        /// <summary>
        /// Is an extension controller inserted?
        /// </summary>
        public bool Extension;
        /// <summary>
        /// Extension controller currently inserted, if any
        /// </summary>
        public ExtensionType ExtensionType;
        /// <summary>
        /// Current state of Nunchuk extension
        /// </summary>
        public NunchukState NunchukState;
        /// <summary>
        /// Current state of Classic Controller extension
        /// </summary>
        public ClassicControllerState ClassicControllerState;
        /// <summary>
        /// Current state of Guitar extension
        /// </summary>
        public GuitarState GuitarState;
        /// <summary>
        /// Current state of Drums extension
        /// </summary>
        public DrumsState DrumsState;
        /// <summary>
        /// Current state of the Wii Fit Balance Board
        /// </summary>
        public BalanceBoardState BalanceBoardState;
        /// <summary>
        /// Current state of the MotionPlus
        /// </summary>
        public MotionPlusState MotionPlusState;
        /// <summary>
        /// Current state of LEDs
        /// </summary>
        public LEDState LEDState;

        /// <summary>
        /// Constructor for WiimoteState class
        /// </summary>
        public WiimoteState()
        {
            IRState.IRSensors = new IRSensor[4];
        }
    }
}
