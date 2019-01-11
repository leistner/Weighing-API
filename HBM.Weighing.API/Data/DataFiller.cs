using HBM.Weighing.API.WTX.Jet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    public class DataFiller : IDataFiller
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

        private int _weightMemoryDay;
        private int _weightMemoryMonth;
        private int _weightMemoryYear;
        private int _weightMemorySeqNumber;
        private int _weightMemoryGross;
        private int _weightMemoryNet;
        
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

        public void UpdateFillerDataModbus(ushort[] dataParam)
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
            _legalForTradeOperation = ((_data[8] & 0x200) >> 9);
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

            _weightMemoryDay       = _data[32];
            _weightMemoryMonth     = _data[33];
            _weightMemoryYear      = _data[34];
            _weightMemorySeqNumber = _data[35];
            _weightMemoryGross     = _data[36];
            _weightMemoryNet       = _data[37];

            //Output words:
            /*
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

        public void UpdateFillerDataJet(Dictionary<string, int> _data)
        {
            _adcOverUnderload = 0;       // undefined ID
            _legalForTradeOperation = 0; // undefined ID
            _statusInput1 = 0;           // undefined ID
            _generalScaleError = 0;      // undefined ID
            _coarseFlow = 0;             // undefined ID
            _fineFlow = 0;               // undefined ID
            _ready = 0;                  // undefined ID
            _reDosing = 0;               // undefined ID
            _emptying = 0;               // undefined ID
            _flowError = 0;              // undefined ID
            _alarm = 0;                  // undefined ID
            _toleranceErrorPlus = 0;     // undefined ID
            _toleranceErrorMinus = 0;    // undefined ID
            _currentDosingTime = 0;      // undefined ID
            _currentCoarseFlowTime = 0;  // undefined ID
            _currentFineFlowTime = 0;    // undefined ID

            _parameterSetProduct = 0;    // undefined ID 
            _downwardsDosing = 0;        // undefined ID
            _legalForTradeOperation = 0; // undefined ID

            // Commented out because of undefined ID's:
            /*
            _maxDosingTime = _data[JetBusCommands.MAXIMUM_DOSING_TIME];
            _meanValueDosingResults = _data[JetBusCommands.MEAN_VALUE_DOSING_RESULTS];
            _standardDeviation = _data[JetBusCommands.STANDARD_DEVIATION];
            _fineFlowCutOffPoint = _data[JetBusCommands.FINE_FLOW_CUT_OFF_POINT];
            _coarseFlowCutOffPoint = _data[JetBusCommands.COARSE_FLOW_CUT_OFF_POINT];
            _residualFlowTime = _data[JetBusCommands.RESIDUAL_FLOW_TIME];
            _minimumFineFlow = _data[JetBusCommands.MINIMUM_FINE_FLOW];
            _optimizationOfCutOffPoints = _data[JetBusCommands.OPTIMIZATION];
            _maximumDosingTime = _data[JetBusCommands.STATUS_DIGITAL_OUTPUT_3];
            _coarseLockoutTime = _data[JetBusCommands.COARSE_FLOW_TIME];
            _fineLockoutTime = _data[JetBusCommands.FINE_FLOW_TIME];
            _tareMode = _data[JetBusCommands.TARE_MODE];
            _upperToleranceLimit = _data[JetBusCommands.UPPER_TOLERANCE_LIMIT];
            _lowerToleranceLimit = _data[JetBusCommands.LOWER_TOLERANCE_LOMIT];
            _minimumStartWeight = _data[JetBusCommands.MINIMUM_START_WEIGHT];
            _emptyWeight = _data[JetBusCommands.EMPTY_WEIGHT];
            _tareDelay = _data[JetBusCommands.TARE_DELAY];
            _coarseFlowMonitoringTime = _data[JetBusCommands.COARSE_FLOW_MONITORING_TIME];
            _coarseFlowMonitoring = _data[JetBusCommands.COARSE_FLOW_MONITORING];
            _fineFlowMonitoring = _data[JetBusCommands.FINE_FLOW_MONITORING];
            _fineFlowMonitoringTime = _data[JetBusCommands.FINE_FLOW_MONITORING_TIME];
            _systematicDifference = _data[JetBusCommands.SYSTEMATIC_DIFFERENCE];
            _valveControl = _data[JetBusCommands.VALVE_CONTROL];
            _emptyingMode = _data[JetBusCommands.EMPTYING_MODE];
            _delayTimeAfterFineFlow = _data[JetBusCommands.DELAY1_DOSING];
            _activationTimeAfterFineFlow = _data[JetBusCommands.FINEFLOW_PHASE_BEFORE_COARSEFLOW];
            */

            _totalWeight = 0;             // undefined ID
            _targetFillingWeight = 0;     // undefined ID
            _coarseFlowCutOffPointSet = 0;// undefined ID
            _fineFlowCutOffPointSet = 0;  // undefined ID
            _startWithFineFlow = 0;       // undefined ID
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