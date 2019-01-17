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
    /// ID's/Commands for subscribing values of the WTX device. 
    /// The ID's are commited as a parameter for the read and/or write method call.  
    /// </summary>
    struct JetBusCommands
    {

        #region ID Commands : Maintenance - Calibration

        public const string LDW_DEAD_WEIGHT = "2110/06";                // LDW = Nullpunkt
        public const string LWT_NOMINAL_VALUE = "2110/07";              // LWT = Nennwert
        public const string LFT_SCALE_CALIBRATION_WEIGHT = "6152/00";   // LFT = LFT scale calibration weight

        #endregion

        #region ID commands for process data
        public const string NET_VALUE = "601A/01";
        public const string GROSS_VALUE = "6144/00";
        public const string ZERO_VALUE = "6142/00";

        public const string TARE_VALUE = "6143/00";
        public const string WEIGHING_DEVICE_1_WEIGHT_STATUS = "6012/01";
        public const string UNIT_PREFIX_FIXED_PARAMETER = "6014/01";

        public const string APPLICATION_MODE = "2010/07"; // IMD = Input mode ( Application mode)

        public const string DECIMALS = "6013/01";
        public const string SCALE_COMMAND = "6002/01";
        public const string SCALE_COMMAND_STATUS = "6002/02";
        #endregion

        #region ID commands for standard mode
        public const string STATUS_DIGITAL_INPUT_1 = "2020/18";    // IS1
        public const string STATUS_DIGITAL_INPUT_2 = "2020/19";    // IS2
        public const string STATUS_DIGITAL_INPUT_3 = "2020/1A";    // IS3
        public const string STATUS_DIGITAL_INPUT_4 = "2020/1B";    // IS4

        public const string STATUS_DIGITAL_OUTPUT_1 = "2020/1E";   // OS1
        public const string STATUS_DIGITAL_OUTPUT_2 = "2020/1F";   // OS2
        public const string STATUS_DIGITAL_OUTPUT_3 = "2020/20";   // OS3
        public const string STATUS_DIGITAL_OUTPUT_4 = "2020/21";   // OS4

        public const string LIMIT_VALUE = "2020/25";   // LVS

        #endregion

        #region ID commands for filler data

        public const string BREAK_DOSING = "2240/01";                // BRK = Abbruch Dosierung
        public const string COARSE_FLOW_MONITORING = "2210/01";      // CBK = Füllstromüberwachung Grobstrom
        public const string COARSE_FLOW_MONITORING_TIME = "2220/01"; // CBT = Überwachungszeit Grobstrom
        public const string COARSE_FLOW_CUT_OFF_POINT = "2210/02";   // CFD = Grobstromabschaltpunkt
        public const string COARSE_FLOW_TIME = "2230/01";            // CFT = Grobstromzeit
        public const string DELETE_DOSING_RESULT = "2230/02";        // CSN = Löschen Dosierergebniss
        public const string DELAY1_DOSING = "2220/0B";               // DL1 = Delay 1 für Dosieren
        public const string DELAY2_DOSING = "2220/0C";               // DL2 = Delay 2 für Dosieren
        public const string DOSING_MODE = "2200/04";                 // DMD = Dosiermodus
        public const string DOSING_TIME = "2230/03";                 // DST = Dosieristzeit
        public const string EMPTYING_MODE = "2200/05";               // EMD = Entleermodus
        public const string DISCHARGE_TIME = "2220/02";              // EPT = Entleerzeit

        public const string EXCEEDING_WEIGHT_BREAK = "2200/0F";      // EWB = Dosierabbruch bei Leergewichtsüberschreitung
        public const string EMPTY_WEIGHT_TOLERANCE = "2210/03";      // EWT = Entleertoleranz
        public const string FINE_FLOW_MONITORING = "2210/04";        // FBK = Füllstromüberwachung Feinstrom
        public const string FINE_FLOW_MONITORING_TIME = "2220/03";   // FBT = Überwachungszeit Feinstrom
        public const string FINE_FLOW_CUT_OFF_POINT = "2210/05";     // FFD = Feinstromabschaltpunkt
        public const string FINE_FLOW_PHASE_BEFORE_COARSE_FLOW = "2220/0A"; // FFL = Feinstromphase vor Grobstrom
        public const string MINIMUM_FINE_FLOW = "2210/06";           // FFM = Minimaler Feinstromanteil

        public const string FINE_FLOW_TIME = "2230/04";              // FFT = Feinstromzeit
        public const string DOSING_RESULT = "2000/05";               // FRS1 = Dosierergebnis
        public const string DOSING_STATE = "2000/06";                // FRS2 = Dosierstatus
        public const string REFERENCE_VALUE_DOSING = "2210/07";      // FWT = Sollwert dosieren
        public const string LOCKOUT_TIME_COARSE_FLOW = "2220/04";    // LTC = Sperrzeit Grobstrom
        public const string LOCKOUT_TIME_FINE_FLOW = "2220/05";      // LTF = Sperrzeit Feinstrom
        public const string LOWER_TOLERANCE_LOMIT = "2210/08";       // LTL = Untere Toleranz
        public const string MAXIMAL_DOSING_TIME = "2220/06";         // MDT = Maximale Dosierzeit
        public const string MATERIAL_STREAM_LAST_DOSING = "2000/0E"; // MFO = Materialstrom des letzten Dosierzyklus
        public const string MINIMUM_START_WEIGHT = "2210/0B";        // MSW = Minimum Startgewicht
        public const string DOSING_COUNTER = "2230/05";              // NDS = Dosierzähler
        public const string OPTIMIZATION = "2200/07";                // OSN = Optimierung
        public const string RANGE_SELECTION_PARAMETER = "2200/02";   // RDP = Auswahl Dosierparameter

        public const string REDOSING = "2200/08";                    // RDS = Nachdosieren
        public const string RESIDUAL_FLOW_DOSING_CYCLE = "2000/0F";  // RFO = Nachstrom des letzten Dosierzyklus
        public const string RESIDUAL_FLOW_TIME = "2220/07";          // RFT = Nachstromzeit
        public const string RUN_START_DOSING = "2240/02";            // RUN = Start Dosieren
        public const string SPECIAL_DOSING_FUNCTIONS = "2200/0A";    // SDF = Sonderfunktionen
        public const string MEAN_VALUE_DOSING_RESULTS = "2230/06";   // SDM = Mittelwert Dosieren

        public const string DOSING_STATE_FILLER = "2D00/02";               // SDO = Dosierstatus
        public const string STANDARD_DEVIATION = "2230/07";                // SDS = Standardabweichung
        public const string SETTLING_TIME_TRANSIENT_RESPONSE = "2220/08";  // STT = Beruhigungszeit
        public const string SUM = "2230/08";                               // SUM = Summe
        public const string SYSTEMATIC_DIFFERENCE = "2210/09";             // SYD = Systematische Differenz
        public const string TARE_DELAY = "2220/09";                        // TAD = Tarierverzögerung
        public const string TARE_MODE = "2200/0B";                         // TMD = Tariermodus
        public const string UPPER_TOLERANCE_LIMIT = "2210/0A";             // UTL = Obere Toleranz

        public const string VALVE_CONTROL = "2200/0C";               // VCT = Ventilsteuerung
        public const string WRITE_DOSING_PARAMETER_SET = "2200/01";  // WDP = Dosierparametersatz schreiben
        public const string STORAGE_WEIGHT = "2040/05";              // STO = Gewichtsspeicherung
        public const string STORAGE_WEIGHT_MODE = "2300/08";         // SMD = Modus Gewichtsspeicherung

        #endregion

        #region ID commands for filler extended data

        #endregion

    }
}
