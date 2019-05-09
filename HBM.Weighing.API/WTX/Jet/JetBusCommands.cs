// <copyright file="JetBusCommands.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API.WTX.Jet
{
    /// <summary>
    /// Class for using commands, respectively indexes/paths, to read/write the 
    /// registers of the WTX device via Jetbus to get the data.
    /// ID's/Commands for subscribing values of the WTX device. 
    /// The ID's are commited as a parameter for the read and/or write method call.  
    /// This class inherits from interface ICommands. 
    /// </summary>
    /// </summary>
    public static class JetBusCommands
    {
        static JetBusCommands()
        {
            ResidualFlowTime = new JetBusCommand(1, "2220/07", 0, 0);
            Ldw_dead_weight = new JetBusCommand(1, "2110/06", 0, 0);                // LDW = Nullpunkt
            Lwt_nominal_value = new JetBusCommand(1, "2110/07", 0, 0);              // LWT = Nennwert
            Lft_scale_calibration_weight = new JetBusCommand(1, "6152/00", 0, 0);   // LFT = LFT scale calibration weight

            Net_value = new JetBusCommand(1, "601A/01", 0, 0);
            Gross_value = new JetBusCommand(1, "6144/00", 0, 0);
            Zero_value = new JetBusCommand(1, "6142/00", 0, 0);

            Tare_value = new JetBusCommand(1, "6143/00", 0, 0);
            Weighing_device_1_weight_status = new JetBusCommand(1, "6012/01", 0, 0);

            WS_GeneralWeightError = new JetBusCommand(1, "6012/01", 0, 1);


            Unit_prefix_fixed_parameter = new JetBusCommand(1, "6014/01", 0, 0);
            Application_mode = new JetBusCommand(1, "2010/07", 0, 0); // IMD = Input mode ( Application mode)
            Decimals = new JetBusCommand(1, "6013/01", 0, 0);
            Scale_command = new JetBusCommand(1, "6002/01", 0, 0);
            Scale_command_status = new JetBusCommand(1, "6002/02", 0, 0);

            Status_digital_input_1 = new JetBusCommand(1, "2020/18", 0, 0);    // IS1
            Status_digital_input_2 = new JetBusCommand(1, "2020/19", 0, 0);    // IS2
            Status_digital_input_3 = new JetBusCommand(1, "2020/1A", 0, 0);    // IS3
            Status_digital_input_4 = new JetBusCommand(1, "2020/1B", 0, 0);    // IS4
            Status_digital_output_1 = new JetBusCommand(1, "2020/1E", 0, 0);   // OS1
            Status_digital_output_2 = new JetBusCommand(1, "2020/1F", 0, 0);   // OS2
            Status_digital_output_3 = new JetBusCommand(1, "2020/20", 0, 0);   // OS3
            Status_digital_output_4 = new JetBusCommand(1, "2020/21", 0, 0);   // OS4

            Limit_value = new JetBusCommand(1, "2020/25", 0, 0);   // LVS
            Coarse_flow_monitoring = new JetBusCommand(1, "2210/01", 0, 0);      // CBK = Füllstromüberwachung Grobstrom
            Coarse_flow_monitoring_time = new JetBusCommand(1, "2220/01", 0, 0); // CBT = Überwachungszeit Grobstrom
            Coarse_flow_cut_off_point = new JetBusCommand(1, "2210/02", 0, 0);   // CFD = Grobstromabschaltpunkt
            Coarse_flow_time = new JetBusCommand(1, "2230/01", 0, 0);            // CFT = Grobstromzeit
            Dosing_mode = new JetBusCommand(1, "2200/04", 0, 0);                 // DMD = Dosiermodus
            Dosing_time = new JetBusCommand(1, "2230/03", 0, 0);                 // DST = Dosieristzeit
            Emptying_mode = new JetBusCommand(1, "2200/05", 0, 0);               // EMD = Entleermodus
            Fine_flow_monitoring = new JetBusCommand(1, "2210/04", 0, 0);        // FBK = Füllstromüberwachung Feinstrom
            Fine_flow_monitoring_time = new JetBusCommand(1, "2220/03", 0, 0);   // FBT = Überwachungszeit Feinstrom
            Fine_flow_cut_off_point = new JetBusCommand(1, "2210/05", 0, 0);     // FFD = Feinstromabschaltpunkt
            Fine_flow_phase_before_coarse_flow = new JetBusCommand(1, "2220/0A", 0, 0); // FFL = Feinstromphase vor Grobstrom
            Minimum_fine_flow = new JetBusCommand(1, "2210/06", 0, 0);           // FFM = Minimaler Feinstromanteil
            Fine_flow_time = new JetBusCommand(1, "2230/04", 0, 0);              // FFT = Feinstromzeit
            Dosing_result = new JetBusCommand(1, "2000/05", 0, 0);               // FRS1 = Dosierergebnis
            dosing_state = new JetBusCommand(1, "2000/06", 0, 0);                // FRS2 = Dosierstatus
            Reference_value_dosing = new JetBusCommand(1, "2210/07", 0, 0);      // FWT = Sollwert dosieren
            Lockout_time_coarse_flow = new JetBusCommand(1, "2220/04", 0, 0);    // LTC = Sperrzeit Grobstrom
            Lockout_time_fine_flow = new JetBusCommand(1, "2220/05", 0, 0);      // LTF = Sperrzeit Feinstrom
            Lower_tolerance_limit = new JetBusCommand(1, "2210/08", 0, 0);       // LTL = Untere Toleranz
            Maximal_dosing_time = new JetBusCommand(1, "2220/06", 0, 0);         // MDT = Maximale Dosierzeit
            Minimum_start_weight = new JetBusCommand(1, "2210/0B", 0, 0);        // MSW = Minimum Startgewicht
            Dosing_counter = new JetBusCommand(1, "2230/05", 0, 0);              // NDS = Dosierzähler
            Optimization = new JetBusCommand(1, "2200/07", 0, 0);                // OSN = Optimierung
            Range_selection_parameter = new JetBusCommand(1, "2200/02", 0, 0);   // RDP = Auswahl Dosierparameter
            Redosing = new JetBusCommand(1, "2200/08", 0, 0);                    // RDS = Nachdosieren
            Residual_flow_time = new JetBusCommand(1, "2220/07", 0, 0);          // RFT = Nachstromzeit
            Run_start_dosing = new JetBusCommand(1, "2240/02", 0, 0);            // RUN = Start Dosieren
            Mean_value_dosing_results = new JetBusCommand(1, "2230/06", 0, 0);         // SDM = Mittelwert Dosieren
            Dosing_state_filler = new JetBusCommand(1, "2D00/02", 0, 0);               // SDO = Dosierstatus
            Standard_deviation = new JetBusCommand(1, "2230/07", 0, 0);                // SDS = Standardabweichung
            Settling_time_transient_response = new JetBusCommand(1, "2220/08", 0, 0);  // STT = Beruhigungszeit
            Systematic_difference = new JetBusCommand(1, "2210/09", 0, 0);             // SYD = Systematische Differenz
            Tare_delay = new JetBusCommand(1, "2220/09", 0, 0);                        // TAD = Tarierverzögerung
            Tare_mode = new JetBusCommand(1, "2200/0B", 0, 0);                         // TMD = Tariermodus
            Upper_tolerance_limit = new JetBusCommand(1, "2210/0A", 0, 0);             // UTL = Obere Toleranz
            Valve_control = new JetBusCommand(1, "2200/0C", 0, 0);               // VCT = Ventilsteuerung
            _write_dosing_parameter_set = new JetBusCommand(1, "2200/01", 0, 0);  // WDP = Dosierparametersatz schreiben
            Storage_weight = new JetBusCommand(1, "2040/05", 0, 0);              // STO = Gewichtsspeicherung
            Storage_weight_mode = new JetBusCommand(1, "2300/08", 0, 0);         // SMD = Modus Gewichtsspeicherung

            // Paths/Commands : Digital input in the extended filler mode, only via Jetbus
            Function_digital_input_1 = new JetBusCommand(1, "2022/01", 0, 0);
            Function_digital_input_2 = new JetBusCommand(1, "2022/02", 0, 0);
            Function_digital_input_3 = new JetBusCommand(1, "2022/03", 0, 0);
            Function_digital_input_4 = new JetBusCommand(1, "2022/04", 0, 0);
            // Paths/Commands : Digital output in the extended filler mode, only via Jetbus
            Function_digital_output_1 = new JetBusCommand(1, "2021/01", 0, 0);
            Function_digital_output_2 = new JetBusCommand(1, "2021/02", 0, 0);
            Function_digital_output_3 = new JetBusCommand(1, "2021/03", 0, 0);
            Function_digital_output_4 = new JetBusCommand(1, "2021/04", 0, 0);

            Error_register = new JetBusCommand(1, "1001/00", 0, 0);
            Save_all_parameters = new JetBusCommand(1, "1010/01", 0, 0);
            Restore_all_default_parameters = new JetBusCommand(1, "1011/01", 0, 0);
            Vendor_id = new JetBusCommand(1, "1018/01", 0, 0);
            Product_code = new JetBusCommand(1, "1018/02", 0, 0);
            Serial_number = new JetBusCommand(1, "1018/04", 0, 0);
            Implemented_profile_specification = new JetBusCommand(1, "1030/01", 0, 0);
            Lc_capability = new JetBusCommand(1, "6001/01", 0, 0);
            Weighing_device_1_unit_prefix_output_parameter = new JetBusCommand(1, "6015/01", 0, 0);

            Weighing_device_1_weight_step = new JetBusCommand(1, "6016/01", 0, 0);
            Alarms = new JetBusCommand(1, "6018/01", 0, 0);
            Weighing_device_1_output_weight = new JetBusCommand(1, "601A/01", 0, 0);
            Weighing_device_1_setting = new JetBusCommand(1, "6020/01", 0, 0);
            Local_gravity_factor = new JetBusCommand(1, "6021/01", 0, 0);
            Scale_filter_setup = new JetBusCommand(1, "6040/01", 0, 0);
            Data_sample_rate = new JetBusCommand(1, "6050/01", 0, 0);

            Filter_order_critically_damped = new JetBusCommand(1, "60A1/01", 0, 0);

            Cut_off_frequency_critically_damped = new JetBusCommand(1, "60A1/02", 0, 0);
            Filter_order_butterworth = new JetBusCommand(1, "60A1/01", 0, 0);
            Cut_off_frequency_butterworth = new JetBusCommand(1, "60A2/02", 0, 0);
            Filter_order_bessel = new JetBusCommand(1, "60B1/01", 0, 0);
            Cut_off_frequency_bessel = new JetBusCommand(1, "60B1/02", 0, 0);
            Scale_suppy_nominal_voltage = new JetBusCommand(1, "6110/01", 0, 0);
            Scale_suppy_minimum_voltage = new JetBusCommand(1, "6110/02", 0, 0);
            Scale_suppy_maximum_voltage = new JetBusCommand(1, "6110/03", 0, 0);

            Scale_accuracy_class = new JetBusCommand(1, "6111/01", 0, 0);
            Scale_minimum_dead_load = new JetBusCommand(1, "6112/01", 0, 0);
            Scale_maximum_capacity = new JetBusCommand(1, "6113/01", 0, 0);
            Scale_maximum_number_of_verification_interval = new JetBusCommand(1, "6114/01", 0, 0);
            Scale_apportionment_factor = new JetBusCommand(1, "6116/01", 0, 0);
            Scale_safe_load_limit = new JetBusCommand(1, "6117/01", 0, 0);
            Scale_operation_nominal_temperature = new JetBusCommand(1, "6118/01", 0, 0);
            Scale_operation_minimum_temperature = new JetBusCommand(1, "6118/02", 0, 0);
            Scale_operation_maximum_temperature = new JetBusCommand(1, "6118/03", 0, 0);
            Scale_relative_minimum_load_cell_verification_interval = new JetBusCommand(1, "611B/01", 0, 0);
            Interval_range_control = new JetBusCommand(1, "611C/01", 0, 0);
            Multi_limit_1 = new JetBusCommand(1, "611C/02", 0, 0);
            Multi_limit_2 = new JetBusCommand(1, "611C/03", 0, 0);
            Oiml_certificaiton_information = new JetBusCommand(1, "6138/01", 0, 0);
            Ntep_certificaiton_information = new JetBusCommand(1, "6138/02", 0, 0);
            Maximum_zeroing_time = new JetBusCommand(1, "6141/02", 0, 0);
            Maximum_peak_value_gross = new JetBusCommand(1, "6149/01", 0, 0);
            Minimum_peak_value_gross = new JetBusCommand(1, "6149/02", 0, 0);
            Maximum_peak_value = new JetBusCommand(1, "6149/03", 0, 0);
            Minimum_peak_value = new JetBusCommand(1, "6149/04", 0, 0);
            Weight_moving_detection = new JetBusCommand(1, "6153/00", 0, 0);
            Device_address = new JetBusCommand(1, "2600/00", 0, 0);

            Hardware_version = new JetBusCommand(1, "2520/0A", 0, 0); // = Hardware Variante
            Identification = new JetBusCommand(1, "2520/01", 0, 0);
            Limit_value_monitoring_liv11 = new JetBusCommand(1, "2030/01", 0, 0); // = Grenzwertüberwachung
            Signal_source_liv12 = new JetBusCommand(1, "2030/02", 0, 0);
            Switch_on_level_liv13 = new JetBusCommand(1, "2030/03", 0, 0);  // = Einschaltpegel
            Switch_off_level_liv14 = new JetBusCommand(1, "2030/04", 0, 0);  // = Ausschaltpegel
            Limit_value_monitoring_liv21 = new JetBusCommand(1, "2030/05", 0, 0);
            Signal_source_liv22 = new JetBusCommand(1, "2030/06", 0, 0);
            Switch_on_level_liv23 = new JetBusCommand(1, "2030/07", 0, 0);
            Switch_off_level_liv24 = new JetBusCommand(1, "2030/08", 0, 0);
            Limit_value_monitoring_liv31 = new JetBusCommand(1, "2030/09", 0, 0);
            Signal_source_liv32 = new JetBusCommand(1, "2030/0A", 0, 0);
            Switch_on_level_liv33 = new JetBusCommand(1, "2030/0B", 0, 0);
            Switch_off_level_liv34 = new JetBusCommand(1, "2030/0C", 0, 0);
            Limit_value_monitoring_liv41 = new JetBusCommand(1, "2030/0D", 0, 0);
            Signal_source_liv42 = new JetBusCommand(1, "2030/0E", 0, 0);
            Switch_on_level_liv43 = new JetBusCommand(1, "2030/0F", 0, 0);
            Switch_off_level_liv44 = new JetBusCommand(1, "2030/10", 0, 0);
            Output_scale = new JetBusCommand(1, "2110/0A", 0, 0);
            Firmware_date = new JetBusCommand(1, "2520/05", 0, 0);
            Reset_trigger = new JetBusCommand(1, "2D00/04", 0, 0);
            State_digital_io_extended = new JetBusCommand(1, "2020/12", 0, 0);  //Zustand Digital-IO(erweitert)
            Software_identification = new JetBusCommand(1, "2600/22", 0, 0);
            Software_version = new JetBusCommand(1, "2600/16", 0, 0);
            Date_time = new JetBusCommand(1, "2E00/02", 0, 0);

            Break_dosing = new JetBusCommand(1, "2240/01", 0, 0);                // BRK = Abbruch Dosierung
            Delete_dosing_result = new JetBusCommand(1, "2230/02", 0, 0);        // CSN = Löschen Dosierergebniss
            Material_stream_last_dosing = new JetBusCommand(1, "2000/0E", 0, 0); // MFO = Materialstrom des letzten Dosierzyklus
            Sum = new JetBusCommand(1, "2230/08", 0, 0);                         // SUM = Summe
            Special_dosing_functions = new JetBusCommand(1, "2200/0A", 0, 0);    // SDF = Sonderfunktionen
            Discharge_time = new JetBusCommand(1, "2220/02", 0, 0);              // EPT = Entleerzeit
            Exceeding_weight_break = new JetBusCommand(1, "2200/0F", 0, 0);      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
            Delay1_dosing = new JetBusCommand(1, "2220/0B", 0, 0);               // DL1 = Delay 1 für Dosieren
            Delay2_dosing = new JetBusCommand(1, "2220/0C", 0, 0);               // DL2 = Delay 2 für Dosieren
            Empty_weight_tolerance = new JetBusCommand(1, "2210/03", 0, 0);      // EWT = Entleertoleranz
            Residual_flow_dosing_cycle = new JetBusCommand(1, "2000/0F", 0, 0);  // RFO = Nachstrom des letzten Dosierzyklus
        }

        public static JetBusCommand ResidualFlowTime { get; private set; }
        public static JetBusCommand Ldw_dead_weight { get; private set; }                // LDW = Nullpunkt
        public static JetBusCommand Lwt_nominal_value { get; private set; }              // LWT = Nennwert
        public static JetBusCommand Lft_scale_calibration_weight { get; private set; }   // LFT = LFT scale calibration weight

        public static JetBusCommand Net_value { get; private set; }
        public static JetBusCommand Gross_value { get; private set; }
        public static JetBusCommand Zero_value { get; private set; }

        public static JetBusCommand Tare_value { get; private set; }
        public static JetBusCommand Weighing_device_1_weight_status { get; private set; }
        public static JetBusCommand WS_GeneralWeightError { get; private set; }
        public static JetBusCommand Unit_prefix_fixed_parameter { get; private set; }

        public static JetBusCommand Application_mode { get; private set; } // IMD = Input mode ( Application mode)

        public static JetBusCommand Decimals { get; private set; }
        public static JetBusCommand Scale_command { get; private set; }
        public static JetBusCommand Scale_command_status { get; private set; }

        public static JetBusCommand Status_digital_input_1 { get; private set; }    // IS1
        public static JetBusCommand Status_digital_input_2 { get; private set; }    // IS2
        public static JetBusCommand Status_digital_input_3 { get; private set; }    // IS3
        public static JetBusCommand Status_digital_input_4 { get; private set; }    // IS4

        public static JetBusCommand Status_digital_output_1 { get; private set; }   // OS1
        public static JetBusCommand Status_digital_output_2 { get; private set; }   // OS2
        public static JetBusCommand Status_digital_output_3 { get; private set; }   // OS3
        public static JetBusCommand Status_digital_output_4 { get; private set; }   // OS4

        public static JetBusCommand Limit_value { get; private set; }   // LVS

        //private string[] _weightMemArray = new string[2] { _storage_weight, _storage_weight_mode };

        public static JetBusCommand Coarse_flow_monitoring { get; private set; }      // CBK = Füllstromüberwachung Grobstrom
        public static JetBusCommand Coarse_flow_monitoring_time { get; private set; } // CBT = Überwachungszeit Grobstrom
        public static JetBusCommand Coarse_flow_cut_off_point { get; private set; }   // CFD = Grobstromabschaltpunkt
        public static JetBusCommand Coarse_flow_time { get; private set; }            // CFT = Grobstromzeit
        public static JetBusCommand Dosing_mode { get; private set; }                 // DMD = Dosiermodus
        public static JetBusCommand Dosing_time { get; private set; }                 // DST = Dosieristzeit
        public static JetBusCommand Emptying_mode { get; private set; }               // EMD = Entleermodus

        public static JetBusCommand Fine_flow_monitoring { get; private set; }        // FBK = Füllstromüberwachung Feinstrom
        public static JetBusCommand Fine_flow_monitoring_time { get; private set; }   // FBT = Überwachungszeit Feinstrom
        public static JetBusCommand Fine_flow_cut_off_point { get; private set; }     // FFD = Feinstromabschaltpunkt
        public static JetBusCommand Fine_flow_phase_before_coarse_flow { get; private set; } // FFL = Feinstromphase vor Grobstrom
        public static JetBusCommand Minimum_fine_flow { get; private set; }           // FFM = Minimaler Feinstromanteil

        public static JetBusCommand Fine_flow_time { get; private set; }              // FFT = Feinstromzeit
        public static JetBusCommand Dosing_result { get; private set; }               // FRS1 = Dosierergebnis
        public static JetBusCommand dosing_state { get; private set; }                // FRS2 = Dosierstatus
        public static JetBusCommand Reference_value_dosing { get; private set; }      // FWT = Sollwert dosieren
        public static JetBusCommand Lockout_time_coarse_flow { get; private set; }    // LTC = Sperrzeit Grobstrom
        public static JetBusCommand Lockout_time_fine_flow { get; private set; }      // LTF = Sperrzeit Feinstrom
        public static JetBusCommand Lower_tolerance_limit { get; private set; }       // LTL = Untere Toleranz
        public static JetBusCommand Maximal_dosing_time { get; private set; }         // MDT = Maximale Dosierzeit
        public static JetBusCommand Minimum_start_weight { get; private set; }        // MSW = Minimum Startgewicht
        public static JetBusCommand Dosing_counter { get; private set; }              // NDS = Dosierzähler
        public static JetBusCommand Optimization { get; private set; }                // OSN = Optimierung
        public static JetBusCommand Range_selection_parameter { get; private set; }   // RDP = Auswahl Dosierparameter
        public static JetBusCommand Redosing { get; private set; }                    // RDS = Nachdosieren
        public static JetBusCommand Residual_flow_time { get; private set; }          // RFT = Nachstromzeit
        public static JetBusCommand Run_start_dosing { get; private set; }            // RUN = Start Dosieren

        public static JetBusCommand Mean_value_dosing_results { get; private set; }         // SDM = Mittelwert Dosieren
        public static JetBusCommand Dosing_state_filler { get; private set; }               // SDO = Dosierstatus
        public static JetBusCommand Standard_deviation { get; private set; }                // SDS = Standardabweichung
        public static JetBusCommand Settling_time_transient_response { get; private set; }  // STT = Beruhigungszeit
        public static JetBusCommand Systematic_difference { get; private set; }             // SYD = Systematische Differenz
        public static JetBusCommand Tare_delay { get; private set; }                        // TAD = Tarierverzögerung
        public static JetBusCommand Tare_mode { get; private set; }                         // TMD = Tariermodus
        public static JetBusCommand Upper_tolerance_limit { get; private set; }             // UTL = Obere Toleranz
        public static JetBusCommand Valve_control { get; private set; }                    // VCT = Ventilsteuerung
        public static JetBusCommand _write_dosing_parameter_set { get; private set; }      // WDP = Dosierparametersatz schreiben
        public static JetBusCommand Storage_weight { get; private set; }                   // STO = Gewichtsspeicherung
        public static JetBusCommand Storage_weight_mode { get; private set; }              // SMD = Modus Gewichtsspeicherung



        // Paths/Commands : Digital input in the extended filler mode, only via Jetbus
        public static JetBusCommand Function_digital_input_1 { get; private set; }
        public static JetBusCommand Function_digital_input_2 { get; private set; }
        public static JetBusCommand Function_digital_input_3 { get; private set; }
        public static JetBusCommand Function_digital_input_4 { get; private set; }
        // Paths/Commands : Digital output in the extended filler mode, only via Jetbus
        public static JetBusCommand Function_digital_output_1 { get; private set; }
        public static JetBusCommand Function_digital_output_2 { get; private set; }
        public static JetBusCommand Function_digital_output_3 { get; private set; }
        public static JetBusCommand Function_digital_output_4 { get; private set; }

        public static JetBusCommand Error_register { get; private set; }
        public static JetBusCommand Save_all_parameters { get; private set; }
        public static JetBusCommand Restore_all_default_parameters { get; private set; }
        public static JetBusCommand Vendor_id { get; private set; }
        public static JetBusCommand Product_code { get; private set; }
        public static JetBusCommand Serial_number { get; private set; }
        public static JetBusCommand Implemented_profile_specification { get; private set; }
        public static JetBusCommand Lc_capability { get; private set; }
        public static JetBusCommand Weighing_device_1_unit_prefix_output_parameter { get; private set; }

        public static JetBusCommand Weighing_device_1_weight_step { get; private set; }
        public static JetBusCommand Alarms { get; private set; }
        public static JetBusCommand Weighing_device_1_output_weight { get; private set; }
        public static JetBusCommand Weighing_device_1_setting { get; private set; }
        public static JetBusCommand Local_gravity_factor { get; private set; }
        public static JetBusCommand Scale_filter_setup { get; private set; }
        public static JetBusCommand Data_sample_rate { get; private set; }

        public static JetBusCommand Filter_order_critically_damped { get; private set; }

        public static JetBusCommand Cut_off_frequency_critically_damped { get; private set; }
        public static JetBusCommand Filter_order_butterworth { get; private set; }
        public static JetBusCommand Cut_off_frequency_butterworth { get; private set; }
        public static JetBusCommand Filter_order_bessel { get; private set; }
        public static JetBusCommand Cut_off_frequency_bessel { get; private set; }
        public static JetBusCommand Scale_suppy_nominal_voltage { get; private set; }
        public static JetBusCommand Scale_suppy_minimum_voltage { get; private set; }
        public static JetBusCommand Scale_suppy_maximum_voltage { get; private set; }

        public static JetBusCommand Scale_accuracy_class { get; private set; }
        public static JetBusCommand Scale_minimum_dead_load { get; private set; }
        public static JetBusCommand Scale_maximum_capacity { get; private set; }
        public static JetBusCommand Scale_maximum_number_of_verification_interval { get; private set; }
        public static JetBusCommand Scale_apportionment_factor { get; private set; }
        public static JetBusCommand Scale_safe_load_limit { get; private set; }
        public static JetBusCommand Scale_operation_nominal_temperature { get; private set; }
        public static JetBusCommand Scale_operation_minimum_temperature { get; private set; }
        public static JetBusCommand Scale_operation_maximum_temperature { get; private set; }
        public static JetBusCommand Scale_relative_minimum_load_cell_verification_interval { get; private set; }
        public static JetBusCommand Interval_range_control { get; private set; }
        public static JetBusCommand Multi_limit_1 { get; private set; }
        public static JetBusCommand Multi_limit_2 { get; private set; }
        public static JetBusCommand Oiml_certificaiton_information { get; private set; }
        public static JetBusCommand Ntep_certificaiton_information { get; private set; }
        public static JetBusCommand Maximum_zeroing_time { get; private set; }
        public static JetBusCommand Maximum_peak_value_gross { get; private set; }
        public static JetBusCommand Minimum_peak_value_gross { get; private set; }
        public static JetBusCommand Maximum_peak_value { get; private set; }
        public static JetBusCommand Minimum_peak_value { get; private set; }
        public static JetBusCommand Weight_moving_detection { get; private set; }
        public static JetBusCommand Device_address { get; private set; }

        public static JetBusCommand Hardware_version { get; private set; } // = Hardware Variante
        public static JetBusCommand Identification { get; private set; }
        public static JetBusCommand Limit_value_monitoring_liv11 { get; private set; } // = Grenzwertüberwachung
        public static JetBusCommand Signal_source_liv12 { get; private set; }
        public static JetBusCommand Switch_on_level_liv13 { get; private set; }  // = Einschaltpegel
        public static JetBusCommand Switch_off_level_liv14 { get; private set; }  // = Ausschaltpegel
        public static JetBusCommand Limit_value_monitoring_liv21 { get; private set; }
        public static JetBusCommand Signal_source_liv22 { get; private set; }
        public static JetBusCommand Switch_on_level_liv23 { get; private set; }
        public static JetBusCommand Switch_off_level_liv24 { get; private set; }
        public static JetBusCommand Limit_value_monitoring_liv31 { get; private set; }
        public static JetBusCommand Signal_source_liv32 { get; private set; }
        public static JetBusCommand Switch_on_level_liv33 { get; private set; }
        public static JetBusCommand Switch_off_level_liv34 { get; private set; }
        public static JetBusCommand Limit_value_monitoring_liv41 { get; private set; }
        public static JetBusCommand Signal_source_liv42 { get; private set; }
        public static JetBusCommand Switch_on_level_liv43 { get; private set; }
        public static JetBusCommand Switch_off_level_liv44 { get; private set; }
        public static JetBusCommand Output_scale { get; private set; }
        public static JetBusCommand Firmware_date { get; private set; }
        public static JetBusCommand Reset_trigger { get; private set; }
        public static JetBusCommand State_digital_io_extended { get; private set; }  //Zustand Digital-IO(erweitert)
        public static JetBusCommand Software_identification { get; private set; }
        public static JetBusCommand Software_version { get; private set; }
        public static JetBusCommand Date_time { get; private set; }

        public static JetBusCommand Break_dosing { get; private set; }                // BRK = Abbruch Dosierung
        public static JetBusCommand Delete_dosing_result { get; private set; }        // CSN = Löschen Dosierergebniss
        public static JetBusCommand Material_stream_last_dosing { get; private set; } // MFO = Materialstrom des letzten Dosierzyklus
        public static JetBusCommand Sum { get; private set; }                         // SUM = Summe
        public static JetBusCommand Special_dosing_functions { get; private set; }    // SDF = Sonderfunktionen
        public static JetBusCommand Discharge_time { get; private set; }              // EPT = Entleerzeit
        public static JetBusCommand Exceeding_weight_break { get; private set; }      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
        public static JetBusCommand Delay1_dosing { get; private set; }               // DL1 = Delay 1 für Dosieren
        public static JetBusCommand Delay2_dosing { get; private set; }               // DL2 = Delay 2 für Dosieren
        public static JetBusCommand Empty_weight_tolerance { get; private set; }      // EWT = Entleertoleranz
        public static JetBusCommand Residual_flow_dosing_cycle { get; private set; }  // RFO = Nachstrom des letzten Dosierzyklus


        // Missing IDs (so far : Modbus only - look at ModbusCommands)
        public static JetBusCommand ADC_OVER_UNDERLOAD { get; private set; }
        public static JetBusCommand LEGAL_FOR_TRADE_OPERATION { get; private set; }
        public static JetBusCommand STATUS_INPUT_1 { get; private set; }
        public static JetBusCommand GENERAL_SCALE_ERROR { get; private set; }
        public static JetBusCommand COARSE_FLOW { get; private set; }
        public static JetBusCommand FINE_FLOW { get; private set; }
        public static JetBusCommand READY { get; private set; }
        public static JetBusCommand EMPTYING { get; private set; }
        public static JetBusCommand FLOW_ERROR { get; private set; }
        public static JetBusCommand ALARM { get; private set; }
        public static JetBusCommand TOLERANCE_ERROR_PLUS { get; private set; }
        public static JetBusCommand TOLERANCE_ERROR_MINUS { get; private set; }
        public static JetBusCommand CURRENT_DOSING_TIME { get; private set; }
        public static JetBusCommand CURRENT_COARSE_FLOW_TIME { get; private set; }
        public static JetBusCommand CURRENT_FINE_FLOW_TIME { get; private set; }
        public static JetBusCommand PARAMETER_SET_PRODUCT { get; private set; }
        public static JetBusCommand DELAY_TIME_AFTER_FINE_FLOW { get; private set; }
        public static JetBusCommand ACTIVATION_TIME_AFTER_FINE_FLOW { get; private set; }
        public static JetBusCommand DOWNWARDS_DOSING { get; private set; }
        public static JetBusCommand TOTAL_WEIGHT { get; private set; }
        public static JetBusCommand TARGET_FILLING_WEIGHT { get; private set; }
        public static JetBusCommand COARSE_FLOW_CUT_OFF_POINT_SET { get; private set; }
        public static JetBusCommand FINE_FLOW_CUT_OFF_POINT_SET { get; private set; }
        public static JetBusCommand START_WITH_FINE_FLOW { get; private set; }
    }
}
