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
        private int _NetValue;      // data type = double according to OPC-UA standard
        private int _GrossValue;    // data type = double according to OPC-UA standard
        private int _Tare;          // data type = double according to OPC-UA standard
        private bool _GeneralWeightError;
        private bool _ScaleAlarmTriggered;
        private int _LimitStatus;
        private bool _WeightMoving;
        private bool _ScaleSealIsOpen;
        private bool _ManualTare;
        private bool _WeightType;
        private int _ScaleRange;
        private bool _ZeroRequired;
        private bool _WeightWithinTheCenterOfZero;
        private bool _WeightInZeroRange;
        private int _ApplicationMode;
        private int _Decimals;
        private int _Unit;
        private bool _Handshake;
        private bool _Status;
        private bool _Underload;
        private bool _Overload;
        private bool _weightWithinLimits;
        private bool _higherSafeLoadLimit;
        private int _LegalTradeOp;

        public ProcessData()
        {
             _NetValue = 0;     
             _GrossValue = 0;    
             _Tare = 0;       
             _GeneralWeightError = false;
             _ScaleAlarmTriggered = false;
             _LimitStatus = 0;
             _WeightMoving = false;
             _ScaleSealIsOpen = false;
             _ManualTare = false;
             _WeightType = false;
             _ScaleRange = 0;
             _ZeroRequired = false;
             _WeightWithinTheCenterOfZero = false;
             _WeightInZeroRange = false;
             _ApplicationMode = 0;
             _Decimals = 0;
             _Unit = 0;
             _Handshake = false;
             _Status = false;
             _Underload = false;
             _Overload = false;
             _weightWithinLimits = false;
             _higherSafeLoadLimit = false;
             _LegalTradeOp = 0;
        }

    public int NetValue     // data type = double according to OPC-UA standard
        {
            get { return _NetValue; }
            set { this._NetValue = value; }
        }

        public int GrossValue   // data type = double according to OPC-UA standard
        {
            get { return _GrossValue; }
            set { this._GrossValue = value; }
        }

        public int Tare         // data type = double according to OPC-UA standard
        {
            get { return _Tare; }
            set { this._Tare = value; }
        }

        public bool GeneralWeightError
        {
            get { return _GeneralWeightError; }
            set { this._GeneralWeightError = value; }
        }

        public bool ScaleAlarmTriggered
        {
            get { return _ScaleAlarmTriggered; }
            set { this._ScaleAlarmTriggered = value; }
        }

        public int LimitStatus
        {
            get { return _LimitStatus; }
            set { this._LimitStatus = value; }
        }

        public bool WeightMoving
        {
            get { return _WeightMoving; }
            set { this._WeightMoving = value; }
        }

        public bool ScaleSealIsOpen
        {
            get { return _ScaleSealIsOpen; }
            set { this._ScaleSealIsOpen = value; }
        }

        public bool ManualTare
        {
            get { return _ManualTare; }
            set { this._ManualTare = value; }
        }

        public bool WeightType
        {
            get { return _WeightType; }
            set { this._WeightType = value; }
        }

        public int ScaleRange
        {
            get { return _ScaleRange; }
            set { this._ScaleRange = value; }
        }

        public bool ZeroRequired
        {
            get { return _ZeroRequired; }
            set { this._ZeroRequired = value; }
        }

        public bool WeightWithinTheCenterOfZero
        {
            get { return _WeightWithinTheCenterOfZero; }
            set { this._WeightWithinTheCenterOfZero = value; }
        }

        public bool WeightInZeroRange
        {
            get { return _WeightInZeroRange; }
            set { this._WeightInZeroRange = value; }
        }

        public int ApplicationMode
        {
            get { return _ApplicationMode; }
            set { this._ApplicationMode = value; }
        }

        public int Decimals
        {
            get { return _Decimals; }
            set { this._Decimals = value; }
        }

        public int Unit
        {
            get { return _Unit; }
            set { this._Unit = value; }
        }

        public bool Handshake
        {
            get { return _Handshake; }
            set { this._Handshake = value; }
        }

        public bool Status
        {
            get { return _Status; }
            set { this._Status = value; }
        }

        public bool Underload
        {
            get { return _Underload; }
            set { this._Underload = value; }
        }

        public bool Overload
        {
            get { return _Overload; }
            set { this._Overload = value; }
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
            get { return _LegalTradeOp; }
            set { this._LegalTradeOp = value; }
        }
    }
}
