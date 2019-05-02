// <copyright file="IDataStandard.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    /// <summary>
    /// Interface containing the data for the standard mode of your WTX device.
    /// A class inheriting from interface IDataStandard contains the input word 
    /// and output words for the standard mode of WTX device 120 and 110.
    /// </summary>
    public interface IDataStandard 
    {
        #region Input words for standard mode

        int Input1 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Input2 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Input3 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Input4 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus

        int Output1 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Output2 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Output3 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus
        int Output4 { get; set; } // set : Jetbus only ; get : Modbus and Jetbus

        int LimitStatus1 { get; }
        int LimitStatus2 { get; }
        int LimitStatus3 { get; }
        int LimitStatus4 { get; }

        int WeightMemDay      { get; }
        int WeightMemMonth    { get; }
        int WeightMemYear     { get; }
        int WeightMemSeqNumber{ get; }
        int WeightMemGross    { get; }
        int WeightMemNet      { get; }
        int WeightStorage { get; set; }

        #endregion

        #region Output words for standard mode 

        int ManualTareValue { get;  set; }
        int LimitValue1Input { get;  set; }
        int LimitValue1Mode { get;  set; }

        int LimitValue1ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue1HysteresisBandHeight { get;  set; }
        int LimitValue2Source { get;  set; }
        int LimitValue2Mode { get;  set; }

        int LimitValue2ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue2HysteresisBandHeight { get;  set; }
        int LimitValue3Source { get;  set; }
        int LimitValue3Mode { get;  set; }

        int LimitValue3ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue3HysteresisBandHeight { get;  set; }
        int LimitValue4Source { get;  set; }

        int LimitValue4Mode { get;  set; }
        int LimitValue4ActivationLevelLowerBandLimit { get;  set; }
        int LimitValue4HysteresisBandHeight { get;  set; }

        int CalibrationWeight { get;  set; }
        int ZeroLoad { get;  set; }
        int NominalLoad { get;  set; }

        #endregion

        #region Update methods for the data of standard mode

        void UpdateStandardData(object sender, DataEventArgs e);

        #endregion
    }
}
