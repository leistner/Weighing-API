// <copyright file="DataStandard.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using HBM.Weighing.API.WTX.Modbus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// Implementation of the interface IDataStandard for the standard mode.
    /// The class DataStandard contains the data input word and data output words for the filler mode
    /// of WTX device 120 and 110.
    /// </summary>
    public class DataStandardModbus : IDataStandard
    {
        #region privates for standard mode

        // Input words :

        private int _input1;
        private int _input2;
        private int _input3;
        private int _input4;

        private int _output1;
        private int _output2;
        private int _output3;
        private int _output4;

        private int _limitStatus1;
        private int _limitStatus2;
        private int _limitStatus3;
        private int _limitStatus4;

        // Modbus only:

        private int _weightMemoryDay;
        private int _weightMemoryMonth;
        private int _weightMemoryYear;
        private int _weightMemorySeqNumber;
        private int _weightMemoryGross;
        private int _weightMemoryNet;

        // Jetbus only:

        private int _weight_storage;
        private int _mode_weight_storage;

        // Output words: 

        private int _LimitSwitch1Input;
        private int _LimitSwitch1Mode;
        private int _LimitSwitch1ActivationLevelLowerBandLimit;
        private int _LimitSwitch1HysteresisBandHeight;

        private int _LimitSwitch2Source;
        private int _LimitSwitch2Mode;
        private int _LimitSwitch2ActivationLevelLowerBandLimit;
        private int _LimitSwitch2HysteresisBandHeight;

        private int _LimitSwitch3Source;
        private int _LimitSwitch3Mode;
        private int _LimitSwitch3ActivationLevelLowerBandLimit;
        private int _LimitSwitch3HysteresisBandHeight;

        private int _LimitSwitch4Source;
        private int _LimitSwitch4Mode;
        private int _LimitSwitch4ActivationLevelLowerBandLimit;
        private int _LimitSwitch4HysteresisBandHeight;

        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nominalLoad;

        private int _LimitSwitchMonitoringLIV11;
        private int _signalSourceLIV12;
        private int _switchOnLevelLIV13;
        private int _switchOffLevelLIV14;
        private int _LimitSwitchMonitoringLIV21;
        private int _signalSourceLIV22;
        private int _switchOnLevelLIV23;
        private int _switchOffLevelLIV24;
        private int _LimitSwitchMonitoringLIV31;
        private int _signalSourceLIV32;
        private int _switchOnLevelLIV33;
        private int _switchOffLevelLIV34;
        private int _LimitSwitchMonitoringLIV41;
        private int _signalSourceLIV42;
        private int _switchOnLevelLIV43;
        private int _switchOffLevelLIV44;

        private INetConnection _connection;
        #endregion

        #region constructor

        public DataStandardModbus(INetConnection Connection)
        {
            _connection = Connection;

            _connection.UpdateDataClasses += UpdateStandardData;

            _input1 = 0;
            _input2 = 0;
            _input3 = 0;
            _input4 = 0;

            _output1 = 0;
            _output2 = 0;
            _output3 = 0;
            _output4 = 0;

            _limitStatus1 = 0;
            _limitStatus2 = 0;
            _limitStatus3 = 0;
            _limitStatus4 = 0;

            _weightMemoryDay = 0;
            _weightMemoryMonth = 0;
            _weightMemoryYear = 0;
            _weightMemorySeqNumber = 0;
            _weightMemoryGross = 0;
            _weightMemoryNet = 0;

            _weight_storage = 0;
            _mode_weight_storage = 0;

            _LimitSwitch1Input = 0;
            _LimitSwitch1Mode = 0;

            _LimitSwitch1ActivationLevelLowerBandLimit = 0;
            _LimitSwitch1HysteresisBandHeight = 0;
            _LimitSwitch2Source = 0;
            _LimitSwitch2Mode = 0;

            _LimitSwitch2ActivationLevelLowerBandLimit = 0;
            _LimitSwitch2HysteresisBandHeight = 0;
            _LimitSwitch3Source = 0;
            _LimitSwitch3Mode = 0;

            _LimitSwitch3ActivationLevelLowerBandLimit = 0;
            _LimitSwitch3HysteresisBandHeight = 0;
            _LimitSwitch4Source = 0;

            _LimitSwitch4Mode = 0;
            _LimitSwitch4ActivationLevelLowerBandLimit = 0;
            _LimitSwitch4HysteresisBandHeight = 0;

            _calibrationWeight = 0;
            _zeroLoad = 0;
            _nominalLoad = 0;

            _LimitSwitchMonitoringLIV11 = 0;
            _signalSourceLIV12 = 0;
            _switchOnLevelLIV13 = 0;
            _switchOffLevelLIV14 = 0;
            _LimitSwitchMonitoringLIV21 = 0;
            _signalSourceLIV22 = 0;
            _switchOnLevelLIV23 = 0;
            _switchOffLevelLIV24 = 0;
            _LimitSwitchMonitoringLIV31 = 0;
            _signalSourceLIV32 = 0;
            _switchOnLevelLIV33 = 0;
            _switchOffLevelLIV34 = 0;
            _LimitSwitchMonitoringLIV41 = 0;
            _signalSourceLIV42 = 0;
            _switchOnLevelLIV43 = 0;
            _switchOffLevelLIV44 = 0;

        }

        #endregion

        #region Update methods for standard mode

        public void UpdateStandardData(object sender, DataEventArgs e)
        {
            _input1 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_INPUT_1] & 0x1);
            _input2 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_INPUT_2] & 0x2) >> 1;
            _input3 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_INPUT_3] & 0x4) >> 2;
            _input4 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_INPUT_4] & 0x8) >> 3;

            _output1 = e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_OUTPUT_1] & 0x1;
            _output2 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_OUTPUT_2] & 0x2) >> 1;
            _output3 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_OUTPUT_3] & 0x4) >> 2;
            _output4 = (e.DataDictionary[_connection.IDCommands.STATUS_DIGITAL_OUTPUT_4] & 0x8) >> 3;

            _limitStatus1 = (e.DataDictionary[_connection.IDCommands.LIMIT_VALUE] & 0x1);
            _limitStatus2 = (e.DataDictionary[_connection.IDCommands.LIMIT_VALUE] & 0x2) >> 1;
            _limitStatus3 = (e.DataDictionary[_connection.IDCommands.LIMIT_VALUE] & 0x4) >> 2;
            _limitStatus4 = (e.DataDictionary[_connection.IDCommands.LIMIT_VALUE] & 0x8) >> 3;

            _weightMemoryDay = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[0]]);
            _weightMemoryMonth = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[1]]);
            _weightMemoryYear = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[2]]);
            _weightMemorySeqNumber = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[3]]);
            _weightMemoryGross = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[4]]);
            _weightMemoryNet = Convert.ToInt16(e.DataDictionary[_connection.IDCommands.WEIGHT_MEMORY_STANDARD[5]]);

            if (e.DataDictionary[_connection.IDCommands.APPLICATION_MODE] == 0 || e.DataDictionary[_connection.IDCommands.APPLICATION_MODE] == 1)  // If application mode is in standard mode
            {
                _LimitSwitchMonitoringLIV11 = e.DataDictionary[_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV11];
                _signalSourceLIV12 = e.DataDictionary[_connection.IDCommands.SIGNAL_SOURCE_LIV12];
                _switchOnLevelLIV13 = e.DataDictionary[_connection.IDCommands.SWITCH_ON_LEVEL_LIV13];
                _switchOffLevelLIV14 = e.DataDictionary[_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14];

                _LimitSwitchMonitoringLIV21 = e.DataDictionary[_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV21];
                _signalSourceLIV22 = e.DataDictionary[_connection.IDCommands.SIGNAL_SOURCE_LIV22];
                _switchOnLevelLIV23 = e.DataDictionary[_connection.IDCommands.SWITCH_ON_LEVEL_LIV23];
                _switchOffLevelLIV24 = e.DataDictionary[_connection.IDCommands.SWITCH_OFF_LEVEL_LIV24];

                _LimitSwitchMonitoringLIV31 = e.DataDictionary[_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV31];
                _signalSourceLIV32 = e.DataDictionary[_connection.IDCommands.SIGNAL_SOURCE_LIV32];
                _switchOnLevelLIV33 = e.DataDictionary[_connection.IDCommands.SWITCH_ON_LEVEL_LIV33];
                _switchOffLevelLIV34 = e.DataDictionary[_connection.IDCommands.SWITCH_OFF_LEVEL_LIV34];

                _LimitSwitchMonitoringLIV41 = e.DataDictionary[_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV41];
                _signalSourceLIV42 = e.DataDictionary[_connection.IDCommands.SIGNAL_SOURCE_LIV42];
                _switchOnLevelLIV43 = e.DataDictionary[_connection.IDCommands.SWITCH_ON_LEVEL_LIV43];
                _switchOffLevelLIV44 = e.DataDictionary[_connection.IDCommands.SWITCH_OFF_LEVEL_LIV44];
            }
        }
        #endregion

        #region Get-properties for standard mode

        public int Input1
        {
            get { return _input1; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_INPUT_1), value);
                _input1 = value;
            }
        }
        public int Input2
        {
            get { return _input2; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_INPUT_2), value);
                _input2 = value;
            }
        }
        public int Input3
        {
            get { return _input3; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_INPUT_3), value);
                _input3 = value;
            }
        }
        public int Input4
        {
            get { return _input4; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_INPUT_4), value);
                _input4 = value;
            }
        }
        public int Output1
        {
            get { return _output1; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_OUTPUT_1), value);
                _output1 = value;
            }
        }
        public int Output2
        {
            get { return _output2; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_OUTPUT_2), value);
                _output2 = value;
            }
        }
        public int Output3
        {
            get { return _output3; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_OUTPUT_3), value);
                _output3 = value;
            }
        }
        public int Output4
        {
            get { return _output4; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.FUNCTION_DIGITAL_OUTPUT_4), value);
                _output4 = value;
            }
        }
        public int LimitStatus1
        {
            get { return _limitStatus1; }
        }
        public int LimitStatus2
        {
            get { return _limitStatus2; }
        }
        public int LimitStatus3
        {
            get { return _limitStatus3; }
        }
        public int LimitStatus4
        {
            get { return _limitStatus4; }
        }
        public int WeightMemDay
        {
            get { return _weightMemoryDay; }
        }
        public int WeightMemMonth
        {
            get { return _weightMemoryMonth; }
        }
        public int WeightMemYear
        {
            get { return _weightMemoryYear; }
        }
        public int WeightMemSeqNumber
        {
            get { return _weightMemorySeqNumber; }
        }
        public int WeightMemGross
        {
            get { return _weightMemoryGross; }
        }
        public int WeightMemNet
        {
            get { return _weightMemoryNet; }
        }
        public int WeightStorage
        {
            get { return _weight_storage; }
            set { this._weight_storage = value; }
        }
        #endregion

        #region Get-/Set-properties for standard mode 


        public int LimitSwitch1Source // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch1Input; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.SIGNAL_SOURCE_LIV12), value);
                _LimitSwitch1Input = value;
            }
        }
        public int LimitSwitch1Mode // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch1Mode; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV11), value);
                _LimitSwitch1Mode = value;
            }
        }
        public int LimitSwitch1Level // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV13), value);
                _LimitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch1Hysteresis // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14), value);
                _LimitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch1LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV13), value);
                _LimitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch1BandHeight // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14), value);
                _LimitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch2Source // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch2Source; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.SIGNAL_SOURCE_LIV22), value);
                _LimitSwitch2Source = value;
            }
        }
        public int LimitSwitch2Mode // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch2Mode; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV21), value);
                _LimitSwitch2Mode = value;
            }
        }
        public int LimitSwitch2Level // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch2ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV23), value);
                _LimitSwitch2ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2Hysteresis // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch2HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV24), value);
                _LimitSwitch2HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch2LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV13), value);
                _LimitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2BandHeight // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14), value);
                _LimitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3Source // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch3Source; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.SIGNAL_SOURCE_LIV32), value);
                _LimitSwitch3Source = value;
            }
        }
        public int LimitSwitch3Mode // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch3Mode; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV31), value);
                _LimitSwitch3Mode = value;
            }
        }
        public int LimitSwitch3ActivationLevelLowerBandLimit // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch3ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV33), value);
                _LimitSwitch3ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch3Hysteresis // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch3HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV34), value);
                _LimitSwitch3HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV13), value);
                _LimitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch3BandHeight // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14), value);
                _LimitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch4Source // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch4Source; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.SIGNAL_SOURCE_LIV42), value);
                _LimitSwitch4Source = value;
            }
        }
        public int LimitSwitch4Mode // Type : unsigned integer 8 Bit
        {
            get { return _LimitSwitch4Mode; }
            set
            {
                _connection.Write(this.getIndex(_connection.IDCommands.LIMIT_VALUE_MONITORING_LIV41), value);
                _LimitSwitch4Mode = value;
            }
        }
        public int LimitSwitch4Level // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch4ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV43), value);
                _LimitSwitch4ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch4Hysteresis // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch4HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV44), value);
                _LimitSwitch4HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch4LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_ON_LEVEL_LIV13), value);
                _LimitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch4BandHeight // Type : signed integer 32 Bit
        {
            get { return _LimitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(this.getIndex(_connection.IDCommands.SWITCH_OFF_LEVEL_LIV14), value);
                _LimitSwitch1HysteresisBandHeight = value;
            }
        }
        #endregion

        private string getIndex(string IDCommandParam)
        {
            return IDCommandParam.Split('/')[0];
        }


    }
}