using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Native;

namespace WiiInputMapper.Template
{
	public class Keyboard
	{
		public enum Key
		{
			N0 = VirtualKeyCode.VK_0,
			N1 = VirtualKeyCode.VK_1,
			N2 = VirtualKeyCode.VK_2,
			N3 = VirtualKeyCode.VK_3,
			N4 = VirtualKeyCode.VK_4,
			N5 = VirtualKeyCode.VK_5,
			N6 = VirtualKeyCode.VK_6,
			N7 = VirtualKeyCode.VK_7,
			N8 = VirtualKeyCode.VK_8,
			N9 = VirtualKeyCode.VK_9,
			A = VirtualKeyCode.VK_A,
			B = VirtualKeyCode.VK_B,
			C = VirtualKeyCode.VK_C,
			D = VirtualKeyCode.VK_D,
			E = VirtualKeyCode.VK_E,
			F = VirtualKeyCode.VK_F,
			G = VirtualKeyCode.VK_G,
			H = VirtualKeyCode.VK_H,
			I = VirtualKeyCode.VK_I,
			J = VirtualKeyCode.VK_J,
			K = VirtualKeyCode.VK_K,
			L = VirtualKeyCode.VK_L,
			M = VirtualKeyCode.VK_M,
			N = VirtualKeyCode.VK_N,
			O = VirtualKeyCode.VK_O,
			P = VirtualKeyCode.VK_P,
			Q = VirtualKeyCode.VK_Q,
			R = VirtualKeyCode.VK_R,
			S = VirtualKeyCode.VK_S,
			T = VirtualKeyCode.VK_T,
			U = VirtualKeyCode.VK_U,
			V = VirtualKeyCode.VK_V,
			W = VirtualKeyCode.VK_W,
			X = VirtualKeyCode.VK_X,
			Y = VirtualKeyCode.VK_Y,
			Z = VirtualKeyCode.VK_Z,
			//AbntC1 = 36,
			//AbntC2 = 37,
			//Apostrophe = 38,
			//Applications = 39,
			//AT = 40,
			//AX = 41,
			Backspace = VirtualKeyCode.BACK,
			//Backslash = VirtualKeyCode.SLASH,
			//Calculator = 44,
			CapsLock = VirtualKeyCode.CAPITAL,
			//Colon = 46,
			//Comma = 47,
			//Convert = 48,
			Delete = VirtualKeyCode.DELETE,
			Down = VirtualKeyCode.DOWN,
			End = VirtualKeyCode.END,
			//Equals = VirtualKeyCode.,
			Escape = VirtualKeyCode.ESCAPE,
			F1 = VirtualKeyCode.F1,
			F2 = VirtualKeyCode.F2,
			F3 = VirtualKeyCode.F3,
			F4 = VirtualKeyCode.F4,
			F5 = VirtualKeyCode.F5,
			F6 = VirtualKeyCode.F6,
			F7 = VirtualKeyCode.F7,
			F8 = VirtualKeyCode.F8,
			F9 = VirtualKeyCode.F9,
			F10 = VirtualKeyCode.F10,
			F11 = VirtualKeyCode.F11,
			F12 = VirtualKeyCode.F12,
			//Grave = VirtualKeyCode.,
			Home = VirtualKeyCode.HOME,
			Insert = VirtualKeyCode.INSERT,
			//Kana = 72,
			//Kanji = 73,
			//LeftBracket = VirtualKeyCode,
			Ctrl = VirtualKeyCode.LCONTROL,
			Left = VirtualKeyCode.LEFT,
			//Alt = VirtualKeyCode.,
			Shift = VirtualKeyCode.LSHIFT,
			Windows = VirtualKeyCode.LWIN,
			//Mail = 80,
			//MediaSelect = 81,
			//MediaStop = 82,
			//Minus = 83,
			//Mute = 84,
			//MyComputer = 85,
			//NextTrack = 86,
			//NoConvert = 87,
			//NumberLock = 88,
			//NumberPad0 = 89,
			//NumberPad1 = 90,
			//NumberPad2 = 91,
			//NumberPad3 = 92,
			//NumberPad4 = 93,
			//NumberPad5 = 94,
			//NumberPad6 = 95,
			//NumberPad7 = 96,
			//NumberPad8 = 97,
			//NumberPad9 = 98,
			//NumberPadComma = 99,
			//NumberPadEnter = 100,
			//NumberPadEquals = 101,
			//NumberPadMinus = 102,
			//NumberPadPeriod = 103,
			//NumberPadPlus = 104,
			//NumberPadSlash = 105,
			//NumberPadStar = 106,
			//Oem102 = 107,
			//PageDown = VirtualKeyCode.,
			//PageUp = 109,
			//Pause = 110,
			//Period = 111,
			//PlayPause = 112,
			//Power = 113,
			//PreviousTrack = 114,
			//RightBracket = 115,
			//RightControl = 116,
			Return = VirtualKeyCode.RETURN,
			Right = VirtualKeyCode.RIGHT,
			//RightWindowsKey = 121,
			//ScrollLock = 122,
			//Semicolon = 123,
			//Slash = 124,
			//Sleep = 125,
			Space = VirtualKeyCode.SPACE,
			//Stop = 127,
			//PrintScreen = 128,
			Tab = VirtualKeyCode.TAB,
			//Underline = 130,
			//Unlabeled = 131,
			Up = VirtualKeyCode.UP,
			//VolumeDown = 133,
			//VolumeUp = 134,
			//Wake = 135,
			//WebBack = 136,
			//WebFavorites = 137,
			//WebForward = 138,
			//WebHome = 139,
			//WebRefresh = 140,
			//WebSearch = 141,
			//WebStop = 142,
			//Yen = 143,
			//Unknown = 144,
		}

        internal void Stop()
        {
			foreach(var keyStateKV in keyStateMap)
			{
				Down(keyStateKV.Key, true); // Make sure all the keys are not pressed anymore
			}
        }

        private Dictionary<Key, bool> keyStateMap = new Dictionary<Key, bool>();

		InputSimulator inputSimulator;
		public Keyboard(InputSimulator _inputSimulator)
		{
			inputSimulator = _inputSimulator;
			foreach (Key key in Enum.GetValues(typeof(Key)))
			{
				keyStateMap.Add(key, false);
			}
		}

		public void Press(Key key, bool active)
		{
			bool keyState;
			if (keyStateMap.TryGetValue(key, out keyState))
			{
				if (active && !keyState) Down(key, true);
				else if (!active && keyState) Up(key, true);
			}
		}

		public void Up(Key key, bool shouldDo)
		{
			if (!shouldDo || !keyStateMap[key]) return; // If we do not have to do it or the key is already up then we can return
			keyStateMap[key] = false;
			inputSimulator.Keyboard.KeyUp((VirtualKeyCode)(key));
		}

		public void Down(Key key, bool shouldDo)
		{
			if (!shouldDo || keyStateMap[key]) return; // If we do not have to do it or the key is already down then we can return
			keyStateMap[key] = true;
			inputSimulator.Keyboard.KeyDown((VirtualKeyCode)(key));
		}
	}
}
