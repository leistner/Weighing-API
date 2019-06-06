// <copyright file="ExtendedWtDevice.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API
{
    using Hbm.Weighing.API;

    /// <summary>
    /// Class containing extended data and functionality to BaseWtDevice.
    /// </summary>
    public abstract class ExtendedWTDevice : BaseWTDevice
    {
        #region =============== constructors & destructors =================
        public ExtendedWTDevice(INetConnection connection, int timerIntervalms) : base(connection, timerIntervalms)
        {
        }

        public ExtendedWTDevice(INetConnection connection) : base(connection)
        {
        }
        #endregion


        #region ======================== properties ========================
        public abstract int MaximumPeakValueGross { get; }

        public abstract int MinimumPeakValueGross { get; }

        public abstract int MaximumPeakValue { get; }

        public abstract int MinimumPeakValue { get; }

        public abstract bool GeneralScaleError { get; }

        public abstract int ErrorCode { get; }

        public abstract int VendorID { get; }

        public abstract int ProductCode { get; }

        public abstract int SerialNumber { get; }
        
        public abstract int WeightStep { get; set; }

        public abstract int LocalGravityFactor { get; set; }

        public abstract int DataRate { get; set; }

        public abstract int LowPassFilterMode { get; set; }

        public abstract int LowPassFilterOrder { get; set; }

        public abstract int LowPasCutOffFrequency { get; set; }
        
        public abstract ScaleRangeMode ScaleRangeMode { get; set; }

        public abstract int MultiScaleLimit1 { get; set; }

        public abstract int MultiScaleLimit2 { get; set; }

        public abstract int WeightMovementDetection { get; set; }
                
        public abstract string Identification { get; }

        public abstract string HardwareVersion { get; }

        public abstract string SoftwareVersion { get; }

        public abstract string SoftwareIdentification { get; }

        public abstract string FirmwareDate { get; }

        public abstract InputFunction Input1Function { get; set; }

        public abstract InputFunction Input2Function { get; set; }

        public abstract InputFunction Input3Function { get; set; }

        public abstract InputFunction Input4Function { get; set; }

        public abstract OutputFunction Output1Function { get; set;
        }
        public abstract OutputFunction Output2Function { get; set; }

        public abstract OutputFunction Output3Function { get; set; }

        public abstract OutputFunction Output4Function { get; set; }
        #endregion

        #region ================ public & internal methods =================
        public abstract void SaveAllParameters();

        public abstract void RestoreAllDefaultParameters();
        #endregion



    }
}
