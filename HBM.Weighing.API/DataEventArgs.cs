// <copyright file="DataEventArgs.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    using System.Collections.Generic;

    /// <summary>
    /// Event to update the data classes (e.g. DataStandard/DataFiller/DataFillerExtended)
    /// </summary>
    public class DataEventArgs
    {
        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="DataEventArgs" /> class
        /// </summary>
        /// <param name="dataDictionaryParam">Dictionary to be injected</param>
        public DataEventArgs(Dictionary<string, string> dataDictionaryParam)
        {
            this.DataDictionary = dataDictionaryParam;
        }
        #endregion

        #region ======================== properties ========================
        /// <summary>
        /// Gets the dictionary with the the data
        /// </summary>
        public Dictionary<string, string> DataDictionary { get; private set; }
        #endregion
    }
}