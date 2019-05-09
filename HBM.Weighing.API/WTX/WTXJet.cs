﻿// <copyright file="WTXJet.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    public class WTXJet : BaseWTDevice
    {
        private ApplicationMode _applicationMode;

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

        private int _manualTareValue;
        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nominalLoad;

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region Constructors
        public WTXJet(INetConnection Connection, EventHandler<ProcessDataReceivedEventArgs> OnProcessData) : base(Connection)
        {
            Connection = Connection;
            
            ProcessData = new ProcessDataJet(Connection);
            DataStandard = new DataStandardJet(Connection);
            DataFillerExtended = new DataFillerExtendedJet(Connection);

            this.ProcessDataReceived += OnProcessData;

            Connection.IncomingDataReceived += this.OnData;   // Subscribe to the event.              
        }
        #endregion

        #region Connection
        public override void Disconnect(Action<bool> DisconnectCompleted)
        {
            Connection.Disconnect();
        }


        public override void Disconnect()
        {
            Connection.Disconnect();
        }


        public override bool IsConnected
        {
            get
            {
                return Connection.IsConnected;
            }
        }

        public override void Connect(double timeoutMs = 20000)
        {
            Connection.Connect();          
        }

        public override void Connect(Action<bool> completed, double timeoutMs)
        {
            Connection.Connect();
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
            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_ZERO);
        }

        public override void SetGross()
        {
            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_SET_GROSS);
        }

        public override void Tare()
        {
            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_TARE);
        }

        public override void TareManually(double manualTareValue)
        {
            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_TARE);
        }

        public override void RecordWeight()
        {
        }

        #endregion

        #region Process data

        /* 
        *In the following methods the different options for the single integer values are used to define and
        *interpret the value. Finally a string should be returned from the methods to write it onto the GUI Form. 
        */
        public override string CurrentWeight
        {
            get
            {
                string returnvalue;
                double dvalue = this.ProcessData.NetValue / Math.Pow(10, this.ProcessData.Decimals);
                switch (this.ProcessData.Decimals)
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

        public override WeightType WeightType
        {
            get
            {
                if (ProcessData.TareMode == false)
                {
                    return WeightType.Gross;
                }
                else
                {
                    return WeightType.Net;
                }
            }
        }
        public override int ScaleRange
        {
            get
            {
                return ProcessData.ScaleRange+1;
            }
        }

        public override int ManualTareValue 
        {
            get { return _manualTareValue; }
            set
            {
                // _connection.Write(this.getIndex(_connection.IDCommands.TARE_VALUE), value); DDD Change to new command!!!
                _manualTareValue = value;
            }
        }
        #endregion
                       
        #region Adjustment methods
        public override int AdjustmentWeight // Type : signed integer 32 Bit
        {
            get { return _calibrationWeight; }
            set
            {
                //_connection.WriteArray(this.getIndex(_connection.IDCommands.LFT_SCALE_CALIBRATION_WEIGHT), value);
                _calibrationWeight = value;
            }
        }
        public override int ZeroSignal // Type : signed integer 32 Bit
        {
            get { return _zeroLoad; }
            set
            {
                //_connection.WriteArray(this.getIndex(_connection.IDCommands.LDW_DEAD_WEIGHT), value);
                _zeroLoad = value;
            }
        }
        public override int NominalSignal // Type : signed integer 32 Bit
        {
            get { return _nominalLoad; }
            set
            {
                //_connection.WriteArray(this.getIndex(_connection.IDCommands.LWT_NOMINAL_VALUE), value);
                _nominalLoad = value;
            }
        }

        /// <summary>
        /// Calculates the values for deadload and nominal load in d from the inputs in mV/V and writes the into the WTX registers.
        /// </summary>
        /// <param name="scaleZeroLoad_mVV"></param>
        /// <param name="scaleCapacity_mVV"></param>
        public override void CalculateAdjustment(double scaleZeroLoad_mVV, double scaleCapacity_mVV)
        {
            int scalZeroLoad_d;
            int scaleCapacity_d; 

            scalZeroLoad_d = (int) (scaleZeroLoad_mVV * CONVERISION_FACTOR_MVV_TO_D);
            scaleCapacity_d = (int)(scalZeroLoad_d + (scaleCapacity_mVV * CONVERISION_FACTOR_MVV_TO_D));


            // write path 2110/06 - dead load = LDW_DEAD_WEIGHT 

            Connection.Write(JetBusCommands.Ldw_dead_weight.PathIndex, scalZeroLoad_d);         // Zero point = LDW_DEAD_WEIGHT= "2110/06" 

            // write path 2110/07 - capacity/span = Nominal value = LWT_NOMINAL_VALUE        

            Connection.Write(JetBusCommands.Lwt_nominal_value.PathIndex, Convert.ToInt32(scaleCapacity_d));    // Nominal value = LWT_NOMINAL_VALUE = "2110/07" ; 

            //this._isCalibrating = true;
        }


        public override void AdjustZeroSignal()
        {
            //write "calz" 0x7A6C6163 ( 2053923171 ) to path(ID)=6002/01
            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_CALIBRATE_ZERO);       // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution
            while (Connection.Read(JetBusCommands.Scale_command_status.PathIndex) != SCALE_COMMAND_STATUS_ONGOING);

            // check : command "ok" = command is done
            while (Connection.Read(JetBusCommands.Scale_command_status.PathIndex) != SCALE_COMMAND_STATUS_OK);
            
        }

        // This method sets the value for the nominal weight in the WTX.
        public override void AdjustNominalSignalWithAdjustmentWeight(int calibrationValue)
        {
            Connection.Write(JetBusCommands.Lft_scale_calibration_weight.PathIndex, calibrationValue);   // LFT_SCALE_CALIBRATION_WEIGHT = "6152/00" 

            Connection.Write(JetBusCommands.Scale_command.PathIndex, SCALE_COMMAND_CALIBRATE_NOMINAL);  // CALIBRATE_NOMINAL_WEIGHT = 1852596579 // SCALE_COMMAND = "6002/01"

            // check : command "on go" = command is in execution
            while (Connection.Read(JetBusCommands.Scale_command_status.PathIndex) != SCALE_COMMAND_STATUS_ONGOING) ;      // ID_keys.SCALE_COMMAND_STATUS = 6002/02

            // check : command "ok" = command is done
            while (Connection.Read(JetBusCommands.Scale_command_status.PathIndex) != SCALE_COMMAND_STATUS_OK) ;     

            //this._isCalibrating = true;
        }

        public override void AdjustNominalSignal()
        {
            throw new NotImplementedException();
        }

        public void SetOutput(object index, int value)
        {
            Connection.Write(Convert.ToString(index),value);
        }

        public override void Stop()
        {
            Connection.IncomingDataReceived -= this.OnData;   // Subscribe to the event. 
        }

        public override void Restart()
        {
            Connection.IncomingDataReceived += this.OnData;   // Subscribe to the event. 
        }
        #endregion
    }
}