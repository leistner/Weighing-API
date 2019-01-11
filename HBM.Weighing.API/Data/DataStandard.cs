﻿using System;
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

        // Input words :

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

        // Output words : 

        private int _manualTareValue;
        private int _limitValue1Input;
        private int _limitValue1Mode;

        private int _limitValue1ActivationLevelLowerBandLimit;
        private int _limitValue1HysteresisBandHeight;
        private int _limitValue2Source;
        private int _limitValue2Mode;

        private int _limitValue2ActivationLevelLowerBandLimit;
        private int _limitValue2HysteresisBandHeight;
        private int _limitValue3Source;
        private int _limitValue3Mode;

        private int _limitValue3ActivationLevelLowerBandLimit;
        private int _limitValue3HysteresisBandHeight;
        private int _limitValue4Source;

        private int _limitValue4Mode;
        private int _limitValue4ActivationLevelLowerBandLimit;
        private int _limitValue4HysteresisBandHeight;

        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nomnialLoad; 

        #endregion

        #region constructor

        public DataStandard()
        {
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

            _manualTareValue=0;
            _limitValue1Input=0;
            _limitValue1Mode=0;

            _limitValue1ActivationLevelLowerBandLimit=0;
            _limitValue1HysteresisBandHeight=0;
            _limitValue2Source=0;
            _limitValue2Mode=0;

            _limitValue2ActivationLevelLowerBandLimit=0;
            _limitValue2HysteresisBandHeight=0;
            _limitValue3Source=0;
            _limitValue3Mode=0;

            _limitValue3ActivationLevelLowerBandLimit=0;
            _limitValue3HysteresisBandHeight=0;
            _limitValue4Source=0;

            _limitValue4Mode=0;
            _limitValue4ActivationLevelLowerBandLimit=0;
            _limitValue4HysteresisBandHeight=0;

            _calibrationWeight=0;
            _zeroLoad=0;
            _nomnialLoad=0;

    }

    #endregion

        #region Update methods for standard mode

    public void UpdateStandardDataModbus(ushort[] _dataParam)
        {
            this._data = _dataParam;

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

        public void UpdateStandardDataJet(Dictionary<string, int> _dataParam)
        {

        }

        #endregion

        #region Get-properties for standard mode

        public int Input1
        {
            get{ return _input1; }
        }
        public int Input2
        {
            get{ return _input2; }
        }
        public int Input3
        {
            get{ return _input3; }
        }
        public int Input4
        {
            get{ return _input4; }
        }
        public int Output1
        {
            get{ return _output1; }
        }
        public int Output2
        {
            get{ return _output2; }
        }
        public int Output3
        {
            get{ return _output3; }
        }
        public int Output4
        {
            get{ return _output4; }
        }
        public int LimitStatus1
        {
            get{ return _limitValue1; }
        }
        public int LimitStatus2
        {
            get{ return _limitValue2; }
        }
        public int LimitStatus3
        {
            get{ return _limitValue3; }
        }
        public int LimitStatus4
        {
            get{ return _limitValue4; }
        }
        public int WeightMemDay
        {
            get{ return _weightMemDay; }
            set { this._weightMemDay = value; }
        }
        public int WeightMemMonth
        {
            get{ return _weightMemMonth; }
            set { this._weightMemMonth = value; }
        }
        public int WeightMemYear
        {
            get{ return _weightMemYear;}
            set { this._weightMemYear = value; }
        }
        public int WeightMemSeqNumber
        {
            get{ return _weightMemSeqNumber; }
            set { this._weightMemSeqNumber = value; }
        }
        public int WeightMemGross
        {
            get{ return _weightMemGross; }
            set { this._weightMemGross = value; }
        }
        public int WeightMemNet
        {
            get{ return _weightMemNet; }
            set { this._weightMemNet = value; }
        }

        public int LimitValue1
        {
            get { return _limitValue1; }
        }
        public int LimitValue2
        {
            get { return _limitValue2; }
        }
        public int LimitValue3
        {
            get { return _limitValue3; }
        }
        public int LimitValue4
        {
            get { return _limitValue4; }
        }
        #endregion

        #region Get-/Set-properties for standard mode 

        public int ManualTareValue
        {
            get { return _manualTareValue; }
            set { _manualTareValue = value; }
        }
        public int LimitValue1Input
        {
            get { return _limitValue1Input; }
            set { _limitValue1Input = value; }
        }
        public int LimitValue1Mode
        {
            get { return _limitValue1Mode; }
            set { _limitValue1Mode = value; }
        }
        public int LimitValue1ActivationLevelLowerBandLimit
        {
            get { return _limitValue1ActivationLevelLowerBandLimit; }
            set { _limitValue1ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue1HysteresisBandHeight
        {
            get { return _limitValue1HysteresisBandHeight; }
            set { _limitValue1HysteresisBandHeight = value; }
        }
        public int LimitValue2Source
        {
            get { return _limitValue2Source; }
            set { _limitValue2Source = value; }
        }
        public int LimitValue2Mode
        {
            get { return _limitValue2Mode; }
            set { _limitValue2Mode = value; }
        }
        public int LimitValue2ActivationLevelLowerBandLimit
        {
            get { return _limitValue2ActivationLevelLowerBandLimit; }
            set { _limitValue2ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue2HysteresisBandHeight
        {
            get { return _limitValue2HysteresisBandHeight; }
            set { _limitValue2HysteresisBandHeight = value; }
        }
        public int LimitValue3Source
        {
            get { return _limitValue3Source; }
            set { _limitValue3Source = value; }
        }
        public int LimitValue3Mode
        {
            get { return _limitValue3Mode; }
            set { _limitValue3Mode = value; }
        }
        public int LimitValue3ActivationLevelLowerBandLimit
        {
            get { return _limitValue3ActivationLevelLowerBandLimit; }
            set { _limitValue3ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue3HysteresisBandHeight
        {
            get { return _limitValue3HysteresisBandHeight; }
            set { _limitValue3HysteresisBandHeight = value; }
        }
        public int LimitValue4Source
        {
            get { return _limitValue4Source; }
            set { _limitValue4Source = value; }
        }
        public int LimitValue4Mode
        {
            get { return _limitValue4Mode; }
            set { _limitValue4Mode = value; }
        }
        public int LimitValue4ActivationLevelLowerBandLimit
        {
            get { return _limitValue4ActivationLevelLowerBandLimit; }
            set { _limitValue4ActivationLevelLowerBandLimit = value; }
        }
        public int LimitValue4HysteresisBandHeight
        {
            get { return _limitValue4HysteresisBandHeight; }
            set { _limitValue4HysteresisBandHeight = value; }
        }
        public int CalibrationWeight
        {
            get { return _calibrationWeight; }
            set { _calibrationWeight = value; }
        }
        public int ZeroLoad
        {
            get { return _zeroLoad; }
            set { _zeroLoad = value; }
        }
        public int NominalLoad
        {
            get { return _nomnialLoad; }
            set { _nomnialLoad = value; }
        }
        #endregion

    }
}
