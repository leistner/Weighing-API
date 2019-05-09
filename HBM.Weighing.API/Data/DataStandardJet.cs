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
    public class DataStandardJet : IDataStandard
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

        private int _limitSwitch1Source;
        private int _limitSwitch1Mode;
        private int _limitSwitch1ActivationLevelLowerBandLimit;
        private int _limitSwitch1HysteresisBandHeight;

        private int _limitSwitch2Source;
        private int _limitSwitch2Mode;
        private int _limitSwitch2ActivationLevelLowerBandLimit;
        private int _limitSwitch2HysteresisBandHeight;

        private int _limitSwitch3Source;
        private int _limitSwitch3Mode;
        private int _limitSwitch3ActivationLevelLowerBandLimit;
        private int _limitSwitch3HysteresisBandHeight;

        private int _limitSwitch4Source;
        private int _limitSwitch4Mode;
        private int _limitSwitch4ActivationLevelLowerBandLimit;
        private int _limitSwitch4HysteresisBandHeight;

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
        private int _nominalLoad;

        private INetConnection _connection;
        private JetBusCommands _commands;
        #endregion

        #region constructor

        public DataStandardJet(INetConnection Connection)
        {
            _commands = new JetBusCommands();

            _connection = Connection;

            _connection.UpdateDataClasses += UpdateStandardData;

            _input1 = 0;
            _input2=0;
            _input3=0;
            _input4=0;

            _output1=0;
            _output2=0;
            _output3=0;
            _output4=0;

            _limitStatus1 = 0;
            _limitStatus2 = 0;
            _limitStatus3 = 0;
            _limitStatus4 = 0;

            _weightMemoryDay=0;
            _weightMemoryMonth=0;
            _weightMemoryYear=0;
            _weightMemorySeqNumber=0;
            _weightMemoryGross=0;
            _weightMemoryNet=0;

            _weight_storage=0;
            _mode_weight_storage=0;

            _limitSwitch1Source=0;
            _limitSwitch1Mode=0;
            _limitSwitch1ActivationLevelLowerBandLimit=0;
            _limitSwitch1HysteresisBandHeight=0;

            _limitSwitch2Source=0;
            _limitSwitch2Mode=0;
            _limitSwitch2ActivationLevelLowerBandLimit=0;
            _limitSwitch2HysteresisBandHeight=0;

            _limitSwitch3Source=0;
            _limitSwitch3Mode=0;
            _limitSwitch3ActivationLevelLowerBandLimit=0;
            _limitSwitch3HysteresisBandHeight=0;

            _limitSwitch4Source=0;
            _limitSwitch4Mode=0;
            _limitSwitch4ActivationLevelLowerBandLimit=0;
            _limitSwitch4HysteresisBandHeight=0;

            _manualTareValue = 0;
            _limitValue1Input = 0;
            _limitValue1Mode = 0;
            _limitValue1ActivationLevelLowerBandLimit = 0;
            _limitValue1HysteresisBandHeight = 0;
            _limitValue2Source = 0;
            _limitValue2Mode = 0;
            _limitValue2ActivationLevelLowerBandLimit = 0;
            _limitValue2HysteresisBandHeight = 0;

            _limitValue3Source = 0;
            _limitValue3Mode = 0;
            _limitValue3ActivationLevelLowerBandLimit = 0;
            _limitValue3HysteresisBandHeight = 0;
            _limitValue4Source = 0;
            _limitValue4Mode = 0;
            _limitValue4ActivationLevelLowerBandLimit = 0;
            _limitValue4HysteresisBandHeight = 0;
            _calibrationWeight = 0;
            _zeroLoad = 0;
            _nominalLoad = 0;
    }

        #endregion

        #region Update methods for standard mode

        public void UpdateStandardData(object sender, DataEventArgs e)
        {
            _input1 = e.DataDictionary[_commands.Status_digital_input_1.PathIndex];
            _input2 = e.DataDictionary[_commands.Status_digital_input_2.PathIndex];
            _input3 = e.DataDictionary[_commands.Status_digital_input_3.PathIndex];
            _input4 = e.DataDictionary[_commands.Status_digital_input_4.PathIndex];

            _output1 = e.DataDictionary[_commands.Status_digital_output_1.PathIndex];
            _output2 = e.DataDictionary[_commands.Status_digital_output_2.PathIndex];
            _output3 = e.DataDictionary[_commands.Status_digital_output_3.PathIndex];
            _output4 = e.DataDictionary[_commands.Status_digital_output_4.PathIndex];

            _limitStatus1 = (e.DataDictionary[_commands.Limit_value.PathIndex] & 0x1);
            _limitStatus2 = (e.DataDictionary[_commands.Limit_value.PathIndex] & 0x2) >> 1;
            _limitStatus3 = (e.DataDictionary[_commands.Limit_value.PathIndex] & 0x4) >> 2;
            _limitStatus4 = (e.DataDictionary[_commands.Limit_value.PathIndex] & 0x8) >> 3;

            _weight_storage = Convert.ToInt16(e.DataDictionary[(_commands.Storage_weight_mode).PathIndex]);

            if (e.DataDictionary[_commands.Application_mode.PathIndex] == 0 || e.DataDictionary[_commands.Application_mode.PathIndex] == 1)  // If application mode is in standard mode
            {
                _limitSwitch1Source = e.DataDictionary[_commands.Limit_value_monitoring_liv11.PathIndex];
                _limitSwitch1Mode = e.DataDictionary[_commands.Signal_source_liv12.PathIndex];
                _limitSwitch1ActivationLevelLowerBandLimit = e.DataDictionary[_commands.Switch_on_level_liv13.PathIndex];
                _limitSwitch1HysteresisBandHeight = e.DataDictionary[_commands.Switch_off_level_liv14.PathIndex];

                _limitSwitch2Source = e.DataDictionary[_commands.Limit_value_monitoring_liv21.PathIndex];
                _limitSwitch2Mode = e.DataDictionary[_commands.Signal_source_liv22.PathIndex];
                _limitSwitch2ActivationLevelLowerBandLimit = e.DataDictionary[_commands.Switch_on_level_liv23.PathIndex];
                _limitSwitch2HysteresisBandHeight = e.DataDictionary[_commands.Switch_off_level_liv24.PathIndex];

                _limitSwitch3Source = e.DataDictionary[_commands.Limit_value_monitoring_liv31.PathIndex];
                _limitSwitch3Mode = e.DataDictionary[_commands.Signal_source_liv32.PathIndex];
                _limitSwitch3ActivationLevelLowerBandLimit = e.DataDictionary[_commands.Switch_on_level_liv33.PathIndex];
                _limitSwitch3HysteresisBandHeight = e.DataDictionary[_commands.Switch_off_level_liv34.PathIndex];

                _limitSwitch4Source = e.DataDictionary[_commands.Limit_value_monitoring_liv41.PathIndex];
                _limitSwitch4Mode = e.DataDictionary[_commands.Signal_source_liv42.PathIndex];
                _limitSwitch4ActivationLevelLowerBandLimit = e.DataDictionary[_commands.Switch_on_level_liv43.PathIndex];
                _limitSwitch4HysteresisBandHeight = e.DataDictionary[_commands.Switch_off_level_liv44.PathIndex];
            }
        }
        #endregion

        #region Get-properties for standard mode

        public int Input1
        {
            get{ return _input1; }
            set
            {
                _connection.Write(_commands.Status_digital_input_1.PathIndex, value);
                _input1 = value;
            }
        }
        public int Input2
        {
            get{ return _input2; }
            set
            {
                _connection.Write(_commands.Status_digital_input_2.PathIndex, value);
                _input2 = value;
            }
        }
        public int Input3
        {
            get{ return _input3; }
            set
            {
                _connection.Write(_commands.Status_digital_input_3.PathIndex, value);
                _input3 = value;
            }
        }
        public int Input4
        {
            get{ return _input4; }
            set
            {
                _connection.Write(_commands.Status_digital_input_4.PathIndex, value);
                _input4 = value;
            }
        }
        public int Output1
        {
            get{ return _output1; }
            set
            {
                _connection.Write(_commands.Status_digital_output_1.PathIndex, value);
                _output1 = value;
            }
        }
        public int Output2
        {
            get{ return _output2; }
            set
            {
                _connection.Write(_commands.Status_digital_output_2.PathIndex, value);
                _output2 = value;
            }
        }
        public int Output3
        {
            get{ return _output3; }
            set
            {
                _connection.Write(_commands.Status_digital_output_3.PathIndex, value);
                _output3 = value;
            }
        }
        public int Output4
        {
            get{ return _output4; }
            set
            {
                _connection.Write(_commands.Status_digital_output_4.PathIndex, value);
                _output4 = value;
            }
        }
        public int LimitStatus1
        {
            get{ return _limitStatus1; }
        }
        public int LimitStatus2
        {
            get{ return _limitStatus2; }
        }
        public int LimitStatus3
        {
            get{ return _limitStatus3; }
        }
        public int LimitStatus4
        {
            get{ return _limitStatus4; }
        }
        public int WeightMemDay
        {
            get{ return _weightMemoryDay; }
        }
        public int WeightMemMonth
        {
            get{ return _weightMemoryMonth; }
        }
        public int WeightMemYear
        {
            get{ return _weightMemoryYear;}
        }
        public int WeightMemSeqNumber
        {
            get{ return _weightMemorySeqNumber; }
        }
        public int WeightMemGross
        {
            get{ return _weightMemoryGross; }
        }
        public int WeightMemNet
        {
            get{ return _weightMemoryNet; }
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
            get { return _limitSwitch1Source; }
            set
            {
                  _connection.Write(_commands.Tare_value.PathIndex, value);
                  _manualTareValue = value;
            }
        }
        public int LimitSwitch1Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch1Mode; }
            set
            {
                _connection.Write(_commands.Signal_source_liv12.PathIndex, value);
                _limitValue1Input = value;
            }
        }
        public int LimitSwitch1Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                _connection.Write(_commands.Limit_value_monitoring_liv11.PathIndex, value);
                _limitValue1Mode = value;
            }
        }
        public int LimitSwitch1Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv14.PathIndex, value);
                _limitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch1LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(_commands.Switch_on_level_liv13.PathIndex, value);
                _limitValue1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch1BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv14.PathIndex, value);
                _limitValue1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch2Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch2Source; }
            set
            {
                _connection.Write(_commands.Signal_source_liv22.PathIndex, value);
                _limitValue2Source = value;
            }
        }
        public int LimitSwitch2Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch2Mode; }
            set
            {
                _connection.Write(_commands.Limit_value_monitoring_liv21.PathIndex, value);
                _limitValue2Mode = value;
            }
        }
        public int LimitSwitch2Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(_commands.Switch_on_level_liv23.PathIndex, value);
                _limitValue2ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv24.PathIndex, value);
                _limitValue2HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch2LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(_commands.Switch_on_level_liv13.PathIndex, value);
                _limitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv14.PathIndex, value);
                _limitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch3Source; }
            set
            {
                _connection.Write(_commands.Signal_source_liv32.PathIndex, value);
                _limitValue3Source = value;
            }
        }

        public int LimitSwitch3Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch3Mode; }
            set
            {
                _connection.Write(_commands.Limit_value_monitoring_liv31.PathIndex, value);
                _limitValue3Mode = value;
            }
        }
        public int LimitSwitch3ActivationLevelLowerBandLimit // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(_commands.Switch_on_level_liv33.PathIndex, value);
                _limitValue3ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch3Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv34.PathIndex, value);
                _limitValue3HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                _connection.Write(_commands.Signal_source_liv42.PathIndex, value);
                _limitValue4Source = value;
            }
        }
        public int LimitSwitch3BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.Write(_commands.Limit_value_monitoring_liv41.PathIndex, value);
                _limitValue4Mode = value;
            }
        }
        public int LimitSwitch4Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch4Source; }
            set
            {
                _connection.WriteArray(_commands.Switch_on_level_liv43.PathIndex, value);
                _limitValue4ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch4Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch4Mode; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv44.PathIndex, value);
                _limitValue4HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch4Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(_commands.Lft_scale_calibration_weight.PathIndex, value);
                _calibrationWeight = value;
            }
        }
        public int LimitSwitch4Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Ldw_dead_weight.PathIndex, value);
                _zeroLoad = value;
            }
        }
        public int LimitSwitch4LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(_commands.Lwt_nominal_value.PathIndex, value);
                _nominalLoad = value;
            }
        }
        public int LimitSwitch4BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(_commands.Switch_off_level_liv14.PathIndex, value);
                _limitSwitch1HysteresisBandHeight = value;
            }
        }
        #endregion

    }
}