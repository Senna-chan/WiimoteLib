using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WindowsInput;

namespace WiiInputMapper.Template
{
	public class Mouse
	{
		InputSimulator inputSimulator;
		public int X = 0;
		public int Y = 0;
		public Mouse(InputSimulator _inputSimulator)
		{
			inputSimulator = _inputSimulator;
		}

        internal void Stop()
        {
        }

        internal void MoveRel(double x, double y)
        {
			if (x > -5 && x < 5) x = 0; // Deadzone
			if (y > -5 && y < 5) y = 0; // Deadzone
			int mX = (int)(x);
			int mY = (int)(y);
			inputSimulator.Mouse.MoveMouseBy(mX, mY);
        }
    }
}
