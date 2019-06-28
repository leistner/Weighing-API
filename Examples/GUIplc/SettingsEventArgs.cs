using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIplc
{
    /// <summary>
    /// Event argument for the ip adress and timer interval.
    /// </summary>
    public class SettingsEventArgs : EventArgs
    {

        #region ==================== constants & fields ====================

        private string _ipAdress;
        private int _timer;

        #endregion

        #region =============== constructors & destructors =================

        /// <summary>
        /// Constructor of SettingsEvent. Sets the IP address and timer interval
        /// </summary>
        /// <param name="ipAdress"></param>
        /// <param name="timer"></param>
        public SettingsEventArgs(string ipAddress, int timer)
        {
            _ipAdress = ipAddress;
            _timer = timer;
        }

        #endregion

        #region ======================== properties ========================

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

        #endregion

    }
}
