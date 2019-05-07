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
    public class JetBusCommands : ICommands
    {
        public JetBusCommands()
        {
            ResidualFlowTime = new JetBusCommand(1, "2220/07", 0, 0);
        }

        public JetBusCommand ResidualFlowTime { get; private set; }

        #region ID Commands : Maintenance - Calibration

        private const string _ldw_dead_weight = "2110/06";                // LDW = Nullpunkt
        private const string _lwt_nominal_value = "2110/07";              // LWT = Nennwert
        private const string _lft_scale_calibration_weight = "6152/00";   // LFT = LFT scale calibration weight

        #endregion

        #region ID commands for process data
        private const string _net_value = "601A/01";
        private const string _gross_value = "6144/00";
        private const string _zero_value = "6142/00";

        private const string _tare_value = "6143/00";
        private const string _weighing_device_1_weight_status = "6012/01";
        private const string _unit_prefix_fixed_parameter = "6014/01";

        private const string _application_mode = "2010/07"; // IMD = Input mode ( Application mode)

        private const string _decimals = "6013/01";
        private const string _scale_command = "6002/01";
        private const string _scale_command_status = "6002/02";
        #endregion

        #region ID commands for standard mode
        private const string _status_digital_input_1 = "2020/18";    // IS1
        private const string _status_digital_input_2 = "2020/19";    // IS2
        private const string _status_digital_input_3 = "2020/1A";    // IS3
        private const string _status_digital_input_4 = "2020/1B";    // IS4

        private const string _status_digital_output_1 = "2020/1E";   // OS1
        private const string _status_digital_output_2 = "2020/1F";   // OS2
        private const string _status_digital_output_3 = "2020/20";   // OS3
        private const string _status_digital_output_4 = "2020/21";   // OS4

        private const string _limit_value = "2020/25";   // LVS

        private string[] _weightMemArray = new string[2] { _storage_weight, _storage_weight_mode };

        #endregion

        #region ID commands for filler data

        private const string _coarse_flow_monitoring = "2210/01";      // CBK = Füllstromüberwachung Grobstrom
        private const string _coarse_flow_monitoring_time = "2220/01"; // CBT = Überwachungszeit Grobstrom
        private const string _coarse_flow_cut_off_point = "2210/02";   // CFD = Grobstromabschaltpunkt
        private const string _coarse_flow_time = "2230/01";            // CFT = Grobstromzeit
        private const string _dosing_mode = "2200/04";                 // DMD = Dosiermodus
        private const string _dosing_time = "2230/03";                 // DST = Dosieristzeit
        private const string _emptying_mode = "2200/05";               // EMD = Entleermodus

        private const string _fine_flow_monitoring = "2210/04";        // FBK = Füllstromüberwachung Feinstrom
        private const string _fine_flow_monitoring_time = "2220/03";   // FBT = Überwachungszeit Feinstrom
        private const string _fine_flow_cut_off_point = "2210/05";     // FFD = Feinstromabschaltpunkt
        private const string _fine_flow_phase_before_coarse_flow = "2220/0A"; // FFL = Feinstromphase vor Grobstrom
        private const string _minimum_fine_flow = "2210/06";           // FFM = Minimaler Feinstromanteil

        private const string _fine_flow_time = "2230/04";              // FFT = Feinstromzeit
        private const string _dosing_result = "2000/05";               // FRS1 = Dosierergebnis
        private const string _dosing_state = "2000/06";                // FRS2 = Dosierstatus
        private const string _reference_value_dosing = "2210/07";      // FWT = Sollwert dosieren
        private const string _lockout_time_coarse_flow = "2220/04";    // LTC = Sperrzeit Grobstrom
        private const string _lockout_time_fine_flow = "2220/05";      // LTF = Sperrzeit Feinstrom
        private const string _lower_tolerance_lomit = "2210/08";       // LTL = Untere Toleranz
        private const string _maximal_dosing_time = "2220/06";         // MDT = Maximale Dosierzeit
        private const string _minimum_start_weight = "2210/0B";        // MSW = Minimum Startgewicht
        private const string _dosing_counter = "2230/05";              // NDS = Dosierzähler
        private const string _optimization = "2200/07";                // OSN = Optimierung
        private const string _range_selection_parameter = "2200/02";   // RDP = Auswahl Dosierparameter

        private const string _redosing = "2200/08";                    // RDS = Nachdosieren
        private const string _residual_flow_time = "2220/07";          // RFT = Nachstromzeit
        private const string _run_start_dosing = "2240/02";            // RUN = Start Dosieren

        private const string _mean_value_dosing_results = "2230/06";         // SDM = Mittelwert Dosieren
        private const string _dosing_state_filler = "2D00/02";               // SDO = Dosierstatus
        private const string _standard_deviation = "2230/07";                // SDS = Standardabweichung
        private const string _settling_time_transient_response = "2220/08";  // STT = Beruhigungszeit
        private const string _systematic_difference = "2210/09";             // SYD = Systematische Differenz
        private const string _tare_delay = "2220/09";                        // TAD = Tarierverzögerung
        private const string _tare_mode = "2200/0B";                         // TMD = Tariermodus
        private const string _upper_tolerance_limit = "2210/0A";             // UTL = Obere Toleranz

        private const string _valve_control = "2200/0C";               // VCT = Ventilsteuerung
        private const string _write_dosing_parameter_set = "2200/01";  // WDP = Dosierparametersatz schreiben

        private const string _storage_weight = "2040/05";              // STO = Gewichtsspeicherung
        private const string _storage_weight_mode = "2300/08";         // SMD = Modus Gewichtsspeicherung

        #endregion

        #region ID commands for filler extended data

        #endregion

        #region All other ID commands for Operator, Administrator and Maintenance : 

        // Paths/Commands : Digital input in the extended filler mode, only via Jetbus
        private const string _function_digital_input_1 = "2022/01";
        private const string _function_digital_input_2 = "2022/02";
        private const string _function_digital_input_3 = "2022/03";
        private const string _function_digital_input_4 = "2022/04";
        // Paths/Commands : Digital output in the extended filler mode, only via Jetbus
        private const string _function_digital_output_1 = "2021/01";
        private const string _function_digital_output_2 = "2021/02";
        private const string _function_digital_output_3 = "2021/03";
        private const string _function_digital_output_4 = "2021/04";

        private const string _error_register = "1001/00";
        private const string _save_all_parameters = "1010/01";
        private const string _restore_all_default_parameters ="1011/01";
        private const string _vendor_id ="1018/01";
        private const string _product_code ="1018/02";
        private const string _serial_number ="1018/04";
        private const string _implemented_profile_specification = "1030/01";
        private const string _lc_capability ="6001/01";
        private const string _weighing_device_1_unit_prefix_output_parameter ="6015/01";

        private const string _weighing_device_1_weight_step = "6016/01";
        private const string _alarms = "6018/01";
        private const string _weighing_device_1_output_weight = "601A/01";
        private const string _weighing_device_1_setting = "6020/01";
        private const string _local_gravity_factor = "6021/01";
        private const string _scale_filter_setup = "6040/01";
        private const string _data_sample_rate = "6050/01";
        private const string _filter_order_critically_damped ="60A1/01";
        private const string _cut_off_frequency_critically_damped = "60A1/02";
        private const string _filter_order_butterworth = "60A1/01";
        private const string _cut_off_frequency_butterworth = "60A2/02";
        private const string _filter_order_bessel = "60B1/01";
        private const string _cut_off_frequency_bessel = "60B1/02";
        private const string _scale_suppy_nominal_voltage = "6110/01";
        private const string _scale_suppy_minimum_voltage = "6110/02";
        private const string _scale_suppy_maximum_voltage = "6110/03";
        private const string _scale_accuracy_class ="6111/01";
        private const string _scale_minimum_dead_load ="6112/01";
        private const string _scale_maximum_capacity = "6113/01";
        private const string _scale_maximum_number_of_verification_interval = "6114/01";
        private const string _scale_apportionment_factor = "6116/01";
        private const string _scale_safe_load_limit ="6117/01";
        private const string _scale_operation_nominal_temperature = "6118/01";
        private const string _scale_operation_minimum_temperature = "6118/02";
        private const string _scale_operation_maximum_temperature = "6118/03";
        private const string _scale_relative_minimum_load_cell_verification_interval = "611B/01";     
        private const string _interval_range_control = "611C/01";
        private const string _multi_limit_1 = "611C/02";
        private const string _multi_limit_2 = "611C/03";
        private const string _oiml_certificaiton_information = "6138/01";
        private const string _ntep_certificaiton_information = "6138/02";
        private const string _maximum_zeroing_time = "6141/02";
        private const string _maximum_peak_value_gross = "6149/01";
        private const string _minimum_peak_value_gross = "6149/02";
        private const string _maximum_peak_value = "6149/03";
        private const string _minimum_peak_value = "6149/04";
        private const string _weight_moving_detection = "6153/00";
        private const string _device_address = "2600/00";

        private const string _hardware_version = "2520/0A"; // = Hardware Variante
        private const string _identification = "2520/01";
        private const string _limit_value_monitoring_liv11 = "2030/01"; // = Grenzwertüberwachung
        private const string _signal_source_liv12 = "2030/02";
        private const string _switch_on_level_liv13 =  "2030/03";  // = Einschaltpegel
        private const string _switch_off_level_liv14 = "2030/04";  // = Ausschaltpegel
        private const string _limit_value_monitoring_liv21 = "2030/05";
        private const string _signal_source_liv22 = "2030/06";
        private const string _switch_on_level_liv23 = "2030/07";
        private const string _switch_off_level_liv24 = "2030/08";
        private const string _limit_value_monitoring_liv31 = "2030/09";
        private const string _signal_source_liv32 = "2030/0A";
        private const string _switch_on_level_liv33 = "2030/0B";
        private const string _switch_off_level_liv34 = "2030/0C";
        private const string _limit_value_monitoring_liv41 = "2030/0D";
        private const string _signal_source_liv42 = "2030/0E";
        private const string _switch_on_level_liv43 = "2030/0F";
        private const string _switch_off_level_liv44 = "2030/10";
        private const string _output_scale = "2110/0A";
        private const string _firmware_date = "2520/05";
        private const string _reset_trigger = "2D00/04";
        private const string _state_digital_io_extended = "2020/12";  //Zustand Digital-IO(erweitert)
        private const string _software_identification = "2600/22";
        private const string _software_version = "2600/16";
        private const string _date_time = "2E00/02";

        private const string _break_dosing = "2240/01";                // BRK = Abbruch Dosierung
        private const string _delete_dosing_result = "2230/02";        // CSN = Löschen Dosierergebniss
        private const string _material_stream_last_dosing = "2000/0E"; // MFO = Materialstrom des letzten Dosierzyklus
        private const string _sum = "2230/08";                         // SUM = Summe
        private const string _special_dosing_functions = "2200/0A";    // SDF = Sonderfunktionen
        private const string _discharge_time = "2220/02";              // EPT = Entleerzeit
        private const string _exceeding_weight_break = "2200/0F";      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
        private const string _delay1_dosing = "2220/0B";               // DL1 = Delay 1 für Dosieren
        private const string _delay2_dosing = "2220/0C";               // DL2 = Delay 2 für Dosieren
        private const string _empty_weight_tolerance = "2210/03";      // EWT = Entleertoleranz
        private const string _residual_flow_dosing_cycle = "2000/0F";  // RFO = Nachstrom des letzten Dosierzyklus

        #endregion

        #region Get properties of all ID commands

        public string[] WEIGHT_MEMORY_STANDARD
        {
            get
            {
                return _weightMemArray;
            }
        }

        public string[] WEIGHT_MEMORY_FILLER
        {
            get
            {
                return _weightMemArray;
            }
        }

        string ICommands.LDW_DEAD_WEIGHT
        {
            get { return _ldw_dead_weight; }
        }

        string ICommands.LWT_NOMINAL_VALUE
        {
            get { return _lwt_nominal_value; }
        }

        string ICommands.LFT_SCALE_CALIBRATION_WEIGHT
        {
            get { return _lft_scale_calibration_weight; }
        }

        string ICommands.NET_VALUE
        {
            get { return _net_value; }
        }

        string ICommands.GROSS_VALUE
        {
            get { return _gross_value; }
        }

        string ICommands.ZERO_VALUE
        {
            get { return _zero_value; }
        }

        string ICommands.TARE_VALUE
        {
            get { return _tare_value; }
        }

        string ICommands.WEIGHING_DEVICE_1_WEIGHT_STATUS
        {
            get { return _weighing_device_1_weight_status; }
        }

        string ICommands.UNIT_PREFIX_FIXED_PARAMETER
        {
            get { return _unit_prefix_fixed_parameter; }
        }

        string ICommands.APPLICATION_MODE
        {
            get { return _application_mode; }
        }

        string ICommands.DECIMALS
        {
            get { return _decimals; }
        }

        string ICommands.SCALE_COMMAND
        {
            get { return _scale_command; }
        }

        string ICommands.SCALE_COMMAND_STATUS
        {
            get { return _scale_command_status; }
        }

        string ICommands.STATUS_DIGITAL_INPUT_1
        {
            get { return _status_digital_input_1; }
        }

        string ICommands.STATUS_DIGITAL_INPUT_2
        {
            get { return _status_digital_input_2; }
        }

        string ICommands.STATUS_DIGITAL_INPUT_3
        {
            get { return _status_digital_input_3; }
        }

        string ICommands.STATUS_DIGITAL_INPUT_4
        {
            get { return _status_digital_input_4; }
        }

        string ICommands.STATUS_DIGITAL_OUTPUT_1
        {
            get { return _status_digital_output_1; }
        }

        string ICommands.STATUS_DIGITAL_OUTPUT_2
        {
            get { return _status_digital_output_2; }
        }

        string ICommands.STATUS_DIGITAL_OUTPUT_3
        {
            get { return _status_digital_output_3; }
        }

        string ICommands.STATUS_DIGITAL_OUTPUT_4
        {
            get { return _status_digital_output_4; }
        }

        string ICommands.LIMIT_VALUE
        {
            get { return _limit_value; }
        }

        public string COARSE_FLOW_MONITORING
        {
            get { return _coarse_flow_monitoring; }
        }

        public string COARSE_FLOW_MONITORING_TIME
        {
            get { return _coarse_flow_monitoring_time; }
        }

        public string COARSE_FLOW_CUT_OFF_POINT
        {
            get { return _coarse_flow_cut_off_point; }
        }

        public string COARSE_FLOW_TIME
        {
            get { return _coarse_flow_time; }
        }

        public string DOSING_MODE
        {
            get { return _dosing_mode; }
        }

        public string DOSING_TIME
        {
            get { return _dosing_time; }
        }

        public string EMPTYING_MODE
        {
            get { return _emptying_mode; }
        }

        public string FINE_FLOW_MONITORING
        {
            get { return _fine_flow_monitoring; }
        }

        public string FINE_FLOW_MONITORING_TIME
        {
            get { return _fine_flow_monitoring_time; }
        }

        public string FINE_FLOW_CUT_OFF_POINT
        {
            get { return _fine_flow_cut_off_point; }
        }

        public string FINE_FLOW_PHASE_BEFORE_COARSE_FLOW
        {
            get { return _fine_flow_phase_before_coarse_flow; }
        }

        public string MINIMUM_FINE_FLOW
        {
            get { return _minimum_fine_flow; }
        }

        public string FINE_FLOW_TIME
        {
            get { return _fine_flow_time; }
        }

        public string DOSING_RESULT
        {
            get { return _dosing_result; }
        }

        public string DOSING_STATE
        {
            get { return _dosing_state; }
        }

        public string REFERENCE_VALUE_DOSING
        {
            get { return _reference_value_dosing; }
        }

        public string LOCKOUT_TIME_COARSE_FLOW
        {
            get { return _lockout_time_coarse_flow; }
        }

        public string LOCKOUT_TIME_FINE_FLOW
        {
            get { return _lockout_time_fine_flow; }
        }

        public string LOWER_TOLERANCE_LIMIT
        {
            get { return _lower_tolerance_lomit; }
        }

        public string MAXIMAL_DOSING_TIME
        {
            get { return _maximal_dosing_time; }
        }

        public string MINIMUM_START_WEIGHT
        {
            get { return _minimum_start_weight; }
        }

        public string DOSING_COUNTER
        {
            get { return _dosing_counter; }
        }

        public string OPTIMIZATION
        {
            get { return _optimization; }
        }

        public string RANGE_SELECTION_PARAMETER
        {
            get { return _range_selection_parameter; }
        }

        public string REDOSING
        {
            get { return _redosing; }
        }

        public string RESIDUAL_FLOW_TIME
        {
            get { return _residual_flow_time; }
        }

        public string RUN_START_DOSING
        {
            get { return _run_start_dosing; }
        }

        public string MEAN_VALUE_DOSING_RESULTS
        {
            get { return _mean_value_dosing_results; }
        }

        public string DOSING_STATE_FILLER
        {
            get { return _dosing_state_filler; }
        }

        public string STANDARD_DEVIATION
        {
            get { return _standard_deviation; }
        }

        public string SETTLING_TIME_TRANSIENT_RESPONSE
        {
            get { return _settling_time_transient_response; }
        }

        public string SYSTEMATIC_DIFFERENCE
        {
            get { return _systematic_difference; }
        }

        public string TARE_DELAY
        {
            get { return _tare_delay; }
        }

        public string TARE_MODE
        {
            get { return _tare_mode; }
        }

        public string UPPER_TOLERANCE_LIMIT
        {
            get { return _upper_tolerance_limit; }
        }

        public string VALVE_CONTROL
        {
            get { return _valve_control; }
        }

        public string WRITE_DOSING_PARAMETER_SET
        {
            get { return _write_dosing_parameter_set; }
        }

        public string STORAGE_WEIGHT
        {
            get { return _storage_weight; }
        }

        public string STORAGE_WEIGHT_MODE
        {
            get { return _storage_weight_mode; }
        }


        public string FUNCTION_DIGITAL_INPUT_1
        {
            get { return _function_digital_input_1; }
        }
        public string FUNCTION_DIGITAL_INPUT_2
        {
            get { return _function_digital_input_2; }
        }
        public string FUNCTION_DIGITAL_INPUT_3
        {
            get { return _function_digital_input_3; }
        }
        public string FUNCTION_DIGITAL_INPUT_4
        {
            get { return _function_digital_input_4; }
        }
        public string FUNCTION_DIGITAL_OUTPUT_1
        {
            get { return _function_digital_output_1; }
        }
        public string FUNCTION_DIGITAL_OUTPUT_2
        {
            get { return _function_digital_output_2; }
        }
        public string FUNCTION_DIGITAL_OUTPUT_3
        {
            get { return _function_digital_output_3; }
        }
        public string FUNCTION_DIGITAL_OUTPUT_4
        {
            get { return _function_digital_output_4; }
        }

        public string ERROR_REGISTER
        {
            get { return _error_register; }
        }

        public string SAVE_ALL_PARAMETERS
        {
            get { return _save_all_parameters; }
        }

        public string RESTORE_ALL_DEFAULT_PARAMETERS
        {
            get { return _restore_all_default_parameters; }
        }

        public string VENDOR_ID
        {
            get { return _vendor_id; }
        }

        public string PRODUCT_CODE
        {
            get { return _product_code; }
        }

        public string SERIAL_NUMBER
        {
            get { return _serial_number; }
        }

        public string IMPLEMENTED_PROFILE_SPECIFICATION
        {
            get { return _implemented_profile_specification; }
        }

        public string LC_CAPABILITY
        {
            get { return _lc_capability; }
        }

        public string WEIGHING_DEVICE_1_UNIT_PREFIX_OUTPUT_PARAMETER
        {
            get { return _weighing_device_1_unit_prefix_output_parameter; }
        }

        public string WEIGHING_DEVICE_1_WEIGHT_STEP
        {
            get { return _weighing_device_1_weight_step; }
        }

        public string ALARMS
        {
            get { return _alarms; }
        }

        public string WEIGHING_DEVICE_1_OUTPUT_WEIGHT
        {
            get { return _weighing_device_1_output_weight; }
        }

        public string WEIGHING_DEVICE_1_SETTING
        {
            get { return _weighing_device_1_setting; }
        }

        public string LOCAL_GRAVITY_FACTOR
        {
            get { return _local_gravity_factor; }
        }

        public string SCALE_FILTER_SETUP
        {
            get { return _scale_filter_setup; }
        }

        public string DATA_SAMPLE_RATE
        {
            get { return _data_sample_rate; }
        }

        public string FILTER_ORDER_CRITICALLY_DAMPED
        {
            get { return _filter_order_critically_damped; }
        }

        public string CUT_OFF_FREQUENCY_CRITICALLY_DAMPED
        {
            get { return _cut_off_frequency_critically_damped; }
        }

        public string FILTER_ORDER_BUTTERWORTH
        {
            get { return _filter_order_butterworth; }
        }

        public string CUT_OFF_FREQUENCY_BUTTERWORTH
        {
            get { return _cut_off_frequency_butterworth; }
        }

        public string FILTER_ORDER_BESSEL
        {
            get { return _filter_order_bessel; }
        }

        public string CUT_OFF_FREQUENCY_BESSEL
        {
            get { return _cut_off_frequency_bessel; }
        }

        public string SCALE_SUPPY_NOMINAL_VOLTAGE
        {
            get { return _scale_suppy_nominal_voltage; }
        }

        public string SCALE_SUPPY_MINIMUM_VOLTAGE
        {
            get { return _scale_suppy_minimum_voltage; }
        }

        public string SCALE_SUPPY_MAXIMUM_VOLTAGE
        {
            get { return _scale_suppy_maximum_voltage; }
        }

        public string SCALE_ACCURACY_CLASS
        {
            get { return _scale_accuracy_class; }
        }

        public string SCALE_MINIMUM_DEAD_LOAD
        {
            get { return _scale_minimum_dead_load; }
        }

        public string SCALE_MAXIMUM_CAPACITY
        {
            get { return _scale_maximum_capacity; }
        }

        public string SCALE_MAXIMUM_NUMBER_OF_VERIFICATION_INTERVAL
        {
            get { return _scale_maximum_number_of_verification_interval; }
        }

        public string SCALE_APPORTIONMENT_FACTOR
        {
            get { return _scale_apportionment_factor; }
        }

        public string SCALE_SAFE_LOAD_LIMIT
        {
            get { return _scale_safe_load_limit; }
        }

        public string SCALE_OPERATION_NOMINAL_TEMPERATURE
        {
            get { return _scale_operation_nominal_temperature; }
        }

        public string SCALE_OPERATION_MINIMUM_TEMPERATURE
        {
            get { return _scale_operation_minimum_temperature; }
        }

        public string SCALE_OPERATION_MAXIMUM_TEMPERATURE
        {
            get { return _scale_operation_maximum_temperature; }
        }

        public string SCALE_RELATIVE_MINIMUM_LOAD_CELL_VERIFICATION_INTERVAL
        {
            get { return _scale_relative_minimum_load_cell_verification_interval; }
        }

        public string INTERVAL_RANGE_CONTROL
        {
            get { return _interval_range_control; }
        }

        public string MULTI_LIMIT_1
        {
            get { return _multi_limit_1; }
        }

        public string MULTI_LIMIT_2
        {
            get { return _multi_limit_2; }
        }

        public string OIML_CERTIFICAITON_INFORMATION
        {
            get { return _oiml_certificaiton_information; }
        }

        public string NTEP_CERTIFICAITON_INFORMATION
        {
            get { return _ntep_certificaiton_information; }
        }

        public string MAXIMUM_ZEROING_TIME
        {
            get { return _maximum_zeroing_time; }
        }

        public string MAXIMUM_PEAK_VALUE_GROSS
        {
            get { return _maximum_peak_value_gross; }
        }

        public string MINIMUM_PEAK_VALUE_GROSS
        {
            get { return _minimum_peak_value_gross; }
        }

        public string MAXIMUM_PEAK_VALUE
        {
            get { return _maximum_peak_value; }
        }

        public string MINIMUM_PEAK_VALUE
        {
            get { return _minimum_peak_value; }
        }

        public string WEIGHT_MOVING_DETECTION
        {
            get { return _weight_moving_detection; }
        }

        public string DEVICE_ADDRESS
        {
            get { return _device_address; }
        }

        public string HARDWARE_VERSION
        {
            get { return _hardware_version; }
        }

        public string IDENTIFICATION
        {
            get { return _identification; }
        }

        public string LIMIT_VALUE_MONITORING_LIV11
        {
            get { return _limit_value_monitoring_liv11; }
        }

        public string SIGNAL_SOURCE_LIV12
        {
            get { return _signal_source_liv12; }
        }

        public string SWITCH_ON_LEVEL_LIV13
        {
            get { return _switch_on_level_liv13; }
        }

        public string SWITCH_OFF_LEVEL_LIV14
        {
            get { return _switch_off_level_liv14; }
        }

        public string LIMIT_VALUE_MONITORING_LIV21
        {
            get { return _limit_value_monitoring_liv21; }
        }

        public string SIGNAL_SOURCE_LIV22
        {
            get { return _signal_source_liv22; }
        }

        public string SWITCH_ON_LEVEL_LIV23
        {
            get { return _switch_on_level_liv23; }
        }

        public string SWITCH_OFF_LEVEL_LIV24
        {
            get { return _switch_off_level_liv24; }
        }

        public string LIMIT_VALUE_MONITORING_LIV31
        {
            get { return _limit_value_monitoring_liv31; }
        }

        public string SIGNAL_SOURCE_LIV32
        {
            get { return _signal_source_liv32; }
        }

        public string SWITCH_ON_LEVEL_LIV33
        {
            get { return _switch_on_level_liv33; }
        }

        public string SWITCH_OFF_LEVEL_LIV34
        {
            get { return _switch_off_level_liv34; }
        }

        public string LIMIT_VALUE_MONITORING_LIV41
        {
            get { return _limit_value_monitoring_liv41; }
        }

        public string SIGNAL_SOURCE_LIV42
        {
            get { return _signal_source_liv42; }
        }

        public string SWITCH_ON_LEVEL_LIV43
        {
            get { return _switch_on_level_liv43; }
        }

        public string SWITCH_OFF_LEVEL_LIV44
        {
            get { return _switch_off_level_liv44; }
        }

        public string OUTPUT_SCALE
        {
            get { return _output_scale; }
        }

        public string FIRMWARE_DATE
        {
            get { return _firmware_date; }
        }

        public string RESET_TRIGGER
        {
            get { return _reset_trigger; }
        }

        public string STATE_DIGITAL_IO_EXTENDED
        {
            get { return _state_digital_io_extended; }
        }

        public string SOFTWARE_IDENTIFICATION
        {
            get { return _software_identification; }
        }

        public string SOFTWARE_VERSION
        {
            get { return _software_version; }
        }

        public string DATE_TIME
        {
            get { return _date_time; }
        }

        public string BREAK_DOSING
        {
            get { return _break_dosing; }
        }

        public string DELETE_DOSING_RESULT
        {
            get { return _delete_dosing_result; }
        }

        public string MATERIAL_STREAM_LAST_DOSING
        {
            get { return _material_stream_last_dosing; }
        }

        public string SUM
        {
            get { return _sum; }
        }

        public string SPECIAL_DOSING_FUNCTIONS
        {
            get { return _special_dosing_functions; }
        }

        public string DISCHARGE_TIME
        {
            get { return _discharge_time; }
        }

        public string EXCEEDING_WEIGHT_BREAK
        {
            get { return _exceeding_weight_break; }
        }

        public string DELAY1_DOSING
        {
            get { return _delay1_dosing; }
        }

        public string DELAY2_DOSING
        {
            get { return _delay2_dosing; }
        }

        public string EMPTY_WEIGHT_TOLERANCE
        {
            get { return _empty_weight_tolerance; }
        }

        public string RESIDUAL_FLOW_DOSING_CYCLE
        {
            get { return _residual_flow_dosing_cycle; }
        }

        // Missing IDs (so far : Modbus only - look at ModbusCommands)
        public string ADC_OVER_UNDERLOAD { get { return ""; }}
        public string LEGAL_FOR_TRADE_OPERATION { get { return ""; }}
        public string STATUS_INPUT_1 { get { return ""; }}
        public string GENERAL_SCALE_ERROR { get { return ""; }}
        public string COARSE_FLOW { get { return ""; }}
        public string FINE_FLOW { get { return ""; }}
        public string READY { get { return ""; }}
        public string EMPTYING { get { return ""; }}
        public string FLOW_ERROR { get { return ""; }}
        public string ALARM { get { return ""; }}
        public string TOLERANCE_ERROR_PLUS { get { return ""; }}
        public string TOLERANCE_ERROR_MINUS { get { return ""; }}
        public string CURRENT_DOSING_TIME { get { return ""; }}
        public string CURRENT_COARSE_FLOW_TIME { get { return ""; }}
        public string CURRENT_FINE_FLOW_TIME { get { return ""; }}
        public string PARAMETER_SET_PRODUCT { get { return ""; }}
        public string DELAY_TIME_AFTER_FINE_FLOW { get { return ""; } }
        public string ACTIVATION_TIME_AFTER_FINE_FLOW { get { return ""; } }
        public string DOWNWARDS_DOSING { get { return ""; }}
        public string TOTAL_WEIGHT { get { return ""; }}
        public string TARGET_FILLING_WEIGHT { get { return ""; }}
        public string COARSE_FLOW_CUT_OFF_POINT_SET { get { return ""; }}
        public string FINE_FLOW_CUT_OFF_POINT_SET { get { return ""; }}
        public string START_WITH_FINE_FLOW { get { return ""; }}

        #endregion
    }
}
