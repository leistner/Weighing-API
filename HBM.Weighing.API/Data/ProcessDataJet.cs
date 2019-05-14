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
using HBM.Weighing.API.WTX.Jet;
using System;

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// The class ProcessData contains all provess data like net weight, gross weight or tare value.
    /// </summary>
    public class ProcessDataJet : IProcessData
    {
        private JetBusConnection _connection;

        #region Constructor
        public ProcessDataJet(INetConnection Connection)
        {
            _connection = (JetBusConnection) Connection;

            _connection.UpdateDataClasses += UpdateData;

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
            Unit = 0;
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
            ApplicationMode = (ApplicationMode)_connection.GetDataFromDictionary(JetBusCommands.Application_mode);
            NetValue = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.Net_value));
            GrossValue = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.Gross_value));
            TareValue = NetValue - GrossValue;

            GeneralWeightError = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_GeneralWeightError));
            ScaleAlarm = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_ScaleAlarm));
            LimitStatus = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.WS_LimitStatus));
            WeightWithinLimits = (LimitStatus == 0);
            Underload = (LimitStatus == 1);
            Overload = (LimitStatus == 2);
            HigherSafeLoadLimit = (LimitStatus == 3);
            WeightMoving = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_WeightMoving));
            ScaleSealIsOpen = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_ScaleSealIsOpen));
            ManualTare = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_ManualTare));
            TareMode = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_WeightType));  // TareMode != WeightType
            ScaleRange = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.WS_ScaleRange));
            ZeroRequired = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_ZeroRequired));
            CenterOfZero = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_CenterOfZero));
            InsideZero = Convert.ToBoolean(_connection.GetDataFromDictionary(JetBusCommands.WS_InsideZero));            
            try
            {
                Decimals = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.Decimals)); //DDD Workaround
                Unit = Convert.ToInt32(_connection.GetDataFromDictionary(JetBusCommands.WS_Unit));
            }
            catch
            {
                Decimals = 1;
                Unit = 2;
            }

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

        public int Unit { get; private set; }

        public bool Handshake { get; private set; }

        public int Status { get; private set; }

        public bool Underload { get; private set; }

        public bool Overload { get; private set; }

        public bool WeightWithinLimits { get; private set; }

        public bool HigherSafeLoadLimit { get; private set; }
        #endregion
    }
}
