// <copyright file="WTXModbus.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.API, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.API.WTX
{
    using System;
    using Hbm.Weighing.API.Data;
    using Hbm.Weighing.API.Utils;
    using Hbm.Weighing.API.WTX.Modbus;

    /// <summary>
    /// This class handles the data from ModbusTcpConnection for IProcessData. 
    /// WtxModbus fetches, interprets the data( method OnData(data) ) and 
    /// send it to the GUI or application class by an eventhandler (=ProcessDataReceived). 
    /// </summary>
    ///
    public class WTXModbus : BaseWTDevice
    {
        #region ==================== constants & fields ====================        
        private int _manualTareValue;
        #endregion

        #region ==================== events & delegates ====================
        /// <summary>
        /// Event handler to raise an event whenever new process data from the device is available
        /// </summary>
        public event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="WTXJet" /> class.
        /// </summary>
        /// <param name="connection">Inject connection (e.g. JetBusConnection)</param>
        /// <param name="onProcessData">This event is automatically called when new ProcessData is available</param>
        public WTXModbus(INetConnection connection, int timerIntervalms, EventHandler<ProcessDataReceivedEventArgs> onProcessData)
            : base(connection, timerIntervalms)
        {
            ProcessData = new ProcessDataModbus(base.Connection);
            DataStandard = new DataStandardModbus(base.Connection);
            DataFiller = new DataFillerModbus(base.Connection);
            ProcessDataReceived += onProcessData;
        }

        /// <summary>
        /// For a more simple solution : Constructor with no asynchronous callback and no timer interval for continously updating the data. 
        /// </summary>
        /// <param name="connection"></param>
        public WTXModbus(INetConnection connection) : base(connection)
        {
            ProcessData = new ProcessDataModbus(base.Connection);
            DataStandard = new DataStandardModbus(base.Connection);
            DataFiller = new DataFillerModbus(base.Connection);
        }
        #endregion

        #region ======================== properties ========================
        /// <inheritdoc />
        public override bool IsConnected
        {
            get
            {
                return Connection.IsConnected;
            }
        }

        /// <inheritdoc />
        public override string ConnectionType
        {
            get
            {
                return "Modbus";
            }
        }
        
        #endregion


        #region ================ public & internal methods =================
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
        #endregion

        protected override void ProcessDataUpdateTick(object info)
        {
            Stop();
            if (IsConnected)
            {
                ((ModbusTCPConnection)Connection).SyncData();
                ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
            }

            Restart();
        }

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
            get
            {
                return _manualTareValue;
            }
            set
            {
                Connection.Write(ModbusCommands.ManualTareValue, value);
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
        public override int CalibrationWeight { get; set; }

        public override int ZeroSignal { get; set; }

        public override int NominalSignal { get; set; }


        // This methods sets the value of the WTX to zero. 
        public override bool AdjustZeroSignal()
        {
            Stop();                        
            Connection.Write(ModbusCommands.LDWZeroSignal, 0x7FFFFFFF);
            return Connection.Write(ModbusCommands.Control_word_AdjustZero, 1);
        }

        public override bool AdjustNominalSignal()
        {
            this.Stop();
            Connection.Write(ModbusCommands.LWTNominalSignal, 0x7FFFFFFF);
            return Connection.Write(ModbusCommands.Control_word_AdjustZero, 1);
        }

        // This method sets the value for the nominal weight in the WTX.
        public override bool AdjustNominalSignalWithCalibrationWeight(double adjustmentWeight)
        {
            Connection.Write(ModbusCommands.CWTScaleCalibrationWeight, MeasurementUtils.DoubleToDigit(adjustmentWeight,ProcessData.Decimals));    
            Connection.Write(ModbusCommands.LWTNominalSignal, 0x7FFFFFFF);          
            bool result = Connection.Write(ModbusCommands.Control_word_AdjustNominal, 1);
            this.Restart();
            return result;
        }
        
        // Calculates the values for deadload and nominal load in d from the inputs in mV/V
        // and writes the into the WTX registers.
        public override void CalculateAdjustment(double preload, double capacity)
        {
            double multiplierMv2D = 500000.0;
            int _preLoad = (int)(preload * multiplierMv2D);
            int _nominalLoad = (int)(preload + (capacity * multiplierMv2D));
                                       
            Stop();       
            Connection.Write(ModbusCommands.LDWZeroSignal, _preLoad);
            Connection.Write(ModbusCommands.Control_word_AdjustZero, 1);  
            Connection.Write(ModbusCommands.LWTNominalSignal, _nominalLoad);
            Connection.Write(ModbusCommands.Control_word_AdjustNominal, 1);
            Restart();
        }
        #endregion

        #region Process data methods - Standard
        public async override void SetGross()
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_GrossNet, 1);
        }
        
        public async override void Tare()
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_Taring, 1);
        }
        
        public async override void Zero()
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_Zeroing, 1);
        }
        
        public async void ActivateData()
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_ActivateData, 1);
        }

        public async override void TareManually(double manualTareValue)
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_Taring, 1);
        }
        #endregion

        public async override void RecordWeight()
        {
            await Connection.WriteAsync(ModbusCommands.Control_word_RecordWeight, 1);
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