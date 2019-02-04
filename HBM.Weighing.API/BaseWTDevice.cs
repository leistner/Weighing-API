// <copyright file="BaseWTDevice.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using HBM.Weighing.API.Data;
using HBM.Weighing.API.WTX;
using HBM.Weighing.API.WTX.Jet;
using HBM.Weighing.API.WTX.Modbus;
using System;

namespace HBM.Weighing.API
{
    public enum ApplicationMode
    {
        Standard = 0,
        Checkweigher = 1,
        Filler = 2
    };

    public abstract class BaseWtDevice
    {
        #region Attributes

        protected INetConnection _connection;

        private IProcessData _processData;

        /// Eventhandler to raise an event and commit the data to the GUI/application from WTXJet and WTXModbus
        public abstract event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;

        public abstract bool isConnected { get; }
        #endregion

        #region constructor of BaseWtDevice
        public BaseWtDevice(INetConnection connection) : base()
        {
            _processData = new ProcessData(connection);

            this._connection = connection;
        }
        #endregion

        #region get-properties
        /// <summary>
        /// Get-Property : Interface for the classes JetbusConnection and ModbusTcpconnection
        /// </summary>
        public INetConnection Connection
        {
            get
            {
                return _connection;
            }
        }
        /// <summary>
        /// Get-Property : Class of interface IProcessData containing the real-time data
        /// </summary>
        public IProcessData ProcessData
        {
            get
            {
                return _processData;
            }
        }

        #endregion

        #region Abstract methods for the wtx class(WTXJet or WTXModbus) to get, to send and to analyse data
        /// <summary>
        /// Synchronous call to connect
        /// </summary>
        /// <param name="timeoutMs">Timeout to wait for connect response</param>
        public abstract void Connect(double timeoutMs);

        /// <summary>
        /// Asynchronous calll to connect
        /// </summary>
        /// <param name="completed">Callback raised after connection completed</param>
        /// <param name="timeoutMs">Timeout to wait for connect response</param>
        public abstract void Connect(Action<bool> completed, double timeoutMs);
               
        /// <summary>
        /// Sets the the device to gross values
        /// </summary>
        public abstract void SetGross();

        /// <summary>
        /// Zeroes the device
        /// </summary>
        public abstract void Zero();

        /// <summary>
        /// Tares the device
        /// </summary>
        public abstract void Tare();

        public abstract void adjustZero();

        public abstract void adjustNominal();

        public abstract void activateData();
        public abstract void manualTaring();
        public abstract void recordWeight();
        public abstract void clearDosingResults();
        public abstract void abortDosing();
        public abstract void startDosing();
        public abstract void manualReDosing();

        /// <summary>
        /// Sets the output via Jetbus or Modbus
        /// </summary>
        /// <param name="value">value to be written/set via Jet or Modbus</param>
        /// <param name="index">path, index of the value to be set</param>
        public abstract void SetOutput(object index, int value);

        /// <summary>
        /// Synchronous call to disconnect
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Asynchronous call to disconnect
        /// </summary
        /// <param name="disconnectCompleted">Callback raised after disconnect completed</param>
        public abstract void Disconnect(Action<bool> disconnectCompleted);

        /// <summary>
        /// Current weight of device depending on decimal count, gross or net
        /// </summary>
        /// <returns>Current weight of device</returns>
        public abstract string CurrentWeight(int weightNoDecimals, int decimals);


        /// <summary>
        /// Zeroing the wtx device. 
        /// </summary>
        public abstract void MeasureZero();

        /// <summary>
        /// Calibration with an individual calibration weight. 
        /// </summary>
        /// <param name="PotencyCalibrationWeight"></param>
        /// <param name="calibrationWeight"></param>
        public abstract void Calibrate(int PotencyCalibrationWeight, string calibrationWeight);

        /// <summary>
        /// Calibration with dead load and span. 
        /// </summary>
        /// <param name="_preload"></param>
        /// <param name="_capacity"></param>
        public abstract void Calculate(double _preload, double _capacity);

        /// <summary>
        /// Identify the device type (e.g. "Jetbus" or "Modbus")
        /// </summary>
        public abstract string ConnectionType { get; }

        /// <summary>
        /// Sets the unit according to an integer value : g, kg, lb, t 
        /// </summary>
        /// <returns></returns>
        public abstract string Unit { get; }
        #endregion

        /// <summary>
        /// Sets the status according according to the integer value in ProcessData : F.e. "Execution OK!" or "Execution not OK!"
        /// </summary>
        /// <returns></returns>
        //public abstract string StatusStringComment();

        /// <summary>
        /// Sets the application mode according to the integer value in ProcessData : Standard or filler mode
        /// </summary>
        /// <returns></returns>
        public abstract ApplicationMode ApplicationMode { get; }

        /// <summary>
        /// Sets the weight type according to the integer value in ProcessData: gross or net
        /// </summary>
        /// <returns></returns>
        public abstract string WeightTypeStringComment();

        /// <summary>
        /// Sets the scale range according to the integer value in ProcessData : Range 1, 2, 3
        /// </summary>
        /// <returns></returns>
        public abstract string ScaleRangeStringComment();


    }
}

