using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Jet
{
    /// <summary>
    /// ID's/Commands for subscribing values of the WTX device. 
    /// The ID's are commited as a parameter a the read and/or write method call.  
    /// </summary>
    struct JetBusCommands
    {
        public const string NET_VALUE = "601A/01";
        public const string GROSS_VALUE = "6144/00";

        public const string ZERO_VALUE = "6142/00";
        public const string TARE_VALUE = "6143/00";

        public const string DECIMALS = "6013/01";
        public const string DOSING_COUNTER = "NDS";
        public const string DOSING_STATUS = "SDO";
        public const string DOSING_RESULT = "FRS1";

        public const string WEIGHING_DEVICE_1_WEIGHT_STATUS = "6012/01";

        public const string SCALE_COMMAND = "6002/01";
        public const string SCALE_COMMAND_STATUS = "6002/02";

        public const string LDW_DEAD_WEIGHT = "2110/06";
        public const string LWT_NOMINAL_VALUE = "2110/07";

        public const string LFT_SCALE_CALIBRATION_WEIGHT = "6152/00";

        public const string UNIT_PREFIX_FIXED_PARAMETER = "6014/01";

        public const string FUNCTION_DIGITAL_INPUT_1 = "IM1";
        public const string FUNCTION_DIGITAL_INPUT_2 = "IM2";
        public const string FUNCTION_DIGITAL_INPUT_3 = "IM3";
        public const string FUNCTION_DIGITAL_INPUT_4 = "IM4";

        public const string FUNCTION_DIGITAL_OUTPUT_1 = "OM1";
        public const string FUNCTION_DIGITAL_OUTPUT_2 = "OM2";
        public const string FUNCTION_DIGITAL_OUTPUT_3 = "OM3";
        public const string FUNCTION_DIGITAL_OUTPUT_4 = "OM4";

        public const string STATUS_DIGITAL_OUTPUT_1 = "OS1";
        public const string STATUS_DIGITAL_OUTPUT_2 = "OS2";
        public const string STATUS_DIGITAL_OUTPUT_3 = "OS3";
        public const string STATUS_DIGITAL_OUTPUT_4 = "OS4";

        public const string COARSE_FLOW_TIME = "CFT";
        public const string FINE_FLOW_TIME = "FFT";
        public const string TARE_MODE = "TMD";
        public const string UPPER_TOLERANCE_LIMIT = "UTL";
        public const string LOWER_TOLERANCE_LOMIT = "LTL";
        public const string MINIMUM_START_WEIGHT = "MSW";
        public const string EMPTY_WEIGHT = "EWT";
        public const string TARE_DELAY = "TAD";
        public const string COARSE_FLOW_MONITORING_TIME = "CBT";
        public const string COARSE_FLOW_MONITORING = "CBK";
        public const string FINE_FLOW_MONITORING = "FBK";
        public const string FINE_FLOW_MONITORING_TIME = "FBT";
        public const string SYSTEMATIC_DIFFERENCE = "SYD";
        public const string VALVE_CONTROL = "VCT";
        public const string EMPTYING_MODE = "EMD";
        public const string COARSE_FLOW_CUT_OFF_POINT = "CFD";
        public const string FINE_FLOW_CUT_OFF_POINT = "FFD";
        public const string MEAN_VALUE_DOSING_RESULTS = "SDM";
        public const string STANDARD_DEVIATION = "SDS";
        public const string RESIDUAL_FLOW_TIME = "RFT";

        public const string MAXIMUM_DOSING_TIME = "MDT";
        public const string MINIMUM_FINE_FLOW = "FFM";
        public const string OPTIMIZATION = "OSN";

        public const string FINEFLOW_PHASE_BEFORE_COARSEFLOW = "FFL";
        public const string DELAY1_DOSING = "DL1";

        public const string LIMIT_VALUE = "2020/25";
    }
}
