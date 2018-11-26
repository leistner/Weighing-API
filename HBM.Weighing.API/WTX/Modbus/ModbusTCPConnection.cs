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

        private bool connected;     
        private string ipAddress;       
        private ushort _numOfPoints;
        private int _port;
        private ushort _startAdress;

        private ushort[] _data;

        private Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();

        private int command;

        public ModbusTcpConnection(string IpAddress)
        {
            connected = false;
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
                return this.connected;
            }
            set
            {
                this.connected = value;
            }
        }

        // Declaration of the event Eventhandler. For the message information from the register.
        // public event EventHandler<MessageEvent<ushort>> RaiseDataEvent;

        public event EventHandler BusActivityDetection;

        public virtual event EventHandler<DeviceDataReceivedEventArgs> IncomingDataReceived; // virtual new due to tesing - 3.5.2018


        // This method is called from the device class "WTX120" and calls the method ReadRegisterPublishing(e:MessageEvent)
        // to create a new MessageEvent to read the register of the device. 
        public int Read(object index)
        {
            if (connected)
                ReadRegisterPublishing(new DeviceDataReceivedEventArgs(_data, new string[0]));

            return 0; 
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

        public Dictionary<string, int> getData()
        {

            for(int i=0; i<_data.Length;i++)
            {
                _dataIntegerBuffer.Add(i.ToString(), Convert.ToInt32(_data[i]));
            }
            return _dataIntegerBuffer;
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

        // This method publishes the event (MessageEvent) and read the register, afterwards the message(from the register) will be sent back to WTX120.  
        // This method is declared as a virtual method to allow derived class to override the event call.
        //protected virtual void ReadRegisterPublishing(MessageEvent<ushort> e)

        public virtual void ReadRegisterPublishing(DeviceDataReceivedEventArgs e) // 25.4 Comment : 'virtual' machte hier probleme beim durchlaufen :o 
        {
            // virtual new due to tesing - 3.5.2018
            try
            {
                // Read the data: e.Message's type - ushort[]  
                //e.Args = masterParam.ReadHoldingRegisters(this.StartAdress, this.getNumOfPoints);

                e.ushortArgs = _master.ReadHoldingRegisters(StartAdress, NumOfPoints);
                connected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Read successful: Registers have been read"));
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
            }
            catch (InvalidOperationException)
            {
                BusActivityDetection?.Invoke(this, new LogEvent("Read failed : Registers have not been read"));

                connected = false;

                Connect();
                Thread.Sleep(100);
            }
            
            _data = e.ushortArgs;

            // copy of the event to avoid that a race condition is prevented, if the former subscriber directly logs off after the last
            // condition( and after if(handler!=null) ) and before the event is triggered. 

            //RaiseDataEvent?.Invoke(this, e);

            var handler = IncomingDataReceived;

            //If a subscriber exists: 
            if (handler != null) handler(this, e);
        }

        // This method establishs a connection to the device. Therefore an IP address and the port number
        // for the TcpClient is need. The client itself is used for the implementation of the ModbusIpMaster. 
        public void Connect()
        {
            try
            {
                _client = new TcpClient(ipAddress, _port);
                _master = ModbusIpMaster.CreateIp(_client);
                connected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has been established successfully"));
            }
            catch (Exception)
            {
                connected = false; // If the connection establishment has not been successful - connected=false. 

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has NOT been established successfully"));
            }
        }

        // This method closes the connection to the device.
        public void Disconnect()
        {
            _client.Close();

            connected = false;
            IncomingDataReceived = null;
        }


    }
}