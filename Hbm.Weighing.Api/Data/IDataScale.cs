﻿// <copyright file="IDataScale.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.Api, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.Api.Data
{
    /// <summary>
    /// Class containing extended data and functionality to BaseWtDevice.
    /// It inherits from BaseWtdevice. Used by a Jetbus connection and application.
    /// </summary>
    public interface IDataScale
    {

        #region ======================== properties ========================
        
        /// <summary>
        /// Gets or sets the weight step
        /// </summary>
        int WeightStep { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ScaleRangeMode"/>
        /// </summary>
        ScaleRangeMode ScaleRangeMode { get; set; }

        /// <summary>
        /// Gets or sets the lower limit of a scale with 2 or 3 ranges/intervals
        /// </summary>
        int MultiScaleLimit1 { get; set; }

        /// <summary>
        /// Gets or sets the upper limit of a scale with 3 ranges/intervals
        /// </summary>
        int MultiScaleLimit2 { get; set; }

        /// <summary>
        /// Gets or sets the weight movement detection
        /// </summary>
        int WeightMovementDetection { get; set; }
        #endregion

        #region ================ public & internal methods =================
        #endregion

    }
}
