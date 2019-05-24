// <copyright file="INetConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace Hbm.Weighing.API
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    /// <summary>
    /// Defines the common communication interface
    /// </summary>
    public interface INetConnection
    {
        #region ==================== events & delegates ====================
          
        event EventHandler CommunicationLog;
        
        event EventHandler<EventArgs> UpdateData;
        #endregion

        #region ======================== properties ========================
        string IpAddress { get; set; }
      
        ConnectionType ConnectionType { get; }      
       
        bool IsConnected { get; }
        #endregion

        #region ================ public & internal methods =================

        void Connect(int timeoutMs = 20000);
        
        void Disconnect();

        #endregion

        #region =============================== Read/Write methods ====================================

        int Read(object command);

        void Write(object command, int value);

        Task<ushort[]> ReadAsync(object command);                          

        Task<int> WriteAsync(object command, int value); 
        
        string ReadFromBuffer(object command);

        #endregion

    }

}
