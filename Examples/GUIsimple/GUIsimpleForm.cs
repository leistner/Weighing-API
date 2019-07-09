// <copyright file="GUIsimpleForm.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace GUIsimple
{
    using System;
    using System.Windows.Forms;
    using Hbm.Weighing.Api;
    using Hbm.Weighing.Api.Data;
    using Hbm.Weighing.Api.WTX;
    using Hbm.Weighing.Api.WTX.Jet;
    using Hbm.Weighing.Api.WTX.Modbus;

    /// <summary>
    /// This application example demonstrates the usage of HBM Weighing-API.
    /// It shows how to connect a WTX device via Modbus and Jetbus, how to get weight values, to calibrate and how to adjust the scale.
    /// </summary>
    public partial class GUIsimpleForm : Form
    {
        #region ==================== constants & fields ====================

        private const string DEFAULT_IP_ADDRESS = "192.168.100.88";
        private const string MESSAGE_CONNECTION_FAILED = "Connection failed!";
        private const string MESSAGE_CONNECTING = "Connecting...";
        private const int WAIT_DISCONNECT = 2000;

        private static BaseWTDevice _wtxDevice;
        private AdjustmentCalculator _adjustmentCalculator;
        private AdjustmentWeigher _adjustmentWeigher;
        private FunctionIO _functionIOForm;
        private string _ipAddress = DEFAULT_IP_ADDRESS;
        private int _timerInterval = 200;

        #endregion
        
        #region =============== constructors & destructors =================

        /// <summary>
        /// Constructor of GUIsimpleForm
        /// </summary>
        /// <param name="args">Parameter from command line</param>
        public GUIsimpleForm(string[] args)
        {
            InitializeComponent();
            DisplayText("Check IP address, select 'Jet' or 'Modbus/TCP' and press 'Connect'.");
            EvaluateCommandLine(args);    
            txtIPAddress.Text = _ipAddress;
            picNE107.Image = Properties.Resources.NE107_DiagnosisPassive;
        }
        #endregion

        #region =============== protected & private methods ================

        /// <summary>
        /// Initialze a Jetbus or Modbus/Tcp connection, creates objects of INetConnection derivations, BaseWtDevice derivations
        /// </summary>
        private void InitializeConnection()
        {
            this._ipAddress = txtIPAddress.Text;

            if (this.rbtConnectionModbus.Checked)  
            {
                // Creating objects of ModbusTcpConnection and WTXModbus: 
                ModbusTCPConnection _modbusConnection = new ModbusTCPConnection(this._ipAddress);
                _wtxDevice = new WTXModbus(_modbusConnection, this._timerInterval, this.update);

            }
            else
            {
                if (this.rbtConnectionJet.Checked)
                {
                    // Creating objects of JetBusConnection and WTXJet: 
                    JetBusConnection _jetConnection = new JetBusConnection(_ipAddress, "Administrator", "wtx");
                    _wtxDevice = new WTXJet(_jetConnection,500, update);
                }
            }

            // Connection establishment via Modbus or Jetbus            
            try
            {
                _wtxDevice.Connect(5000);
            }
            catch (Exception)
            {
                DisplayText(MESSAGE_CONNECTION_FAILED);
            }

            if (_wtxDevice.IsConnected)
            {
                picNE107.Image = Properties.Resources.NE107_DiagnosisActive;
                Properties.Settings.Default.IPAddress = this._ipAddress;
                Properties.Settings.Default.IsJetBus = rbtConnectionJet.Checked;
                Properties.Settings.Default.Save();
            }
            else
            {
                picNE107.Image = Properties.Resources.NE107_DiagnosisPassive;
                DisplayText(MESSAGE_CONNECTION_FAILED);
            }

        }

        /// <summary>
        /// How to get process data automatically
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">Here you find the current process data (e.g. weight value)</param>
        private void update(object sender, ProcessDataReceivedEventArgs e)
        {           
            this.BeginInvoke(new Action(() =>
            {
                DisplayText("Net:" + _wtxDevice.PrintableWeight.Net + _wtxDevice.Unit + Environment.NewLine
                + "Gross:" + _wtxDevice.PrintableWeight.Gross + _wtxDevice.Unit + Environment.NewLine
                + "Tara:" + _wtxDevice.PrintableWeight.Gross + _wtxDevice.Unit);

                if (e.ProcessData.Underload == true)
                {
                    DisplayText("Underload : Lower than minimum" + Environment.NewLine);
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;

                }
                else if (e.ProcessData.Overload == true)
                {
                    DisplayText("Overload : Higher than maximum capacity" + Environment.NewLine);
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;

                }
                else if (e.ProcessData.HigherSafeLoadLimit == true)
                {
                    DisplayText("Higher than safe load limit" + Environment.NewLine);
                    picNE107.Image = Properties.Resources.NE107_OutOfSpecification;
                }
                else
                    picNE107.Image = Properties.Resources.NE107_DiagnosisActive;
            }));
        }

        /// <summary>
        /// Command line control
        /// </summary>
        /// <param name="args">Possible arguments: modbus or jet, ip address</param>
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
            else
            {
                if (Properties.Settings.Default.IsJetBus)
                    rbtConnectionJet.Checked = true;
                else
                    rbtConnectionModbus.Checked = true;
            }

            if (args.Length > 1)
                _ipAddress = args[1];
            else
                _ipAddress = Properties.Settings.Default.IPAddress;

            if (args.Length > 2)
                this._timerInterval = Convert.ToInt32(args[2]);
        }

        /// <summary>
        /// Displays the string, the measured values, unit etc. 
        /// </summary>
        /// <param name="text"></param>
        private void DisplayText(string text)
        {
            txtInfo.Text = text;
            Application.DoEvents();
        }
        
        /// <summary>
        /// Connects to wtx device
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdConnect_Click(object sender, EventArgs e)
        {
            if (_wtxDevice != null)
            {
                DisplayText("Disconnecting...");
                _wtxDevice.Connection.Disconnect();
                _wtxDevice = null;
            }

            DisplayText("Connecting...");
            this.InitializeConnection();          
        }

        /// <summary>
        /// button click event for switching to gross or net value. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdGrossNet_Click(object sender, EventArgs e)
        {
                _wtxDevice.SetGross();
        }

        /// <summary>
        /// button click event for zeroing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdZero_Click(object sender, EventArgs e)
        {
                _wtxDevice.Zero();
        }

        /// <summary>
        /// button click event for taring 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmdTare_Click(object sender, EventArgs e)
        {
            _wtxDevice.Tare();
        }

        //Method for calculate adjustment with dead load and span: 
        private void calibrationWithWeightToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (_wtxDevice != null)
            {
                _adjustmentCalculator = new AdjustmentCalculator(_wtxDevice);
                DialogResult res = _adjustmentCalculator.ShowDialog();
            }
        }

        /// <summary>
        /// Adjustment with weight
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void calibrationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (_wtxDevice != null)
            {
                _adjustmentWeigher = new AdjustmentWeigher(_wtxDevice);
                DialogResult res = _adjustmentWeigher.ShowDialog();
            }
        }

        /// <summary>
        /// Toolstrip Click Event for Digital IO funcion settings: Input & Output functions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (_wtxDevice != null)
            {
                _wtxDevice.Stop();

                if (_wtxDevice.Connection.ConnectionType == ConnectionType.Modbus)
                {
                    _wtxDevice.Disconnect();

                    JetBusConnection _connection = new JetBusConnection(_ipAddress);
                    _wtxDevice = new WTXJet(_connection, 500, update);

                    _wtxDevice.Connect(5000);

                    _functionIOForm = new FunctionIO();

                    _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                    _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                    DialogResult res = _functionIOForm.ShowDialog();
                }
                else
                    if (_wtxDevice.Connection.ConnectionType == ConnectionType.Jetbus)
                {
                    _functionIOForm = new FunctionIO();

                    _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                    _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                    DialogResult res = _functionIOForm.ShowDialog();
                }
                _wtxDevice.Restart();
            }
        }

        private void ReadDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            /*DDD Todo
            int out1 = _wtxDevice.DataStandard.Output1;
            int out2 = _wtxDevice.DataStandard.Output2;
            int out3 = _wtxDevice.DataStandard.Output3;
            int out4 = _wtxDevice.DataStandard.Output4;
            int in1 = _wtxDevice.DataStandard.Input1;
            int in2 = _wtxDevice.DataStandard.Input2;
            */
            if (this.rbtConnectionModbus.Checked)    // If 'Modbus/Tcp' is selected, disconnect and reconnect from Jetbus to Modbus
            {
                _wtxDevice.Disconnect();
                ModbusTCPConnection _connection = new ModbusTCPConnection(_ipAddress);
                _wtxDevice = new Hbm.Weighing.Api.WTX.WTXModbus(_connection, this._timerInterval, this.update);
            }
        }

        private void WriteDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            /*DDD Todo
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
            */
            if (this.rbtConnectionModbus.Checked)    // If 'Modbus/Tcp' is selected, disconnect and reconnect from Jetbus to Modbus
            {
                _wtxDevice.Disconnect();
                ModbusTCPConnection _connection = new ModbusTCPConnection(_ipAddress);
                _wtxDevice = new Hbm.Weighing.Api.WTX.WTXModbus(_connection, this._timerInterval, this.update);
            }
        }
        #endregion

    }
}
