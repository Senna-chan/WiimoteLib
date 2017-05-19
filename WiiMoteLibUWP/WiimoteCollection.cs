using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WiiMoteLibUWP
{
    public class WiimoteCollection : Collection<Wiimote>
    {
        /// <summary>
        /// Finds all Wiimotes connected to the system and adds them to the collection
        /// </summary>
        public void FindAllWiimotes()
        {
            Wiimote.FindWiimote(WiimoteFound);
        }

        private bool WiimoteFound(string devicePath)
        {
            this.Add(new Wiimote(devicePath));
            return true;
        }
    }
}
