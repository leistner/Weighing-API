// <copyright file="ModbusCommands.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API.WTX.Modbus
{
    /// <summary>
    /// Class for using commands, respectively indexes/paths, to read/write the 
    /// registers of the WTX device via Modbus to get the data.
    /// ID's/Commands for subscribing values of the WTX device. 
    /// The ID's are commited as a parameter for the read and/or write method call.  
    /// This class inherits from interface ICommands. 
    /// </summary>
    public class ModbusCommands
    {
        public List<ModbusCommand> items;

        public ModbusCommands()
        {
            items = new List<ModbusCommand>();

            this.CreateList();

            // region : ID Commands : Memory - day, month, year, seqNumber, gross, net
            //Limit_value_monitoring_liv31 Minimum_fine_flow
            // For standard mode: 
            ReadWeightMemDay_ID   = new ModbusCommand(DataType.Int16, "9",  IOType.Input, ApplicationMode.Standard, 0, 0);
            ReadWeightMemMonth_ID = new ModbusCommand(DataType.Int16, "10", IOType.Input, ApplicationMode.Standard, 0, 0);
            ReadWeightMemYear_ID  = new ModbusCommand(DataType.Int16, "11", IOType.Input, ApplicationMode.Standard, 0, 0);
            ReadWeightMemSeqNumber_ID = new ModbusCommand(DataType.Int16, "12", IOType.Input, ApplicationMode.Standard, 0, 0);
            ReadWeightMemGross_ID = new ModbusCommand(DataType.Int16, "13", IOType.Input, ApplicationMode.Standard, 0, 0);
            ReadWeightMemNet_ID   = new ModbusCommand(DataType.Int16, "14", IOType.Input, ApplicationMode.Standard, 0, 0);

            // For filler mode: 
            WriteWeightMemDay_ID   = new ModbusCommand(DataType.Int16, "32", IOType.Output, ApplicationMode.Filler, 0, 0);
            WriteWeightMemMonth_ID = new ModbusCommand(DataType.Int16, "33", IOType.Output, ApplicationMode.Filler, 0, 0);
            WriteWeightMemYear_ID  = new ModbusCommand(DataType.Int16, "34", IOType.Output, ApplicationMode.Filler, 0, 0);
            WriteWeightMemSeqNumber_ID = new ModbusCommand(DataType.Int16, "35", IOType.Output, ApplicationMode.Filler, 0, 0);
            WriteWeightMemGross_ID = new ModbusCommand(DataType.Int16, "36", IOType.Output, ApplicationMode.Filler, 0, 0);
            WriteWeightMemNet_ID   = new ModbusCommand(DataType.Int16, "37", IOType.Output, ApplicationMode.Filler, 0, 0);

            // region ID Commands : Maintenance - Calibration

            Lft_scale_calibration_weight = new ModbusCommand(DataType.S32, "46", IOType.Output, ApplicationMode.Standard, 0, 0);   // LFT = LFT scale calibration weight
            Ldw_dead_weight   = new ModbusCommand(DataType.S32, "48", IOType.Output, ApplicationMode.Standard, 0, 0);              // LDW = Nullpunkt
            Lwt_nominal_value = new ModbusCommand(DataType.S32, "50", IOType.Output, ApplicationMode.Standard, 0, 0);              // LWT = Nennwert

            // region ID commands for process data
            Net_value   = new ModbusCommand(DataType.Int32, "0", IOType.Input, ApplicationMode.Standard, 32, 0);
            Gross_value = new ModbusCommand(DataType.Int32, "2", IOType.Input, ApplicationMode.Standard, 32, 0);
            Zero_value  = new ModbusCommand(DataType.Int32, "",  IOType.Input, ApplicationMode.Standard, 32, 0);

            Weighing_device_1_weight_status = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 0, 0);

            GeneralWeightError = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 0);
            ScaleAlarmTriggered = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 1);
            Limit_status = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 2, 2);                   // data word = 4 ; length = 2 ; offset = 2;
            WeightMoving = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 4);
            ScaleSealIsOpen = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 5);
            ManualTare = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 6);
            WeightType = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 7);
            ScaleRange = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 2, 8);
            ZeroRequired = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 10);
            WeightinCenterOfZero = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 11);
            WeightinZeroRange = new ModbusCommand(DataType.U08, "4", IOType.Input, ApplicationMode.Standard, 1, 12);

            Application_mode = new ModbusCommand(DataType.U08, "5", IOType.Input, ApplicationMode.Standard, 2, 0);               // data word = 5 ; length = 2 ; offset = 0;
            Decimals = new ModbusCommand(DataType.U08, "5", IOType.Input, ApplicationMode.Standard, 3, 4);                       // data word = 5 ; length = 3 ; offset = 4;
            Unit_prefix_fixed_parameter = new ModbusCommand(DataType.U08, "5", IOType.Input, ApplicationMode.Standard, 2, 7);    // data word = 5 ; length = 2 ; offset = 7;
            Handshake = new ModbusCommand(DataType.U08, "5", IOType.Input, ApplicationMode.Standard, 1, 14);
            Status = new ModbusCommand(DataType.U08, "5", IOType.Input, ApplicationMode.Standard, 1, 15);          // data word = 5 ; length = 1 ; offset = 15;

            // region ID commands for standard mode
            Status_digital_input_1 = new ModbusCommand(DataType.U08, "6", IOType.Input, ApplicationMode.Standard, 1, 1);    // IS1
            Status_digital_input_2 = new ModbusCommand(DataType.U08, "6", IOType.Input, ApplicationMode.Standard, 2, 1);    // IS2
            Status_digital_input_3 = new ModbusCommand(DataType.U08, "6", IOType.Input, ApplicationMode.Standard, 3, 1);    // IS3
            Status_digital_input_4 = new ModbusCommand(DataType.U08, "6", IOType.Input, ApplicationMode.Standard, 4, 1);    // IS4

            Status_digital_output_1 = new ModbusCommand(DataType.U08, "7", IOType.Input, ApplicationMode.Standard, 1, 1);   // OS1
            Status_digital_output_2 = new ModbusCommand(DataType.U08, "7", IOType.Input, ApplicationMode.Standard, 2, 1);   // OS2
            Status_digital_output_3 = new ModbusCommand(DataType.U08, "7", IOType.Input, ApplicationMode.Standard, 3, 1);   // OS3
            Status_digital_output_4 = new ModbusCommand(DataType.U08, "7", IOType.Input, ApplicationMode.Standard, 4, 1);   // OS4

            Limit_value = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Standard, 0, 0);   // LVS , standard
            Tare_value = new ModbusCommand(DataType.U08, "2", IOType.Input, ApplicationMode.Standard, 0, 0);  // manual tare value

            LimitValue1Input = new ModbusCommand(DataType.U08, "4", IOType.Output, ApplicationMode.Standard, 0, 0); 
            LimitValue1Mode = new ModbusCommand(DataType.U08, "5", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue1ActivationLevelLowerBandLimit = new ModbusCommand(DataType.S32, "6", IOType.Output, ApplicationMode.Standard, 0, 0);       
            LimitValue1HysteresisBandHeight = new ModbusCommand(DataType.S32, "8", IOType.Output, ApplicationMode.Standard, 0, 0);       

            LimitValue2Source = new ModbusCommand(DataType.U08, "10", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue2Mode = new ModbusCommand(DataType.U08, "11", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue2ActivationLevelLowerBandLimit = new ModbusCommand(DataType.S32, "12", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue2HysteresisBandHeight = new ModbusCommand(DataType.S32, "14", IOType.Output, ApplicationMode.Standard, 0, 0);

            LimitValue3Source = new ModbusCommand(DataType.U08, "16", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue3Mode = new ModbusCommand(DataType.U08, "17", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue3ActivationLevelLowerBandLimit = new ModbusCommand(DataType.S32, "18", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue3HysteresisBandHeight = new ModbusCommand(DataType.S32, "20", IOType.Output, ApplicationMode.Standard, 0, 0);

            LimitValue4Source = new ModbusCommand(DataType.U08, "22", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue4Mode = new ModbusCommand(DataType.U08, "23", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue4ActivationLevelLowerBandLimit = new ModbusCommand(DataType.S32, "24", IOType.Output, ApplicationMode.Standard, 0, 0);
            LimitValue4HysteresisBandHeight = new ModbusCommand(DataType.S32, "26", IOType.Output, ApplicationMode.Standard, 0, 0);

            // region ID commands for filler data

            CoarseFlow = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 0, 1);               // data input word 8, bit .0, application mode=filler
            FineFlow = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 1, 1);                 // data input word 8, bit .1, application mode=filler
            Ready = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 2, 1);                    // data input word 8, bit .2, application mode=filler
            ReDosing = new ModbusCommand(DataType.U08, "8",  IOType.Input, ApplicationMode.Filler, 3, 1);                // data input word 8, bit .3, application mode=filler; RDS = Nachdosieren
            Emptying = new ModbusCommand(DataType.U08, "8",  IOType.Input, ApplicationMode.Filler, 4, 1);                // data input word 8, bit .4, application mode=filler
            FlowError = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 5, 1);                // data input word 8, bit .5, application mode=filler
            Alarm = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 6, 1);                    // data input word 8, bit .6, application mode=filler
            AdcOverUnderload = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 7, 1);         // data input word 8, bit .7, application mode=filler
            MaximalDosingTimeInput = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 8, 1);   // data input word 8, bit .8, application mode=filler
            LegalForTradeOperation = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 9, 1);   // data input word 8, bit .9, application mode=filler
            ToleranceErrorPlus = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 10, 1);      // data input word 8, bit .10, application mode=filler
            ToleranceErrorMinus = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 11, 1);     // data input word 8, bit .11, application mode=filler
            StatusInput1 = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 14, 1);            // data input word 8, bit .14, application mode=filler
            GeneralScaleError = new ModbusCommand(DataType.U08, "8", IOType.Input, ApplicationMode.Filler, 15, 1);       // data input word 8, bit .15, application mode=filler

            TotalWeight = new ModbusCommand(DataType.S32, "18", IOType.Input, ApplicationMode.Filler, 0, 0);             // data input word 18, application mode=filler
            Dosing_time = new ModbusCommand(DataType.U16, "24", IOType.Input, ApplicationMode.Filler, 0, 0);             // DST = Dosieristzeit
            Coarse_flow_time = new ModbusCommand(DataType.U16, "25", IOType.Input, ApplicationMode.Filler, 0, 0);        // CFT = Grobstromzeit
            CurrentFineFlowTime = new ModbusCommand(DataType.U16, "26", IOType.Input, ApplicationMode.Filler, 0, 0);     // data input word 26, application mode=filler; FFT = Feinstromzeit
            ParameterSetProduct = new ModbusCommand(DataType.U08, "27", IOType.Input, ApplicationMode.Filler, 0, 0);     // data input word 27, application mode=filler

            TargetFillingWeight = new ModbusCommand(DataType.S32, "10", IOType.Output, ApplicationMode.Filler, 0, 0);        // data output word 10, application mode=filler
            Residual_flow_time = new ModbusCommand(DataType.U16, "9", IOType.Output, ApplicationMode.Filler, 0, 0);          // RFT = Nachstromzeit
            //Reference_value_dosing = new ModbusCommand(1, "10", IOType.Output, ApplicationMode.Filler,0, 0);     // FWT = Sollwert dosieren = Target filling weight
            Coarse_flow_cut_off_point = new ModbusCommand(DataType.S32, "12", IOType.Output, ApplicationMode.Filler, 0, 0);  // CFD = Grobstromabschaltpunkt
            Fine_flow_cut_off_point = new ModbusCommand(DataType.S32, "14", IOType.Output, ApplicationMode.Filler, 0, 0);    // FFD = Feinstromabschaltpunkt

            Minimum_fine_flow = new ModbusCommand(DataType.S32, "16", IOType.Output, ApplicationMode.Filler, 0, 0);          // FFM = Minimaler Feinstromanteil
            Optimization = new ModbusCommand(DataType.U08, "18", IOType.Output, ApplicationMode.Filler, 0, 0);               // OSN = Optimierung
            Maximal_dosing_time = new ModbusCommand(DataType.U16, "19", IOType.Output, ApplicationMode.Filler, 0, 0);        // MDT = Maximale Dosierzeit
            Run_start_dosing = new ModbusCommand(DataType.U16, "20", IOType.Output, ApplicationMode.Filler, 0, 0);           // RUN = Start Dosieren

            Lockout_time_coarse_flow = new ModbusCommand(DataType.U16, "21", IOType.Output, ApplicationMode.Filler, 0, 0);   // LTC = Sperrzeit Grobstrom
            Lockout_time_fine_flow = new ModbusCommand(DataType.U16, "22", IOType.Output, ApplicationMode.Filler, 0, 0);     // LTF = Sperrzeit Feinstrom
            Tare_mode = new ModbusCommand(DataType.U08, "23", IOType.Output, ApplicationMode.Filler, 0, 0);                  // TMD = Tariermodus
            Upper_tolerance_limit = new ModbusCommand(DataType.S32, "24", IOType.Output, ApplicationMode.Filler, 0, 0);      // UTL = Obere Toleranz

            Lower_tolerance_limit = new ModbusCommand(DataType.S32, "26", IOType.Output, ApplicationMode.Filler, 0, 0);      // LTL = Untere Toleranz
            Minimum_start_weight = new ModbusCommand(DataType.S32, "28", IOType.Output, ApplicationMode.Filler, 0, 0);       // MSW = Minimum Startgewicht
            Empty_weight = new ModbusCommand(DataType.S32, "30", IOType.Output, ApplicationMode.Filler, 0, 0);
            Tare_delay = new ModbusCommand(DataType.U16, "32", IOType.Output, ApplicationMode.Filler, 0, 0);                  // TAD = Tarierverzögerung

            Coarse_flow_monitoring_time = new ModbusCommand(DataType.U16, "33", IOType.Output, ApplicationMode.Filler, 0, 0); // CBT = Überwachungszeit Grobstrom
            Coarse_flow_monitoring = new ModbusCommand(DataType.U32, "34", IOType.Output, ApplicationMode.Filler, 0, 0);      // CBK = Füllstromüberwachung Grobstrom
            Fine_flow_monitoring = new ModbusCommand(DataType.U32, "36", IOType.Output, ApplicationMode.Filler, 0, 0);        // FBK = Füllstromüberwachung Feinstrom
            Fine_flow_monitoring_time = new ModbusCommand(DataType.U16, "38", IOType.Output, ApplicationMode.Filler, 0, 0);   // FBT = Überwachungszeit Feinstrom

            Delay_time_after_fine_flow = new ModbusCommand(DataType.U08, "39", IOType.Output, ApplicationMode.Filler, 0, 0);
            Activation_time_after_fine_flow = new ModbusCommand(DataType.U08, "40", IOType.Output, ApplicationMode.Filler, 0, 0);

            Systematic_difference = new ModbusCommand(DataType.U32, "41", IOType.Output, ApplicationMode.Filler, 0, 0);       // SYD = Systematische Differenz
            DownwardsDosing = new ModbusCommand(DataType.U08, "42", IOType.Output, ApplicationMode.Filler, 0, 0);             // data output word 42, application mode=filler
            Valve_control = new ModbusCommand(DataType.U08, "43", IOType.Output, ApplicationMode.Filler, 0, 0);               // VCT = Ventilsteuerung
            Emptying_mode = new ModbusCommand(DataType.U08, "44", IOType.Output, ApplicationMode.Filler, 0, 0);               // EMD = Entleermodus
         }


        private void CreateList()
        {
            items.Add(ReadWeightMemDay_ID);
            items.Add(ReadWeightMemMonth_ID);
            items.Add(ReadWeightMemYear_ID);
            items.Add(ReadWeightMemSeqNumber_ID);
            items.Add(ReadWeightMemGross_ID);
            items.Add(ReadWeightMemNet_ID);

            // For filler mode: 
            items.Add(WriteWeightMemDay_ID);
            items.Add(WriteWeightMemMonth_ID);
            items.Add(WriteWeightMemYear_ID);
            items.Add(WriteWeightMemSeqNumber_ID);
            items.Add(WriteWeightMemGross_ID);
            items.Add(WriteWeightMemNet_ID);

            // region ID Commands : Maintenance - Calibration

            items.Add(Ldw_dead_weight);              // LDW = Nullpunkt
            items.Add(Lwt_nominal_value);              // LWT = Nennwert
            items.Add(Lft_scale_calibration_weight);   // LFT = LFT scale calibration weight

            // region ID commands for process data
            items.Add(Net_value);
            items.Add(Gross_value);
            items.Add(Zero_value);

            items.Add(Weighing_device_1_weight_status);

            items.Add(Limit_status);                   // data word = 4 ; length = 2 ; offset = 2;
            items.Add(Unit_prefix_fixed_parameter);    // data word = 5 ; length = 2 ; offset = 7;

            items.Add(Application_mode);               // data word = 5 ; length = 2 ; offset = 0;
            items.Add(Decimals);                       // data word = 5 ; length = 3 ; offset = 4;
            items.Add(Status);          // data word = 5 ; length = 1 ; offset = 15;

            // region ID commands for standard mode
            items.Add(Status_digital_input_1);    // IS1
            items.Add(Status_digital_input_2);    // IS2
            items.Add(Status_digital_input_3);    // IS3
            items.Add(Status_digital_input_4);    // IS4

            items.Add(Status_digital_output_1);   // OS1
            items.Add(Status_digital_output_2);   // OS2
            items.Add(Status_digital_output_3);   // OS3
            items.Add(Status_digital_output_4);   // OS4

            items.Add(Limit_value);   // LVS , standard
            items.Add(Tare_value);  // manual tare value

            items.Add(LimitValue1Input); // = Grenzwertüberwachung 
            items.Add(LimitValue1Mode);
            items.Add(LimitValue1ActivationLevelLowerBandLimit);        // = Einschaltpegel
            items.Add(LimitValue1HysteresisBandHeight);       // = Ausschaltpegel

            items.Add(LimitValue2Source);
            items.Add(LimitValue2Mode);
            items.Add(LimitValue2ActivationLevelLowerBandLimit);
            items.Add(LimitValue2HysteresisBandHeight);

            items.Add(LimitValue3Source);
            items.Add(LimitValue3Mode);
            items.Add(LimitValue3ActivationLevelLowerBandLimit);
            items.Add(LimitValue3HysteresisBandHeight);

            items.Add(LimitValue4Source);
            items.Add(LimitValue4Mode);
            items.Add(LimitValue4ActivationLevelLowerBandLimit);
            items.Add(LimitValue4HysteresisBandHeight);

        // region ID commands for filler data:

            items.Add(CoarseFlow);               // data input word 8, bit .0, application mode=filler
            items.Add(FineFlow);                 // data input word 8, bit .1, application mode=filler
            items.Add(Ready);                    // data input word 8, bit .2, application mode=filler
            items.Add(ReDosing);                 // data input word 8, bit .3, application mode=filler; RDS = Nachdosieren
            items.Add(Emptying);                 // data input word 8, bit .4, application mode=filler
            items.Add(FlowError);                // data input word 8, bit .5, application mode=filler
            items.Add(Alarm);                    // data input word 8, bit .6, application mode=filler
            items.Add(AdcOverUnderload);         // data input word 8, bit .7, application mode=filler
            items.Add(MaximalDosingTimeInput);   // data input word 8, bit .8, application mode=filler
            items.Add(LegalForTradeOperation);   // data input word 8, bit .9, application mode=filler
            items.Add(ToleranceErrorPlus);       // data input word 8, bit .10, application mode=filler
            items.Add(ToleranceErrorMinus);      // data input word 8, bit .11, application mode=filler
            items.Add(StatusInput1);             // data input word 8, bit .14, application mode=filler
            items.Add(GeneralScaleError);        // data input word 8, bit .15, application mode=filler

            items.Add(TotalWeight);             // data input word 18, application mode=filler
            items.Add(Dosing_time);             // DST = Dosieristzeit
            items.Add(Coarse_flow_time);        // CFT = Grobstromzeit
            items.Add(CurrentFineFlowTime);     // data input word 26, application mode=filler; FFT = Feinstromzeit
            items.Add(ParameterSetProduct);     // data input word 27, application mode=filler

            items.Add(TargetFillingWeight);        // data output word 10, application mode=filler
            items.Add(Residual_flow_time);          // RFT = Nachstromzeit
            //items.Add(Reference_value_dosing);     // FWT = Sollwert dosieren = Target filling weight
            items.Add(Coarse_flow_cut_off_point);  // CFD = Grobstromabschaltpunkt
            items.Add(Fine_flow_cut_off_point);    // FFD = Feinstromabschaltpunkt

            items.Add(Minimum_fine_flow);          // FFM = Minimaler Feinstromanteil
            items.Add(Optimization);               // OSN = Optimierung
            items.Add(Maximal_dosing_time);        // MDT = Maximale Dosierzeit
            items.Add(Run_start_dosing);          // RUN = Start Dosieren

            items.Add(Lockout_time_coarse_flow);   // LTC = Sperrzeit Grobstrom
            items.Add(Lockout_time_fine_flow);     // LTF = Sperrzeit Feinstrom
            items.Add(Tare_mode);                  // TMD = Tariermodus
            items.Add(Upper_tolerance_limit);      // UTL = Obere Toleranz

            items.Add(Lower_tolerance_limit);      // LTL = Untere Toleranz
            items.Add(Minimum_start_weight);       // MSW = Minimum Startgewicht
            items.Add(Empty_weight);
            items.Add(Tare_delay);                  // TAD = Tarierverzögerung

            items.Add(Coarse_flow_monitoring_time); // CBT = Überwachungszeit Grobstrom
            items.Add(Coarse_flow_monitoring);      // CBK = Füllstromüberwachung Grobstrom
            items.Add(Fine_flow_monitoring);        // FBK = Füllstromüberwachung Feinstrom
            items.Add(Fine_flow_monitoring_time);   // FBT = Überwachungszeit Feinstrom

            items.Add(Delay_time_after_fine_flow);
            items.Add(Activation_time_after_fine_flow);

            items.Add(Systematic_difference);       // SYD = Systematische Differenz
            items.Add(DownwardsDosing);             // data output word 42, application mode=filler
            items.Add(Valve_control);               // VCT = Ventilsteuerung
            items.Add(Emptying_mode);               // EMD = Entleermodus
        }

        // region ID Commands : Memory - day, month, year, seqNumber, gross, net

        // For standard mode: 
        public ModbusCommand ReadWeightMemDay_ID { get; private set; }
        public ModbusCommand ReadWeightMemMonth_ID { get; private set; }
        public ModbusCommand ReadWeightMemYear_ID { get; private set; }
        public ModbusCommand ReadWeightMemSeqNumber_ID { get; private set; }
        public ModbusCommand ReadWeightMemGross_ID { get; private set; }
        public ModbusCommand ReadWeightMemNet_ID { get; private set; }

        // For filler mode: 
        public ModbusCommand WriteWeightMemDay_ID { get; private set; }
        public ModbusCommand WriteWeightMemMonth_ID { get; private set; }
        public ModbusCommand WriteWeightMemYear_ID { get; private set; }
        public ModbusCommand WriteWeightMemSeqNumber_ID { get; private set; }
        public ModbusCommand WriteWeightMemGross_ID { get; private set; }
        public ModbusCommand WriteWeightMemNet_ID { get; private set; }

        // region ID Commands : Maintenance - Calibration

        public ModbusCommand Ldw_dead_weight { get; private set; }                // LDW = Nullpunkt
        public ModbusCommand Lwt_nominal_value { get; private set; }              // LWT = Nennwert
        public ModbusCommand Lft_scale_calibration_weight { get; private set; }   // LFT = LFT scale calibration weight

        // region ID commands for process data
        public ModbusCommand Net_value { get; private set; }
        public ModbusCommand Gross_value { get; private set; }
        public ModbusCommand Zero_value { get; private set; }
        public ModbusCommand Weighing_device_1_weight_status { get; private set; }

        public ModbusCommand GeneralWeightError { get; private set; }
        public ModbusCommand ScaleAlarmTriggered { get; private set; }
        public ModbusCommand WeightMoving { get; private set; }
        public ModbusCommand ScaleSealIsOpen { get; private set; }
        public ModbusCommand ManualTare { get; private set; }
        public ModbusCommand WeightType { get; private set; }
        public ModbusCommand ScaleRange { get; private set; }
        public ModbusCommand ZeroRequired { get; private set; }
        public ModbusCommand WeightinCenterOfZero { get; private set; }
        public ModbusCommand WeightinZeroRange { get; private set; }
        public ModbusCommand Decimals { get; private set; }
        public ModbusCommand Handshake { get; private set; }
        public ModbusCommand Limit_status { get; private set; }             
        public ModbusCommand Unit_prefix_fixed_parameter { get; private set; }   
        public ModbusCommand Application_mode { get; private set; }            
        public ModbusCommand Status { get; private set; }    

        // region ID commands for standard mode
        public ModbusCommand Status_digital_input_1 { get; private set; }    // IS1
        public ModbusCommand Status_digital_input_2 { get; private set; }    // IS2
        public ModbusCommand Status_digital_input_3 { get; private set; }    // IS3
        public ModbusCommand Status_digital_input_4 { get; private set; }    // IS4

        public ModbusCommand Status_digital_output_1 { get; private set; }   // OS1
        public ModbusCommand Status_digital_output_2 { get; private set; }   // OS2
        public ModbusCommand Status_digital_output_3 { get; private set; }   // OS3
        public ModbusCommand Status_digital_output_4 { get; private set; }   // OS4

        public ModbusCommand Limit_value { get; private set; }   // LVS

        public ModbusCommand Tare_value { get; private set; }  // manual tare value

        public ModbusCommand LimitValue1Input { get; private set; } // = Grenzwertüberwachung 
        public ModbusCommand LimitValue1Mode { get; private set; }
        public ModbusCommand LimitValue1ActivationLevelLowerBandLimit { get; private set; }        // = Einschaltpegel
        public ModbusCommand LimitValue1HysteresisBandHeight { get; private set; }       // = Ausschaltpegel

        public ModbusCommand LimitValue2Source { get; private set; }
        public ModbusCommand LimitValue2Mode { get; private set; }
        public ModbusCommand LimitValue2ActivationLevelLowerBandLimit { get; private set; }
        public ModbusCommand LimitValue2HysteresisBandHeight { get; private set; }

        public ModbusCommand LimitValue3Source { get; private set; }
        public ModbusCommand LimitValue3Mode { get; private set; }
        public ModbusCommand LimitValue3ActivationLevelLowerBandLimit { get; private set; }
        public ModbusCommand LimitValue3HysteresisBandHeight { get; private set; }

        public ModbusCommand LimitValue4Source { get; private set; }
        public ModbusCommand LimitValue4Mode { get; private set; }
        public ModbusCommand LimitValue4ActivationLevelLowerBandLimit { get; private set; }
        public ModbusCommand LimitValue4HysteresisBandHeight { get; private set; }

        // region ID commands for filler data

        public ModbusCommand CoarseFlow { get; private set; }                // data input word 8, bit .0, application mode=filler
        public ModbusCommand FineFlow { get; private set; }                 // data input word 8, bit .1, application mode=filler
        public ModbusCommand Ready { get; private set; }                    // data input word 8, bit .2, application mode=filler
        public ModbusCommand ReDosing { get; private set; }                 // data input word 8, bit .3, application mode=filler; RDS = Nachdosieren
        public ModbusCommand Emptying { get; private set; }                 // data input word 8, bit .4, application mode=filler
        public ModbusCommand FlowError { get; private set; }                // data input word 8, bit .5, application mode=filler
        public ModbusCommand Alarm { get; private set; }                    // data input word 8, bit .6, application mode=filler
        public ModbusCommand AdcOverUnderload { get; private set; }         // data input word 8, bit .7, application mode=filler
        public ModbusCommand MaximalDosingTimeInput { get; private set; }   // data input word 8, bit .8, application mode=filler
        public ModbusCommand LegalForTradeOperation { get; private set; }   // data input word 8, bit .9, application mode=filler
        public ModbusCommand ToleranceErrorPlus { get; private set; }       // data input word 8, bit .10, application mode=filler
        public ModbusCommand ToleranceErrorMinus { get; private set; }      // data input word 8, bit .11, application mode=filler
        public ModbusCommand StatusInput1 { get; private set; }             // data input word 8, bit .14, application mode=filler
        public ModbusCommand GeneralScaleError { get; private set; }        // data input word 8, bit .15, application mode=filler

        public ModbusCommand TotalWeight { get; private set; }             // data input word 18, application mode=filler
        public ModbusCommand Dosing_time { get; private set; }             // DST = Dosieristzeit
        public ModbusCommand Coarse_flow_time { get; private set; }        // CFT = Grobstromzeit
        public ModbusCommand CurrentFineFlowTime { get; private set; }     // data input word 26, application mode=filler; FFT = Feinstromzeit
        public ModbusCommand ParameterSetProduct { get; private set; }     // data input word 27, application mode=filler
        public ModbusCommand TargetFillingWeight { get; private set; }     // data output word 10, application mode=filler

        public ModbusCommand Residual_flow_time { get; private set; }          // RFT = Nachstromzeit
        public ModbusCommand Reference_value_dosing { get; private set; }      // FWT = Sollwert dosieren = Target filling weight
        public ModbusCommand Coarse_flow_cut_off_point { get; private set; }   // CFD = Grobstromabschaltpunkt
        public ModbusCommand Fine_flow_cut_off_point { get; private set; }     // FFD = Feinstromabschaltpunkt

        public ModbusCommand Minimum_fine_flow { get; private set; }           // FFM = Minimaler Feinstromanteil
        public ModbusCommand Optimization { get; private set; }                // OSN = Optimierung
        public ModbusCommand Maximal_dosing_time { get; private set; }         // MDT = Maximale Dosierzeit
        public ModbusCommand Run_start_dosing { get; private set; }            // RUN = Start Dosieren

        public ModbusCommand Lockout_time_coarse_flow { get; private set; }    // LTC = Sperrzeit Grobstrom
        public ModbusCommand Lockout_time_fine_flow { get; private set; }      // LTF = Sperrzeit Feinstrom
        public ModbusCommand Tare_mode { get; private set; }                   // TMD = Tariermodus
        public ModbusCommand Upper_tolerance_limit { get; private set; }       // UTL = Obere Toleranz

        public ModbusCommand Lower_tolerance_limit { get; private set; }   // LTL = Untere Toleranz
        public ModbusCommand Minimum_start_weight { get; private set; }   // MSW = Minimum Startgewicht
        public ModbusCommand Empty_weight { get; private set; }
        public ModbusCommand Tare_delay { get; private set; }   // TAD = Tarierverzögerung

        public ModbusCommand Coarse_flow_monitoring_time { get; private set; }  // CBT = Überwachungszeit Grobstrom
        public ModbusCommand Coarse_flow_monitoring { get; private set; }       // CBK = Füllstromüberwachung Grobstrom
        public ModbusCommand Fine_flow_monitoring { get; private set; }         // FBK = Füllstromüberwachung Feinstrom
        public ModbusCommand Fine_flow_monitoring_time { get; private set; }    // FBT = Überwachungszeit Feinstrom

        public ModbusCommand Delay_time_after_fine_flow { get; private set; }
        public ModbusCommand Activation_time_after_fine_flow { get; private set; }

        public ModbusCommand Systematic_difference { get; private set; }       // SYD = Systematische Differenz
        public ModbusCommand DownwardsDosing { get; private set; }             // data output word 42, application mode=filler
        public ModbusCommand Valve_control { get; private set; }               // VCT = Ventilsteuerung
        public ModbusCommand Emptying_mode { get; private set; }               // EMD = Entleermodus
    }
}
