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

        // Output words: 

        private int _limitSwitch1Input;
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
               
        private ModbusTcpConnection _connection;
        #endregion

        #region constructor

        public DataStandardModbus(INetConnection Connection)
        {
            _connection = (ModbusTcpConnection)Connection;

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
            _weightMemoryNet = 0;;

            _limitSwitch1Input = 0;
            _limitSwitch1Mode = 0;

            _limitSwitch1ActivationLevelLowerBandLimit = 0;
            _limitSwitch1HysteresisBandHeight = 0;
            _limitSwitch2Source = 0;
            _limitSwitch2Mode = 0;

            _limitSwitch2ActivationLevelLowerBandLimit = 0;
            _limitSwitch2HysteresisBandHeight = 0;
            _limitSwitch3Source = 0;
            _limitSwitch3Mode = 0;

            _limitSwitch3ActivationLevelLowerBandLimit = 0;
            _limitSwitch3HysteresisBandHeight = 0;
            _limitSwitch4Source = 0;

            _limitSwitch4Mode = 0;
            _limitSwitch4ActivationLevelLowerBandLimit = 0;
            _limitSwitch4HysteresisBandHeight = 0;
            
        }

        #endregion

        #region Update methods for standard mode

        public void UpdateStandardData(object sender, EventArgs e)
        {
            _input1 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_input_1);
            _input2 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_input_2);
            _input3 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_input_3);
            _input4 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_input_4);

            _output1 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_output_1);
            _output2 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_output_2);
            _output3 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_output_3);
            _output4 = _connection.GetDataFromDictionary(ModbusCommands.Status_digital_output_4);

            _limitStatus1 = _connection.GetDataFromDictionary(ModbusCommands.Limit_value);
            _limitStatus2 = _connection.GetDataFromDictionary(ModbusCommands.Limit_value);
            _limitStatus3 = _connection.GetDataFromDictionary(ModbusCommands.Limit_value);
            _limitStatus4 = _connection.GetDataFromDictionary(ModbusCommands.Limit_value);

            _weightMemoryDay = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemDayStandard));
            _weightMemoryMonth = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemMonthStandard));
            _weightMemoryYear = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemYearStandard));
            _weightMemorySeqNumber = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemSeqNumberStandard));
            _weightMemoryGross = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemGrossStandard));
            _weightMemoryNet = Convert.ToInt16(_connection.GetDataFromDictionary(ModbusCommands.WeightMemNetStandard));

            if (_connection.GetDataFromDictionary(ModbusCommands.Application_mode) == 0 || _connection.GetDataFromDictionary(ModbusCommands.Application_mode) == 1)  // If application mode is in standard mode
            {
                _limitSwitch1Input = _connection.GetDataFromDictionary(ModbusCommands.LimitValue1Mode);
                _limitSwitch1Mode = _connection.GetDataFromDictionary(ModbusCommands.LimitValue1Input);
                _limitSwitch1ActivationLevelLowerBandLimit = _connection.GetDataFromDictionary(ModbusCommands.LimitValue1ActivationLevelLowerBandLimit);
                _limitSwitch1HysteresisBandHeight = _connection.GetDataFromDictionary(ModbusCommands.LimitValue1HysteresisBandHeight);

                _limitSwitch2Mode = _connection.GetDataFromDictionary(ModbusCommands.LimitValue2Source);
                _limitSwitch2Source = _connection.GetDataFromDictionary(ModbusCommands.LimitValue2Mode);
                _limitSwitch2ActivationLevelLowerBandLimit = _connection.GetDataFromDictionary(ModbusCommands.LimitValue2ActivationLevelLowerBandLimit);
                _limitSwitch2HysteresisBandHeight = _connection.GetDataFromDictionary(ModbusCommands.LimitValue2HysteresisBandHeight);

                _limitSwitch3Mode = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3Source);
                _limitSwitch3Source = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3Mode);
                _limitSwitch3ActivationLevelLowerBandLimit = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3ActivationLevelLowerBandLimit);
                _limitSwitch3HysteresisBandHeight = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3HysteresisBandHeight);

                _limitSwitch4Mode = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3Source);
                _limitSwitch4Source = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3Mode);
                _limitSwitch4ActivationLevelLowerBandLimit = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3ActivationLevelLowerBandLimit);
                _limitSwitch4HysteresisBandHeight = _connection.GetDataFromDictionary(ModbusCommands.LimitValue3HysteresisBandHeight);
            }
        }
        #endregion

        #region Get-properties for standard mode

        public int Input1
        {
            get { return _input1; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_input_1.Register, value);
                _input1 = value;
            }
        }
        public int Input2
        {
            get { return _input2; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_input_2.Register, value);
                _input2 = value;
            }
        }
        public int Input3
        {
            get { return _input3; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_input_3.Register, value);
                _input3 = value;
            }
        }
        public int Input4
        {
            get { return _input4; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_input_4.Register, value);
                _input4 = value;
            }
        }
        public int Output1
        {
            get { return _output1; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_output_1.Register, value);
                _output1 = value;
            }
        }
        public int Output2
        {
            get { return _output2; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_output_2.Register, value);
                _output2 = value;
            }
        }
        public int Output3
        {
            get { return _output3; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_output_3.Register, value);
                _output3 = value;
            }
        }
        public int Output4
        {
            get { return _output4; }
            set
            {
                _connection.Write(ModbusCommands.Status_digital_output_4.Register, value);
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
        #endregion

        #region Get-/Set-properties for standard mode 
        
        public int LimitSwitch1Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch1Input; }
            set
            {
                _connection.Write(ModbusCommands.ManualTareValue.Register, value);
                _limitSwitch1Input = value;
            }
        }
        public int LimitSwitch1Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch1Mode; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue1Input.Register, value);
                _limitSwitch1Mode = value;
            }
        }
        public int LimitSwitch1Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue1Mode.Register, value);
                _limitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch1Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue1HysteresisBandHeight.Register, value);
                _limitSwitch1HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch1LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.WriteArray(ModbusCommands.LimitValue2ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch1ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch1BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch1HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue1HysteresisBandHeight.Register, value);
                _limitSwitch1HysteresisBandHeight = value;
            }
        }

        public int LimitSwitch2Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch2Source; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue2Source.Register, value);
                _limitSwitch2Source = value;
            }
        }
        public int LimitSwitch2Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch2Mode; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue2Mode.Register, value);
                _limitSwitch2Mode = value;
            }
        }
        public int LimitSwitch2Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2ActivationLevelLowerBandLimit; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue2ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch2ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2HysteresisBandHeight; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue2HysteresisBandHeight.Register, value);
                _limitSwitch2HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch2LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2ActivationLevelLowerBandLimit; }
            set
            {
                this._connection.Write(ModbusCommands.LimitValue2ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch2ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch2BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch2HysteresisBandHeight; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue2HysteresisBandHeight.Register, value);
                _limitSwitch2HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch3Source; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue3Source.Register, value);
                _limitSwitch3Source = value;
            }
        }
        public int LimitSwitch3Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch3Mode; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue3Mode.Register, value);
                _limitSwitch3Mode = value;
            }
        }
        public int LimitSwitch3ActivationLevelLowerBandLimit // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue3ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch3ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch3Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue3HysteresisBandHeight.Register, value);
                _limitSwitch3HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch3LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3ActivationLevelLowerBandLimit; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue3ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch3ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch3BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch3HysteresisBandHeight; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue3HysteresisBandHeight.Register, value);
                _limitSwitch3HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch4Source // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch4Source; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue4Source.Register, value);
                _limitSwitch4Source = value;
            }
        }
        public int LimitSwitch4Mode // Type : unsigned integer 8 Bit
        {
            get { return _limitSwitch4Mode; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue4Mode.Register, value);
                _limitSwitch4Mode = value;
            }
        }
        public int LimitSwitch4Level // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue4ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch4ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch4Hysteresis // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4HysteresisBandHeight; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue4HysteresisBandHeight.Register, value);
                _limitSwitch4HysteresisBandHeight = value;
            }
        }
        public int LimitSwitch4LowerBandValue // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4ActivationLevelLowerBandLimit; }
            set
            {
                _connection.WriteArray(ModbusCommands.LimitValue3ActivationLevelLowerBandLimit.Register, value);
                _limitSwitch4ActivationLevelLowerBandLimit = value;
            }
        }
        public int LimitSwitch4BandHeight // Type : signed integer 32 Bit
        {
            get { return _limitSwitch4HysteresisBandHeight; }
            set
            {
                _connection.Write(ModbusCommands.LimitValue4HysteresisBandHeight.Register, value);
                _limitSwitch4HysteresisBandHeight = value;
            }
        }
        public int WeightStorage { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        #endregion
    }
}