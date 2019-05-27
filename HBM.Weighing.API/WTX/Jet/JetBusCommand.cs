// <copyright file="JetBusCommand.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using System;
using System.Collections.Generic;
using System.Text;

namespace Hbm.Weighing.API.WTX.Jet
{
    public class JetBusCommand
    {
        public JetBusCommand(DataType DataTypeParam, string PathIndexParam, int BitIndexParam, int BitLengthParam)
        {
            this.DataType  = DataTypeParam;
            this.PathIndex = PathIndexParam;
            this.BitIndex  = BitIndexParam;
            this.BitLength = BitLengthParam;
        }

        public DataType DataType { get; private set; }

        public string PathIndex { get; private set; }

        public int BitIndex { get; private set; }

        public int BitLength { get; private set; }

        public string ToValue(string input)
        {
            ushort _bitMask = 0;
            ushort _mask = 0;
            string _value;

            try
            {               
                switch (DataType)
                {
                    case DataType.BIT:
                        {
                            switch (BitLength)
                            {
                                case 0: _bitMask = 0xFFFF; break;
                                case 1: _bitMask = 1; break; 
                                case 2: _bitMask = 3; break;
                                case 3: _bitMask = 7; break;
                                default: _bitMask = 1; break;
                            }
                            _mask = (ushort)(_bitMask << BitIndex);
                            _value = ((Convert.ToUInt16(input) & _mask) >> BitIndex).ToString();
                            break;
                        }

                    default:
                        {
                            _value = input;
                            break;
                        }
                }
            }
            catch
            {
                _value = "0";
            }
            return _value;
        }
    }
}
