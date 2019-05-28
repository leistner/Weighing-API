// <copyright file="WTXJet.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    using System.Threading;
    using Hbm.Weighing.API.Data;
    using Hbm.Weighing.API.Utils;
    using Hbm.Weighing.API.WTX.Jet;

    /// <summary>
    /// This class represents a WTX device.
    /// Subscribe to ProcessDataReceived over the constructor to automatically get weight values.
    /// </summary>
    public class WTXJet : BaseWTDevice
    {
        #region ==================== constants & fields ====================
        private const int CONVERISION_FACTOR_MVV_TO_D = 500000;  
        private const int SCALE_COMMAND_CALIBRATE_ZERO = 2053923171;
        private const int SCALE_COMMAND_CALIBRATE_NOMINAL = 1852596579;
        private const int SCALE_COMMAND_EXIT_CALIBRATE = 1953069157;
        private const int SCALE_COMMAND_TARE = 1701994868;
        private const int SCALE_COMMAND_CLEAR_PEAK_VALUES = 1801545072;
        private const int SCALE_COMMAND_ZERO = 1869768058;
        private const int SCALE_COMMAND_SET_GROSS = 1936683623;
        private const int SCALE_COMMAND_STATUS_ONGOING = 1634168417;
        private const int SCALE_COMMAND_STATUS_OK = 1801543519;
        private const int SCALE_COMMAND_STATUS_ERROR_E1 = 826629983;
        private const int SCALE_COMMAND_STATUS_ERROR_E2 = 843407199;
        private const int SCALE_COMMAND_STATUS_ERROR_E3 = 860184415;

        private int _manualTareValue;
        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nominalLoad;
        #endregion
        
        #region ==================== events & delegates ====================
        /// <summary>
        /// Event handler to raise an event whenever new process data from the device is available
        /// </summary>
        public event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;

        /// <summary>
        /// Asynchronous callback for process data
        /// </summary>
        /// <param name="sender">ProcessData sender</param>
        /// <param name="e">DataEventArgs containing data dictionary</param>
        public void OnData(object sender, DataEventArgs e)
        {
            ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
        }
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="WTXJet" /> class.
        /// </summary>
        /// <param name="connection">Inject connection (e.g. JetBusConnection)</param>
        /// <param name="onProcessData">This event is automatically called when new ProcessData is available</param>
        public WTXJet(INetConnection connection, int timerIntervalms, EventHandler<ProcessDataReceivedEventArgs> onProcessData) 
            : base(connection, timerIntervalms)
        {          
            ProcessData = new ProcessDataJet(Connection);
            DataStandard = new DataStandardJet(Connection);
            DataFiller = new DataFillerExtendedJet(Connection);
            ProcessDataReceived += onProcessData;
        }
        #endregion

        #region ======================== Properties ========================
        /// <summary>
        /// Gets or sets the extended filler data 
        /// </summary>
        public IDataFillerExtended DataFillerExtended { get; protected set; }
        
        /// <summary>
        /// Gets the the connection type
        /// </summary>
        public override string ConnectionType
        {
            get
            {
                return "Jetbus";
            }
        }

        public override bool IsConnected
        {
            get
            {
                return Connection.IsConnected;
            }
        }
        
        /// <summary>
        /// Gets the weight type containing gross, net and tare value in double
        /// </summary>
        public override WeightType Weight
        {
            get
            {
                return ProcessData.Weight;
            }
        }

        /// <summary>
        /// Gets the printable weight type  containing gross, net and tare value in string
        /// </summary>
        public override PrintableWeightType PrintableWeight
        {
            get
            {
                return ProcessData.PrintableWeight;
            }
        }

        /// <summary>
        /// Gets the application mode containing an enumeration (Standard, Checkweigher, Filler)
        /// </summary>
        public override ApplicationMode ApplicationMode { get; set; }

        /// <summary>
        /// Gets the unit of the measured value as string
        /// </summary>
        public override string Unit
        {
            get
            {
                return ProcessData.Unit;
            }
        }

        /// <summary>
        /// Gets the tare mode containing None, Tare, PresetTare
        /// </summary>
        public override TareMode TareMode
        {
            get
            {
                return ProcessData.TareMode;
            }
        }

        /// <summary>
        /// Gets the scale range of the device
        /// </summary>
        public override int ScaleRange
        {
            get
            {
                return ProcessData.ScaleRange + 1;
            }
        }

        /// <summary>
        /// Gets the manual tare value
        /// </summary>
        public override int ManualTareValue
        {
            get
            {
                return _manualTareValue;
            }

            set
            {
                Connection.Write(JetBusCommands.TareValue, value);
                _manualTareValue = value;
            }
        }

        /// <summary>
        /// Gets or sets the calibration weight value
        /// </summary>
        public override int CalibrationWeight
        {
            get
            {
                return _calibrationWeight;
            }

            set
            {
                Connection.Write(JetBusCommands.CalibrationWeight, value);
                _calibrationWeight = value;
            }
        }

        /// <summary>
        /// Gets or sets the zero value
        /// </summary>
        public override int ZeroSignal
        {
            get
            {
                return _zeroLoad;
            }

            set
            {
                Connection.Write(JetBusCommands.ZeroValue, value);
                _zeroLoad = value;
            }
        }

        /// <summary>
        /// Gets or set the nominal value
        /// </summary>
        public override int NominalSignal
        {
            get
            {
                return _nominalLoad;
            }

            set
            {
                Connection.Write(JetBusCommands.LWTNominalValue, value);
                _nominalLoad = value;
            }
        }
        #endregion

        #region ================ public & internal methods ================= 
        /// <inheritdoc />
        public override void Connect(double timeoutMs = 20000)
        {
            Connection.Connect();
            _processDataTimer.Change(0, ProcessDataInterval);
        }
        
        /// <inheritdoc />
        public override void Connect(Action<bool> completed, double timeoutMs)
        {
            Connection.Connect();
            _processDataTimer.Change(0, ProcessDataInterval);
        }

        /// <inheritdoc />
        public override void Disconnect(Action<bool> disconnectCompleted)
        {
            Connection.Disconnect();
        }

        /// <inheritdoc />
        public override void Disconnect()
        {
            Connection.Disconnect();
        }
        
        /// <inheritdoc />
        public override void Zero()
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_ZERO);
        }
        
        /// <inheritdoc />
        public override void SetGross()
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_SET_GROSS);
        }
        
        /// <inheritdoc />
        public override void Tare()
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_TARE);
        }
        
        /// <inheritdoc />
        public override void TareManually(double manualTareValue)
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_TARE);
        }
        
        /// <inheritdoc />
        public override void RecordWeight()
        {
            Connection.Write(JetBusCommands.RecordWeight, SCALE_COMMAND_TARE);
        }
                
        /// <inheritdoc />
        public override void CalculateAdjustment(double scaleZeroLoad_mVV, double scaleCapacity_mVV)
        {
            int scalZeroLoad_d;
            int scaleCapacity_d; 

            scalZeroLoad_d = (int)(scaleZeroLoad_mVV * CONVERISION_FACTOR_MVV_TO_D);
            scaleCapacity_d = (int)(scalZeroLoad_d + (scaleCapacity_mVV * CONVERISION_FACTOR_MVV_TO_D));
                        
            Connection.Write(JetBusCommands.LDWZeroValue, scalZeroLoad_d); 
            Connection.Write(JetBusCommands.LWTNominalValue, Convert.ToInt32(scaleCapacity_d)); 
        }

        /// <inheritdoc />
        public override bool AdjustZeroSignal()
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_CALIBRATE_ZERO); 

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
            {
                return true;
            }
            
            return false;
        }
        
        /// <inheritdoc />
        public override bool AdjustNominalSignal()
        {
            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_CALIBRATE_NOMINAL);

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
            {
                return true;
            }

            return false;
        }
        
        /// <inheritdoc />
        public override bool AdjustNominalSignalWithCalibrationWeight(double calibrationWeight)
        {
            Connection.Write(JetBusCommands.CalibrationWeight, MeasurementUtils.DoubleToDigit(calibrationWeight, ProcessData.Decimals));

            Connection.Write(JetBusCommands.ScaleCommand, SCALE_COMMAND_CALIBRATE_NOMINAL);

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
            { 
                return true;
            }

            return false;
        }
        
        protected override void ProcessDataUpdateTick(object info)
        {
            Stop();
            if (IsConnected)
            {
                ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
            }

            Restart();
        }
        #endregion
    }
}