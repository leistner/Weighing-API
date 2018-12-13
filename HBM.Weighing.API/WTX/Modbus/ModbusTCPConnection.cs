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
using Modbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Modbus
{

    /// <summary>
    ///     This class establishs the communication to the device(here: WTX120), starts/ends the connection,
    ///     read and write the register and shows the status of the connection and closes the connection to
    ///     the device (here: WTX120).
    ///     Once a button event is clicked in class GUI, an asynchronous call in class WTX120 is started
    ///     and finally in this class "Modbus_TCP" the register (of the device) is read or written.
    ///     The data exchange for reading a register between class "Modbus_TCP" and class "WTX_120" is event-based.
    ///     This class publishes the event (MessageEvent) and read the register, afterwards it will be sent back to WTX120.
    /// </summary>
    public class ModbusTcpConnection : INetConnection 
    {
        private ModbusIpMaster _master;
        private TcpClient _client;

        private bool _connected;     
        private string ipAddress;       
        private ushort _numOfPoints;
        private int _port;
        private ushort _startAdress;

        private ushort[] _data;

        private Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();

        private int command;

        public ModbusTcpConnection(string IpAddress)
        {
            _connected = false;
            _port = 502;
            ipAddress = IpAddress; //IP-address to establish a successful connection to the device

            _numOfPoints = 38;
            _startAdress = 0;
        }
        
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
            set
            {
                this._connected = value;
            }
        }

        // Declaration of the event Eventhandler. For the message information from the register.
        // public event EventHandler<MessageEvent<ushort>> RaiseDataEvent;

        public event EventHandler BusActivityDetection;

        public virtual event EventHandler<ProcessDataReceivedEventArgs> IncomingDataReceived; // virtual new due to tesing - 3.5.2018

        /// <summary>
        /// This method is called from the device class "WTX120" to read the register of the device. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>dataword of the wtx device</returns>
        public int Read(object index)
        {
            try
            {
                _data = _master.ReadHoldingRegisters(this.StartAdress, this.NumOfPoints);

                _connected = true;
                BusActivityDetection?.Invoke(this, new LogEvent("Read successful: Registers have been read"));

                return _data[Convert.ToInt16(index)];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
            }

            return 0;
        }

        public async Task<ushort[]> ReadAsync()
        {
            _data = await _master.ReadHoldingRegistersAsync(_startAdress, _numOfPoints);

            return _data;
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this.command = commandParam;

            await _master.WriteSingleRegisterAsync(index, (ushort)command);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));

            return this.command;
        }

        public void Write(object index, int data)
        {
            this.command = data;
            
            _master.WriteSingleRegister((ushort)Convert.ToInt32(index), (ushort)data);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        public int getCommand
        {
            get { return this.command ; }
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

        public Dictionary<string, int> AllData
        {
            get
            {
                for (int i = 0; i < _data.Length; i++)
                {
                    _dataIntegerBuffer.Add(i.ToString(), Convert.ToInt32(_data[i]));
                }
                return _dataIntegerBuffer;
            }
        }

        public ushort arr1; // For test purpose
        public ushort arr2; // For test purpose

        public void WriteArray(ushort index, ushort[] data)
        {
            this.arr1 = data[0];
            this.arr1 = data[1]; 

            _master.WriteMultipleRegisters(index, data);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort array) have been written successfully to multiple registers"));
        }

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

        // This method closes the connection to the device.
        public void Disconnect()
        {
            _client.Close();

            _connected = false;
            IncomingDataReceived = null;
        }


    }
}