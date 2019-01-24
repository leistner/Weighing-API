// <copyright file="WTXModbus.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// HBM.Weighing.API, a library to communicate with HBM weighing technology devices  
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
using HBM.Weighing.API.Data;
using System;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HBM.Weighing.API.WTX
{
    /// <summary>
    /// This class handles the data from ModbusTcpConnection for IProcessData. 
    /// WtxModbus fetches, interprets the data and send it to the GUI or application by an eventhandler. 
    /// </summary>
    public class WtxModbus : BaseWtDevice
    {

        #region privates
        private ushort[] _data;
        private ushort[] _outputData;
        private ushort[] _dataWritten;
        
        private bool _isCalibrating;
        private int _timerInterval;
        private ApplicationMode _applicationMode;

        private ushort _command;
        private double dPreload, dNominalLoad, multiplierMv2D;      
        public System.Timers.Timer _aTimer;
        #endregion

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region Constructor
        public WtxModbus(INetConnection connection, int paramTimerInterval, EventHandler<ProcessDataReceivedEventArgs> OnProcessData) : base(connection)
        {
            this._connection = connection;

            this.ProcessDataReceived += OnProcessData;
            
            this._data = new ushort[100];
            this._outputData = new ushort[43]; // Output data length for filler application, also used for the standard application.
            this._dataWritten = new ushort[2];

            this._command = 0x00; 
            this._isCalibrating = false;
            this._timerInterval = 0;

            this.dPreload = 0;
            this.dNominalLoad = 0;
            this.multiplierMv2D = 500000;

            this.ReadBufferLength = 38; // inital setting. 

            // For the connection and initializing of the timer:            
            this.initialize_timer(paramTimerInterval);

            this.DataFiller = new DataFiller();
            this.DataStandard = new DataStandard();
        }
        #endregion

        #region Connection
        // To establish a connection to the WTX device via class WTX120_Modbus.
        public override void Connect(double timeoutMs)
        {
            this._connection.Connect();
        }


        // To establish a connection to the WTX device via class WTX120_Modbus.
        public override void Connect(Action<bool> ConnectCompleted, double timeoutMs)
        {
            this._connection.Connect();
        }



        public override bool isConnected
        {
            get
            {
                return _connection.IsConnected;
            }
        }

        // To terminate,break, a connection to the WTX device via class WTX120_Modbus.
        public override void Disconnect(Action<bool> DisconnectCompleted)
        {
            this._connection.Disconnect();
        }

        // To terminate,break, a connection to the WTX device via class WTX120_Modbus.
        public override void Disconnect()
        {
            this._connection.Disconnect();
        }

        public override string ConnectionType
        {
            get
            {
                return "Modbus";
            }
        }

        /// <summary>
        /// Length of read buffer in bytes for receiving ModbusTCP data.
        /// Only change for very fast applications with reduced data.
        /// </summary>
        public int ReadBufferLength {get; set;}
        #endregion

        #region Write methods
        // This method writes a data word to the WTX120 device synchronously. 
        private void WriteSync(ushort wordNumber, ushort commandParam)
        {
            int dataWord = 0x00;
            int handshakeBit = 0;

            this._command = commandParam;

            //if (this._command == 0x00)        ???
            //    dataWord = this._connection.Read(5);

            //else

            {
                // (1) Sending of a command:        
                this._connection.Write(wordNumber, this._command);
                dataWord = this._connection.Read(5);

                handshakeBit = ((dataWord & 0x4000) >> 14);
                // Handshake protocol as given in the manual:                            

                while (handshakeBit == 0)
                {
                    dataWord = this._connection.Read(5);
                    handshakeBit = ((dataWord & 0x4000) >> 14);
                }

                // (2) If the handshake bit is equal to 0, the command has to be set to 0x00.
                if (handshakeBit == 1)
                {
                    this._connection.Write(wordNumber, 0x00);
                }

                while (handshakeBit == 1) // Before : 'this.status == 1' additionally in the while condition. 
                {
                    dataWord = this._connection.Read(5);
                    handshakeBit = ((dataWord & 0x4000) >> 14);
                }
                
            }
        }

        public int getCommand
        {
            get { return this._command; }
        }

        
        private void WriteOutputWordS32(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0xffff0000) >> 16);
            _dataWritten[1] = (ushort)(valueParam & 0x0000ffff);

            this._connection.WriteArray(wordNumber, _dataWritten);
        }
        /*
        private void WriteOutputWordU08(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0x000000ff));
            this._connection.Write(wordNumber, _dataWritten[0]);
        }
        */
        /*
        public void WriteOutputWordU16(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0xffff0000) >> 16);

            this._connection.Write(wordNumber, _dataWritten[0]);
        }
        */
        public async Task<int> AsyncWrite(ushort index, ushort commandParam)
        {
            if (isConnected)
            {
                Task<int> WriteValue = _connection.WriteAsync(index, commandParam);
                DoHandshake(index);
                int command = await WriteValue;
                return command;
            }
            else
                return 0;
        }
        /// <summary>
        /// Handshake protocol as given in the manual
        /// </summary>
        /// <param name="index"></param>
        private void DoHandshake(ushort index)
        {
            int handshakeBit = 0;
            int dataWord = 0x00;
                       
            do
            {
                dataWord = this._connection.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
                Thread.Sleep(50);
            }
            while (handshakeBit == 0);
            
            this._connection.Write(index, 0x00);

            do
            {
                dataWord = this._connection.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
                Thread.Sleep(50);
            }
            while (handshakeBit == 1);
        }

        #endregion

        #region Timer methods
        // This methods sets the interval value of the timer. 
        public void ResetTimer(int timerIntervalParam)
        {
            this._aTimer.Enabled = false;
            this._aTimer.Stop();

            this._timerInterval = timerIntervalParam;

            this.initialize_timer(timerIntervalParam);
        }

        // This method initializes the with the timer interval as a parameter: 
        public void initialize_timer(int paramTimerInterval)
        {
            // Create a timer with an interval of the parameter value, if the argument paramTimerInterval is not valid,
            // an exception is catched and a default value for the timer interval is set, the timer tries to start again. 
            try
            {
                _aTimer = new System.Timers.Timer(paramTimerInterval);
            }
            catch (ArgumentException)
            {
                this._timerInterval = 100;   // In case if the timer interval is not valid, an 'ArgumentException' is catched and a default value for
                                             // the timer interval is set. 
                _aTimer = new System.Timers.Timer(this._timerInterval);
            }

            // Connect the elapsed event for the timer. 
            _aTimer.Elapsed += OnTimedEvent;

            _aTimer.AutoReset = true;
            _aTimer.Enabled = true;
            _aTimer.Start();
        }

        /*
        * This method stops the timer, for example in case for the calibration.
        */
        public void StopTimer()
        {
            _aTimer.Elapsed -= OnTimedEvent;
            _aTimer.Enabled = false;
            _aTimer.Stop();
        }

        /*
         * This method restarts the timer, for example in case for the calibration.
         */
        public void RestartTimer()
        {
            _aTimer.Elapsed += OnTimedEvent;
            _aTimer.Enabled = true;
            _aTimer.Start();
        }

        // Event method, which will be triggered after a interval of the timer is elapsed- 
        // After triggering (after 500ms) the register is read. 
        public async void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            // Call the async method 'AsyncTaskCall' by an Eventhandler:     
            if (isConnected)
            {
                Task<ushort[]> FetchValues = _connection.ReadAsync();
                //DoIndependentWork();
                ushort[] _asyncData = await FetchValues;
                OnData(_asyncData);
            }
        }
        #endregion

        public DataStandard DataStandard { get; set; }
        public DataFiller DataFiller { get; set; }

        int _previousNetValue = 0;

        #region Asynchronous process data callback
        /// <summary>
        /// Called whenever new device data is available 
        /// </summary>
        /// <param name="_asyncData"></param>
        public void OnData(ushort[] _data)
        {
            this.Update(_data);

            this._previousNetValue = ProcessData.NetValue;

            // Update process data : 
            if (_applicationMode == ApplicationMode.Standard)
            {
                ProcessData.UpdateProcessDataModbus(_data);
            }
            // Update data for standard mode:
            DataStandard.UpdateStandardDataModbus(_data);

            // Update data for filler mode:
            if (ApplicationMode == ApplicationMode.Filler)         
                DataFiller.UpdateFillerDataModbus(_data);
            
            // Only if the net value changed, the data will be send to the GUI
            if(_previousNetValue != ProcessData.NetValue)
                // Invoke Event - GUI/application class receives _processData: 
                this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
       
        }

        public void Update(ushort[] Data)
        {
            if ((_data[5] & 0x03) == 0)
                _applicationMode = ApplicationMode.Standard;
            else
                _applicationMode = ApplicationMode.Filler;        
        }
        #endregion

        #region Comment methods

        // In the following methods the different options for the single integer values are used to define and
        // interpret the value. Finally a string should be returned from the methods to write it onto the GUI Form. 
        public override string CurrentWeight(int value, int decimals)
        {
            double dvalue = value / Math.Pow(10, decimals);
            string returnvalue = "";

            switch (decimals)
            {
                case 0: returnvalue = dvalue.ToString(); break;
                case 1: returnvalue = dvalue.ToString("0.0"); break;
                case 2: returnvalue = dvalue.ToString("0.00"); break;
                case 3: returnvalue = dvalue.ToString("0.000"); break;
                case 4: returnvalue = dvalue.ToString("0.0000"); break;
                case 5: returnvalue = dvalue.ToString("0.00000"); break;
                case 6: returnvalue = dvalue.ToString("0.000000"); break;
                default: returnvalue = dvalue.ToString(); break;

            }
            return returnvalue;
        }


        public string WeightMovingStringComment()
        {
            if (ProcessData.WeightMoving == false)
                return "0=Weight is not moving.";
            else
                return "1=Weight is moving";
        }


        public string LimitStatusStringComment()
        {
            switch (ProcessData.LimitStatus)
            {
                case 0:
                    return "Weight within limits";
                case 1:
                    return "Lower than minimum";
                case 2:
                    return "Higher than maximum capacity";
                case 3:
                    return "Higher than safe load limit";
                default: 
                    return "Lower than minimum";
            }
        }

        public override string WeightTypeStringComment()
        {
            if (ProcessData.WeightType == false)
            {
                return "gross";
            }
            else
            {
                return "net";
            }
        }


        public override string ScaleRangeStringComment()
        {
            switch (ProcessData.ScaleRange)
            {
                case 0:
                    return "Range 1";
                case 1:
                    return "Range 2";
                case 2:
                    return "Range 3";
                default:
                    return "error";
            }
        }


        public override ApplicationMode ApplicationMode
        {
            get
            {
                return _applicationMode;
            }
        }


        public override string UnitStringComment()
        {
            switch (ProcessData.Unit)
            {
                case 0:
                    return "kg";
                case 1:
                    return "g";
                case 2:
                    return "t";
                case 3:
                    return "lb";
                default:
                    return "error";
            }
        }
        /*
        public override string StatusStringComment()
        {
            if (ProcessData.Status == 1)
                return "Execution OK!";
            else
                if (ProcessData.Status != 1)
                return "Execution not OK!";
            else
                return "error.";

        }
        */
        #endregion

        #region Adjustment methods 

        // This methods sets the value of the WTX to zero. 
        public override void MeasureZero()
        {
            this.StopTimer();

            //todo: write reg 48, 0x7FFFFFFF

            this.WriteOutputWordS32(0x7FFFFFFF, 48);

            Console.Write(".");

            this.WriteSync(0, 0x80);
        }

        // This method sets the value for the nominal weight in the WTX.
        public override void Calibrate(int calibrationValue, string calibrationWeightStr)
        {
            //write reg 46, CalibrationWeight     

            this.WriteOutputWordS32(calibrationValue, 46);

            //write reg 50, 0x7FFFFFFF

            this.WriteOutputWordS32(0x7FFFFFFF, 50);

            Console.Write(".");

            this.WriteSync(0, 0x100);

            this.RestartTimer();

            this._isCalibrating = true;

            // Check if the values of the WTX device are equal to the calibration value. It is also checked within a certain interval if the measurement is noisy.
            if ((ProcessData.NetValue != calibrationValue || ProcessData.GrossValue != calibrationValue))
            {
                //Read method
            }
        }
        
        public double getDPreload
        {
            get
            {
                return dPreload;
            }
        }

        public double getDNominalLoad
        {
            get
            {
                return dNominalLoad;
            }
        }


        // Calculates the values for deadload and nominal load in d from the inputs in mV/V
        // and writes the into the WTX registers.
        public override void Calculate(double preload, double capacity)
        {
            dPreload = 0;
            dNominalLoad = 0; 

            multiplierMv2D = 500000; //   2 / 1000000; // 2mV/V correspond 1 million digits (d)

            dPreload = preload * multiplierMv2D;
            dNominalLoad = dPreload + (capacity * multiplierMv2D);
                           
            this.StopTimer();

            //write reg 48, DPreload;         

            this.WriteOutputWordS32(Convert.ToInt32(dPreload), 48);

            this.WriteSync(0, 0x80);

            //write reg 50, DNominalLoad;          

            this.WriteOutputWordS32(Convert.ToInt32(dNominalLoad), 50);

            this.WriteSync(0, 0x100);

            this._isCalibrating = true;

            this.RestartTimer();

        }

        public bool Calibrating
        {
            get { return this._isCalibrating; }
            set { this._isCalibrating = value; }
        }
        #endregion

        #region Process data methods - Standard
        public async override void SetGross()
        {
            _command = (ushort)await AsyncWrite(0, 0x2);
        }
        
        public async override void Tare()
        {
            _command = (ushort)await AsyncWrite(0, 0x1);
        }
        
        public async override void Zero()
        {
            _command = (ushort)await AsyncWrite(0, 0x40);
        }

        public async override void adjustZero()
        {
            _command = (ushort)await AsyncWrite(0, 0x80);
        }

        public async override void adjustNominal()
        {
            _command = (ushort)await AsyncWrite(0, 0x100);
        }

        public async override void activateData()
        {
            _command = (ushort)await AsyncWrite(0, 0x800);
        }

        public async override void manualTaring()
        {
            _command = (ushort)await AsyncWrite(0, 0x1000);
        }
        #endregion

        #region Process data methods - Filling
        public async override void clearDosingResults()
        {
            _command = (ushort)await AsyncWrite(0, 0x4);
        }

        public async override void abortDosing()
        {
            _command = (ushort)await AsyncWrite(0, 0x8);
        }

        public async override void startDosing()
        {
            _command = (ushort)await AsyncWrite(0, 0x10);
        }

        public async override void recordWeight()
        {
            _command = (ushort)await AsyncWrite(0, 0x4000);
        }

        public async override void manualReDosing()
        {
            _command = (ushort)await AsyncWrite(0, 0x8000);
        }
        #endregion


    }
}