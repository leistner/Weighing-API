using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    public class DataFiller : DataStandard, IDataFiller
    {
        #region privates for filler mode

        private ushort[] _data;

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

        private int _fillerWeightMemoryDay;
        private int _fillerWeightMemoryMonth;
        private int _fillerWeightMemoryYear;
        private int _fillerWeightMemorySeqNumber;
        private int _fillerWeightMemoryGross;
        private int _fillerWeightMemoryNet;

       //Output words - Update neccessary?

        private int _manualTareValue;
        private int _limitValue1Input;
        private int _limitValue1Mode;

        private int _limitValue1ActivationLevelLowerBandLimit;
        private int _limitValue1HysteresisBandHeight;
        private int _limitValue2Source;
        private int _limitValue2Mode;

        private int _limitValue2ActivationLevelLowerBandLimit;
        private int _limitValue2HysteresisBandHeight;
        private int _limitValue3Source;
        private int _limitValue3Mode;

        private int _limitValue3ActivationLevelLowerBandLimit;
        private int _limitValue3HysteresisBandHeight;
        private int _limitValue4Source;

        private int _limitValue4Mode;
        private int _limitValue4ActivationLevelLowerBandLimit;
        private int _limitValue4HysteresisBandHeight;


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

        #region contructor

        public DataFiller() : base()
        {
            _coarseFlow=0;
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
            _manualTareValue=0;
            _limitValue1Input=0;
            _limitValue1Mode=0;

            _limitValue1ActivationLevelLowerBandLimit=0;
            _limitValue1HysteresisBandHeight=0;
            _limitValue2Source=0;
            _limitValue2Mode=0;

            _limitValue2ActivationLevelLowerBandLimit=0;
            _limitValue2HysteresisBandHeight=0;
            _limitValue3Source=0;
            _limitValue3Mode=0;

            _limitValue3ActivationLevelLowerBandLimit=0;
            _limitValue3HysteresisBandHeight=0;
            _limitValue4Source=0;

            _limitValue4Mode=0;
            _limitValue4ActivationLevelLowerBandLimit=0;
            _limitValue4HysteresisBandHeight=0;
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
       }

        #endregion

        #region Update methods for the filler mode

        public void UpdateFillerData(ushort[] dataParam)
        {
            this._data = dataParam;

            _coarseFlow = (_data[8] & 0x1);
            _fineFlow   = ((_data[8] & 0x2) >> 1);
            _ready      = ((_data[8] & 0x4) >> 2);
            _reDosing   = ((_data[8] & 0x8) >> 3);

            _emptying  = ((_data[8] & 0x10) >> 4);
            _flowError = ((_data[8] & 0x20) >> 5);
            _alarm     = ((_data[8] & 0x40) >> 6);
            _adcOverUnderload = ((_data[8] & 0x80) >> 7);

            _maxDosingTime          = ((_data[8] & 0x100) >> 8);
            _legalForTradeOperation = 0;
            _toleranceErrorPlus     = ((_data[8] & 0x400) >> 10);
            _toleranceErrorMinus    = ((_data[8] & 0x800) >> 11);

            _statusInput1      = ((_data[8] & 0x4000) >> 14);
            _generalScaleError = ((_data[8] & 0x8000) >> 15);

            _fillingProcessStatus   = _data[9];
            _numberDosingResults    = _data[11];
            _dosingResult           = _data[12];
            _meanValueDosingResults = _data[14];

            _standardDeviation     = _data[16];
            _totalWeight           = _data[18];
            _fineFlowCutOffPoint   = _data[20];
            _coarseFlowCutOffPoint = _data[22];

            _currentDosingTime     = _data[24];
            _currentCoarseFlowTime = _data[25];
            _currentFineFlowTime   = _data[26];      
            _parameterSetProduct   = _data[27];

            _fillerWeightMemoryDay       = _data[32];
            _fillerWeightMemoryMonth     = _data[33];
            _fillerWeightMemoryYear      = _data[34];
            _fillerWeightMemorySeqNumber = _data[35];
            _fillerWeightMemoryGross     = _data[36];
            _fillerWeightMemoryNet       = _data[37];

            //Output words:
            /*
            _manualTareValue;
            _limitValue1Input;
            _limitValue1Mode;

            _limitValue1ActivationLevelLowerBandLimit;
            _limitValue1HysteresisBandHeight;
            _limitValue2Source;
            _limitValue2Mode;

            _limitValue2ActivationLevelLowerBandLimit;
            _limitValue2HysteresisBandHeight;
            _limitValue3Source;
            _limitValue3Mode;

            _limitValue3ActivationLevelLowerBandLimit;
            _limitValue3HysteresisBandHeight;
            _limitValue4Source;

            _limitValue4Mode;
            _limitValue4ActivationLevelLowerBandLimit;
            _limitValue4HysteresisBandHeight;


            _residualFlowTime;
            _targetFillingWeight;
            _coarseFlowCutOffPointSet;
            _fineFlowCutOffPointSet;

            _minimumFineFlow;
            _optimizationOfCutOffPoints;
            _maximumDosingTime;
            _startWithFineFlow;

            _coarseLockoutTime;
            _fineLockoutTime;
            _tareMode;
            _upperToleranceLimit;

            _lowerToleranceLimit;
            _minimumStartWeight;
            _emptyWeight;
            _tareDelay;

            _coarseFlowMonitoringTime;
            _coarseFlowMonitoring;
            _fineFlowMonitoring;
            _fineFlowMonitoringTime;

            _delayTimeAfterFineFlow;
            _activationTimeAfterFineFlow;
            _systematicDifference;
            _downwardsDosing;

            _valveControl;
            _emptyingMode;
            */
        }

    #endregion

        #region properties for filler mode

    public int CoarseFlow
        {
            get { return _coarseFlow; }
            set { this._coarseFlow = value; }
        }
        public int FineFlow
        {
            get { return _fineFlow; }
            set { this._fineFlow = value; }
        }
        public int Ready
        {
            get { return _ready; }
            set { this._ready = value; }
        }
        public int ReDosing
        {
            get { return _reDosing; }
            set { this._reDosing = value; }
        }
        public int Emptying
        {
            get { return _emptying; }
            set { this._emptying = value; }
        }
        public int FlowError
        {
            get { return _flowError; }
            set { this._flowError = value; }
        }
        public int Alarm
        {
            get { return _alarm; }
            set { this._alarm = value; }
        }
        public int AdcOverUnderload
        {
            get { return _adcOverUnderload; }
            set { this._adcOverUnderload = value; }
        }
        public int FillingProcessStatus
        {
            get { return _fillingProcessStatus; }
            set { this._fillingProcessStatus = value; }
        }
        public int NumberDosingResults
        {
            get { return _numberDosingResults; }
            set { this._numberDosingResults = value; }
        }
        public int DosingResult
        {
            get { return _dosingResult; }
            set { this._dosingResult = value; }
        }
        public int MeanValueDosingResults
        {
            get { return _meanValueDosingResults; }
            set { this._meanValueDosingResults = value; }
        }
        public int StandardDeviation
        {
            get { return _standardDeviation; }
            set { this._standardDeviation = value; }
        }
        public int TotalWeight
        {
            get { return _totalWeight; }
            set { this._totalWeight = value; }
        }
        public int CurrentDosingTime
        {
            get { return _currentDosingTime; }
            set { this._currentDosingTime = value; }
        }
        public int CurrentCoarseFlowTime
        {
            get { return _currentCoarseFlowTime; }
            set { this._currentCoarseFlowTime = value; }
        }
        public int CurrentFineFlowTime
        {
            get { return _currentFineFlowTime; }
            set { this._currentFineFlowTime = value; }
        }
        public int ToleranceErrorPlus
        {
            get { return _toleranceErrorPlus; }
            set { this._toleranceErrorPlus = value; }
        }
        public int ToleranceErrorMinus
        {
            get { return _toleranceErrorMinus; }
            set { this._toleranceErrorMinus = value; }
        }
        public int StatusInput1
        {
            get { return _statusInput1; }
            set { this._statusInput1 = value; }
        }
        public int GeneralScaleError
        {
            get { return _generalScaleError; }
            set { this._generalScaleError = value; }
        }
        public int ManualTareValue
        {
            get { return _manualTareValue; }
            set { this._manualTareValue = value; }
        }
        public int LimitValue1Input
        {
            get { return _limitValue1Input; }
            set { this._limitValue1Input = value; }
        }
        public int LimitValue1Mode
        {
            get { return _limitValue1Mode; }
            set { this._limitValue1Mode = value; }
        }
        public int LimitValue1ActivationLevelLowerBandLimit
        {
            get { return _limitValue1ActivationLevelLowerBandLimit; }
            set { this._limitValue1ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue1HysteresisBandHeight
        {
            get { return _limitValue1HysteresisBandHeight; }
            set { this._limitValue1HysteresisBandHeight = value; }
        }
        public int LimitValue2Source
        {
            get { return _limitValue2Source; }
            set { this._limitValue2Source = value; }
        }
        public int LimitValue2Mode
        {
            get { return _limitValue2Mode; }
            set { this._limitValue2Mode = value; }
        }
        public int LimitValue2ActivationLevelLowerBandLimit
        {
            get { return _limitValue2ActivationLevelLowerBandLimit; }
            set { this._limitValue2ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue2HysteresisBandHeight
        {
            get { return _limitValue2HysteresisBandHeight; }
            set { this._limitValue2HysteresisBandHeight = value; }
        }
        public int LimitValue3Source
        {
            get { return _limitValue3Source; }
            set { this._limitValue3Source = value; }
        }
        public int LimitValue3Mode
        {
            get { return _limitValue3Mode; }
            set { this._limitValue3Mode = value; }
        }
        public int LimitValue3ActivationLevelLowerBandLimit
        {
            get { return _limitValue3ActivationLevelLowerBandLimit; }
            set { this._limitValue3ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue3HysteresisBandHeight
        {
            get { return _limitValue3HysteresisBandHeight; }
            set { this._limitValue3HysteresisBandHeight = value; }
        }
        public int LimitValue4Source
        {
            get { return _limitValue4Source; }
            set { this._limitValue4Source = value; }
        }
        public int LimitValue4Mode
        {
            get { return _limitValue4Mode; }
            set { this._limitValue4Mode = value; }
        }
        public int LimitValue4ActivationLevelLowerBandLimit
        {
            get { return _limitValue4ActivationLevelLowerBandLimit; }
            set { this._limitValue4ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue4HysteresisBandHeight
        {
            get { return _limitValue4HysteresisBandHeight; }
            set { this._limitValue4HysteresisBandHeight = value; }
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
            set { this._parameterSetProduct = value; }
        }
        public int MaxDosingTime
        {
            get { return _maxDosingTime; }
            set { this._maxDosingTime = value; }
        }
        public int LegalForTradeOperation
        {
            get { return _legalForTradeOperation; }
            set { this._legalForTradeOperation = value; }
        }
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