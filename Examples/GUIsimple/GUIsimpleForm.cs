﻿// <copyright file="GUIsimple.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// WTXGUIsimple, a demo application for HBM Weighing-API  
//
// The MIT License (MIT)
//
// Copyright (C) Hottinger Baldwin Messtechnik GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

using HBM.Weighing.API;
using HBM.Weighing.API.WTX;
using HBM.Weighing.API.WTX.Jet;
using HBM.Weighing.API.WTX.Modbus;

using System;
using System.Threading;
using System.Windows.Forms;
using WTXModbus;
/*
* This application example enables communication and a connection to the WTX120 device via 
* Modbus and Jetbus.
*/
namespace GUIsimple
{
    public partial class GUIsimpleForm : Form
    {
        #region Locales

        private static BaseWTDevice _wtxDevice;

        private AdjustmentCalculator _adjustmentCalculator;
        private AdjustmentWeigher _adjustmentWeigher;
        private FunctionIO _functionIOForm;

        private const string DEFAULT_IP_ADDRESS = "192.168.100.88";

        private const string MESSAGE_CONNECTION_FAILED = "Connection failed!";
        private const string MESSAGE_CONNECTING = "Connecting...";

        private const int WAIT_DISCONNECT = 2000;

        private string _ipAddress = DEFAULT_IP_ADDRESS;

        private int _timerInterval = 200;       
        #endregion

        
        #region Constructor
        public GUIsimpleForm(string[] args)
        {
            InitializeComponent();

            txtInfo.Text = "To Connect to the WTX device, please enter an IP address, select 'Jet' or 'Modbus/TCP' and press 'connect'.";

            EvaluateCommandLine(args);      

            txtIPAddress.Text = _ipAddress;

            picNE107.Image = Properties.Resources.NE107_DiagnosisPassive;
        }
        #endregion

        
        #region Command line
        private void EvaluateCommandLine(string[] args)
        {
            if (args.Length > 0)
            {
                if (args[0].ToLower() == "modbus")
                {              
                    rbtConnectionModbus.Checked = true;
                }
                if (args[0].ToLower() == "jet")
                {
                    rbtConnectionJet.Checked = true;
                }
            }

            if (args.Length > 1)
                _ipAddress = args[1];

            if (args.Length > 2)
                this._timerInterval = Convert.ToInt32(args[2]);
        }
        #endregion


        #region Connection
        // This method connects to the given IP address
        private void InitializeConnection()
        {
            txtInfo.Text = "Connecting...";
            this._ipAddress = txtIPAddress.Text;

            if (this.rbtConnectionModbus.Checked)    // If 'Modbus/Tcp' is selected: 
            {
                // Creating objects of ModbusTcpConnection and WTXModbus: 
                ModbusTcpConnection _modbusConnection = new ModbusTcpConnection(this._ipAddress);

                _wtxDevice = new HBM.Weighing.API.WTX.WTXModbus(_modbusConnection, this._timerInterval, this.update);

            }
            else
            {
                if (this.rbtConnectionJet.Checked)  // If 'JetBus' is selected: 
                {
                    // Creating objects of JetBusConnection and WTXJet: 
                    JetBusConnection _jetConnection = new JetBusConnection(_ipAddress, "Administrator", "wtx");

                    _wtxDevice = new WTXJet(_jetConnection,update);
                }
            }

            // Connection establishment via Modbus or Jetbus :            
            try
            {
                _wtxDevice.Connect(5000);
            }
            catch (Exception)
            {
                txtInfo.Text = MESSAGE_CONNECTION_FAILED;
            }

            if (_wtxDevice.IsConnected == true)
            {
                picNE107.Image = Properties.Resources.NE107_DiagnosisActive;
            }
            else
            {
                picNE107.Image = Properties.Resources.NE107_DiagnosisPassive;
                txtInfo.Text = MESSAGE_CONNECTION_FAILED;
            }

        }

        //Callback for automatically receiving event based data from the device
        private void update(object sender, ProcessDataReceivedEventArgs e)
        {           
            txtInfo.BeginInvoke(new Action(() =>
            {
                int netValue   = e.ProcessData.NetValue;
                int grossValue = e.ProcessData.GrossValue;
                int decimals   = e.ProcessData.Decimals;

                int taraValue = netValue - grossValue;

                txtInfo.Text = "Net:" + _wtxDevice.CurrentWeight + _wtxDevice.Unit + Environment.NewLine
                + "Gross:" + _wtxDevice.ProcessData.NetValue + _wtxDevice.Unit + Environment.NewLine
                + "Tara:" + _wtxDevice.ProcessData.TareValue + _wtxDevice.Unit;
                txtInfo.TextAlign = HorizontalAlignment.Right;

                if (e.ProcessData.Underload == true)
                {
                    txtInfo.Text = "Underload : Lower than minimum" + Environment.NewLine;
                    txtInfo.TextAlign = HorizontalAlignment.Right;
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;

                }
                else if (e.ProcessData.Overload == true)
                {
                    txtInfo.Text = "Overload : Higher than maximum capacity" + Environment.NewLine;
                    txtInfo.TextAlign = HorizontalAlignment.Right;
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;

                }
                else if (e.ProcessData.HigherSafeLoadLimit == true)
                {
                    txtInfo.Text = "Higher than safe load limit" + Environment.NewLine;
                    txtInfo.TextAlign = HorizontalAlignment.Right;
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;
                }
                else
                    picNE107.Image = Properties.Resources.NE107_DiagnosisActive;

            }));
        }
        
        #endregion


        #region Button & tool strips clicks
        //Connect device
        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (_wtxDevice != null)    // Necessary to check if the object of BaseWtDevice have been created and a connection exists. 
                if (this.rbtConnectionJet.Checked == true && _wtxDevice.ConnectionType == "Jetbus" && txtIPAddress.Text==_wtxDevice.Connection.IpAddress)
                    return;
                else
                if (this.rbtConnectionModbus.Checked == true && _wtxDevice.ConnectionType == "Modbus" && txtIPAddress.Text == _wtxDevice.Connection.IpAddress)
                    return;
                else
                {
                    _wtxDevice.Connection.Disconnect();
                    _wtxDevice = null;                    

                    Thread.Sleep(WAIT_DISCONNECT);     // Wait for 2 seconds till the disconnection request is finished. 

                    txtInfo.Text = "Connecting..." + Environment.NewLine;
                }

            /* Establish a connection in Method InitializeConnection() */
            this.InitializeConnection();          
        }

        // button click event for switching to gross or net value. 
        private void cmdGrossNet_Click(object sender, EventArgs e)
        {
                _wtxDevice.SetGross();
        }

        // button click event for zeroing
        private void cmdZero_Click(object sender, EventArgs e)
        {
                _wtxDevice.Zero();
        }

        // button click event for taring 
        private void cmdTare_Click(object sender, EventArgs e)
        {
            _wtxDevice.Tare();
        }

        //Method for calculate adjustment with dead load and span: 
        private void calibrationWithWeightToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _adjustmentCalculator = new AdjustmentCalculator(_wtxDevice);            
            DialogResult res = _adjustmentCalculator.ShowDialog();
        }


        //Method for adjustment with weight: 
        private void calibrationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            _adjustmentWeigher = new AdjustmentWeigher(_wtxDevice);
            DialogResult res = _adjustmentWeigher.ShowDialog();
        }
        // Toolstrip Click Event for Digital IO : Input & Output
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            _wtxDevice.Stop();

            if (_wtxDevice.Connection.ConnType == ConnectionType.Modbus)
            {
                _wtxDevice.Disconnect();

                JetBusConnection _connection = new JetBusConnection(_ipAddress);
                _wtxDevice = new WTXJet(_connection, update);

                _wtxDevice.Connect(5000);

                _functionIOForm = new WTXModbus.FunctionIO();

                _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                DialogResult res = _functionIOForm.ShowDialog();
            }
            else
                if (_wtxDevice.Connection.ConnType == ConnectionType.Jetbus)
            {
                _functionIOForm = new WTXModbus.FunctionIO();

                _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                DialogResult res = _functionIOForm.ShowDialog();
            }
            _wtxDevice.Restart();
        }

        public void ReadDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            int out1 = _wtxDevice.DataStandard.Output1;
            int out2 = _wtxDevice.DataStandard.Output2;
            int out3 = _wtxDevice.DataStandard.Output3;
            int out4 = _wtxDevice.DataStandard.Output4;

            int in1 = _wtxDevice.DataStandard.Input1;
            int in2 = _wtxDevice.DataStandard.Input2;

            if (this.rbtConnectionModbus.Checked)    // If 'Modbus/Tcp' is selected, disconnect and reconnect from Jetbus to Modbus
            {
                _wtxDevice.Disconnect();
                ModbusTcpConnection _connection = new ModbusTcpConnection(_ipAddress);
                _wtxDevice = new HBM.Weighing.API.WTX.WTXModbus(_connection, this._timerInterval, this.update);
            }
        }
        public void WriteDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output1 = (int)e.FunctionOutputIO1;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output2 = (int)e.FunctionOutputIO2;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output3 = (int)e.FunctionOutputIO3;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output4 = (int)e.FunctionOutputIO4;

            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Input1 = (int)e.FunctionInputIO1;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Input2 = (int)e.FunctionInputIO2;

            if (this.rbtConnectionModbus.Checked)    // If 'Modbus/Tcp' is selected, disconnect and reconnect from Jetbus to Modbus
            {
                _wtxDevice.Disconnect();
                ModbusTcpConnection _connection = new ModbusTcpConnection(_ipAddress);
                _wtxDevice = new HBM.Weighing.API.WTX.WTXModbus(_connection, this._timerInterval, this.update);
            }
        }
        #endregion
        private void GUIsimple_Load(object sender, EventArgs e)
        {

        }
    }
}
