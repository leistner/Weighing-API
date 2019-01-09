// <copyright file="IDeviceData.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// HBM.Weighing.API, a library to communicate with HBM weighing technology devices  
//
// The MIT License (MIT)
//
// Copyright (C) Hottinger Baldwin Messtechnik GmbH
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS
// BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN
// ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
// CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
//
// </copyright>

using System;

namespace HBM.Weighing.API
{
    public class ProcessData : IProcessData
    {
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

        public ProcessData()
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
             
        }

    public int NetValue     // data type = double according to OPC-UA standard
        {
            get { return _netValue; }
            set { this._netValue = value; }
        }

        public int GrossValue   // data type = double according to OPC-UA standard
        {
            get { return _grossValue; }
            set { this._grossValue = value; }
        }

        public string NetValueStr     // data type = double according to OPC-UA standard
        {
            get { return _netValueStr; }
            set { this._netValueStr = value; }
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

        public bool GeneralWeightError
        {
            get { return _generalWeightError; }
            set { this._generalWeightError = value; }
        }

        public bool ScaleAlarmTriggered
        {
            get { return _scaleAlarmTriggered; }
            set { this._scaleAlarmTriggered = value; }
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
            get { return _scaleRange; }
            set { this._scaleRange = value; }
        }

        public bool ZeroRequired
        {
            get { return _zeroRequired; }
            set { this._zeroRequired = value; }
        }

        public bool WeightWithinTheCenterOfZero
        {
            get { return _weightWithinTheCenterOfZero; }
            set { this._weightWithinTheCenterOfZero = value; }
        }

        public bool WeightInZeroRange
        {
            get { return _weightInZeroRange; }
            set { this._weightInZeroRange = value; }
        }

        public int ApplicationMode
        {
            get { return _applicationMode; }
            set { this._applicationMode = value; }
        }

        public string ApplicationModeStr
        {
            get { return _applicationModeStr; }
            set { this._applicationModeStr = value; }
        }

        public int Decimals
        {
            get { return _decimals; }
            set { this._decimals = value; }
        }

        public int Unit
        {
            get { return _unit; }
            set { this._unit = value; }
        }

        public bool Handshake
        {
            get { return _handshake; }
            set { this._handshake = value; }
        }

        public bool Status
        {
            get { return _status; }
            set { this._status = value; }
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

        public bool weightWithinLimits
        {
            get { return _weightWithinLimits; }
            set { this._weightWithinLimits = value; }
        }

        public bool higherSafeLoadLimit
        {
            get { return _higherSafeLoadLimit; }
            set { this._higherSafeLoadLimit = value; }
        }

        public int LegalTradeOp
        {
            get { return _legalTradeOp; }
            set { this._legalTradeOp = value; }
        }
    }
}
