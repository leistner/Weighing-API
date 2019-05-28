﻿// <copyright file="ModbusTCPConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
namespace Hbm.Weighing.API.WTX.Modbus
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Threading;
    using System.Threading.Tasks;
    using NModbus;

    /// <summary>
    /// This class holda a connection via Modbus/TCP,  starts/ends the connection, reads/writes and buffers data.
    /// </summary>
    public class ModbusTCPConnection : INetConnection
    {
        #region ==================== constants & fields ====================
        private const int MODBUS_TCP_PORT = 502;
        private const int WTX_SLAVE_ADDRESS = 0;
        private const int WTX_REGISTER_START_ADDRESS = 0;
        private const int WTX_REGISTER_DATAWORD_COUNT = 38;
        private const int WTX_REGISTER_EXECUTION_COMMANDS = 0;

        private IModbusMaster _master;
        private TcpClient _client;   
        #endregion

        #region ==================== events & delegates ====================
        public event EventHandler CommunicationLog;
        public event EventHandler<EventArgs> UpdateData;
        #endregion
                     
        #region =============== constructors & destructors =================
        public ModbusTCPConnection(string ipAddress)
        {
            IpAddress = ipAddress;            
            CreateDictionary();
        }
        #endregion

        #region ======================== properties ========================
        /// <inheritdoc />
        public bool IsConnected { get; private set; }

        /// <inheritdoc />
        public ConnectionType ConnectionType
        {
            get { return ConnectionType.Modbus; }
        }

        /// <inheritdoc />
        public string IpAddress { get; set; }

        /// <inheritdoc />
        public Dictionary<int, int> AllData { get; private set; } = new Dictionary<int, int>();
        #endregion

        #region ================ public & internal methods =================
        
        /// <summary>
        /// Connect a Modbus/TCP device
        /// </summary>
        public void Connect(int timeoutMs = 20000)
        {
            try
            {
                _client = new TcpClient(IpAddress, MODBUS_TCP_PORT);
                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_client);
                IsConnected = true;
                CommunicationLog?.Invoke(this, new LogEvent("Connection successful"));
            }
            catch (Exception)
            {
                IsConnected = false;
                CommunicationLog?.Invoke(this, new LogEvent("Connection failed"));
            }
        }
     
        /// <summary>
        /// Closes the Modbus/TCP connection
        /// </summary>
        public void Disconnect()
        {
            _client.Close();
            IsConnected = false;
            CommunicationLog?.Invoke(this, new LogEvent("Disconnected"));
        }

        /// <summary>
        /// Synchronizes the Modbus data 
        /// </summary>
        /// <returns>Synchronized Modbus registers</returns>
        public ushort[] SyncData()
        {
            ushort[] _data = ReadModbusRegisters();
            this.UpdateData?.Invoke(this, new EventArgs());
            return _data;
        }

        /// <summary>
        /// Reads a single Modbus/TCP register 
        /// </summary>
        /// <param name="command">Modbus/TCP register index for holding register</param>
        /// <returns>Register content</returns>
        public string Read(object command)
        {
            int _value = 0;
            ModbusCommand _command = (ModbusCommand)command;
            ushort[]_data = ReadModbusRegisters();
            _value = _command.ToValue(_data);
            return _value.ToString();
        }

        /// <summary>
        /// Reads a single Modbus/TCP register 
        /// </summary>
        /// <param name="command">Modbus/TCP register index for holding register</param>
        /// <returns>Register content</returns>
        public int ReadInteger(object command)
        {
            int _value = 0;
            ModbusCommand _command = (ModbusCommand)command;
            ushort[] _data = ReadModbusRegisters();
            _value = _command.ToValue(_data);
            return _value;
        }

        /// <inheritdoc />
        public async Task<string> ReadAsync(object command)
        {
            int _value = 0;
            ModbusCommand _command = (ModbusCommand)command;
            ushort[] _data = await ReadModbusRegistersAsync();
            _value = _command.ToValue(_data);
            this.UpdateData?.Invoke(this, new EventArgs());
            return _value.ToString();
        }

        /// <inheritdoc />
        public bool Write(object command, int value)
        {
            bool result = true;

            ModbusCommand _command = (ModbusCommand)command;
            ushort[] _dataToWrite = new ushort[2];

            switch (_command.DataType)
            {
                case DataType.U32:
                case DataType.S32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);
                    _master.WriteMultipleRegisters(WTX_SLAVE_ADDRESS, _command.Register, _dataToWrite);
                    break;

                case DataType.BIT:
                    _master.WriteSingleRegister(WTX_SLAVE_ADDRESS, _command.Register, (ushort)(value << _command.BitIndex));
                    break;
                case DataType.U08:
                case DataType.S16:
                case DataType.U16:
                default:
                    _master.WriteSingleRegister(WTX_SLAVE_ADDRESS, _command.Register, (ushort)value);
                    break;
            }

            if (_command.Register == WTX_REGISTER_EXECUTION_COMMANDS)
            {
                if (DoHandshake())
                {
                    CommunicationLog?.Invoke(this, new LogEvent("Write register " + _command.Register + " to " + value.ToString() + "successful"));
                }
                else
                {
                    CommunicationLog?.Invoke(this, new LogEvent("Write register " + _command.Register + " to " + value.ToString() + "error"));
                    result = false;
                }
            }
            else
                CommunicationLog?.Invoke(this, new LogEvent("Write register " + _command.Register + " to " + value.ToString()));

            return result;
        }

        /// <inheritdoc />
        public async Task<int> WriteAsync(object command, int value)
        {
            ModbusCommand _command = (ModbusCommand)command;

            ushort registerAddress = (ushort)Convert.ToInt16(_command.Register);

            await _master.WriteSingleRegisterAsync(0, registerAddress, (ushort)(value << _command.BitIndex));

            CommunicationLog?.Invoke(this, new LogEvent("Write register " + _command.Register + " to " + value.ToString()));

            return value;
        }
        #endregion

        #region =============== protected & private methods ================
        /// This method writes a data word to the WTX120 device synchronously. 
        private bool DoHandshake()
        {
            while (ReadInteger(ModbusCommands.Handshake) == 0)
            {
                Thread.Sleep(50);
            }
           _master.WriteSingleRegister(WTX_SLAVE_ADDRESS, WTX_REGISTER_EXECUTION_COMMANDS, 0x00);

            while (ReadInteger(ModbusCommands.Handshake) == 1) 
            {
                Thread.Sleep(50);
            }
            return (ReadInteger(ModbusCommands.Status) == 1);
        }
                
        public string ReadFromBuffer(object command)
        {
            ushort _bitMask = 0;
            ushort _mask = 0;
            string _value = "0";

            try
            {
                ModbusCommand modBusCommand = (ModbusCommand)command;
                    
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

                        _value = ((AllData[modBusCommand.Register] & _mask) >> modBusCommand.BitIndex).ToString();                        
                        break;
         
                    case DataType.U32:
                    case DataType.S32:
                    
                        _value = (AllData[modBusCommand.Register + 1] + (AllData[modBusCommand.Register] << 16)).ToString();
                        break;

                    default:
                        _value = AllData[modBusCommand.Register].ToString();                       
                        break;
                }
            }
            catch
            {
                _value = "0";
            }

            return _value;
        }      

        private void CreateDictionary()
        {
            AllData = new Dictionary<int, int>();           
            for (int i = 0; i<WTX_REGISTER_DATAWORD_COUNT; i++)
            {
                AllData.Add(i, 0);
            }
        }

        private void ModbusRegistersToDictionary(ushort[] data)
        {
            for (int i = 0; i<WTX_REGISTER_DATAWORD_COUNT; i++)
            {
                AllData[i] = data[i];
            }
        }   
        
        private async Task<ushort[]> ReadModbusRegistersAsync()
        {
            ushort[] _data = await _master.ReadHoldingRegistersAsync(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);
            ModbusRegistersToDictionary(_data);
            CommunicationLog?.Invoke(this, new LogEvent("Read all: " + string.Join(",", _data.Select(x => x.ToString("X")).ToArray())));
            return _data;
        }

        private ushort[] ReadModbusRegisters()
        {
            ushort[] _data = _master.ReadHoldingRegisters(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);
            ModbusRegistersToDictionary(_data);
            CommunicationLog?.Invoke(this, new LogEvent("Read all: " + string.Join(",", _data.Select(x => x.ToString("X")).ToArray())));
            return _data;
        }
        #endregion
    }
}

   