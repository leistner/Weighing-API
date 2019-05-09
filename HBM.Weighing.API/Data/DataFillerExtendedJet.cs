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
    /// The class DataFillerExtended contains the data input word and data output words for the filler extended mode
    /// of WTX device 120 and 110. 
    /// 
    /// This is only available via a JetBus Ethernet connection, not via Modbus. 
    /// </summary>
    public class DataFillerExtendedJet : DataFillerJet, IDataFillerExtended
    {
        #region privates

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

        private INetConnection _connection;
        #endregion

        #region constructor

        public DataFillerExtendedJet(INetConnection Connection):base(Connection)          
        {
            _connection = Connection;

            _connection.UpdateDataClasses += UpdateFillerExtendedData;

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

        public void UpdateFillerExtendedData(object sender, DataEventArgs e)
        {
            if (e.DataDictionary[JetBusCommands.Application_mode.PathIndex] == 2 ||  e.DataDictionary[JetBusCommands.Application_mode.PathIndex] == 3) // If application mode = filler
            {
                this.UpdateFillerData(this, e);

                //_errorRegister = Convert.ToInt32(e.DataDictionary[JetBusCommands.ERROR_REGISTER.PathIndex]);
                _saveAllParameters = Convert.ToInt32(e.DataDictionary[JetBusCommands.Save_all_parameters.PathIndex]);
                _restoreAllDefaultParameters = Convert.ToInt32(e.DataDictionary[JetBusCommands.Restore_all_default_parameters.PathIndex]);
                _vendorID = Convert.ToInt32(e.DataDictionary[JetBusCommands.Vendor_id.PathIndex]);

                _productCode = Convert.ToInt32(e.DataDictionary[JetBusCommands.Product_code.PathIndex]);
                _serialNumber = Convert.ToInt32(e.DataDictionary[JetBusCommands.Serial_number.PathIndex]);
                _implementedProfileSpecification = Convert.ToInt32(e.DataDictionary[JetBusCommands.Implemented_profile_specification.PathIndex]);
                //_lcCapability = Convert.ToInt32(e.DataDictionary[JetBusCommands.LC_CAPABILITY.PathIndex]);
                _weighingDevice1UnitPrefixOutputParameter = Convert.ToInt32(e.DataDictionary[JetBusCommands.Weighing_device_1_unit_prefix_output_parameter.PathIndex]);             

                _weighingDevice1WeightStep = Convert.ToInt32(e.DataDictionary[JetBusCommands.Weighing_device_1_weight_step.PathIndex]);
                _alarms = Convert.ToInt32(e.DataDictionary[JetBusCommands.Alarms.PathIndex]);
                _weighingDevice1OutputWeight = Convert.ToInt32(e.DataDictionary[JetBusCommands.Weighing_device_1_output_weight.PathIndex]);
                _weighingDevice1Setting = Convert.ToInt32(e.DataDictionary[JetBusCommands.Weighing_device_1_setting.PathIndex]);

                _localGravityFactor = Convert.ToInt32(e.DataDictionary[JetBusCommands.Local_gravity_factor.PathIndex]);
                _scaleFilterSetup = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_filter_setup.PathIndex]);
                _dataSampleRate = Convert.ToInt32(e.DataDictionary[JetBusCommands.Data_sample_rate.PathIndex]);

                _filterOrderCriticallyDamped = Convert.ToInt32(e.DataDictionary[JetBusCommands.Filter_order_critically_damped.PathIndex]);
                _cutOffFrequencyCriticallyDamped = Convert.ToInt32(e.DataDictionary[JetBusCommands.Cut_off_frequency_critically_damped.PathIndex]);
                _filterOrderButterworth = Convert.ToInt32(e.DataDictionary[JetBusCommands.Filter_order_butterworth.PathIndex]);
                _cutOffFrequencyButterWorth = Convert.ToInt32(e.DataDictionary[JetBusCommands.Cut_off_frequency_butterworth.PathIndex]);
                _filterOrderBessel = Convert.ToInt32(e.DataDictionary[JetBusCommands.Filter_order_bessel.PathIndex]);

                _cutOffFrequencyBessel = Convert.ToInt32(e.DataDictionary[JetBusCommands.Cut_off_frequency_bessel.PathIndex]);
                _scaleSupplyNominalVoltage = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_suppy_nominal_voltage.PathIndex]);
                _scaleSupplyMinimumVoltage = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_suppy_minimum_voltage.PathIndex]);
                _scaleSupplyMaximumVoltage = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_suppy_maximum_voltage.PathIndex]);

                _scaleAccuracyClass = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_accuracy_class.PathIndex]);
                _scaleMinimumDeadLoad = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_minimum_dead_load.PathIndex]);
                _scaleMaximumCapacity = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_maximum_capacity.PathIndex]);
                _scaleMaximumNumberVerificationInterval = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_maximum_number_of_verification_interval.PathIndex]);
                _scaleApportionmentFactor = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_apportionment_factor.PathIndex]);
                _scaleSafeLoadLimit = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_safe_load_limit.PathIndex]);
                _scaleOperationNominalTemperature = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_operation_nominal_temperature.PathIndex]);
                _scaleOperationMinimumTemperature = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_operation_minimum_temperature.PathIndex]);
                _scaleOperationMaximumTemperature = Convert.ToInt32(e.DataDictionary[JetBusCommands.Scale_operation_maximum_temperature.PathIndex]);

                //_scaleRelativeMinimumLoadCellVerficationInterval = e.DataDictionary[JetBusCommands.Scale_relative_minimum_load_cell_verification_interval.PathIndex];
                _intervalRangeControl = Convert.ToInt32(e.DataDictionary[JetBusCommands.Interval_range_control.PathIndex]);
                _multiLimit1 = Convert.ToInt32(e.DataDictionary[JetBusCommands.Multi_limit_1.PathIndex]);
                _multiLimit2 = Convert.ToInt32(e.DataDictionary[JetBusCommands.Multi_limit_2.PathIndex]);
                //_oimlCertificationInformation = e.DataDictionary[JetBusCommands.Oiml_certificaiton_information.PathIndex];
                //_ntepCertificationInformation = e.DataDictionary[JetBusCommands.Ntep_certificaiton_information.PathIndex];
                _maximumZeroingTime = Convert.ToInt32(e.DataDictionary[JetBusCommands.Maximum_zeroing_time.PathIndex]);
                _maximumPeakValueGross = Convert.ToInt32(e.DataDictionary[JetBusCommands.Maximum_peak_value_gross.PathIndex]);
                _minimumPeakValueGross = Convert.ToInt32(e.DataDictionary[JetBusCommands.Minimum_peak_value_gross.PathIndex]);

                _maximumPeakValue = Convert.ToInt32(e.DataDictionary[JetBusCommands.Maximum_peak_value.PathIndex]);
                _minimumPeakValue = Convert.ToInt32(e.DataDictionary[JetBusCommands.Minimum_peak_value.PathIndex]);
                _weightMovingDetection = Convert.ToInt32(e.DataDictionary[JetBusCommands.Weight_moving_detection.PathIndex]);
                //_deviceAddress = e.DataDictionary[JetBusCommands.Device_address.PathIndex];
                //_hardwareVersion = e.DataDictionary[JetBusCommands.Hardware_version.PathIndex];
                //_identification = e.DataDictionary[JetBusCommands.Identification.PathIndex];

                _outputScale = Convert.ToInt32(e.DataDictionary[JetBusCommands.Output_scale.PathIndex]);
                //_firmwareDate = e.DataDictionary[JetBusCommands.Firmware_date.PathIndex];
                _resetTrigger = Convert.ToInt32(e.DataDictionary[JetBusCommands.Reset_trigger.PathIndex]);
                _stateDigital_IO_Extended = Convert.ToInt32(e.DataDictionary[JetBusCommands.State_digital_io_extended.PathIndex]);

                //_softwareIdentification = e.DataDictionary[JetBusCommands.Software_identification.PathIndex];
                //_softwareVersion = e.DataDictionary[JetBusCommands.Software_version.PathIndex];
                _dateTime = Convert.ToInt32(e.DataDictionary[JetBusCommands.Date_time.PathIndex]);

                _breakDosing = Convert.ToInt32(e.DataDictionary[JetBusCommands.Break_dosing.PathIndex]);
                _deleteDosingResult = Convert.ToInt32(e.DataDictionary[JetBusCommands.Delete_dosing_result.PathIndex]);
                _materialStreamLastDosing = Convert.ToInt32(e.DataDictionary[JetBusCommands.Material_stream_last_dosing.PathIndex]);
                _sum = Convert.ToInt32(e.DataDictionary[JetBusCommands.Sum.PathIndex]);
                _specialDosingFunctions = Convert.ToInt32(e.DataDictionary[JetBusCommands.Special_dosing_functions.PathIndex]);
                _dischargeTime = Convert.ToInt32(e.DataDictionary[JetBusCommands.Discharge_time.PathIndex]);
                _exceedingWeightBreak = Convert.ToInt32(e.DataDictionary[JetBusCommands.Exceeding_weight_break.PathIndex]);
                _delay1Dosing = Convert.ToInt32(e.DataDictionary[JetBusCommands.Delay1_dosing.PathIndex]);
                _delay2Dosing = Convert.ToInt32(e.DataDictionary[JetBusCommands.Delay2_dosing.PathIndex]);
                _emptyWeightTolerance = Convert.ToInt32(e.DataDictionary[JetBusCommands.Empty_weight_tolerance.PathIndex]);
                _residualFlowDosingCycle = Convert.ToInt32(e.DataDictionary[JetBusCommands.Residual_flow_dosing_cycle.PathIndex]);
            }
        }
        #endregion

        #region Properties for the filler extended data

        public int ErrorRegister
        {
            get { return _errorRegister; }
            set {_connection.Write(JetBusCommands.Error_register.PathIndex, value);
                this._errorRegister = value;
            }
        }
        public int SaveAllParameters
        {
            get { return _saveAllParameters; }
            set { _connection.Write(JetBusCommands.Save_all_parameters.PathIndex, value);
                 this._saveAllParameters = value; }
        }
        public int RestoreAllDefaultParameters
        {
            get { return _restoreAllDefaultParameters; }
            set { _connection.Write(JetBusCommands.Restore_all_default_parameters.PathIndex, value);
                this._restoreAllDefaultParameters = value; }
        }
        public int VendorID
        {
            get { return _vendorID; }
            set { _connection.Write(JetBusCommands.Vendor_id.PathIndex, value);
                this._vendorID = value; }
        }
        public int ProductCode
        {
            get { return _productCode; }
            set { _connection.Write(JetBusCommands.Product_code.PathIndex, value);
                this._productCode = value; }
        }
        public int SerialNumber
        {
            get { return _serialNumber; }
            set { _connection.Write(JetBusCommands.Serial_number.PathIndex, value);
                this._serialNumber = value; }
        }
        public int ImplementedProfileSpecification
        {
            get { return _implementedProfileSpecification; }
            set { _connection.Write(JetBusCommands.Implemented_profile_specification.PathIndex, value);
                this._implementedProfileSpecification = value; }
        }
        public int LcCapability
        {
            get { return _lcCapability; }
            set { _connection.Write(JetBusCommands.Lc_capability.PathIndex, value);
                this._lcCapability = value; }
        }
        public int WeighingDevice1UnitPrefixOutputParameter
        {
            get { return _weighingDevice1UnitPrefixOutputParameter; }
            set { _connection.Write(JetBusCommands.Weighing_device_1_unit_prefix_output_parameter.PathIndex, value);
                this._weighingDevice1UnitPrefixOutputParameter = value; }
        }
        public int WeighingDevice1WeightStep
        {
            get { return _weighingDevice1WeightStep; }
            set { _connection.Write(JetBusCommands.Weighing_device_1_weight_step.PathIndex, value);
                this._weighingDevice1WeightStep = value; }
        }
        public int Alarms
        {
            get { return _alarms; }
            set { _connection.Write(JetBusCommands.Alarms.PathIndex, value);
                this._alarms = value; }
        }
        public int WeighingDevice1OutputWeight
        {
            get { return _weighingDevice1OutputWeight; }
            set { _connection.Write(JetBusCommands.Weighing_device_1_output_weight.PathIndex, value);
                this._weighingDevice1OutputWeight = value; }
        }
        public int WeighingDevice1Setting
        {
            get { return _weighingDevice1Setting; }
            set { _connection.Write(JetBusCommands.Weighing_device_1_setting.PathIndex, value);
                this._weighingDevice1Setting = value; }
        }
        public int LocalGravityFactor
        {
            get { return _localGravityFactor; }
            set { _connection.Write(JetBusCommands.Local_gravity_factor.PathIndex, value);
                this._localGravityFactor = value; }
        }
        public int ScaleFilterSetup
        {
            get { return _scaleFilterSetup; }
            set { _connection.Write(JetBusCommands.Scale_filter_setup.PathIndex, value);
                this._scaleFilterSetup = value; }
        }
        public int DataSampleRate
        {
            get { return _dataSampleRate; }
            set { _connection.Write(JetBusCommands.Data_sample_rate.PathIndex, value);
                this._dataSampleRate = value; }
        }
        public int FilterOrderCriticallyDamped
        {
            get { return _filterOrderCriticallyDamped; }
            set { _connection.Write(JetBusCommands.Filter_order_critically_damped.PathIndex, value);
                this._filterOrderCriticallyDamped = value; }
        }
        public int CutOffFrequencyCriticallyDamped
        {
            get { return _cutOffFrequencyCriticallyDamped; }
            set { _connection.Write(JetBusCommands.Cut_off_frequency_critically_damped.PathIndex, value);
                this._cutOffFrequencyCriticallyDamped = value; }
        }
        public int FilterOrderButterworth
        {
            get { return _filterOrderButterworth; }
            set { _connection.Write(JetBusCommands.Filter_order_butterworth.PathIndex, value);
                this._filterOrderButterworth = value; }
        }
        public int CutOffFrequencyButterWorth
        {
            get { return _cutOffFrequencyButterWorth; }
            set { _connection.Write(JetBusCommands.Cut_off_frequency_butterworth.PathIndex, value);
                this._cutOffFrequencyButterWorth = value; }
        }
        public int FilterOrderBessel
        {
            get { return _filterOrderBessel; }
            set { _connection.Write(JetBusCommands.Filter_order_bessel.PathIndex, value);
                this._filterOrderBessel = value; }
        }
        public int CutOffFrequencyBessel
        {
            get { return _cutOffFrequencyBessel; }
            set { _connection.Write(JetBusCommands.Cut_off_frequency_bessel.PathIndex, value);
                this._cutOffFrequencyBessel = value; }
        }
        public int ScaleSupplyNominalVoltage
        {
            get { return _scaleSupplyNominalVoltage; }
            set { _connection.Write(JetBusCommands.Scale_suppy_nominal_voltage.PathIndex, value);
                this._scaleSupplyNominalVoltage = value; }
        }
        public int ScaleSupplyMinimumVoltage
        {
            get { return _scaleSupplyMinimumVoltage; }
            set { _connection.Write(JetBusCommands.Scale_suppy_minimum_voltage.PathIndex, value);
                this._scaleSupplyMinimumVoltage = value; }
        }
        public int ScaleSupplyMaximumVoltage
        {
            get { return _scaleSupplyMaximumVoltage; }
            set { _connection.Write(JetBusCommands.Scale_suppy_maximum_voltage.PathIndex, value);
                this._scaleSupplyMaximumVoltage = value; }
        }
        public int ScaleAccuracyClass
        {
            get { return _scaleAccuracyClass; }
            set { _connection.Write(JetBusCommands.Scale_accuracy_class.PathIndex, value);
                this._scaleAccuracyClass = value; }
        }
        public int ScaleMinimumDeadLoad
        {
            get { return _scaleMinimumDeadLoad; }
            set { _connection.Write(JetBusCommands.Scale_minimum_dead_load.PathIndex, value);
                this._scaleMinimumDeadLoad = value; }
        }
        public int ScaleMaximumCapacity
        {
            get { return _scaleMaximumCapacity; }
            set { _connection.Write(JetBusCommands.Scale_maximum_capacity.PathIndex, value);
                this._scaleMaximumCapacity = value; }
        }
        public int ScaleMaximumNumberVerificationInterval
        {
            get { return _scaleMaximumNumberVerificationInterval; }
            set { _connection.Write(JetBusCommands.Scale_maximum_number_of_verification_interval.PathIndex, value);
                this._scaleMaximumNumberVerificationInterval = value; }
        }
        public int ScaleApportionmentFactor
        {
            get { return _scaleApportionmentFactor; }
            set { _connection.Write(JetBusCommands.Scale_apportionment_factor.PathIndex, value);
                this._scaleApportionmentFactor = value; }
        }
        public int ScaleSafeLoadLimit
        {
            get { return _scaleSafeLoadLimit; }
            set { _connection.Write(JetBusCommands.Scale_safe_load_limit.PathIndex, value);
                this._scaleSafeLoadLimit = value; }
        }
        public int ScaleOperationNominalTemperature
        {
            get { return _scaleOperationNominalTemperature; }
            set { _connection.Write(JetBusCommands.Scale_operation_nominal_temperature.PathIndex, value);
                this._scaleOperationNominalTemperature = value; }
        }
        public int ScaleOperationMinimumTemperature
        {
            get { return _scaleOperationMinimumTemperature; }
            set { _connection.Write(JetBusCommands.Scale_operation_minimum_temperature.PathIndex, value);
                this._scaleOperationMinimumTemperature = value; }
        }
        public int ScaleOperationMaximumTemperature
        {
            get { return _scaleOperationMaximumTemperature; }
            set { _connection.Write(JetBusCommands.Scale_operation_maximum_temperature.PathIndex, value);
                this._scaleOperationMaximumTemperature = value; }
        }
        public int ScaleRelativeMinimumLoadCellVerficationInterval
        {
            get { return _scaleRelativeMinimumLoadCellVerficationInterval; }
            set { _connection.Write(JetBusCommands.Scale_relative_minimum_load_cell_verification_interval.PathIndex, value);
                this._scaleRelativeMinimumLoadCellVerficationInterval = value; }
        }
        public int IntervalRangeControl
        {
            get { return _intervalRangeControl; }
            set { _connection.Write(JetBusCommands.Interval_range_control.PathIndex, value);
                this._intervalRangeControl = value; }
        }
        public int MultiLimit1
        {
            get { return _multiLimit1; }
            set { _connection.Write(JetBusCommands.Multi_limit_1.PathIndex, value);
                this._multiLimit1 = value; }
        }
        public int MultiLimit2
        {
            get { return _multiLimit2; }
            set { _connection.Write(JetBusCommands.Multi_limit_2.PathIndex, value);
                this._multiLimit2 = value; }
        }
        public int OimlCertificationInformation
        {
            get { return _oimlCertificationInformation; }
            set { _connection.Write(JetBusCommands.Oiml_certificaiton_information.PathIndex, value);
                this._oimlCertificationInformation = value; }
        }
        public int NtepCertificationInformation
        {
            get { return _ntepCertificationInformation; }
            set { _connection.Write(JetBusCommands.Ntep_certificaiton_information.PathIndex, value);
                this._ntepCertificationInformation = value; }
        }
        public int MaximumZeroingTime
        {
            get { return _maximumZeroingTime; }
            set { _connection.Write(JetBusCommands.Maximum_zeroing_time.PathIndex, value);
                this._maximumZeroingTime = value; }
        }
        public int MaximumPeakValueGross
        {
            get { return _maximumPeakValueGross; }
            set { _connection.Write(JetBusCommands.Maximum_peak_value_gross.PathIndex, value);
                this._maximumPeakValueGross = value; }
        }
        public int MinimumPeakValueGross
        {
            get { return _minimumPeakValueGross; }
            set { _connection.Write(JetBusCommands.Minimum_peak_value_gross.PathIndex, value);
                this._minimumPeakValueGross = value; }
        }
        public int MaximumPeakValue
        {
            get { return _maximumPeakValue; }
            set { _connection.Write(JetBusCommands.Maximum_peak_value.PathIndex, value);
                this._maximumPeakValue = value; }
        }
        public int MinimumPeakValue
        {
            get { return _minimumPeakValue; }
            set { _connection.Write(JetBusCommands.Minimum_peak_value.PathIndex, value);
                this._minimumPeakValue = value; }
        }
        public int WeightMovingDetection
        {
            get { return _weightMovingDetection; }
            set { _connection.Write(JetBusCommands.Weight_moving_detection.PathIndex, value);
                this._weightMovingDetection = value; }
        }
        public int DeviceAddress
        {
            get { return _deviceAddress; }
            set { _connection.Write(JetBusCommands.Device_address.PathIndex, value);
                this._deviceAddress = value; }
        }
        public int HardwareVersion
        {
            get { return _hardwareVersion; }
            set { _connection.Write(JetBusCommands.Hardware_version.PathIndex, value);
                this._hardwareVersion = value; }
        }
        public int Identification
        {
            get { return _identification; }
            set { _connection.Write(JetBusCommands.Identification.PathIndex, value);
                this._identification = value; }
        }     
        public int OutputScale
        {
            get { return _outputScale; }
            set { _connection.Write(JetBusCommands.Output_scale.PathIndex, value);
                this._outputScale = value; }
        }
        public int FirmwareDate
        {
            get { return _firmwareDate; }
            set { _connection.Write(JetBusCommands.Firmware_date.PathIndex, value);
                this._firmwareDate = value; }
        }
        public int ResetTrigger
        {
            get { return _resetTrigger; }
            set { _connection.Write(JetBusCommands.Reset_trigger.PathIndex, value);
                this._resetTrigger = value; }
        }
        public int StateDigital_IO_Extended
        {
            get { return _stateDigital_IO_Extended; }
            set { _connection.Write(JetBusCommands.State_digital_io_extended.PathIndex, value);
                this._stateDigital_IO_Extended = value; }
        }
        public int SoftwareIdentification
        {
            get { return _softwareIdentification; }
            set { _connection.Write(JetBusCommands.Software_identification.PathIndex, value);
                this._softwareIdentification = value; }
        }
        public int SoftwareVersion
        {
            get { return _softwareVersion; }
            set { _connection.Write(JetBusCommands.Software_version.PathIndex, value);
                this._softwareVersion = value; }
        }
        public int DateTime
        {
            get { return _dateTime; }
            set { _connection.Write(JetBusCommands.Date_time.PathIndex, value);
                this._dateTime = value; }
        }
        public int BreakDosing
        {
            get { return _breakDosing; }
            set { _connection.Write(JetBusCommands.Break_dosing.PathIndex, value);
                this._breakDosing = value; }
        }
        public int DeleteDosingResult
        {
            get { return _deleteDosingResult; }
            set { _connection.Write(JetBusCommands.Delete_dosing_result.PathIndex, value);
                this._deleteDosingResult = value; }
        }
        public int MaterialStreamLastDosing
        {
            get { return _materialStreamLastDosing; }
            set { _connection.Write(JetBusCommands.Material_stream_last_dosing.PathIndex, value);
                this._materialStreamLastDosing = value; }
        }
        public int Sum
        {
            get { return _sum; }
            set { _connection.Write(JetBusCommands.Sum.PathIndex, value);
                this._sum = value; }
        }
        public int SpecialDosingFunctions
        {
            get { return _specialDosingFunctions; }
            set { _connection.Write(JetBusCommands.Special_dosing_functions.PathIndex, value);
                this._specialDosingFunctions = value; }
        }
        public int DischargeTime
        {
            get { return _dischargeTime; }
            set { _connection.Write(JetBusCommands.Discharge_time.PathIndex, value);
                this._dischargeTime = value; }
        }
        public int ExceedingWeightBreak
        {
            get { return _exceedingWeightBreak; }
            set { _connection.Write(JetBusCommands.Exceeding_weight_break.PathIndex, value);
                this._exceedingWeightBreak = value; }
        }
        public int Delay1Dosing
        {
            get { return _delay1Dosing; }
            set { _connection.Write(JetBusCommands.Delay1_dosing.PathIndex, value);
                this._delay1Dosing = value; }
        }
        public int Delay2Dosing
        {
            get { return _delay2Dosing; }
            set { _connection.Write(JetBusCommands.Delay2_dosing.PathIndex, value);
                this._delay2Dosing = value; }
        }
        public int EmptyWeightTolerance
        {
            get { return _emptyWeightTolerance; }
            set { _connection.Write(JetBusCommands.Empty_weight_tolerance.PathIndex, value);
                this._emptyWeightTolerance = value; }
        }
        public int ResidualFlowDosingCycle
        {
            get { return _residualFlowDosingCycle; }
            set { _connection.Write(JetBusCommands.Residual_flow_dosing_cycle.PathIndex, value);
                this._residualFlowDosingCycle = value; }
        }

        #endregion

    }
}