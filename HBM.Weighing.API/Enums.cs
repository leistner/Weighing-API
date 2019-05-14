// <copyright file="Enum.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API
{
    public enum ConnectionType
    {
        Modbus = 0,
        Jetbus = 1
    };

    public enum IOType
    {
        Input = 0,
        Output = 1
    };

    public enum DataType
    {
        Int32 = 0,
        U08 = 1,
        U16 = 2,
        Int16 = 3,
        S32 = 4,
        U32 = 5
    };

    public enum InputFunction
    {
        Off,
        Tare,
        Trigger,      // Only for IMD1
        reserved,
        BreakFilling, // = BRK ; Stop dosing ; Only for IMD2
        RunFilling,   // = RUN ; Start dosing; Only for IMD2
        Redosing,
        WeightDetection,
        Sum
    }

    public enum OutputFunction
    {
        Off,
        Manually,     // manual with instruction OSx
        LimitSwitch1, // Only for IMD0
        LimitSwitch2, // Only for IMD0
        LimitSwitch3, // Only for IMD0
        LimitSwitch4, // Only for IMD0
        reserved,
        StopMaterial,   //Undefined
        CoarseFlow,   // Only for IMD2
        FineFlow,     // Only for IMD2
        Ready,        // Only for IMD2
        ToleranceExceeded,  // Only for IMD2
        ToleranceUnderrun, // Only for IMD2
        ToleranceExceededUnderrun, // Only for IMD2
        Alert,    // Only for IMD2
        DL1DL2,
        LS1Blinking,
        LS2Blinking,
        LS3Blinking,
        LS4Blinking
    }

    public enum ApplicationMode
    {
        Standard = 0,
        Checkweigher = 1,
        Filler = 2
    };

    public enum LimitSwitchMode
    {
        AboveLevel = 0,
        BelowLevel = 1,
        OutsideBand = 2,
        InsideBand = 3
    };

    public enum LimitSwitchSource
    {
        Net = 1,
        Gross = 2
    };

    public enum TareMode
    {
        None,
        Tare,
        PresetTare
    };
}