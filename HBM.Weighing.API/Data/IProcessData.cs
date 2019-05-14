// <copyright file="IProcessData.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API.Data
{
    /// <summary>
    /// Interface containing the process data of your WTX device.
    /// A class inheriting from interface IProcessData contains the 
    /// input word concerning real-time data of WTX device 120 and 110.
    /// </summary>
    public interface IProcessData
    {
        #region Process device data 
        ApplicationMode ApplicationMode { get; }

        WeightType Weight { get; }

        PrintableWeightType PrintableWeight { get; }

        int NetValue { get; }  
        
        int GrossValue { get; } 

        int TareValue { get; } 

        bool GeneralWeightError { get; }

        bool ScaleAlarm { get; }

        int LimitStatus { get; }

        bool WeightMoving { get; }

        bool ScaleSealIsOpen { get; }

        bool ManualTare { get; }

        bool TareMode { get; }

        int ScaleRange { get; }

        bool ZeroRequired { get; }

        bool CenterOfZero { get; }

        bool InsideZero { get; }
        
        int Decimals { get; }

        int Unit { get; }

        bool Handshake { get; }

        int Status { get; }

        bool Underload { get; }

        bool Overload { get; }

        bool WeightWithinLimits { get; }

        bool HigherSafeLoadLimit{ get; }
        #endregion

        #region Update method
        void UpdateData(object sender, EventArgs e);
        #endregion
    }
}