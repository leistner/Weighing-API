// <copyright file="IDataStandard.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
namespace Hbm.Weighing.API.Data
{
    using System;

    /// <summary>
    /// Interface containing the data for the standard mode of your WTX device.
    /// (e.g. settings for limit switches and digital I/O)
    /// </summary>
    public interface IDataStandard 
    {
        #region ==================== events & delegates ====================
        void UpdateStandardData(object sender, EventArgs e);
        #endregion

        #region ======================== properties ========================
        /// <summary>
        /// Gets a value indicating whether the digital input 1 is active or not
        /// </summary>
        bool Input1 { get; }

        /// <summary>
        /// Gets a value indicating whether the digital input 2 is active or not
        /// </summary>
        bool Input2 { get; }

        /// <summary>
        /// Gets a value indicating whether the digital input 3 is active or not
        /// </summary>
        bool Input3 { get; }

        /// <summary>
        /// Gets a value indicating whether the digital input 4 is active or not
        /// </summary>
        bool Input4 { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the digital output 1 is active or not
        /// </summary>
        bool Output1 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the digital output 2 is active or not
        /// </summary>
        bool Output2 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the digital output 3 is active or not
        /// </summary>
        bool Output3 { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the digital output 4 is active or not
        /// </summary>
        bool Output4 { get; set; }

        /// <summary>
        /// Gets a value indicating whether the limit switch 1 is active or not
        /// </summary>
        bool LimitStatus1 { get; }

        /// <summary>
        /// Gets a value indicating whether the limit switch 2 is active or not
        /// </summary>
        bool LimitStatus2 { get; }

        /// <summary>
        /// Gets a value indicating whether the limit switch 3 is active or not
        /// </summary>
        bool LimitStatus3 { get; }

        /// <summary>
        /// Gets a value indicating whether the limit switch 4 is active or not
        /// </summary>
        bool LimitStatus4 { get; }

        /// <summary>
        /// Gets or sets the source od limit switch 1 (gross or net)
        /// </summary>
        LimitSwitchSource LimitSwitch1Source { get;  set; }

        /// <summary>
        /// Gets or sets the mode of limit switch 1 (e.g. AboveLevel,BelowLevel, OutsideBand or InsideBand)
        /// </summary>
        LimitSwitchMode LimitSwitch1Mode { get;  set; }

        /// <summary>
        /// Gets or sets the level of limit switch 1 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch1Level { get;  set; }

        /// <summary>
        /// Gets or sets the hysteresis of limit switch 1 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch1Hysteresis { get;  set; }

        /// <summary>
        /// Gets or sets the lower band value of limit switch 1 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch1LowerBandValue { get; set; }

        /// <summary>
        /// Gets or sets the band height of limit switch 1 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch1BandHeight { get; set; }

        /// <summary>
        /// Gets or sets the source of limit switch 2 (gross or net)
        /// </summary>
        LimitSwitchSource LimitSwitch2Source { get;  set; }

        /// <summary>
        /// Gets or sets the mode of limit switch 2 (e.g. AboveLevel,BelowLevel, OutsideBand or InsideBand)
        /// </summary>
        LimitSwitchMode LimitSwitch2Mode { get;  set; }

        /// <summary>
        /// Gets or sets the level of limit switch 2 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch2Level { get;  set; }

        /// <summary>
        /// Gets or sets the hysteresis of limit switch 2 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch2Hysteresis { get;  set; }

        /// <summary>
        /// Gets or sets the lower band value of limit switch 2 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch2LowerBandValue { get; set; }

        /// <summary>
        /// Gets or sets the band height of limit switch 2 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch2BandHeight { get; set; }

        /// <summary>
        /// Gets or sets the source of limit switch 3 (gross or net)
        /// </summary>
        LimitSwitchSource LimitSwitch3Source { get;  set; }

        /// <summary>
        /// Gets or sets the mode of limit switch 3 (e.g. AboveLevel,BelowLevel, OutsideBand or InsideBand)
        /// </summary>
        LimitSwitchMode LimitSwitch3Mode { get;  set; }

        /// <summary>
        /// Gets or sets the level of limit switch 3 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch3Level { get;  set; }

        /// <summary>
        /// Gets or sets the hysteresis of limit switch 3 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch3Hysteresis { get;  set; }

        /// <summary>
        /// Gets or sets the lower band value of limit switch 3 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch3LowerBandValue { get; set; }

        /// <summary>
        /// Gets or sets the band height of limit switch 3 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch3BandHeight { get; set; }

        /// <summary>
        /// Gets or sets the source of limit switch 4 (gross or net)
        /// </summary>
        LimitSwitchSource LimitSwitch4Source { get;  set; }

        /// <summary>
        /// Gets or sets the mode of limit switch 4 (e.g. AboveLevel,BelowLevel, OutsideBand or InsideBand)
        /// </summary>
        LimitSwitchMode LimitSwitch4Mode { get;  set; }

        /// <summary>
        /// Gets or sets the level of limit switch 4 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch4Level { get;  set; }

        /// <summary>
        /// Gets or sets the hysteresis of limit switch 4 for modes AboveLevel and BelowLevel
        /// </summary>
        int LimitSwitch4Hysteresis { get;  set; }

        /// <summary>
        /// Gets or sets the lower band value of limit switch 4 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch4LowerBandValue { get; set; }

        /// <summary>
        /// Gets or sets the band height of limit switch 4 for modes OutsideBand and InsideBand
        /// </summary>
        int LimitSwitch4BandHeight { get; set; }

        /// <summary>
        /// Gets the last recording from the weight memory
        /// </summary>
        WeightMemory WeightMemory { get; }
        #endregion
    }
}
