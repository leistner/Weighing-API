// <copyright file="Enums.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    /// <summary>
    /// Identify a connection
    /// </summary>
    public enum ConnectionType
    {
        Modbus = 0,
        Jetbus = 1
    }

    /// <summary>
    /// IO type
    /// </summary>
    public enum IOType
    {
        Input = 0,
        Output = 1
    }

    /// <summary>
    /// Data types for the device data
    /// </summary>
    public enum DataType
    {
        NIL,
        BIT,
        S08,
        U08,
        S16,
        U16,
        S32,
        U32,
        ASCII
    }

    /// <summary>
    /// Digital input functions
    /// </summary>
    public enum InputFunction
    {
        Off,
        Tare,
        Trigger,        // Only ApplicationMode.Checkweigher
        reserved,
        BreakFilling,   // Only ApplicationMode.Filler
        RunFilling,     // Only ApplicationMode.Filler
        Redosing,       // Only ApplicationMode.Filler
        WeightDetection,
        Sum             // Only ApplicationMode.Count
    }

    /// <summary>
    /// Digital output functions
    /// </summary>
    public enum OutputFunction
    {
        Off,
        Manually,     
        LimitSwitch1,               // Only ApplicationMode.Standard
        LimitSwitch2,               // Only ApplicationMode.Standard
        LimitSwitch3,               // Only ApplicationMode.Standard
        LimitSwitch4,               // Only ApplicationMode.Standard
        reserved,
        StopMaterial,               // Only ApplicationMode.Filler
        CoarseFlow,                 // Only ApplicationMode.Filler
        FineFlow,                   // Only ApplicationMode.Filler
        Ready,                      // Only ApplicationMode.Filler
        ToleranceExceeded,          // Only ApplicationMode.Filler
        ToleranceUnderrun,          // Only ApplicationMode.Filler
        ToleranceExceededUnderrun,  // Only ApplicationMode.Filler
        Alert,                      // Only ApplicationMode.Filler
        DL1DL2                      // Only ApplicationMode.Filler
    }

    /// <summary>
    /// Application mode
    /// </summary>
    public enum ApplicationMode
    {
        Standard = 0,
        Checkweigher = 1,
        Filler = 2
    }

    /// <summary>
    /// Modes for limit switches
    /// </summary>
    public enum LimitSwitchMode
    {
        AboveLevel = 0,
        BelowLevel = 1,
        OutsideBand = 2,
        InsideBand = 3
    }

    /// <summary>
    /// Sources for limit switches
    /// </summary>
    public enum LimitSwitchSource
    {
        Net = 1,
        Gross = 2
    }

    /// <summary>
    /// Tare mode
    /// </summary>
    public enum TareMode
    {
        None,
        Tare,
        PresetTare
    }
}