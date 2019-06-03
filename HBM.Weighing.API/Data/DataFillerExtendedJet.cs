// <copyright file="DataFillerExtended.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hbm.Weighing.API.Data
{
    /// <summary>
    /// Implementation of the interface IDataFillerExtended for the filler extended mode.
    /// The class DataFillerExtended contains the data input word and data output words for the filler extended mode
    /// of WTX device 120 and 110. 
    /// 
    /// This is only available via a JetBus Ethernet connection, not via Modbus. 
    /// </summary>
    public class DataFillerExtendedJet : DataFillerJet, IDataFillerExtended
    {

        #region ========================= privates =========================

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
        private int _maximumZeroingTime;
        private int _maximumPeakValueGross;
        private int _minimumPeakValueGross;
        private int _maximumPeakValue;
        private int _minimumPeakValue;
        private int _weightMovingDetection;
        private int _deviceAddress;
        private int _outputScale;
        private int _resetTrigger;
        private int _stateDigital_IO_Extended;
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

        private INetConnection _connection;
        #endregion

        #region =============== constructors & destructors =================

        /// <summary>
        /// Constructor of class DataFillerExtendedJet : Initalizes values and connects 
        /// the eventhandler from Connection to the interal update method
        /// </summary>
        public DataFillerExtendedJet(INetConnection Connection):base(Connection)          
        {
            _connection = Connection;

            _connection.UpdateData += UpdateFillerExtendedData;
            Console.WriteLine("DataFillerExtendedJet");

            _errorRegister =0;
            _saveAllParameters =0;
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
            _maximumZeroingTime=0;
            _maximumPeakValueGross=0;
            _minimumPeakValueGross=0;
            _maximumPeakValue=0;
            _minimumPeakValue=0;
            _weightMovingDetection=0;
            _deviceAddress=0;
            _outputScale=0;
            _resetTrigger=0;
            _stateDigital_IO_Extended=0;
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

            OimlCertificationInformation = "0";
            NtepCertificationInformation = "0";
            HardwareVersion = "0";
            Identification = "0";
            FirmwareDate = "0";
            SoftwareIdentification = "0";
            SoftwareVersion = "0";
        }

        #endregion

        #region ========== update method - filler extended data ============

        /// <summary>
        /// Updates & converts the values from buffer (Dictionary<string,string>) 
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateFillerExtendedData(object sender, EventArgs e)
        {
            try
            {
                if (Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.IMDApplicationMode)) == 2 || Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.IMDApplicationMode)) == 3) // If application mode = filler
                {
                    this.UpdateFillerData(this, e);

                    _saveAllParameters = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SaveAllParameters));
                    _restoreAllDefaultParameters = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461RestoreAllDefaultParameters));
                    _vendorID = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461VendorID));

                    _productCode = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ProductCode));
                    _serialNumber = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SerialNumber));
                    _implementedProfileSpecification = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ImplementedProfileSpecification));
                    _lcCapability = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461LoadCellCapability));
                    _weighingDevice1UnitPrefixOutputParameter = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461UnitPrefixFixedParameters));
                    _weighingDevice1WeightStep = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461WeightStep));
                    _alarms = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Alarms));
                    _weighingDevice1OutputWeight = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461OutputWeight));
                    _weighingDevice1Setting = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleSettings));

                    _localGravityFactor = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461LocalGravityFactor));
                    _scaleFilterSetup = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleFilter));
                    _dataSampleRate = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SampleRate));

                    _filterOrderCriticallyDamped = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterCriticallyDampedFilterOrder));
                    _cutOffFrequencyCriticallyDamped = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterCriticallyDampedCutOffFrequency));
                    _filterOrderButterworth = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterButterworthFilterOrder));
                    _cutOffFrequencyButterWorth = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterButterworthCutOffFrequency));
                    _filterOrderBessel = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterBesselFilterOrder));

                    _cutOffFrequencyBessel = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461FilterBesselCutOffFrequency));
                    _scaleSupplyNominalVoltage = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SuppyVoltageNominal));
                    _scaleSupplyMinimumVoltage = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SuppyVoltageNominalMinimal));
                    _scaleSupplyMaximumVoltage = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461SuppyVoltageMaximal));

                    _scaleAccuracyClass = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleAccuracyClass));
                    _scaleMinimumDeadLoad = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleMinimumDeadLoad));
                    _scaleMaximumCapacity = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleMaximumCapacity));
                    _scaleMaximumNumberVerificationInterval = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleMaximumNumberOfVerifications));
                    _scaleApportionmentFactor = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleApportionmentFactor));
                    _scaleSafeLoadLimit = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleSafeLoadLimit));
                    _scaleOperationNominalTemperature = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleOperationTemperatureNominal));
                    _scaleOperationMinimumTemperature = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleOperationTemperatureMinimal));
                    _scaleOperationMaximumTemperature = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461ScaleOperationTemperatureMaximal));

                    _intervalRangeControl = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461MultiIntervalRangeControl));
                    _multiLimit1 = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461MultiLimit1));
                    _multiLimit2 = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461MultiLimit2));
                    OimlCertificationInformation = _connection.ReadFromBuffer(JetBusCommands.CIA461CertificaitonInfoOIML);
                    NtepCertificationInformation = _connection.ReadFromBuffer(JetBusCommands.CIA461CertificaitonInfoNTEP);
                    _maximumZeroingTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461MaximumZeroingTime));
                    _maximumPeakValueGross = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461PeakValueGrossMax));
                    _minimumPeakValueGross = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461PeakValueGrossMin));

                    _maximumPeakValue = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461PeakValueMax));
                    _minimumPeakValue = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461PeakValuMin));
                    _weightMovingDetection = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CIA461WeightMovingDetection));
                    _deviceAddress = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.ADRDeviceAddress));
                    HardwareVersion = _connection.ReadFromBuffer(JetBusCommands.HWVHardwareVersion);
                    Identification = _connection.ReadFromBuffer(JetBusCommands.IDNDeviceIdentification);

                    _outputScale = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.NOVScaleCapacity));
                    FirmwareDate = _connection.ReadFromBuffer(JetBusCommands.PDTFirmwareDate);
                    _resetTrigger = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.RESResetDevice));
                    _stateDigital_IO_Extended = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.RIODigitalIOStatus));

                    SoftwareIdentification = _connection.ReadFromBuffer(JetBusCommands.SWISoftwareIdentification);
                    SoftwareVersion = _connection.ReadFromBuffer(JetBusCommands.SWVSoftwareVersion);
                    _dateTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.TIMCurrentDatetime));

                    _breakDosing = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.BRKBreakFilling));
                    _deleteDosingResult = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.CSNClearFillingResult));
                    _materialStreamLastDosing = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.MFOMaterialFlow));
                    _sum = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.SUMFillingResultSum));
                    _specialDosingFunctions = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.SDFSpecialDosingFunctions));
                    _dischargeTime = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.EPTDischargeTime));
                    _exceedingWeightBreak = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.EWBEmptyWeightBreak));
                    _delay1Dosing = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.DL1DosingDelay1));
                    _delay2Dosing = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.DL2DosingDelay2));
                    _emptyWeightTolerance = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.EWTEmptyWeight));
                    _residualFlowDosingCycle = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.RFOResidualFlow));

                    // JetBusCommands.Error_register.PathIndex and JetBusCommands.Scale_relative_minimum_load_cell_verification_interval.PathIndex throw KeyNotFoundException :

                    //_errorRegister = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Error_register));
                    //_scaleRelativeMinimumLoadCellVerficationInterval = Convert.ToInt32(_connection.ReadFromBuffer(JetBusCommands.Scale_relative_minimum_load_cell_verification_interval));
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException in class DataFillerExtendedJet, update method");
                //_connection.CommunicationLog.Invoke(this, new LogEvent((new KeyNotFoundException()).Message));
            }
        }
        #endregion

        #region ======== Get-/Set properties - filler extended data ========

        public int ErrorRegister
        {
            get { return _errorRegister; }
            set {_connection.Write(JetBusCommands.ESRErrorRegister , value);
                this._errorRegister = value;
            }
        }
        public int SaveAllParameters
        {
            get { return _saveAllParameters; }
            set { _connection.Write(JetBusCommands.CIA461SaveAllParameters , value);
                 this._saveAllParameters = value; }
        }
        public int RestoreAllDefaultParameters
        {
            get { return _restoreAllDefaultParameters; }
            set { _connection.Write(JetBusCommands.CIA461RestoreAllDefaultParameters , value);
                this._restoreAllDefaultParameters = value; }
        }
        public int VendorID
        {
            get { return _vendorID; }
            set { _connection.Write(JetBusCommands.CIA461VendorID , value);
                this._vendorID = value; }
        }
        public int ProductCode
        {
            get { return _productCode; }
            set { _connection.Write(JetBusCommands.CIA461ProductCode , value);
                this._productCode = value; }
        }
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { _connection.Write(JetBusCommands.CIA461SerialNumber , value);
                this._serialNumber = value; }
        }
        public int ImplementedProfileSpecification
        {
            get { return _implementedProfileSpecification; }
            set { _connection.Write(JetBusCommands.CIA461ImplementedProfileSpecification , value);
                this._implementedProfileSpecification = value; }
        }
        public int LcCapability
        {
            get { return _lcCapability; }
            set { _connection.Write(JetBusCommands.CIA461LoadCellCapability , value);
                this._lcCapability = value; }
        }
        public int WeighingDevice1UnitPrefixOutputParameter
        {
            get { return _weighingDevice1UnitPrefixOutputParameter; }
            set { _connection.Write(JetBusCommands.CIA461UnitPrefixFixedParameters , value);
                this._weighingDevice1UnitPrefixOutputParameter = value; }
        }
        public int WeighingDevice1WeightStep
        {
            get { return _weighingDevice1WeightStep; }
            set { _connection.Write(JetBusCommands.CIA461WeightStep , value);
                this._weighingDevice1WeightStep = value; }
        }
        public int Alarms
        {
            get { return _alarms; }
            set { _connection.Write(JetBusCommands.Alarms , value);
                this._alarms = value; }
        }
        public int WeighingDevice1OutputWeight
        {
            get { return _weighingDevice1OutputWeight; }
            set { _connection.Write(JetBusCommands.CIA461OutputWeight , value);
                this._weighingDevice1OutputWeight = value; }
        }
        public int WeighingDevice1Setting
        {
            get { return _weighingDevice1Setting; }
            set { _connection.Write(JetBusCommands.CIA461ScaleSettings , value);
                this._weighingDevice1Setting = value; }
        }
        public int LocalGravityFactor
        {
            get { return _localGravityFactor; }
            set { _connection.Write(JetBusCommands.CIA461LocalGravityFactor , value);
                this._localGravityFactor = value; }
        }
        public int ScaleFilterSetup
        {
            get { return _scaleFilterSetup; }
            set { _connection.Write(JetBusCommands.CIA461ScaleFilter , value);
                this._scaleFilterSetup = value; }
        }
        public int DataSampleRate
        {
            get { return _dataSampleRate; }
            set { _connection.Write(JetBusCommands.CIA461SampleRate , value);
                this._dataSampleRate = value; }
        }
        public int FilterOrderCriticallyDamped
        {
            get { return _filterOrderCriticallyDamped; }
            set { _connection.Write(JetBusCommands.CIA461FilterCriticallyDampedFilterOrder , value);
                this._filterOrderCriticallyDamped = value; }
        }
        public int CutOffFrequencyCriticallyDamped
        {
            get { return _cutOffFrequencyCriticallyDamped; }
            set { _connection.Write(JetBusCommands.CIA461FilterCriticallyDampedCutOffFrequency , value);
                this._cutOffFrequencyCriticallyDamped = value; }
        }
        public int FilterOrderButterworth
        {
            get { return _filterOrderButterworth; }
            set { _connection.Write(JetBusCommands.CIA461FilterButterworthFilterOrder , value);
                this._filterOrderButterworth = value; }
        }
        public int CutOffFrequencyButterWorth
        {
            get { return _cutOffFrequencyButterWorth; }
            set { _connection.Write(JetBusCommands.CIA461FilterButterworthCutOffFrequency , value);
                this._cutOffFrequencyButterWorth = value; }
        }
        public int FilterOrderBessel
        {
            get { return _filterOrderBessel; }
            set { _connection.Write(JetBusCommands.CIA461FilterBesselFilterOrder , value);
                this._filterOrderBessel = value; }
        }
        public int CutOffFrequencyBessel
        {
            get { return _cutOffFrequencyBessel; }
            set { _connection.Write(JetBusCommands.CIA461FilterBesselCutOffFrequency , value);
                this._cutOffFrequencyBessel = value; }
        }
        public int ScaleSupplyNominalVoltage
        {
            get { return _scaleSupplyNominalVoltage; }
            set { _connection.Write(JetBusCommands.CIA461SuppyVoltageNominal , value);
                this._scaleSupplyNominalVoltage = value; }
        }
        public int ScaleSupplyMinimumVoltage
        {
            get { return _scaleSupplyMinimumVoltage; }
            set { _connection.Write(JetBusCommands.CIA461SuppyVoltageNominalMinimal , value);
                this._scaleSupplyMinimumVoltage = value; }
        }
        public int ScaleSupplyMaximumVoltage
        {
            get { return _scaleSupplyMaximumVoltage; }
            set { _connection.Write(JetBusCommands.CIA461SuppyVoltageMaximal , value);
                this._scaleSupplyMaximumVoltage = value; }
        }
        public int ScaleAccuracyClass
        {
            get { return _scaleAccuracyClass; }
            set { _connection.Write(JetBusCommands.CIA461ScaleAccuracyClass , value);
                this._scaleAccuracyClass = value; }
        }
        public int ScaleMinimumDeadLoad
        {
            get { return _scaleMinimumDeadLoad; }
            set { _connection.Write(JetBusCommands.CIA461ScaleMinimumDeadLoad , value);
                this._scaleMinimumDeadLoad = value; }
        }
        public int ScaleMaximumCapacity
        {
            get { return _scaleMaximumCapacity; }
            set { _connection.Write(JetBusCommands.CIA461ScaleMaximumCapacity , value);
                this._scaleMaximumCapacity = value; }
        }
        public int ScaleMaximumNumberVerificationInterval
        {
            get { return _scaleMaximumNumberVerificationInterval; }
            set { _connection.Write(JetBusCommands.CIA461ScaleMaximumNumberOfVerifications , value);
                this._scaleMaximumNumberVerificationInterval = value; }
        }
        public int ScaleApportionmentFactor
        {
            get { return _scaleApportionmentFactor; }
            set { _connection.Write(JetBusCommands.CIA461ScaleApportionmentFactor , value);
                this._scaleApportionmentFactor = value; }
        }
        public int ScaleSafeLoadLimit
        {
            get { return _scaleSafeLoadLimit; }
            set { _connection.Write(JetBusCommands.CIA461ScaleSafeLoadLimit , value);
                this._scaleSafeLoadLimit = value; }
        }
        public int ScaleOperationNominalTemperature
        {
            get { return _scaleOperationNominalTemperature; }
            set { _connection.Write(JetBusCommands.CIA461ScaleOperationTemperatureNominal , value);
                this._scaleOperationNominalTemperature = value; }
        }
        public int ScaleOperationMinimumTemperature
        {
            get { return _scaleOperationMinimumTemperature; }
            set { _connection.Write(JetBusCommands.CIA461ScaleOperationTemperatureMinimal , value);
                this._scaleOperationMinimumTemperature = value; }
        }
        public int ScaleOperationMaximumTemperature
        {
            get { return _scaleOperationMaximumTemperature; }
            set { _connection.Write(JetBusCommands.CIA461ScaleOperationTemperatureMaximal , value);
                this._scaleOperationMaximumTemperature = value; }
        }
        public int ScaleRelativeMinimumLoadCellVerficationInterval
        {
            get { return _scaleRelativeMinimumLoadCellVerficationInterval; }
            set { _connection.Write(JetBusCommands.CIA461ScaleMinimumVerificationInterval , value);
                this._scaleRelativeMinimumLoadCellVerficationInterval = value; }
        }
        public int IntervalRangeControl
        {
            get { return _intervalRangeControl; }
            set { _connection.Write(JetBusCommands.CIA461MultiIntervalRangeControl , value);
                this._intervalRangeControl = value; }
        }
        public int MultiLimit1
        {
            get { return _multiLimit1; }
            set { _connection.Write(JetBusCommands.CIA461MultiLimit1 , value);
                this._multiLimit1 = value; }
        }
        public int MultiLimit2
        {
            get { return _multiLimit2; }
            set { _connection.Write(JetBusCommands.CIA461MultiLimit2 , value);
                this._multiLimit2 = value; }
        }
        public int MaximumZeroingTime
        {
            get { return _maximumZeroingTime; }
            set { _connection.Write(JetBusCommands.CIA461MaximumZeroingTime , value);
                this._maximumZeroingTime = value; }
        }
        public int MaximumPeakValueGross
        {
            get { return _maximumPeakValueGross; }
            set { _connection.Write(JetBusCommands.CIA461PeakValueGrossMax , value);
                this._maximumPeakValueGross = value; }
        }
        public int MinimumPeakValueGross
        {
            get { return _minimumPeakValueGross; }
            set { _connection.Write(JetBusCommands.CIA461PeakValueGrossMin , value);
                this._minimumPeakValueGross = value; }
        }
        public int MaximumPeakValue
        {
            get { return _maximumPeakValue; }
            set { _connection.Write(JetBusCommands.CIA461PeakValueMax , value);
                this._maximumPeakValue = value; }
        }
        public int MinimumPeakValue
        {
            get { return _minimumPeakValue; }
            set { _connection.Write(JetBusCommands.CIA461PeakValuMin , value);
                this._minimumPeakValue = value; }
        }
        public int WeightMovingDetection
        {
            get { return _weightMovingDetection; }
            set { _connection.Write(JetBusCommands.CIA461WeightMovingDetection , value);
                this._weightMovingDetection = value; }
        }
        public int DeviceAddress
        {
            get { return _deviceAddress; }
            set { _connection.Write(JetBusCommands.ADRDeviceAddress , value);
                this._deviceAddress = value; }
        } 
        public int OutputScale
        {
            get { return _outputScale; }
            set { _connection.Write(JetBusCommands.NOVScaleCapacity , value);
                this._outputScale = value; }
        }
        public int ResetTrigger
        {
            get { return _resetTrigger; }
            set { _connection.Write(JetBusCommands.RESResetDevice , value);
                this._resetTrigger = value; }
        }
        public int StateDigital_IO_Extended
        {
            get { return _stateDigital_IO_Extended; }
            set { _connection.Write(JetBusCommands.RIODigitalIOStatus , value);
                this._stateDigital_IO_Extended = value; }
        }
        public int DateTime
        {
            get { return _dateTime; }
            set { _connection.Write(JetBusCommands.TIMCurrentDatetime , value);
                this._dateTime = value; }
        }
        public int BreakDosing
        {
            get { return _breakDosing; }
            set { _connection.Write(JetBusCommands.BRKBreakFilling , value);
                this._breakDosing = value; }
        }
        public int DeleteDosingResult
        {
            get { return _deleteDosingResult; }
            set { _connection.Write(JetBusCommands.CSNClearFillingResult , value);
                this._deleteDosingResult = value; }
        }
        public int MaterialStreamLastDosing
        {
            get { return _materialStreamLastDosing; }
            set { _connection.Write(JetBusCommands.MFOMaterialFlow , value);
                this._materialStreamLastDosing = value; }
        }
        public int Sum
        {
            get { return _sum; }
            set { _connection.Write(JetBusCommands.SUMFillingResultSum , value);
                this._sum = value; }
        }
        public int SpecialDosingFunctions
        {
            get { return _specialDosingFunctions; }
            set { _connection.Write(JetBusCommands.SDFSpecialDosingFunctions , value);
                this._specialDosingFunctions = value; }
        }
        public int DischargeTime
        {
            get { return _dischargeTime; }
            set { _connection.Write(JetBusCommands.EPTDischargeTime , value);
                this._dischargeTime = value; }
        }
        public int ExceedingWeightBreak
        {
            get { return _exceedingWeightBreak; }
            set { _connection.Write(JetBusCommands.EWBEmptyWeightBreak , value);
                this._exceedingWeightBreak = value; }
        }
        public int Delay1Dosing
        {
            get { return _delay1Dosing; }
            set { _connection.Write(JetBusCommands.DL1DosingDelay1 , value);
                this._delay1Dosing = value; }
        }
        public int Delay2Dosing
        {
            get { return _delay2Dosing; }
            set { _connection.Write(JetBusCommands.DL2DosingDelay2 , value);
                this._delay2Dosing = value; }
        }
        public int EmptyWeightTolerance
        {
            get { return _emptyWeightTolerance; }
            set { _connection.Write(JetBusCommands.EWTEmptyWeight , value);
                this._emptyWeightTolerance = value; }
        }
        public int ResidualFlowDosingCycle
        {
            get { return _residualFlowDosingCycle; }
            set { _connection.Write(JetBusCommands.RFOResidualFlow , value);
                this._residualFlowDosingCycle = value; }
        }
        public string OimlCertificationInformation { get; private set; }
        public string NtepCertificationInformation { get; private set; }
        public string HardwareVersion { get; private set; }
        public string FirmwareDate { get; private set; }
        public string Identification { get; private set; }
        public string SoftwareIdentification { get; private set; }
        public string SoftwareVersion { get; private set; }
        #endregion

    }
}