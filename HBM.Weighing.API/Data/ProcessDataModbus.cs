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

using HBM.Weighing.API.Utils;
using HBM.Weighing.API.WTX.Modbus;
using System;

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// The class ProcessData contains all provess data like net weight, gross weight or tare value.
    /// </summary>
    public class ProcessDataModbus : IProcessData
    {
        private INetConnection _connection;
        #region Constructor
        public ProcessDataModbus(INetConnection Connection)
        {
            PrintableWeight = new PrintableWeightType();
            Weight = new WeightType();

            _connection = Connection;

            _connection.UpdateDataClasses += UpdateData;
            Console.WriteLine("ProcessDataModbus");

            NetValue = 0;
            GrossValue = 0;
            TareValue = 0;
            GeneralWeightError = false;
            ScaleAlarm = false;
            LimitStatus = 0;
            WeightMoving = false;
            ScaleSealIsOpen = false;
            ManualTare = false;
            TareMode = false;
            ScaleRange = 0;
            ZeroRequired = false;
            CenterOfZero = false;
            InsideZero = false;
            Decimals = 0;
            Unit = "";
            Handshake = false;
            Status = 0;
            Underload = false;
            Overload = false;
            WeightWithinLimits = false;
            HigherSafeLoadLimit = false;
        }
        #endregion

        #region Update method
        public void UpdateData(object sender, EventArgs e)
        {
            NetValue = _connection.GetDataFromDictionary(ModbusCommands.Net);
            GrossValue = _connection.GetDataFromDictionary(ModbusCommands.Gross);
            TareValue = NetValue - GrossValue;
            GeneralWeightError = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.GeneralWeightError));

            ScaleAlarm = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ScaleAlarmTriggered));
            LimitStatus = (Convert.ToInt32(_connection.GetDataFromDictionary(ModbusCommands.Limit_status))); 
            WeightWithinLimits = (LimitStatus == 0);
            Underload = (LimitStatus == 1);
            Overload = (LimitStatus == 2);
            HigherSafeLoadLimit = (LimitStatus == 3);

            WeightMoving = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightMoving));
            ScaleSealIsOpen = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ScaleSealIsOpen));
            ManualTare = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ManualTare));
            TareMode = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.Tare_mode));
            ScaleRange = Convert.ToInt32(_connection.GetDataFromDictionary(ModbusCommands.ScaleRange));
            ZeroRequired = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ZeroRequired));
            CenterOfZero = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightinCenterOfZero));
            InsideZero = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightinZeroRange));

            ApplicationMode = (ApplicationMode)_connection.GetDataFromDictionary(ModbusCommands.Application_mode);
            Decimals = _connection.GetDataFromDictionary(ModbusCommands.Decimals);
            Unit = UnitIDToString(_connection.GetDataFromDictionary(ModbusCommands.Unit));
            Handshake = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.Handshake));
            Status = _connection.GetDataFromDictionary(ModbusCommands.Status);

            Weight.Update(MeasurementUtils.DigitToDouble(NetValue, Decimals), MeasurementUtils.DigitToDouble(GrossValue, Decimals));
            PrintableWeight.Update(MeasurementUtils.DigitToDouble(NetValue, Decimals), MeasurementUtils.DigitToDouble(GrossValue, Decimals), Decimals);
        }
        #endregion

        #region Properties
        public ApplicationMode ApplicationMode { get; private set; }

        public WeightType Weight { get; private set; }

        public PrintableWeightType PrintableWeight { get; private set; }

        public int NetValue { get; private set; }

        public int GrossValue { get; private set; }

        public int TareValue { get; private set; }

        public bool GeneralWeightError { get; private set; }

        public bool ScaleAlarm { get; private set; }

        public int LimitStatus { get; private set; }

        public bool WeightMoving { get; private set; }

        public bool ScaleSealIsOpen { get; private set; }

        public bool ManualTare { get; private set; }

        public bool TareMode { get; private set; }

        public int ScaleRange { get; private set; }

        public bool ZeroRequired { get; private set; }

        public bool CenterOfZero { get; private set; }

        public bool InsideZero { get; private set; }

        public int Decimals { get; private set; }

        public string Unit { get; private set; }

        public bool Handshake { get; private set; }

        public int Status { get; private set; }

        public bool Underload { get; private set; }

        public bool Overload { get; private set; }

        public bool WeightWithinLimits { get; private set; }

        public bool HigherSafeLoadLimit { get; private set; }
        #endregion

        private string UnitIDToString(int id)
        {
            switch (id)
            {
                case 0:
                    return "kg";
                case 1:
                    return "g";
                case 2:
                    return "t";
                case 3:
                    return "lb";
                case 4:
                    return "N";
                default:
                    return "-";
            }
        }
    }
}
