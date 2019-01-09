
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
        int NetValue // Net value of weight 
        {
            get;
        }

        string NetValueStr
        {
            get;
        }

        int GrossValue // Gross value of weight
        {
            get;
        }

        string GrossValueStr
        {
            get;
        }

        int TareValue // Tare value of weight
        {
            get;
        }

        bool GeneralWeightError
        {
            get;
        }

        bool ScaleAlarmTriggered
        {
            get;
        }

        int LimitStatus
        {
            get;
        }

        bool WeightMoving  // = WeightStable (OPC-UA Standard)
        {
            get;
        }

        bool ScaleSealIsOpen
        {
            get;
        }

        bool ManualTare
        {
            get;
        }

        bool WeightType
        {
            get;
        }

        int ScaleRange // = CurrentRangeId (OPC-UA Standard)
        {
            get;
        }

        bool ZeroRequired
        {
            get;
        }

        bool WeightWithinTheCenterOfZero // = CenterOfZero (OPC-UA Standard)
        {
            get;

        }

        bool WeightInZeroRange // = Inside zero (OPC-UA Standard)
        {
            get;
        }

        int ApplicationMode
        {
            get;
        }

        string ApplicationModeStr
        {
            get;
        }

        int Decimals
        {
            get;
        }

        int Unit
        {
            get;
        }

        bool Handshake
        {
            get;
        }

        bool Status
        {
            get;
        }

        bool Underload // = Underload (OPC-UA Standard)
        {
            get;
        }

        bool Overload // = Overload (OPC-UA Standard)
        {
            get;
        }

        bool weightWithinLimits 
        {
            get;
        }

        bool higherSafeLoadLimit
        {
            get;
        }

        int LegalTradeOp // = LegalForTrade (OPC-UA Standard)
        {
            get;
        }

    }
}