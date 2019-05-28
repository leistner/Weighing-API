// <copyright file="DataFiller.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using Hbm.Weighing.API.WTX.Jet;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hbm.Weighing.API.Data
{
    /// <summary>
    /// Implementation of the interface IDataFiller for the filler mode.
    /// The class DataFiller contains the data input word and data output words for the filler mode
    /// of WTX device 120 and 110.
    /// </summary>
    public class DataFillerJet : IDataFiller
    {

        #region ================= privates for filler mode =================

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

        #region =============== constructors & destructors =================

        /// <summary>
        /// Constructor of class DataFillerJet : Initalizes values and connects 
        /// the eventhandler from Connection to the interal update method
        /// </summary>
        public DataFillerJet(INetConnection Connection)
        {
            _connection = Connection;

            _connection.UpdateData += UpdateFillerData;
            Console.WriteLine("DataFillerJet");

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
        }

        #endregion

        #region =============== Update methods - filler mode ===============

        /// <summary>
        /// Updates & converts the values from buffer (Dictionary<string,string>) 
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateFillerData(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Application_mode)) == 2 || Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Application_mode)) == 3)  // If application mode = filler
                {
                    // Via Modbus and Jetbus IDs: 
                    MaxDosingTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Maximal_dosing_time));
                    MeanValueDosingResults = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Mean_value_dosing_results));
                    StandardDeviation = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Standard_deviation));
                    FineFlowCutOffPoint = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Fine_flow_cut_off_point));
                    CoarseFlowCutOffPoint = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Coarse_flow_cut_off_point));

                    _residualFlowTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Residual_flow_time));
                    _minimumFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Minimum_fine_flow));
                    _optimizationOfCutOffPoints = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Optimization));
                    _maximumDosingTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Maximal_dosing_time));
                    _coarseLockoutTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Coarse_flow_time));
                    _fineLockoutTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Fine_flow_time));
                    _tareMode = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Tare_mode));

                    _upperToleranceLimit = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Upper_tolerance_limit));
                    _lowerToleranceLimit = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Lower_tolerance_limit));
                    _minimumStartWeight = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Minimum_start_weight));
                    _emptyWeight = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Empty_weight_tolerance));
                    _tareDelay = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Tare_delay));

                    _coarseFlowMonitoringTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Coarse_flow_monitoring_time));
                    _coarseFlowMonitoring = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Coarse_flow_monitoring));
                    _fineFlowMonitoring = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Fine_flow_monitoring));
                    _fineFlowMonitoringTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Fine_flow_monitoring_time));

                    _systematicDifference = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Systematic_difference));
                    _valveControl = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Valve_control));
                    _emptyingMode = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Emptying_mode));
                    _delayTimeAfterFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Delay1_dosing));
                    _activationTimeAfterFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Fine_flow_phase_before_coarse_flow));

                    WeightStorage = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Storage_weight));
                    ModeWeightStorage = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Storage_weight_mode));
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException in class DataFillerJet, update method");
                //_connection.CommunicationLog.Invoke(this, new LogEvent((new KeyNotFoundException()).Message));
            }
        }
        #endregion

        #region === Get/Private set properties - input words filler mode ===

        public int CoarseFlow { get; private set; }
        public int FineFlow { get; }
        public int Ready { get; }
        public int ReDosing { get; }

        public int Emptying { get; private set; }

        public int FlowError { get; private set; }

        public int Alarm { get; private set; }

        public int AdcOverUnderload { get; private set; }

        public int FillingProcessStatus { get; }
        public int NumberDosingResults { get; }
        public int DosingResult { get; }
        public int MeanValueDosingResults { get; private set; }
        public int StandardDeviation { get; private set; }
        public int TotalWeight { get; }
        public int CurrentDosingTime { get; }
        public int CurrentCoarseFlowTime { get; }
        public int CurrentFineFlowTime { get; }
        public int ToleranceErrorPlus { get; }
        public int ToleranceErrorMinus { get; }
        public int StatusInput1 { get; }
        public int GeneralScaleError { get; }
        public int FineFlowCutOffPoint { get; set; }
        public int CoarseFlowCutOffPoint { get; set; }
        public int ParameterSetProduct { get; }
        public int MaxDosingTime { get; private set; }
        public int WeightMemDay { get; }
        public int WeightMemMonth { get; }
        public int WeightMemYear { get; }
        public int WeightMemSeqNumber { get; }
        public int WeightMemGross { get; }
        public int WeightMemNet { get; }

        #endregion

        #region === Get/Private set properties - output words filler mode ==

        public int ResidualFlowTime // Type : unsigned integer 16 Bit
        {
            get
            {
                return _residualFlowTime;
            }
            set
            {
                _connection.Write(JetBusCommands.Residual_flow_time, value);
                this._residualFlowTime = value;
            }
        }
        public int TargetFillingWeight // Type : signed integer 32 Bit
        {
            get { return _targetFillingWeight; }
            set { _connection.Write(JetBusCommands.Reference_value_dosing, value);
                this._targetFillingWeight = value; }
        }
        public int CoarseFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _coarseFlowCutOffPointSet; }
            set { _connection.Write(JetBusCommands.Coarse_flow_cut_off_point, value);
                this._coarseFlowCutOffPointSet = value; }
        }
        public int FineFlowCutOffPointSet // Type : signed integer 32 Bit
        {
            get { return _fineFlowCutOffPointSet; }
            set { _connection.Write(JetBusCommands.Fine_flow_cut_off_point, value);
                this._fineFlowCutOffPointSet = value; }
        }
        public int MinimumFineFlow // Type : signed integer 32 Bit
        {
            get { return _minimumFineFlow; }
            set { _connection.Write(JetBusCommands.Minimum_fine_flow, value);
                this._minimumFineFlow = value; }
        }
        public int OptimizationOfCutOffPoints // Type : unsigned integer 8 Bit
        {
            get { return _optimizationOfCutOffPoints; }
            set { _connection.Write(JetBusCommands.Optimization, value);
                this._optimizationOfCutOffPoints = value; }
        }
        public int MaximumDosingTime // Type : unsigned integer 16 Bit
        {
            get { return _maximumDosingTime; }
            set { _connection.Write(JetBusCommands.Maximal_dosing_time, value);
                this._maximumDosingTime = value; }
        }
        public int StartWithFineFlow // Type : unsigned integer 16 Bit
        {
            get { return _startWithFineFlow; }
            set { _connection.Write(JetBusCommands.Run_start_dosing, value);
                this._startWithFineFlow = value; }
        }
        public int CoarseLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseLockoutTime; }
            set { _connection.Write(JetBusCommands.Lockout_time_coarse_flow, value);
                this._coarseLockoutTime = value; }
        }
        public int FineLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _fineLockoutTime; }
            set { _connection.Write(JetBusCommands.Lockout_time_fine_flow, value);
                this._fineLockoutTime = value; }
        }
        public int TareMode // Type : unsigned integer 8 Bit
        {
            get { return _tareMode; }
            set { _connection.Write(JetBusCommands.Tare_mode, value);
                this._tareMode = value; }
        }
        public int UpperToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _upperToleranceLimit; }
            set { _connection.Write(JetBusCommands.Upper_tolerance_limit, value);
                this._upperToleranceLimit = value; }
        }
        public int LowerToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _lowerToleranceLimit; }
            set { _connection.Write(JetBusCommands.Lower_tolerance_limit, value);
                this._lowerToleranceLimit = value; }
        }
        public int MinimumStartWeight // Type : signed integer 32 Bit
        {
            get { return _minimumStartWeight; }
            set { _connection.Write(JetBusCommands.Minimum_start_weight, value);
                this._minimumStartWeight = value; }
        }
        public int EmptyWeight // Type : signed integer 32 Bit
        {
            get { return _emptyWeight; }
            set { _connection.Write(JetBusCommands.Empty_weight_tolerance, value);
                this._emptyWeight = value; }
        }
        public int TareDelay // Type : unsigned integer 16 Bit
        {
            get { return _tareDelay; }
            set { _connection.Write(JetBusCommands.Tare_delay, value);
                this._tareDelay = value; }
        }
        public int CoarseFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseFlowMonitoringTime; }
            set { _connection.Write(JetBusCommands.Coarse_flow_monitoring_time, value);
                this._coarseFlowMonitoringTime = value; }
        }
        public int CoarseFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _coarseFlowMonitoring; }
            set { _connection.Write(JetBusCommands.Coarse_flow_monitoring, value);
                this._coarseFlowMonitoring = value; }
        }
        public int FineFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _fineFlowMonitoring; }
            set { _connection.Write(JetBusCommands.Fine_flow_monitoring, value);
                this._fineFlowMonitoring = value; }
        }
        public int FineFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _fineFlowMonitoringTime; }
            set { _connection.Write(JetBusCommands.Fine_flow_monitoring_time, value);
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
            set { _connection.Write(JetBusCommands.Systematic_difference, value);
                this._systematicDifference = value; }
        }
        public int DownwardsDosing  // Type : unsigned integer 8 Bit
        {
            get { return _downwardsDosing; }
            set
            {
                _connection.Write(JetBusCommands.DOWNWARDS_DOSING, value); 
                this._downwardsDosing = value;
            }
        }
        public int ValveControl  // Type : unsigned integer 8 Bit
        {
            get { return _valveControl; }
            set { _connection.Write(JetBusCommands.Valve_control, value);
                this._valveControl = value; }
        }
    
        public int EmptyingMode  // Type : unsigned integer 8 Bit
        {
            get { return _emptyingMode; }
            set { _connection.Write(JetBusCommands.Emptying_mode, value);
                this._emptyingMode = value; }
        }
        public int WeightStorage { get; set; }
        public int ModeWeightStorage { get; set; }

        #endregion

    }
}