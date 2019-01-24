// <copyright file="DataFillerExtended.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// Implementation of the interface IDataFillerExtended for the filler extended mode.
    /// </summary>
    public class DataFillerExtended : DataFiller, IDataFillerExtended
    {
        #region privates

        private ushort[] _data;

        private int _errorRegister;
        private int _saveAllParameters;
        private int _restoreAllDefaultParameters;
        private int _vendorID;
        private int _productCode;
        private int _serialNumber;
        private int _implementedProfileSpecification;
        private int _lcCapability;
        private int _weighingDevice1UnitPrefixOutputParameter;
        private int _weighingDevice1WeightStep;
        private int _alarms;
        private int _weighingDevice1OutputWeight;
        private int _weighingDevice1Setting;
        private int _localGravityFactor;
        private int _scaleFilterSetup;
        private int _dataSampleRate;
        private int _filterOrderCriticallyDamped;
        private int _cutOffFrequencyCriticallyDamped;
        private int _filterOrderButterworth;
        private int _cutOffFrequencyButterWorth;
        private int _filterOrderBessel;
        private int _cutOffFrequencyBessel;
        private int _scaleSupplyNominalVoltage;
        private int _scaleSupplyMinimumVoltage;
        private int _scaleSupplyMaximumVoltage;
        private int _scaleAccuracyClass;
        private int _scaleMinimumDeadLoad;
        private int _scaleMaximumCapacity;
        private int _scaleMaximumNumberVerificationInterval;
        private int _scaleApportionmentFactor;
        private int _scaleSafeLoadLimit;
        private int _scaleOperationNominalTemperature;
        private int _scaleOperationMinimumTemperature;
        private int _scaleOperationMaximumTemperature;
        private int _scaleRelativeMinimumLoadCellVerficationInterval;
        private int _intervalRangeControl;
        private int _multiLimit1;
        private int _multiLimit2;
        private int _oimlCertificationInformation;
        private int _ntepCertificationInformation;
        private int _maximumZeroingTime;
        private int _maximumPeakValueGross;
        private int _minimumPeakValueGross;
        private int _maximumPeakValue;
        private int _minimumPeakValue;
        private int _weightMovingDetection;
        private int _deviceAddress;
        private int _hardwareVersion;
        private int _identification;
        private int _limitValueMonitoringLIV11;
        private int _signalSourceLIV12;
        private int _switchOnLevelLIV13;
        private int _switchOffLevelLIV14;
        private int _limitValueMonitoringLIV21;
        private int _signalSourceLIV22;
        private int _switchOnLevelLIV23;
        private int _switchOffLevelLIV24;

        private int _limitValueMonitoringLIV31;
        private int _signalSourceLIV32;
        private int _switchOnLevelLIV33;
        private int _switchOffLevelLIV34;
        private int _limitValueMonitoringLIV41;
        private int _signalSourceLIV42;
        private int _switchOnLevelLIV43;
        private int _switchOffLevelLIV44;
        private int _outputScale;
        private int _firmwareDate;
        private int _resetTrigger;
        private int _stateDigital_IO_Extended;
        private int _softwareIdentification;
        private int _softwareVersion;
        private int _dateTime;

        private int _breakDosing;
        private int _deleteDosingResult;
        private int _materialStreamLastDosing;
        private int _sum;
        private int _specialDosingFunctions;
        private int _dischargeTime;
        private int _exceedingWeightBreak;
        private int _delay1Dosing;
        private int _delay2Dosing;
        private int _emptyWeightTolerance;
        private int _residualFlowDosingCycle;


        #endregion

        #region constructor

        public DataFillerExtended()
        {
            _errorRegister=0;
            _saveAllParameters=0;
            _restoreAllDefaultParameters=0;
            _vendorID=0;
            _productCode=0;
            _serialNumber=0;
            _implementedProfileSpecification=0;
            _lcCapability=0;
            _weighingDevice1UnitPrefixOutputParameter=0;
            _weighingDevice1WeightStep=0;
            _alarms=0;
            _weighingDevice1OutputWeight=0;
            _weighingDevice1Setting=0;
            _localGravityFactor=0;
            _scaleFilterSetup=0;
            _dataSampleRate=0;
            _filterOrderCriticallyDamped=0;
            _cutOffFrequencyCriticallyDamped=0;
            _filterOrderButterworth=0;
            _cutOffFrequencyButterWorth=0;
            _filterOrderBessel=0;
            _cutOffFrequencyBessel=0;
            _scaleSupplyNominalVoltage=0;
            _scaleSupplyMinimumVoltage=0;
            _scaleSupplyMaximumVoltage=0;
            _scaleAccuracyClass=0;
            _scaleMinimumDeadLoad=0;
            _scaleMaximumCapacity=0;
            _scaleMaximumNumberVerificationInterval=0;
            _scaleApportionmentFactor=0;
            _scaleSafeLoadLimit=0;
            _scaleOperationNominalTemperature=0;
            _scaleOperationMinimumTemperature=0;
            _scaleOperationMaximumTemperature=0;
            _scaleRelativeMinimumLoadCellVerficationInterval=0;
            _intervalRangeControl=0;
            _multiLimit1=0;
            _multiLimit2=0;
            _oimlCertificationInformation=0;
            _ntepCertificationInformation=0;
            _maximumZeroingTime=0;
            _maximumPeakValueGross=0;
            _minimumPeakValueGross=0;
            _maximumPeakValue=0;
            _minimumPeakValue=0;
            _weightMovingDetection=0;
            _deviceAddress=0;
            _hardwareVersion=0;
            _identification=0;
            _limitValueMonitoringLIV11=0;
            _signalSourceLIV12=0;
            _switchOnLevelLIV13=0;
            _switchOffLevelLIV14=0;
            _limitValueMonitoringLIV21=0;
            _signalSourceLIV22=0;
            _switchOnLevelLIV23=0;
            _switchOffLevelLIV24=0;

            _limitValueMonitoringLIV31=0;
            _signalSourceLIV32=0;
            _switchOnLevelLIV33=0;
            _switchOffLevelLIV34=0;
            _limitValueMonitoringLIV41=0;
            _signalSourceLIV42=0;
            _switchOnLevelLIV43=0;
            _switchOffLevelLIV44=0;
            _outputScale=0;
            _firmwareDate=0;
            _resetTrigger=0;
            _stateDigital_IO_Extended=0;
            _softwareIdentification=0;
            _softwareVersion=0;
            _dateTime=0;

            _breakDosing = 0 ;
            _deleteDosingResult = 0 ;
            _materialStreamLastDosing = 0 ;
            _sum = 0 ;
            _specialDosingFunctions = 0 ;
            _dischargeTime = 0 ;
            _exceedingWeightBreak = 0 ;
            _delay1Dosing = 0 ;
            _delay2Dosing = 0 ;
            _emptyWeightTolerance = 0 ;
            _residualFlowDosingCycle = 0 ;
    }

        #endregion

        #region update method for the filler extended data

        public void UpdateFillerExtendedDataJet(Dictionary<string, int> _data)
        {
            this.UpdateFillerDataJet(_data);

            _errorRegister = _data[JetBusCommands.ERROR_REGISTER];
            _saveAllParameters = _data[JetBusCommands.SAVE_ALL_PARAMETERS];
            _restoreAllDefaultParameters = _data[JetBusCommands.RESTORE_ALL_DEFAULT_PARAMETERS];
            _vendorID = _data[JetBusCommands.VENDOR_ID];

            _productCode = _data[JetBusCommands.PRODUCT_CODE];
            _serialNumber = _data[JetBusCommands.SERIAL_NUMBER];
            _implementedProfileSpecification = _data[JetBusCommands.IMPLEMENTED_PROFILE_SPECIFICATION];
            _lcCapability = _data[JetBusCommands.LC_CAPABILITY];
            _weighingDevice1UnitPrefixOutputParameter = _data[JetBusCommands.WEIGHING_DEVICE_1_UNIT_PREFIX_OUTPUT_PARAMETER];

            _weighingDevice1WeightStep = _data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STEP];
            _alarms = _data[JetBusCommands.ALARMS];
            _weighingDevice1OutputWeight = _data[JetBusCommands.WEIGHING_DEVICE_1_OUTPUT_WEIGHT];
            _weighingDevice1Setting = _data[JetBusCommands.WEIGHING_DEVICE_1_SETTING];

            _localGravityFactor = _data[JetBusCommands.LOCAL_GRAVITY_FACTOR];
            _scaleFilterSetup = _data[JetBusCommands.SCALE_FILTER_SETUP];
            _dataSampleRate = _data[JetBusCommands.DATA_SAMPLE_RATE];

            _filterOrderCriticallyDamped = _data[JetBusCommands.FILTER_ORDER_CRITICALLY_DAMPED];
            _cutOffFrequencyCriticallyDamped = _data[JetBusCommands.CUT_OFF_FREQUENCY_CRITICALLY_DAMPED];
            _filterOrderButterworth = _data[JetBusCommands.FILTER_ORDER_BUTTERWORTH];
            _cutOffFrequencyButterWorth = _data[JetBusCommands.CUT_OFF_FREQUENCY_BUTTERWORTH];
            _filterOrderBessel = _data[JetBusCommands.FILTER_ORDER_BESSEL];

            _cutOffFrequencyBessel = _data[JetBusCommands.CUT_OFF_FREQUENCY_BESSEL];
            _scaleSupplyNominalVoltage = _data[JetBusCommands.SCALE_SUPPY_NOMINAL_VOLTAGE];
            _scaleSupplyMinimumVoltage = _data[JetBusCommands.SCALE_SUPPY_MINIMUM_VOLTAGE];
            _scaleSupplyMaximumVoltage = _data[JetBusCommands.SCALE_SUPPY_MAXIMUM_VOLTAGE];

            _scaleAccuracyClass = _data[JetBusCommands.SCALE_ACCURACY_CLASS];
            _scaleMinimumDeadLoad = _data[JetBusCommands.SCALE_MINIMUM_DEAD_LOAD];
            _scaleMaximumCapacity = _data[JetBusCommands.SCALE_MAXIMUM_CAPACITY];
            _scaleMaximumNumberVerificationInterval = _data[JetBusCommands.SCALE_MAXIMUM_NUMBER_OF_VERIFICATION_INTERVAL];
            _scaleApportionmentFactor = _data[JetBusCommands.SCALE_APPORTIONMENT_FACTOR];
            _scaleSafeLoadLimit = _data[JetBusCommands.SCALE_SAFE_LOAD_LIMIT];
            _scaleOperationNominalTemperature = _data[JetBusCommands.SCALE_OPERATION_NOMINAL_TEMPERATURE];
            _scaleOperationMinimumTemperature = _data[JetBusCommands.SCALE_OPERATION_MINIMUM_TEMPERATURE];
            _scaleOperationMaximumTemperature = _data[JetBusCommands.SCALE_OPERATION_MAXIMUM_TEMPERATURE];

            _scaleRelativeMinimumLoadCellVerficationInterval = _data[JetBusCommands.SCALE_RELATIVE_MINIMUM_LOAD_CELL_VERIFICATION_INTERVAL];
            _intervalRangeControl = _data[JetBusCommands.INTERVAL_RANGE_CONTROL];
            _multiLimit1 = _data[JetBusCommands.MULTI_LIMIT_1];
            _multiLimit2 = _data[JetBusCommands.MULTI_LIMIT_2];
            _oimlCertificationInformation = _data[JetBusCommands.OIML_CERTIFICAITON_INFORMATION];
            _ntepCertificationInformation = _data[JetBusCommands.NTEP_CERTIFICAITON_INFORMATION];
            _maximumZeroingTime = _data[JetBusCommands.MAXIMUM_ZEROING_TIME];
            _maximumPeakValueGross = _data[JetBusCommands.MAXIMUM_PEAK_VALUE_GROSS];
            _minimumPeakValueGross = _data[JetBusCommands.MINIMUM_PEAK_VALUE_GROSS];

            _maximumPeakValue = _data[JetBusCommands.MAXIMUM_PEAK_VALUE];
            _minimumPeakValue = _data[JetBusCommands.MINIMUM_PEAK_VALUE];
            _weightMovingDetection = _data[JetBusCommands.WEIGHT_MOVING_DETECTION];
            _deviceAddress = _data[JetBusCommands.DEVICE_ADDRESS];
            _hardwareVersion = _data[JetBusCommands.HAREWARE_VERSION];
            _identification = _data[JetBusCommands.IDENTIFICATION];

            _limitValueMonitoringLIV11 = _data[JetBusCommands.LIMIT_VALUE_MONITORING_LIV11];
            _signalSourceLIV12 = _data[JetBusCommands.SIGNAL_SOURCE_LIV12];
            _switchOnLevelLIV13 = _data[JetBusCommands.SWITCH_ON_LEVEL_LIV13];
            _switchOffLevelLIV14 = _data[JetBusCommands.SWTICH_OFF_LEVEL_LIV14];

            _limitValueMonitoringLIV21 = _data[JetBusCommands.LIMIT_VALUE_MONITORING_LIV21];
            _signalSourceLIV22 = _data[JetBusCommands.SIGNAL_SOURCE_LIV22];
            _switchOnLevelLIV23 = _data[JetBusCommands.SWITCH_ON_LEVEL_LIV23];
            _switchOffLevelLIV24 = _data[JetBusCommands.SWTICH_OFF_LEVEL_LIV24];

            _limitValueMonitoringLIV31 = _data[JetBusCommands.LIMIT_VALUE_MONITORING_LIV31];
            _signalSourceLIV32 = _data[JetBusCommands.SIGNAL_SOURCE_LIV32];
            _switchOnLevelLIV33 = _data[JetBusCommands.SWITCH_ON_LEVEL_LIV33];
            _switchOffLevelLIV34 = _data[JetBusCommands.SWTICH_OFF_LEVEL_LIV34];

            _limitValueMonitoringLIV41 = _data[JetBusCommands.LIMIT_VALUE_MONITORING_LIV41];
            _signalSourceLIV42 = _data[JetBusCommands.SIGNAL_SOURCE_LIV42];
            _switchOnLevelLIV43 = _data[JetBusCommands.SWITCH_ON_LEVEL_LIV43];
            _switchOffLevelLIV44 = _data[JetBusCommands.SWTICH_OFF_LEVEL_LIV44];

            _outputScale = _data[JetBusCommands.OUTPUT_SCALE];
            _firmwareDate = _data[JetBusCommands.FIRMWARE_DATE];
            _resetTrigger = _data[JetBusCommands.RESET_TRIGGER];
            _stateDigital_IO_Extended = _data[JetBusCommands.STATE_DIGITAL_IO_EXTENDED];

            _softwareIdentification = _data[JetBusCommands.SOFTWARE_IDENTIFICATION];
            _softwareVersion = _data[JetBusCommands.SOFTWARE_VERSION];
            _dateTime = _data[JetBusCommands.DATE_TIME];

            _breakDosing = _data[JetBusCommands.BREAK_DOSING];
            _deleteDosingResult = _data[JetBusCommands.DELETE_DOSING_RESULT];
            _materialStreamLastDosing = _data[JetBusCommands.MATERIAL_STREAM_LAST_DOSING];
            _sum = _data[JetBusCommands.SUM];
            _specialDosingFunctions = _data[JetBusCommands.SPECIAL_DOSING_FUNCTIONS];
            _dischargeTime = _data[JetBusCommands.DISCHARGE_TIME];
            _exceedingWeightBreak = _data[JetBusCommands.EXCEEDING_WEIGHT_BREAK];
            _delay1Dosing = _data[JetBusCommands.DELAY1_DOSING];
            _delay2Dosing = _data[JetBusCommands.DELAY2_DOSING];
            _emptyWeightTolerance = _data[JetBusCommands.EMPTY_WEIGHT_TOLERANCE];
            _residualFlowDosingCycle = _data[JetBusCommands.RESIDUAL_FLOW_DOSING_CYCLE];
        }

        #endregion

        #region Properties for the filler extended data

        public int ErrorRegister
        {
            get { return _errorRegister; }
            set { this._errorRegister = value; }
        }
        public int SaveAllParameters
        {
            get { return _saveAllParameters; }
            set { this._saveAllParameters = value; }
        }
        public int RestoreAllDefaultParameters
        {
            get { return _restoreAllDefaultParameters; }
            set { this._restoreAllDefaultParameters = value; }
        }
        public int VendorID
        {
            get { return _vendorID; }
            set { this._vendorID = value; }
        }
        public int ProductCode
        {
            get { return _productCode; }
            set { this._productCode = value; }
        }
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { this._serialNumber = value; }
        }
        public int ImplementedProfileSpecification
        {
            get { return _implementedProfileSpecification; }
            set { this._implementedProfileSpecification = value; }
        }
        public int LcCapability
        {
            get { return _lcCapability; }
            set { this._lcCapability = value; }
        }
        public int WeighingDevice1UnitPrefixOutputParameter
        {
            get { return _weighingDevice1UnitPrefixOutputParameter; }
            set { this._weighingDevice1UnitPrefixOutputParameter = value; }
        }
        public int WeighingDevice1WeightStep
        {
            get { return _weighingDevice1WeightStep; }
            set { this._weighingDevice1WeightStep = value; }
        }
        public int Alarms
        {
            get { return _alarms; }
            set { this._alarms = value; }
        }
        public int WeighingDevice1OutputWeight
        {
            get { return _weighingDevice1OutputWeight; }
            set { this._weighingDevice1OutputWeight = value; }
        }
        public int WeighingDevice1Setting
        {
            get { return _weighingDevice1Setting; }
            set { this._weighingDevice1Setting = value; }
        }
        public int LocalGravityFactor
        {
            get { return _localGravityFactor; }
            set { this._localGravityFactor = value; }
        }
        public int ScaleFilterSetup
        {
            get { return _scaleFilterSetup; }
            set { this._scaleFilterSetup = value; }
        }
        public int DataSampleRate
        {
            get { return _dataSampleRate; }
            set { this._dataSampleRate = value; }
        }
        public int FilterOrderCriticallyDamped
        {
            get { return _filterOrderCriticallyDamped; }
            set { this._filterOrderCriticallyDamped = value; }
        }
        public int CutOffFrequencyCriticallyDamped
        {
            get { return _cutOffFrequencyCriticallyDamped; }
            set { this._cutOffFrequencyCriticallyDamped = value; }
        }
        public int FilterOrderButterworth
        {
            get { return _filterOrderButterworth; }
            set { this._filterOrderButterworth = value; }
        }
        public int CutOffFrequencyButterWorth
        {
            get { return _cutOffFrequencyButterWorth; }
            set { this._cutOffFrequencyButterWorth = value; }
        }
        public int FilterOrderBessel
        {
            get { return _filterOrderBessel; }
            set { this._filterOrderBessel = value; }
        }
        public int CutOffFrequencyBessel
        {
            get { return _cutOffFrequencyBessel; }
            set { this._cutOffFrequencyBessel = value; }
        }
        public int ScaleSupplyNominalVoltage
        {
            get { return _scaleSupplyNominalVoltage; }
            set { this._scaleSupplyNominalVoltage = value; }
        }
        public int ScaleSupplyMinimumVoltage
        {
            get { return _scaleSupplyMinimumVoltage; }
            set { this._scaleSupplyMinimumVoltage = value; }
        }
        public int ScaleSupplyMaximumVoltage
        {
            get { return _scaleSupplyMaximumVoltage; }
            set { this._scaleSupplyMaximumVoltage = value; }
        }
        public int ScaleAccuracyClass
        {
            get { return _scaleAccuracyClass; }
            set { this._scaleAccuracyClass = value; }
        }
        public int ScaleMinimumDeadLoad
        {
            get { return _scaleMinimumDeadLoad; }
            set { this._scaleMinimumDeadLoad = value; }
        }
        public int ScaleMaximumCapacity
        {
            get { return _scaleMaximumCapacity; }
            set { this._scaleMaximumCapacity = value; }
        }
        public int ScaleMaximumNumberVerificationInterval
        {
            get { return _scaleMaximumNumberVerificationInterval; }
            set { this._scaleMaximumNumberVerificationInterval = value; }
        }
        public int ScaleApportionmentFactor
        {
            get { return _scaleApportionmentFactor; }
            set { this._scaleApportionmentFactor = value; }
        }
        public int ScaleSafeLoadLimit
        {
            get { return _scaleSafeLoadLimit; }
            set { this._scaleSafeLoadLimit = value; }
        }
        public int ScaleOperationNominalTemperature
        {
            get { return _scaleOperationNominalTemperature; }
            set { this._scaleOperationNominalTemperature = value; }
        }
        public int ScaleOperationMinimumTemperature
        {
            get { return _scaleOperationMinimumTemperature; }
            set { this._scaleOperationMinimumTemperature = value; }
        }
        public int ScaleOperationMaximumTemperature
        {
            get { return _scaleOperationMaximumTemperature; }
            set { this._scaleOperationMaximumTemperature = value; }
        }
        public int ScaleRelativeMinimumLoadCellVerficationInterval
        {
            get { return _scaleRelativeMinimumLoadCellVerficationInterval; }
            set { this._scaleRelativeMinimumLoadCellVerficationInterval = value; }
        }
        public int IntervalRangeControl
        {
            get { return _intervalRangeControl; }
            set { this._intervalRangeControl = value; }
        }
        public int MultiLimit1
        {
            get { return _multiLimit1; }
            set { this._multiLimit1 = value; }
        }
        public int MultiLimit2
        {
            get { return _multiLimit2; }
            set { this._multiLimit2 = value; }
        }
        public int OimlCertificationInformation
        {
            get { return _oimlCertificationInformation; }
            set { this._oimlCertificationInformation = value; }
        }
        public int NtepCertificationInformation
        {
            get { return _ntepCertificationInformation; }
            set { this._ntepCertificationInformation = value; }
        }
        public int MaximumZeroingTime
        {
            get { return _maximumZeroingTime; }
            set { this._maximumZeroingTime = value; }
        }
        public int MaximumPeakValueGross
        {
            get { return _maximumPeakValueGross; }
            set { this._maximumPeakValueGross = value; }
        }
        public int MinimumPeakValueGross
        {
            get { return _minimumPeakValueGross; }
            set { this._minimumPeakValueGross = value; }
        }
        public int MaximumPeakValue
        {
            get { return _maximumPeakValue; }
            set { this._maximumPeakValue = value; }
        }
        public int MinimumPeakValue
        {
            get { return _minimumPeakValue; }
            set { this._minimumPeakValue = value; }
        }
        public int WeightMovingDetection
        {
            get { return _weightMovingDetection; }
            set { this._weightMovingDetection = value; }
        }
        public int DeviceAddress
        {
            get { return _deviceAddress; }
            set { this._deviceAddress = value; }
        }
        public int HardwareVersion
        {
            get { return _hardwareVersion; }
            set { this._hardwareVersion = value; }
        }
        public int Identification
        {
            get { return _identification; }
            set { this._identification = value; }
        }
        public int LimitValueMonitoringLIV11
        {
            get { return _limitValueMonitoringLIV11; }
            set { this._limitValueMonitoringLIV11 = value; }
        }
        public int SignalSourceLIV12
        {
            get { return _signalSourceLIV12; }
            set { this._signalSourceLIV12 = value; }
        }
        public int SwitchOnLevelLIV13
        {
            get { return _switchOnLevelLIV13; }
            set { this._switchOnLevelLIV13 = value; }
        }
        public int SwitchOffLevelLIV14
        {
            get { return _switchOffLevelLIV14; }
            set { this._switchOffLevelLIV14 = value; }
        }
        public int LimitValueMonitoringLIV21
        {
            get { return _limitValueMonitoringLIV21; }
            set { this._limitValueMonitoringLIV21 = value; }
        }
        public int SignalSourceLIV22
        {
            get { return _signalSourceLIV22; }
            set { this._signalSourceLIV22 = value; }
        }
        public int SwitchOnLevelLIV23
        {
            get { return _switchOnLevelLIV23; }
            set { this._switchOnLevelLIV23 = value; }
        }
        public int SwitchOffLevelLIV24
        {
            get { return _switchOffLevelLIV24; }
            set { this._switchOffLevelLIV24 = value; }
        }
        public int LimitValueMonitoringLIV31
        {
            get { return _limitValueMonitoringLIV31; }
            set { this._limitValueMonitoringLIV31 = value; }
        }
        public int SignalSourceLIV32
        {
            get { return _signalSourceLIV32; }
            set { this._signalSourceLIV32 = value; }
        }
        public int SwitchOnLevelLIV33
        {
            get { return _switchOnLevelLIV33; }
            set { this._switchOnLevelLIV33 = value; }
        }
        public int SwitchOffLevelLIV34
        {
            get { return _switchOffLevelLIV34; }
            set { this._switchOffLevelLIV34 = value; }
        }
        public int LimitValueMonitoringLIV41
        {
            get { return _limitValueMonitoringLIV41; }
            set { this._limitValueMonitoringLIV41 = value; }
        }
        public int SignalSourceLIV42
        {
            get { return _signalSourceLIV42; }
            set { this._signalSourceLIV42 = value; }
        }
        public int SwitchOnLevelLIV43
        {
            get { return _switchOnLevelLIV43; }
            set { this._switchOnLevelLIV43 = value; }
        }
        public int SwitchOffLevelLIV44
        {
            get { return _switchOffLevelLIV44; }
            set { this._switchOffLevelLIV44 = value; }
        }
        public int OutputScale
        {
            get { return _outputScale; }
            set { this._outputScale = value; }
        }
        public int FirmwareDate
        {
            get { return _firmwareDate; }
            set { this._firmwareDate = value; }
        }
        public int ResetTrigger
        {
            get { return _resetTrigger; }
            set { this._resetTrigger = value; }
        }
        public int StateDigital_IO_Extended
        {
            get { return _stateDigital_IO_Extended; }
            set { this._stateDigital_IO_Extended = value; }
        }
        public int SoftwareIdentification
        {
            get { return _softwareIdentification; }
            set { this._softwareIdentification = value; }
        }
        public int SoftwareVersion
        {
            get { return _softwareVersion; }
            set { this._softwareVersion = value; }
        }
        public int DateTime
        {
            get { return _dateTime; }
            set { this._dateTime = value; }
        }
        public int BreakDosing
        {
            get { return _breakDosing; }
            set { this._breakDosing = value; }
        }
        public int DeleteDosingResult
        {
            get { return _deleteDosingResult; }
            set { this._deleteDosingResult = value; }
        }
        public int MaterialStreamLastDosing
        {
            get { return _materialStreamLastDosing; }
            set { this._materialStreamLastDosing = value; }
        }
        public int Sum
        {
            get { return _sum; }
            set { this._sum = value; }
        }
        public int SpecialDosingFunctions
        {
            get { return _specialDosingFunctions; }
            set { this._specialDosingFunctions = value; }
        }
        public int DischargeTime
        {
            get { return _dischargeTime; }
            set { this._dischargeTime = value; }
        }
        public int ExceedingWeightBreak
        {
            get { return _exceedingWeightBreak; }
            set { this._exceedingWeightBreak = value; }
        }
        public int Delay1Dosing
        {
            get { return _delay1Dosing; }
            set { this._delay1Dosing = value; }
        }
        public int Delay2Dosing
        {
            get { return _delay2Dosing; }
            set { this._delay2Dosing = value; }
        }
        public int EmptyWeightTolerance
        {
            get { return _emptyWeightTolerance; }
            set { this._emptyWeightTolerance = value; }
        }
        public int ResidualFlowDosingCycle
        {
            get { return _residualFlowDosingCycle; }
            set { this._residualFlowDosingCycle = value; }
        }

        #endregion

    }
}