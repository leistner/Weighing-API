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
        //To be eliminated eventually !?...-------------
        private string[] _dataStrArr;
        private ushort[] _dataUshort;

        public override string[] GetDataStr
        {
            get
            {
                return this._dataStrArr;
            }
        }

        public override ushort[] GetDataUshort
        {
            get
            {
                return this._dataUshort;
            }
        }
        
        public override void OnData(ushort[] _asyncData)
        {
            throw new NotImplementedException();
        }
        //----------------------------------


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
            
            _dataStrArr = new string[185];
            _dataUshort = new ushort[185];

            for (int index = 0; index < _dataStrArr.Length; index++)
            {
                _dataStrArr[index] = "";
                _dataUshort[index] = 0;
            }                   
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
            _processData.NetValue = this.NetValue;
            _processData.GrossValue = this.GrossValue;
            _processData.Tare = this.NetValue-this.GrossValue;
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
            _processData.Decimals = this.Decimals;
            _processData.Unit = this.Unit;
            _processData.Handshake = Convert.ToBoolean(this.Handshake);
            _processData.Status = Convert.ToBoolean(this.Status);
            _processData.Underload = false;
            _processData.Overload = false;
            _processData.weightWithinLimits = false;
            _processData.higherSafeLoadLimit = false;
            _processData.LegalTradeOp = 0;

            // Do something with the data, like in the class WTXModbus.cs           
            this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(_processData));
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
        
        public override int ApplicationMode { get { return 1; } }
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

        public override int NetValue
        {
            get
            {
                return _connection.AllData[JetBusCommands.NET_VALUE];
             }
        }

        public override int GrossValue
        {
            get
            {
                return _connection.AllData[JetBusCommands.GROSS_VALUE];
            }
        }

        public override int Decimals
        {
            get
            {
                    return _connection.AllData[JetBusCommands.DECIMALS];
            }
        }


        public override int ManualTareValue { get; set; }


        public override int GeneralWeightError
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1);
            }
        }

        public override int ScaleAlarmTriggered
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x2) >> 1;
            }
        }


        public override int LimitStatus
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0xC) >> 2;
            }
        }

        public override int WeightMoving
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x10) >> 4;
            }
        }

        public override int ScaleSealIsOpen
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x20) >> 5;
            }
        }

        public override int ManualTare
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x40) >> 6;
            }
        }
        public override int WeightType
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x80) >> 7;
            }
        }

        public override int ScaleRange
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x300) >> 8;
            }
        }

        public override int ZeroRequired
        {
            get
            {
                ;
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x400) >> 10;
            }
        }

        public override int WeightWithinTheCenterOfZero
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x800) >> 11;
            }
        }

        public override int WeightInZeroRange
        {
            get
            {
                return (_connection.AllData[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1000) >> 12;
            }
        }

        public override int Unit
        {
            get
            {
                try
                {
                    return (_connection.AllData[JetBusCommands.UNIT_PREFIX_FIXED_PARAMETER] & 0xFF0000) >> 16;
                }
                catch (Exception)
                {
                    return 76;
                }

            }
        }

        public override int AdcOverUnderload { get { return 1; } }

        public override int LegalTradeOp { get { return 1; } }

        public override int StatusInput1 { get { return 1; } }

        public override int GeneralScaleError { get { return 1; } }

        public override int Input1
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_1];
            }
        }         // ID = IM1

        public override int Input2
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_2];
            }
        }         // ID = IM2

        public override int Input3
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_3];
            }
        }         // ID = IM3        

        public override int Input4
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_INPUT_4];
            }
        }         // ID = IM4          

        public override int Output1
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_1];
            }
        }        // ID = OM1

        public override int Output2
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_2];
            }
        }        // ID = OM2 

        public override int Output3
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_3];
            }
        }        // ID = OM3

        public override int Output4
        {
            get
            {
                return _connection.AllData[JetBusCommands.FUNCTION_DIGITAL_OUTPUT_4];
            }
        }        // ID = OM4

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
            throw new NotImplementedException();
        }

        public override void manualTaring()
        {
            throw new NotImplementedException();
        }

        public override void recordWeight()
        {
            throw new NotImplementedException();
        }

        public override int Status
        {
            get
            {
                return _connection.AllData[JetBusCommands.SCALE_COMMAND_STATUS];
            }
        }

        // Method to check if the handshake is done.
        public override int Handshake
        {
            get
            {
                if (_connection.AllData[JetBusCommands.SCALE_COMMAND_STATUS] == 1801543519)
                    return 1;
                else
                    return 0;
            }
        }
        #endregion


        #region Process data - Standard
        public override int LimitStatus1
        {
            get
            {
                return _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_1];
            }
        }  

        public override int LimitStatus2
        {
            get
            {
                return _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_2];
            }
        } 

        public override int LimitStatus3
        {
            get
            {
                return _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_3];
            }
        }   

        public override int LimitStatus4
        {
            get
            {
                return _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_4];
            }
        }
        #endregion


        #region Process data - Filling
        public override int FillingProcessStatus
        {
            get
            {
                return _connection.AllData[JetBusCommands.DOSING_STATUS];
            }
        }


        public override int NumberDosingResults
        {
            get
            {
                return _connection.AllData[JetBusCommands.DOSING_COUNTER];
            }
        }


        public override int DosingResult
        {
            get
            {
                return _connection.AllData[JetBusCommands.DOSING_RESULT];
            }
        }


        public override int CoarseFlow { get { return 1; } }
        public override int FineFlow { get { return 1; } }
        public override int Ready { get { return 1; } }
        public override int ReDosing { get { return 1; } }
        public override int Emptying { get { return 1; } }
        public override int FlowError { get { return 1; } }
        public override int Alarm { get { return 1; } }


        public override int ToleranceErrorPlus { get { return 1; } }
        public override int ToleranceErrorMinus { get { return 1; } }

        public override int CurrentDosingTime { get { return 1; } }
        public override int CurrentCoarseFlowTime { get { return 1; } }
        public override int CurrentFineFlowTime { get { return 1; } }

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


        public override void adjustZero()
        {
            throw new NotImplementedException();
        }


        public override void adjustNominal()
        {
            throw new NotImplementedException();
        }
        #endregion


        #region Filler

        public override int ParameterSetProduct { get { return 1; } }

        public override int MaxDosingTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.MAXIMUM_DOSING_TIME];
            }
        } // MDT

        public override int MeanValueDosingResults
        {
            get
            {
                return _connection.AllData[JetBusCommands.MEAN_VALUE_DOSING_RESULTS];
            }
        }    // SDM
        public override int StandardDeviation
        {
            get
            {
                return _connection.AllData[JetBusCommands.STANDARD_DEVIATION];
            }
        }         // SDS 

        public override int FineFlowCutOffPoint
        {
            get
            {
                return _connection.AllData[JetBusCommands.FINE_FLOW_CUT_OFF_POINT];
            }
        }       // FFD
        public override int CoarseFlowCutOffPoint
        {
            get
            {
                return _connection.AllData[JetBusCommands.COARSE_FLOW_CUT_OFF_POINT];
            }
        }     // CFD

        public override int ResidualFlowTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.RESIDUAL_FLOW_TIME];
            }
            set
            {
                _connection.AllData[JetBusCommands.RESIDUAL_FLOW_TIME] = value;
                _connection.Write(JetBusCommands.RESIDUAL_FLOW_TIME, value);
            }
        }    // RFT

        public override int MinimumFineFlow
        {
            get
            {
                return _connection.AllData[JetBusCommands.MINIMUM_FINE_FLOW];
            }
            set
            {
                _connection.AllData[JetBusCommands.MINIMUM_FINE_FLOW] = value;
                _connection.Write(JetBusCommands.MINIMUM_FINE_FLOW, value);
            }
        }     //FFM
        public override int OptimizationOfCutOffPoints
        {
            get
            {
                return _connection.AllData[JetBusCommands.OPTIMIZATION];
            }
            set
            {
                _connection.AllData[JetBusCommands.OPTIMIZATION] = value;
                _connection.Write(JetBusCommands.OPTIMIZATION, value);
            }
        }   // OSN
        public override int MaximumDosingTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_3];
            }
            set
            {
                _connection.AllData[JetBusCommands.STATUS_DIGITAL_OUTPUT_3] = value;
                _connection.Write(JetBusCommands.STATUS_DIGITAL_OUTPUT_3, value);
            }
        }   // MDT

        public override int CoarseLockoutTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.COARSE_FLOW_TIME];
            }
            set
            {
                _connection.AllData[JetBusCommands.COARSE_FLOW_TIME] = value;
                _connection.Write(JetBusCommands.COARSE_FLOW_TIME, value);
            }
        }    // CFT
        public override int FineLockoutTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.FINE_FLOW_TIME];
            }
            set
            {
                _connection.AllData[JetBusCommands.FINE_FLOW_TIME] = value;
                _connection.Write(JetBusCommands.FINE_FLOW_TIME, value);
            }
        }      // Fine flow time = FFT
        public override int TareMode
        {
            get
            {
                return _connection.AllData[JetBusCommands.TARE_MODE];
            }
            set
            {
                _connection.AllData[JetBusCommands.TARE_MODE] = value;
                _connection.Write(JetBusCommands.TARE_MODE, value);
            }
        }             // ID = TMD 

        public override int UpperToleranceLimit
        {
            get
            {
                return _connection.AllData[JetBusCommands.UPPER_TOLERANCE_LIMIT];
            }
            set
            {
                _connection.AllData[JetBusCommands.UPPER_TOLERANCE_LIMIT] = value;
                _connection.Write(JetBusCommands.UPPER_TOLERANCE_LIMIT, value);
            }
        }      // UTL
        public override int LowerToleranceLimit
        {
            get
            {
                return _connection.AllData[JetBusCommands.LOWER_TOLERANCE_LOMIT];
            }
            set
            {
                _connection.AllData[JetBusCommands.LOWER_TOLERANCE_LOMIT] = value;
                _connection.Write(JetBusCommands.LOWER_TOLERANCE_LOMIT, value);
            }
        }      // LTL

        public override int MinimumStartWeight
        {
            get
            {
                return _connection.AllData[JetBusCommands.MINIMUM_START_WEIGHT];
            }
            set
            {
                _connection.AllData[JetBusCommands.MINIMUM_START_WEIGHT] = value;
                _connection.Write(JetBusCommands.MINIMUM_START_WEIGHT, value);
            }
        }        // MSW
        public override int EmptyWeight
        {
            get
            {
                return _connection.AllData[JetBusCommands.EMPTY_WEIGHT];
            }
            set
            {
                _connection.AllData[JetBusCommands.EMPTY_WEIGHT] = value;
                _connection.Write(JetBusCommands.EMPTY_WEIGHT, value);
            }
        }  // EWT

        public override int TareDelay
        {
            get
            {
                return _connection.AllData[JetBusCommands.TARE_DELAY];
            }
            set
            {
                _connection.AllData[JetBusCommands.TARE_DELAY] = value;
                _connection.Write(JetBusCommands.TARE_DELAY, value);
            }
        }    // TAD

        public override int CoarseFlowMonitoringTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING_TIME];
            }
            set
            {
                _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING_TIME] = value;
                _connection.Write(JetBusCommands.COARSE_FLOW_MONITORING_TIME, value);
            }
        }  // CBT
        public override int CoarseFlowMonitoring
        {
            get
            {
                return _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING];
            }
            set
            {
                _connection.AllData[JetBusCommands.COARSE_FLOW_MONITORING] = value;
                _connection.Write(JetBusCommands.COARSE_FLOW_MONITORING, value);
            }
        }      // CBK
        public override int FineFlowMonitoring
        {
            get
            {
                return _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING];
            }
            set
            {
                _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING] = value;
                _connection.Write(JetBusCommands.FINE_FLOW_MONITORING, value);
            }
        }        // FBK
        public override int FineFlowMonitoringTime
        {
            get
            {
                return _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING_TIME];
            }
            set
            {
                _connection.AllData[JetBusCommands.FINE_FLOW_MONITORING_TIME] = value;
                _connection.Write(JetBusCommands.FINE_FLOW_MONITORING_TIME, value);
            }
        }    // FBT

        public override int SystematicDifference
        {
            get
            {
                return _connection.AllData[JetBusCommands.SYSTEMATIC_DIFFERENCE];
            }
            set
            {
                _connection.AllData[JetBusCommands.SYSTEMATIC_DIFFERENCE] = value;
                _connection.Write(JetBusCommands.SYSTEMATIC_DIFFERENCE, value);
            }
        }  // SYD

        public override int DownwardsDosing { get; set; }

        public override int ValveControl
        {
            get
            {
                return _connection.AllData[JetBusCommands.VALVE_CONTROL];
            }
            set
            {
                _connection.AllData[JetBusCommands.VALVE_CONTROL] = value;
                _connection.Write(JetBusCommands.VALVE_CONTROL, value);
            }
        }      // VCT
        public override int EmptyingMode
        {
            get
            {
                return _connection.AllData[JetBusCommands.EMPTYING_MODE];
            }
            set
            {
                _connection.AllData[JetBusCommands.EMPTYING_MODE] = value;
                _connection.Write(JetBusCommands.EMPTYING_MODE, value);
            }
        }      // EMD
        

        public override int DelayTimeAfterFineFlow
        {
            get
            {
                return _connection.AllData[JetBusCommands.DELAY1_DOSING];
            }
            set
            {
                _connection.AllData[JetBusCommands.DELAY1_DOSING] = value;
                _connection.Write(JetBusCommands.DELAY1_DOSING, value);
            }
        }      

        public override int ActivationTimeAfterFineFlow
        {
            get
            {
                return _connection.AllData[JetBusCommands.FINEFLOW_PHASE_BEFORE_COARSEFLOW];
            }
            set
            {
                _connection.AllData[JetBusCommands.FINEFLOW_PHASE_BEFORE_COARSEFLOW] = value;
                _connection.Write(JetBusCommands.FINEFLOW_PHASE_BEFORE_COARSEFLOW, value);
            }
        }
        
       
        public override int TotalWeight { get { return 1; } }
        public override int TargetFillingWeight { get; set; }
        public override int CoarseFlowCutOffPointSet { get; set; }
        public override int FineFlowCutOffPointSet { get; set; }
        public override int StartWithFineFlow { get; set; }
        #endregion


        #region Limit switches
        public override int LimitValue1Input
        {
            get
            {
                int value = _connection.AllData[JetBusCommands.LIMIT_VALUE];
                return (value & 0x1);
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public override int LimitValue2Source
        {
            get
            {
                int value = _connection.AllData[JetBusCommands.LIMIT_VALUE];
                return (value & 0x2)>>1;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public override int LimitValue3Source
        {
            get
            {
                int value = _connection.AllData[JetBusCommands.LIMIT_VALUE];
                return (value & 0x4) >> 2;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }

        public override int LimitValue4Source
        {
            get
            {
                int value = _connection.AllData[JetBusCommands.LIMIT_VALUE];
                return (value & 0x8) >> 3;
            }
            set
            {
                _connection.AllData[JetBusCommands.LIMIT_VALUE] = value;
                _connection.Write(JetBusCommands.LIMIT_VALUE, value);
            }
        }        

        public override int LimitValue2Mode { get; set; }
        public override int LimitValue2ActivationLevelLowerBandLimit { get; set; }
        public override int LimitValue2HysteresisBandHeight { get; set; }
        public override int LimitValue3Mode { get; set; }
        public override int LimitValue3ActivationLevelLowerBandLimit { get; set; }
        public override int LimitValue3HysteresisBandHeight { get; set; }
        public override int LimitValue4Mode { get; set; }
        public override int LimitValue4ActivationLevelLowerBandLimit { get; set; }
        public override int LimitValue4HysteresisBandHeight { get; set; }
        public override int LimitValue1Mode { get; set; }
        public override int LimitValue1ActivationLevelLowerBandLimit { get; set; }
        public override int LimitValue1HysteresisBandHeight { get; set; }
        #endregion


        #region Alibi memory
        public override int WeightMemDay { get { return 1; } }
        public override int WeightMemMonth { get { return 1; } }
        public override int WeightMemYear { get { return 1; } }
        public override int WeightMemSeqNumber { get { return 1; } }
        public override int WeightMemGross { get { return 1; } }
        public override int WeightMemNet { get { return 1; } }
        #endregion               
    }
}