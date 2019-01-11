using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    interface IDataStandard 
    {
        #region Input words for standard mode

        int Input1 { get; }
        int Input2 { get; }
        int Input3 { get; }
        int Input4 { get; }

        int Output1 { get; }
        int Output2 { get; }
        int Output3 { get; }
        int Output4 { get; }

        int LimitValue1 { get; }
        int LimitValue2 { get; }
        int LimitValue3 { get; }
        int LimitValue4 { get; }

        int WeightMemDay      { get; set; }
        int WeightMemMonth    { get; set; }
        int WeightMemYear     { get; set; }
        int WeightMemSeqNumber{ get; set; }
        int WeightMemGross    { get; set; }
        int WeightMemNet      { get; set; }

        #endregion

        #region Output words for standard mode 

        int ManualTareValue { get;  set; }
        int LimitValue1Input { get;  set; }
        int LimitValue1Mode { get;  set; }

        int LimitValue1ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue1HysteresisBandHeight { get;  set; }
        int LimitValue2Source { get;  set; }
        int LimitValue2Mode { get;  set; }

        int LimitValue2ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue2HysteresisBandHeight { get;  set; }
        int LimitValue3Source { get;  set; }
        int LimitValue3Mode { get;  set; }

        int LimitValue3ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue3HysteresisBandHeight { get;  set; }
        int LimitValue4Source { get;  set; }

        int LimitValue4Mode { get;  set; }
        int LimitValue4ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue4HysteresisBandHeight { get;  set; }

        int CalibrationWeight { get;  set; }
        int ZeroLoad { get;  set; }
        int NominalLoad { get;  set; }

        #endregion
    }
}
