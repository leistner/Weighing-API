// <copyright file="ModbusTCPConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using NModbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using HBM.Weighing.API.WTX.Jet;
using System.Reflection;

namespace HBM.Weighing.API.WTX.Modbus
{
    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishs the communication to your WTX device, starts/ends the connection,
    /// read and write the register and shows the status of the connection and closes the connection to the device.
    /// 
    /// It works by reading registers via Modbus to get the data of the WTX device. By referencing the index in the method Read(index)
    /// it returns a ushort array containing all information about the index.
    /// Once the read method is called, the data is read from the WTX device, put into registers and loaded into a Dictionary containing
    /// pairs of values and keys. The values are shifted and masked. The keys are the indexes(data word number) given by ModbusCommands.
    /// </summary>
    public class ModbusTcpConnection : INetConnection
    {
        #region consts

        const int MODBUS_TCP_DEFAULT_PORT = 502;
        const int WTX_DEFAULT_START_ADDRESS = 0;
        const int WTX_DEFAULT_DATAWORD_COUNT = 38;

        #endregion

        #region privates

        private IModbusMaster _master;
        private TcpClient _client;

        private bool _connected;
        private string ipAddress;
        private ushort _numOfPoints;
        private int _port;
        private ushort _startAdress;

        private ushort[] _data;
        private ushort[] _dataToWrite;

        private Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();
        private Dictionary<ModbusCommand, int> _dataCommandsBuffer = new Dictionary<ModbusCommand, int>();

        private int _dataCommand;

        private ModbusCommands _commands;

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

            _dataToWrite = new ushort[2] { 0, 0 };

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
            get { return this._dataCommand; }
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
        #endregion

        #region Connect/Disconnect methods

        // This method establishs a connection to the device. Therefore an IP address and the port number
        // for the TcpClient is needed. The client itself is used for the implementation of the ModbusIpMaster. 
        public void Connect()
        {
            try
            {
                _client = new TcpClient(ipAddress, _port);

                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_client);

                _connected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has been established successfully"));
            }
            catch (Exception)
            {
                _connected = false; // If the connection establishment has not been successful - connected=false. 

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has NOT been established successfully"));
            }
        }
        public ConnectionType ConnType
        {
            get { return ConnectionType.Modbus; }
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
                _data = _master.ReadHoldingRegisters(0, _startAdress, _numOfPoints);

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
            _data = new ushort[100];

            _data = await _master.ReadHoldingRegistersAsync(0, _startAdress, _numOfPoints);

            this.UpdateDictionary();

            // Update data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

            return _data;
        }

        #endregion

        #region Write methods


        public void Write(ModbusCommand ModbusFrame, int value)
        {
            switch(ModbusFrame.DataType)
            {

                case DataType.U08:
                    _master.WriteSingleRegister(0, (ushort)Convert.ToInt32(ModbusFrame.Register), (ushort)value);
                    break;

                case DataType.Int16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.Register), _dataToWrite);
                    break;
                case DataType.U16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.Register), _dataToWrite);
                    break;

                case DataType.U32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.Register), _dataToWrite);
                    break;
                case DataType.Int32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.Register), _dataToWrite);
                    break;
                case DataType.S32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.Register), _dataToWrite);
                    break;
            }
        }



        public void Write(string index, int data)
        {
            this._dataCommand = data;

            _master.WriteSingleRegister(0, (ushort)Convert.ToInt32(index), (ushort)data);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this._dataCommand = commandParam;

            await _master.WriteSingleRegisterAsync(0, index, (ushort)_dataCommand);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));

            return this._dataCommand;
        }

        public void WriteArray(string index, int value)
        {
            _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
            _dataToWrite[1] = (ushort)(value & 0x0000ffff);

            _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(index), _dataToWrite);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort array) have been written successfully to multiple registers"));
        }

        #endregion

        #region Update dictionary methods, properties

        private void CreateDictionary()
        {
            _dataIntegerBuffer.Clear();

            Type pType = _commands.GetType();
            PropertyInfo[] pInfos = pType.GetProperties();
            foreach (PropertyInfo pInfo in pInfos)
            {
                object propertyValue = pInfo.GetValue(_commands, null);
                if (propertyValue != null)
                {
                    Type propertyValueType = propertyValue.GetType();

                    if (propertyValueType == typeof(ModbusCommand))
                        _dataIntegerBuffer.Add(((ModbusCommand)propertyValue).Path, 0);
                }
                else
                    Console.WriteLine("Prop: {0} ", pInfo.Name);
            }
        }

        private void UpdateDictionary()
        {
            _dataIntegerBuffer[_commands.Net.Path] = _data[1] + (_data[0] << 16);
            _dataIntegerBuffer[_commands.Gross.Path] =  _data[3] + (_data[2] << 16);
            _dataIntegerBuffer[_commands.CiA461WeightStatus.Path] = _data[4];
            _dataIntegerBuffer[_commands.Status_digital_input_1.Path] = _data[6];
            _dataIntegerBuffer[_commands.Status_digital_output_1.Path] = _data[7];
            _dataIntegerBuffer[_commands.Limit_value.Path] = _data[8];
            _dataIntegerBuffer[_commands.Fine_flow_cut_off_point.Path] = _data[20];
            _dataIntegerBuffer[_commands.Coarse_flow_cut_off_point.Path] = _data[22];

            _dataIntegerBuffer[_commands.Application_mode.Path] = _data[5] & 0x1;             // application mode 
            _dataIntegerBuffer[_commands.Decimals.Path] = (_data[5] & 0x70) >> 4;             // decimals
            _dataIntegerBuffer[_commands.Unit.Path] = (_data[5] & 0x180) >> 7;    // unit

            
            _dataIntegerBuffer[_commands.Coarse_flow_monitoring.Path] = _data[8] & 0x1;           //_coarseFlow
            _dataIntegerBuffer[_commands.Fine_flow_monitoring.Path] = ((_data[8] & 0x2) >> 1);  // _fineFlow

            _dataIntegerBuffer[_commands.Ready.Path] = ((_data[8] & 0x4) >> 2);
            _dataIntegerBuffer[_commands.ReDosing.Path] = ((_data[8] & 0x8) >> 3);
            _dataIntegerBuffer[_commands.Emptying_mode.Path] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.Maximal_dosing_time.Path] = ((_data[8] & 0x100) >> 8);
            _dataIntegerBuffer[_commands.Upper_tolerance_limit.Path] = ((_data[8] & 0x400) >> 10);
            _dataIntegerBuffer[_commands.Lower_tolerance_limit.Path] = ((_data[8] & 0x800) >> 11);
            _dataIntegerBuffer[_commands.StatusInput1.Path] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.LegalForTradeOperation.Path] = ((_data[8] & 0x200) >> 9);
            
            _dataIntegerBuffer[_commands.WeightMemDayStandard.Path]         = (_data[9]);
            _dataIntegerBuffer[_commands.WeightMemMonthStandard.Path] = (_data[10]);
            _dataIntegerBuffer[_commands.WeightMemYearStandard.Path]     = (_data[11]);
            _dataIntegerBuffer[_commands.WeightMemSeqNumberStandard.Path] = (_data[12]);
            _dataIntegerBuffer[_commands.WeightMemGrossStandard.Path] = (_data[13]);
            _dataIntegerBuffer[_commands.WeightMemNetStandard.Path]         = (_data[14]);

            _dataIntegerBuffer[_commands.Emptying.Path] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.FlowError.Path] = ((_data[8] & 0x20) >> 5);
            _dataIntegerBuffer[_commands.Alarm.Path] = ((_data[8] & 0x40) >> 6);
            _dataIntegerBuffer[_commands.AdcOverUnderload.Path] = ((_data[8] & 0x80) >> 7);

            _dataIntegerBuffer[_commands.StatusInput1.Path] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.GeneralScaleError.Path] = ((_data[8] & 0x8000) >> 15);
            _dataIntegerBuffer[_commands.TotalWeight.Path] = _data[18];

            // Undefined IDs:
            /*
            _dataIntegerBuffer[IDCommands.DOSING_RESULT]      = _data[12];
            _dataIntegerBuffer[IDCommands.MEAN_VALUE_DOSING_RESULTS] = _data[14];
            _dataIntegerBuffer[IDCommands.STANDARD_DEVIATION] = _data[16];
            _dataIntegerBuffer[IDCommands.CURRENT_DOSING_TIME]        = _data[24];    // _currentDosingTime = _data[24];

            _dataIntegerBuffer[IDCommands.CURRENT_COARSE_FLOW_TIME] = _data[25];      // _currentCoarseFlowTime
            _dataIntegerBuffer[IDCommands.CURRENT_FINE_FLOW_TIME]   = _data[26];      // _currentFineFlowTime
            _dataIntegerBuffer[IDCommands.RANGE_SELECTION_PARAMETER] = _data[27];     // _parameterSetProduct

            _dataIntegerBuffer[IDCommands.] = _fillingProcessStatus = _data[9];  // Undefined
            _dataIntegerBuffer[IDCommands.] = _numberDosingResults = _data[11];        
            */
        }

        public ModbusCommands Commands
        {
            get
            {
                return _commands;
            }
        }

        public Dictionary<string, int> AllData
        {
            get
            {
                return _dataIntegerBuffer;
            }
        }

        public Dictionary<JetBusCommand, int> JetBusData
        {
            get
            {
                return new Dictionary<JetBusCommand, int>();
            }
        }

        #endregion

    }
}

   