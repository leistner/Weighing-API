﻿// <copyright file="ModbusTCPConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using Modbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Modbus
{

    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishs the communication to your WTX device, starts/ends the connection,
    /// read and write the register and shows the status of the connection and closes the connection to the device.
    /// </summary>
    public class ModbusTcpConnection : INetConnection 
    {
        #region consts

        const int MODBUS_TCP_DEFAULT_PORT = 502;
        const int WTX_DEFAULT_START_ADDRESS = 0;
        const int WTX_DEFAULT_DATAWORD_COUNT = 38;

        #endregion

        #region privates

        private ModbusIpMaster _master;
        private TcpClient _client;

        private bool _connected;     
        private string ipAddress;       
        private ushort _numOfPoints;
        private int _port;
        private ushort _startAdress;

        private ushort[] _data;
        private ushort[] _dataToWrite;

        private Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();

        private int command;

        private ICommands _commands;

        #endregion

        #region Events
        public event EventHandler BusActivityDetection;
        public event EventHandler<DataEventArgs> UpdateDataClasses;
        public event EventHandler<DataEventArgs> IncomingDataReceived;
        #endregion

        #region Constructor

        public ModbusTcpConnection(string IpAddress)
        {
            _connected = false;
            _port = MODBUS_TCP_DEFAULT_PORT;
            ipAddress = IpAddress; //IP-address to establish a successful connection to the device

            _commands = new ModbusCommands();

            this.CreateDictionary();

            _dataToWrite = new ushort[2]{0,0};

            _numOfPoints = WTX_DEFAULT_DATAWORD_COUNT;
            _startAdress = WTX_DEFAULT_START_ADDRESS;
        }

        #endregion

        #region Get/Set-Properties

        // Getter/Setter for the IP_Adress, StartAdress, NumofPoints, Sending_interval, Port, Is_connected()
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public ushort StartAdress
        {
            get { return _startAdress; }
            set { _startAdress = value; }
        }

        public ushort NumOfPoints
        {
            get { return _numOfPoints; }
            set { _numOfPoints = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool IsConnected
        {
            get
            {
                return this._connected;
            }
        }


        public int getCommand
        {
            get { return this.command; }
        }

        public int NumofPoints
        {
            get
            {
                return this._numOfPoints;
            }
            set
            {
                this._numOfPoints = (ushort)value;
            }
        }

        public ICommands IDCommands
        {
            get { return this._commands; }
        }

        #endregion

        #region Connect/Disconnect methods

        // This method establishs a connection to the device. Therefore an IP address and the port number
        // for the TcpClient is need. The client itself is used for the implementation of the ModbusIpMaster. 
        public void Connect()
        {
            try
            {
                _client = new TcpClient(ipAddress, _port);
                _master = ModbusIpMaster.CreateIp(_client);
                _connected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has been established successfully"));
            }
            catch (Exception)
            {
                _connected = false; // If the connection establishment has not been successful - connected=false. 

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has NOT been established successfully"));
            }
        }

        public string ConnectionType
        {
            get { return "Modbus"; }
        }

        // This method closes the connection to the device.
        public void Disconnect()
        {
            _client.Close();

            _connected = false;
            IncomingDataReceived = null;
        }

        #endregion

        #region Read methods

        /// <summary>
        /// This method is called from the device class "WTX120" to read the register of the device. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>dataword of the wtx device</returns>
        public int Read(object index)
        {
            try
            {
                _data = _master.ReadHoldingRegisters(_startAdress, _numOfPoints);

                BusActivityDetection?.Invoke(this, new LogEvent("Read successful: Registers have been read"));

                this.UpdateDictionary();
                // Updata data in data classes : 
                this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

                return _data[Convert.ToInt16(index)];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
            }

            return _data[Convert.ToInt16(index)];
        }

        public async Task<ushort[]> ReadAsync()
        {
            _data = await _master.ReadHoldingRegistersAsync(_startAdress, _numOfPoints);

            this.UpdateDictionary();

            // Updata data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

            return _data;
        }

        #endregion

        #region Write methods

        public void Write(string index, int data)
        {
            this.command = data;

            _master.WriteSingleRegister((ushort)Convert.ToInt32(index), (ushort)data);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this.command = commandParam;

            await _master.WriteSingleRegisterAsync(index, (ushort)command);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));

            return this.command;
        }

        public void WriteArray(string index, int value)
        {
            _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
            _dataToWrite[1] = (ushort)(value & 0x0000ffff);
 
            _master.WriteMultipleRegisters((ushort) Convert.ToInt32(index), _dataToWrite);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort array) have been written successfully to multiple registers"));
        }

        #endregion

        #region Update dictionary methods, properties

        private void CreateDictionary()
        {
            _dataIntegerBuffer.Add(IDCommands.NET_VALUE, 0);
            _dataIntegerBuffer.Add(IDCommands.GROSS_VALUE, 0);

            _dataIntegerBuffer.Add(IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS, 0);
            _dataIntegerBuffer.Add(IDCommands.UNIT_PREFIX_FIXED_PARAMETER, 0);
           
            _dataIntegerBuffer.Add(IDCommands.FINE_FLOW_CUT_OFF_POINT, 0);
            _dataIntegerBuffer.Add(IDCommands.COARSE_FLOW_CUT_OFF_POINT, 0);
            _dataIntegerBuffer.Add(IDCommands.DECIMALS, 0);
            _dataIntegerBuffer.Add(IDCommands.APPLICATION_MODE, 0);
            _dataIntegerBuffer.Add(IDCommands.SCALE_COMMAND_STATUS, 0);

            _dataIntegerBuffer.Add(IDCommands.COARSE_FLOW_MONITORING, 0);
            _dataIntegerBuffer.Add(IDCommands.FINE_FLOW_MONITORING, 0);
            _dataIntegerBuffer.Add(IDCommands.EMPTYING_MODE, 0);
            _dataIntegerBuffer.Add(IDCommands.MAXIMAL_DOSING_TIME, 0);

            _dataIntegerBuffer.Add(IDCommands.UPPER_TOLERANCE_LIMIT, 0);
            _dataIntegerBuffer.Add(IDCommands.LOWER_TOLERANCE_LIMIT, 0);
                    
            //_dataIntegerBuffer.Add(IDCommands.DOSING_STATE, 0);
            //_dataIntegerBuffer.Add(IDCommands.DOSING_RESULT, 0);

            _dataIntegerBuffer.Add(IDCommands.DOSING_TIME, 0);
            _dataIntegerBuffer.Add(IDCommands.COARSE_FLOW_TIME, 0);
            _dataIntegerBuffer.Add(IDCommands.FINE_FLOW_TIME, 0);
            _dataIntegerBuffer.Add(IDCommands.RANGE_SELECTION_PARAMETER, 0);
         
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_INPUT_1, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_INPUT_2, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_INPUT_3, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_INPUT_4, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_OUTPUT_1, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_OUTPUT_2, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_OUTPUT_3, 0);
            _dataIntegerBuffer.Add(IDCommands.STATUS_DIGITAL_OUTPUT_4, 0);

            _dataIntegerBuffer.Add(IDCommands.LIMIT_VALUE, 0);

            _dataIntegerBuffer.Add(IDCommands.LIMIT_VALUE_MONITORING_LIV11, 0); ;
            _dataIntegerBuffer.Add(IDCommands.SIGNAL_SOURCE_LIV12, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_ON_LEVEL_LIV13, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_OFF_LEVEL_LIV14, 0);

            _dataIntegerBuffer.Add(IDCommands.LIMIT_VALUE_MONITORING_LIV21, 0);
            _dataIntegerBuffer.Add(IDCommands.SIGNAL_SOURCE_LIV22, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_ON_LEVEL_LIV23, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_OFF_LEVEL_LIV24, 0); ;

            _dataIntegerBuffer.Add(IDCommands.LIMIT_VALUE_MONITORING_LIV31, 0);
            _dataIntegerBuffer.Add(IDCommands.SIGNAL_SOURCE_LIV32, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_ON_LEVEL_LIV33, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_OFF_LEVEL_LIV34, 0);

            _dataIntegerBuffer.Add(IDCommands.LIMIT_VALUE_MONITORING_LIV41, 0);
            _dataIntegerBuffer.Add(IDCommands.SIGNAL_SOURCE_LIV42, 0);
            _dataIntegerBuffer.Add(IDCommands.SWITCH_ON_LEVEL_LIV43, 0); ;
            _dataIntegerBuffer.Add(IDCommands.SWITCH_OFF_LEVEL_LIV44, 0);            
        }


        private void UpdateDictionary()
        {
            // Process data : 

                _dataIntegerBuffer[IDCommands.NET_VALUE] = _data[1] + (_data[0] << 16);
                _dataIntegerBuffer[IDCommands.GROSS_VALUE] = _data[3] + (_data[2] << 16);
                _dataIntegerBuffer[IDCommands.WEIGHING_DEVICE_1_WEIGHT_STATUS] = _data[4];
                _dataIntegerBuffer[IDCommands.SCALE_COMMAND_STATUS] = _data[5];                       // status -> Measured value status
                _dataIntegerBuffer[IDCommands.STATUS_DIGITAL_INPUT_1] = _data[6];
                _dataIntegerBuffer[IDCommands.STATUS_DIGITAL_OUTPUT_1] = _data[7];
                _dataIntegerBuffer[IDCommands.LIMIT_VALUE] = _data[8];
                _dataIntegerBuffer[IDCommands.FINE_FLOW_CUT_OFF_POINT] = _data[20];
                _dataIntegerBuffer[IDCommands.COARSE_FLOW_CUT_OFF_POINT] = _data[22];

                _dataIntegerBuffer[IDCommands.APPLICATION_MODE] = _data[5] & 0x1;                      // application mode 
                _dataIntegerBuffer[IDCommands.DECIMALS] = (_data[5] & 0x70) >> 4;                      // decimals
                _dataIntegerBuffer[IDCommands.UNIT_PREFIX_FIXED_PARAMETER] = (_data[5] & 0x180) >> 7;  // unit
            
                _dataIntegerBuffer[IDCommands.COARSE_FLOW_MONITORING] = _data[8] & 0x1;         //_coarseFlow
                _dataIntegerBuffer[IDCommands.FINE_FLOW_MONITORING] = ((_data[8] & 0x2) >> 1);  // _fineFlow

                _dataIntegerBuffer[IDCommands.EMPTYING_MODE]= ((_data[8] & 0x10) >> 4);
                _dataIntegerBuffer[IDCommands.MAXIMAL_DOSING_TIME] = ((_data[8] & 0x100) >> 8);
                _dataIntegerBuffer[IDCommands.UPPER_TOLERANCE_LIMIT] = ((_data[8] & 0x400) >> 10);
                _dataIntegerBuffer[IDCommands.LOWER_TOLERANCE_LIMIT] = ((_data[8] & 0x800) >> 11);
                _dataIntegerBuffer[IDCommands.STATUS_DIGITAL_INPUT_1] = ((_data[8] & 0x4000) >> 14);

                _dataIntegerBuffer[IDCommands.DOSING_RESULT] = _data[12];
                _dataIntegerBuffer[IDCommands.MEAN_VALUE_DOSING_RESULTS] = _data[14];
                _dataIntegerBuffer[IDCommands.STANDARD_DEVIATION] = _data[16];
                _dataIntegerBuffer[IDCommands.DOSING_TIME] = _data[24];                 // _currentDosingTime = _data[24];

                _dataIntegerBuffer[IDCommands.COARSE_FLOW_TIME] = _data[25];            // _currentCoarseFlowTime
                _dataIntegerBuffer[IDCommands.FINE_FLOW_TIME] = _data[26];              // _currentFineFlowTime
                _dataIntegerBuffer[IDCommands.RANGE_SELECTION_PARAMETER] = _data[27];   // _parameterSetProduct

            // Standard data: Missing ID's
            /*
                _weightMemDay = (_data[9]);
                _weightMemMonth = (_data[10]);
                _weightMemYear = (_data[11]);
                _weightMemSeqNumber = (_data[12]);
                _weightMemGross = (_data[13]);
                _weightMemNet = (_data[14]);
           */

            // Filler data: Missing ID's
            /*
                _ready = ((_data[8] & 0x4) >> 2);
                _reDosing = ((_data[8] & 0x8) >> 3)
                _emptying = ((_data[8] & 0x10) >> 4);
                _flowError = ((_data[8] & 0x20) >> 5);
                _alarm = ((_data[8] & 0x40) >> 6);
                _adcOverUnderload = ((_data[8] & 0x80) >> 7);           
                _legalForTradeOperation = ((_data[8] & 0x200) >> 9);
                _statusInput1 = ((_data[8] & 0x4000) >> 14);
                _generalScaleError = ((_data[8] & 0x8000) >> 15);
                _fillingProcessStatus = _data[9];
                _numberDosingResults = _data[11];          
                _totalWeight = _data[18];
            */
        }

        public Dictionary<string, int> AllData
        {
            get
            {

                for(int index = 0; index < _dataIntegerBuffer.Count; index++)
                {
                    var item = _dataIntegerBuffer.ElementAt(index);
                    _dataIntegerBuffer[item.Key] = _data[index];
                }

                return _dataIntegerBuffer;
            }
        }

        #endregion

    }
}