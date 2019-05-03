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
    /// The class ProcessData contains the input word concerning real-time data 
    /// of WTX device 120 and 110.
    /// </summary>
    public class ProcessDataJet : IProcessData
    {
        private int _netValue;      // data type = double according to OPC-UA standard
        private int _grossValue;    // data type = double according to OPC-UA standard
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
        private int _decimals;
        private int _unit;
        private bool _handshake;
        private int _status;
        private bool _underload;
        private bool _overload;
        private bool _weightWithinLimits;
        private bool _higherSafeLoadLimit;

        private INetConnection _connection;

        #region constructor of ProcessDataJet

        public ProcessDataJet(INetConnection Connection)
        {
            _connection = Connection;

            _connection.UpdateDataClasses += UpdateProcessData;

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

        public void UpdateProcessData(object sender, DataEventArgs e)
        {
            _netValue   = Convert.ToInt32(e.DataDictionary[_connection.IDCommands.NET_VALUE]);
            _grossValue = Convert.ToInt32(e.DataDictionary[_connection.IDCommands.GROSS_VALUE]);

            _tareValue = _netValue - _grossValue;

            _generalWeightError  = Convert.ToBoolean(Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x1);
            _scaleAlarmTriggered = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x2) >> 1);
            _limitStatus         = (Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0xC) >> 2;

            _weightMoving    = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x10) >> 4);
            _scaleSealIsOpen = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x20) >> 5);

            _manualTare = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x40) >> 6);
            _weightType = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x80) >> 7);
            _scaleRange = (Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x300) >> 8;

            _applicationMode = e.DataDictionary[_connection.IDCommands.APPLICATION_MODE];
            _zeroRequired    = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x400) >> 10);
            _weightWithinTheCenterOfZero = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x800) >> 11);
            _weightInZeroRange = Convert.ToBoolean((Convert.ToInt32(e.DataDictionary[_connection.IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS]) & 0x1000) >> 12);

            _decimals = Convert.ToInt32(e.DataDictionary[_connection.IDCommands.DECIMALS]);
            _status = Convert.ToInt32(e.DataDictionary[_connection.IDCommands.SCALE_COMMAND_STATUS]);
             _unit = (Convert.ToInt32(e.DataDictionary[_connection.IDCommands.UNIT_PREFIX_FIXED_PARAMETER]) & 0xFF0000) >> 16;

            this.limitStatusBool();  // update the booleans 'Underload', 'Overload', 'weightWithinLimits', 'higherSafeLoadLimit'.             
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

        #endregion
    }
}
