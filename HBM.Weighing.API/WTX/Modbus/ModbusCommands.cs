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
        public ModbusCommands()
        {
            // region : ID Commands : Memory - day, month, year, seqNumber, gross, net

            // For standard mode: 
            ReadWeightMemDay_ID   = new ModbusCommand(DataType.Int16, "9", IOType.Input, 0, 0);
            ReadWeightMemMonth_ID = new ModbusCommand(DataType.Int16, "10", IOType.Input, 0, 0);
            ReadWeightMemYear_ID  = new ModbusCommand(DataType.Int16, "11", IOType.Input, 0, 0);
            ReadWeightMemSeqNumber_ID = new ModbusCommand(DataType.Int16, "12", IOType.Input, 0, 0);
            ReadWeightMemGross_ID = new ModbusCommand(DataType.Int16, "13", IOType.Input, 0, 0);
            ReadWeightMemNet_ID   = new ModbusCommand(DataType.Int16, "14", IOType.Input, 0, 0);

            // For filler mode: 
            WriteWeightMemDay_ID   = new ModbusCommand(DataType.Int16, "32", IOType.Output, 0, 0);
            WriteWeightMemMonth_ID = new ModbusCommand(DataType.Int16, "33", IOType.Output, 0, 0);
            WriteWeightMemYear_ID  = new ModbusCommand(DataType.Int16, "34", IOType.Output, 0, 0);
            WriteWeightMemSeqNumber_ID = new ModbusCommand(DataType.Int16, "35", IOType.Output, 0, 0);
            WriteWeightMemGross_ID = new ModbusCommand(DataType.Int16, "36", IOType.Output, 0, 0);
            WriteWeightMemNet_ID   = new ModbusCommand(DataType.Int16, "37", IOType.Output, 0, 0);

            // region ID Commands : Maintenance - Calibration

            Ldw_dead_weight   = new ModbusCommand(DataType.S32, "48", IOType.Output, 0, 0);              // LDW = Nullpunkt
            Lwt_nominal_value = new ModbusCommand(DataType.S32, "50", IOType.Output, 0, 0);              // LWT = Nennwert
            Lft_scale_calibration_weight = new ModbusCommand(DataType.S32, "46", IOType.Output, 0, 0);   // LFT = LFT scale calibration weight

            // region ID commands for process data
            Net_value   = new ModbusCommand(DataType.U08, "0", IOType.Input, 0, 0);
            Gross_value = new ModbusCommand(DataType.U08, "2", IOType.Input, 0, 0);
            Zero_value  = new ModbusCommand(DataType.U08, "", IOType.Input, 0, 0);

            Weighing_device_1_weight_status = new ModbusCommand(DataType.U08, "4", IOType.Input, 0, 0);

            Limit_status = new ModbusCommand(DataType.U08, "4", IOType.Input, 2, 2);                   // data word = 4 ; length = 2 ; offset = 2;
            Unit_prefix_fixed_parameter = new ModbusCommand(DataType.U08, "5", IOType.Input, 2, 7);    // data word = 5 ; length = 2 ; offset = 7;

            Application_mode = new ModbusCommand(DataType.U08, "5", IOType.Input, 2, 0);               // data word = 5 ; length = 2 ; offset = 0;
            Decimals = new ModbusCommand(DataType.U08, "5", IOType.Input, 3, 4);                       // data word = 5 ; length = 3 ; offset = 4;
            Scale_command_status = new ModbusCommand(DataType.U08, "5", IOType.Input, 1, 15);          // data word = 5 ; length = 1 ; offset = 15;

            Scale_command = new ModbusCommand(DataType.U08, "", IOType.Input, 0, 0);

            // region ID commands for standard mode
            Status_digital_input_1 = new ModbusCommand(DataType.U08, "6", IOType.Input, 1, 1);    // IS1
            Status_digital_input_2 = new ModbusCommand(DataType.U08, "6", IOType.Input, 2, 1);    // IS2
            Status_digital_input_3 = new ModbusCommand(DataType.U08, "6", IOType.Input, 3, 1);    // IS3
            Status_digital_input_4 = new ModbusCommand(DataType.U08, "6", IOType.Input, 4, 1);    // IS4

            Status_digital_output_1 = new ModbusCommand(DataType.U08, "7", IOType.Input, 1, 1);   // OS1
            Status_digital_output_2 = new ModbusCommand(DataType.U08, "7", IOType.Input, 2, 1);   // OS2
            Status_digital_output_3 = new ModbusCommand(DataType.U08, "7", IOType.Input, 3, 1);   // OS3
            Status_digital_output_4 = new ModbusCommand(DataType.U08, "7", IOType.Input, 4, 1);   // OS4

            Limit_value = new ModbusCommand(DataType.U08, "8", IOType.Input, 0, 0);   // LVS , standard

            Tare_value = new ModbusCommand(DataType.U08, "2", IOType.Input, 0, 0);  // manual tare value

            Limit_value_monitoring_liv11 = new ModbusCommand(DataType.U08, "4", IOType.Output, 0, 0); // = Grenzwertüberwachung 
            Signal_source_liv12 = new ModbusCommand(DataType.U08, "5", IOType.Output, 0, 0);
            Switch_on_level_liv13 = new ModbusCommand(DataType.S32, "6", IOType.Output, 0, 0);        // = Einschaltpegel
            Switch_off_level_liv14 = new ModbusCommand(DataType.S32, "8", IOType.Output, 0, 0);       // = Ausschaltpegel

            Limit_value_monitoring_liv21 = new ModbusCommand(DataType.U08, "10", IOType.Output, 0, 0);
            Signal_source_liv22 = new ModbusCommand(DataType.U08, "11", IOType.Output, 0, 0);
            Switch_on_level_liv23 = new ModbusCommand(DataType.S32, "12", IOType.Output, 0, 0);
            Switch_off_level_liv24 = new ModbusCommand(DataType.S32, "14", IOType.Output, 0, 0);

            Limit_value_monitoring_liv31 = new ModbusCommand(DataType.U08, "16", IOType.Output, 0, 0);
            Signal_source_liv32 = new ModbusCommand(DataType.U08, "17", IOType.Output, 0, 0);
            Switch_on_level_liv33 = new ModbusCommand(DataType.S32, "18", IOType.Output, 0, 0);
            Switch_off_level_liv34 = new ModbusCommand(DataType.S32, "20", IOType.Output, 0, 0);

            Limit_value_monitoring_liv41 = new ModbusCommand(DataType.U08, "22", IOType.Output, 0, 0);
            Signal_source_liv42 = new ModbusCommand(DataType.U08, "23", IOType.Output, 0, 0);
            Switch_on_level_liv43 = new ModbusCommand(DataType.S32, "24", IOType.Output, 0, 0);
            Switch_off_level_liv44 = new ModbusCommand(DataType.S32, "26", IOType.Output, 0, 0);

            // region ID commands for filler data

            CoarseFlow = new ModbusCommand(DataType.U08, "8", IOType.Input, 0, 1);               // data input word 8, bit .0, application mode=filler
            FineFlow = new ModbusCommand(DataType.U08, "8", IOType.Input, 1, 1);                 // data input word 8, bit .1, application mode=filler
            Ready = new ModbusCommand(DataType.U08, "8", IOType.Input, 2, 1);                    // data input word 8, bit .2, application mode=filler
            ReDosing = new ModbusCommand(DataType.U08, "8",  IOType.Input, 3, 1);                // data input word 8, bit .3, application mode=filler; RDS = Nachdosieren
            Emptying = new ModbusCommand(DataType.U08, "8",  IOType.Input, 4, 1);                // data input word 8, bit .4, application mode=filler
            FlowError = new ModbusCommand(DataType.U08, "8", IOType.Input, 5, 1);                // data input word 8, bit .5, application mode=filler
            Alarm = new ModbusCommand(DataType.U08, "8", IOType.Input, 6, 1);                    // data input word 8, bit .6, application mode=filler
            AdcOverUnderload = new ModbusCommand(DataType.U08, "8", IOType.Input, 7, 1);         // data input word 8, bit .7, application mode=filler
            MaximalDosingTimeInput = new ModbusCommand(DataType.U08, "8", IOType.Input, 8, 1);   // data input word 8, bit .8, application mode=filler
            LegalForTradeOperation = new ModbusCommand(DataType.U08, "8", IOType.Input, 9, 1);   // data input word 8, bit .9, application mode=filler
            ToleranceErrorPlus = new ModbusCommand(DataType.U08, "8", IOType.Input, 10, 1);      // data input word 8, bit .10, application mode=filler
            ToleranceErrorMinus = new ModbusCommand(DataType.U08, "8", IOType.Input, 11, 1);     // data input word 8, bit .11, application mode=filler
            StatusInput1 = new ModbusCommand(DataType.U08, "8", IOType.Input, 14, 1);            // data input word 8, bit .14, application mode=filler
            GeneralScaleError = new ModbusCommand(DataType.U08, "8", IOType.Input, 15, 1);       // data input word 8, bit .15, application mode=filler

            TotalWeight = new ModbusCommand(DataType.S32, "18", IOType.Input, 0, 0);             // data input word 18, application mode=filler
            Dosing_time = new ModbusCommand(DataType.U16, "24", IOType.Input, 0, 0);             // DST = Dosieristzeit
            Coarse_flow_time = new ModbusCommand(DataType.U16, "25", IOType.Input, 0, 0);        // CFT = Grobstromzeit
            CurrentFineFlowTime = new ModbusCommand(DataType.U16, "26", IOType.Input, 0, 0);     // data input word 26, application mode=filler; FFT = Feinstromzeit
            ParameterSetProduct = new ModbusCommand(DataType.U08, "27", IOType.Input, 0, 0);     // data input word 27, application mode=filler

            TargetFillingWeight = new ModbusCommand(DataType.S32, "10", IOType.Output, 0, 0);        // data output word 10, application mode=filler
            Residual_flow_time = new ModbusCommand(DataType.U16, "9", IOType.Output, 0, 0);          // RFT = Nachstromzeit
            //Reference_value_dosing = new ModbusCommand(1, "10", IOType.Output, 0, 0);     // FWT = Sollwert dosieren = Target filling weight
            Coarse_flow_cut_off_point = new ModbusCommand(DataType.S32, "12", IOType.Output, 0, 0);  // CFD = Grobstromabschaltpunkt
            Fine_flow_cut_off_point = new ModbusCommand(DataType.S32, "14", IOType.Output, 0, 0);    // FFD = Feinstromabschaltpunkt

            Minimum_fine_flow = new ModbusCommand(DataType.S32, "16", IOType.Output, 0, 0);          // FFM = Minimaler Feinstromanteil
            Optimization = new ModbusCommand(DataType.U08, "18", IOType.Output, 0, 0);               // OSN = Optimierung
            Maximal_dosing_time = new ModbusCommand(DataType.U16, "19", IOType.Output, 0, 0);        // MDT = Maximale Dosierzeit
            Run_start_dosing = new ModbusCommand(DataType.U16, "20", IOType.Output, 0, 0);           // RUN = Start Dosieren

            Lockout_time_coarse_flow = new ModbusCommand(DataType.U16, "21", IOType.Output, 0, 0);   // LTC = Sperrzeit Grobstrom
            Lockout_time_fine_flow = new ModbusCommand(DataType.U16, "22", IOType.Output, 0, 0);     // LTF = Sperrzeit Feinstrom
            Tare_mode = new ModbusCommand(DataType.U08, "23", IOType.Output, 0, 0);                  // TMD = Tariermodus
            Upper_tolerance_limit = new ModbusCommand(DataType.S32, "24", IOType.Output, 0, 0);      // UTL = Obere Toleranz

            Lower_tolerance_limit = new ModbusCommand(DataType.S32, "26", IOType.Output, 0, 0);      // LTL = Untere Toleranz
            Minimum_start_weight = new ModbusCommand(DataType.S32, "28", IOType.Output, 0, 0);       // MSW = Minimum Startgewicht
            Empty_weight = new ModbusCommand(DataType.S32, "30", IOType.Output, 0, 0);
            Tare_delay = new ModbusCommand(DataType.U16, "32", IOType.Output, 0, 0);                  // TAD = Tarierverzögerung

            Coarse_flow_monitoring_time = new ModbusCommand(DataType.U16, "33", IOType.Output, 0, 0); // CBT = Überwachungszeit Grobstrom
            Coarse_flow_monitoring = new ModbusCommand(DataType.U32, "34", IOType.Output, 0, 0);      // CBK = Füllstromüberwachung Grobstrom
            Fine_flow_monitoring = new ModbusCommand(DataType.U32, "36", IOType.Output, 0, 0);        // FBK = Füllstromüberwachung Feinstrom
            Fine_flow_monitoring_time = new ModbusCommand(DataType.U16, "38", IOType.Output, 0, 0);   // FBT = Überwachungszeit Feinstrom

            Delay_time_after_fine_flow = new ModbusCommand(DataType.U08, "39", IOType.Output, 0, 0);
            Activation_time_after_fine_flow = new ModbusCommand(DataType.U08, "40", IOType.Output, 0, 0);

            Systematic_difference = new ModbusCommand(DataType.U32, "41", IOType.Output, 0, 0);       // SYD = Systematische Differenz
            DownwardsDosing = new ModbusCommand(DataType.U08, "42", IOType.Output, 0, 0);             // data output word 42, application mode=filler
            Valve_control = new ModbusCommand(DataType.U08, "43", IOType.Output, 0, 0);               // VCT = Ventilsteuerung
            Emptying_mode = new ModbusCommand(DataType.U08, "44", IOType.Output, 0, 0);               // EMD = Entleermodus
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

        public ModbusCommand Limit_status { get; private set; }                   // data word = 4 ; length = 2 ; offset = 2;
        public ModbusCommand Unit_prefix_fixed_parameter { get; private set; }    // data word = 5 ; length = 2 ; offset = 7;

        public ModbusCommand Application_mode { get; private set; }               // data word = 5 ; length = 2 ; offset = 0;
        public ModbusCommand Decimals { get; private set; }                       // data word = 5 ; length = 3 ; offset = 4;
        public ModbusCommand Scale_command_status { get; private set; }           // data word = 5 ; length = 1 ; offset = 15;

        public ModbusCommand Scale_command { get; private set; }

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

        public ModbusCommand Limit_value_monitoring_liv11 { get; private set; } // = Grenzwertüberwachung 
        public ModbusCommand Signal_source_liv12 { get; private set; }
        public ModbusCommand Switch_on_level_liv13 { get; private set; }        // = Einschaltpegel
        public ModbusCommand Switch_off_level_liv14 { get; private set; }       // = Ausschaltpegel

        public ModbusCommand Limit_value_monitoring_liv21 { get; private set; }
        public ModbusCommand Signal_source_liv22 { get; private set; }
        public ModbusCommand Switch_on_level_liv23 { get; private set; }
        public ModbusCommand Switch_off_level_liv24 { get; private set; }

        public ModbusCommand Limit_value_monitoring_liv31 { get; private set; }
        public ModbusCommand Signal_source_liv32 { get; private set; }
        public ModbusCommand Switch_on_level_liv33 { get; private set; }
        public ModbusCommand Switch_off_level_liv34 { get; private set; }

        public ModbusCommand Limit_value_monitoring_liv41 { get; private set; }
        public ModbusCommand Signal_source_liv42 { get; private set; }
        public ModbusCommand Switch_on_level_liv43  { get; private set; }
        public ModbusCommand Switch_off_level_liv44 { get; private set; }

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
