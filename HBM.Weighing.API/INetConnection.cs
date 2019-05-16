// <copyright file="INetConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using HBM.Weighing.API.WTX;
using HBM.Weighing.API.WTX.Jet;
using HBM.Weighing.API.WTX.Modbus;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    /// <summary>
    /// Defines the common communication interface
    /// </summary>
    public interface INetConnection
    {

        #region Eventhandlers for Log status, data received and update data

        event EventHandler BusActivityDetection;

        event EventHandler<DataEventArgs> IncomingDataReceived;

        event EventHandler<EventArgs> UpdateDataClasses;

        #endregion

        #region Attributes for data, commands, ip address, connection, connection type

        Dictionary<string,int>AllData { get; }

        string IpAddress    { get; set; }                   // ip address establishing a connection to the device

        bool IsConnected    { get; }                        // boolean stating the connection status

        ConnectionType ConnType { get; }                    // enumeration containg Jetbus,Modbus

        #endregion

        #region Connect & Disconnect method

        void Connect();
        
        void Disconnect();

        #endregion

        #region Read/Write methods

        Task<ushort[]> ReadAsync();                              // For reading asynchronously

        Task<int> WriteAsync(ushort index, ushort commandParam); // For writing asynchronously
        
        int GetDataFromDictionary(object command);

        void Write(string register, DataType dataType, int value);
        #endregion

    }

}
