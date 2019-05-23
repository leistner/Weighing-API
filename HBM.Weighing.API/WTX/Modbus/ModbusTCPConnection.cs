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
namespace HBM.Weighing.API.WTX.Modbus
{
    using System;
    using System.Collections.Generic;
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

        private IModbusMaster _master;
        private TcpClient _client;   
        #endregion

        #region ==================== events & delegates ====================
        public event EventHandler CommunicationLog;
        public event EventHandler<EventArgs> UpdateData;
        //public event EventHandler<DataEventArgs> IncomingDataReceived;
        #endregion
                     
        #region =============== constructors & destructors =================
        public ModbusTCPConnection(string ipAddress)
        {
            IpAddress = ipAddress;            
            CreateDictionary();
        }
        #endregion
        
        #region ======================== properties ========================
        public bool IsConnected { get; private set; }

        public ConnectionType ConnectionType
        {
            get { return ConnectionType.Modbus; }
        }

        public string IpAddress { get; set; }

        public Dictionary<int, int> AllData { get; private set; } = new Dictionary<int, int>();

        #endregion

        #region ================ public & internal methods =================
        
        /// <summary>
        /// Connect a Modbus/TCP device
        /// </summary>
        public void Connect()
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
                IsConnected = false; // If the connection establishment has not been successful - connected=false. 

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
        }

        public async Task<ushort[]> SyncData()
        {
            ushort[] _data = new ushort[WTX_REGISTER_DATAWORD_COUNT];
            _data = await _master.ReadHoldingRegistersAsync(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);
            ModbusRegistersToDictionary(_data);

            // Update data in data classes
            this.UpdateData?.Invoke(this, new EventArgs());

            return _data;
        }

        /// <summary>
        /// Reads a single Modbus/TCP register 
        /// </summary>
        /// <param name="index">Modbus/TCP register index for holding register</param>
        /// <returns>Register content</returns>
        public int Read(object index)
        {
            int _value = 0;
            try
            {
                ushort[]_data = _master.ReadHoldingRegisters(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);

                CommunicationLog?.Invoke(this, new LogEvent("Read successful: 1 Registers has been read"));
                       
                _value =_data[Convert.ToInt16(index)];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
                _value = 0;
            }
            return _value;
        }

        public async Task<ushort[]> ReadAsync(object command)
        {
            ushort[]_data = new ushort[WTX_REGISTER_DATAWORD_COUNT];
            _data = await _master.ReadHoldingRegistersAsync(WTX_SLAVE_ADDRESS, WTX_REGISTER_START_ADDRESS, WTX_REGISTER_DATAWORD_COUNT);
            ModbusRegistersToDictionary(_data);

            // Update data in data classes
            this.UpdateData?.Invoke(this, new EventArgs());

            return _data;
        }
            
        public void Write(object command, int value)
        {
            ModbusCommand _command = (ModbusCommand)command;

            ushort[] _dataToWrite = new ushort[2];
            ushort _register = Convert.ToUInt16(_command.Register);

            switch (_command.DataType)
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

            if(_register == 0)
                this.DoHandshake(_register);

            CommunicationLog?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        // This method writes a data word to the WTX120 device synchronously. 
        private void DoHandshake(ushort register)
        {
            int dataWord = this.Read(5);

            int handshakeBit = ((dataWord & 0x4000) >> 14);
            // Handshake protocol as given in the manual:                            

            while (handshakeBit == 0)
            {
                dataWord = this.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
            }

            // (2) If the handshake bit is equal to 0, the command has to be set to 0x00.
            if (handshakeBit == 1)
            {
                _master.WriteSingleRegister(WTX_SLAVE_ADDRESS, 0, 0x00);
            }

            while (handshakeBit == 1) // Before : 'this.status == 1' additionally in the while condition. 
            {
                dataWord = this.Read(5);
                handshakeBit = ((dataWord & 0x4000) >> 14);
            }
        }

        public async Task<int> WriteAsync(object command, int value)
        {
            ModbusCommand _command = (ModbusCommand)command;

            ushort registerAddress = (ushort)Convert.ToInt16(_command.Register);

            await _master.WriteSingleRegisterAsync(0, registerAddress, (ushort)value);

            CommunicationLog?.Invoke(this, new LogEvent("Write register " + _command.Register + " successful"));

            return value;
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
        #endregion

    }
}

   