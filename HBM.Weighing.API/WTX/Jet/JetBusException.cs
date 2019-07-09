﻿// <copyright file="JetBusException.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace Hbm.Weighing.API.WTX.Jet
{
    using System;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Class for exception handling
    /// </summary>
    public class JetBusException : Exception
    {
        #region ==================== constants & fields ====================
        private string message;
        #endregion

        /// <summary>
        /// Constructor defining the information of an exception by extracting the token (type JToken)
        /// Information is error code and its message
        /// </summary>
        /// <param name="token"></param>
        #region =============== constructors & destructors =================
        public JetBusException(JToken token)
        {
            ErrorCode = token["error"]["code"].ToObject<int>();
            message = token["error"]["message"].ToString();
        }
        #endregion

        #region ======================== properties ========================
        public int ErrorCode { get; private set; }
        
        public override string Message
        {
            get
            {
                return message + " [ 0x" + ErrorCode.ToString("X") + " ]";
            }
        }
        #endregion
    }
}
