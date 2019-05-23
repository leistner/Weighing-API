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
using System.Globalization;
using System.Threading;
using HBM.Weighing.API.Data;
using HBM.Weighing.API.Utils;
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

        #region privates

        private int _manualTareValue;
        private int _calibrationWeight;
        private int _zeroLoad;
        private int _nominalLoad;

        #endregion

        #region Events
        public override event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;
        #endregion

        #region Constructors
        public WTXJet(INetConnection connection, EventHandler<ProcessDataReceivedEventArgs> onProcessData) : base(connection)
        {
            Connection = connection;
            
            ProcessData = new ProcessDataJet(Connection);
            DataStandard = new DataStandardJet(Connection);
            DataFiller = new DataFillerExtendedJet(Connection);
            //DataFillerExtended = new DataFillerExtendedJet(Connection);
            
            this.ProcessDataReceived += onProcessData;

           ((JetBusConnection)Connection).IncomingDataReceived += this.OnData;   // Subscribe to the event.              
        }

        #endregion

        #region property data class

        /// <summary>
        /// Gets and sets the extended filler data 
        /// </summary>
        public IDataFillerExtended DataFillerExtended { get; protected set; }
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
        /// <summary>
        /// Asynchronous callback for process data
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">DataEventArgs containing data dictionary</param>
        public void OnData(object sender, DataEventArgs e)
        {
            this.ProcessDataReceived?.Invoke(this, new ProcessDataReceivedEventArgs(ProcessData));
        }

        #endregion

        #region Identification
        /// <summary>
        /// Gets the the connection type : Jetbus
        /// </summary>
        public override string ConnectionType
        {
            get
            {
                return "Jetbus";
            }
        }
        #endregion

        #region Process data methods

        /// <summary>
        /// Zeros the wtx device
        /// </summary>
        public override void Zero()
        {
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_ZERO);
        }

        /// <summary>
        /// Switches the wtx device to gross value
        /// </summary>
        public override void SetGross()
        {
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_SET_GROSS);
        }

        /// <summary>
        /// Tares the wtx device automatically
        /// </summary>
        public override void Tare()
        {
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_TARE);
        }

        /// <summary>
        /// Tares the wtx device manually 
        /// </summary>
        /// <param name="manualTareValue">manual tare value</param>
        public override void TareManually(double manualTareValue)
        {
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_TARE);
        }

        /// <summary>
        /// Records the weight of the wtx device
        /// </summary>
        public override void RecordWeight()
        {
        }

        #endregion

        #region Process data
        /// <summary>
        /// Gets the weight type containing gross, net and tare value in double
        /// </summary>
        public override WeightType Weight
        {
            get
            {
                return ProcessData.Weight;
            }
        }

        /// <summary>
        /// Gets the printable weight type  containing gross, net and tare value in string
        /// </summary>
        public override PrintableWeightType PrintableWeight
        {
            get
            {
                return ProcessData.PrintableWeight;
            }
        }

        /// <summary>
        /// Gets the application mode containing an enumeration (Standard, Checkweigher, Filler)
        /// </summary>
        public override ApplicationMode ApplicationMode { get; set; }

        /// <summary>
        /// Updates the Application mode
        /// </summary>
        /// <param name="Data">ushort array containing the data from the wtx device registers</param>
        public void UpdateApplicationMode(ushort[] Data)
        {
            if ((Data[5] & 0x03) == 0)
                ApplicationMode = ApplicationMode.Standard;
            else
                ApplicationMode = ApplicationMode.Filler;
        }

        /// <summary>
        /// Gets the unit of the measured value as string
        /// </summary>
        public override string Unit
        {
            get
            {
                return ProcessData.Unit;
            }
        }

        /// <summary>
        /// Gets the tare mode containing None, Tare, PresetTare
        /// </summary>
        public override TareMode TareMode
        {
            get
            {
                return ProcessData.TareMode;
            }
        }
        /// <summary>
        /// Gets the scale range of the wtx device
        /// </summary>
        public override int ScaleRange
        {
            get
            {
                return ProcessData.ScaleRange+1;
            }
        }

        /// <summary>
        /// Gets the manual tare value
        /// </summary>
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

        /// <summary>
        /// Gets & sets the calibration weight value
        /// </summary>
        public override int CalibrationWeight // Type : signed integer 32 Bit
        {
            get { return _calibrationWeight; }
            set
            {
                //_connection.WriteArray(this.getIndex(_connection.IDCommands.LFT_SCALE_CALIBRATION_WEIGHT), value);
                _calibrationWeight = value;
            }
        }
        /// <summary>
        /// Gets & sets the zero value
        /// </summary>
        public override int ZeroSignal // Type : signed integer 32 Bit
        {
            get { return _zeroLoad; }
            set
            {
                //_connection.WriteArray(this.getIndex(_connection.IDCommands.LDW_DEAD_WEIGHT), value);
                _zeroLoad = value;
            }
        }
        /// <summary>
        /// Gets & set the nominal value
        /// </summary>
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

            Connection.Write(JetBusCommands.Ldw_dead_weight, scalZeroLoad_d);         // Zero point = LDW_DEAD_WEIGHT= "2110/06" 

            // write path 2110/07 - capacity/span = Nominal value = LWT_NOMINAL_VALUE        

            Connection.Write(JetBusCommands.Lwt_nominal_value, Convert.ToInt32(scaleCapacity_d));    // Nominal value = LWT_NOMINAL_VALUE = "2110/07" ; 

            //this._isCalibrating = true;
        }

        /// <summary>
        /// Sets a zero value to the wtx device and checks if the command has been set successfully
        /// </summary>
        /// <returns></returns>
        public override bool AdjustZeroSignal()
        {
            //write "calz" 0x7A6C6163 ( 2053923171 ) to path(ID)=6002/01
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_CALIBRATE_ZERO);       // SCALE_COMMAND = "6002/01"

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(200);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Sets a nominal value to the wtx device and checks if the command has been set successfully
        /// </summary>
        /// <returns></returns>
        public override bool AdjustNominalSignal()
        {
            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_CALIBRATE_NOMINAL);

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }
      
            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Adjusts the wtx device with a calibration weight
        /// </summary>
        /// <param name="calibrationWeight"></param>
        /// <returns></returns>
        public override bool AdjustNominalSignalWithCalibrationWeight(double calibrationWeight)
        {
            Connection.Write(JetBusCommands.Lft_scale_calibration_weight, MeasurementUtils.DoubleToDigit(calibrationWeight, ProcessData.Decimals));   

            Connection.Write(JetBusCommands.Scale_command, SCALE_COMMAND_CALIBRATE_NOMINAL); 
  
            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) != SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            while (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_ONGOING)
            {
                Thread.Sleep(100);
            }

            if (Convert.ToInt32(Connection.ReadFromBuffer(JetBusCommands.Scale_command_status)) == SCALE_COMMAND_STATUS_OK)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Stops the data transfer between the classes JetBusConnection and WtxJet
        /// </summary>
        public override void Stop()
        {
            ((JetBusConnection)Connection).IncomingDataReceived -= this.OnData;   // Subscribe to the event. 
        }

        /// <summary>
        /// Restarts the data transfer between the classes JetBusConnection and WtxJet
        /// </summary>
        public override void Restart()
        {
            ((JetBusConnection)Connection).IncomingDataReceived += this.OnData;   // Subscribe to the event. 
        }
        #endregion
    }
}