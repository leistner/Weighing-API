// <copyright file="ProcessData.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using HBM.Weighing.API.WTX.Jet;
using System;
using System.Collections.Generic;

namespace HBM.Weighing.API
{
    /// <summary>
    /// Implementation of the interface IProcessData for the process data
    /// </summary>
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
        //private int _applicationMode;
        //private string _applicationModeStr;
        private int _decimals;
        private int _unit;
        private bool _handshake;
        private int _status;
        private bool _underload;
        private bool _overload;
        private bool _weightWithinLimits;
        private bool _higherSafeLoadLimit;

#region constructor of ProcessData

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
             //_applicationMode = 0;
             //_applicationModeStr = "";
             _decimals = 0;
             _unit = 0;
             _handshake = false;
             _status = 0;
             _underload = false;
             _overload = false;
             _weightWithinLimits = false;
             _higherSafeLoadLimit = false;
        }

#endregion

        #region update methods for process data

        public void UpdateProcessDataModbus(ushort[] _data)
        {
            _netValue = _data[1] + (_data[0] << 16);
            _grossValue = _data[3] + (_data[2] << 16);
            _netValueStr = this.CurrentWeight(_netValue, _decimals);
            _grossValueStr = this.CurrentWeight(_grossValue, _decimals);

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

            _decimals = ((_data[5] & 0x70) >> 4);
            _unit = ((_data[5] & 0x180) >> 7);
            _handshake = Convert.ToBoolean(((_data[5] & 0x4000) >> 14));
            _status = ((_data[5] & 0x8000) >> 15);

            this.limitStatusBool();  // update the booleans 'Underload', 'Overload', 'weightWithinLimits', 'higherSafeLoadLimit'. 
        }

        public void UpdateProcessDataJet(Dictionary <string,int> _data)
        {
            _netValue = _data[JetBusCommands.NET_VALUE];
            _grossValue = _data[JetBusCommands.GROSS_VALUE];
            _netValueStr = this.CurrentWeight(_netValue, _decimals);
            _grossValueStr = this.CurrentWeight(_grossValue, _decimals);

            _tareValue = _netValue - _grossValue;
            _generalWeightError = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1));
            _scaleAlarmTriggered = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x2) >> 1);
            _limitStatus = (_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0xC) >> 2;

            
            _weightMoving = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x10) >> 4);
            _scaleSealIsOpen = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x20) >> 5);
            _manualTare = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x40) >> 6);
            _weightType = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x80) >> 7);
            _scaleRange = (_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x300) >> 8;
            
            _zeroRequired = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x400) >> 10);
            _weightWithinTheCenterOfZero = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x800) >> 11);
            _weightInZeroRange = Convert.ToBoolean((_data[JetBusCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] & 0x1000) >> 12);

            _decimals = _data[JetBusCommands.DECIMALS];
            _unit = (_data[JetBusCommands.UNIT_PREFIX_FIXED_PARAMETER] & 0xFF0000) >> 16;

            _handshake = UpdateHandshake(_data[JetBusCommands.SCALE_COMMAND_STATUS]);
            _status = _data[JetBusCommands.SCALE_COMMAND_STATUS];

            this.limitStatusBool();  // update the booleans 'Underload', 'Overload', 'weightWithinLimits', 'higherSafeLoadLimit'. 
        }

        public bool UpdateHandshake(int _handshakeValue)
        {
            if (_handshakeValue == 1801543519)
                return true;
            else
                return false;
        }

        // In the following methods the different options for the single integer values are used to define and
        // interpret the value. Finally a string should be returned from the methods to write it onto the GUI Form. 
        public string CurrentWeight(int value, int decimals)
        {
            double dvalue = value / Math.Pow(10, decimals);
            string returnvalue = "";

            switch (_decimals)
            {
                case 0: returnvalue = dvalue.ToString(); break;
                case 1: returnvalue = dvalue.ToString("0.0"); break;
                case 2: returnvalue = dvalue.ToString("0.00"); break;
                case 3: returnvalue = dvalue.ToString("0.000"); break;
                case 4: returnvalue = dvalue.ToString("0.0000"); break;
                case 5: returnvalue = dvalue.ToString("0.00000"); break;
                case 6: returnvalue = dvalue.ToString("0.000000"); break;
                default: returnvalue = dvalue.ToString(); break;

            }
            return returnvalue;
        }

        private void limitStatusBool()
        {
            switch (_limitStatus)
            {
                case 0: // Weight within limits
                    _underload = false;
                    _overload = false;
                    _weightWithinLimits = true;
                    _higherSafeLoadLimit = false;
                    break;
                case 1: // Lower than minimum
                    _underload = true;
                    _overload = false;
                    _weightWithinLimits = false;
                    _higherSafeLoadLimit = false;
                    break;
                case 2: // Higher than maximum capacity
                    _underload = false;
                    _overload = true;
                    _weightWithinLimits = false;
                    _higherSafeLoadLimit = false;
                    break;
                case 3: // Higher than safe load limit
                    _underload = false;
                    _overload = false;
                    _weightWithinLimits = false;
                    _higherSafeLoadLimit = true;
                    break;
                default: // Lower than minimum
                    _underload = true;
                    _overload = false;
                    _weightWithinLimits = false;
                    _higherSafeLoadLimit = false;
                    break;
            }
        }

        public string WeightMovingStringComment()
        {
            if (_weightMoving == false)
                return "0=Weight is not moving.";
            else
                return "1=Weight is moving";
        }


        #endregion

        #region Get-properties of process data

        public int NetValue     // data type = double according to OPC-UA standard
        {
            get { return _netValue; }
        }

        public int GrossValue   // data type = double according to OPC-UA standard
        {
            get { return _grossValue; }
        }

        public string NetValueStr     // data type = double according to OPC-UA standard
        {
            get { return _netValueStr; }
        }

        public string GrossValueStr   // data type = double according to OPC-UA standard
        {
            get { return _grossValueStr; }
        }

        public int TareValue         // data type = double according to OPC-UA standard
        {
            get { return _tareValue; }
        }

        public bool GeneralWeightError
        {
            get { return _generalWeightError; }
        }

        public bool ScaleAlarmTriggered
        {
            get { return _scaleAlarmTriggered; }
        }

        public int LimitStatus
        {
            get { return _limitStatus; }
        }

        public bool WeightMoving
        {
            get { return _weightMoving; }
        }

        public bool ScaleSealIsOpen
        {
            get { return _scaleSealIsOpen; }
        }

        public bool ManualTare
        {
            get { return _manualTare; }
        }

        public bool WeightType
        {
            get { return _weightType; }
        }

        public int ScaleRange
        {
            get { return _scaleRange; }
        }

        public bool ZeroRequired
        {
            get { return _zeroRequired; }
        }

        public bool WeightWithinTheCenterOfZero
        {
            get { return _weightWithinTheCenterOfZero; }
        }

        public bool WeightInZeroRange
        {
            get { return _weightInZeroRange; }
        }

        public int Decimals
        {
            get { return _decimals; }
        }

        public int Unit
        {
            get { return _unit; }
        }

        public bool Handshake
        {
            get { return _handshake; }
        }

        public int Status
        {
            get { return _status; }
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

        #endregion
    }
}
