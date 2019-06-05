// <copyright file="IDataFillerExtended.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hbm.Weighing.API.Data
{
    /// <summary>
    /// Interface containing the data for the filler extended mode of your WTX device.
    /// A class inheriting from interface IDataFillerExtended contains the input word 
    /// and output words for the extended filler mode of WTX device 120 and 110.
    /// 
    /// This is only available via a JetBus Ethernet connection, not via Modbus. 
    /// </summary>
    public interface IDataFillerExtended : IDataFiller
    {

        #region ==================== events & delegates ====================

        void UpdateFillerExtendedData(object sender, EventArgs e);

        #endregion

        #region ======================== properties ========================

        int ErrorRegister { get; set; }
        int SaveAllParameters { get; set; }
        int RestoreAllDefaultParameters { get; set; }
        int VendorID { get; set; }
        int ProductCode { get; set; }
        int SerialNumber { get; set; }
        int ImplementedProfileSpecification { get; set; }
        int LcCapability { get; set; }
        int WeighingDevice1UnitPrefixOutputParameter{ get; set; }
        
        int WeighingDevice1WeightStep { get; set; }
        int Alarms { get; set; }
        int WeighingDevice1OutputWeight { get; set; }
        int WeighingDevice1Setting { get; set; }
        int LocalGravityFactor { get; set; }
        int ScaleFilterSetup { get; set; }
        int DataSampleRate { get; set; }
        int FilterOrderCriticallyDamped { get; set; }
        int CutOffFrequencyCriticallyDamped { get; set; }
        int FilterOrderButterworth { get; set; }
        int CutOffFrequencyButterWorth { get; set; }
        int FilterOrderBessel { get; set; }
        int CutOffFrequencyBessel { get; set; }
        int ScaleSupplyNominalVoltage { get; set; }
        int ScaleSupplyMinimumVoltage { get; set; }
        int ScaleSupplyMaximumVoltage { get; set; }
        int ScaleAccuracyClass { get; set; }
        int ScaleMinimumDeadLoad { get; set; }
        int ScaleMaximumCapacity { get; set; }
        
        int ScaleMaximumNumberVerificationInterval { get; set; }
        int ScaleApportionmentFactor { get; set; }
        int ScaleSafeLoadLimit { get; set; }
        int ScaleOperationNominalTemperature { get; set; }
        int ScaleOperationMinimumTemperature { get; set; }
        int ScaleOperationMaximumTemperature { get; set; }
        int ScaleRelativeMinimumLoadCellVerficationInterval { get; set; }
        int IntervalRangeControl { get; set; }
        int MultiLimit1 { get; set; }
        int MultiLimit2 { get; set; }
        int MaximumZeroingTime { get; set; }
        int MaximumPeakValueGross { get; set; }
        int MinimumPeakValueGross { get; set; }

        int MaximumPeakValue { get; set; }
        int MinimumPeakValue { get; set; }
        int WeightMovingDetection{ get; set; }
        int DeviceAddress { get; set; }

        int OutputScale { get; set; }
        int ResetTrigger { get; set; }
        int StateDigital_IO_Extended { get; set; }  
        int DateTime { get; set; }
        int BreakDosing { get; set; }
        int DeleteDosingResult { get; set; }
        int MaterialStreamLastDosing { get; set; }
        int Sum { get; set; }
        int SpecialDosingFunctions { get; set; }
        int DischargeTime { get; set; }
        int ExceedingWeightBreak { get; set; }
        int Delay1Dosing { get; set; }
        int Delay2Dosing { get; set; }
        int EmptyWeightTolerance { get; set; }
        int ResidualFlowDosingCycle { get; set; }

        string OimlCertificationInformation { get; }
        string NtepCertificationInformation { get; }
        string HardwareVersion { get; }
        string Identification { get; }
        string FirmwareDate { get; }
        string SoftwareIdentification { get; }
        string SoftwareVersion { get; }

        #endregion

    }
}
