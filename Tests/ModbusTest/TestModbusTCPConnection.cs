
namespace HBM.Weighing.API.WTX.Modbus
{
    using HBM.Weighing.API;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public enum Behavior
    {

         ConnectionFail, 
         ConnectionSuccess,

         DisconnectionFail,
         DisconnectionSuccess,
         
         ReadFail,
         ReadSuccess,

         WriteFail,
         WriteSuccess,

         WriteSyncFail,
         WriteSyncSuccess,

         WriteArrayFail,
         WriteArraySuccess,

         MeasureZeroFail,
         MeasureZeroSuccess,

         TareFail,
         TareSuccess,

         AsyncWriteBackgroundworkerFail,
         AsyncWriteBackgroundworkerSuccess,

         HandshakeFail,
         HandshakeSuccess,

         CalibrationFail,
         CalibrationSuccess,

         InStandardMode,
         InFillerMode,

         LogEvent_Fail,
         LogEvent_Success,

         t_UnitValue_Fail,
         t_UnitValue_Success,
         kg_UnitValue_Success,
         kg_UnitValue_Fail,
         g_UnitValue_Success,
         g_UnitValue_Fail,
         lb_UnitValue_Success,
         lb_UnitValue_Fail,

         NetGrossValueStringComment_0D_Fail,
         NetGrossValueStringComment_0D_Success,
         NetGrossValueStringComment_1D_Fail,
         NetGrossValueStringComment_1D_Success,
         NetGrossValueStringComment_2D_Fail,
         NetGrossValueStringComment_2D_Success,
         NetGrossValueStringComment_3D_Fail,
         NetGrossValueStringComment_3D_Success,
         NetGrossValueStringComment_4D_Fail,
         NetGrossValueStringComment_4D_Success,
         NetGrossValueStringComment_5D_Fail,
         NetGrossValueStringComment_5D_Success,
         NetGrossValueStringComment_6D_Fail,
         NetGrossValueStringComment_6D_Success,

         ScaleRangeStringComment_Range1_Fail,
         ScaleRangeStringComment_Range1_Success,
         ScaleRangeStringComment_Range2_Fail,
         ScaleRangeStringComment_Range2_Success,
         ScaleRangeStringComment_Range3_Fail,
         ScaleRangeStringComment_Range3_Success,

         LimitStatusStringComment_Case0_Fail,
         LimitStatusStringComment_Case0_Success,
         LimitStatusStringComment_Case1_Fail,
         LimitStatusStringComment_Case1_Success,
         LimitStatusStringComment_Case2_Fail,
         LimitStatusStringComment_Case2_Success,
         LimitStatusStringComment_Case3_Fail,
         LimitStatusStringComment_Case3_Success,

         WeightMovingStringComment_Case0_Fail,
         WeightMovingStringComment_Case0_Success,
         WeightMovingStringComment_Case1_Fail,
         WeightMovingStringComment_Case1_Success,

         WeightTypeStringComment_Case0_Fail,
         WeightTypeStringComment_Case0_Success,
         WeightTypeStringComment_Case1_Fail,
         WeightTypeStringComment_Case1_Success,

         WriteHandshakeTestSuccess,
         WriteHandshakeTestFail,

         GrosMethodTestSuccess,
         GrosMethodTestFail, 

         TareMethodTestSuccess,
         TareMethodTestFail,

         ZeroMethodTestSuccess,
         ZeroMethodTestFail,

         AdjustingZeroMethodSuccess,
         AdjustingZeroMethodFail,

         AdjustNominalMethodTestSuccess,
         AdjustNominalMethodTestFail,

         ActivateDataMethodTestSuccess,
         ActivateDataMethodTestFail,

         ManualTaringMethodTestSuccess,
         ManualTaringMethodTestFail,

         ClearDosingResultsMethodTestSuccess,
         ClearDosingResultsMethodTestFail,

         AbortDosingMethodTestSuccess,
         AbortDosingMethodTestFail,

         StartDosingMethodTestSuccess,
         StartDosingMethodTestFail,
 
         RecordWeightMethodTestSuccess,
         RecordWeightMethodTestFail,

         ManualRedosingMethodTestSuccess,
         ManualRedosingMethodTestFail,

         WriteS32ArrayTestSuccess,
         WriteS32ArrayTestFail,
 
         WriteU16ArrayTestSuccess,
         WriteU16ArrayTestFail,

         ResetTimerTestSuccess,
        //ResetTimerTestFail,

         WriteU08ArrayTestSuccess,
         WriteU08ArrayTestFail,

         UpdateOutputTestSuccess,
         UpdateOutputTestFail,

         WriteLimitSwitch1ModeTestSuccess,
         WriteLimitSwitch1ModeTestFail,
         WriteLimitSwitch2ModeTestSuccess,
         WriteLimitSwitch2ModeTestFail,
         WriteLimitSwitch3ModeTestSuccess,
         WriteLimitSwitch3ModeTestFail,
         WriteLimitSwitch4ModeTestSuccess,
         WriteLimitSwitch4ModeTestFail,
    }

    public class TestModbusTCPConnection : INetConnection, IDisposable
    {
        private Behavior behavior;

        private ushort arrayElement1;
        private ushort arrayElement2;
        private ushort arrayElement3;
        private ushort arrayElement4;

        private bool _connected;

        private ushort[] _dataWTX;

        public int command;
        private ModbusCommands _commands;

        public event EventHandler BusActivityDetection;
        public event EventHandler<DataEventArgs> IncomingDataReceived;
        public event EventHandler<DataEventArgs> UpdateDataClasses;
        
        private Dictionary<string, int> _dataIntegerBuffer;

        private string IP;
        private int interval;
        private int wordNumberIndex; 

        private int numPoints;

        private ushort[] _data;
        private int _index;

        public LogEvent _logObj;

        public TestModbusTCPConnection(Behavior behavior,string ipAddress) 
        {
            _dataWTX = new ushort[38];
            // size of 38 elements for the standard and filler application mode.            

            _commands = new ModbusCommands();

             _dataIntegerBuffer = new Dictionary<string, int>();

            this.CreateDictionary();
           
            this.behavior = behavior;

            this.numPoints = 6;

             _data = new ushort[2];
            _index = 0;

            for (int index = 0; index < _dataWTX.Length; index++)
                _dataWTX[index] = 0x00;

            _dataWTX[0] = 0x00;
            _dataWTX[1] = 0x2710;
            _dataWTX[2] = 0x00;
            _dataWTX[3] = 0x2710;
            _dataWTX[4] = 0x00;
            _dataWTX[5] = 0x00;
        }


        public void Connect()
        {
            switch(this.behavior)
            {
                case Behavior.ConnectionFail:
                    _connected = false;
                    break;

                case Behavior.ConnectionSuccess:
                    _connected = true;
                    break;

                default:
                    _connected = false;
                    break; 
            }

            Write(Convert.ToString(0), 0);
    }

        public bool IsConnected
        {
            get
            {
                return this._connected;
            }
            set
            {
                this._connected = value;
            }
        }

        public void Disconnect()
        {
            switch (this.behavior)
            {
                case Behavior.DisconnectionFail:
                    _connected = true;
                    break;

                case Behavior.DisconnectionSuccess:
                    _connected = false;
                    break;

                default:
                    _connected = true;
                    break;
            }
        }

        private void CreateDictionary()
        {
            _dataIntegerBuffer.Add(_commands.Net.Path, 0);
            _dataIntegerBuffer.Add(_commands.Gross.Path, 0);

            _dataIntegerBuffer.Add(_commands.CiA461WeightStatus.Path, 0);
            _dataIntegerBuffer.Add(_commands.Unit.Path, 0);

            _dataIntegerBuffer.Add(_commands.Fine_flow_cut_off_point.Path, 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_cut_off_point.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Decimals.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Application_mode.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Scale_command_status.Path, 0);

            _dataIntegerBuffer.Add(_commands.Status_digital_input_1.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_input_2.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_input_3.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_input_4.Path, 0);

            _dataIntegerBuffer.Add(_commands.Status_digital_output_1.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_output_2.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_output_3.Path, 0);
            //_dataIntegerBuffer.Add(_commands.Status_digital_output_4.Path, 0);

            _dataIntegerBuffer.Add(_commands.Limit_value.Path, 0);
            /*
            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv11.Path, 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv12.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv13.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv14.Path, 0);

            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv21.Path, 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv22.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv23.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv24.Path, 0);

            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv31.Path, 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv32.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv33.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv34.Path, 0);

            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv41.Path, 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv42.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv43.Path, 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv44.Path, 0);
            */

            _dataIntegerBuffer.Add(_commands.WeightMemDayStandard.Path, 0);
            _dataIntegerBuffer.Add(_commands.WeightMemMonthStandard.Path, 0);
            _dataIntegerBuffer.Add(_commands.WeightMemYearStandard.Path, 0);
            _dataIntegerBuffer.Add(_commands.WeightMemSeqNumberStandard.Path, 0);
            _dataIntegerBuffer.Add(_commands.WeightMemGrossStandard.Path, 0);
            _dataIntegerBuffer.Add(_commands.WeightMemNetStandard.Path, 0);

            _dataIntegerBuffer.Add(_commands.Residual_flow_time.Path, 0);
            _dataIntegerBuffer.Add(_commands.Minimum_fine_flow.Path, 0);
            _dataIntegerBuffer.Add(_commands.Optimization.Path, 0);
            _dataIntegerBuffer.Add(_commands.Tare_mode.Path, 0);
            _dataIntegerBuffer.Add(_commands.Minimum_start_weight.Path, 0);
            _dataIntegerBuffer.Add(_commands.Tare_delay.Path, 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring_time.Path, 0);
            _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring_time.Path, 0);
            _dataIntegerBuffer.Add(_commands.Systematic_difference.Path, 0);
            _dataIntegerBuffer.Add(_commands.Valve_control.Path, 0);
            //_dataIntegerBuffer.Add(_commands.AdcOverUnderload.Path, 0);
            //_dataIntegerBuffer.Add(_commands.LegalForTradeOperation.Path, 0);
            /*
            _dataIntegerBuffer.Add(_commands.StatusInput1.Path, 0);
            _dataIntegerBuffer.Add(_commands.GeneralScaleError.Path, 0);
            _dataIntegerBuffer.Add(_commands.CoarseFlow.Path, 0);
            _dataIntegerBuffer.Add(_commands.FineFlow.Path, 0);
            _dataIntegerBuffer.Add(_commands.Ready.Path, 0);
            _dataIntegerBuffer.Add(_commands.ReDosing.Path, 0);
            _dataIntegerBuffer.Add(_commands.Emptying.Path, 0);
            _dataIntegerBuffer.Add(_commands.FlowError.Path, 0);

            _dataIntegerBuffer.Add(_commands.Alarm.Path, 0);
            _dataIntegerBuffer.Add(_commands.ToleranceErrorPlus.Path, 0);
            _dataIntegerBuffer.Add(_commands.ToleranceErrorMinus.Path, 0);
            _dataIntegerBuffer.Add(_commands.Dosing_time.Path, 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_time.Path, 0);
            _dataIntegerBuffer.Add(_commands.CurrentFineFlowTime.Path, 0);
            _dataIntegerBuffer.Add(_commands.ParameterSetProduct.Path, 0);
            _dataIntegerBuffer.Add(_commands.DownwardsDosing.Path, 0);

            _dataIntegerBuffer.Add(_commands.TotalWeight.Path, 0);
            _dataIntegerBuffer.Add(_commands.TargetFillingWeight.Path, 0);
            _dataIntegerBuffer.Add(_commands.Run_start_dosing.Path, 0);

            _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring.Path, 0);
            _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring.Path, 0);
            _dataIntegerBuffer.Add(_commands.Emptying_mode.Path, 0);
            _dataIntegerBuffer.Add(_commands.Maximal_dosing_time.Path, 0);

            _dataIntegerBuffer.Add(_commands.Upper_tolerance_limit.Path, 0);
            _dataIntegerBuffer.Add(_commands.Lower_tolerance_limit.Path, 0);

            //_dataIntegerBuffer.Add(_commands.IDCommands.RANGE_SELECTION_PARAMETER, 0);

            _dataIntegerBuffer.Add(_commands.Delay_time_after_fine_flow.Path, 0);
            _dataIntegerBuffer.Add(_commands.Activation_time_after_fine_flow.Path, 0);
            */
            // Undefined IDs : 
            /*
            _dataIntegerBuffer.Add(_commands.DOSING_STATE, 0);
            _dataIntegerBuffer.Add(_commands.DOSING_RESULT, 0);
            _dataIntegerBuffer.Add(IDCommands.DELAY1_DOSING, 0);
            _dataIntegerBuffer.Add(IDCommands.STANDARD_DEVIATION, 0);
            _dataIntegerBuffer.Add(IDCommands.EMPTY_WEIGHT_TOLERANCE, 0);
            _dataIntegerBuffer.Add(IDCommands.MEAN_VALUE_DOSING_RESULTS, 0);
            _dataIntegerBuffer.Add(IDCommands.FINE_FLOW_PHASE_BEFORE_COARSE_FLOW, 0);
            */
        }

        private void UpdateDictionary()
        {
            _dataIntegerBuffer[_commands.Net.Path] = _data[1] + (_data[0] << 16);
            _dataIntegerBuffer[_commands.Gross.Path] = _data[3] + (_data[2] << 16);
            _dataIntegerBuffer[_commands.CiA461WeightStatus.Path] = _data[4];
            _dataIntegerBuffer[_commands.Status_digital_input_1.Path] = _data[6];
            _dataIntegerBuffer[_commands.Status_digital_output_1.Path] = _data[7];
            _dataIntegerBuffer[_commands.Limit_value.Path] = _data[8];
            _dataIntegerBuffer[_commands.Fine_flow_cut_off_point.Path] = _data[20];
            _dataIntegerBuffer[_commands.Coarse_flow_cut_off_point.Path] = _data[22];

            _dataIntegerBuffer[_commands.Application_mode.Path] = _data[5] & 0x1;             // application mode 
            _dataIntegerBuffer[_commands.Decimals.Path] = (_data[5] & 0x70) >> 4;     // decimals
            _dataIntegerBuffer[_commands.Unit.Path] = (_data[5] & 0x180) >> 7;    // unit

            _dataIntegerBuffer[_commands.Coarse_flow_monitoring.Path] = _data[8] & 0x1;           //_coarseFlow
            _dataIntegerBuffer[_commands.Fine_flow_monitoring.Path] = ((_data[8] & 0x2) >> 1);  // _fineFlow

            _dataIntegerBuffer[_commands.Ready.Path] = ((_data[8] & 0x4) >> 2);
            _dataIntegerBuffer[_commands.ReDosing.Path] = ((_data[8] & 0x8) >> 3);
            _dataIntegerBuffer[_commands.Emptying_mode.Path] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.Maximal_dosing_time.Path] = ((_data[8] & 0x100) >> 8);
            _dataIntegerBuffer[_commands.Upper_tolerance_limit.Path] = ((_data[8] & 0x400) >> 10);
            _dataIntegerBuffer[_commands.Lower_tolerance_limit.Path] = ((_data[8] & 0x800) >> 11);
            _dataIntegerBuffer[_commands.Status_digital_input_1.Path] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.LegalForTradeOperation.Path] = ((_data[8] & 0x200) >> 9);

            /*
            _dataIntegerBuffer[IDCommands.DOSING_RESULT]      = _data[12];
            _dataIntegerBuffer[IDCommands.MEAN_VALUE_DOSING_RESULTS] = _data[14];
            _dataIntegerBuffer[IDCommands.STANDARD_DEVIATION] = _data[16];
            _dataIntegerBuffer[IDCommands.CURRENT_DOSING_TIME]        = _data[24];    // _currentDosingTime = _data[24];

            _dataIntegerBuffer[IDCommands.CURRENT_COARSE_FLOW_TIME] = _data[25];      // _currentCoarseFlowTime
            _dataIntegerBuffer[IDCommands.CURRENT_FINE_FLOW_TIME]   = _data[26];      // _currentFineFlowTime
            _dataIntegerBuffer[IDCommands.RANGE_SELECTION_PARAMETER] = _data[27];     // _parameterSetProduct
            */

            _dataIntegerBuffer[_commands.WeightMemDayStandard.Path] = (_data[9]);   // = weightMemDay
            _dataIntegerBuffer[_commands.WeightMemMonthStandard.Path] = (_data[10]);  // = weightMemMonth
            _dataIntegerBuffer[_commands.WeightMemYearStandard.Path] = (_data[11]);  // = weightMemYear
            _dataIntegerBuffer[_commands.WeightMemSeqNumberStandard.Path] = (_data[12]);  // = weightMemSeqNumber
            _dataIntegerBuffer[_commands.WeightMemGrossStandard.Path] = (_data[13]);  // = weightMemGross
            _dataIntegerBuffer[_commands.WeightMemNetStandard.Path] = (_data[14]);  // = weightMemNet

            _dataIntegerBuffer[_commands.Emptying.Path] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.FlowError.Path] = ((_data[8] & 0x20) >> 5);
            _dataIntegerBuffer[_commands.Alarm.Path] = ((_data[8] & 0x40) >> 6);
            _dataIntegerBuffer[_commands.AdcOverUnderload.Path] = ((_data[8] & 0x80) >> 7);

            _dataIntegerBuffer[_commands.StatusInput1.Path] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.GeneralScaleError.Path] = ((_data[8] & 0x8000) >> 15);
            _dataIntegerBuffer[_commands.TotalWeight.Path] = _data[18];

            // Filler data: Missing ID's
            /*
            _dataIntegerBuffer[IDCommands.] = _fillingProcessStatus = _data[9];  // Undefined
            _dataIntegerBuffer[IDCommands.] = _numberDosingResults = _data[11];          
            */
        }

        public int Read(object index)
        {
            switch (this.behavior)
            {
                case Behavior.WriteHandshakeTestSuccess:

                    if (_dataWTX[4] == 0x0000)
                        _dataWTX[4] = 0x4000;
                    else
                        if (_dataWTX[4] == 0x4000)
                        _dataWTX[4] = 0x0000;
                    break;

                case Behavior.InFillerMode:

                    //data word for a application mode being in filler mode: Bit .0-1 = 1 || 2 (2 is the given value for filler mode according to the manual, but actually it is 1.)
                    _dataWTX[5] = 0x1;
                    break;

                case Behavior.InStandardMode:

                    //data word for a application mode being in standard mode, not in filler mode: Bit .0-1 = 0
                    _dataWTX[5] = 0x00;

                    break;

                case Behavior.CalibrationFail:

                    //Handshake bit:

                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.CalibrationSuccess:

                    //Handshake bit:

                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.MeasureZeroFail:

                    // Net value in hexadecimal: 
                    _dataWTX[0] = 0x00;
                    _dataWTX[1] = 0x2710;

                    // Gross value in hexadecimal:
                    _dataWTX[2] = 0x00;
                    _dataWTX[3] = 0x2710;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;

                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.MeasureZeroSuccess:

                    // Net value in hexadecimal: 
                    _dataWTX[0] = 0x00;
                    _dataWTX[1] = 0x00;

                    // Gross value in hexadecimal:
                    _dataWTX[2] = 0x00;
                    _dataWTX[3] = 0x00;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.NetGrossValueStringComment_0D_Success:
                    _dataWTX[5] = 0x0000;
                    break;

                case Behavior.NetGrossValueStringComment_1D_Success:
                    _dataWTX[5] = 0x10;
                    break;

                case Behavior.NetGrossValueStringComment_2D_Success:
                    _dataWTX[5] = 0x20;
                    break;

                case Behavior.NetGrossValueStringComment_3D_Success:
                    _dataWTX[5] = 0x30;
                    break;

                case Behavior.NetGrossValueStringComment_4D_Success:
                    _dataWTX[5] = 0x40;
                    break;

                case Behavior.NetGrossValueStringComment_5D_Success:
                    _dataWTX[5] = 0x50;
                    break;

                case Behavior.NetGrossValueStringComment_6D_Success:
                    _dataWTX[5] = 0x60;
                    break;
                    
                case Behavior.WriteSyncSuccess:

                    this.command = 0x100;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.WriteSyncFail:

                    this.command = 0;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;
                    
                case Behavior.WriteFail:

                    this.command = 0;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                default:
                    /*
                    for (int index = 0; index < _dataWTX.Length; index++)
                    {
                        _dataWTX[index] = 0;
                    }
                    _logObj = new LogEvent("Read failed : Registers have not been read");
                    BusActivityDetection?.Invoke(this, _logObj);
                    */
                    break;
            }

            _dataIntegerBuffer["0"] = 1;

            this.UpdateDictionary();
            // Updata data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

            return _dataWTX[Convert.ToInt16(index)];
        }

        public int getCommand
        {
            get { return this.command; }
        }

        public void Write(string index, int data)
        {
            switch (this.behavior)
            {
                case Behavior.UpdateOutputTestSuccess:
                    this.command = 0x800;
                    break;

                case Behavior.UpdateOutputTestFail:
                    this.command = 0x00;
                    break;

                case Behavior.WriteU08ArrayTestSuccess:
                    this.wordNumberIndex = (ushort)Convert.ToUInt16(index);
                    this.arrayElement1 = (ushort)data;
                    break;

                case Behavior.WriteU08ArrayTestFail:
                    this.wordNumberIndex = 0;
                    this.arrayElement1 = 0;
                    break;
                case Behavior.WriteU16ArrayTestSuccess:
                    this.wordNumberIndex = (ushort)Convert.ToUInt16(index);
                    this.arrayElement1 = (ushort)data;
                    break;
                case Behavior.WriteU16ArrayTestFail:
                    this.wordNumberIndex = 0;
                    this.arrayElement1 = 0;
                    break;
                case Behavior.GrosMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.GrosMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.TareMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.TareMethodTestFail:
                    this.command = 0;
                    break;

                case Behavior.AdjustingZeroMethodSuccess:
                    this.command = data;
                    break;
                case Behavior.AdjustingZeroMethodFail:
                    this.command = 0;
                    break;
                case Behavior.AdjustNominalMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.AdjustNominalMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.ActivateDataMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.ActivateDataMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.ManualTaringMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.ManualTaringMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.ClearDosingResultsMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.ClearDosingResultsMethodTestFail:
                    this.command = 0;
                    break;

                case Behavior.StartDosingMethodTestSuccess:
                    command = data;
                    break;
                case Behavior.StartDosingMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.RecordWeightMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.RecordWeightMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.ManualRedosingMethodTestSuccess:
                    this.command = data;
                    break;
                case Behavior.ManualRedosingMethodTestFail:
                    this.command = 0;
                    break;
                case Behavior.WriteHandshakeTestSuccess:

                    if (_dataWTX[4] == 0x0000)
                    {
                        this.command = data;
                        _dataWTX[4] = 0x4000;
                    }
                    else
                    if (_dataWTX[4] == 0x4000)
                    {
                        this.command = 0x0;
                        _dataWTX[4] = 0x0000;
                    }

                    break;

                case Behavior.WriteHandshakeTestFail:
                    _dataWTX[4] = 0x0000;
                    break;

                case Behavior.InFillerMode:
                    //data word for a application mode being in filler mode: Bit .0-1 = 1 || 2 (2 is the given value for filler mode according to the manual, but actually it is 1.)
                    _dataWTX[5] = 0x1;
                    break;

                case Behavior.InStandardMode:
                    //data word for a application mode being in standard mode, not in filler mode: Bit .0-1 = 0
                    _dataWTX[5] = 0x00;
                    break;

                case Behavior.CalibrationFail:
                    this.command = 0;
                    break;

                case Behavior.CalibrationSuccess:
                    this.command = data;
                    break;

                case Behavior.WriteSyncSuccess:

                    this.command = data;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;

                case Behavior.WriteSyncFail:

                    this.command = 0x100;

                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;

                    break;
                    
                case Behavior.WriteFail:
                    this.command = 0x2;
                    break;

                case Behavior.WriteSuccess:
                    command = 0;
                    break;

                case Behavior.HandshakeSuccess:
                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;
                    break;

                case Behavior.HandshakeFail:
                    //Handshake bit:
                    if (_dataWTX[5] >> 14 == 0)
                        _dataWTX[5] = 0x4000;
                    else if (_dataWTX[5] >> 14 == 1)
                        _dataWTX[5] = 0x0000;
                    break;

                case Behavior.WriteLimitSwitch1ModeTestSuccess:
                    this.command = 4; 
                    break;
                case Behavior.WriteLimitSwitch1ModeTestFail:
                    this.command = 0;
                    break;
                case Behavior.WriteLimitSwitch2ModeTestSuccess:
                    this.command = 11;
                    break;
                case Behavior.WriteLimitSwitch2ModeTestFail:
                    this.command = 0;
                    break;
                case Behavior.WriteLimitSwitch3ModeTestSuccess:
                    this.command = 17;
                    break;
                case Behavior.WriteLimitSwitch3ModeTestFail:
                    this.command = 0;
                    break;
                case Behavior.WriteLimitSwitch4ModeTestSuccess:
                    this.command = 23;
                    break;
                case Behavior.WriteLimitSwitch4ModeTestFail:
                    this.command = 0;
                    break;
            }

        }

        public void WriteArray(string index, int value)
        {
            _data[0] = (ushort)((value & 0xffff0000) >> 16);
            _data[1] = (ushort)(value & 0x0000ffff);

            _index = Convert.ToInt16(index);

            switch (this.behavior)
            {
                case Behavior.UpdateOutputTestSuccess:
                        this.arrayElement1 = _data[0];
                        this.arrayElement2 = _data[1];
                    break;

                case Behavior.UpdateOutputTestFail:
                    this.arrayElement1 = 0;
                    this.arrayElement2 = 0;
                    break;

                case Behavior.WriteS32ArrayTestSuccess:
                        this.wordNumberIndex = _index;
                        this.arrayElement1 = _data[0];
                        this.arrayElement2 = _data[1];
                    break;

                case Behavior.WriteS32ArrayTestFail:
                        this.wordNumberIndex = 0;
                        this.arrayElement1 = 0;
                        this.arrayElement2 = 0;
                    break;

                case Behavior.CalibrationFail:
                        this.arrayElement1 = 0;
                        this.arrayElement2 = 0;
                   break;

                case Behavior.CalibrationSuccess:

                    if ((int)_index == 48 || (int)_index == 46)       // According to the index 48 (=wordnumber) the preload is written. 
                    {
                        this.arrayElement1 = _data[0];
                        this.arrayElement2 = _data[1];
                    }
                    else
                    if ((int)_index == 50)       // According to the index 50 (=wordnumber) the nominal load is written. 
                    {
                        this.arrayElement3 = _data[0];
                        this.arrayElement4 = _data[1];
                    }
                        break;

                case Behavior.WriteArrayFail:
                    this.arrayElement1 = 0;
                    this.arrayElement2 = 0;

                    break;

                case Behavior.WriteArraySuccess:
                    this.arrayElement1 = _data[0];
                    this.arrayElement2 = _data[1];

                    break;

                case Behavior.MeasureZeroSuccess:

                    _dataWTX[0] = 0;
                    _dataWTX[0] = 0; 
                    this.arrayElement1 = _data[0];
                    this.arrayElement2 = _data[1];

                    break;

                case Behavior.MeasureZeroFail:
                    
                    _dataWTX[0] = 555;
                    this.arrayElement1 = 0;
                    this.arrayElement2 = 0;

                    break;
                default:
                    break; 
            }
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public async Task<ushort[]> ReadAsync()
        {
            ushort[] value = new ushort[1];
            await Task.Run(async () =>
            {
            switch (behavior)
            {
                case Behavior.ReadFail:

                    // If there is a connection fail, all data attributes get 0 as value.

                    for (int i = 0; i < _dataWTX.Length; i++)
                    {
                        _dataWTX[i] = 0x0000;
                    }

                    _logObj = new LogEvent("Read failed : Registers have not been read");

                    BusActivityDetection?.Invoke(this, _logObj);

                    break;

                case Behavior.ReadSuccess:


                    // The most important data attributes from the WTX120 device: 
                    
                    _dataWTX[0] = 0x0000;
                    _dataWTX[1] = 0x4040;
                    _dataWTX[2] = 0x0000;
                    _dataWTX[3] = 0x4040;
                    _dataWTX[4] = 0x0000;
                    _dataWTX[5] = 0x0000;

                    _logObj = new LogEvent("Read successful: Registers have been read");
                    BusActivityDetection?.Invoke(this, _logObj);
                    break;

                // Simulate for testing 'Unit': 

                case Behavior.t_UnitValue_Success:
                    _dataWTX[5] = 0x100;
                    break;
                case Behavior.t_UnitValue_Fail:
                    _dataWTX[5] = 0x0000;
                    break;

                case Behavior.kg_UnitValue_Success:
                    _dataWTX[5] = 0x0000;
                    break;

                case Behavior.kg_UnitValue_Fail:
                    _dataWTX[5] = 0xFFFF;
                    break;

                case Behavior.g_UnitValue_Success:
                    _dataWTX[5] = 0x80;
                    break;

                case Behavior.g_UnitValue_Fail:
                    _dataWTX[5] = 0x0000;
                    break;

                case Behavior.lb_UnitValue_Success:
                    _dataWTX[5] = 0x180;
                    break;


                case Behavior.lb_UnitValue_Fail:
                    _dataWTX[5] = 0x0000;
                    break;


                // Simulate for testing 'Limit status': 

                case Behavior.LimitStatusStringComment_Case0_Fail:
                    _dataWTX[4] = 0xC;
                    break;
                case Behavior.LimitStatusStringComment_Case1_Fail:
                    _dataWTX[4] = 0x8;
                    break;
                case Behavior.LimitStatusStringComment_Case2_Fail:
                    _dataWTX[4] = 0x0000;
                    break;
                case Behavior.LimitStatusStringComment_Case3_Fail:
                    _dataWTX[4] = 0x4;
                    break;

                case Behavior.LimitStatusStringComment_Case0_Success:
                    _dataWTX[4] = 0x0000;
                    break;
                case Behavior.LimitStatusStringComment_Case1_Success:
                    _dataWTX[4] = 0x4;
                    break;
                case Behavior.LimitStatusStringComment_Case2_Success:
                    _dataWTX[4] = 0x8;
                    break;
                case Behavior.LimitStatusStringComment_Case3_Success:
                    _dataWTX[4] = 0xC;
                    break;

                // Simulate for testing 'Weight moving': 
                case Behavior.WeightMovingStringComment_Case0_Fail:
                    _dataWTX[4] = 0x0010;
                    break;
                case Behavior.WeightMovingStringComment_Case1_Fail:
                    _dataWTX[4] = 0x0000;
                    break;
                case Behavior.WeightMovingStringComment_Case0_Success:
                    _dataWTX[4] = 0x0000;
                    break;
                case Behavior.WeightMovingStringComment_Case1_Success:
                    _dataWTX[4] = 0x0010;
                    break;

                // Simulate for testing 'Weight type': 
                case Behavior.WeightTypeStringComment_Case0_Fail:
                    _dataWTX[4] = 0x0080;
                    break;
                case Behavior.WeightTypeStringComment_Case1_Fail:
                    _dataWTX[4] = 0x0000;
                    break;

                case Behavior.WeightTypeStringComment_Case0_Success:
                    _dataWTX[4] = 0x0000;
                    break;
                case Behavior.WeightTypeStringComment_Case1_Success:
                    _dataWTX[4] = 0x0080;
                    break;
                // Simulate for testing 'Scale range': 

                case Behavior.ScaleRangeStringComment_Range1_Fail:
                    _dataWTX[4] = 0x200;
                    break;

                case Behavior.ScaleRangeStringComment_Range2_Fail:
                    _dataWTX[4] = 0x0000;
                    break;

                case Behavior.ScaleRangeStringComment_Range3_Fail:
                    _dataWTX[4] = 0x100;
                    break;

                case Behavior.ScaleRangeStringComment_Range1_Success:
                    _dataWTX[4] = 0x0000;
                    break;

                case Behavior.ScaleRangeStringComment_Range2_Success:
                    _dataWTX[4] = 0x100;
                    break;

                case Behavior.ScaleRangeStringComment_Range3_Success:
                    _dataWTX[4] = 0x200;
                    break;

                case Behavior.LogEvent_Fail:

                    _logObj = new LogEvent("Read failed : Registers have not been read");
                    BusActivityDetection?.Invoke(this, _logObj);
                    break;

                case Behavior.LogEvent_Success:

                    _logObj = new LogEvent("Read successful: Registers have been read");
                    BusActivityDetection?.Invoke(this, _logObj);
                    break;
            }
            if (_dataWTX[5] == 0x0000)
            {
                _dataWTX[5] = 0x4000;
            }
            else
            if (_dataWTX[5] == 0x4000)
            {
                _dataWTX[5] = 0x0000;
            }
            
                this.UpdateDictionary();
                // Update data in data classes : 
                this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

                return _dataWTX;

             });

            this.UpdateDictionary();
            // Update data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

            return _dataWTX;
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this.command = commandParam;

            switch (behavior)
            {
                case Behavior.ZeroMethodTestSuccess:
                    this.command = commandParam;
                    break;
                case Behavior.ZeroMethodTestFail:
                    this.command = 0x00;
                    break;

                case Behavior.AbortDosingMethodTestSuccess:
                    this.command = commandParam;
                    break;
                case Behavior.AbortDosingMethodTestFail:
                    this.command = 0;
                    break;
            }

            // Change the handshake bit : bit .14 from 0 to 1.
            if (_dataWTX[5] == 0x0000)
                _dataWTX[5] = 0x4000;
            else
                if (_dataWTX[5] == 0x4000)
                _dataWTX[5] = 0x0000;


            this.command = commandParam;

            return this.command;
        }


        public Dictionary<string, int> AllData
        {
            get
            {
                return this._dataIntegerBuffer;
            }
        }

        public int getWordNumber
        {
            get
            {
                return this.wordNumberIndex;
            }
        }

        public ushort getArrElement1
        {
            get
            {
                return this.arrayElement1;
            }
        }

        public ushort getArrElement2
        {
            get
            {
                return this.arrayElement2;
            }
        }

        public ushort getArrElement3
        {
            get
            {
                return this.arrayElement3;
            }
        }

        public ushort getArrElement4
        {
            get
            {
                return this.arrayElement4;
            }
        }

        public int NumofPoints
        {
            get
            {
                return this.numPoints;
            }
            set
            {
                this.numPoints = value; 
            }
        }
        
        public string IpAddress
        {
            get
            {
                return this.IP;
            }
            set
            {
                this.IP = value; 
            }
        }

        public int SendingInterval
        {
            get
            {
                return this.interval;
            }
            set
            {
                this.interval = value;
            }
        }

        public ushort[] getData {

            get
            {
                return this._dataWTX;
            }
            set
            {
                this._dataWTX = value; 
            }

        }
        public ConnectionType ConnType
        {
            get { return ConnectionType.Modbus; }
        }
        //public Dictionary<string, JToken> getDataBuffer => throw new NotImplementedException();
    }
}
