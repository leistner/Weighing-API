// <copyright file="ICommands.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace HBM.Weighing.API.WTX
{
    /// <summary>
    /// Interface for using commands, respectively indexes/paths, to read/write the registers of the WTX device via Modbus
    /// or to the paths via Jetbus to get the data.
    /// Its subclasses are : ModbusCommands, JetBusCommands. 
    /// </summary>
    public interface ICommands
    {
        #region ID Commands : Maintenance - Calibration

        string LDW_DEAD_WEIGHT{ get; }                // LDW = Nullpunkt
        string LWT_NOMINAL_VALUE{ get; }              // LWT = Nennwert
        string LFT_SCALE_CALIBRATION_WEIGHT{ get; }   // LFT = LFT scale calibration weight

        #endregion

        #region ID commands for process data
        string NET_VALUE { get; }
        string GROSS_VALUE { get; }
        string ZERO_VALUE { get; }

        string TARE_VALUE { get; }
        string WEIGHING_DEVICE_1_WEIGHT_STATUS { get; }
        string UNIT_PREFIX_FIXED_PARAMETER { get; }

        string APPLICATION_MODE { get; } // IMD = Input mode ( Application mode)

        string DECIMALS { get; }
        string SCALE_COMMAND { get; }
        string SCALE_COMMAND_STATUS { get; }
        #endregion

        #region ID commands for standard mode
        string STATUS_DIGITAL_INPUT_1 { get; }    // IS1
        string STATUS_DIGITAL_INPUT_2 { get; }    // IS2
        string STATUS_DIGITAL_INPUT_3 { get; }    // IS3
        string STATUS_DIGITAL_INPUT_4 { get; }    // IS4

        string STATUS_DIGITAL_OUTPUT_1 { get; }   // OS1
        string STATUS_DIGITAL_OUTPUT_2 { get; }   // OS2
        string STATUS_DIGITAL_OUTPUT_3 { get; }   // OS3
        string STATUS_DIGITAL_OUTPUT_4 { get; }   // OS4

        string LIMIT_VALUE { get; }   // LVS

        #endregion
        
        #region ID commands for filler data

        string COARSE_FLOW_MONITORING { get; }      // CBK = Füllstromüberwachung Grobstrom
        string COARSE_FLOW_MONITORING_TIME { get; } // CBT = Überwachungszeit Grobstrom
        string COARSE_FLOW_CUT_OFF_POINT { get; }   // CFD = Grobstromabschaltpunkt
        string COARSE_FLOW_TIME { get; }            // CFT = Grobstromzeit
        string DOSING_MODE { get; }                 // DMD = Dosiermodus
        string DOSING_TIME { get; }                 // DST = Dosieristzeit
        string EMPTYING_MODE { get; }               // EMD = Entleermodus

        string FINE_FLOW_MONITORING { get; }        // FBK = Füllstromüberwachung Feinstrom
        string FINE_FLOW_MONITORING_TIME { get; }   // FBT = Überwachungszeit Feinstrom
        string FINE_FLOW_CUT_OFF_POINT { get; }     // FFD = Feinstromabschaltpunkt
        string FINE_FLOW_PHASE_BEFORE_COARSE_FLOW { get; } // FFL = Feinstromphase vor Grobstrom
        string MINIMUM_FINE_FLOW { get; }           // FFM = Minimaler Feinstromanteil

        string FINE_FLOW_TIME { get; }              // FFT = Feinstromzeit
        string DOSING_RESULT { get; }               // FRS1 = Dosierergebnis
        string DOSING_STATE { get; }                // FRS2 = Dosierstatus
        string REFERENCE_VALUE_DOSING { get; }      // FWT = Sollwert dosieren
        string LOCKOUT_TIME_COARSE_FLOW { get; }    // LTC = Sperrzeit Grobstrom
        string LOCKOUT_TIME_FINE_FLOW { get; }      // LTF = Sperrzeit Feinstrom
        string LOWER_TOLERANCE_LIMIT { get; }       // LTL = Untere Toleranz
        string MAXIMAL_DOSING_TIME { get; }         // MDT = Maximale Dosierzeit
        string MINIMUM_START_WEIGHT { get; }        // MSW = Minimum Startgewicht
        string DOSING_COUNTER { get; }              // NDS = Dosierzähler
        string OPTIMIZATION { get; }                // OSN = Optimierung
        string RANGE_SELECTION_PARAMETER { get; }   // RDP = Auswahl Dosierparameter

        string REDOSING { get; }                    // RDS = Nachdosieren
        string RESIDUAL_FLOW_TIME { get; }          // RFT = Nachstromzeit
        string RUN_START_DOSING { get; }            // RUN = Start Dosieren

        string MEAN_VALUE_DOSING_RESULTS { get; }         // SDM = Mittelwert Dosieren
        string DOSING_STATE_FILLER { get; }               // SDO = Dosierstatus
        string STANDARD_DEVIATION { get; }                // SDS = Standardabweichung
        string SETTLING_TIME_TRANSIENT_RESPONSE { get; }  // STT = Beruhigungszeit
        string SYSTEMATIC_DIFFERENCE { get; }             // SYD = Systematische Differenz
        string TARE_DELAY { get; }                        // TAD = Tarierverzögerung
        string TARE_MODE { get; }                         // TMD = Tariermodus
        string UPPER_TOLERANCE_LIMIT { get; }             // UTL = Obere Toleranz

        string VALVE_CONTROL { get; }               // VCT = Ventilsteuerung
        string WRITE_DOSING_PARAMETER_SET { get; }  // WDP = Dosierparametersatz schreiben
        string STORAGE_WEIGHT { get; }              // STO = Gewichtsspeicherung
        string STORAGE_WEIGHT_MODE { get; }         // SMD = Modus Gewichtsspeicherung

        #endregion

        #region ID commands for filler extended data

        #endregion
        
        #region All other ID commands for Operator, Administrator and Maintenance : 

        string ERROR_REGISTER { get; }
        string SAVE_ALL_PARAMETERS { get; }
        string RESTORE_ALL_DEFAULT_PARAMETERS { get; }
        string VENDOR_ID { get; }
        string PRODUCT_CODE { get; }
        string SERIAL_NUMBER { get; }
        string IMPLEMENTED_PROFILE_SPECIFICATION { get; }
        string LC_CAPABILITY { get; }
        string WEIGHING_DEVICE_1_UNIT_PREFIX_OUTPUT_PARAMETER { get; }

        string WEIGHING_DEVICE_1_WEIGHT_STEP { get; }
        string ALARMS { get; }
        string WEIGHING_DEVICE_1_OUTPUT_WEIGHT { get; }
        string WEIGHING_DEVICE_1_SETTING { get; }
        string LOCAL_GRAVITY_FACTOR { get; }
        string SCALE_FILTER_SETUP { get; }
        string DATA_SAMPLE_RATE { get; }
        string FILTER_ORDER_CRITICALLY_DAMPED { get; }
        string CUT_OFF_FREQUENCY_CRITICALLY_DAMPED { get; }
        string FILTER_ORDER_BUTTERWORTH { get; }
        string CUT_OFF_FREQUENCY_BUTTERWORTH { get; }
        string FILTER_ORDER_BESSEL { get; }
        string CUT_OFF_FREQUENCY_BESSEL { get; }
        string SCALE_SUPPY_NOMINAL_VOLTAGE { get; }
        string SCALE_SUPPY_MINIMUM_VOLTAGE { get; }
        string SCALE_SUPPY_MAXIMUM_VOLTAGE { get; }
        string SCALE_ACCURACY_CLASS { get; }
        string SCALE_MINIMUM_DEAD_LOAD { get; }
        string SCALE_MAXIMUM_CAPACITY { get; }
        string SCALE_MAXIMUM_NUMBER_OF_VERIFICATION_INTERVAL { get; }
        string SCALE_APPORTIONMENT_FACTOR { get; }
        string SCALE_SAFE_LOAD_LIMIT { get; }
        string SCALE_OPERATION_NOMINAL_TEMPERATURE { get; }
        string SCALE_OPERATION_MINIMUM_TEMPERATURE { get; }
        string SCALE_OPERATION_MAXIMUM_TEMPERATURE { get; }
        string SCALE_RELATIVE_MINIMUM_LOAD_CELL_VERIFICATION_INTERVAL { get; }
        string INTERVAL_RANGE_CONTROL { get; }
        string MULTI_LIMIT_1 { get; }
        string MULTI_LIMIT_2 { get; }

        
        string OIML_CERTIFICAITON_INFORMATION { get; }
        string NTEP_CERTIFICAITON_INFORMATION { get; }
        string MAXIMUM_ZEROING_TIME { get; }
        string MAXIMUM_PEAK_VALUE_GROSS { get; }
        string MINIMUM_PEAK_VALUE_GROSS { get; }
        string MAXIMUM_PEAK_VALUE { get; }
        string MINIMUM_PEAK_VALUE { get; }
        string WEIGHT_MOVING_DETECTION { get; }
        string DEVICE_ADDRESS { get; }
        
        string HARDWARE_VERSION { get; } // = Hardware Variante
        string IDENTIFICATION { get; }
        string LIMIT_VALUE_MONITORING_LIV11 { get; } // = Grenzwertüberwachung
        string SIGNAL_SOURCE_LIV12 { get; }
        string SWITCH_ON_LEVEL_LIV13 { get; }       // = Einschaltpegel
        string SWITCH_OFF_LEVEL_LIV14 { get; }      // = Ausschaltpegel
        string LIMIT_VALUE_MONITORING_LIV21 { get; }
        string SIGNAL_SOURCE_LIV22 { get; }
        string SWITCH_ON_LEVEL_LIV23 { get; }
        string SWITCH_OFF_LEVEL_LIV24 { get; }
        string LIMIT_VALUE_MONITORING_LIV31 { get; }
        string SIGNAL_SOURCE_LIV32 { get; }
        
        string SWITCH_ON_LEVEL_LIV33 { get; }
        string SWITCH_OFF_LEVEL_LIV34 { get; }
        string LIMIT_VALUE_MONITORING_LIV41 { get; }
        string SIGNAL_SOURCE_LIV42 { get; }
        string SWITCH_ON_LEVEL_LIV43 { get; }
        string SWITCH_OFF_LEVEL_LIV44 { get; }
        string OUTPUT_SCALE { get; }
        string FIRMWARE_DATE { get; }
        string RESET_TRIGGER { get; }
        string STATE_DIGITAL_IO_EXTENDED { get; }  // Zustand Digital-IO(erweitert)
        string SOFTWARE_IDENTIFICATION { get; }
        string SOFTWARE_VERSION { get; }
        string DATE_TIME { get; }
        
        string BREAK_DOSING { get; }                // BRK = Abbruch Dosierung
        string DELETE_DOSING_RESULT { get; }        // CSN = Löschen Dosierergebniss
        string MATERIAL_STREAM_LAST_DOSING { get; } // MFO = Materialstrom des letzten Dosierzyklus
        string SUM { get; }                         // SUM = Summe
        string SPECIAL_DOSING_FUNCTIONS { get; }    // SDF = Sonderfunktionen
        string DISCHARGE_TIME { get; }              // EPT = Entleerzeit
        string EXCEEDING_WEIGHT_BREAK { get; }      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
        string DELAY1_DOSING { get; }               // DL1 = Delay 1 für Dosieren
        string DELAY2_DOSING { get; }               // DL2 = Delay 2 für Dosieren
        string EMPTY_WEIGHT_TOLERANCE { get; }      // EWT = Entleertoleranz
        string RESIDUAL_FLOW_DOSING_CYCLE { get; }  // RFO = Nachstrom des letzten Dosierzyklus
        
        #endregion
    
    }
}
