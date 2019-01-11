using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    public interface IDataFiller
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

        int ResidualFlowTime { get; set; }
        int TargetFillingWeight { get;  set; }
        int CoarseFlowCutOffPointSet { get;  set; }
        int FineFlowCutOffPointSet { get;  set; }

        int MinimumFineFlow { get;  set; }
        int OptimizationOfCutOffPoints { get;  set; }
        int MaximumDosingTime { get;  set; }
        int StartWithFineFlow { get;  set; }

        int CoarseLockoutTime { get;  set; }
        int FineLockoutTime { get;  set; }
        int TareMode { get;  set; }
        int UpperToleranceLimit { get;  set; }

        int LowerToleranceLimit { get;  set; }
        int MinimumStartWeight { get;  set; }
        int EmptyWeight { get;  set; }
        int TareDelay { get;  set; }

        int CoarseFlowMonitoringTime { get;  set; }
        int CoarseFlowMonitoring { get;  set; }
        int FineFlowMonitoring { get;  set; }
        int FineFlowMonitoringTime { get;  set; }

        int DelayTimeAfterFineFlow { get;  set; }
        int ActivationTimeAfterFineFlow { get;  set; }
        int SystematicDifference { get;  set; }
        int DownwardsDosing { get;  set; }

        int ValveControl { get;  set; }
        int EmptyingMode { get;  set; }
        #endregion

        #region Filler device data - Input and output words

        int FineFlowCutOffPoint { get; set; }
        int CoarseFlowCutOffPoint { get; set; }

        #endregion

        #region Update methods for the data of standard mode

        void UpdateFillerDataModbus(ushort[] _dataParam);
        void UpdateFillerDataJet(Dictionary<string, int> _dataParam);

        #endregion
    }
}