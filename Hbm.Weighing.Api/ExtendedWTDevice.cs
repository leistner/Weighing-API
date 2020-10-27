// <copyright file="ExtendedWtDevice.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.Api, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.Api
{
    using Hbm.Weighing.Api;

    /// <summary>
    /// Class containing extended data and functionality to BaseWtDevice.
    /// It inherits from BaseWtdevice. Used by a Jetbus connection and application.
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
        /// <summary>
        /// Gets the gross maximum peak value
        /// </summary>
        public abstract int MaximumPeakValueGross { get; }

        /// <summary>
        /// Gets the the gross minimum peak value
        /// </summary>
        public abstract int MinimumPeakValueGross { get; }

        /// <summary>
        /// Gets the net maximum peak value
        /// </summary>
        public abstract int MaximumPeakValue { get; }

        /// <summary>
        /// Gets the net minimum peak value
        /// </summary>
        public abstract int MinimumPeakValue { get; }

        /// <summary>
        /// Gets the general scale error status (e.g. if no sensor is connected)
        /// </summary>
        public abstract bool GeneralScaleError { get; }

        /// <summary>
        /// Gets last current code
        /// </summary>
        public abstract int ErrorCode { get; }

        /// <summary>
        /// Gets the vendor id
        /// </summary>
        public abstract int VendorID { get; }

        /// <summary>
        /// Gets the product code 
        /// </summary>
        public abstract int ProductCode { get; }

        /// <summary>
        /// Gets the serial number
        /// </summary>
        public abstract int SerialNumber { get; }

        /// <summary>
        /// Gets or sets the maximum capacity
        /// </summary>
        public abstract int MaximumCapacity { get; set; }

        /// <summary>
        /// Gets or sets the weight step
        /// </summary>
        public abstract int WeightStep { get; set; }

        /// <summary>
        /// Gets or sets the local gravity factor
        /// </summary>
        public abstract int LocalGravityFactor { get; set; }

        /// <summary>
        /// Gets or sets the data rate
        /// </summary>
        public abstract int DataRate { get; set; }

        /// <summary>
        /// Gets or sets the mode of the low-pass filter
        /// </summary>
        public abstract int LowPassFilterMode { get; set; }

        /// <summary>
        /// Gets or sets the order of the low-pass filter
        /// </summary>
        public abstract int LowPassFilterOrder { get; set; }

        /// <summary>
        /// Gets or sets the cut-off frequency of the low-pass filter
        /// </summary>
        public abstract int LowPasCutOffFrequency { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ScaleRangeMode"/>
        /// </summary>
        public abstract ScaleRangeMode ScaleRangeMode { get; set; }

        /// <summary>
        /// Gets or sets the lower limit of a scale with 2 or 3 ranges/intervals
        /// </summary>
        public abstract int MultiScaleLimit1 { get; set; }

        /// <summary>
        /// Gets or sets the upper limit of a scale with 3 ranges/intervals
        /// </summary>
        public abstract int MultiScaleLimit2 { get; set; }

        /// <summary>
        /// Gets or sets the weight movement detection
        /// </summary>
        public abstract int WeightMovementDetection { get; set; }

        /// <summary>
        /// Gets or sets the device identification
        /// </summary>
        public abstract string Identification { get; set; }

        /// <summary>
        /// Gets the hardware version (e.g. WTX120)
        /// </summary>
        public abstract string HardwareVersion { get; }

        /// <summary>
        /// Gets the firmware version
        /// </summary>
        public abstract string SoftwareVersion { get; }

        /// <summary>
        /// Gets the legal-for-trade software identification  
        /// </summary>
        public abstract string SoftwareIdentification { get; }

        /// <summary>
        /// Gets the firmware date
        /// </summary>
        public abstract string FirmwareDate { get; }

        /// <summary>
        /// Gets or sets the input function of input 1
        /// </summary>
        public abstract InputFunction Input1Function { get; set; }

        /// <summary>
        /// Gets or sets the input function of input 2
        /// </summary>
        public abstract InputFunction Input2Function { get; set; }

        /// <summary>
        /// Gets or sets the input function of input 3
        /// </summary>
        public abstract InputFunction Input3Function { get; set; }

        /// <summary>
        /// Gets or sets the input function of input 4
        /// </summary>
        public abstract InputFunction Input4Function { get; set; }

        /// <summary>
        /// Gets or sets the output function of output 1
        /// </summary>
        public abstract OutputFunction Output1Function { get; set; }

        /// <summary>
        /// Gets or sets the output function of output 2
        /// </summary>
        public abstract OutputFunction Output2Function { get; set; }

        /// <summary>
        /// Gets or sets the output function of output 3
        /// </summary>
        public abstract OutputFunction Output3Function { get; set; }

        /// <summary>
        /// Gets or sets the output function of output 4
        /// </summary>
        public abstract OutputFunction Output4Function { get; set; }
        #endregion

        #region ================ public & internal methods =================
        /// <summary>
        /// Saves all parameters in non-volatile memory
        /// </summary>
        public abstract void SaveAllParameters();

        /// <summary>
        /// Restores all parameters from non-volatile memory
        /// </summary>
        public abstract void RestoreAllDefaultParameters();
        #endregion

    }
}
