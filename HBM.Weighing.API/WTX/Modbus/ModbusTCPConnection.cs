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
    public class ModbusTCPConnection : INetConnection
    {
        #region consts

        const int MODBUS_TCP_PORT = 502;
        const int WTX_SLAVE_ADDRESS = 0;
        const int WTX_REGISTER_START_ADDRESS = 0;
        const int WTX_REGISTER_DATAWORD_COUNT = 38;

        #endregion

        #region privates

        private IModbusMaster _master;
        private TcpClient _client;
        
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

        public ModbusTCPConnection(string ipAddress)
        {
            IpAddress = ipAddress;            
            CreateDictionary();

            _data = new ushort[38];
            _dataToWrite = new ushort[2];
        }
        #endregion
        
        public bool IsConnected { get; private set; }

        #region Connect/Disconnect methods

        // This method establishs a connection to the device. Therefore an IP address and the port number
        // for the TcpClient is needed. The client itself is used for the implementation of the ModbusIpMaster. 
        public void Connect()
        {
            try
            {
                _client = new TcpClient(IpAddress, MODBUS_TCP_PORT);

                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_client);

                IsConnected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has been established successfully"));
            }
            catch (Exception)
            {
                IsConnected = false; // If the connection establishment has not been successful - connected=false. 

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
            IsConnected = false;
            IncomingDataReceived = null;
        }

        public string IpAddress { get; set; }

        #endregion

        #region Read methods

        /// <summary>
        /// This method is called from the device class "WTX120" to read the register of the device. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>dataword of the wtx device</returns>
        public int ReadSingle(object index)
        {
            int _value = 0;
            try
            {
                _data = _master.ReadHoldingRegisters(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);

                BusActivityDetection?.Invoke(this, new LogEvent("Read successful: 1 Registers has been read"));
                       
                _value =_data[Convert.ToInt16(index)];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
                _value = 0;
            }
            return _value;
        }

        public async Task<ushort[]> ReadAsync()
        {
            _data = new ushort[WTX_REGISTER_DATAWORD_COUNT];
            _data = await _master.ReadHoldingRegistersAsync(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);
            ModbusRegistersToDictionary(_data);

            // Update data in data classes
            this.UpdateDataClasses?.Invoke(this, new EventArgs());

            return _data;
        }

        #endregion

        #region Write methods

        public void Write(string register, DataType dataType, int value)
        {
            ushort _register = Convert.ToUInt16(register);

            switch (dataType)
            {
                case DataType.U08:
                    _master.WriteSingleRegister(WTX_SLAVE_ADDRESS, _register, (ushort)value);
                    break;

                case DataType.S16:
                case DataType.U16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);
                    _master.WriteMultipleRegisters(WTX_SLAVE_ADDRESS, _register, _dataToWrite);
                    break;

                case DataType.U32:
                case DataType.S32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);
                    _master.WriteMultipleRegisters(WTX_SLAVE_ADDRESS, _register, _dataToWrite);
                    break;
            }
            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        // This method writes a data word to the WTX120 device synchronously. 
        public void WriteSync(ushort wordNumber, ushort commandParam)
        {
            int dataWord = 0x00;
            int handshakeBit = 0;

            // (1) Sending of a command:        
            this.Write(Convert.ToString(wordNumber), DataType.U08, commandParam);
            dataWord = this.ReadSingle(5);

            handshakeBit = ((dataWord & 0x4000) >> 14);
            // Handshake protocol as given in the manual:                            

            while (handshakeBit == 0)
            {
                dataWord = this.ReadSingle(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
            }

            // (2) If the handshake bit is equal to 0, the command has to be set to 0x00.
            if (handshakeBit == 1)
            {
                this.Write(Convert.ToString(wordNumber), DataType.U08, 0x00);
            }

            while (handshakeBit == 1) // Before : 'this.status == 1' additionally in the while condition. 
            {
                dataWord = this.ReadSingle(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
            }
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
            _dataIntegerBuffer = new Dictionary<string, int>();
           
            for (int i = 0; i<WTX_REGISTER_DATAWORD_COUNT; i++)
            {
                _dataIntegerBuffer.Add(i.ToString(), 0);
            }
        }

        private void ModbusRegistersToDictionary(ushort[] data)
        {
            for (int i = 0; i<WTX_REGISTER_DATAWORD_COUNT; i++)
            {
                _dataIntegerBuffer[i.ToString()] = _data[i];
            }
        }

        public int GetDataFromDictionary(object command)
        {
            int _register = 0;
            ushort _bitMask = 0;
            ushort _mask = 0;
            int _value = 0;

            try
            {
                ModbusCommand modBusCommand = (ModbusCommand)command;

                Console.WriteLine(modBusCommand.Register);

                _register = Convert.ToInt32(modBusCommand.Register);
                switch (modBusCommand.DataType)
                {
                    case DataType.BIT:
                        switch (modBusCommand.BitLength)
                        {
                            case 0: _bitMask = 0xFFFF; break;
                            case 1: _bitMask = 1; break;
                            case 2: _bitMask = 3; break;
                            case 3: _bitMask = 7; break;
                            default: _bitMask = 1; break;
                        }
                        _mask = (ushort)(_bitMask << modBusCommand.BitIndex);                     

                        _value = (_data[_register] & _mask) >> modBusCommand.BitIndex;
                        break;
         
                    case DataType.U32:
                    case DataType.S32:
                        _value = _data[_register + 1] + (_data[_register] << 16);
                        break;

                    default:
                        _value = _data[_register];
                        break;
                }
            }
            catch
            {
                _value = 0;
            }

            return _value;
        }

        public Dictionary<string, int> AllData
        {
            get
            {
                return _dataIntegerBuffer;
            }
        }

        #endregion

    }
}

   