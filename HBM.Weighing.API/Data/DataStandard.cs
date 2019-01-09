using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    public class DataStandard : IDataStandard
    {
        #region privates for standard mode

        private ushort[] _data;

        private int _netValue;      // data type = double according to OPC-UA standard
        private int _grossValue;    // data type = double according to OPC-UA standard
        private string _netValueStr;
        private string _grossValueStr;
        private int _tareValue;          // data type = double according to OPC-UA standard
        private bool _generalWeightError;
        private bool _scaleAlarmTriggered;
        private int _limitStatus;
        private bool _weightMoving;
        private bool _scaleSealIsOpen;
        private bool _manualTare;
        private bool _weightType;
        private int _scaleRange;
        private bool _zeroRequired;
        private bool _weightWithinTheCenterOfZero;
        private bool _weightInZeroRange;
        private int _applicationMode;
        private string _applicationModeStr;
        private int _decimals;
        private int _unit;
        private bool _handshake;
        private bool _status;
        private bool _underload;
        private bool _overload;
        private bool _weightWithinLimits;
        private bool _higherSafeLoadLimit;
        private int _legalTradeOp;

        private int _input1;
        private int _input2;
        private int _input3;
        private int _input4;

        private int _output1;
        private int _output2;
        private int _output3;
        private int _output4;

        private int _limitValue1;
        private int _limitValue2;
        private int _limitValue3;
        private int _limitValue4;

        private int _weightMemDay;
        private int _weightMemMonth;
        private int _weightMemYear;
        private int _weightMemSeqNumber;
        private int _weightMemGross;
        private int _weightMemNet;

        #endregion

        #region constructor

        public DataStandard()
        {
            _netValue = 0;
            _grossValue = 0;

            _tareValue = 0;
            _generalWeightError = false;
            _scaleAlarmTriggered = false;
            _limitStatus = 0;
            _weightMoving = false;
            _scaleSealIsOpen = false;
            _manualTare = false;
            _weightType = false;
            _scaleRange = 0;
            _zeroRequired = false;
            _weightWithinTheCenterOfZero = false;
            _weightInZeroRange = false;
            _applicationMode = 0;
            _applicationModeStr = "";
            _decimals = 0;
            _unit = 0;
            _handshake = false;
            _status = false;
            _underload = false;
            _overload = false;
            _weightWithinLimits = false;
            _higherSafeLoadLimit = false;
            _legalTradeOp = 0;

            _input1 =0;
            _input2=0;
            _input3=0;
            _input4=0;

            _output1=0;
            _output2=0;
            _output3=0;
            _output4=0;

            _limitValue1=0;
            _limitValue2=0;
            _limitValue3=0;
            _limitValue4=0;

            _weightMemDay=0;
            _weightMemMonth=0;
            _weightMemYear=0;
            _weightMemSeqNumber=0;
            _weightMemGross=0;
            _weightMemNet=0;
    }

        #endregion

        #region Update methods for standard mode

        public void UpdateStandardData(ushort[] dataParam)
        {
            this._data = dataParam;

            _netValue = _data[1] + (_data[0] << 16);
            _grossValue = _data[3] + (_data[2] << 16);

            _tareValue = _netValue - _grossValue;
            _generalWeightError = Convert.ToBoolean((_data[4] & 0x1));
            _scaleAlarmTriggered = Convert.ToBoolean(((_data[4] & 0x2) >> 1));
            _limitStatus = ((_data[4] & 0xC) >> 2);
            _weightMoving = Convert.ToBoolean(((_data[4] & 0x10) >> 4));

            _scaleSealIsOpen = Convert.ToBoolean(((_data[4] & 0x20) >> 5));
            _manualTare = Convert.ToBoolean(((_data[4] & 0x40) >> 6));
            _weightType = Convert.ToBoolean(((_data[4] & 0x80) >> 7));
            _scaleRange = ((_data[4] & 0x300) >> 8);

            _zeroRequired = Convert.ToBoolean((_data[4] & 0x400) >> 10);
            _weightWithinTheCenterOfZero = Convert.ToBoolean(((_data[4] & 0x800) >> 11));
            _weightInZeroRange = Convert.ToBoolean(((_data[4] & 0x1000) >> 12));
            _applicationMode = (_data[5] & 0x3 >> 1);
            _applicationModeStr = "";

            _decimals = ((_data[5] & 0x70) >> 4);
            _unit = ((_data[5] & 0x180) >> 7);
            _handshake = Convert.ToBoolean(((_data[5] & 0x4000) >> 14));
            _status = Convert.ToBoolean(((_data[5] & 0x8000) >> 15));

            _underload = false;
            _overload = false;
            _weightWithinLimits = false;
            _higherSafeLoadLimit = false;
            _legalTradeOp = 0;

            _input1 = (_data[6] & 0x1);
            _input2 = ((_data[6] & 0x2) >> 1);
            _input3 = ((_data[6] & 0x4) >> 2);
            _input4 = ((_data[6] & 0x8) >> 3);

            _output1 = (_data[7] & 0x1); ;
            _output2 = ((_data[7] & 0x2) >> 1);
            _output3 = ((_data[7] & 0x4) >> 2);
            _output4 = ((_data[7] & 0x8) >> 3);

            _limitValue1 = (_data[8] & 0x1); ;
            _limitValue2 = ((_data[8] & 0x2) >> 1);
            _limitValue3 = ((_data[8] & 0x4) >> 2);
            _limitValue4 = ((_data[8] & 0x8) >> 3);

            _weightMemDay = (_data[9]);
            _weightMemMonth = (_data[10]);
            _weightMemYear = (_data[11]);
            _weightMemSeqNumber = (_data[12]);
            _weightMemGross = (_data[13]);
            _weightMemNet = (_data[14]);

        }

        #endregion

        #region properties for standard mode

        public int NetValue
        {
            get { return _netValue; }
            set { this._netValue = value; }

        }
        public int GrossValue
        {
            get{ return _grossValue; }
            set{ this._grossValue = value; }
        }
        public bool GeneralWeightError
        {
            get{ return _generalWeightError; }
            set{ this._generalWeightError = value; }
        }
        public bool ScaleAlarmTriggered
        {
            get{ return _scaleAlarmTriggered; }
            set{ this._scaleAlarmTriggered = value; }
        }
        public int LimitStatus
        {
            get { return _limitStatus; }
            set { this._limitStatus = value; }
        }
        public bool WeightMoving
        {
            get { return _weightMoving; }
            set { this._weightMoving = value; }
        }
        public bool ScaleSealIsOpen
        {
            get { return _scaleSealIsOpen; }
            set { this._scaleSealIsOpen = value; }
        }
        public bool ManualTare
        {
            get { return _manualTare; }
            set { this._manualTare = value; }
        }
        public bool WeightType
        {
            get { return _weightType; }
            set { this._weightType = value; }
        }
        public int ScaleRange
        {
            get{ return _scaleRange; }
            set{ this._scaleRange = value; }
        }
        public bool ZeroRequired
        {
            get{ return _zeroRequired; }
            set{ this._zeroRequired = value; }
        }
        public bool WeightWithinTheCenterOfZero
        {
            get{ return _weightWithinTheCenterOfZero; }
            set{ this._weightWithinTheCenterOfZero = value; }
        }
        public bool WeightInZeroRange
        {
            get{ return _weightInZeroRange; }
            set{ this._weightInZeroRange = value; }
        }
        public int ApplicationMode
        {
            get{ return _applicationMode; }
            set{ this._applicationMode = value; }
        }
        public int Decimals
        {
            get{ return _decimals; }
            set{ this._decimals = value; }
        }
        public int Unit
        {
            get{ return _unit; }
            set{ this._unit = value; }
        }
        public bool Handshake
        {
            get{ return _handshake; }
            set{ this._handshake = value; }
        }
        public bool Status
        {
            get{ return _status; }
            set{ this._status = value; }
        }

        public int Input1
        {
            get{ return _input1; }
            set{ this._input1 = value; }
        }
        public int Input2
        {
            get{ return _input2; }
            set{ this._input2 = value; }
        }
        public int Input3
        {
            get{ return _input3; }
            set{ this._input3 = value; }
        }
        public int Input4
        {
            get{ return _input4; }
            set{ this._input4 = value; }
        }
        public int Output1
        {
            get{ return _output1; }
            set{ this._output1 = value; }
        }
        public int Output2
        {
            get{ return _output2; }
            set{ this._output2 = value; }
        }
        public int Output3
        {
            get{ return _output3; }
            set{ this._output3 = value; }
        }
        public int Output4
        {
            get{ return _output4; }
            set{ this._output4 = value; }
        }
        public int LimitStatus1
        {
            get{ return _limitValue1; }
            set{ this._limitValue1 = value; }
        }
        public int LimitStatus2
        {
            get{ return _limitValue2; }
            set{ this._limitValue2 = value; }
        }
        public int LimitStatus3
        {
            get{ return _limitValue3; }
            set{ this._limitValue3 = value;}
        }
        public int LimitStatus4
        {
            get{ return _limitValue4; }
            set{ this._limitValue4 = value; }
        }
        public int WeightMemDay
        {
            get{ return _weightMemDay; }
            set{ this._weightMemDay = value; }
        }
        public int WeightMemMonth
        {
            get{ return _weightMemMonth; }
            set{ this._weightMemMonth = value; }
        }
        public int WeightMemYear
        {
            get{ return _weightMemYear;}
            set{ this._weightMemYear = value; }
        }
        public int WeightMemSeqNumber
        {
            get{ return _weightMemSeqNumber; }
            set{ this._weightMemSeqNumber = value; }
        }
        public int WeightMemGross
        {
            get{ return _weightMemGross; }
            set{ this._weightMemGross = value; }
        }
        public int WeightMemNet
        {
            get{ return WeightMemNet; }
            set{ this._weightMemNet = value; }
        }

        public string NetValueStr     // data type = double according to OPC-UA standard
        {
            get { return _netValueStr; }
            set { this._netValueStr = value;}
        }

        public string GrossValueStr   // data type = double according to OPC-UA standard
        {
            get { return _grossValueStr; }
            set { this._grossValueStr = value; }
        }

        public int TareValue         // data type = double according to OPC-UA standard
        {
            get { return _tareValue; }
            set { this._tareValue = value; }
        }
      
        public bool Underload
        {
            get { return _underload; }
            set { this._underload = value; }
        }

        public bool Overload
        {
            get { return _overload; }
            set { this._overload = value; }
        }


        public bool WeightWithinLimits
        {
            get { return _weightWithinLimits; }
            set { this._weightWithinLimits = value; }
        }

        public bool HigherSafeLoadLimit
        {
            get { return _higherSafeLoadLimit; }
            set { this._higherSafeLoadLimit = value; }
        }

        public int LegalTradeOp
        {
            get { return _legalTradeOp; }
            set { this._legalTradeOp = value; }
        }

        public int LimitValue1
        {
            get { return _limitValue1; }
            set { this._limitValue1 = value; }
        }
        public int LimitValue2
        {
            get { return _limitValue2; }
            set { this._limitValue2 = value; }
        }
        public int LimitValue3
        {
            get { return _limitValue3; }
            set { this._limitValue3 = value; }
        }
        public int LimitValue4
        {
            get { return _limitValue4; }
            set { this._limitValue4 = value; }
        }

        public string ApplicationModeStr
        {
            get{ return this._applicationModeStr;}
            set { this._applicationModeStr = value; }
        }

        #endregion
    }
}
