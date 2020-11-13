// <copyright file="DSEJet.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.Api, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.Api.DSE
{
    using System;
    using System.Threading;
    using Hbm.Weighing.Api.Data;
    using Hbm.Weighing.Api.Utils;
    using Hbm.Weighing.Api.WTX.Jet;
    using Hbm.Weighing.Api;

    /// <summary>
    /// This class represents a DSE device with the Jet ethernet interface.
    /// Subscribe to ProcessDataReceived over the constructor to automatically get weight values.
    /// </summary>
    public class DSEJet : BaseWTDevice, IDataScale, IDataDigitalFilter
    {
        #region ==================== constants & fields ====================
        private const int CONVERISION_FACTOR_MVV_TO_D = 1000000;  
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
        private const int COMMAND_FACTORY_DEFAULTS = 1684107116;
        private INetConnection _connection;
        #endregion
        
        #region ==================== events & delegates ====================
        /// <summary>
        /// Event handler to raise whenever new process data from the device is available
        /// </summary>
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived; 
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="DSEJet" /> class.
        /// </summary>
        /// <param name="connection">Inject connection (e.g. JetBusConnection)</param>
        /// <param name="timerIntervalms">Interval for updating ProcessData</param>
        /// <param name="onProcessData">This event is automatically called when new ProcessData is available</param>
        public DSEJet(INetConnection connection, int timerIntervalms, EventHandler<ProcessDataReceivedEventArgs> onProcessData) 
            : base(connection, timerIntervalms)
        {
            _connection = connection;
            ProcessData = new JetProcessData(Connection);
            ProcessDataReceived += onProcessData;
        }
        #endregion

        #region ======================== Properties ========================

        ///<inheritdoc/>
        public override string ConnectionType
        {
            get
            {
                return "Jetbus";
            }
        }

        ///<inheritdoc/>
        public override bool IsConnected
        {
            get
            {
                return Connection.IsConnected;
            }
        }

        ///<inheritdoc/>
        public override WeightType Weight
        {
            get
            {
                return ProcessData.Weight;
            }
        }

        ///<inheritdoc/>
        public override PrintableWeightType PrintableWeight
        {
            get
            {
                return ProcessData.PrintableWeight;
            }
        }

        ///<inheritdoc/>
        public override ApplicationMode ApplicationMode { get; set; }

        ///<inheritdoc/>
        public override string Unit
        {
            get
            {
                return ProcessData.Unit;
            }

            set
            {
                int _unit=0;
                switch (value)
                {
                    case "kg": _unit = 0x00020000; break;
                    case "g": _unit = 0x004B0000; break;
                    case "lb": _unit = 0x00A60000; break;
                    case "t": _unit = 0x004C0000; break;
                    case "N": _unit = 0x00210000; break;
                    default: throw new NotSupportedException();
                }
                Connection.WriteInteger(JetBusCommands.CIA461Unit, _unit);
            }
        }

        ///<inheritdoc/>
        public override TareMode TareMode
        {
            get
            {
                return ProcessData.TareMode;
            }
        }

        ///<inheritdoc/>
        public override bool GeneralScaleError
        {
            get
            {
                return Connection.ReadIntegerFromBuffer(JetBusCommands.CIA461WeightStatusGeneralWeightError) != 0;
            }
        }

        ///<inheritdoc/>
        public override bool WeightStable
        {
            get
            {
                return ProcessData.WeightStable;
            }
        }

        ///<inheritdoc/>
        public override int ScaleRange
        {
            get
            {
                return ProcessData.ScaleRange + 1;
            }
        }

        ///<inheritdoc/>
        public override double ManualTareValue
        {
            get
            {
                int readValue = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461TareValue);
                return MeasurementUtils.DigitToDouble(readValue, ProcessData.Decimals);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461TareValue, MeasurementUtils.DoubleToDigit(value, ProcessData.Decimals));
            }
        }

        ///<inheritdoc/>
        public override int MaximumCapacity
        {
            get
            {
                return _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461ScaleMaximumCapacity);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461ScaleMaximumCapacity, value);
            }
        }

        ///<inheritdoc/>
        public override double CalibrationWeight
        {
            get
            {
                int readValue = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461CalibrationWeight);
                return MeasurementUtils.DigitToDouble(readValue, ProcessData.Decimals);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461CalibrationWeight, MeasurementUtils.DoubleToDigit(value, ProcessData.Decimals));
            }
        }

        ///<inheritdoc/>
        public override double ZeroValue
        {
            get
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                return MeasurementUtils.DigitToDouble(_connection.ReadIntegerFromBuffer(JetBusCommands.DSEZeroValue), decimals);
            }
        }

        ///<inheritdoc/>
        public override int ZeroSignal
        {
            get
            {
                return _connection.ReadIntegerFromBuffer(JetBusCommands.SDLScaleZeroSignal);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.SDLScaleZeroSignal, value);
            }
        }

        ///<inheritdoc/>
        public override int NominalSignal
        {
            get
            {
                return _connection.ReadIntegerFromBuffer(JetBusCommands.SNLScaleNominalSignal);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.SNLScaleNominalSignal, value);
            }
        }


        public override string SerialNumber 
        {
            get
            {
                return Connection.ReadFromDevice(JetBusCommands.DSESerialNumber);
            }
        }

        ///<inheritdoc/>
        public override string Identification
        { 
            get
            {
                return Connection.ReadFromDevice(JetBusCommands.DSEIDNDeviceIdentification);
            }

            set
            {
                Connection.Write(JetBusCommands.DSEIDNDeviceIdentification, value);
            }
        }


        ///<inheritdoc/>
        public override string FirmwareVersion
        {
            get
            {
                return Connection.ReadFromDevice(JetBusCommands.DSEFirmwareVersion);
            }
        }

       ///<inheritdoc/>
        public double WeightStep
        {
            get
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                return MeasurementUtils.DigitToDouble(_connection.ReadIntegerFromBuffer(JetBusCommands.CIA461WeightStep), decimals);
            }

            set
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                Connection.WriteInteger(JetBusCommands.CIA461WeightStep, MeasurementUtils.DoubleToDigit(value, decimals));
            }
        }

        ///<inheritdoc/>
        public int WeightMovementDetection
        {
            get
            {
                return _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461WeightMovingDetection);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461WeightMovingDetection, value);
            }
        }

        ///<inheritdoc/>
        public ScaleRangeMode ScaleRangeMode
        {
            get
            {
                ScaleRangeMode srm = ScaleRangeMode.None;
                switch (_connection.ReadIntegerFromBuffer(JetBusCommands.CIA461MultiLimit1) )
                {
                    case 0: srm = ScaleRangeMode.None; break;
                    case 1: srm = ScaleRangeMode.MultiInterval; break;
                    case 2: srm = ScaleRangeMode.MultiRange; break;
                }
                return srm;
            }

            set
            {
                int setValue = 0;
                switch (value)
                {
                    case ScaleRangeMode.None: setValue = 0; break;
                    case ScaleRangeMode.MultiInterval: setValue = 1; break;
                    case ScaleRangeMode.MultiRange: setValue =  2; break;
                }
                Connection.WriteInteger(JetBusCommands.CIA461MultiLimit1, setValue);
            }
        }

        ///<inheritdoc/>
        public double MultiScaleLimit1
        { 
            get
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                return MeasurementUtils.DigitToDouble(_connection.ReadIntegerFromBuffer(JetBusCommands.CIA461MultiLimit1), decimals);
            }

            set
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                Connection.WriteInteger(JetBusCommands.CIA461MultiLimit1, MeasurementUtils.DoubleToDigit(value, decimals));
            }
        }

        ///<inheritdoc/>
        public double MultiScaleLimit2
        {
            get
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                return MeasurementUtils.DigitToDouble(_connection.ReadIntegerFromBuffer(JetBusCommands.CIA461MultiLimit2), decimals);
            }

            set
            {
                int decimals = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461Decimals);
                Connection.WriteInteger(JetBusCommands.CIA461MultiLimit2, MeasurementUtils.DoubleToDigit(value, decimals));
            }
        }


        /// <summary>
        /// Gets the data rate in Hz (DSE is 2000Hz fix)
        /// </summary>
        public int DataRate
        {
            get
            {
                return 2000;
            }

            set
            {
                
            }
        }

        ///<inheritdoc/>
        public int LowPassFilterMode
        {
            get
            {
                return _connection.ReadIntegerFromBuffer(JetBusCommands.DSEFilterSetup);
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461ScaleFilter, value);
            }
        }

        ///<inheritdoc/>
        public int LowPasCutOffFrequency
        {
            get
            {
                int __value;
                switch (LowPassFilterMode)
                {
                    case 0x60A1: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterCriticallyDampedCutOffFrequency); break;
                    case 0x60A2: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterBesselCutOffFrequency); break;
                    default:
                    case 0x60B1: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterButterworthCutOffFrequency); break;
                }
                return __value;
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461FilterCriticallyDampedCutOffFrequency, value);
                Connection.WriteInteger(JetBusCommands.CIA461FilterBesselCutOffFrequency, value);
                Connection.WriteInteger(JetBusCommands.CIA461FilterButterworthCutOffFrequency, value);
            }
        }

        ///<inheritdoc/>
        public int LowPassFilterOrder
        {
            get
            {
                int __value;
                switch (LowPassFilterMode)
                {
                    case 0x60A1: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterCriticallyDampedFilterOrder); break;
                    case 0x60A2: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterBesselFilterOrder); break;
                    default:
                    case 0x60B1: __value = _connection.ReadIntegerFromBuffer(JetBusCommands.CIA461FilterButterworthFilterOrder); break;
                }
                return __value;
            }

            set
            {
                Connection.WriteInteger(JetBusCommands.CIA461FilterCriticallyDampedFilterOrder, value);
                Connection.WriteInteger(JetBusCommands.CIA461FilterBesselFilterOrder, value);
                Connection.WriteInteger(JetBusCommands.CIA461FilterButterworthFilterOrder, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum zeroing time (DSE device)
        /// </summary>
        public int MaximumZeroingTime { get; set; }

        /// <summary>
        /// Gets or sets the current time (DSE device)
        /// </summary>
        public int DateTime { get; set; }

        #endregion

        #region ================ public & internal methods ================= 
        ///<inheritdoc/>
        public override void Connect(double timeoutMs = 20000)
        {
            Connection.Connect();
            ProcessDataTimer.Change(0, ProcessDataInterval);
        }
        
        ///<inheritdoc/>
        public override void Connect(Action<bool> completed, double timeoutMs)
        {
            Connection.Connect();
            ProcessDataTimer.Change(0, ProcessDataInterval);
        }

        ///<inheritdoc/>
        public override void Disconnect(Action<bool> disconnectCompleted)
        {
            Connection.Disconnect();
        }

        ///<inheritdoc/>
        public override void Disconnect()
        {
            Connection.Disconnect();
        }

        ///<inheritdoc/>
        public override void SaveAllParameters()
        {
            Connection.WriteInteger(JetBusCommands.CIA461SaveAllParameters, 0);
        }

        ///<inheritdoc/>
        public override void RestoreAllDefaultParameters()
        {
            Connection.WriteInteger(JetBusCommands.DSERestoreAllDefaultParameters, COMMAND_FACTORY_DEFAULTS);       //command missing int.Parse("6c6f6164", System.Globalization.NumberStyles.HexNumber)
        }

        ///<inheritdoc/>
        public override void Zero()
        {
            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_ZERO);
        }
        
        ///<inheritdoc/>
        public override void SetGross()
        {
            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_SET_GROSS);
        }
        
        ///<inheritdoc/>
        public override void Tare()
        {
            if (Connection.IsConnected)
                Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_TARE);
        }
        
        ///<inheritdoc/>
        public override void TareManually(double manualTareValue)
        {
            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_TARE);
        }
        
        ///<inheritdoc/>
        public override void RecordWeight()
        {
            Connection.WriteInteger(JetBusCommands.STORecordWeight, SCALE_COMMAND_TARE);
        }
                
        ///<inheritdoc/>
        public override void CalculateAdjustment(double scaleZeroLoad_mVV, double scaleCapacity_mVV)
        {
            int scalZeroLoad_d;
            int scaleCapacity_d; 

            scalZeroLoad_d = (int)(scaleZeroLoad_mVV * CONVERISION_FACTOR_MVV_TO_D);
            scaleCapacity_d = (int)(scalZeroLoad_d + (scaleCapacity_mVV * CONVERISION_FACTOR_MVV_TO_D));
                        
            Connection.WriteInteger(JetBusCommands.LDWZeroValue, scalZeroLoad_d); 
            Connection.WriteInteger(JetBusCommands.LWTNominalValue, Convert.ToInt32(scaleCapacity_d)); 
        }

        ///<inheritdoc/>
        public override bool AdjustZeroSignal()
        {
            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_CALIBRATE_ZERO); 
            
            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }     
            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }
            
            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_OK)
            {
                return true;
            }
            
            return false;
        }
        
        ///<inheritdoc/>
        public override bool AdjustNominalSignal()
        {
            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_CALIBRATE_NOMINAL);

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_OK)
            {
                return true;
            }

            return false;
        }
        
        ///<inheritdoc/>
        public override bool AdjustNominalSignalWithCalibrationWeight(double calibrationWeight)
        {
            Connection.WriteInteger(JetBusCommands.CIA461CalibrationWeight, MeasurementUtils.DoubleToDigit(calibrationWeight, ProcessData.Decimals));

            Connection.WriteInteger(JetBusCommands.CIA461ScaleCommand, SCALE_COMMAND_CALIBRATE_NOMINAL);
           
            
            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }
            
            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.CIA461ScaleCommandStatus)) == SCALE_COMMAND_STATUS_OK)
            { 
                return true;
            }

            return false;
        }

        ///<inheritdoc/>
        protected override void ProcessDataUpdateTick(object info)
        {
            if (IsConnected)
            {
                ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
            }
        }

        #endregion
    }
}