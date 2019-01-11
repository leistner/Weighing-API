using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    public interface IDataFillerExtended : IDataFiller
    {
        #region Output words for the data of filler extended mode

        int ResidualFlowTime { get;  set; }
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

        #region Update methods for the data of filler extended mode

        void UpdateFillerExtendedDataModbus(ushort[] _dataParam);
        void UpdateFillerExtendedDataJet(Dictionary<string, int> _dataParam);

        #endregion
    }
}
