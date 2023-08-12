using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsInput;
using WindowsInput.Events;
using WindowsInput.Events.Sources;

namespace WiiInputMapper.Template
{
	public class Mouse
	{
		public EventBuilder mouse = WindowsInput.Simulate.Events();
		public int X = 0;
		public int Y = 0;
		public int currentLocatedScreen = 0;
		public int desktopWidth = 0;
		public int desktopHeight = 0;
		public List<Rectangle> screenSizes = new List<Rectangle>();

		private bool leftActive = false;
		private bool rightActive = false;
		private bool middleActive = false;

		public Mouse()
		{
			Screen.AllScreens.ToList().ForEach(s => {
				screenSizes.Add(s.Bounds);
				desktopWidth += s.Bounds.Width;
				if (s.Bounds.Height > desktopHeight)
				{
					desktopHeight = s.Bounds.Height;
				}
			});
		}

        public void Stop()
        {
        }

        public void MoveRel(double x, double y)
        {
			if (x > -5 && x < 5) x = 0; // Deadzone
			if (y > -5 && y < 5) y = 0; // Deadzone
			int mX = (int)(x);
			int mY = (int)(y);
			mouse.MoveBy(mX, mY);
			mouse.Invoke();
        }

		public void MoveAbs(double x, double y)
		{
			int mX = (int)(x);
			int mY = (int)(y);
			Console.WriteLine(123);
		}
		public void Scroll(double x, double y)
		{
			int mX = (int)(x);
			int mY = (int)(y);
			mouse.Scroll(ButtonCode.VScroll, mX);
			mouse.Scroll(ButtonCode.HScroll, mY);
			mouse.Invoke();
		}

		public void Left(bool state)
		{
			if (state && !leftActive) mouse.Hold(ButtonCode.Left);
			if (!state && leftActive) mouse.Release(ButtonCode.Left);
			mouse.Invoke();
			leftActive = state;
		}

		public void Right(bool state)
		{
			if (state && !rightActive) mouse.Hold(ButtonCode.Right);
			if (!state && rightActive) mouse.Release(ButtonCode.Right);
			mouse.Invoke();
			rightActive = state;
		}

		public void Middle(bool state)
		{
			if (state && !middleActive) mouse.Hold(ButtonCode.Middle);
			if (!state && middleActive) mouse.Release(ButtonCode.Middle);
			mouse.Invoke();
			leftActive = state;
		}
	}
}
