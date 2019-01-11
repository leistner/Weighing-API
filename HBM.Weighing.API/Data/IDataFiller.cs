using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    interface IDataFiller
    {
        #region Filler device data - Input words

        int CoarseFlow { get; }
        int FineFlow { get; }
        int Ready { get; }
        int ReDosing { get; }

        int Emptying { get; }
        int FlowError { get; }
        int Alarm { get; }
        int AdcOverUnderload { get; }

        int FillingProcessStatus { get; }
        int NumberDosingResults { get; }
        int DosingResult { get; }
        int MeanValueDosingResults { get; }

        int StandardDeviation { get; }
        int TotalWeight { get; }
        int CurrentDosingTime { get; }
        int CurrentCoarseFlowTime { get; }

        int CurrentFineFlowTime { get; }
        int ToleranceErrorPlus { get; }
        int ToleranceErrorMinus { get; }
        int StatusInput1 { get; }

        int GeneralScaleError { get; }
        int ParameterSetProduct { get; }
        int MaxDosingTime { get; }
        int LegalForTradeOperation { get; }

        int WeightMemDay { get; }
        int WeightMemMonth { get; }
        int WeightMemYear { get; }
        int WeightMemSeqNumber { get; }
        int WeightMemGross { get; }
        int WeightMemNet { get; }

        #endregion

        #region Filler device data - Output words

        int ResidualFlowTime { set; }
        int TargetFillingWeight { set; }
        int CoarseFlowCutOffPointSet { set; }
        int FineFlowCutOffPointSet { set; }

        int MinimumFineFlow { set; }
        int OptimizationOfCutOffPoints { set; }
        int MaximumDosingTime { set; }
        int StartWithFineFlow { set; }

        int CoarseLockoutTime { set; }
        int FineLockoutTime { set; }
        int TareMode { set; }
        int UpperToleranceLimit { set; }

        int LowerToleranceLimit { set; }
        int MinimumStartWeight { set; }
        int EmptyWeight { set; }
        int TareDelay { set; }

        int CoarseFlowMonitoringTime { set; }
        int CoarseFlowMonitoring { set; }
        int FineFlowMonitoring { set; }
        int FineFlowMonitoringTime { set; }

        int DelayTimeAfterFineFlow { set; }
        int ActivationTimeAfterFineFlow { set; }
        int SystematicDifference { set; }
        int DownwardsDosing { set; }

        int ValveControl { set; }
        int EmptyingMode { set; }
        #endregion

        #region Filler device data - Input and output words

        int FineFlowCutOffPoint { get; set; }
        int CoarseFlowCutOffPoint { get; set; }

        #endregion

    }
}