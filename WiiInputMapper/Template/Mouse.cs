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
    }
}
