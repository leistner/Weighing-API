// <copyright file="JetBusConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    /// Interface containing the data for the filler extended mode of your WTX device
    /// </summary>
    public interface IDataFillerExtended : IDataFiller
    {
        #region Output words for the data of filler extended mode

        int ResidualFlowTime { get;  set; }
        int TargetFillingWeight { get;  set; }
        int CoarseFlowCutOffPointSet { get;  set; }
        int FineFlowCutOffPointSet { get;  set; }

        int MinimumFineFlow { get;  set; }
        int OptimizationOfCutOffPoints { get;  set; }
        int MaximumDosingTime { get;  set; }
        int StartWithFineFlow { get;  set; }

        int CoarseLockoutTime { get;  set; }
        int FineLockoutTime { get;  set; }
        int TareMode { get;  set; }
        int UpperToleranceLimit { get;  set; }

        int LowerToleranceLimit { get;  set; }
        int MinimumStartWeight { get;  set; }
        int EmptyWeight { get;  set; }
        int TareDelay { get;  set; }

        int CoarseFlowMonitoringTime { get;  set; }
        int CoarseFlowMonitoring { get;  set; }
        int FineFlowMonitoring { get;  set; }
        int FineFlowMonitoringTime { get;  set; }

        int DelayTimeAfterFineFlow { get;  set; }
        int ActivationTimeAfterFineFlow { get;  set; }
        int SystematicDifference { get;  set; }
        int DownwardsDosing { get;  set; }

        int ValveControl { get;  set; }
        int EmptyingMode { get;  set; }

        #endregion 

        #region Update methods for the data of filler extended mode

        void UpdateFillerExtendedDataModbus(ushort[] _dataParam);
        void UpdateFillerExtendedDataJet(Dictionary<string, int> _dataParam);

        #endregion
    }
}
