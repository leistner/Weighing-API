// <copyright file="WTXJet.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using System.Threading;
using HBM.Weighing.API.WTX.Jet;

namespace HBM.Weighing.API.WTX
{
    public class WtxJet : BaseWtDevice
    {
        #region Const
        private const int CONVERISION_FACTOR_MVV_TO_D = 500000; //   2 / 1000000; // 2mV/V correspond 1 million digits (d)   

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
        #endregion

        #region Privates 
        private ProcessData _processData;
        #endregion

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region Constructors
        public WtxJet(INetConnection Connection, EventHandler<ProcessDataReceivedEventArgs> OnProcessData) : base(Connection)  // ParameterProperty umändern 
        {
            _processData = new ProcessData();

            _connection = Connection;
            
            this.ProcessDataReceived += OnProcessData;

            _connection.IncomingDataReceived += this.OnData;   // Subscribe to the event.                          
        }
        #endregion

        #region Connection
        public override void Disconnect(Action<bool> DisconnectCompleted)
        {
            _connection.Disconnect();
        }


        public override void Disconnect()
        {
            _connection.Disconnect();
        }


        public override bool isConnected
        {
            get
            {
                return _connection.IsConnected;
            }
        }

        public override void Connect(double timeoutMs = 20000)
        {
            _connection.Connect();

            //this.UpdateEvent(this, null);
        }

        public override void Connect(Action<bool> completed, double timeoutMs)
        {
            _connection.Connect();
        }
        #endregion

        #region Asynchronous process data callback
        public void OnData(object sender, ProcessDataReceivedEventArgs e)
        {
            // Update data for standard mode:
            this.UpdateStandardData();

            // Update process data : 
            this.UpdateProcessData();

            // Update data for filler mode:
            this.UpdateFillerData();

            // Update data for filler extended mode:
            //this.UpdateFillerExtendedData();

            this.limitStatusBool();                                      // update the booleans 'Underload', 'Overload', 'weightWithinLimits', 'higherSafeLoadLimit'. 
            _processData.LegalTradeOp = this.LegalTradeOp;

            // Do something with the data, like in the class WTXModbus.cs           
            this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(_processData));
        }

        private void UpdateProcessData()
        {
            ProcessData.NetValue = this.NetValue;
            ProcessData.GrossValue = this.GrossValue;
            ProcessData.NetValueStr = this.CurrentWeight(this.NetValue, this.Decimals);
            ProcessData.GrossValueStr = this.CurrentWeight(this.GrossValue, this.Decimals);

            ProcessData.TareValue = this.NetValue - this.GrossValue;
            ProcessData.GeneralWeightError = Convert.ToBoolean(this.GeneralWeightError);
            ProcessData.ScaleAlarmTriggered = Convert.ToBoolean(this.ScaleAlarmTriggered);
            ProcessData.LimitStatus = this.LimitStatus;
            ProcessData.WeightMoving = Convert.ToBoolean(this.WeightMoving);
            ProcessData.ScaleSealIsOpen = Convert.ToBoolean(this.ScaleSealIsOpen);
            ProcessData.ManualTare = Convert.ToBoolean(this.ManualTare);
            ProcessData.WeightType = Convert.ToBoolean(this.WeightType);
            ProcessData.ScaleRange = this.ScaleRange;
            ProcessData.ZeroRequired = Convert.ToBoolean(this.ZeroRequired);
            ProcessData.WeightWithinTheCenterOfZero = Convert.ToBoolean(this.WeightWithinTheCenterOfZero);
            ProcessData.WeightInZeroRange = Convert.ToBoolean(this.WeightInZeroRange);
            ProcessData.ApplicationMode = this.ApplicationMode;
            ProcessData.ApplicationModeStr = ApplicationModeStringComment();
            ProcessData.Decimals = this.Decimals;
            ProcessData.Unit = this.Unit;
            ProcessData.Handshake = Convert.ToBoolean(this.Handshake);
            ProcessData.Status = Convert.ToBoolean(this.Status);
        }

        private void UpdateStandardData()
        {
            this.NetValue = _connection.AllData[JetBusCommands.NET_VALUE];
            this.GrossValue = _connection.AllData[JetBusCommands.GROSS_VALUE];
            this.Decimals = _connection.AllData[JetBusCommands.DECIMALS];
            this.ManualTareValue = 0;
            this.GeneralWeightError = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1));
            this.ScaleAlarmTriggered = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x2) >> 1);
            this.LimitStatus = (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0xC) >> 2;
            this.WeightMoving = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x10) >> 4);
            this.ScaleSealIsOpen = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x20) >> 5);
            this.ManualTare = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x40) >> 6);
            this.WeightType = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x80) >> 7);
            this.ScaleRange = (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x300) >> 8;
            this.ZeroRequired = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x400) >> 10);
            this.WeightWithinTheCenterOfZero = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x800) >> 11);
            this.WeightInZeroRange = Convert.ToBoolean((_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1000) >> 12);
            this.Unit = (_connection.AllData[JetBusCommands.UNIT_PREFIX_FIXED_PARAMETER] & 0xFF0000) >> 16;
            this.Handshake = UpdateHandshake();
            this.Status = Convert.ToBoolean(_connection.AllData[JetBusCommands.SCALE_COMMAND_STATUS]);

            // Commented out because of undefined ID's:
            /*
            this.LimitStatus1 = _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_1];
            this.LimitStatus2 = _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_2];
            this.LimitStatus3 = _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_3];
            this.LimitStatus4 = _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_4];
            
            this.FillingProcessStatus  = _connection.AllData[JetBusCommands.DOSING_STATUS];
            this.NumberDosingResults = _connection.AllData[JetBusCommands.DOSING_COUNTER];
            this.DosingResult = _connection.AllData[JetBusCommands.DOSING_RESULT];
            
            this.Input1 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_1];
            this.Input2 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_2];
            this.Input3 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_3];
            this.Input4 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_4];
            
            this.Output1 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_1];
            this.Output2 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_2];
            this.Output3 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_3];
            this.Output4 = _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_4];
            */

        }

        private void UpdateFillerData()
        {
            this.AdcOverUnderload = 0;   // undefined ID
            this.LegalTradeOp = 0;       // undefined ID
            this.StatusInput1 = 0;       // undefined ID
            this.GeneralScaleError = 0;  // undefined ID
            this.CoarseFlow = 0;         // undefined ID
            this.FineFlow = 0;           // undefined ID
            this.Ready = 0;              // undefined ID
            this.ReDosing = 0;           // undefined ID
            this.Emptying = 0;           // undefined ID
            this.FlowError = 0;          // undefined ID
            this.Alarm = 0;              // undefined ID
            this.ToleranceErrorPlus = 0; // undefined ID
            this.ToleranceErrorMinus = 0;// undefined ID
            this.CurrentDosingTime = 0;  // undefined ID
            this.CurrentCoarseFlowTime = 0; // undefined ID
            this.CurrentFineFlowTime = 0;   // undefined ID

            this.ParameterSetProduct = 0;    // undefined ID 
            this.DownwardsDosing = 0;        // undefined ID
            this.LegalForTradeOperation = 0; // undefined ID

            // Commented out because of undefined ID's:
            /*
            this.MaxDosingTime = _connection.AllData[JetBusCommands.MAXIMUM_DOSING_TIME];
            this.MeanValueDosingResults = _connection.AllData[JetBusCommands.MEAN_VALUE_DOSING_RESULTS];
            this.StandardDeviation = _connection.AllData[JetBusCommands.STANDARD_DEVIATION];
            this.FineFlowCutOffPoint = _connection.AllData[JetBusCommands.FINE_FLOW_CUT_OFF_POINT];
            this.CoarseFlowCutOffPoint = _connection.AllData[JetBusCommands.COARSE_FLOW_CUT_OFF_POINT];
            this.ResidualFlowTime = _connection.AllData[JetBusCommands.RESIDUAL_FLOW_TIME];
            this.MinimumFineFlow = _connection.AllData[JetBusCommands.MINIMUM_FINE_FLOW];
            this.OptimizationOfCutOffPoints = _connection.AllData[JetBusCommands.OPTIMIZATION];
            this.MaximumDosingTime = _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_3];
            this.CoarseLockoutTime = _connection.AllData[JetBusCommands.COARSE_FLOW_TIME];
            this.FineLockoutTime = _connection.AllData[JetBusCommands.FINE_FLOW_TIME];
            this.TareMode = _connection.AllData[JetBusCommands.TARE_MODE];
            this.UpperToleranceLimit = _connection.AllData[JetBusCommands.UPPER_TOLERANCE_LIMIT];
            this.LowerToleranceLimit = _connection.AllData[JetBusCommands.LOWER_TOLERANCE_LOMIT];
            this.MinimumStartWeight = _connection.AllData[JetBusCommands.MINIMUM_START_WEIGHT];
            this.EmptyWeight = _connection.AllData[JetBusCommands.EMPTY_WEIGHT];
            this.TareDelay = _connection.AllData[JetBusCommands.TARE_DELAY];
            this.CoarseFlowMonitoringTime = _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING_TIME];
            this.CoarseFlowMonitoring = _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING];
            this.FineFlowMonitoring = _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING];
            this.FineFlowMonitoringTime = _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING_TIME];
            this.SystematicDifference = _connection.AllData[JetBusCommands.SYSTEMATIC_DIFFERENCE];
            this.ValveControl = _connection.AllData[JetBusCommands.VALVE_CONTROL];
            this.EmptyingMode = _connection.AllData[JetBusCommands.EMPTYING_MODE];
            this.DelayTimeAfterFineFlow = _connection.AllData[JetBusCommands.DELAY1_DOSING];
            this.ActivationTimeAfterFineFlow = _connection.AllData[JetBusCommands.FINEFLOW_PHASE_BEFORE_COARSEFLOW];
            */

            this.TotalWeight = 0;             // undefined ID
            this.TargetFillingWeight = 0;     // undefined ID
            this.CoarseFlowCutOffPointSet = 0;// undefined ID
            this.FineFlowCutOffPointSet = 0;  // undefined ID
            this.StartWithFineFlow = 0;       // undefined ID
        }

        public bool UpdateHandshake()
        {
            if (_connection.AllData[JetBusCommands.SCALE_COMMAND_STATUS] == 1801543519)
                return true;
            else
                return false;
        }

        public override void OnData(ushort[] _asyncData)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Identification
        public override string ConnectionType
        {
            get
            {
                return "Jetbus";
            }
        }
        #endregion

        #region Process data methods

        public override void Zero()
        {
            _connection.Write(JetBusCommands.SCALE_COMMAND, SCALE_COMMAND_ZERO);
        }

        public override void SetGross()
        {
            _connection.Write(JetBusCommands.SCALE_COMMAND, SCALE_COMMAND_SET_GROSS);
        }

        public override void Tare()
        {
            _connection.Write(JetBusCommands.SCALE_COMMAND, SCALE_COMMAND_TARE);
        }


        public override void activateData()
        {
        }

        public override void manualTaring()
        {
        }

        public override void recordWeight()
        {
        }

        #endregion

        #region Process data

        /* 
        *In the following methods the different options for the single integer values are used to define and
        *interpret the value. Finally a string should be returned from the methods to write it onto the GUI Form. 
        */
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


        public override string UnitStringComment()
        {
            switch (this.Unit)
            {
                case 0x02:
                    return "kg";
                case 0x4B:
                    return "g";
                case 0x4C:
                    return "t";
                case 0XA6:
                    return "lb";
                default:
                    return "error";
            }
        }

        public string StatusStringComment(int statusParam)
        {
            switch (statusParam)
            {
                case SCALE_COMMAND_STATUS_OK:
                    return "Execution OK!";

                case SCALE_COMMAND_STATUS_ONGOING:
                    return "Execution on go!";

                case SCALE_COMMAND_STATUS_ERROR_E1:
                    return "Error 1, E1";

                case SCALE_COMMAND_STATUS_ERROR_E2:
                    return "Error 2, E2";

                case SCALE_COMMAND_STATUS_ERROR_E3:
                    return "Error 3, E3";

                default:
                    return "Invalid status";
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

        #endregion

        #region Process data methods - Filling
        public override void clearDosingResults()
        {
            throw new NotImplementedException();
        }

        public override void abortDosing()
        {
            throw new NotImplementedException();
        }

        public override void startDosing()
        {
            throw new NotImplementedException();
        }

        public override void manualReDosing()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Adjustment methods

        /// <summary>
        /// Calculates the values for deadload and nominal load in d from the inputs in mV/V and writes the into the WTX registers.
        /// </summary>
        /// <param name="preload"></param>
        /// <param name="capacity"></param>
        public override void Calculate(double scaleZeroLoad_mVV, double scaleCapacity_mVV)
        {
            int scalZeroLoad_d;
            int scaleCapacity_d; 

            scalZeroLoad_d = (int) (scaleZeroLoad_mVV * CONVERISION_FACTOR_MVV_TO_D);
            scaleCapacity_d = (int)(scalZeroLoad_d + (scaleCapacity_mVV * CONVERISION_FACTOR_MVV_TO_D));


            // write path 2110/06 - dead load = LDW_DEAD_WEIGHT 

            _connection.Write(JetBusCommands.LDW_DEAD_WEIGHT, scalZeroLoad_d);         // Zero point = LDW_DEAD_WEIGHT= "2110/06"

            // write path 2110/07 - capacity/span = Nominal value = LWT_NOMINAL_VALUE        

            _connection.Write(JetBusCommands.LWT_NOMINAL_VALUE, Convert.ToInt32(scaleCapacity_d));    // Nominal value = LWT_NOMINAL_VALUE = "2110/07" ; 

            //this._isCalibrating = true;
        }


        public override void MeasureZero()
        {
            //write "calz" 0x7A6C6163 ( 2053923171 ) to path(ID)=6002/01

            _connection.Write(JetBusCommands.SCALE_COMMAND, SCALE_COMMAND_CALIBRATE_ZERO);       // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution = 
            while (_connection.Read(JetBusCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_ONGOING);
            
            // check : command "ok" = command is done = 
            while (_connection.Read(JetBusCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_OK);
            
        }


        // This method sets the value for the nominal weight in the WTX.
        public override void Calibrate(int calibrationValue, string calibrationWeightStr)
        {
            _connection.Write(JetBusCommands.LFT_SCALE_CALIBRATION_WEIGHT, calibrationValue);          // LFT_SCALE_CALIBRATION_WEIGHT = "6152/00" 

            _connection.Write(JetBusCommands.SCALE_COMMAND, SCALE_COMMAND_CALIBRATE_NOMINAL);  // CALIBRATE_NOMINAL_WEIGHT = 1852596579 // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution = 
            while (_connection.Read(JetBusCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_ONGOING) ;      // ID_keys.SCALE_COMMAND_STATUS = 6002/02

            // check : command "ok" = command is done = 
            while (_connection.Read(JetBusCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_OK) ;     

            //this._isCalibrating = true;
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


        public override void adjustZero()
        {
            throw new NotImplementedException();
        }


        public override void adjustNominal()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Limit switches
        public int LimitValue1Input
        {
            get
            {
                return _connection.AllData[JetBusCommands.LIMIT_VALUE] & 0x1;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public int LimitValue2Source
        {
            get
            {
                return (_connection.AllData[JetBusCommands.LIMIT_VALUE] & 0x2) >> 1;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public int LimitValue3Source
        {
            get
            {
                return (_connection.AllData[JetBusCommands.LIMIT_VALUE] & 0x4) >> 2;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public int LimitValue4Source
        {
            get
            {
                return (_connection.AllData[JetBusCommands.LIMIT_VALUE] & 0x8) >> 3;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }
        #endregion


    }
}