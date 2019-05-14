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
        
        #endregion

        #region Events
        public event EventHandler BusActivityDetection;
        public event EventHandler<EventArgs> UpdateDataClasses;
        public event EventHandler<DataEventArgs> IncomingDataReceived;
        #endregion

        #region Constructor

        public ModbusTcpConnection(string IpAddress)
        {
            _connected = false;
            _port = MODBUS_TCP_DEFAULT_PORT;
            ipAddress = IpAddress; //IP-address to establish a successful connection to the device
            
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
                this.UpdateDataClasses?.Invoke(this, new EventArgs());

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
            this.UpdateDataClasses?.Invoke(this, new EventArgs());

            return _data;
        }

        #endregion

        #region Write methods

        public void Write(string register, DataType dataType, int value)
        {
            switch (dataType)
            {

                case DataType.U08:
                    _master.WriteSingleRegister(0, (ushort)Convert.ToInt32(register), (ushort)value);
                    break;

                case DataType.Int16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(register), _dataToWrite);
                    break;
                case DataType.U16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(register), _dataToWrite);
                    break;

                case DataType.U32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(register), _dataToWrite);
                    break;
                case DataType.Int32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(register), _dataToWrite);
                    break;
                case DataType.S32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(register), _dataToWrite);
                    break;
            }
            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this._dataCommand = commandParam;

            await _master.WriteSingleRegisterAsync(0, index, (ushort)_dataCommand);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));

            return this._dataCommand;
        }

        #endregion

        #region Update dictionary methods, properties

        private void CreateDictionary()
        {
            _dataIntegerBuffer.Clear();

            Type pType = typeof(ModbusCommands); 
            PropertyInfo[] pInfos = pType.GetProperties();
            foreach (PropertyInfo pInfo in pInfos)
            {
                object propertyValue = pInfo.GetValue(typeof(ModbusCommands), null);
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
            this.GetDataFromDictionary(ModbusCommands.Net);
            this.GetDataFromDictionary(ModbusCommands.Gross);

            this.GetDataFromDictionary(ModbusCommands.WeightMoving);
            this.GetDataFromDictionary(ModbusCommands.ScaleSealIsOpen);
            this.GetDataFromDictionary(ModbusCommands.ManualTare);
            this.GetDataFromDictionary(ModbusCommands.Tare_mode);
            this.GetDataFromDictionary(ModbusCommands.ScaleRange);
            this.GetDataFromDictionary(ModbusCommands.ZeroRequired);
            this.GetDataFromDictionary(ModbusCommands.WeightinCenterOfZero);
            this.GetDataFromDictionary(ModbusCommands.WeightinZeroRange);

            this.GetDataFromDictionary(ModbusCommands.Application_mode);     // application mode 
            this.GetDataFromDictionary(ModbusCommands.Decimals);             // decimals
            this.GetDataFromDictionary(ModbusCommands.Unit);                 // unit
            this.GetDataFromDictionary(ModbusCommands.Handshake);            // handshake

            this.GetDataFromDictionary(ModbusCommands.Status_digital_input_1);
            this.GetDataFromDictionary(ModbusCommands.Status_digital_output_1);
            this.GetDataFromDictionary(ModbusCommands.Limit_value);

            this.GetDataFromDictionary(ModbusCommands.Fine_flow_cut_off_point);
            this.GetDataFromDictionary(ModbusCommands.Coarse_flow_cut_off_point);
            this.GetDataFromDictionary(ModbusCommands.Coarse_flow_monitoring);   
            this.GetDataFromDictionary(ModbusCommands.Fine_flow_monitoring);   

            this.GetDataFromDictionary(ModbusCommands.Ready);
            this.GetDataFromDictionary(ModbusCommands.ReDosing);
            
            //this.GetDataFromDictionary(ModbusCommands.Emptying_mode);
            this.GetDataFromDictionary(ModbusCommands.Maximal_dosing_time);
            this.GetDataFromDictionary(ModbusCommands.Upper_tolerance_limit);
            this.GetDataFromDictionary(ModbusCommands.Lower_tolerance_limit);
            this.GetDataFromDictionary(ModbusCommands.StatusInput1);
            this.GetDataFromDictionary(ModbusCommands.LegalForTradeOperation);

            this.GetDataFromDictionary(ModbusCommands.WeightMemDayStandard);
            this.GetDataFromDictionary(ModbusCommands.WeightMemMonthStandard);
            this.GetDataFromDictionary(ModbusCommands.WeightMemYearStandard);
            this.GetDataFromDictionary(ModbusCommands.WeightMemSeqNumberStandard);
            this.GetDataFromDictionary(ModbusCommands.WeightMemGrossStandard);
            this.GetDataFromDictionary(ModbusCommands.WeightMemNetStandard);

            this.GetDataFromDictionary(ModbusCommands.Emptying);
            this.GetDataFromDictionary(ModbusCommands.FlowError);
            this.GetDataFromDictionary(ModbusCommands.Alarm);
            this.GetDataFromDictionary(ModbusCommands.AdcOverUnderload);

            this.GetDataFromDictionary(ModbusCommands.StatusInput1);
            this.GetDataFromDictionary(ModbusCommands.GeneralScaleError);
            this.GetDataFromDictionary(ModbusCommands.TotalWeight);

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
        public int GetDataFromDictionary(object frame)
        {
            int _register = 0;
            ushort _bitMask = 0;
            ushort _mask = 0;

            ModbusCommand ConvertedFrame = frame as ModbusCommand;

            if (ConvertedFrame.DataType == DataType.Int32) // if the register of 'Net measured value'(=0) or 'Gross measured value'(=2)
                _dataIntegerBuffer[ConvertedFrame.Path] = _data[Convert.ToInt16(ConvertedFrame.Register) + 1] + (_data[Convert.ToInt16(ConvertedFrame.Register)] << 16);

            if (ConvertedFrame.DataType != DataType.Int32 && ConvertedFrame.DataType != DataType.S32 && ConvertedFrame.DataType != DataType.U32)
            {
                switch (ConvertedFrame.BitLength)
                {
                    case 0: _bitMask = 0xFFFF; break;
                    case 1: _bitMask = 1; break;
                    case 2: _bitMask = 3; break;
                    case 3: _bitMask = 7; break;

                    default: _bitMask = 1; break;
                }

                _mask = (ushort)(_bitMask << ConvertedFrame.BitIndex);

                _register = Convert.ToInt32(ConvertedFrame.Register);
                _dataIntegerBuffer[ConvertedFrame.Path] = (_data[_register] & _mask) >> ConvertedFrame.BitIndex;
            }

            return _dataIntegerBuffer[ConvertedFrame.Path];
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

   