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
using HBM.Weighing.API.Utils;
using HBM.Weighing.API.WTX.Modbus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HBM.Weighing.API.WTX
{
    /// <summary>
    /// This class handles the data from ModbusTcpConnection for IProcessData. 
    /// WtxModbus fetches, interprets the data( method OnData(data) ) and 
    /// send it to the GUI or application class by an eventhandler (=ProcessDataReceived). 
    /// </summary>
    public class WTXModbus : BaseWTDevice
    {

        #region privates

        private ushort[] _data;
        private ushort[] _outputData;
        private ushort[] _dataWritten;
        private ushort[] _asyncData;

        private int _timerInterval;
        
        private int _manualTareValue;
        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nominalLoad;

        private ushort _command;
        private double dPreload, dNominalLoad, multiplierMv2D;      
        public System.Timers.Timer _aTimer;

        private Dictionary<string, JToken> _dataDict;
        #endregion

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        /// <summary>
        /// Constructor with  
        /// </summary>
        /// <param name="connection"></param>
        #region Constructor
        public WTXModbus(INetConnection connection, int paramTimerInterval, EventHandler<ProcessDataReceivedEventArgs> OnProcessData) : base(connection)
        {
            this.Connection = connection;

            this.ProcessDataReceived += OnProcessData;

            ProcessData = new ProcessDataModbus(base.Connection);
            DataStandard = new DataStandardModbus(base.Connection);
            DataFiller = new DataFillerModbus(base.Connection);

            this._data = new ushort[100];
            this._asyncData = new ushort[100];
            this._outputData = new ushort[43]; // Output data length for filler application, also used for the standard application.
            this._dataWritten = new ushort[2];

            this._command = 0x00; 
            this._timerInterval = 0;

            this.dPreload = 0;
            this.dNominalLoad = 0;
            this.multiplierMv2D = 500000;

            this.ReadBufferLength = 38; // inital setting. 

            // For the connection and initializing of the timer:            
            this.initialize_timer(paramTimerInterval);
        }

        /// <summary>
        /// For a more simple solution : Constructor with no asynchronous callback and no timer interval for continously updating the data. 
        /// </summary>
        /// <param name="connection"></param>
        public WTXModbus(INetConnection connection) : base(connection)
        {
            this.Connection = connection;

            ProcessData = new ProcessDataModbus(base.Connection);
            DataStandard = new DataStandardModbus(base.Connection);
            DataFiller = new DataFillerModbus(base.Connection);

            this._data = new ushort[100];
            this._asyncData = new ushort[100];
            this._outputData = new ushort[43]; // Output data length for filler application, also used for the standard application.
            this._dataWritten = new ushort[2];

            this._command = 0x00;

            this.dPreload = 0;
            this.dNominalLoad = 0;
            this.multiplierMv2D = 500000;

            this.ReadBufferLength = 38; // inital setting. 
        }
        #endregion

        #region Connection
        // To establish a connection to the WTX device via class WTX120_Modbus.
        public override void Connect(double timeoutMs = 2000)
        {
            this.Connection.Connect();
        }
        // To establish a connection to the WTX device via class WTX120_Modbus.
        public override void Connect(Action<bool> ConnectCompleted, double timeoutMs)
        {
            this.Connection.Connect();
        }
        public override bool IsConnected
        {
            get
            {
                return Connection.IsConnected;
            }
        }

        // To terminate,break, a connection to the WTX device via class WTX120_Modbus.
        public override void Disconnect(Action<bool> DisconnectCompleted)
        {
            this.Connection.Disconnect();
        }

        // To terminate,break, a connection to the WTX device via class WTX120_Modbus.
        public override void Disconnect()
        {
            this.Connection.Disconnect();
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

        public int getCommand
        {
            get { return this._command; }
        }

        public async Task<int> AsyncWrite(ushort index, ushort commandParam)
        {
            if (IsConnected)
            {
                Task<int> WriteValue = Connection.WriteAsync(index, commandParam);
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
                dataWord = ((ModbusTCPConnection)this.Connection).ReadSingle(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
                Thread.Sleep(50);
            }
            while (handshakeBit == 0);

            this.Connection.Write(new ModbusCommand(DataType.U08, "0", IOType.Output, ApplicationMode.Standard, 0, 0),0);  

            do
            {
                dataWord = ((ModbusTCPConnection)this.Connection).ReadSingle(5);
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
        public override void Stop()
        {
            _aTimer.Elapsed -= OnTimedEvent;
            _aTimer.Enabled = false;
            _aTimer.Stop();
        }

        /*
         * This method restarts the timer, for example in case for the calibration.
         */
        public override void Restart()
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
            if (IsConnected)
            {
                Task<ushort[]> FetchValues = Connection.ReadAsync();
                //DoIndependentWork();
                _asyncData = await FetchValues;
                OnData(_asyncData);
            }
        }
        #endregion

        #region Asynchronous process data callback
        /// <summary>
        /// Called whenever new device data is available 
        /// </summary>
        /// <param name="_data"></param>
        public void OnData(ushort[] _data)
        {
            this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));   
        }

        public void UpdateApplicationMode(ushort[] Data)
        {
            if ((_data[5] & 0x03) == 0)
                ApplicationMode = ApplicationMode.Standard;
            else
                ApplicationMode = ApplicationMode.Filler;        
        }
        #endregion

        #region Comment methods

        public string WeightMovingStringComment()
        {
            if (ProcessData.WeightStable)
                return "0=Weight is not moving.";
            else
                return "1=Weight is moving";
        }

        public override int ManualTareValue 
        {
            get { return _manualTareValue; }
            set
            {
                //Connection.Write(this.getIndex(Connection.IDCommands.TARE_VALUE), value);
                _manualTareValue = value;
            }
        }

        public override TareMode TareMode
        {
            get
            {
                return ProcessData.TareMode;
            }
        }


        public override int ScaleRange
        {
            get
            {
                return ProcessData.ScaleRange + 1;
            }
        }
        public override ApplicationMode ApplicationMode { get; set; }

        public override string Unit
        {
            get
            {
                return ProcessData.Unit;
            }
        }
        
        #endregion

        #region Adjustment methods 
        public override int CalibrationWeight // Type : signed integer 32 Bit
        {
            get { return _calibrationWeight; }
            set
            {
                //Connection.WriteArray(this.getIndex(Connection.IDCommands.LFT_SCALE_CALIBRATION_WEIGHT), value);
                _calibrationWeight = value;
            }
        }
        public override int ZeroSignal // Type : signed integer 32 Bit
        {
            get { return _zeroLoad; }
            set
            {
                //Connection.WriteArray(this.getIndex(Connection.IDCommands.LDW_DEAD_WEIGHT), value);
                _zeroLoad = value;
            }
        }
        public override int NominalSignal // Type : signed integer 32 Bit
        {
            get { return _nominalLoad; }
            set
            {
                //Connection.WriteArray(this.getIndex(Connection.IDCommands.LWT_NOMINAL_VALUE), value);
                _nominalLoad = value;
            }
        }

        // This methods sets the value of the WTX to zero. 
        public override bool AdjustZeroSignal()
        {
            this.Stop();
                        
            Connection.Write(ModbusCommands.LDWZeroSignal, 0x7FFFFFFF);

            Connection.Write(ModbusCommands.Control_word_AdjustZero, 0x80);

            return true; //DDD
        }

        public override bool AdjustNominalSignal()
        {
            this.Stop();

            Connection.Write(ModbusCommands.LWTNominalSignal, 0x7FFFFFFF);
        
            Connection.Write(ModbusCommands.Control_word_AdjustNominal, 0x100);

            return true; //DDD
        }

        // This method sets the value for the nominal weight in the WTX.
        public override bool AdjustNominalSignalWithCalibrationWeight(double adjustmentWeight)
        {
            //write reg 46, CalibrationWeight     

            Connection.Write(ModbusCommands.CWTScaleCalibrationWeight, MeasurementUtils.DoubleToDigit(adjustmentWeight,ProcessData.Decimals));

            //write reg 50, 0x7FFFFFFF
                  
            Connection.Write(ModbusCommands.LWTNominalSignal, 0x7FFFFFFF);

            Connection.Write(ModbusCommands.Control_word_AdjustNominal, 0x100);
            
            this.Restart();

            return true; //DDD
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
        public override void CalculateAdjustment(double preload, double capacity)
        {
            dPreload = 0;
            dNominalLoad = 0; 

            multiplierMv2D = 500000; //   2 / 1000000; // 2mV/V correspond 1 million digits (d)

            dPreload = preload * multiplierMv2D;
            dNominalLoad = dPreload + (capacity * multiplierMv2D);
                           
            this.Stop();

            //write reg 48, DPreload;         
            Connection.Write(ModbusCommands.LDWZeroSignal, Convert.ToInt32(dPreload));

            Connection.Write(ModbusCommands.Control_word_AdjustZero, 0x80);

            //write reg 50, DNominalLoad;          
            Connection.Write(ModbusCommands.LWTNominalSignal, Convert.ToInt32(dNominalLoad));

            Connection.Write(ModbusCommands.Control_word_AdjustNominal, 0x100);

            this.Restart();

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
        
        public async void ActivateData()
        {
            _command = (ushort)await AsyncWrite(0, 0x800);
        }

        public async override void TareManually(double manualTareValue)
        {
            _command = (ushort)await AsyncWrite(0, 0x1000);
        }
        #endregion

        public async override void RecordWeight()
        {
            _command = (ushort)await AsyncWrite(0, 0x4000);
        }

        public void ActivateDate()
        {
        }

        public override WeightType Weight
        {
            get
            {
                return ProcessData.Weight;
            }
        }

        public override PrintableWeightType PrintableWeight
        {
            get
            {
                return ProcessData.PrintableWeight;
            }
        }

    }
}