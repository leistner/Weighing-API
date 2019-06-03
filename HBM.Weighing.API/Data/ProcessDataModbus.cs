// <copyright file="ProcessDataModbus.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.API, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.API.Data
{
    using System;
    using System.Collections.Generic;
    using Hbm.Weighing.API.Utils;
    using Hbm.Weighing.API.WTX.Modbus;

    /// <summary>
    /// The class ProcessData contains all process data like net weight, gross weight or tare value.
    /// </summary>
    public class ProcessDataModbus : IProcessData
    {
        #region ==================== constants & fields ====================
        private INetConnection _connection;
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Constructor of class ProcessDataModbus : Initalizes values and connects 
        /// the eventhandler from Connection to the interal update method
        /// </summary>
        public ProcessDataModbus(INetConnection Connection)
        {
            _connection = Connection;
            _connection.UpdateData += UpdateData;

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

        #region ==================== events & delegates ====================
        /// <summary>
        /// Updates & converts the values from buffer (Dictionary<string,string>) 
        /// </summary>
        /// <param name="sender">Connection class</param>
        /// <param name="e">EventArgs, Event argument</param>
        public void UpdateData(object sender, EventArgs e)
        {
            try
            {
            GeneralWeightError = Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusGeneralWeightError)));
            ScaleAlarm = Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusScaleAlarm)));
            int LimitStatus = (Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusLimitStatus)));
            Underload = (LimitStatus == 1);
            Overload = (LimitStatus == 2);
            HigherSafeLoadLimit = (LimitStatus == 3);
            WeightStable = !Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusWeightMoving)));
            LegalForTrade = !Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusScaleSealIsOpen)));
            TareMode = EvaluateTareMode(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.TMDTareMode)), Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusManualTare)));
            ScaleRange = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusScaleRange));
            ZeroRequired = Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusZeroRequired)));
            CenterOfZero = Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusCenterOfZero)));
            InsideZero = Convert.ToBoolean(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461WeightStatusInsideZero)));
            ApplicationMode = (ApplicationMode)Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.IMDApplicationMode));
            Decimals = Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461Decimals));
            Unit = UnitIDToString(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461Unit)));
            Weight.Update(MeasurementUtils.DigitToDouble(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461NetValue)), Decimals), MeasurementUtils.DigitToDouble(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461GrossValue)), Decimals));
            PrintableWeight.Update(MeasurementUtils.DigitToDouble(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461NetValue)), Decimals), MeasurementUtils.DigitToDouble(Convert.ToInt32(_connection.ReadFromBuffer(ModbusCommands.CIA461GrossValue)), Decimals), Decimals);
            }
            catch (KeyNotFoundException)
            {
                Console.WriteLine("KeyNotFoundException in class ProcessDataModbus, update method");
                //_connection.CommunicationLog.Invoke(this, new LogEvent((new KeyNotFoundException()).Message));
            }
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

        /// <summary>
        /// Sets the integer from the wtx device register to the specific unit as a string
        /// </summary>
        /// <param name="id">integer from wtx register</param>
        /// <returns></returns>
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
        
        /// <summary>
        /// Sets the tare value
        /// </summary>
        /// <param name="tare">tare value as integer</param>
        /// <param name="presettare">tare value set before as integer</param>
        /// <returns></returns>
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
