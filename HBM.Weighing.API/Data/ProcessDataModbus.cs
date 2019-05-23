// <copyright file="ProcessDataModbus.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API.Data
{
    using System;
    using HBM.Weighing.API.Utils;
    using HBM.Weighing.API.WTX.Modbus;

    /// <summary>
    /// The class ProcessData contains all process data like net weight, gross weight or tare value.
    /// </summary>
    public class ProcessDataModbus : IProcessData
    {
        #region ==================== constants & fields ====================
        private INetConnection _connection;
        #endregion

        #region ==================== events & delegates ====================
        public void UpdateData(object sender, EventArgs e)
        {
            GeneralWeightError = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.GeneralWeightError));
            ScaleAlarm = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ScaleAlarmTriggered));
            int LimitStatus = (Convert.ToInt32(_connection.GetDataFromDictionary(ModbusCommands.Limit_status)));
            Underload = (LimitStatus == 1);
            Overload = (LimitStatus == 2);
            HigherSafeLoadLimit = (LimitStatus == 3);
            WeightStable = !Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightMoving));
            LegalForTrade = !Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ScaleSealIsOpen));
            TareMode = EvaluateTareMode(_connection.GetDataFromDictionary(ModbusCommands.Tare_mode), _connection.GetDataFromDictionary(ModbusCommands.ManualTare));
            ScaleRange = Convert.ToInt32(_connection.GetDataFromDictionary(ModbusCommands.ScaleRange));
            ZeroRequired = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.ZeroRequired));
            CenterOfZero = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightinCenterOfZero));
            InsideZero = Convert.ToBoolean(_connection.GetDataFromDictionary(ModbusCommands.WeightinZeroRange));
            ApplicationMode = (ApplicationMode)_connection.GetDataFromDictionary(ModbusCommands.Application_mode);
            Decimals = _connection.GetDataFromDictionary(ModbusCommands.Decimals);
            Unit = UnitIDToString(_connection.GetDataFromDictionary(ModbusCommands.Unit));
            Weight.Update(MeasurementUtils.DigitToDouble(_connection.GetDataFromDictionary(ModbusCommands.Net), Decimals), MeasurementUtils.DigitToDouble(_connection.GetDataFromDictionary(ModbusCommands.Gross), Decimals));
            PrintableWeight.Update(MeasurementUtils.DigitToDouble(_connection.GetDataFromDictionary(ModbusCommands.Net), Decimals), MeasurementUtils.DigitToDouble(_connection.GetDataFromDictionary(ModbusCommands.Gross), Decimals), Decimals);
        }
        #endregion
        
        #region =============== constructors & destructors =================
        public ProcessDataModbus(INetConnection Connection)
        {
            _connection = Connection;
            _connection.UpdateDataClasses += UpdateData;

            PrintableWeight = new PrintableWeightType();
            Weight = new WeightType();
            GeneralWeightError = false;
            ScaleAlarm = false;
            WeightStable = false;
            LegalForTrade = false;
            TareMode = TareMode.None;
            ScaleRange = 0;
            ZeroRequired = false;
            CenterOfZero = false;
            InsideZero = false;
            Decimals = 0;
            Unit = "";
            Underload = false;
            Overload = false;
            HigherSafeLoadLimit = false;
        }
        #endregion
        
        #region ======================== properties ========================
        public ApplicationMode ApplicationMode { get; private set; }

        public WeightType Weight { get; private set; }

        public PrintableWeightType PrintableWeight { get; private set; }

        public string Unit { get; private set; }

        public int Decimals { get; private set; }

        public TareMode TareMode { get; private set; }

        public bool WeightStable { get; private set; }

        public bool CenterOfZero { get; private set; }

        public bool InsideZero { get; private set; }

        public bool ZeroRequired { get; private set; }
        
        public int ScaleRange { get; private set; }

        public bool LegalForTrade { get; private set; }

        public bool Underload { get; private set; }

        public bool Overload { get; private set; }

        public bool HigherSafeLoadLimit { get; private set; }
        
        public bool GeneralWeightError { get; private set; }

        public bool ScaleAlarm { get; private set; }
        #endregion

        #region =============== protected & private methods ================
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
                    return "";
            }
        }
        
        private TareMode EvaluateTareMode(int tare, int presettare)
        {
            if (tare > 0)
                if (presettare > 0)
                    return TareMode.PresetTare;
                else
                    return TareMode.Tare;
            else
                return TareMode.None;
        }
        #endregion
    }
}
