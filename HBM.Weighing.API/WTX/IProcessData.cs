using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// Stand : 11-12-2018

namespace HBM.Weighing.API.WTX
{
    // According to OPC-UA Standard: 
    /*
    string WeightId
    bool GrossNegativ
    bool CenterOfZero
    Enum TareMode
    bool WeightStable
    uint CurrentRangeId
    bool Overload
    bool Underload
    bool InsideZero
    bool Invalid
    bool LegalForTrade
    'PrintableWeightType erbt von AbstractWeightType' PrintableValue
    'WeightType erbt von AbstractWeightType' HighResolutionValue
    */

    interface IProcessData
    {
        double NetValue // Gross value of weight 
        {
            get;
        }

        double GrossValue // Net value of weight
        {
            get;
        }

        double Tare // Tare value of weight
        {
            get;
        }

        bool GeneralWeightError
        {
            get;
        }

        bool ScaleAlarmTriggered
        {
            get;
        }

        int LimitStatus
        {
            get;
        }

        bool WeightMoving  // = WeightStable (OPC-UA Standard)
        {
            get;
        }

        bool ScaleSealIsOpen
        {
            get;
        }

        bool ManualTare
        {
            get;
        }

        bool WeightType
        {
            get;
        }

        int ScaleRange // = CurrentRangeId (OPC-UA Standard)
        {
            get;
        }

        bool ZeroRequired
        {
            get;
        }

        bool WeightWithinTheCenterOfZero // = CenterOfZero (OPC-UA Standard)
        {
            get;
        }

        bool WeightInZeroRange // = Inside zero (OPC-UA Standard)
        {
            get;
        }

        int ApplicationMode
        {
            get;
        }

        int Decimals
        {
            get;
        }

        int Unit
        {
            get;
        }

        bool Handshake
        {
            get;
        }

        bool Status
        {
            get;
        }


        bool Underload // = Underload (OPC-UA Standard)
        {
            get;
        }

        bool Overload // = Overload (OPC-UA Standard)
        {
            get;
        }

        bool weightWithinLimits 
        {
            get;
        }

        bool higherSafeLoadLimit
        {
            get;
        }

        int LegalTradeOp // = LegalForTrade (OPC-UA Standard)
        {
            get;
        }

    }
}