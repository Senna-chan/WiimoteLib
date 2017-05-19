using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace WiiPairUWP
{
    class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;


        private string _consoleLog = String.Empty;

        public string ConsoleLog
        {
            get { return _consoleLog; }
            set
            {
                if (_consoleLog != value)
                {
                    _consoleLog = value;
                    OnPropertyChanged("ConsoleLog");
                }
            }
        }

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
