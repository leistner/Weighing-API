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
    #region Enumerations ethernet connection type application mode, limit switches for standard application

    public enum ConnectionType
    {
        Modbus = 0,
        Jetbus = 1
    };

    public enum InputFunction
    {
        Off,
        Tare,
        Trigger,      // Only for IMD1
        reserved,
        BreakFilling, // = BRK ; Stop dosing ; Only for IMD2
        RunFilling,   // = RUN ; Start dosing; Only for IMD2
        Redosing,
        WeightDetection,
        Sum
    }

    public enum OutputFunction
    {
        Off,
        Manually,     // manual with instruction OSx
        LimitSwitch1, // Only for IMD0
        LimitSwitch2, // Only for IMD0
        LimitSwitch3, // Only for IMD0
        LimitSwitch4, // Only for IMD0
        reserved,
        StopMaterial,   //Undefined
        CoarseFlow,   // Only for IMD2
        FineFlow,     // Only for IMD2
        Ready,        // Only for IMD2
        ToleranceExceeded,  // Only for IMD2
        ToleranceUnderrun, // Only for IMD2
        ToleranceExceededUnderrun, // Only for IMD2
        Alert,    // Only for IMD2
        DL1DL2,
        LS1Blinking, 
        LS2Blinking,
        LS3Blinking, 
        LS4Blinking
    }

    public enum ApplicationMode
    {
        Standard = 0,
        Checkweigher = 1,
        Filler = 2
    };

    public enum LimitSwitchesSourceStandard
    {
        AboveLevel  = 0,
        BelowLevel  = 1,
        OutsideBand = 2,
        InsideBand  = 3
    };

    public enum LimitSwitchesModeStandard
    {
        Net = 1,
        Gross = 2
    };

    #endregion

    /// <summary>
    /// Abstract class to declare methods and properties/attributes for its subclasses WtxJet and WtxModbus implementing  BaseWtDevice.
    /// 
    /// By implementing the subclasses of BaseWtDevice (WtxModbus or WtxJet), you can implement your communication to the WTX device either via Jetbus or via Modbus.
    /// The connection establishment and read/write functions via Jet-/or Modbus are given by interface INetConnection (_connection).
    /// Real-Time data is given by the interface IProcessData (_processData).
    /// By function calls in your application you can use all the methods like Tare(), Zero(), Calibrate( PotencyCalibrationWeight, calibrationWeight) via Jetbus or Modbus.
    public abstract class BaseWtDevice
    {
        #region Attributes

        protected INetConnection _connection;
        private IProcessData _processData;
        private IDataStandard _dataStandard;
        private IDataFiller _dataFiller;
        private IDataFillerExtended _dataFillerExtended;

        /// Eventhandler to raise an event and commit the data to the GUI/application from WTXJet and WTXModbus
        public abstract event EventHandler<ProcessDataReceivedEventArgs> ProcessDataReceived;

        public abstract bool isConnected { get; }
        #endregion

        #region constructor of BaseWtDevice
        public BaseWtDevice(INetConnection connection) : base()
        {
            _processData = new ProcessData(connection);

            _dataStandard = new DataStandard(connection);
            _dataFiller = new DataFiller(connection);
            _dataFillerExtended = new DataFillerExtended(connection);

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

        public IDataStandard DataStandard
        {
            get
            {
                return _dataStandard;
            }
        }

        public IDataFiller DataFiller
        {
            get
            {
                return _dataFiller;
            }
        }

        public IDataFillerExtended DataFillerExtended
        {
            get
            {
                return _dataFillerExtended;
            }
        }

        /// <summary>
        /// Sets the application mode according to the integer value in ProcessData : Standard or filler mode
        /// </summary>
        /// <returns></returns>
        public abstract ApplicationMode ApplicationMode { get; }

        /// <summary>
        /// Returns the enumeration of the limit switches for the standard application : Source
        /// </summary>
        public abstract LimitSwitchesSourceStandard LimitSwitchesSourceStandard { get; }

        /// <summary>
        /// Returns the enumeration of the limit switches for the filler application : Mode 
        /// </summary>
        public abstract LimitSwitchesModeStandard LimitSwitchesModeStandard { get; }

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
        /// Synchronous call to disconnect
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Asynchronous call to disconnect
        /// </summary>
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
        /// Gets the unit according to an integer value : g, kg, lb, t 
        /// </summary>
        /// <returns></returns>
        public abstract string Unit { get; }
        
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

        /// <summary>
        /// Stop the data update - Stop timer update
        /// </summary>
        /// <returns></returns>
        public abstract void StopUpdate();

        /// <summary>
        /// Restart the data update - Restart timer update
        /// </summary>
        /// <returns></returns>
        public abstract void RestartUpdate();

        #endregion

    }
}

