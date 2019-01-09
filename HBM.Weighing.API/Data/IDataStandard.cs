using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    interface IDataStandard 
    {
        int NetValue // Net value of weight 
        {
            get; set;
        }

        string NetValueStr
        {
            get; set;
        }

        int GrossValue // Gross value of weight
        {
            get; set;
        }

        string GrossValueStr
        {
            get; set;
        }

        int TareValue // Tare value of weight
        {
            get; set;
        }

        bool GeneralWeightError
        {
            get; set;
        }

        bool ScaleAlarmTriggered
        {
            get; set;
        }

        int LimitStatus
        {
            get; set;
        }

        bool WeightMoving  // = WeightStable (OPC-UA Standard)
        {
            get; set;
        }

        bool ScaleSealIsOpen
        {
            get; set;
        }

        bool ManualTare
        {
            get; set;
        }

        bool WeightType
        {
            get; set;
        }

        int ScaleRange // = CurrentRangeId (OPC-UA Standard)
        {
            get; set;
        }

        bool ZeroRequired
        {
            get; set;
        }

        bool WeightWithinTheCenterOfZero // = CenterOfZero (OPC-UA Standard)
        {
            get; set;

        }

        bool WeightInZeroRange // = Inside zero (OPC-UA Standard)
        {
            get; set;
        }

        int ApplicationMode
        {
            get; set;
        }
        string ApplicationModeStr
        {
            get; set;
        }

        int Decimals
        {
            get; set;
        }

        int Unit
        {
            get; set;
        }

        bool Handshake
        {
            get; set;
        }

        bool Status
        {
            get; set;
        }


        bool Underload // = Underload (OPC-UA Standard)
        {
            get; set;
        }

        bool Overload // = Overload (OPC-UA Standard)
        {
            get; set;
        }

        bool WeightWithinLimits
        {
            get; set;
        }

        bool HigherSafeLoadLimit
        {
            get; set;
        }

        int LegalTradeOp // = LegalForTrade (OPC-UA Standard)
        {
            get; set;
        }

        int Input1 
        {
            get; set;
        }
        int Input2
        {
            get; set;
        }
        int Input3
        {
            get; set;
        }
        int Input4
        {
            get; set;
        }
        int Output1
        {
            get; set;
        }
        int Output2
        {
            get; set;
        }
        int Output3
        {
            get; set;
        }
        int Output4
        {
            get; set;
        }
        int LimitValue1
        {
            get; set;
        }
        int LimitValue2
        {
            get; set;
        }
        int LimitValue3
        {
            get; set;
        }
        int LimitValue4
        {
            get; set;
        }

        int WeightMemDay
        {
            get; set;
        }     
        
        int WeightMemMonth
        {
            get; set;
        }        

        int WeightMemYear
        {
            get; set;
        }        

        int WeightMemSeqNumber
        {
            get; set;
        }   

        int WeightMemGross
        {
            get; set;
        }     
        
        int WeightMemNet
        {
            get; set;
        }        
    }
}
