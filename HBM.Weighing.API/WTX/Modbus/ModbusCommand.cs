// <copyright file="ModbusCommand.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using System.Text;

namespace HBM.Weighing.API.WTX.Modbus
{
    public class ModbusCommand
    {
        private string path;

        public ModbusCommand(DataType dataType, string register, IOType io , ApplicationMode app, int bitIndex, int bitLength)
        {
            this.DataType  = dataType;
            this.Register  = register;
            this.IO  = io;
            this.App = app;
            this.BitIndex  = bitIndex;
            this.BitLength = bitLength;

            this.Path = register + dataType + app + io + bitIndex + bitLength;
        }

        public DataType DataType { get; private set; }

        public string Register { get; private set; }

        public IOType IO { get; private set; }

        public ApplicationMode App { get; private set; } 

        public int BitIndex { get; private set; }

        public int BitLength { get; private set; }

        public string Path { get; private set; }
    }
}
