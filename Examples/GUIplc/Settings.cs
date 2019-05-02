/* @@@@ HOTTINGER BALDWIN MESSTECHNIK - DARMSTADT @@@@@
 * 
 * TCP/MODBUS Interface for WTX120_Modbus | 01/2018
 * 
 * Author : Felix Huettl 
 * 
 *  */

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WTXModbus;

namespace GUIplc
{
    // This class implements a windows form to change the specific values of the connection, like
    // IP Adress, number of inputs read out in the register and the sending interval, which
    // is the interval of the timer. 

    partial class SettingsForm : Form
    {
        public EventHandler<SettingsEventArgs> ValuesChanged;

        private string _ipAddressBefore;
        private string _ipAddress;

        private int _sendingInterval;

        private GUIplcForm _guiInfo;

        // Constructor of class 'SettingForm': 
        public SettingsForm(string ipAddressParam, int sendingIntervalParam)
        {
            InitializeComponent();
           
            this._ipAddressBefore = ipAddressParam;    // IP_address_before is used to change the IP adress. 
            this._ipAddress = ipAddressParam;
            this._sendingInterval = sendingIntervalParam;

            textBox1.Text = this._ipAddress;
            textBox2.Text = this._sendingInterval.ToString();
            
            label2.Text = "IP address";
            label3.Text = "Read timer interval";
        }
        // This method sets and actualize the attributes of the connection
        // (IP adress, sending/timer interval, number of inputs), if they have changed. 
        private void button2_Click(object sender, EventArgs e)
        {
            this._ipAddress = textBox1.Text;

            this._sendingInterval = Convert.ToInt32(textBox2.Text);

            ValuesChanged.Invoke(this, new SettingsEventArgs(this._ipAddress, this._sendingInterval));

            //Store IPAddress in Settings .settings
            WTXModbus.Properties.Settings.Default.IPAddress = this._ipAddress;
            WTXModbus.Properties.Settings.Default.Save();

            this.Close();
        }
        public string GetIpAddress
        {
            get
            {
                return this._ipAddress;
            }
        }
        public int GetSendingInterval
        {
            get
            {
                return this._sendingInterval;
            }
        }
        private void Settings_Form_Load(object sender, EventArgs e)
        {

        }
    }
}
