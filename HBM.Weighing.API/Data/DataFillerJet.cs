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
    public class DataFillerJet : IDataFiller
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
        private JetBusCommands _commands;
       
        #endregion

        #region contructor

        public DataFillerJet(INetConnection Connection) : base()
        {
            _connection = Connection;
           
            _connection.UpdateDataClasses += UpdateFillerData;

            _commands = new JetBusCommands();
            
            _coarseFlow = 0;
            _fineFlow=0;
            _ready=0;
            _reDosing=0;

            _emptying=0;
            _flowError=0;
            _alarm=0;
            _adcOverUnderload=0;

            _fillingProcessStatus=0;
            _numberDosingResults=0;
            _dosingResult=0;
            _meanValueDosingResults=0;

            _standardDeviation=0;
            _totalWeight=0;
            _currentDosingTime=0;
            _currentCoarseFlowTime=0;

            _currentFineFlowTime=0;
            _toleranceErrorPlus=0;
            _toleranceErrorMinus=0;
            _statusInput1=0;

            _generalScaleError=0;
            _fineFlowCutOffPoint=0;

            _coarseFlowCutOffPoint=0;
            _parameterSetProduct=0;
            _maxDosingTime=0;

            _residualFlowTime=0;
            _targetFillingWeight=0;
            _coarseFlowCutOffPointSet=0;
            _fineFlowCutOffPointSet=0;

            _minimumFineFlow=0;
            _optimizationOfCutOffPoints=0;
            _maximumDosingTime=0;
            _startWithFineFlow=0;

            _coarseLockoutTime=0;
            _fineLockoutTime=0;
            _tareMode=0;
            _upperToleranceLimit=0;

            _lowerToleranceLimit=0;
            _minimumStartWeight=0;
            _emptyWeight=0;
            _tareDelay=0;

            _coarseFlowMonitoringTime=0;
            _coarseFlowMonitoring=0;
            _fineFlowMonitoring=0;
            _fineFlowMonitoringTime=0;

            _delayTimeAfterFineFlow=0;
            _activationTimeAfterFineFlow=0;
            _systematicDifference=0;
            _downwardsDosing=0;

            _valveControl=0;
            _emptyingMode=0;

            _weight_storage=0;
            _mode_weight_storage=0;
    }

        #endregion

        #region Update methods for the filler mode

        public void UpdateFillerData(object sender, DataEventArgs e)
        {
            if (e.DataDictionary[_commands.Application_mode.PathIndex] == 2 || e.DataDictionary[_commands.Application_mode.PathIndex] == 3)  // If application mode = filler
            {             
                // Via Modbus and Jetbus IDs: 
                _maxDosingTime = Convert.ToInt32(e.DataDictionary[(_commands.Maximal_dosing_time.PathIndex)]);
                _meanValueDosingResults = Convert.ToInt32(e.DataDictionary[(_commands.Mean_value_dosing_results.PathIndex)]);
                _standardDeviation = Convert.ToInt32(e.DataDictionary[(_commands.Standard_deviation.PathIndex)]);
                _fineFlowCutOffPoint = Convert.ToInt32(e.DataDictionary[(_commands.Fine_flow_cut_off_point.PathIndex)]);
                _coarseFlowCutOffPoint = Convert.ToInt32(e.DataDictionary[(_commands.Coarse_flow_cut_off_point.PathIndex)]);

                _residualFlowTime = Convert.ToInt32(e.DataDictionary[(_commands.Residual_flow_time.PathIndex)]);
                _minimumFineFlow = Convert.ToInt32(e.DataDictionary[(_commands.Minimum_fine_flow.PathIndex)]);
                _optimizationOfCutOffPoints = Convert.ToInt32(e.DataDictionary[(_commands.Optimization.PathIndex)]);
                _maximumDosingTime = Convert.ToInt32(e.DataDictionary[(_commands.Maximal_dosing_time.PathIndex)]);
                _coarseLockoutTime = Convert.ToInt32(e.DataDictionary[(_commands.Coarse_flow_time.PathIndex)]);
                _fineLockoutTime = Convert.ToInt32(e.DataDictionary[(_commands.Fine_flow_time.PathIndex)]);
                _tareMode = Convert.ToInt32(e.DataDictionary[(_commands.Tare_mode.PathIndex)]);

                _upperToleranceLimit = Convert.ToInt32(e.DataDictionary[(_commands.Upper_tolerance_limit.PathIndex)]);
                _lowerToleranceLimit = Convert.ToInt32(e.DataDictionary[(_commands.Lower_tolerance_limit.PathIndex)]);
                _minimumStartWeight = Convert.ToInt32(e.DataDictionary[(_commands.Minimum_start_weight.PathIndex)]);
                _emptyWeight = Convert.ToInt32(e.DataDictionary[(_commands.Empty_weight_tolerance.PathIndex)]);
                _tareDelay = Convert.ToInt32(e.DataDictionary[(_commands.Tare_delay.PathIndex)]);

                _coarseFlowMonitoringTime = Convert.ToInt32(e.DataDictionary[(_commands.Coarse_flow_monitoring_time.PathIndex)]);
                _coarseFlowMonitoring = Convert.ToInt32(e.DataDictionary[(_commands.Coarse_flow_monitoring.PathIndex)]);
                _fineFlowMonitoring = Convert.ToInt32(e.DataDictionary[(_commands.Fine_flow_monitoring.PathIndex)]);
                _fineFlowMonitoringTime = Convert.ToInt32(e.DataDictionary[(_commands.Fine_flow_monitoring_time.PathIndex)]); ;

                _systematicDifference = Convert.ToInt32(e.DataDictionary[(_commands.Systematic_difference.PathIndex)]);
                _valveControl = Convert.ToInt32(e.DataDictionary[(_commands.Valve_control.PathIndex)]);
                _emptyingMode = Convert.ToInt32(e.DataDictionary[(_commands.Emptying_mode.PathIndex)]);
                _delayTimeAfterFineFlow = Convert.ToInt32(e.DataDictionary[(_commands.Delay1_dosing.PathIndex)]);
                _activationTimeAfterFineFlow = Convert.ToInt32(e.DataDictionary[(_commands.Fine_flow_phase_before_coarse_flow.PathIndex)]);
                
                _weight_storage = Convert.ToInt16(e.DataDictionary[(_commands.Storage_weight.PathIndex)]); 
                _mode_weight_storage = Convert.ToInt16(e.DataDictionary[(_commands.Storage_weight_mode.PathIndex)]);
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

        #region Set-properties for output words of filler mode

        public int ResidualFlowTime // Type : unsigned integer 16 Bit
        {
            get { return _residualFlowTime; }
            set {
                _connection.WriteArray(_commands.Residual_flow_time.PathIndex, value);
                  this._residualFlowTime = value; }
        }
        public int TargetFillingWeight // Type : signed integer 32 Bit
        {
            get { return _targetFillingWeight; }
            set { _connection.WriteArray(_commands.Reference_value_dosing.PathIndex, value);
                this._targetFillingWeight = value; }
        }
        public int CoarseFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _coarseFlowCutOffPointSet; }
            set { _connection.WriteArray(_commands.Coarse_flow_cut_off_point.PathIndex, value);
                this._coarseFlowCutOffPointSet = value; }
        }
        public int FineFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _fineFlowCutOffPointSet; }
            set { _connection.WriteArray(_commands.Fine_flow_cut_off_point.PathIndex, value);
                this._fineFlowCutOffPointSet = value; }
        }
        public int MinimumFineFlow // Type : signed integer 32 Bit
        {
            get { return _minimumFineFlow; }
            set { _connection.WriteArray(_commands.Minimum_fine_flow.PathIndex, value);
                this._minimumFineFlow = value; }
        }
        public int OptimizationOfCutOffPoints // Type : unsigned integer 8 Bit
        {
            get { return _optimizationOfCutOffPoints; }
            set { _connection.Write(_commands.Optimization.PathIndex, value);
                this._optimizationOfCutOffPoints = value; }
        }
        public int MaximumDosingTime // Type : unsigned integer 16 Bit
        {
            get { return _maximumDosingTime; }
            set { _connection.WriteArray(_commands.Maximal_dosing_time.PathIndex, value);
                this._maximumDosingTime = value; }
        }
        public int StartWithFineFlow // Type : unsigned integer 16 Bit
        {
            get { return _startWithFineFlow; }
            set { _connection.WriteArray(_commands.Run_start_dosing.PathIndex, value);
                this._startWithFineFlow = value; }
        }
        public int CoarseLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseLockoutTime; }
            set { _connection.WriteArray(_commands.Lockout_time_coarse_flow.PathIndex, value);
                this._coarseLockoutTime = value; }
        }
        public int FineLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _fineLockoutTime; }
            set { _connection.WriteArray(_commands.Lockout_time_fine_flow.PathIndex, value);
                this._fineLockoutTime = value; }
        }
        public int TareMode // Type : unsigned integer 8 Bit
        {
            get { return _tareMode; }
            set { _connection.Write(_commands.Tare_mode.PathIndex, value);
                this._tareMode = value; }
        }
        public int UpperToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _upperToleranceLimit; }
            set { _connection.WriteArray(_commands.Upper_tolerance_limit.PathIndex, value);
                this._upperToleranceLimit = value; }
        }
        public int LowerToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _lowerToleranceLimit; }
            set { _connection.WriteArray(_commands.Lower_tolerance_limit.PathIndex, value);
                this._lowerToleranceLimit = value; }
        }
        public int MinimumStartWeight // Type : signed integer 32 Bit
        {
            get { return _minimumStartWeight; }
            set { _connection.WriteArray(_commands.Minimum_start_weight.PathIndex, value);
                this._minimumStartWeight = value; }
        }
        public int EmptyWeight // Type : signed integer 32 Bit
        {
            get { return _emptyWeight; }
            set { _connection.WriteArray(_commands.Empty_weight_tolerance.PathIndex, value);
                this._emptyWeight = value; }
        }
        public int TareDelay // Type : unsigned integer 16 Bit
        {
            get { return _tareDelay; }
            set { _connection.WriteArray(_commands.Tare_delay.PathIndex, value);
                this._tareDelay = value; }
        }
        public int CoarseFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseFlowMonitoringTime; }
            set { _connection.WriteArray(_commands.Coarse_flow_monitoring_time.PathIndex, value);
                this._coarseFlowMonitoringTime = value; }
        }
        public int CoarseFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _coarseFlowMonitoring; }
            set { _connection.WriteArray(_commands.Coarse_flow_monitoring.PathIndex, value);
                this._coarseFlowMonitoring = value; }
        }
        public int FineFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _fineFlowMonitoring; }
            set { _connection.WriteArray(_commands.Fine_flow_monitoring.PathIndex, value);
                this._fineFlowMonitoring = value; }
        }
        public int FineFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _fineFlowMonitoringTime; }
            set { _connection.WriteArray(_commands.Fine_flow_monitoring_time.PathIndex, value);
                this._fineFlowMonitoringTime = value; }
        }
        public int DelayTimeAfterFineFlow  // Type : unsigned integer 8 Bit
        {
            get { return _delayTimeAfterFineFlow; }
            set { _connection.Write("", value);
                this._delayTimeAfterFineFlow = value; }
        }
        public int ActivationTimeAfterFineFlow  // Type : unsigned integer 8 Bit
        {
            get { return _activationTimeAfterFineFlow; }
            set { _connection.Write("", value);
                this._activationTimeAfterFineFlow = value; }
        }
        public int SystematicDifference // Type : unsigned integer 32 Bit
        {
            get { return _systematicDifference; }
            set { _connection.WriteArray(_commands.Systematic_difference.PathIndex, value);
                this._systematicDifference = value; }
        }
        public int DownwardsDosing  // Type : unsigned integer 8 Bit
        {
            get { return _downwardsDosing; }
            set { //_connection.Write(_connection.IDCommands.DOWNWARDS_DOSING, value); 
                this._downwardsDosing = value; }
        }
        public int ValveControl  // Type : unsigned integer 8 Bit
        {
            get { return _valveControl; }
            set { _connection.Write(_commands.Valve_control.PathIndex, value);
                this._valveControl = value; }
        }
    
        public int EmptyingMode  // Type : unsigned integer 8 Bit
        {
            get { return _emptyingMode; }
            set { _connection.Write(_commands.Emptying_mode.PathIndex, value);
                this._emptyingMode = value; }
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