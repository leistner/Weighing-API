// <copyright file="ExtendedWtDevice.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using Hbm.Weighing.API;
using Hbm.Weighing.API.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace HBM.Weighing.API
{
    /// <summary>
    /// Class containing extended data and functionality to BaseWtDevice.
    /// Inherited from BaseWtDevice, class WtxJet inherits from ExtendedBaseWtDevice
    /// </summary>
    public abstract class ExtendedWtDevice : BaseWTDevice
    {

        #region =============== constructors & destructors =================

        public ExtendedWtDevice(INetConnection connection, int timerIntervalms) : base(connection, timerIntervalms)
        {
        }

        public ExtendedWtDevice(INetConnection connection) : base(connection)
        {
        }

        #endregion

        #region ======================== properties ========================

        public abstract InputFunction InputIO { get; set; }
        public abstract InputFunction OutputIO { get; set; }

        public abstract int VendorID { get; set; }
        public abstract int ProductCode { get; set; }
        public abstract int SerialNumber { get; set; }

        #endregion

        #region ================ public & internal methods =================

        public abstract void setIOInputFunction(InputFunction IOFunction);
        public abstract void setIOOutputFunction(OutputFunction IOFunction);

        #endregion



    }
}
