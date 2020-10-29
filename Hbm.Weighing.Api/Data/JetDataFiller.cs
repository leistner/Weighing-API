// <copyright file="DataFiller.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace Hbm.Weighing.Api.Data
{
    using Hbm.Weighing.Api.WTX.Jet;
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Implementation of the interface IDataFiller for the filler mode.
    /// The class DataFillerJet contains the data input word and data output words for the filler mode
    /// of WTX device 120 and 110 via Jetbus.
    /// </summary>
    public class JetDataFiller : IDataFiller
    {

        #region ==================== constants & fields ====================
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
        public JetDataFiller(INetConnection Connection)
        {
            _connection = Connection;
            _connection.UpdateData += UpdateFillerData;
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

        #region ==================== events & delegates ====================

        /// <summary>
        /// Updates and converts the values from buffer 
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateFillerData(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.IMDApplicationMode)) == 2 || Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.IMDApplicationMode)) == 3)  // If application mode = filler
                {
                    MaxDosingTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.MDTMaxFillingTime));
                    FillingResultMeanValue = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.SDMFillingResultMeanValue));
                    FillingResultStandardDeviation = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.SDSFillingResultStandardDeviation));
                    FineFlowCutOffPoint = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FFDFineFlowDisconnect));
                    CoarseFlowCutOffPoint = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CFDCoarseFlowDisconnect));
                    _residualFlowTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.RFTResidualFlowTime));
                    _minimumFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FFMMinimumFineFlow));
                    _optimizationOfCutOffPoints = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.OSNOptimization));
                    _maximumDosingTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.MDTMaxFillingTime));
                    _coarseLockoutTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CFTCoarseFlowTime));
                    _fineLockoutTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FFTFineFlowTime));
                    _tareMode = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.TMDTareMode));
                    _upperToleranceLimit = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.UTLUpperToleranceLimit));
                    _lowerToleranceLimit = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.LTLLowerToleranceLimit));
                    _minimumStartWeight = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.MSWMinimumStartWeight));
                    _emptyWeight = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.EWTEmptyWeight));
                    _tareDelay = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.TADTareDelay));
                    _coarseFlowMonitoringTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CBTCoarseFlowMonitoringTime));
                    _coarseFlowMonitoring = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CBKCoarseFlowMonitoring));
                    _fineFlowMonitoring = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FBKFineFlowMonitoring));
                    _fineFlowMonitoringTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FBTFineFlowMonitoringTime));
                    _systematicDifference = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.SYDSystematicDifference));
                    _valveControl = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.VCTValveControl));
                    _emptyingMode = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.EMDEmptyingMode));
                    _delayTimeAfterFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.DL1DosingDelay1));
                    _activationTimeAfterFineFlow = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.FFLFirstFineFlow));
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException in class DataFillerJet, update method");
            }
        }
        #endregion

        #region ======================== properties ========================

        // Get/Private set properties - input words filler mode:
        public int CoarseFlow { get; private set; }
        public int FineFlow { get; }
        public int Ready { get; }
        public int ReDosing { get; }

        public int Emptying { get; private set; }

        public int FlowError { get; private set; }

        public int Alarm { get; private set; }

        public int AdcOverUnderload { get; private set; }

        public int FillingProcessStatus { get; }
        public int FillingResultCount { get; }
        public int FillingResult { get; }
        public int FillingResultMeanValue { get; private set; }
        public int FillingResultStandardDeviation { get; private set; }
        public int FillingResultTotalSum { get; }
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

        // Get/Private set properties - output words filler mode:

        public int ResidualFlowTime // Type : unsigned integer 16 Bit
        {
            get
            {
                return _residualFlowTime;
            }
            set
            {
                _connection.WriteInteger(JetBusCommands.RFTResidualFlowTime, value);
                this._residualFlowTime = value;
            }
        }
        public int TargetFillingWeight // Type : signed integer 32 Bit
        {
            get { return _targetFillingWeight; }
            set { _connection.WriteInteger(JetBusCommands.FWTFillingTargetWeight, value);
                this._targetFillingWeight = value; }
        }
        public int CoarseFlowCutOffLevel // Type : signed integer 32 Bit
        {
            get { return _coarseFlowCutOffPointSet; }
            set { _connection.WriteInteger(JetBusCommands.CFDCoarseFlowDisconnect, value);
                this._coarseFlowCutOffPointSet = value; }
        }
        public int FineFlowCutOffLevel // Type : signed integer 32 Bit
        {
            get { return _fineFlowCutOffPointSet; }
            set { _connection.WriteInteger(JetBusCommands.FFDFineFlowDisconnect, value);
                this._fineFlowCutOffPointSet = value; }
        }
        public int MinimumFineFlow // Type : signed integer 32 Bit
        {
            get { return _minimumFineFlow; }
            set { _connection.WriteInteger(JetBusCommands.FFMMinimumFineFlow, value);
                this._minimumFineFlow = value; }
        }
        public int OptimizationMode // Type : unsigned integer 8 Bit
        {
            get { return _optimizationOfCutOffPoints; }
            set { _connection.WriteInteger(JetBusCommands.OSNOptimization, value);
                this._optimizationOfCutOffPoints = value; }
        }
        public int MaxFillingTime // Type : unsigned integer 16 Bit
        {
            get { return _maximumDosingTime; }
            set { _connection.WriteInteger(JetBusCommands.MDTMaxFillingTime, value);
                this._maximumDosingTime = value; }
        }
        public int StartWithFineFlow // Type : unsigned integer 16 Bit
        {
            get { return _startWithFineFlow; }
            set { _connection.WriteInteger(JetBusCommands.RUNStartFilling, value);
                this._startWithFineFlow = value; }
        }
        public int CoarseLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseLockoutTime; }
            set { _connection.WriteInteger(JetBusCommands.LTCLockoutTimeCoarseFlow, value);
                this._coarseLockoutTime = value; }
        }
        public int FineLockoutTime // Type : unsigned integer 16 Bit
        {
            get { return _fineLockoutTime; }
            set { _connection.WriteInteger(JetBusCommands.LTFLockoutTimeFineFlow, value);
                this._fineLockoutTime = value; }
        }
        public int TareMode // Type : unsigned integer 8 Bit
        {
            get { return _tareMode; }
            set { _connection.WriteInteger(JetBusCommands.TMDTareMode, value);
                this._tareMode = value; }
        }
        public int UpperToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _upperToleranceLimit; }
            set { _connection.WriteInteger(JetBusCommands.UTLUpperToleranceLimit, value);
                this._upperToleranceLimit = value; }
        }
        public int LowerToleranceLimit // Type : signed integer 32 Bit
        {
            get { return _lowerToleranceLimit; }
            set { _connection.WriteInteger(JetBusCommands.LTLLowerToleranceLimit, value);
                this._lowerToleranceLimit = value; }
        }
        public int MinimumStartWeight // Type : signed integer 32 Bit
        {
            get { return _minimumStartWeight; }
            set { _connection.WriteInteger(JetBusCommands.MSWMinimumStartWeight, value);
                this._minimumStartWeight = value; }
        }
        public int EmptyWeight // Type : signed integer 32 Bit
        {
            get { return _emptyWeight; }
            set { _connection.WriteInteger(JetBusCommands.EWTEmptyWeight, value);
                this._emptyWeight = value; }
        }
        public int TareDelay // Type : unsigned integer 16 Bit
        {
            get { return _tareDelay; }
            set { _connection.WriteInteger(JetBusCommands.TADTareDelay, value);
                this._tareDelay = value; }
        }
        public int CoarseFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _coarseFlowMonitoringTime; }
            set { _connection.WriteInteger(JetBusCommands.CBTCoarseFlowMonitoringTime, value);
                this._coarseFlowMonitoringTime = value; }
        }
        public int CoarseFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _coarseFlowMonitoring; }
            set { _connection.WriteInteger(JetBusCommands.CBKCoarseFlowMonitoring, value);
                this._coarseFlowMonitoring = value; }
        }
        public int FineFlowMonitoring  // Type : unsigned integer 32 Bit
        {
            get { return _fineFlowMonitoring; }
            set { _connection.WriteInteger(JetBusCommands.FBKFineFlowMonitoring, value);
                this._fineFlowMonitoring = value; }
        }
        public int FineFlowMonitoringTime // Type : unsigned integer 16 Bit
        {
            get { return _fineFlowMonitoringTime; }
            set { _connection.WriteInteger(JetBusCommands.FBTFineFlowMonitoringTime, value);
                this._fineFlowMonitoringTime = value; }
        }
        public int DelayTimeAfterFilling  // Type : unsigned integer 8 Bit
        {
            get { return _delayTimeAfterFineFlow; }
            set { _connection.WriteInteger("", value);
                this._delayTimeAfterFineFlow = value; }
        }
        public int ActivationTimeAfterFilling  // Type : unsigned integer 8 Bit
        {
            get { return _activationTimeAfterFineFlow; }
            set { _connection.WriteInteger("", value);
                this._activationTimeAfterFineFlow = value; }
        }
        public int SystematicDifference // Type : unsigned integer 32 Bit
        {
            get { return _systematicDifference; }
            set { _connection.WriteInteger(JetBusCommands.SYDSystematicDifference, value);
                this._systematicDifference = value; }
        }
        public int FillingMode  // Type : unsigned integer 8 Bit
        {
            get { return _downwardsDosing; }
            set
            {
                _connection.WriteInteger(JetBusCommands.DMDFillingMode, value); 
                this._downwardsDosing = value;
            }
        }
        public int ValveControl  // Type : unsigned integer 8 Bit
        {
            get { return _valveControl; }
            set { _connection.WriteInteger(JetBusCommands.VCTValveControl, value);
                this._valveControl = value; }
        }
    
        public int EmptyingMode  // Type : unsigned integer 8 Bit
        {
            get { return _emptyingMode; }
            set { _connection.WriteInteger(JetBusCommands.EMDEmptyingMode, value);
                this._emptyingMode = value; }
        }
        #endregion

        #region ================ public & internal methods ================= 
        public void StartFilling()
        {
            _connection.WriteInteger(JetBusCommands.RUNStartFilling, 0);
        }

        public void BreakFilling()
        {
            _connection.WriteInteger(JetBusCommands.CSNClearFillingResult, 0);
        }

        public void ClearFillingResult()
        {
            _connection.WriteInteger(JetBusCommands.CSNClearFillingResult, 0);
        }
        #endregion
    }
}