using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WiimoteLib;

namespace WiiInputMapper.Template
{
    public class WiimoteFunctions
    {
        Wiimote wiimote;
        private bool rumbling = false;
        private bool lastIRState = false;
        public WiimoteFunctions(Wiimote _wiimote)
        {
            wiimote = _wiimote;
        }

        public void Rumble(bool rumble, int forTime = 100)
        {
            if (rumble == rumbling) return;
            rumbling = rumble;
            if (wiimote.WiimoteState.Rumble && !rumble)
            {
                wiimote.SetRumble(false);
            }
            else if(!wiimote.WiimoteState.Rumble && rumble)
            {
                wiimote.SetRumble(true);
                if (forTime != 0)
                {
                    new Thread(new ThreadStart(() =>
                    {
                        Thread.Sleep(forTime);
                        wiimote.SetRumble(false);
                        rumbling = false;
                    })).Start();
                }
            }
        }

        public void IRRumble(bool irState)
        {
            if(!irState && lastIRState)
            {
                Rumble(true);
            }
            lastIRState = irState;
        }
    }
}
