// <copyright file="WTXJet.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using System;
using System.Collections.Generic;
using System.Threading;
using HBM.Weighing.API.Data;
using HBM.Weighing.API.WTX.Jet;

namespace HBM.Weighing.API.WTX
{
    /// <summary>
    /// This class handles the data from JetBusConnection for IProcessData. 
    /// WtxJet fetches, interprets the data( method OnData(sender, DataEventArgs) ) and 
    /// send it to the GUI or application class by an eventhandler (=ProcessDataReceived). 
    /// </summary>
    public class WTXJet : BaseWtDevice
    {       
        #region Constants
        private const int CONVERISION_FACTOR_MVV_TO_D = 500000; //   2 / 1000000; // 2mV/V correspond 1 million digits (d)   

        private const int SCALE_COMMAND_CALIBRATE_ZERO = 2053923171;
        private const int SCALE_COMMAND_CALIBRATE_NOMINAL = 1852596579;
        private const int SCALE_COMMAND_EXIT_CALIBRATE = 1953069157;
        private const int SCALE_COMMAND_TARE = 1701994868;
        private const int SCALE_COMMAND_CLEAR_PEAK_VALUES = 1801545072;
        private const int SCALE_COMMAND_ZERO = 1869768058;
        private const int SCALE_COMMAND_SET_GROSS = 1936683623;

        private const int SCALE_COMMAND_STATUS_ONGOING = 1634168417;
        private const int SCALE_COMMAND_STATUS_OK = 1801543519;
        private const int SCALE_COMMAND_STATUS_ERROR_E1 = 826629983;
        private const int SCALE_COMMAND_STATUS_ERROR_E2 = 843407199;
        private const int SCALE_COMMAND_STATUS_ERROR_E3 = 860184415;
        #endregion

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region Constructors
        public WTXJet(INetConnection Connection, EventHandler<ProcessDataReceivedEventArgs> OnProcessData) : base(Connection)
        {
            _connection = Connection;

            ProcessData = new ProcessDataJet(_connection);
            DataStandard = new DataStandardJet(_connection);
            DataFillerExtended = new DataFillerExtendedJet(_connection);

            this.ProcessDataReceived += OnProcessData;

            _connection.IncomingDataReceived += this.OnData;   // Subscribe to the event.              
        }
        #endregion

        #region Connection
        public override void Disconnect(Action<bool> DisconnectCompleted)
        {
            _connection.Disconnect();
        }


        public override void Disconnect()
        {
            _connection.Disconnect();
        }


        public override bool isConnected
        {
            get
            {
                return _connection.IsConnected;
            }
        }

        public override void Connect(double timeoutMs = 20000)
        {
            _connection.Connect();          
        }

        public override void Connect(Action<bool> completed, double timeoutMs)
        {
            _connection.Connect();
        }
        #endregion

        #region Asynchronous process data callback
        public void OnData(object sender, DataEventArgs e)
        {
            this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
        }

        #endregion

        #region Identification
        public override string ConnectionType
        {
            get
            {
                return "Jetbus";
            }
        }
        #endregion

        #region Process data methods

        public override void Zero()
        {
            _connection.Write(_connection.IDCommands.SCALE_COMMAND, SCALE_COMMAND_ZERO);
        }

        public override void SetGross()
        {
            _connection.Write(_connection.IDCommands.SCALE_COMMAND, SCALE_COMMAND_SET_GROSS);
        }

        public override void Tare()
        {
            _connection.Write(_connection.IDCommands.SCALE_COMMAND, SCALE_COMMAND_TARE);
        }


        public override void activateData()
        {
        }

        public override void manualTaring()
        {
        }

        public override void recordWeight()
        {
        }

        #endregion

        #region Process data

        /* 
        *In the following methods the different options for the single integer values are used to define and
        *interpret the value. Finally a string should be returned from the methods to write it onto the GUI Form. 
        */
        public override string CurrentWeight(int value, int decimals)
        {
            double dvalue = value / Math.Pow(10, decimals);
            string returnvalue = "";

            switch (decimals)
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

        public override ApplicationMode ApplicationMode { get; set; }




        public void UpdateApplicationMode(ushort[] Data)
        {
            if ((Data[5] & 0x03) == 0)
                ApplicationMode = ApplicationMode.Standard;
            else
                ApplicationMode = ApplicationMode.Filler;
        }

        public override string Unit
        {
            get
            {
                switch (ProcessData.Unit)
                {
                    case 0x02:
                        return "kg";
                    case 0x4B:
                        return "g";
                    case 0x4C:
                        return "t";
                    case 0XA6:
                        return "lb";
                    default:
                        return "-";
                }
            }
        }

        public override string WeightTypeStringComment()
        {
            if (ProcessData.TareMode == false)
            {
                return "gross";
            }
            else
            {
                return "net";
            }
        }
        public override string ScaleRangeStringComment()
        {
            switch (ProcessData.ScaleRange)
            {
                case 0:
                    return "Range 1";
                case 1:
                    return "Range 2";
                case 2:
                    return "Range 3";
                default:
                    return "error";
            }
        }
        #endregion

        #region Process data methods - Filling
        public override void clearDosingResults()
        {
            throw new NotImplementedException();
        }

        public override void abortDosing()
        {
            throw new NotImplementedException();
        }

        public override void startDosing()
        {
            throw new NotImplementedException();
        }

        public override void manualReDosing()
        {
            throw new NotImplementedException();
        }
        #endregion

        #region Adjustment methods

        /// <summary>
        /// Calculates the values for deadload and nominal load in d from the inputs in mV/V and writes the into the WTX registers.
        /// </summary>
        /// <param name="scaleZeroLoad_mVV"></param>
        /// <param name="scaleCapacity_mVV"></param>
        public override void Calculate(double scaleZeroLoad_mVV, double scaleCapacity_mVV)
        {
            int scalZeroLoad_d;
            int scaleCapacity_d; 

            scalZeroLoad_d = (int) (scaleZeroLoad_mVV * CONVERISION_FACTOR_MVV_TO_D);
            scaleCapacity_d = (int)(scalZeroLoad_d + (scaleCapacity_mVV * CONVERISION_FACTOR_MVV_TO_D));


            // write path 2110/06 - dead load = LDW_DEAD_WEIGHT 

            _connection.Write(_connection.IDCommands.LDW_DEAD_WEIGHT, scalZeroLoad_d);         // Zero point = LDW_DEAD_WEIGHT= "2110/06" 

            // write path 2110/07 - capacity/span = Nominal value = LWT_NOMINAL_VALUE        

            _connection.Write(_connection.IDCommands.LWT_NOMINAL_VALUE, Convert.ToInt32(scaleCapacity_d));    // Nominal value = LWT_NOMINAL_VALUE = "2110/07" ; 

            //this._isCalibrating = true;
        }


        public override void MeasureZero()
        {
            //write "calz" 0x7A6C6163 ( 2053923171 ) to path(ID)=6002/01
            _connection.Write(_connection.IDCommands.SCALE_COMMAND, SCALE_COMMAND_CALIBRATE_ZERO);       // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution
            while (_connection.Read(_connection.IDCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_ONGOING);

            // check : command "ok" = command is done
            while (_connection.Read(_connection.IDCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_OK);
            
        }


        // This method sets the value for the nominal weight in the WTX.
        public override void Calibrate(int calibrationValue, string calibrationWeightStr)
        {
            _connection.Write(_connection.IDCommands.LFT_SCALE_CALIBRATION_WEIGHT, calibrationValue);   // LFT_SCALE_CALIBRATION_WEIGHT = "6152/00" 

            _connection.Write(_connection.IDCommands.SCALE_COMMAND, SCALE_COMMAND_CALIBRATE_NOMINAL);  // CALIBRATE_NOMINAL_WEIGHT = 1852596579 // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution
            while (_connection.Read(_connection.IDCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_ONGOING) ;      // ID_keys.SCALE_COMMAND_STATUS = 6002/02

            // check : command "ok" = command is done
            while (_connection.Read(_connection.IDCommands.SCALE_COMMAND_STATUS) != SCALE_COMMAND_STATUS_OK) ;     

            //this._isCalibrating = true;
        }

        public override void adjustZero()
        {
            throw new NotImplementedException();
        }


        public override void adjustNominal()
        {
            throw new NotImplementedException();
        }

        public void SetOutput(object index, int value)
        {
            _connection.Write(Convert.ToString(index),value);
        }

        public override void StopUpdate()
        {
            _connection.IncomingDataReceived -= this.OnData;   // Subscribe to the event. 
        }

        public override void RestartUpdate()
        {
            _connection.IncomingDataReceived += this.OnData;   // Subscribe to the event. 
        }
        #endregion
    }
}