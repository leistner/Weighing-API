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
using System.Collections.Generic;

namespace HBM.Weighing.API
{

    /// <summary>
    /// Interface containing the process data of your WTX device
    /// </summary>
    public interface IProcessData
    {
        #region Process device data 

        int NetValue { get; } // Net value of weight 
        string NetValueStr { get; }
        int GrossValue { get; } // Gross value of weight
        string GrossValueStr { get; }

        int TareValue { get; } // Tare value of weight
        bool GeneralWeightError { get; }
        bool ScaleAlarmTriggered { get; }
        int LimitStatus { get; }

        bool WeightMoving { get; } // = WeightStable (OPC-UA Standard)
        bool ScaleSealIsOpen { get; }
        bool ManualTare { get; }
        bool WeightType { get; }

        int ScaleRange { get; } // = CurrentRangeId (OPC-UA Standard)
        bool ZeroRequired { get; }
        bool WeightWithinTheCenterOfZero { get; } // = CenterOfZero (OPC-UA Standard)
        bool WeightInZeroRange { get; }// = Inside zero (OPC-UA Standard)
        
        int Decimals { get; }
        int Unit { get; }

        bool Handshake { get; }
        int Status { get; }
        bool Underload{ get; set; } // = Underload (OPC-UA Standard)
        bool Overload { get; set; } // = Overload (OPC-UA Standard)

        bool weightWithinLimits { get; set; }
        bool higherSafeLoadLimit{ get; set; }

        #endregion

        #region Update methods for the process data

        void UpdateProcessDataModbus(object sender, DataEventArgs e);
        void UpdateProcessDataJet(object sender, DataEventArgs e);


        #endregion
    }
}