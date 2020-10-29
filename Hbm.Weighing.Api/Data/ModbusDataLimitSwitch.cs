// <copyright file="ModbusDataLimitSwitch.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.Api, a library to communicate with HBM weighing technology devices  
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

using Hbm.Weighing.Api.Utils;
using Hbm.Weighing.Api.WTX.Modbus;
using System;
using System.Collections.Generic;

namespace Hbm.Weighing.Api.Data
{
    /// <summary>
    /// Implementation of the interface IDataStandard for the standard mode.
    /// The class DataStandardModbus contains the data input word and data output words for the standard mode
    /// of WTX device 120 and 110 via Modbus.
    /// </summary>
    public class ModbusDataLimitSwitch : IDataLimitSwitch
    {

        #region ==================== constants & fields ====================
        private LimitSwitchSource _limitSwitch1Source;
        private LimitSwitchMode _limitSwitch1Mode;
        private int _limitSwitch1LevelAndLowerBandValue;
        private int _limitSwitch1HysteresisAndBandHeight;
        private LimitSwitchSource _limitSwitch2Source;
        private LimitSwitchMode _limitSwitch2Mode;
        private int _limitSwitch2LevelAndLowerBandValue;
        private int _limitSwitch2HysteresisAndBandHeight;
        private LimitSwitchSource _limitSwitch3Source;
        private LimitSwitchMode _limitSwitch3Mode;
        private int _limitSwitch3LevelAndLowerBandValue;
        private int _limitSwitch3HysteresisAndBandHeight;
        private LimitSwitchSource _limitSwitch4Source;
        private LimitSwitchMode _limitSwitch4Mode;
        private int _limitSwitch4LevelAndLowerBandValue;
        private int _limitSwitch4HysteresisAndBandHeight;               
        private INetConnection _connection;
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Constructor of class DataStandardModbus : Initalizes values and connects 
        /// the eventhandler from Connection to the interal update method
        /// </summary>
        public ModbusDataLimitSwitch(INetConnection Connection)
        {
            _connection = Connection;
            _connection.UpdateData += UpdateDataLimitSwitch;
            LimitStatus1 = false;
            LimitStatus2 = false;
            LimitStatus3 = false;
            LimitStatus4 = false;
            _limitSwitch1Source = LimitSwitchSource.Gross;
            _limitSwitch1Mode = LimitSwitchMode.AboveLevel;
            _limitSwitch1LevelAndLowerBandValue = 0;
            _limitSwitch1HysteresisAndBandHeight = 0;
            _limitSwitch2Source = LimitSwitchSource.Gross;
            _limitSwitch2Mode = LimitSwitchMode.AboveLevel;
            _limitSwitch2LevelAndLowerBandValue = 0;
            _limitSwitch2HysteresisAndBandHeight = 0;
            _limitSwitch3Source = LimitSwitchSource.Gross;
            _limitSwitch3Mode = LimitSwitchMode.AboveLevel;
            _limitSwitch3LevelAndLowerBandValue = 0;
            _limitSwitch3HysteresisAndBandHeight = 0;
            _limitSwitch4Source = LimitSwitchSource.Gross;
            _limitSwitch4Mode = LimitSwitchMode.AboveLevel;
            _limitSwitch4LevelAndLowerBandValue = 0;
            _limitSwitch4HysteresisAndBandHeight = 0;            
        }
        #endregion

        #region ==================== events & delegates ====================

        /// <summary>
        /// Updates and converts the values from buffer
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateDataLimitSwitch(object sender, EventArgs e)
        {
            try
            {
                LimitStatus1 = MeasurementUtils.StringToBool(_connection.ReadFromBuffer(ModbusCommands.LVSLimitValueStatus));
                LimitStatus2 = MeasurementUtils.StringToBool(_connection.ReadFromBuffer(ModbusCommands.LVSLimitValueStatus));
                LimitStatus3 = MeasurementUtils.StringToBool(_connection.ReadFromBuffer(ModbusCommands.LVSLimitValueStatus));
                LimitStatus4 = MeasurementUtils.StringToBool(_connection.ReadFromBuffer(ModbusCommands.LVSLimitValueStatus));
  
                ApplicationMode _applicationMode = (ApplicationMode)Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.IMDApplicationMode));
                if (_applicationMode == ApplicationMode.Standard)
                {
                    _limitSwitch1Mode = StringToLimitSwitchMode(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchMode));
                    _limitSwitch1Source = StringToLimitSwitchSource(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchSource));
                    _limitSwitch1LevelAndLowerBandValue = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchLevel));
                    _limitSwitch1HysteresisAndBandHeight = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchHysteresis));
                    _limitSwitch2Mode = StringToLimitSwitchMode(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchMode));
                    _limitSwitch2Source = StringToLimitSwitchSource(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchSource));
                    _limitSwitch2LevelAndLowerBandValue = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchLevel));
                    _limitSwitch2HysteresisAndBandHeight = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchHysteresis));
                    _limitSwitch3Mode = StringToLimitSwitchMode(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchMode));
                    _limitSwitch3Source = StringToLimitSwitchSource(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchSource));
                    _limitSwitch3LevelAndLowerBandValue = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchLevel));
                    _limitSwitch3HysteresisAndBandHeight = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchHysteresis));
                    _limitSwitch4Mode = StringToLimitSwitchMode(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchMode));
                    _limitSwitch4Source = StringToLimitSwitchSource(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchSource));
                    _limitSwitch4LevelAndLowerBandValue = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchLevel));
                    _limitSwitch4HysteresisAndBandHeight = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.LIV1LimitSwitchHysteresis));
                }
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException in class DataStandardModbus, update method");
            }
        }

        #endregion

        #region ======================== properties ========================

        ///<inheritdoc/>
        public bool LimitStatus1 { get; private set; }

        ///<inheritdoc/>
        public bool LimitStatus2 { get; private set; }

        ///<inheritdoc/>
        public bool LimitStatus3 { get; private set; }

        ///<inheritdoc/>
        public bool LimitStatus4 { get; private set; }

        ///<inheritdoc/>
        public LimitSwitchSource LimitSwitch1Source
        { 
            get
            {
                return _limitSwitch1Source;
            }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV1LimitSwitchSource, LimitSwitchSouceToInt(value));
                _limitSwitch1Source = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchMode LimitSwitch1Mode
        {
            get { return _limitSwitch1Mode; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV1LimitSwitchMode, LimitSwitchModeToInt(value));
                _limitSwitch1Mode = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch1Level
        {
            get { return _limitSwitch1LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV1LimitSwitchLevel, value);
                _limitSwitch1LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch1LowerBandValue
        {
            get { return _limitSwitch1LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV1LimitSwitchHysteresis, value);
                _limitSwitch1HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch1Hysteresis
        {
            get { return _limitSwitch1HysteresisAndBandHeight; }
            set
            {
                this._connection.WriteInteger(ModbusCommands.LIV1LimitSwitchHysteresis, value);
                _limitSwitch1LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch1BandHeight
        {
            get { return _limitSwitch1HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV1LimitSwitchHysteresis, value);
                _limitSwitch1HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchSource LimitSwitch2Source
        {
            get { return _limitSwitch2Source; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV2LimitSwitchSource, LimitSwitchSouceToInt(value));
                _limitSwitch2Source = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchMode LimitSwitch2Mode 
        {
            get { return _limitSwitch2Mode; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV2LimitSwitchMode, LimitSwitchModeToInt(value));
                _limitSwitch2Mode = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch2Level 
        {
            get { return _limitSwitch2LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV2LimitSwitchLevel, value);
                _limitSwitch2LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch2Hysteresis
        {
            get { return _limitSwitch2HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV2LimitSwitchHysteresis, value);
                _limitSwitch2HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch2LowerBandValue 
        {
            get { return _limitSwitch2LevelAndLowerBandValue; }
            set
            {
                this._connection.WriteInteger(ModbusCommands.LIV2LimitSwitchLevel, value);
                _limitSwitch2LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch2BandHeight
        { 
            get { return _limitSwitch2HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV2LimitSwitchHysteresis, value);
                _limitSwitch2HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchSource LimitSwitch3Source
        {
            get { return _limitSwitch3Source; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchSource, LimitSwitchSouceToInt(value));
                _limitSwitch3Source = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchMode LimitSwitch3Mode
        {
            get { return _limitSwitch3Mode; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchMode, LimitSwitchModeToInt(value));
                _limitSwitch3Mode = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch3Level
        {
            get { return _limitSwitch3LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchLevel, value);
                _limitSwitch3LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch3Hysteresis
        {
            get { return _limitSwitch3HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchHysteresis, value);
                _limitSwitch3HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch3LowerBandValue
        {
            get { return _limitSwitch3LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchLevel, value);
                _limitSwitch3LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch3BandHeight
        {
            get { return _limitSwitch3HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchHysteresis, value);
                _limitSwitch3HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchSource LimitSwitch4Source
        {
            get { return _limitSwitch4Source; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV4LimitSwitchSource, LimitSwitchSouceToInt(value));
                _limitSwitch4Source = value;
            }
        }

        ///<inheritdoc/>
        public LimitSwitchMode LimitSwitch4Mode
        {
            get { return _limitSwitch4Mode; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV4LimitSwitchMode, LimitSwitchModeToInt(value));
                _limitSwitch4Mode = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch4Level 
        {
            get { return _limitSwitch4LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV4LimitSwitchLevel, value);
                _limitSwitch4LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch4Hysteresis
        {
            get { return _limitSwitch4HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV4LimitSwitchHysteresis, value);
                _limitSwitch4HysteresisAndBandHeight = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch4LowerBandValue
        {
            get { return _limitSwitch4LevelAndLowerBandValue; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV3LimitSwitchLevel, value);
                _limitSwitch4LevelAndLowerBandValue = value;
            }
        }

        ///<inheritdoc/>
        public int LimitSwitch4BandHeight
        {
            get { return _limitSwitch4HysteresisAndBandHeight; }
            set
            {
                _connection.WriteInteger(ModbusCommands.LIV4LimitSwitchHysteresis, value);
                _limitSwitch4HysteresisAndBandHeight = value;
            }
        }
        #endregion

        #region =============== protected & private methods ================
        /// <summary>
        /// Convert limt switch mode from enum to int
        /// </summary>
        /// <param name="mode">Limit switch mode</param>
        /// <returns></returns>
        private int LimitSwitchModeToInt(LimitSwitchMode mode)
        {
            int result = 0;
            switch (mode)
            {
                case LimitSwitchMode.AboveLevel: result = 0; break;
                case LimitSwitchMode.BelowLevel: result = 1; break;
                case LimitSwitchMode.InsideBand: result = 2; break;
                case LimitSwitchMode.OutsideBand: result = 3; break;
                default:
                    result = 0; break;
            }
            return result;
        }

        /// <summary>
        /// Convert limt switch mode from int to enum
        /// </summary>
        /// <param name="mode">Limit switch mode from wtx device</param>
        /// <returns></returns>
        private LimitSwitchMode StringToLimitSwitchMode(string mode)
        {
            LimitSwitchMode result = LimitSwitchMode.AboveLevel;
            switch (mode)
            {
                case "0": result = LimitSwitchMode.AboveLevel; break;
                case "1": result = LimitSwitchMode.BelowLevel; break;
                case "2": result = LimitSwitchMode.InsideBand; break;
                case "3": result = LimitSwitchMode.OutsideBand; break;
                default:
                    result = LimitSwitchMode.AboveLevel; break;
            }
            return result;
        }

        /// <summary>
        /// Convert limt switch source from enum to int
        /// </summary>
        /// <param name="source">Limit switch source</param>
        /// <returns></returns>
        private int LimitSwitchSouceToInt(LimitSwitchSource source)
        {
            int result = 0;
            switch (source)
            {
                case LimitSwitchSource.Gross: result = 0; break;
                case LimitSwitchSource.Net: result = 1; break;
                default:
                    result = 0;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Convert limt switch source from string to enum
        /// </summary>
        /// <param name="mode">Limit switch source from wtx device</param>
        /// <returns></returns>
        private LimitSwitchSource StringToLimitSwitchSource(string mode)
        {
            LimitSwitchSource result = LimitSwitchSource.Gross;
            switch (mode)
            {
                case "0": result = LimitSwitchSource.Gross; break;
                case "1": result = LimitSwitchSource.Net; break;
                default:
                    result = LimitSwitchSource.Net;
                    break;
            }
            return result;
        }

        #endregion

    }
}