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
        event EventHandler BusActivityDetection;

        event EventHandler<DeviceDataReceivedEventArgs> IncomingDataReceived;

        void Connect();
             
        void Disconnect();

        int Read(object index);       
        void Write(object index, int data);

        Task<ushort[]> ReadAsync();
        Task<int> WriteAsync(ushort index, ushort commandParam);

        void WriteArray(ushort index, ushort[] data);
                
        Dictionary<string, int> getData();

        int NumofPoints     { get; set; }

        bool IsConnected    { get; }

        string IpAddress    { get; set; }        

    }
    
}
