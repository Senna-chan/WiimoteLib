using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;
using WindowsInput.Events;
using WindowsInput.Native;

namespace WiiInputMapper.Template
{
	public class Keyboard
	{

        private Dictionary<KeyCode, bool> keyStateMap = new Dictionary<KeyCode, bool>();
		public EventBuilder keyboard = WindowsInput.Simulate.Events();
		public Keyboard()
		{
			foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
			{
				try
				{
					keyStateMap.Add(key, false);
				} catch(Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
		}
		public void Stop()
		{
			foreach (var keyStateKV in keyStateMap)
			{
				keyboard.Release(keyStateKV.Key); // Make sure all the keys are not pressed anymore
			}
			keyboard.Invoke();
		}

		public void Press(KeyCode key, bool active)
		{
			bool keyState;
			if (keyStateMap.TryGetValue(key, out keyState))
			{
				if (active && !keyState) Down(key, true);
				else if (!active && keyState) Up(key, true);
			}
		}

		public void Up(KeyCode key, bool shouldDo)
		{
			if (!shouldDo || !keyStateMap[key]) return; // If we do not have to do it or the key is already up then we can return
			keyStateMap[key] = false;
			keyboard.Release(key).Invoke();
		}

		public void Down(KeyCode key, bool shouldDo)
		{
			if (!shouldDo || keyStateMap[key]) return; // If we do not have to do it or the key is already down then we can return
			keyStateMap[key] = true;
			keyboard.Hold(key).Invoke();
		}
	}
}
