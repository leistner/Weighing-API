
// <copyright file="IDeviceData.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API
{

    public interface IProcessData
    {
        int NetValue // Gross value of weight 
        {
            get;
            set;
        }

        int GrossValue // Net value of weight
        {
            get;
            set;
        }

        int Tare // Tare value of weight
        {
            get;
            set;
        }

        bool GeneralWeightError
        {
            get;
            set;
        }

        bool ScaleAlarmTriggered
        {
            get;
            set;
        }

        int LimitStatus
        {
            get;
            set;
        }

        bool WeightMoving  // = WeightStable (OPC-UA Standard)
        {
            get;
            set;
        }

        bool ScaleSealIsOpen
        {
            get;
            set;
        }

        bool ManualTare
        {
            get;
            set;
        }

        bool WeightType
        {
            get;
            set;
        }

        int ScaleRange // = CurrentRangeId (OPC-UA Standard)
        {
            get;
            set;
        }

        bool ZeroRequired
        {
            get;
            set;
        }

        bool WeightWithinTheCenterOfZero // = CenterOfZero (OPC-UA Standard)
        {
            get;
            set;
        }

        bool WeightInZeroRange // = Inside zero (OPC-UA Standard)
        {
            get;
            set;
        }

        int ApplicationMode
        {
            get;
            set;
        }

        int Decimals
        {
            get;
            set;
        }

        int Unit
        {
            get;
            set;
        }

        bool Handshake
        {
            get;
            set;
        }

        bool Status
        {
            get;
            set;
        }


        bool Underload // = Underload (OPC-UA Standard)
        {
            get;
            set;
        }

        bool Overload // = Overload (OPC-UA Standard)
        {
            get;
            set;
        }

        bool weightWithinLimits 
        {
            get;
            set;
        }

        bool higherSafeLoadLimit
        {
            get;
            set;
        }

        int LegalTradeOp // = LegalForTrade (OPC-UA Standard)
        {
            get;
            set;
        }

    }
}