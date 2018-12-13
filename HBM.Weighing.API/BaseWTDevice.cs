// <copyright file="BaseWTDevice.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
    public abstract class BaseWtDevice: IDeviceData 
    {

        protected INetConnection connection;
      
        public BaseWtDevice(INetConnection connection)
        {
            this.connection = connection;
        }

        public INetConnection Connection
        {
            get
            {
                return connection;
            }
        }

        public delegate void ProcessDataReceivedEventHandler(object source, ProcessDataReceivedEventArgs args);

        //public abstract event ProcessDataReceivedEventHandler ProcessDataReceived;

        public abstract event ProcessDataReceivedEventHandler ProcessDataReceived;

        public abstract bool isConnected { get; }


        /// <summary>
        /// Synchronous call to connect
        /// </summary>
        /// <param name="timeoutMs">Timeout to wait for connect response</param>
        public abstract void Connect(double timeoutMs);

        /// <summary>
        /// Asynchronous calll to connect
        /// </summary>
        /// <param name="completed">Callback raised after connection completed</param>
        /// <param name="timeoutMs">Timeout to wait for connect response</param>
        public abstract void Connect(Action<bool> completed, double timeoutMs);

        /// <summary>
        /// Method raised whenever new data from device arrives
        /// </summary>
        /// <param name="sender">_asyncData: data array</param>
        public abstract void OnData(ushort[] _asyncData);
               
        public abstract string[] GetDataStr { get;}
        public abstract ushort[] GetDataUshort { get;}

        /// <summary>
        /// Sets the the device to gross values
        /// </summary>
        public abstract void SetGross();

        /// <summary>
        /// Zeroes the device
        /// </summary>
        public abstract void Zero();

        /// <summary>
        /// Tares the device
        /// </summary>
        public abstract void Tare();

        public abstract void adjustZero();
        public abstract void adjustNominal();
        public abstract void activateData();
        public abstract void manualTaring();
        public abstract void recordWeight();
        public abstract void clearDosingResults();
        public abstract void abortDosing();
        public abstract void startDosing();
        public abstract void manualReDosing();


        /// <summary>
        /// Synchronous call to disconnect
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// Asynchronous call to disconnect
        /// </summary
        /// <param name="disconnectCompleted">Callback raised after disconnect completed</param>
        public abstract void Disconnect(Action<bool> disconnectCompleted);

        /// <summary>
        /// Collection with available device data 
        /// </summary>
        public abstract IDeviceData DeviceData { get; }

        /// <summary>
        /// Current weight of device depending on decimal count, gross or net
        /// </summary>
        /// <returns>Current weight of device</returns>
        public abstract string CurrentWeight(int weightNoDecimals, int decimals);


        /// <summary>
        /// Current weight of device, gross or net
        /// </summary>
        /// <returns>Current weight of device</returns>
        public abstract double CurrentWeight();

        /// <summary>
        /// Identify the device type (e.g. "Jetbus" or "Modbus")
        /// </summary>
        public abstract string ConnectionType { get; }

        public abstract string UnitStringComment();

        public abstract void MeasureZero();
        public abstract void Calibrate(int PotencyCalibrationWeight, string calibrationWeight);
        public abstract void Calculate(double _preload, double _capacity);

        public abstract int NetValue { get; }                    // data[1]
        public abstract int GrossValue { get; }                  // data[2]
        public abstract int GeneralWeightError { get; }          // data[3]
        public abstract int ScaleAlarmTriggered { get; }         // data[4]
        public abstract int LimitStatus { get; }                 // data[5]
        public abstract int WeightMoving { get; }                // data[6]
        public abstract int ScaleSealIsOpen { get; }             // data[7]
        public abstract int ManualTare { get; }                  // data[8]
        public abstract int WeightType { get; }                  // data[9]
        public abstract int ScaleRange { get; }                  // data[10]
        public abstract int ZeroRequired { get; }                // data[11]
        public abstract int WeightWithinTheCenterOfZero { get; } // data[12]
        public abstract int WeightInZeroRange { get; }           // data[13]
        public abstract int ApplicationMode { get; }             // data[14]
        public abstract int Decimals { get; }                    // data[15]
        public abstract int Unit { get; }                        // data[16]
        public abstract int Handshake { get; }                   // data[17]
        public abstract int Status { get; }                      // data[18]

        public abstract int Input1 { get; }            // data[19]
        public abstract int Input2 { get; }            // data[20]
        public abstract int Input3 { get; }            // data[21]
        public abstract int Input4 { get; }            // data[22]
        public abstract int Output1 { get; }           // data[23]
        public abstract int Output2 { get; }           // data[24]
        public abstract int Output3 { get; }           // data[25]
        public abstract int Output4 { get; }           // data[26]

        public abstract int LimitStatus1 { get; }       // data[27]
        public abstract int LimitStatus2 { get; }       // data[28]
        public abstract int LimitStatus3 { get; }       // data[29]
        public abstract int LimitStatus4 { get; }       // data[30]

        public abstract int WeightMemDay { get; }          // data[31]
        public abstract int WeightMemMonth { get; }        // data[32]
        public abstract int WeightMemYear { get; }         // data[33]
        public abstract int WeightMemSeqNumber { get; }    // data[34]
        public abstract int WeightMemGross { get; }        // data[35]
        public abstract int WeightMemNet { get; }          // data[36]

        //Filler process data
        public abstract int CoarseFlow { get; }            // data[37]
        public abstract int FineFlow { get; }              // data[38]
        public abstract int Ready { get; }                 // data[39]
        public abstract int ReDosing { get; }              // data[40]
        public abstract int Emptying { get; }              // data[41]        
        public abstract int FlowError { get; }             // data[42]
        public abstract int Alarm { get; }                 // data[43]
        public abstract int AdcOverUnderload { get; }     // data[44]
        public abstract int FillingProcessStatus { get; }    // data[51]
        public abstract int NumberDosingResults { get; }     // data[52]
        public abstract int DosingResult { get; }            // data[53]
        public abstract int MeanValueDosingResults { get; }  // data[54]
        public abstract int StandardDeviation { get; }       // data[55]
        public abstract int TotalWeight { get; }             // data[56]
        public abstract int CurrentDosingTime { get; }        // data[59]
        public abstract int CurrentCoarseFlowTime { get; }    // data[60]
        public abstract int CurrentFineFlowTime { get; }      // data[61]
        public abstract int ToleranceErrorPlus { get; }    // data[47]
        public abstract int ToleranceErrorMinus { get; }   // data[48]


        public abstract int StatusInput1 { get; }          // data[49]
        public abstract int GeneralScaleError { get; }     // data[50]
        public abstract int ManualTareValue { get; set; }
        public abstract int LegalTradeOp { get; }          // data[46]

        #region Limit switch devicde data
        public abstract int LimitValue1Input { get; set; }
        public abstract int LimitValue1Mode { get; set; }
        public abstract int LimitValue1ActivationLevelLowerBandLimit { get; set; }
        public abstract int LimitValue1HysteresisBandHeight { get; set; }
        public abstract int LimitValue2Source { get; set; }
        public abstract int LimitValue2Mode { get; set; }
        public abstract int LimitValue2ActivationLevelLowerBandLimit { get; set; }
        public abstract int LimitValue2HysteresisBandHeight { get; set; }
        public abstract int LimitValue3Source { get; set; }
        public abstract int LimitValue3Mode { get; set; }
        public abstract int LimitValue3ActivationLevelLowerBandLimit { get; set; }
        public abstract int LimitValue3HysteresisBandHeight { get; set; }
        public abstract int LimitValue4Source { get; set; }
        public abstract int LimitValue4Mode { get; set; }
        public abstract int LimitValue4ActivationLevelLowerBandLimit { get; set; }
        public abstract int LimitValue4HysteresisBandHeight { get; set; }
        #endregion

        public abstract int FineFlowCutOffPoint { get; }     // data[57]
        public abstract int CoarseFlowCutOffPoint { get; }   // data[58]
        public abstract int ParameterSetProduct { get; }     // data[62]
        public abstract int MaxDosingTime { get; }         // data[45]

        #region Filler device data
        public abstract int ResidualFlowTime { get; set; }
        public abstract int TargetFillingWeight { get; set; }
        public abstract int CoarseFlowCutOffPointSet { get; set; }
        public abstract int FineFlowCutOffPointSet { get; set; }
        public abstract int MinimumFineFlow { get; set; }
        public abstract int OptimizationOfCutOffPoints { get; set; }
        public abstract int MaximumDosingTime { get; set; }
        public abstract int StartWithFineFlow { get; set; }
        public abstract int CoarseLockoutTime { get; set; }
        public abstract int FineLockoutTime { get; set; }
        public abstract int TareMode { get; set; }
        public abstract int UpperToleranceLimit { get; set; }
        public abstract int LowerToleranceLimit { get; set; }
        public abstract int MinimumStartWeight { get; set; }
        public abstract int EmptyWeight { get; set; }
        public abstract int TareDelay { get; set; }
        public abstract int CoarseFlowMonitoringTime { get; set; }
        public abstract int CoarseFlowMonitoring { get; set; }
        public abstract int FineFlowMonitoring { get; set; }
        public abstract int FineFlowMonitoringTime { get; set; }
        public abstract int DelayTimeAfterFineFlow { get; set; }
        public abstract int ActivationTimeAfterFineFlow { get; set; }
        public abstract int SystematicDifference { get; set; }
        public abstract int DownwardsDosing { get; set; }
        public abstract int ValveControl { get; set; }
        public abstract int EmptyingMode { get; set; }
        #endregion

    }
}

