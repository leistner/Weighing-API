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
using System;
using System.ComponentModel;
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
        private ProcessData _processData;
        
        private ushort[] _data;
        private ushort[] _outputData;
        private ushort[] _dataWritten;
        
        private bool _isCalibrating;
        private int _timerInterval;
        
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
            _processData = new ProcessData();

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
        public void WriteSync(ushort wordNumber, ushort commandParam)
        {
            int dataWord = 0x00;
            int handshakeBit = 0;

            this._command = commandParam;

            if (this._command == 0x00)
                dataWord = this._connection.Read(5);

            else
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
        
        public void WriteOutputWordS32(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0xffff0000) >> 16);
            _dataWritten[1] = (ushort)(valueParam & 0x0000ffff);

            this._connection.WriteArray(wordNumber, _dataWritten);
        }

        public void WriteOutputWordU08(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0x000000ff));
            this._connection.Write(wordNumber, _dataWritten[0]);
        }

        public void WriteOutputWordU16(int valueParam, ushort wordNumber)
        {
            _dataWritten[0] = (ushort)((valueParam & 0xffff0000) >> 16);

            this._connection.Write(wordNumber, _dataWritten[0]);
        }

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

        private void DoHandshake(ushort index)
        {
            int handshakeBit = 0;
            int dataWord = 0x00;

            // Handshake protocol as given in the manual:                            
            do
            {
                dataWord = this._connection.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);

            } while (this.Handshake == 0);

            // (2) If the handshake bit is equal to 0, the command has to be set to 0x00.
            if (handshakeBit == 1)
            {
                this._connection.Write(index, 0x00);
            }

            while (handshakeBit == 1) // Before : 'this.status == 1' additionally in the while condition. 
            {
                dataWord = this._connection.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
            }
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

                /* Alternative
                Task FetchValues = connection.ReadAsync().ContinueWith(t =>
                {
                    ushort[] _asyncData = t.Result;
                    OnData(_asyncData);
                });
                */
            }
        }
        #endregion

        #region Asynchronous process data callback
        /// <summary>
        /// Called whenever new device data is available 
        /// </summary>
        /// <param name="_asyncData"></param>
        public override void OnData(ushort[] _asyncData)
        {
            this._data = _asyncData;
        
                _processData.NetValue = this.NetValue;
                _processData.GrossValue = this.GrossValue;
                _processData.NetValueStr = this.CurrentWeight(this.NetValue, this.Decimals);
                _processData.GrossValueStr = this.CurrentWeight(this.GrossValue, this.Decimals);

                _processData.Tare = this.NetValue - this.GrossValue;
                _processData.GeneralWeightError = Convert.ToBoolean(this.GeneralWeightError);
                _processData.ScaleAlarmTriggered = Convert.ToBoolean(this.ScaleAlarmTriggered);
                _processData.LimitStatus = this.LimitStatus;
                _processData.WeightMoving = Convert.ToBoolean(this.WeightMoving);
                _processData.ScaleSealIsOpen = Convert.ToBoolean(this.ScaleSealIsOpen);
                _processData.ManualTare = Convert.ToBoolean(this.ManualTare);
                _processData.WeightType = Convert.ToBoolean(this.WeightType);
                _processData.ScaleRange = this.ScaleRange;
                _processData.ZeroRequired = Convert.ToBoolean(this.ZeroRequired);
                _processData.WeightWithinTheCenterOfZero = Convert.ToBoolean(this.WeightWithinTheCenterOfZero);
                _processData.WeightInZeroRange = Convert.ToBoolean(this.WeightInZeroRange);
                _processData.ApplicationMode = this.ApplicationMode;
                _processData.ApplicationModeStr = ApplicationModeStringComment();
                _processData.Decimals = this.Decimals;
                _processData.Unit = this.Unit;
                _processData.Handshake = Convert.ToBoolean(this.Handshake);
                _processData.Status = Convert.ToBoolean(this.Status);

                this.limitStatusBool();                                      // update the booleans 'Underload', 'Overload', 'weightWithinLimits', 'higherSafeLoadLimit'. 
                _processData.LegalTradeOp = this.LegalTradeOp;

                this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(_processData));

                this._isCalibrating = false;       
            
        }
        
        public void UpdateOutputWords(ushort []valueArr)
        {
            for(int index=0;index<valueArr.Length;index++)
            {
               _outputData[index] = valueArr[index];
            }         
        }
        #endregion

        #region Process data - Standard 

        public override int NetValue
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 1)
                        return (_data[1] + (_data[0] << 16));
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int GrossValue
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 3)
                        return (_data[3] + (_data[2] << 16));
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int GeneralWeightError
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return (_data[4] & 0x1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ScaleAlarmTriggered
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x2) >> 1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LimitStatus
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0xC) >> 2);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMoving
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x10) >> 4);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ScaleSealIsOpen
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x20) >> 5);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ManualTare
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x40) >> 6);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightType
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x80) >> 7);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ScaleRange
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x300) >> 8);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ZeroRequired
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x400) >> 10);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightWithinTheCenterOfZero
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x800) >> 11);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightInZeroRange
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 4)
                        return ((_data[4] & 0x1000) >> 12);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ApplicationMode
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 5)
                        return (_data[5] & 0x3>>1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Decimals
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 5)
                        return ((_data[5] & 0x70) >> 4);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Unit
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 5)
                        return ((_data[5] & 0x180) >> 7);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Handshake
        {
            get
            {
                try
                {
                    //if (this.connection.NumofPoints > 5)
                    return ((_data[5] & 0x4000) >> 14);
                    //else
                    //    return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }

            }
        }
        public override int Status
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 5)
                        return ((_data[5] & 0x8000) >> 15);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }

        public override int Input1
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 6)
                        return (_data[6] & 0x1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }


        }
        public override int Input2
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 6)
                        return ((_data[6] & 0x2) >> 1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Input3
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 6)
                        return ((_data[6] & 0x4) >> 2);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Input4
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 6)
                        return ((_data[6] & 0x8) >> 3);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Output1
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 7)
                        return (_data[7] & 0x1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Output2
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 7)
                        return ((_data[7] & 0x2) >> 1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Output3
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 7)
                        return ((_data[7] & 0x4) >> 2);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Output4
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 7)
                        return ((_data[7] & 0x8) >> 3);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LimitStatus1
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return (_data[8] & 0x1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LimitStatus2
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x2) >> 1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LimitStatus3
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x4) >> 2);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LimitStatus4
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x8) >> 3);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemDay
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 9)
                        return (_data[9]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemMonth
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 10)
                        return (_data[10]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemYear
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 11)
                        return (_data[11]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemSeqNumber
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 12)
                        return (_data[12]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemGross
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 13)
                        return (_data[13]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int WeightMemNet
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 14)
                        return (_data[14]);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }

        public override int ManualTareValue
        {
            get
            {
                return this._outputData[0];
            }
            set
            {
                this._outputData[0] = (ushort)value;
            }
        }
        public override int LimitValue1Input
        {
            get
            {
                return this._outputData[1];
            }
            set
            {
                this._outputData[1] = (ushort)value;
            }
        }
        public override int LimitValue1Mode
        {
            get
            {
                return this._outputData[2];
            }
            set
            {
                this._outputData[2] = (ushort)value;
            }
        }
        public override int LimitValue1ActivationLevelLowerBandLimit
        {
            get
            {
                return this._outputData[3];
            }
            set
            {
                this._outputData[3] = (ushort)value;
            }
        }
        public override int LimitValue1HysteresisBandHeight
        {
            get
            {
                return this._outputData[4];
            }
            set
            {
                this._outputData[4] = (ushort)value;
            }
        }

        public override int LimitValue2Source
        {
            get
            {
                return this._outputData[5];
            }
            set
            {
                this._outputData[5] = (ushort)value;
            }
        }
        public override int LimitValue2Mode
        {
            get
            {
                return this._outputData[6];
            }
            set
            {
                this._outputData[6] = (ushort)value;
            }
        }
        public override int LimitValue2ActivationLevelLowerBandLimit
        {
            get
            {
                return this._outputData[7];
            }
            set
            {
                this._outputData[7] = (ushort)value;
            }
        }
        public override int LimitValue2HysteresisBandHeight
        {
            get
            {
                return this._outputData[8];
            }
            set
            {
                this._outputData[8] = (ushort)value;
            }
        }

        public override int LimitValue3Source
        {
            get
            {
                return this._outputData[9];
            }
            set
            {
                this._outputData[9] = (ushort)value;
            }
        }
        public override int LimitValue3Mode
        {
            get
            {
                return this._outputData[10];
            }
            set
            {
                this._outputData[10] = (ushort)value;
            }
        }
        public override int LimitValue3ActivationLevelLowerBandLimit
        {
            get
            {
                return this._outputData[11];
            }
            set
            {
                this._outputData[11] = (ushort)value;
            }
        }
        public override int LimitValue3HysteresisBandHeight
        {
            get
            {
                return this._outputData[12];
            }
            set
            {
                this._outputData[12] = (ushort)value;
            }
        }

        public override int LimitValue4Source
        {
            get
            {
                return this._outputData[13];
            }
            set
            {
                this._outputData[13] = (ushort)value;
            }
        }
        public override int LimitValue4Mode
        {
            get
            {
                return this._outputData[14];
            }
            set
            {
                this._outputData[14] = (ushort)value;
            }
        }
        public override int LimitValue4ActivationLevelLowerBandLimit
        {
            get
            {
                return this._outputData[15];
            }
            set
            {
                this._outputData[15] = (ushort)value;
            }
        }
        public override int LimitValue4HysteresisBandHeight
        {
            get
            {
                return this._outputData[16];
            }
            set
            {
                this._outputData[16] = (ushort)value;
            }
        }
        #endregion

        #region Process data - Filling
        public override int CoarseFlow
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return (_data[8] & 0x1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int FineFlow
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x2) >> 1);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Ready
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x4) >> 2);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ReDosing
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x8) >> 3);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Emptying
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x10) >> 4);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int FlowError
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x20) >> 5);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int Alarm
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x40) >> 6);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int AdcOverUnderload
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x80) >> 7);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }

        public override int MaxDosingTime
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x100) >> 8);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int LegalTradeOp
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x200) >> 9);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ToleranceErrorPlus
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x400) >> 10);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ToleranceErrorMinus
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x800) >> 11);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int StatusInput1
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x4000) >> 14);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int GeneralScaleError
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 8)
                        return ((_data[8] & 0x8000) >> 15);
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int FillingProcessStatus
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 9)
                        return _data[9];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int NumberDosingResults
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 11)
                        return _data[11];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int DosingResult
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 12)
                        return _data[12];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int MeanValueDosingResults
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 14)
                        return _data[14];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int StandardDeviation
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 16)
                        return _data[16];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int TotalWeight
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 18)
                        return _data[18];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int FineFlowCutOffPoint
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 20)
                        return _data[20];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int CoarseFlowCutOffPoint
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 22)
                        return _data[22];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int CurrentDosingTime
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 24)
                        return _data[24];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int CurrentCoarseFlowTime
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 25)
                        return _data[25];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int CurrentFineFlowTime
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 26)
                        return _data[26];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public override int ParameterSetProduct
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 27)
                        return _data[27];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }

        public int FillerWeightMemoryDay
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 28)
                        return _data[28];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public int FillerWeightMemoryMonth
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 29)
                        return _data[29];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public int FillerWeightMemoryYear
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 30)
                        return _data[30];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }

        }
        public int FillerWeightMemorySeqNumber
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 31)
                        return _data[31];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public int FillerWeightMemoryGross
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 32)
                        return _data[32];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }
        public int FillerWeightMemoryNet
        {
            get
            {
                try
                {
                    if (this.ReadBufferLength > 33)
                        return _data[33];
                    else
                        return 0;
                }
                catch (System.IndexOutOfRangeException)
                {
                    return 0;
                }
            }
        }

        public override int ResidualFlowTime
        {
            get { return this._outputData[17]; }
            set { this._outputData[17] = (ushort)value; }
        }

        public override int TargetFillingWeight
        {
            get { return this._outputData[18]; }
            set { this._outputData[18] = (ushort)value; }
        }

        public override int CoarseFlowCutOffPointSet
        {
            get { return this._outputData[19]; }
            set { this._outputData[19] = (ushort)value; }
        }

        public override int FineFlowCutOffPointSet
        {
            get { return this._outputData[20]; }
            set { this._outputData[20] = (ushort)value; }
        }
        public override int MinimumFineFlow
        {
            get { return this._outputData[21]; }
            set { this._outputData[21] = (ushort)value; }
        }

        public override int OptimizationOfCutOffPoints
        {
            get { return this._outputData[22]; }
            set { this._outputData[22] = (ushort)value; }
        }
        public override int MaximumDosingTime
        {
            get { return this._outputData[23]; }
            set { this._outputData[23] = (ushort)value; }
        }
        public override int StartWithFineFlow
        {
            get { return this._outputData[24]; }
            set { this._outputData[24] = (ushort)value; }
        }
        public override int CoarseLockoutTime
        {
            get { return this._outputData[25]; }
            set { this._outputData[25] = (ushort)value; }
        }
        public override int FineLockoutTime
        {
            get { return this._outputData[26]; }
            set { this._outputData[26] = (ushort)value; }
        }
        public override int TareMode
        {
            get { return this._outputData[27]; }
            set { this._outputData[27] = (ushort)value; }
        }
        public override int UpperToleranceLimit
        {
            get { return this._outputData[28]; }
            set { this._outputData[28] = (ushort)value; }
        }
        public override int LowerToleranceLimit
        {
            get { return this._outputData[29]; }
            set { this._outputData[29] = (ushort)value; }
        }
        public override int MinimumStartWeight
        {
            get { return this._outputData[30]; }
            set { this._outputData[30] = (ushort)value; }
        }
        public override int EmptyWeight
        {
            get { return this._outputData[31]; }
            set { this._outputData[31] = (ushort)value; }
        }
        public override int TareDelay
        {
            get { return this._outputData[32]; }
            set { this._outputData[32] = (ushort)value; }
        }
        public override int CoarseFlowMonitoringTime
        {
            get { return this._outputData[33]; }
            set { this._outputData[33] = (ushort)value; }
        }
        public override int CoarseFlowMonitoring
        {
            get { return this._outputData[34]; }
            set { this._outputData[34] = (ushort)value; }
        }
        public override int FineFlowMonitoring
        {
            get { return this._outputData[35]; }
            set { this._outputData[35] = (ushort)value; }
        }
        public override int FineFlowMonitoringTime
        {
            get { return this._outputData[36]; }
            set { this._outputData[36] = (ushort)value; }
        }
        public override int DelayTimeAfterFineFlow
        {
            get { return this._outputData[37]; }
            set { this._outputData[37] = (ushort)value; }
        }
        public override int ActivationTimeAfterFineFlow
        {
            get { return this._outputData[38]; }
            set { this._outputData[38] = (ushort)value; }
        }
        public override int SystematicDifference
        {
            get { return this._outputData[39]; }
            set { this._outputData[39] = (ushort)value; }
        }
        public override int DownwardsDosing
        {
            get { return this._outputData[40]; }
            set { this._outputData[40] = (ushort)value; }
        }
        public override int ValveControl
        {
            get { return this._outputData[41]; }
            set { this._outputData[41] = (ushort)value; }
        }
        public override int EmptyingMode
        {
            get { return this._outputData[42]; }
            set { this._outputData[42] = (ushort)value; }
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
            if (this.WeightMoving == 0)
                return "0=Weight is not moving.";
            else
                return "1=Weight is moving";
        }


        public string LimitStatusStringComment()
        {
            switch (this.LimitStatus)
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


        private void limitStatusBool()
        {
            switch (this.LimitStatus)
            {
                case 0: // Weight within limits
                    _processData.Underload = false;
                    _processData.Overload = false;
                    _processData.weightWithinLimits = true;
                    _processData.higherSafeLoadLimit = false;
                    break;
                case 1: // Lower than minimum
                    _processData.Underload = true;
                    _processData.Overload = false;
                    _processData.weightWithinLimits = false;
                    _processData.higherSafeLoadLimit = false;
                    break;
                case 2: // Higher than maximum capacity
                    _processData.Underload = false;
                    _processData.Overload = true;
                    _processData.weightWithinLimits = false;
                    _processData.higherSafeLoadLimit = false;
                    break;
                case 3: // Higher than safe load limit
                    _processData.Underload = false;
                    _processData.Overload = false;
                    _processData.weightWithinLimits = false;
                    _processData.higherSafeLoadLimit = true;
                    break;
                default: // Lower than minimum
                    _processData.Underload = true;
                    _processData.Overload = false;
                    _processData.weightWithinLimits = false;
                    _processData.higherSafeLoadLimit = false;
                    break;
            }
        }

        public string WeightTypeStringComment()
        {
            if (this.WeightType == 0)
            {
                //this._isNet = false;
                return "gross";
            }
            else // if (this.WeightType == 1)
            {
                //this._isNet = true;
                return "net";
            }
        }
        public string ScaleRangeStringComment()
        {
            switch (this.ScaleRange)
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
        public string ApplicationModeStringComment()
        {
            if (this.ApplicationMode == 0)
                return "Standard";
            else

                if (this.ApplicationMode == 2 || this.ApplicationMode == 1)  // Will be changed to '2', so far '1'. 
                return "Filler";
            else

                return "error";
        }
        public override string UnitStringComment()
        {
            switch (this.Unit)
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
        public string StatusStringComment()
        {
            if (this.Status == 1)
                return "Execution OK!";
            else
                if (this.Status != 1)
                return "Execution not OK!";
            else
                return "error.";

        }
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
            if ((this.NetValue != calibrationValue || this.GrossValue != calibrationValue))
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