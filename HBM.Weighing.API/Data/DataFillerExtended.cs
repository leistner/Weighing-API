using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    public class DataFillerExtended : DataFiller, IDataFillerExtended
    {
        #region privates

        private ushort[] _data;

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

        #endregion

        #region constructor

        public DataFillerExtended()
        {
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
         }

        #endregion

        #region update method for the filler extended data

        public void UpdateFillerExtendedDataModbus(ushort[] _data)
        {
            this._data = _data;
        }

        public void UpdateFillerExtendedDataJet(Dictionary<string, int> _data)
        {
            // Example for data update:
            //_maxDosingTime = _data[JetBusCommands.MAXIMUM_DOSING_TIME];

        }

        #endregion

        #region Properties for the filler extended data

        public int ResidualFlowTime
        {
            get { return _residualFlowTime; }
            set { this._residualFlowTime = value; }
        }
        public int TargetFillingWeight
        {
            get { return _targetFillingWeight; }
            set { this._targetFillingWeight = value; }
        }
        public int CoarseFlowCutOffPointSet
        {
            get { return _coarseFlowCutOffPointSet; }
            set { this._coarseFlowCutOffPointSet = value; }
        }
        public int FineFlowCutOffPointSet
        {
            get { return _fineFlowCutOffPointSet; }
            set { this._fineFlowCutOffPointSet = value; }
        }
        public int MinimumFineFlow
        {
            get { return _minimumFineFlow; }
            set { this._minimumFineFlow = value; }
        }
        public int OptimizationOfCutOffPoints
        {
            get { return _optimizationOfCutOffPoints; }
            set { this._optimizationOfCutOffPoints = value; }
        }
        public int MaximumDosingTime
        {
            get { return _maximumDosingTime; }
            set { this._maximumDosingTime = value; }
        }
        public int StartWithFineFlow
        {
            get { return _startWithFineFlow; }
            set { this._startWithFineFlow = value; }
        }
        public int CoarseLockoutTime
        {
            get { return _coarseLockoutTime; }
            set { this._coarseLockoutTime = value; }
        }
        public int FineLockoutTime
        {
            get { return _fineLockoutTime; }
            set { this._fineLockoutTime = value; }
        }
        public int TareMode
        {
            get { return _tareMode; }
            set { this._tareMode = value; }
        }
        public int UpperToleranceLimit
        {
            get { return _upperToleranceLimit; }
            set { this._upperToleranceLimit = value; }
        }
        public int LowerToleranceLimit
        {
            get { return _lowerToleranceLimit; }
            set { this._lowerToleranceLimit = value; }
        }
        public int MinimumStartWeight
        {
            get { return _minimumStartWeight; }
            set { this._minimumStartWeight = value; }
        }
        public int EmptyWeight
        {
            get { return _emptyWeight; }
            set { this._emptyWeight = value; }
        }
        public int TareDelay
        {
            get { return _tareDelay; }
            set { this._tareDelay = value; }
        }
        public int CoarseFlowMonitoringTime
        {
            get { return _coarseFlowMonitoringTime; }
            set { this._coarseFlowMonitoringTime = value; }
        }
        public int CoarseFlowMonitoring
        {
            get { return _coarseFlowMonitoring; }
            set { this._coarseFlowMonitoring = value; }
        }
        public int FineFlowMonitoring
        {
            get { return _fineFlowMonitoring; }
            set { this._fineFlowMonitoring = value; }
        }
        public int FineFlowMonitoringTime
        {
            get { return _fineFlowMonitoringTime; }
            set { this._fineFlowMonitoringTime = value; }
        }
        public int DelayTimeAfterFineFlow
        {
            get { return _delayTimeAfterFineFlow; }
            set { this._delayTimeAfterFineFlow = value; }
        }
        public int ActivationTimeAfterFineFlow
        {
            get { return _activationTimeAfterFineFlow; }
            set { this._activationTimeAfterFineFlow = value; }
        }
        public int SystematicDifference
        {
            get { return _systematicDifference; }
            set { this._systematicDifference = value; }
        }
        public int DownwardsDosing
        {
            get { return _downwardsDosing; }
            set { this._downwardsDosing = value; }
        }
        public int ValveControl
        {
            get { return _valveControl; }
            set { this._valveControl = value; }
        }
        public int EmptyingMode
        {
            get { return _emptyingMode; }
            set { this._emptyingMode = value; }
        }
        #endregion

    }
}