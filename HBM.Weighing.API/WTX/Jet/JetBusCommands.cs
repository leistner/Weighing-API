using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Jet
{
    /// <summary>
    /// ID's/Commands for subscribing values of the WTX device. 
    /// The ID's are commited as a parameter for the read and/or write method call.  
    /// </summary>
    struct JetBusCommands
    {
        #region ID commands for process data
        public const string NET_VALUE = "601A/01";
        public const string GROSS_VALUE = "6144/00";
        public const string ZERO_VALUE = "6142/00";

        public const string TARE_VALUE = "6143/00";
        public const string WEIGHING_DEVICE_1_WEIGHT_STATUS = "6012/01";
        public const string UNIT_PREFIX_FIXED_PARAMETER = "6014/01";

        public const string DECIMALS = "6013/01";
        public const string SCALE_COMMAND = "6002/01";
        public const string SCALE_COMMAND_STATUS = "6002/02";
        #endregion

        #region ID commands for standard mode
        public const string STATUS_DIGITAL_INPUT_1 = "2020/18";    // IS1
        public const string STATUS_DIGITAL_INPUT_2 = "2020/19";    // IS2
        public const string STATUS_DIGITAL_INPUT_3 = "2020/1A";    // IS3
        public const string STATUS_DIGITAL_INPUT_4 = "2020/1B";    // IS4

        public const string STATUS_DIGITAL_OUTPUT_1 = "2020/1E";   // OS1
        public const string STATUS_DIGITAL_OUTPUT_2 = "2020/1F";   // OS2
        public const string STATUS_DIGITAL_OUTPUT_3 = "2020/20";   // OS3
        public const string STATUS_DIGITAL_OUTPUT_4 = "2020/21";   // OS4

        public const string LIMIT_VALUE = "2020/25";   // LVS

        #endregion

        #region ID commands for filler data
        public const string DOSING_COUNTER = "2230/05";
        public const string DOSING_STATUS = "2D00/02";  // SDO
        public const string DOSING_RESULT = "2000/05";  // FRS1      
        public const string LDW_DEAD_WEIGHT = "2110/06";

        public const string LWT_NOMINAL_VALUE = "2110/07";
        public const string LFT_SCALE_CALIBRATION_WEIGHT = "6152/00";
        public const string FUNCTION_DIGITAL_INPUT_1 = "2022/01";  // IM1
        public const string FUNCTION_DIGITAL_INPUT_2 = "2022/02";  // IM2

        public const string FUNCTION_DIGITAL_INPUT_3 = "2022/03";  // IM3
        public const string FUNCTION_DIGITAL_INPUT_4 = "2022/04";  // IM4
        public const string FUNCTION_DIGITAL_OUTPUT_1 = "2021/01"; // OM1
        public const string FUNCTION_DIGITAL_OUTPUT_2 = "2021/02"; // OM2

        public const string FUNCTION_DIGITAL_OUTPUT_3 = "2021/03"; // OM3
        public const string FUNCTION_DIGITAL_OUTPUT_4 = "2021/04"; // OM4
        public const string COARSE_FLOW_TIME = "2230/01";            // CFT
        public const string FINE_FLOW_TIME = "2230/04";              // FFT

        public const string TARE_MODE = "2200/0B";                   // TMD
        public const string UPPER_TOLERANCE_LIMIT = "2210/0A";       // UTL
        public const string LOWER_TOLERANCE_LOMIT = "2210/08";       // LTL
        public const string MINIMUM_START_WEIGHT = "2210/0B";        // MSW

        public const string EMPTY_WEIGHT = "2210/03";                // EWT
        public const string TARE_DELAY = "2220/09";                  // TAD
        public const string COARSE_FLOW_MONITORING_TIME = "2220/01"; // CBT
        public const string COARSE_FLOW_MONITORING = "2210/01";      // CBK

        public const string FINE_FLOW_MONITORING = "2210/04";        // FBK
        public const string FINE_FLOW_MONITORING_TIME = "2220/03";   // FBT
        public const string SYSTEMATIC_DIFFERENCE = "2210/09";       // SYD
        public const string VALVE_CONTROL = "2200/0C";               // VCT

        public const string EMPTYING_MODE = "2200/05";               // EMD
        public const string COARSE_FLOW_CUT_OFF_POINT = "2210/02";   // CFD
        public const string FINE_FLOW_CUT_OFF_POINT = "2210/05";     // FFD
        public const string MEAN_VALUE_DOSING_RESULTS = "2230/06";   // SDM

        public const string STANDARD_DEVIATION = "2230/07";          // SDS
        public const string RESIDUAL_FLOW_TIME = "2220/07";          // RFT
        public const string MAXIMUM_DOSING_TIME = "2220/06";         // MDT
        public const string MINIMUM_FINE_FLOW = "2210/06";           // FFM

        public const string OPTIMIZATION = "2200/07";                // OSN
        public const string FINEFLOW_PHASE_BEFORE_COARSEFLOW = "2220/0A"; // FFL
        public const string DELETE_DOSING_RESULT = "2230/02";             // CSN
        public const string DELAY1_DOSING = "2220/0B"; // DL1
        public const string DELAY2_DOSING = "2220/0C"; // DL2

        #endregion

        #region ID commands for filler extended data

        #endregion
    }
}
