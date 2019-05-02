using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WTXModbus
{
    /// <summary>
    /// Event argument for the ip adress and timer interval. Used by the event '
    /// </summary>
    public class SettingsEventArgs : EventArgs
    {
        private string _ipAdress;
        private int _timer;

        public SettingsEventArgs(string ipAdress, int timer)
        {
            _ipAdress = ipAdress;
            _timer = timer;
        }

        public string IPAdress
        {
            get
            {
                return _ipAdress;
            }
        }
        public int TimerInterval
        {
            get
            {
                return _timer;
            }
        }

    }
}
