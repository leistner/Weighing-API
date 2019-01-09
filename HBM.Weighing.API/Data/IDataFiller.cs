using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    interface IDataFiller : IDataStandard
    {
        //Filler process data
        int CoarseFlow { get; set; }
        int FineFlow { get; set; }
        int Ready { get; set; }
        int ReDosing { get; set; }
        int Emptying { get; set; }
        int FlowError { get; set; }
        int Alarm { get; set; }
        int AdcOverUnderload { get; set; }
        int FillingProcessStatus { get; set; }
        int NumberDosingResults { get; set; }
        int DosingResult { get; set; }
        int MeanValueDosingResults { get; set; }
        int StandardDeviation { get; set; }
        int TotalWeight { get; set; }
        int CurrentDosingTime { get; set; }
        int CurrentCoarseFlowTime { get; set; }
        int CurrentFineFlowTime { get; set; }
        int ToleranceErrorPlus { get; set; }
        int ToleranceErrorMinus { get; set; }

        int StatusInput1 { get; set; }
        int GeneralScaleError { get; set; }
        int ManualTareValue { get; set; }
        //int LegalTradeOp { get; set; }

        #region Limit switch devicde data
        int LimitValue1Input { get; set; }
        int LimitValue1Mode { get; set; }
        int LimitValue1ActivationLevelLowerBandLimit { get; set; }
        int LimitValue1HysteresisBandHeight { get; set; }
        int LimitValue2Source { get; set; }
        int LimitValue2Mode { get; set; }
        int LimitValue2ActivationLevelLowerBandLimit { get; set; }
        int LimitValue2HysteresisBandHeight { get; set; }
        int LimitValue3Source { get; set; }
        int LimitValue3Mode { get; set; }
        int LimitValue3ActivationLevelLowerBandLimit { get; set; }
        int LimitValue3HysteresisBandHeight { get; set; }
        int LimitValue4Source { get; set; }
        int LimitValue4Mode { get; set; }
        int LimitValue4ActivationLevelLowerBandLimit { get; set; }
        int LimitValue4HysteresisBandHeight { get; set; }
        #endregion

        int FineFlowCutOffPoint { get; set; }
        int CoarseFlowCutOffPoint { get; set; }
        int ParameterSetProduct { get; set; }
        int MaxDosingTime { get; set; }
        int LegalForTradeOperation { get; set; }

        #region Filler device data
        int ResidualFlowTime { get; set; }
        int TargetFillingWeight { get; set; }
        int CoarseFlowCutOffPointSet { get; set; }
        int FineFlowCutOffPointSet { get; set; }
        int MinimumFineFlow { get; set; }
        int OptimizationOfCutOffPoints { get; set; }
        int MaximumDosingTime { get; set; }
        int StartWithFineFlow { get; set; }
        int CoarseLockoutTime { get; set; }
        int FineLockoutTime { get; set; }
        int TareMode { get; set; }
        int UpperToleranceLimit { get; set; }
        int LowerToleranceLimit { get; set; }
        int MinimumStartWeight { get; set; }
        int EmptyWeight { get; set; }
        int TareDelay { get; set; }
        int CoarseFlowMonitoringTime { get; set; }
        int CoarseFlowMonitoring { get; set; }
        int FineFlowMonitoring { get; set; }
        int FineFlowMonitoringTime { get; set; }
        int DelayTimeAfterFineFlow { get; set; }
        int ActivationTimeAfterFineFlow { get; set; }
        int SystematicDifference { get; set; }
        int DownwardsDosing { get; set; }
        int ValveControl { get; set; }
        int EmptyingMode { get; set; }
        #endregion
    }
}
