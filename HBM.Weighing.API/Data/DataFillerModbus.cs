// <copyright file="DataFiller.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using HBM.Weighing.API.WTX.Jet;
using HBM.Weighing.API.WTX.Modbus;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// Implementation of the interface IDataFiller for the filler mode.
    /// The class DataFiller contains the data input word and data output words for the filler mode
    /// of WTX device 120 and 110.
    /// </summary>
    public class DataFillerModbus : IDataFiller
    {
        #region privates for filler mode

        private int _coarseFlow;
        private int _fineFlow;
        private int _ready;
        private int _reDosing;

        private int _emptying;
        private int _flowError;
        private int _alarm;
        private int _adcOverUnderload;

        private int _maxDosingTime;
        private int _legalForTradeOperation;
        private int _toleranceErrorPlus;
        private int _toleranceErrorMinus;

        private int _statusInput1;
        private int _generalScaleError;

        private int _fillingProcessStatus;
        private int _numberDosingResults;
        private int _dosingResult;
        private int _meanValueDosingResults;

        private int _standardDeviation;
        private int _totalWeight;
        private int _fineFlowCutOffPoint;
        private int _coarseFlowCutOffPoint;

        private int _currentDosingTime;
        private int _currentCoarseFlowTime;
        private int _currentFineFlowTime;
        private int _parameterSetProduct;

        private int _weightMemoryDay;
        private int _weightMemoryMonth;
        private int _weightMemoryYear;
        private int _weightMemorySeqNumber;
        private int _weightMemoryGross;
        private int _weightMemoryNet;

        private int _weight_storage;
        private int _mode_weight_storage;

        // Output words for filler mode: 

        private int _residualFlowTime;
        private int _targetFillingWeight;
        private int _coarseFlowCutOffPointSet;
        private int _fineFlowCutOffPointSet;

        private int _minimumFineFlow;
        private int _optimizationOfCutOffPoints;
        private int _maximumDosingTime;
        private int _startWithFineFlow;

        private int _coarseLockoutTime;
        private int _fineLockoutTime;
        private int _tareMode;
        private int _upperToleranceLimit;

        private int _lowerToleranceLimit;
        private int _minimumStartWeight;
        private int _emptyWeight;
        private int _tareDelay;

        private int _coarseFlowMonitoringTime;
        private int _coarseFlowMonitoring;
        private int _fineFlowMonitoring;
        private int _fineFlowMonitoringTime;

        private int _delayTimeAfterFineFlow;
        private int _activationTimeAfterFineFlow;
        private int _systematicDifference;
        private int _downwardsDosing;

        private int _valveControl;
        private int _emptyingMode;

        private INetConnection _connection;

        #endregion

        #region contructor

        public DataFillerModbus(INetConnection Connection) : base()
        {
            _connection = Connection;

            _connection.UpdateData += UpdateFillerData;
            Console.WriteLine("DataFillerModbus");

            _coarseFlow = 0;
            _fineFlow = 0;
            _ready = 0;
            _reDosing = 0;

            _emptying = 0;
            _flowError = 0;
            _alarm = 0;
            _adcOverUnderload = 0;

            _fillingProcessStatus = 0;
            _numberDosingResults = 0;
            _dosingResult = 0;
            _meanValueDosingResults = 0;

            _standardDeviation = 0;
            _totalWeight = 0;
            _currentDosingTime = 0;
            _currentCoarseFlowTime = 0;

            _currentFineFlowTime = 0;
            _toleranceErrorPlus = 0;
            _toleranceErrorMinus = 0;
            _statusInput1 = 0;

            _generalScaleError = 0;
            _fineFlowCutOffPoint = 0;

            _coarseFlowCutOffPoint = 0;
            _parameterSetProduct = 0;
            _maxDosingTime = 0;

            _residualFlowTime = 0;
            _targetFillingWeight = 0;
            _coarseFlowCutOffPointSet = 0;
            _fineFlowCutOffPointSet = 0;

            _minimumFineFlow = 0;
            _optimizationOfCutOffPoints = 0;
            _maximumDosingTime = 0;
            _startWithFineFlow = 0;

            _coarseLockoutTime = 0;
            _fineLockoutTime = 0;
            _tareMode = 0;
            _upperToleranceLimit = 0;

            _lowerToleranceLimit = 0;
            _minimumStartWeight = 0;
            _emptyWeight = 0;
            _tareDelay = 0;

            _coarseFlowMonitoringTime = 0;
            _coarseFlowMonitoring = 0;
            _fineFlowMonitoring = 0;
            _fineFlowMonitoringTime = 0;

            _delayTimeAfterFineFlow = 0;
            _activationTimeAfterFineFlow = 0;
            _systematicDifference = 0;
            _downwardsDosing = 0;

            _valveControl = 0;
            _emptyingMode = 0;

            _weight_storage = 0;
            _mode_weight_storage = 0;
        }

        #endregion

        #region Update methods for the filler mode

        public void UpdateFillerData(object sender, EventArgs e)
        {
            if (_connection.ReadFromBuffer(ModbusCommands.Application_mode) == 2 || _connection.ReadFromBuffer(ModbusCommands.Application_mode) == 3)  // If application mode = filler
            {
                // Via Modbus and Jetbus IDs: 
                _maxDosingTime = _connection.ReadFromBuffer(ModbusCommands.Maximal_dosing_time);
                //_meanValueDosingResults = _connection.GetDataFromDictionary(ModbusCommands.Mean_value_dosing_results);
                //_standardDeviation = _connection.GetDataFromDictionary(ModbusCommands.Standard_deviation);
                _fineFlowCutOffPoint = _connection.ReadFromBuffer(ModbusCommands.Fine_flow_cut_off_point);
                _coarseFlowCutOffPoint = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_cut_off_point);

                //_residualFlowTime = _connection.GetDataFromDictionary(ModbusCommands.Residual_flow_time);
                _minimumFineFlow = _connection.ReadFromBuffer(ModbusCommands.Minimum_fine_flow);
                _optimizationOfCutOffPoints = _connection.ReadFromBuffer(ModbusCommands.Optimization);
                _maximumDosingTime = _connection.ReadFromBuffer(ModbusCommands.Maximal_dosing_time);
                _coarseLockoutTime = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_time);
                _fineLockoutTime = _connection.ReadFromBuffer(ModbusCommands.CurrentFineFlowTime);
                _tareMode = _connection.ReadFromBuffer(ModbusCommands.Tare_mode);

                _upperToleranceLimit = _connection.ReadFromBuffer(ModbusCommands.Upper_tolerance_limit);
                _lowerToleranceLimit = _connection.ReadFromBuffer(ModbusCommands.Lower_tolerance_limit);
                _minimumStartWeight = _connection.ReadFromBuffer(ModbusCommands.Minimum_start_weight);
                //_emptyWeight = _connection.GetDataFromDictionary(ModbusCommands.Empty_weight);
                _tareDelay = _connection.ReadFromBuffer(ModbusCommands.Tare_delay);

                _coarseFlowMonitoringTime = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_monitoring_time);
                _coarseFlowMonitoring = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_monitoring);
                _fineFlowMonitoring = _connection.ReadFromBuffer(ModbusCommands.Fine_flow_monitoring);
                //_fineFlowMonitoringTime = _connection.GetDataFromDictionary(ModbusCommands.Fine_flow_monitoring_time); ;
                _systematicDifference = _connection.ReadFromBuffer(ModbusCommands.Systematic_difference);
                
                /*
                _valveControl = _connection.GetDataFromDictionary(ModbusCommands.Valve_control);
                _emptyingMode = _connection.GetDataFromDictionary(ModbusCommands.Emptying_mode);
                _delayTimeAfterFineFlow = _connection.GetDataFromDictionary(ModbusCommands.Delay_time_after_fine_flow);
                _activationTimeAfterFineFlow = _connection.GetDataFromDictionary(ModbusCommands.Activation_time_after_fine_flow);
                */

                _adcOverUnderload = _connection.ReadFromBuffer(ModbusCommands.AdcOverUnderload);
                _legalForTradeOperation = _connection.ReadFromBuffer(ModbusCommands.LegalForTradeOperation);
                _statusInput1 = _connection.ReadFromBuffer(ModbusCommands.StatusInput1);
                _generalScaleError = _connection.ReadFromBuffer(ModbusCommands.GeneralScaleError);
                
                _coarseFlow = _connection.ReadFromBuffer(ModbusCommands.CoarseFlow);
                _fineFlow = _connection.ReadFromBuffer(ModbusCommands.FineFlow);
                _ready = _connection.ReadFromBuffer(ModbusCommands.Ready);
                _reDosing = _connection.ReadFromBuffer(ModbusCommands.ReDosing);

                _emptying = _connection.ReadFromBuffer(ModbusCommands.Emptying);
                _flowError = _connection.ReadFromBuffer(ModbusCommands.FlowError);
                _alarm = _connection.ReadFromBuffer(ModbusCommands.Alarm);
                _toleranceErrorPlus = _connection.ReadFromBuffer(ModbusCommands.ToleranceErrorPlus);

                _toleranceErrorMinus = _connection.ReadFromBuffer(ModbusCommands.ToleranceErrorMinus);
                _currentDosingTime = _connection.ReadFromBuffer(ModbusCommands.Dosing_time);
                _currentCoarseFlowTime = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_time);
                _currentFineFlowTime = _connection.ReadFromBuffer(ModbusCommands.CurrentFineFlowTime);

                _parameterSetProduct = _connection.ReadFromBuffer(ModbusCommands.ParameterSetProduct);
                //_downwardsDosing = _connection.GetDataFromDictionary(ModbusCommands.DownwardsDosing);
                _totalWeight = _connection.ReadFromBuffer(ModbusCommands.TotalWeight);

                //_targetFillingWeight = Convert.ToInt32(e.DataDictionary[ModbusCommands.TargetFillingWeight);
                _coarseFlowCutOffPointSet = _connection.ReadFromBuffer(ModbusCommands.Coarse_flow_cut_off_point);
                _fineFlowCutOffPointSet = _connection.ReadFromBuffer(ModbusCommands.Fine_flow_cut_off_point);
                _startWithFineFlow = _connection.ReadFromBuffer(ModbusCommands.Run_start_dosing);  // Command 'Run_start_dosing' right
                
                _weightMemoryDay = _connection.ReadFromBuffer(ModbusCommands.WeightMemDayStandard);
                _weightMemoryMonth = _connection.ReadFromBuffer(ModbusCommands.WeightMemMonthStandard);
                _weightMemoryYear = _connection.ReadFromBuffer(ModbusCommands.WeightMemYearStandard);
                _weightMemorySeqNumber = _connection.ReadFromBuffer(ModbusCommands.WeightMemSeqNumberStandard);
                _weightMemoryGross = _connection.ReadFromBuffer(ModbusCommands.WeightMemGrossStandard);
                _weightMemoryNet = _connection.ReadFromBuffer(ModbusCommands.WeightMemNetStandard);
            }
        }
        #endregion

        #region Get-properties for input words of filler mode

        public int CoarseFlow
        {
            get { return _coarseFlow; }
        }
        public int FineFlow
        {
            get { return _fineFlow; }
        }
        public int Ready
        {
            get { return _ready; }
        }
        public int ReDosing
        {
            get { return _reDosing; }
        }
        public int Emptying
        {
            get { return _emptying; }
        }
        public int FlowError
        {
            get { return _flowError; }
        }
        public int Alarm
        {
            get { return _alarm; }
        }
        public int AdcOverUnderload
        {
            get { return _adcOverUnderload; }
        }
        public int FillingProcessStatus
        {
            get { return _fillingProcessStatus; }
        }
        public int NumberDosingResults
        {
            get { return _numberDosingResults; }
        }
        public int DosingResult
        {
            get { return _dosingResult; }
        }
        public int MeanValueDosingResults
        {
            get { return _meanValueDosingResults; }
        }
        public int StandardDeviation
        {
            get { return _standardDeviation; }
        }
        public int TotalWeight
        {
            get { return _totalWeight; }
        }
        public int CurrentDosingTime
        {
            get { return _currentDosingTime; }
        }
        public int CurrentCoarseFlowTime
        {
            get { return _currentCoarseFlowTime; }
        }
        public int CurrentFineFlowTime
        {
            get { return _currentFineFlowTime; }
        }
        public int ToleranceErrorPlus
        {
            get { return _toleranceErrorPlus; }
        }
        public int ToleranceErrorMinus
        {
            get { return _toleranceErrorMinus; }
        }
        public int StatusInput1
        {
            get { return _statusInput1; }
        }
        public int GeneralScaleError
        {
            get { return _generalScaleError; }
        }
        public int FineFlowCutOffPoint
        {
            get { return _fineFlowCutOffPoint; }
            set { this._fineFlowCutOffPoint = value; }
        }
        public int CoarseFlowCutOffPoint
        {
            get { return _coarseFlowCutOffPoint; }
            set { this._coarseFlowCutOffPoint = value; }
        }
        public int ParameterSetProduct
        {
            get { return _parameterSetProduct; }
        }
        public int MaxDosingTime
        {
            get { return _maxDosingTime; }
        }
        public int LegalForTradeOperation
        {
            get { return _legalForTradeOperation; }
        }
        public int WeightMemDay
        {
            get { return _weightMemoryDay; }
        }
        public int WeightMemMonth
        {
            get { return _weightMemoryMonth; }
        }
        public int WeightMemYear
        {
            get { return _weightMemoryYear; }
        }
        public int WeightMemSeqNumber
        {
            get { return _weightMemorySeqNumber; }
        }
        public int WeightMemGross
        {
            get { return _weightMemoryGross; }
        }
        public int WeightMemNet
        {
            get { return _weightMemoryNet; }
        }

        #endregion

        #region Get/Set-properties for output words of filler mode

        public int ResidualFlowTime // Type : unsigned integer 16 Bit
        {
            get { return _residualFlowTime; }
            set
            {
                _connection.Write(ModbusCommands.Residual_flow_time, value);
                this._residualFlowTime = value;
            }
        }
        public int TargetFillingWeight // Type : signed integer 32 Bit
        {
            get { return _targetFillingWeight; }
            set
            {
                //_connection.Write(ModbusCommands.Reference_value_dosing, value);
                this._targetFillingWeight = value;
            }
        }
        public int CoarseFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _coarseFlowCutOffPointSet; }
            set
            {
                _connection.Write(ModbusCommands.Coarse_flow_cut_off_point, value);
                this._coarseFlowCutOffPointSet = value;
            }
        }
        public int FineFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _fineFlowCutOffPointSet; }
            set
            {
                _connection.Write(ModbusCommands.Fine_flow_cut_off_point, value);
                this._fineFlowCutOffPointSet = value;
            }
        }
        public int MinimumFineFlow // Type : signed integer 32 Bit
        {
            get { return _minimumFineFlow; }
            set
            {
                _connection.Write(ModbusCommands.Minimum_fine_flow, value);
                this._minimumFineFlow = value;
            }
        }
        public int OptimizationOfCutOffPoints // Type : unsigned integer 8 Bit
        {
            get { return _optimizationOfCutOffPoints; }
            set
            {
                _connection.Write(ModbusCommands.Optimization, value);
                this._optimizationOfCutOffPoints = value;
            }
        }
        public int MaximumDosingTime // Type : unsigned integer 16 Bit
        {
            get { return _maximumDosingTime; }
            set
            {
                _connection.Write(ModbusCommands.Maximal_dosing_time, value);
                this._maximumDosingTime = value;
            }
        }
        public int StartWithFineFlow // Type : unsigned integer 16 Bit
        {
            get { return _startWithFineFlow; }
            set
            {
                _connection.Write(ModbusCommands.Run_start_dosing, value);
                this._startWithFineFlow = value;
            }
        }
        public int CoarseLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseLockoutTime; }
            set
            {
                _connection.Write(ModbusCommands.Lockout_time_coarse_flow, value);
                this._coarseLockoutTime = value;
            }
        }
        public int FineLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _fineLockoutTime; }
            set
            {
                _connection.Write(ModbusCommands.Lockout_time_fine_flow, value);
                this._fineLockoutTime = value;
            }
        }
        public int TareMode // Type : unsigned integer 8 Bit
        {
            get { return _tareMode; }
            set
            {
                _connection.Write(ModbusCommands.Tare_mode, value);
                this._tareMode = value;
            }
        }
        public int UpperToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _upperToleranceLimit; }
            set
            {
                _connection.Write(ModbusCommands.Upper_tolerance_limit, value);
                this._upperToleranceLimit = value;
            }
        }
        public int LowerToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _lowerToleranceLimit; }
            set
            {
                _connection.Write(ModbusCommands.Lower_tolerance_limit, value);
                this._lowerToleranceLimit = value;
            }
        }
        public int MinimumStartWeight // Type : signed integer 32 Bit
        {
            get { return _minimumStartWeight; }
            set
            {
                _connection.Write(ModbusCommands.Minimum_start_weight, value);
                this._minimumStartWeight = value;
            }
        }
        public int EmptyWeight // Type : signed integer 32 Bit
        {
            get { return _emptyWeight; }
            set
            {
                _connection.Write(ModbusCommands.Empty_weight, value);
                this._emptyWeight = value;
            }
        }
        public int TareDelay // Type : unsigned integer 16 Bit
        {
            get { return _tareDelay; }
            set
            {
                _connection.Write(ModbusCommands.Tare_delay, value);
                this._tareDelay = value;
            }
        }
        public int CoarseFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseFlowMonitoringTime; }
            set
            {
                _connection.Write(ModbusCommands.Coarse_flow_monitoring_time, value);
                this._coarseFlowMonitoringTime = value;
            }
        }
        public int CoarseFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _coarseFlowMonitoring; }
            set
            {
                _connection.Write(ModbusCommands.Coarse_flow_monitoring, value);
                this._coarseFlowMonitoring = value;
            }
        }
        public int FineFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _fineFlowMonitoring; }
            set
            {
                _connection.Write(ModbusCommands.Fine_flow_monitoring, value);
                this._fineFlowMonitoring = value;
            }
        }
        public int FineFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _fineFlowMonitoringTime; }
            set
            {
                _connection.Write(ModbusCommands.Fine_flow_monitoring_time, value);
                this._fineFlowMonitoringTime = value;
            }
        }
        public int DelayTimeAfterFineFlow  // Type : unsigned integer 8 Bit
        {
            get { return _delayTimeAfterFineFlow; }
            set
            {
                _connection.Write(ModbusCommands.Delay_time_after_fine_flow, value);
                this._delayTimeAfterFineFlow = value;
            }
        }
        public int ActivationTimeAfterFineFlow  // Type : unsigned integer 8 Bit
        {
            get { return _activationTimeAfterFineFlow; }
            set
            {
                _connection.Write(ModbusCommands.Activation_time_after_fine_flow, value);
                this._activationTimeAfterFineFlow = value;
            }
        }
        public int SystematicDifference // Type : unsigned integer 32 Bit
        {
            get { return _systematicDifference; }
            set
            {
                _connection.Write(ModbusCommands.Systematic_difference, value);
                this._systematicDifference = value;
            }
        }
        public int DownwardsDosing  // Type : unsigned integer 8 Bit
        {
            get { return _downwardsDosing; }
            set
            {
                _connection.Write(ModbusCommands.DownwardsDosing, value);
                this._downwardsDosing = value;
            }
        }
        public int ValveControl  // Type : unsigned integer 8 Bit
        {
            get { return _valveControl; }
            set
            {
                _connection.Write(ModbusCommands.Valve_control, value);
                this._valveControl = value;
            }
        }

        public int EmptyingMode  // Type : unsigned integer 8 Bit
        {
            get { return _emptyingMode; }
            set
            {
                _connection.Write(ModbusCommands.Emptying_mode, value);
                this._emptyingMode = value;
            }
        }
        public int WeightStorage
        {
            get { return _weight_storage; }
            set { this._weight_storage = value; }
        }
        public int ModeWeightStorage
        {
            get { return _mode_weight_storage; }
            set { this._mode_weight_storage = value; }      
        }

        #endregion

    }
}