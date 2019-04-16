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
    public class ModbusCommands : ICommands
    {
        public ModbusCommands()
        {          
        }

        #region ID Commands : Memory - day, month, year, seqNumber, gross, net

        // For standard mode: 
        private const string _readWeightMemDay_ID       = "9";  
        private const string _readWeightMemMonth_ID     = "10";
        private const string _readWeightMemYear_ID      = "11";
        private const string _readWeightMemSeqNumber_ID = "12";
        private const string _readWeightMemGross_ID     = "13";
        private const string _readWeightMemNet_ID       = "14";

        // For filler mode: 
        private const string _writeWeightMemDay_ID       = "32";
        private const string _writeWeightMemMonth_ID     = "33";
        private const string _writeWeightMemYear_ID      = "34";
        private const string _writeWeightMemSeqNumber_ID = "35";
        private const string _writeWeightMemGross_ID     = "36";
        private const string _writeWeightMemNet_ID       = "37";

        private string[] _readWeightMemArray  = new string[6] { _readWeightMemDay_ID, _readWeightMemMonth_ID, _readWeightMemYear_ID, _readWeightMemSeqNumber_ID, _readWeightMemGross_ID, _readWeightMemNet_ID };
        private string[] _writeWeightMemArray = new string[6]{ _writeWeightMemDay_ID, _writeWeightMemMonth_ID, _writeWeightMemYear_ID, _writeWeightMemSeqNumber_ID, _writeWeightMemGross_ID, _writeWeightMemNet_ID };
        #endregion

        #region ID Commands : Maintenance - Calibration

        private const string _ldw_dead_weight   = "48";              // LDW = Nullpunkt
        private const string _lwt_nominal_value = "50";              // LWT = Nennwert
        private const string _lft_scale_calibration_weight = "46";   // LFT = LFT scale calibration weight

        #endregion

        #region ID commands for process data
        private const string _net_value   = "0";
        private const string _gross_value = "2";
        private const string _zero_value  = "";

        private const string _weighing_device_1_weight_status = "4";

        private const string _limit_status = "4/2/2";                   // data word = 4 ; length = 2 ; offset = 2;
        private const string _unit_prefix_fixed_parameter = "5/2/7";    // data word = 5 ; length = 2 ; offset = 7;

        private const string _application_mode = "5/2/0";               // data word = 5 ; length = 2 ; offset = 0;
        private const string _decimals = "5/3/4";                       // data word = 5 ; length = 3 ; offset = 4;
        private const string _scale_command_status = "5/1/15";          // data word = 5 ; length = 1 ; offset = 15;

        private const string _scale_command = "";

        #endregion

        #region ID commands for standard mode
        private const string _status_digital_input_1 = "6/1";    // IS1
        private const string _status_digital_input_2 = "6/2";    // IS2
        private const string _status_digital_input_3 = "6/3";    // IS3
        private const string _status_digital_input_4 = "6/4";    // IS4

        private const string _status_digital_output_1 = "7/1";   // OS1
        private const string _status_digital_output_2 = "7/2";   // OS2
        private const string _status_digital_output_3 = "7/3";   // OS3
        private const string _status_digital_output_4 = "7/4";   // OS4

        private const string _limit_value = "8/standard/input";   // LVS

        private const string _tare_value = "2";  // manual tare value

        private const string _limit_value_monitoring_liv11 = "4/standard/output"; // = Grenzwertüberwachung 
        private const string _signal_source_liv12 = "5/standard/output";
        private const string _switch_on_level_liv13 = "6/standard/output";        // = Einschaltpegel
        private const string _switch_off_level_liv14 = "8/standard/output";       // = Ausschaltpegel

        private const string _limit_value_monitoring_liv21 = "10/standard/output";
        private const string _signal_source_liv22 = "11/standard/output";
        private const string _switch_on_level_liv23 = "12/standard/output";
        private const string _switch_off_level_liv24 = "14/standard/output";

        private const string _limit_value_monitoring_liv31 = "16/standard/output";
        private const string _signal_source_liv32 = "17/standard/output";
        private const string _switch_on_level_liv33 = "18/standard/output";
        private const string _switch_off_level_liv34 = "20/standard/output";

        private const string _limit_value_monitoring_liv41 = "22/standard/output";
        private const string _signal_source_liv42 = "23/standard/output";
        private const string _switch_on_level_liv43 = "24/standard/output";
        private const string _switch_off_level_liv44 = "26/standard/output";
        #endregion

        #region ID commands for filler data

        private const string _coarseFlow = "8/0/filler/input";                // data input word 8, bit .0, application mode=filler
        private const string _fineFlow   = "8/1/filler/input";                // data input word 8, bit .1, application mode=filler
        private const string _ready      = "8/2/filler/input";                // data input word 8, bit .2, application mode=filler
        private const string _reDosing   = "8/3/filler/input";                // data input word 8, bit .3, application mode=filler; RDS = Nachdosieren
        private const string _emptying   = "8/4/filler/input";                // data input word 8, bit .4, application mode=filler
        private const string _flowError  = "8/5/filler/input";                // data input word 8, bit .5, application mode=filler
        private const string _alarm      = "8/6/filler/input";                // data input word 8, bit .6, application mode=filler
        private const string _adcOverUnderload       = "8/7/filler/input";    // data input word 8, bit .7, application mode=filler
        private const string _maximalDosingTimeInput = "8/8/filler/input";    // data input word 8, bit .8, application mode=filler
        private const string _legalForTradeOperation = "8/9/filler/input";    // data input word 8, bit .9, application mode=filler
        private const string _toleranceErrorPlus     = "8/10/filler/input";   // data input word 8, bit .10, application mode=filler
        private const string _toleranceErrorMinus    = "8/11/filler/input";   // data input word 8, bit .11, application mode=filler
        private const string _statusInput1      = "8/14/filler/input";        // data input word 8, bit .14, application mode=filler
        private const string _generalScaleError = "8/15/filler/input";        // data input word 8, bit .15, application mode=filler

        private const string _totalWeight         = "18/filler/input";          // data input word 18, application mode=filler
        private const string _dosing_time         = "24/filler/input";        // DST = Dosieristzeit
        private const string _coarse_flow_time    = "25/filler/input";        // CFT = Grobstromzeit
        private const string _currentFineFlowTime = "26/filler/input";        // data input word 26, application mode=filler; FFT = Feinstromzeit
        private const string _parameterSetProduct = "27/filler/input";        // data input word 27, application mode=filler
        private const string _targetFillingWeight = "10/filler/output";   // data output word 10, application mode=filler

        private const string _residual_flow_time        = "9/filler/output";    // RFT = Nachstromzeit
        private const string _reference_value_dosing    = "10/filler/output";   // FWT = Sollwert dosieren = Target filling weight
        private const string _coarse_flow_cut_off_point = "12/filler/output";   // CFD = Grobstromabschaltpunkt
        private const string _fine_flow_cut_off_point   = "14/filler/output";   // FFD = Feinstromabschaltpunkt

        private const string _minimum_fine_flow   = "16/filler/output";         // FFM = Minimaler Feinstromanteil
        private const string _optimization        = "18/filler/output";         // OSN = Optimierung
        private const string _maximal_dosing_time = "19/filler/output";         // MDT = Maximale Dosierzeit
        private const string _run_start_dosing    = "20/filler/output";         // RUN = Start Dosieren

        private const string _lockout_time_coarse_flow = "21/filler/output";    // LTC = Sperrzeit Grobstrom
        private const string _lockout_time_fine_flow   = "22/filler/output";    // LTF = Sperrzeit Feinstrom
        private const string _tare_mode                = "23/filler/output";    // TMD = Tariermodus
        private const string _upper_tolerance_limit    = "24/filler/output";    // UTL = Obere Toleranz

        private const string _lower_tolerance_lomit     = "26/filler/output";   // LTL = Untere Toleranz
        private const string _minimum_start_weight      = "28/filler/output";   // MSW = Minimum Startgewicht
        private const string _empty_weight              = "30/filler/output"; 
        private const string _tare_delay                = "32/filler/output";   // TAD = Tarierverzögerung

        private const string _coarse_flow_monitoring_time = "33/filler/output"; // CBT = Überwachungszeit Grobstrom
        private const string _coarse_flow_monitoring      = "34/filler/output"; // CBK = Füllstromüberwachung Grobstrom
        private const string _fine_flow_monitoring        = "36/filler/output"; // FBK = Füllstromüberwachung Feinstrom
        private const string _fine_flow_monitoring_time   = "38/filler/output"; // FBT = Überwachungszeit Feinstrom

        private const string _delay_time_after_fine_flow = "39/filler/output";
        private const string _activation_time_after_fine_flow = "40/filler/output"; 

        private const string _systematic_difference = "41/filler/output";       // SYD = Systematische Differenz
        private const string _downwardsDosing       = "42/filler/output";       // data output word 42, application mode=filler
        private const string _valve_control         = "43/filler/output";       // VCT = Ventilsteuerung
        private const string _emptying_mode         = "44/filler/output";       // EMD = Entleermodus

        // Undefined ID's : Jetus only

        private const string _range_selection_parameter  = "";    // Undefined RDP = Auswahl Dosierparameter 
        private const string _write_dosing_parameter_set = "";    // WDP = Dosierparametersatz schreiben 
        private const string _storage_weight = "";                // STO = Gewichtsspeicherung
        private const string _storage_weight_mode = "";           // SMD = Modus Gewichtsspeicherung

        private const string _dosing_mode = "";                        // DMD = Dosiermodus  
        private const string _fine_flow_phase_before_coarse_flow = ""; // FFL = Feinstromphase vor Grobstrom

        private const string _dosing_result = "";     // FRS1 = Dosierergebnis
        private const string _dosing_state = "";     // FRS2 = Dosierstatus
        private const string _dosing_counter = "";    // NDS = Dosierzähler

        private const string _mean_value_dosing_results = "";         // SDM = Mittelwert Dosieren
        private const string _dosing_state_filler = "";               // SDO = Dosierstatus
        private const string _standard_deviation = "";                // SDS = Standardabweichung
        private const string _settling_time_transient_response = "";  // STT = Beruhigungszeit

        #endregion

        #region ID commands for filler extended data

        #endregion

        #region All other ID commands for Operator, Administrator and Maintenance : 

        // Paths/Commands : Digital input in the extended filler mode, only via Jetbus, therefore it is empty for Modbus
        private const string _function_digital_input_1 = "";
        private const string _function_digital_input_2 = "";
        private const string _function_digital_input_3 = "";
        private const string _function_digital_input_4 = "";
        // Paths/Commands : Digital output in the extended filler mode, only via Jetbus, therefore it is empty for Modbus
        private const string _function_digital_output_1 = "";
        private const string _function_digital_output_2 = "";
        private const string _function_digital_output_3 = "";
        private const string _function_digital_output_4 = "";

        private const string _error_register = "";
        private const string _save_all_parameters = "";
        private const string _restore_all_default_parameters = "";
        private const string _vendor_id = "";
        private const string _product_code = "";
        private const string _serial_number = "";
        private const string _implemented_profile_specification = "";
        private const string _lc_capability = "";
        private const string _weighing_device_1_unit_prefix_output_parameter = "";

        private const string _weighing_device_1_weight_step = "";
        private const string _alarms = "";
        private const string _weighing_device_1_output_weight = "";
        private const string _weighing_device_1_setting = "";
        private const string _local_gravity_factor = "";
        private const string _scale_filter_setup = "";
        private const string _data_sample_rate = "";
        private const string _filter_order_critically_damped = "";
        private const string _cut_off_frequency_critically_damped = "";
        private const string _filter_order_butterworth = "";
        private const string _cut_off_frequency_butterworth = "";
        private const string _filter_order_bessel = "";
        private const string _cut_off_frequency_bessel = "";
        private const string _scale_suppy_nominal_voltage = "";
        private const string _scale_suppy_minimum_voltage = "";
        private const string _scale_suppy_maximum_voltage = "";
        private const string _scale_accuracy_class = "";
        private const string _scale_minimum_dead_load = "";
        private const string _scale_maximum_capacity = "";
        private const string _scale_maximum_number_of_verification_interval = "";
        private const string _scale_apportionment_factor = "";
        private const string _scale_safe_load_limit = "";
        private const string _scale_operation_nominal_temperature = "";
        private const string _scale_operation_minimum_temperature = "";
        private const string _scale_operation_maximum_temperature = "";
        private const string _scale_relative_minimum_load_cell_verification_interval = "";
        private const string _interval_range_control = "";
        private const string _multi_limit_1 = "";
        private const string _multi_limit_2 = "";
        private const string _oiml_certificaiton_information = "";
        private const string _ntep_certificaiton_information = "";
        private const string _maximum_zeroing_time = "";
        private const string _maximum_peak_value_gross = "";
        private const string _minimum_peak_value_gross = "";
        private const string _maximum_peak_value = "";
        private const string _minimum_peak_value = "";
        private const string _weight_moving_detection = "";
        private const string _device_address = "";

        private const string _hardware_version = ""; // = Hardware Variante
        private const string _identification = "";

        private const string _output_scale = "";
        private const string _firmware_date = "";
        private const string _reset_trigger = "";
        private const string _state_digital_io_extended = "";  //Zustand Digital-IO(erweitert)
        private const string _software_identification = "";
        private const string _software_version = "";
        private const string _date_time = "";

        private const string _break_dosing = "";                // BRK = Abbruch Dosierung
        private const string _delete_dosing_result = "";        // CSN = Löschen Dosierergebniss
        private const string _material_stream_last_dosing = ""; // MFO = Materialstrom des letzten Dosierzyklus
        private const string _sum = "";                         // SUM = Summe
        private const string _special_dosing_functions = "";    // SDF = Sonderfunktionen
        private const string _discharge_time = "";              // EPT = Entleerzeit
        private const string _exceeding_weight_break = "";      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
        private const string _delay1_dosing = "";               // DL1 = Delay 1 für Dosieren
        private const string _delay2_dosing = "";               // DL2 = Delay 2 für Dosieren
        private const string _empty_weight_tolerance = "";      // EWT = Entleertoleranz
        private const string _residual_flow_dosing_cycle = "";  // RFO = Nachstrom des letzten Dosierzyklus

        public string LDW_DEAD_WEIGHT
        {
            get { return _ldw_dead_weight; }
        }

        public string LWT_NOMINAL_VALUE
        {
            get { return _lwt_nominal_value; }
        }

        public string LFT_SCALE_CALIBRATION_WEIGHT
        {
            get { return _lft_scale_calibration_weight; }
        }

        public string NET_VALUE
        {
            get { return _net_value; }
        }

        public string GROSS_VALUE
        {
            get { return _gross_value; }
        }

        public string ZERO_VALUE
        {
            get { return _zero_value; }
        }

        public string TARE_VALUE
        {
            get { return _tare_value; }
        }

        public string WEIGHING_DEVICE_1_WEIGHT_STATUS
        {
            get { return _weighing_device_1_weight_status; }
        }

        public string UNIT_PREFIX_FIXED_PARAMETER
        {
            get { return _unit_prefix_fixed_parameter; }
        }

        public string APPLICATION_MODE
        {
            get { return _application_mode; }
        }

        public string DECIMALS
        {
            get { return _decimals; }
        }

        public string SCALE_COMMAND
        {
            get { return _scale_command; }
        }

        public string SCALE_COMMAND_STATUS
        {
            get { return _scale_command_status; }
        }

        public string STATUS_DIGITAL_INPUT_1
        {
            get { return _status_digital_input_1; }
        }

        public string STATUS_DIGITAL_INPUT_2
        {
            get { return _status_digital_input_2; }
        }

        public string STATUS_DIGITAL_INPUT_3
        {
            get { return _status_digital_input_3; }
        }

        public string STATUS_DIGITAL_INPUT_4
        {
            get { return _status_digital_input_4; }
        }

        public string STATUS_DIGITAL_OUTPUT_1
        {
            get { return _status_digital_output_1; }
        }

        public string STATUS_DIGITAL_OUTPUT_2
        {
            get { return _status_digital_output_2; }
        }

        public string STATUS_DIGITAL_OUTPUT_3
        {
            get { return _status_digital_output_3; }
        }

        public string STATUS_DIGITAL_OUTPUT_4
        {
            get { return _status_digital_output_4; }
        }

        public string LIMIT_VALUE
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
            get { return _currentFineFlowTime; } // same like CURRENT_FINE_FLOW_TIME
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

        public string DELAY_TIME_AFTER_FINE_FLOW
        {
            get { return _delay_time_after_fine_flow; }
        }
        public string ACTIVATION_TIME_AFTER_FINE_FLOW
        {
            get { return _activation_time_after_fine_flow; }
        }

        public string REDOSING
        {
            get { return _reDosing; }
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
        string ICommands.FUNCTION_DIGITAL_INPUT_1
        {
            get { return _function_digital_input_1; }
        }

        string ICommands.FUNCTION_DIGITAL_INPUT_2
        {
            get { return _function_digital_input_2; }
        }

        string ICommands.FUNCTION_DIGITAL_INPUT_3
        {
            get { return _function_digital_input_3; }
        }

        string ICommands.FUNCTION_DIGITAL_INPUT_4
        {
            get { return _function_digital_input_4; }
        }

        string ICommands.FUNCTION_DIGITAL_OUTPUT_1
        {
            get { return _function_digital_output_1; }
        }

        string ICommands.FUNCTION_DIGITAL_OUTPUT_2
        {
            get { return _function_digital_output_2; }
        }

        string ICommands.FUNCTION_DIGITAL_OUTPUT_3
        {
            get { return _function_digital_output_3; }
        }

        string ICommands.FUNCTION_DIGITAL_OUTPUT_4
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

        public string[] WEIGHT_MEMORY_STANDARD
        {
            get
            {
                return _readWeightMemArray;
            }
        }

        public string[] WEIGHT_MEMORY_FILLER
        {
            get
            {
                return _writeWeightMemArray;
            }
        }

        public string ADC_OVER_UNDERLOAD
        {
            get
            {
                return _adcOverUnderload;
            }
        }

        public string LEGAL_FOR_TRADE_OPERATION
        {
            get
            {
                return _legalForTradeOperation;
            }
        }

        public string STATUS_INPUT_1
        {
            get
            {
                return _statusInput1;
            }
        }

        public string GENERAL_SCALE_ERROR
        {
            get
            {
                return _generalScaleError;
            }
        }

        public string COARSE_FLOW
        {
            get
            {
                return _coarseFlow;
            }
        }

        public string FINE_FLOW
        {
            get
            {
                return _fineFlow;
            }
        }

        public string READY
        {
            get
            {
                return _ready;
            }
        }

        public string EMPTYING
        {
            get
            {
                return _emptying;
            }
        }

        public string FLOW_ERROR
        {
            get
            {
                return _flowError;
            }
        }

        public string ALARM
        {
            get
            {
                return _alarm;
            }
        }

        public string TOLERANCE_ERROR_PLUS
        {
            get
            {
                return _toleranceErrorPlus;
            }
        }

        public string TOLERANCE_ERROR_MINUS
        {
            get
            {
                return _toleranceErrorMinus;
            }
        }

        public string CURRENT_DOSING_TIME
        {
            get
            {
                return _dosing_time;
            }
        }

        public string CURRENT_COARSE_FLOW_TIME
        {
            get
            {
                return _coarse_flow_time;
            }
        }

        public string CURRENT_FINE_FLOW_TIME
        {
            get
            {
                return _currentFineFlowTime;
            }
        }

        public string PARAMETER_SET_PRODUCT
        {
            get
            {
                return _parameterSetProduct;
            }
        }

        public string DOWNWARDS_DOSING
        {
            get
            {
                return _downwardsDosing;
            }
        }

        public string TOTAL_WEIGHT
        {
            get
            {
                return _totalWeight;
            }
        }

        public string TARGET_FILLING_WEIGHT
        {
            get
            {
                return _targetFillingWeight;
            }
        }

        public string COARSE_FLOW_CUT_OFF_POINT_SET
        {
            get
            {
                return _coarse_flow_cut_off_point;
            }
        }

        public string FINE_FLOW_CUT_OFF_POINT_SET
        {
            get
            {
                return _fine_flow_cut_off_point;
            }
        }

        public string START_WITH_FINE_FLOW
        {
            get
            {
                return _run_start_dosing;
            }
        }
        #endregion
    }
}
