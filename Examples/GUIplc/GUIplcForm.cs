/* @@@@ HOTTINGER BALDWIN MESSTECHNIK - DARMSTADT @@@@@
 * 
 * TCP/MODBUS Interface for WTX120_Modbus | 01/2018
 * 
 * Author : Felix Huettl 
 * 
 *  */
using System;
using System.Windows.Forms;
using System.IO;
using HBM.Weighing.API.WTX;
using HBM.Weighing.API;
using HBM.Weighing.API.WTX.Modbus;
using GUIsimple;
using HBM.Weighing.API.WTX.Jet;
using WTXModbus;

namespace GUIplc
{
    /// <summary>
    /// First, objects of class 'ModbusTcpConnection' and 'WTXModbus' are created to establish a connection and data transfer to the device (WTXModbus, its object is '_wtxDevice'). 
    /// Class 'ModbusTcpConnection' has the purpose to establish a connection, to read from the device (its register)
    /// and to write to the device. Class 'WTXModbus' creates a timer to read and update periodically the values of the WTX in a certain timer
    /// interval given in the constructor of class 'WTXModbus' while generating an object of it. Class 'WTXModbus' has all the values, 
    /// which will be interpreted and class 'WTXModbus' manages the asynchronous data transfer to the GUI and the eventbased data transfer 
    /// to class ModbusTcpConnection. 
    ///  
    /// This class 'GUI' represents a window or a dialog box that makes up the application's user interface for the values and their description of the device.
    /// It uses a datagrid to put the description and the periodically updated values together. The description shown in the form and initialized in
    /// the datagrid is based on the manual (see page manual PLC link on page 154-161). 
    /// Futhermore the data is only displayed, if the values have changed to save reconstruction time on the GUI Form. 
    /// 
    /// Beside a form the GUI could also be a console application by applying the API in program.cs instead of a windows form (see on Git "CommandLine").
    /// Therefore the design of the classes and its relations are seperated in 
    /// connection specific classes and interfaces (class ModbusTcpConnection, interface "IProcessData")
    /// and in a device specific class and in a device specific interface (class "WTXModbus", interface "ProcessData").
    ///  
    /// In the Windows form, there are several buttons to activate events for the output words, a menu bar on the top to start/stop the application, to save the values,
    /// to show help (like the manual) and to change the settings.
    /// The latter is implemented by an additional form and changes the IP address, number of inputs read out by the register, sending/timer interval. 
    /// A status bar on the bottom shows the actually updated status of the connection ("Connected", "IP address", "Mode", "TCP/Modbus", "NumInputs").
    /// </summary>
    partial class GUIplcForm : Form
    {
        #region Locales

        private static BaseWtDevice _wtxDevice;
        
        private SettingsForm _settings;

        private AdjustmentCalculator _adjustmentCalculator;
        private AdjustmentWeigher _adjustmentWeigher;
        private FunctionIO _functionIOForm;

        private string _ApplicationModeStr;
        
        private string _ipAddress;
        private int _timerInterval;

        private int _startIndex ;   // Default setting for standard mode. 
        private int _i;
        private int _arrayLength;

        #endregion

        // Constructor of class GUI for the initialisation: 
        public GUIplcForm(string[] argsParam)
        {
            string[] _args;

            // Initialize the GUI Form: 
            InitializeComponent();           // Call of this method to initialize the form.

            _args = argsParam;

            if (_args.Length > 0)
            {
                if (_args[0] == "modbus" || _args[0] == "Modbus")
                    toolStripStatusLabel6.Text = "Modbus";

                if (_args[0] == "jet" || _args[0] == "Jet")
                    toolStripStatusLabel6.Text = "Jetbus";
            }

            //Get IPAddress and the timer interval from the command line of the VS project arguments (at "Debug").
            if (_args.Length > 1)
            {
                this._ipAddress = _args[1];
            }
            else
            {
                WTXModbus.Properties.Settings.Default.Reload();
                _ipAddress = WTXModbus.Properties.Settings.Default.IPAddress;
            }
            if (_args.Length > 2)
            {
                this._timerInterval = Convert.ToInt32(_args[2]);
            }
            else
            {    
                this._timerInterval = 100; // Default value for the timer interval.
            }

            if(_args.Length==1)
                MessageBox.Show("Bitte geben sie unter 'Edit->Settings' die IP-Adresse ein und auf 'File->Start' für einen Verbindungsaufbau. Es wird mit der 'Default IP-Adresse' fortgefahren.");

            if (_args.Length<=2)
                MessageBox.Show("Bitte geben sie unter 'Edit->Settings' die IP-Adresse ein und auf 'File->Start' für einen Verbindungsaufbau. Es wird mit der 'Default IP-Adresse' fortgefahren." +
                                "Kein Timer-Intervall in der Kommandozeile gegeben, bitte unter Edit->Settings einfügen. Es wird einem Timer-Intervall von 100 msec. fortgefahren.");

        }

        public void setTimerInterval(int timerIntervalParam)
        {
            _wtxDevice.RestartUpdate();
            //_wtxDevice.ResetTimer(timerIntervalParam);
        }

        // This method could also load the datagrid at the beginning of the application: For printing the datagrid on the beginning.
        private void GUI_Load_1(object sender, EventArgs e)
        {
                /*
                Connection establishment : 
                Create objects of ModbusTcpConnection and WtxModbus to establish a connection.
                */

                ModbusTcpConnection _connection = new ModbusTcpConnection(_ipAddress);
                _wtxDevice = new WtxModbus(_connection, this._timerInterval, update);
            
                _ApplicationModeStr = "Standard";

                startToolStripMenuItem_Click(this, new EventArgs());

                _startIndex = 8;   // Default setting for standard mode. 
                _i = 0;
                _arrayLength = 0;
        }
        
        // This method is called from the constructor and sets the columns and the rows of the data grid.
        // There are 2 cases:
        // 1) Standard application : Input words "0+2" till "14". Output words "0" till "50". 
        // 2) Filler   application : Input words "0+2" till "37". Output words "0" till "50". 
        public void set_GUI_rows()
        {
            dataGridView1.Columns.Add("Input:Word_header", "Input:Word");                         // column 1
            dataGridView1.Columns.Add("Input:Name_header", "Input:Name");                         // column 2
            dataGridView1.Columns.Add("Input:Type_header", "Input:Type");                         // column 3
            dataGridView1.Columns.Add("Input:Bit_header" , "Input:Bit");                           // column 4
            dataGridView1.Columns.Add("Input:Interface call routine_header", "Input:Interface call routine"); // column 5
            dataGridView1.Columns.Add("Input:Description_header", "Input:Description");           // column 6
            dataGridView1.Columns.Add("Input:Value_header", "Input:Value");                       // column 7

            dataGridView1.Columns.Add("Output:Word_header", "Output:Word");                       // column 8
            dataGridView1.Columns.Add("Output:Name_header", "Output:Name");                       // column 9
            dataGridView1.Columns.Add("Output:Type_header", "Output:Type");                       // column 10
            dataGridView1.Columns.Add("Output:Bit_header" , "Output:Bit");                         // column 11
            dataGridView1.Columns.Add("Input:Content Content_header", "Input:Content Content");   // column 12
            dataGridView1.Columns.Add("Output:Value_header", "Output:Value");                     // column 13

            if (_wtxDevice.ApplicationMode == ApplicationMode.Standard) // case 1) Standard application. Initializing the description and a placeholder for the values in the data grid.
            {
                dataGridView1.Rows.Add("0", "Measured Value", "Int32", "32Bit", "ProcessData.NetValue", "Net measured", "0", "0", "Control word", "Bit", ".0", "Taring", "Button taring");                                           // row 1 ; dataStr[1]      
                dataGridView1.Rows.Add("2", "Measured Value", "Int32", "32Bit", "ProcessData.GrossValue", "Gross measured", "0", "0", "Control word", "Bit", ".1", "Gross/Net", "Button Gross/Net");                               // row 2 ; dataStr[2]      
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".0", "ProcessData.GeneralWeightError", "General weight error", "0", "0", "Control word", "Bit", ".6", "Zeroing", "Button Zeroing");                    // row 3 ; dataStr[3]

                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".1", "ProcessData.ScaleAlarmTriggered", "Scale alarm(s) triggered", "0", "0", "Control word", "Bit", ".7", "Adjust zero", "Button Adjust zero");         // row 4 ; dataStr[4]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bits", ".2-3", "ProcessData.LimitStatus", "Limit status", "0", "0", "Control word", "Bit", ".8", "Adjust nominal", "Button Adjust nominal");                    // row 5 ; dataStr[5]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".4", "ProcessData.WeightMoving", "Weight moving", "0", "0", "Control word", "Bit", ".11", "Activate data", "Button Activate data");                     // row 6 ; dataStr[6]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".5", "ProcessData.ScaleSealIsOpen", "Scale seal is open", "0", "0", "Control word", "Bit", ".12", "Manual taring", "Button Manual taring");            // row 7 ; dataStr[7]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".6", "ProcessData.ManualTare", "Manual tare", "0", "0", "Control word", "Bit", ".14", "Record weight", "Button Record weight");                          // row 8 ; dataStr[8]

                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".7", "ProcessData.WeightType", "Weight type", "0", "2", "Manual tare value", "S32", ".0-31", "Manual tare value", "0");                               // row 9  ; dataStr[9]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bits", ".8-9", "ProcessData.ScaleRange", "Scale range", "0", "4", "Limit value 1", "U08", ".0-7", "Source 1", "0");                                         // row 10 ; dataStr[10]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".10", "ProcessData.ZeroRequired", "Zero required", "0", "5", "Limit value 1", "U08", ".0-7", "Mode 1", "0");                                         // row 11 ; dataStr[11]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".11", "ProcessData.WeightWithinTheCenterOfZero", "Weight within the center of zero", "0", "6", "Limit value 1", "S32", ".0-31", "Activation level/Lower band limit 1", "0");   // row 12 ; dataStr[12]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".12", "ProcessData.WeightInZeroRange", "Weight in zero range", "0", "8", "Limit value 1", "S32", ".0-31", "Hysteresis/Band height 1", "0");        // row 13 ; dataStr[13]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".0-1", "ProcessData.ApplicationMode", "Application mode", "0", "10", "Limit value 2", "U08", ".0-7", "Source 2", "0");                            // row 14 ; dataStr[14]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".4-6", "ProcessData.Decimals", "Decimals", "0", "11", "Limit value 2", "U08", ".0-7", "Mode 2", "0");                                              // row 15 ; dataStr[15]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".7-8", "ProcessData.Unit", "Unit", "0", "12", "Limit value 2", "S32", ".0-31", "Activation level/Lower band limit 2", "0");                        // row 16 ; dataStr[16]

                dataGridView1.Rows.Add("5", "Measured value status", "Bit", ".14", "ProcessData.Handshake", "Handshake", "0", "14", "Limit value 2", "S32", ".0-31", "Hysteresis/Band height 2", "0");            // row 17 ; dataStr[17] 
                dataGridView1.Rows.Add("5", "Measured value status", "Bit", ".15", "ProcessData.Status", "Status", "0", "16", "Limit value 3", "U08", ".0-7", "Source 3", "0");                                   // row 18 ; dataStr[18]
                dataGridView1.Rows.Add("6", "Digital inputs", "Bit", ".0", "_wtxDevice.input1", "Input 1", "0", "17", "Limit value 3", "U08", ".0-7", "Mode 3", "0");                                            // row 19 ; dataStr[19]

                dataGridView1.Rows.Add("6", "Digital inputs", "Bit", ".1", "_wtxDevice.input2",  "Input 2", "0", "19", "Limit value 3", "S32", ".0-31", "Activation level/Lower band limit 3", "0");      // row 20 ; dataStr[20]
                dataGridView1.Rows.Add("6", "Digital inputs", "Bit", ".2", "_wtxDevice.input3", "Input 3", "0", "20", "Limit value 3", "S32", ".0-31", "Hysteresis/Band height 3", "0");                 // row 21 ; dataStr[21]
                dataGridView1.Rows.Add("6", "Digital inputs", "Bit", ".3", "_wtxDevice.input4", "Input 4", "0", "22", "Limit value 4", "U08", ".0-7", "Source 4", "0");                                  // row 22 ; dataStr[22]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".0", "_wtxDevice.output1", "Output 1", "0", "23", "Limit value 4", "U08", ".0-7", "Mode 4", "0");                                 // row 23 ; dataStr[23]

                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".1", "_wtxDevice.output2", "Output 2", "0", "24", "Limit value 4", "S32", ".0-31", "Activation level/Lower band limit 4", "0");      // row 24 ; dataStr[24]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".2", "_wtxDevice.output3", "Output 3", "0", "26", "Limit value 4", "S32", ".0-31", "Hysteresis/Band height 4", "0");                 // row 25 ; dataStr[25]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".3", "_wtxDevice.output4", "Output 4", "0", "46", "Calibration weight", "S32", ".0-31", "Calibration weight", "Tools Calibration");  // row 26 ; dataStr[26]
                dataGridView1.Rows.Add("8", "Limit value 1", "Bit", ".0", "_wtxDevice.LimitSwitch1", "Limit value 1", "0", "48", "Zero load", "S32", ".0-31", "Zero load", "Tools Calibration");           // row 27 ; dataStr[27]

                dataGridView1.Rows.Add("8", "Limit value 2", "Bit", ".1", "_wtxDevice.LimitSwitch2", "Limit value 2", "0", "50", "Nominal load", "S32", ".0-31", "Nominal load", "Tools Calibration");      // row 28 ; dataStr[28]              
                dataGridView1.Rows.Add("8", "Limit value 3", "Bit", ".2", "_wtxDevice.LimitSwitch3", "Limit value 3", "0", "-", "-", "-", "-", "-", "-", "-", "-", "-");                                     // row 30 ; dataStr[29]
                dataGridView1.Rows.Add("8", "Limit value 4", "Bit", ".3", "_wtxDevice.LimitSwitch4", "Limit value 4", "0", "-", "-", "-", "-", "-", "-", "-", "-");                                          // row 31 ; dataStr[30]
                dataGridView1.Rows.Add("9", "Weight memory, Day", "Int16", ".0-15", "_wtxDevice.weightMemDay", "Stored value for day", "0", "-", "-", "-", "-", "-", "-", "-", "-");                        // row 32 ; dataStr[31]

                dataGridView1.Rows.Add("10", "Weight memory, Month", "Int16", ".0-15", "_wtxDevice.weightMemMonth", "Stored value for month", "0", "-", "-", "-", "-", "-", "-", "-", "-");   // row 33 ; dataStr[32]
                dataGridView1.Rows.Add("11", "Weight memory, Year", "Int16", ".0-15", "_wtxDevice.weightMemYear", "Stored value for year", "0", "-", "-", "-", "-", "-", "-", "-", "-");      // row 34 ; dataStr[33]

                dataGridView1.Rows.Add("12", "Weight memory, Seq...number", "Int16", ".0-15", "_wtxDevice.weightMemSeqNumber", "Stored value for seq.number", "0", "-", "-", "-", "-", "-", "-", "-", "-");       // row 35 ; dataStr[34]
                dataGridView1.Rows.Add("13", "Weight memory, gross", "Int16", ".0-15", "_wtxDevice.weightMemGross", "Stored gross value", "0", "-", "-", "-", "-", "-", "-", "-", "-");                           // row 36 ; dataStr[35]
                dataGridView1.Rows.Add("14", "Weight memory, net", "Int16", ".0-15", "_wtxDevice.weightMemNet", "Stored net value", "0", "-", "-", "-", "-", "-", "-", "-", "-");                                 // row 37 ; dataStr[36]             
            }
            else
            {
                dataGridView1.Rows.Add("0", "Measured Value", "Int32", "32Bit", "ProcessData.NetValue", "Net measured", "0", "0", "Control word", "Bit", ".0", "Taring",  "Button Taring");                               // row 1 ; dataStr[1]      
                dataGridView1.Rows.Add("2", "Measured Value", "Int32", "32Bit", "ProcessData.GrossValue", "Gross measured", "0", "0", "Control word", "Bit", ".1", "Gross/Net", "Button Gross/Net");                      // row 2 ; dataStr[2]

                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".0", "ProcessData.GeneralWeightError", "General weight error", "0", "0", "Control word", "Bit", ".2", "Clear dosing results", "Button Clear dosing results");         // row 3 ; dataStr[3]        
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".1", "ProcessData.ScaleAlarmTriggered", "Scale alarm(s) triggered", "0", "0", "Control word", "Bit", ".3", "Abort dosing", "Button Abort dosing");                    // row 4 ; dataStr[4]

                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bits", ".2-3", "ProcessData.LimitStatus", "Limit status", "0", "0", "Control word", "Bit", ".4", "Start dosing", "Button Start dosing");              // row 5 ; dataStr[5]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".4"   , "ProcessData.WeightMoving", "Weight moving", "0", "0", "Control word", "Bit", ".6", "Zeroing", "Button Zeroing");                         // row 6 ; dataStr[6]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".5"   , "ProcessData.ScaleSealIsOpen", "Scale seal is open", "0", "0", "Control word", "Bit", ".7", "Adjust zero", "Button Adjust zero");       // row 7 ; dataStr[7]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".6"   , "ProcessData.ManualTare", "Manual tare", "0", "0", "Control word", "Bit", ".8", "Adjust nominal", "Button Adjust nominal");               // row 8 ; dataStr[8]

                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".7", "ProcessData.WeightType", "Weight type", "0", "0", "Control word", "Bit", ".11", "Activate data", "Button Activate data");                         // row 9 ;  dataStr[9]
                dataGridView1.Rows.Add("4", "DS461-Weight status","Bits", ".8-9", "ProcessData.ScaleRange",  "Scale range", "0", "0", "Control word", "Bit", ".14", "Record weight", "Button Record weight");                     // row 10 ; dataStr[10]
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".10", "ProcessData.ZeroRequired", "Zero required", "0", "1", "Control word", "Bit", ".15", "Manual re-dosing", "Button Manual re-dosing");             // row 11 ; dataStr[11]           
                dataGridView1.Rows.Add("4", "DS461-Weight status", "Bit", ".11", "ProcessData.WeightWithinTheCenterOfZero", "Weight within the center of zero", "0", "9", "Residual flow time", "U16", ".0-15",  "", "0");    // row 12 ; dataStr[12]
                
                dataGridView1.Rows.Add("4", "DS461-Weight status",  "Bit", ".12", "ProcessData.WeightInZeroRange", "Weight in zero range", "0", "10", "Filling weight", "S32", ".0-31",  "", "0");           // row 13 ; dataStr[13]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".0-1", "ProcessData.ApplicationMode", "Application mode", "0", "12", "Coarse flow cut-off point", "S32", ".0-31",  "", "0");    // row 14 ; dataStr[14]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".4-6", "ProcessData.Decimals", "Decimals", "0", "14", "Fine flow cut-off point", "S32", ".0-31",  "", "0");                      // row 15 ; dataStr[15]
                dataGridView1.Rows.Add("5", "Measured value status", "Bits", ".7-8", "ProcessData.Unit", "Unit", "0", "16", "Minimum fine flow", "S32", ".0-31",  "", "0");                                   // row 16 ; dataStr[16]
                
                dataGridView1.Rows.Add("5", "Measured value status", "Bit", ".14", "ProcessData.Handshake", "Handshake", "0", "18", "Optimization of cut-off points", "U08", ".0-7",  "", "0");  // row 17 ; dataStr[17]
                dataGridView1.Rows.Add("5", "Measured value status", "Bit", ".15", "ProcessData.Status"   , "Status"   , "0", "19", "Maximum dosing time" , "U16" , ".0-15",  "" , "0");                  // row 18 ; dataStr[18]
                dataGridView1.Rows.Add("6", "Digital inputs", "Bit",  ".0", "_wtxDevice.input1", "Input 1"  , "0", "20", "Start with fine flow", "U16" , ".0-15",  "" , "0");                        // row 19 ; dataStr[19]
                dataGridView1.Rows.Add("6", "Digital inputs", "Bit",  ".1", "_wtxDevice.input2", "Input 2"  , "0", "21", "Coarse lockout time" , "U16" , ".0-15",  "" , "0");                         // row 20 ; dataStr[20] 
                
                dataGridView1.Rows.Add("6", "Digital inputs" , "Bit", ".2", "_wtxDevice.input3", "Input 3", "0", "22", "Fine lockout time", "U16", ".0-35",  "", "0");                           // row 21 ; dataStr[21]
                dataGridView1.Rows.Add("6", "Digital inputs" , "Bit", ".3", "_wtxDevice.input4", "Input 4", "0", "23", "Tare mode", "U08", ".0-7",  "", "0");                                    // row 22 ; dataStr[22]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".0", "_wtxDevice.output1", "Output 1", "0", "24", "Tolerance limit +", "S32", ".0-31",  "", "0");                        // row 23 ; dataStr[23]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".1", "_wtxDevice.output2", "Output 2", "0", "26", "Tolerance limit -", "S32", ".0-31",  "", "0");                        // row 24 ; dataStr[24]
                
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".2", "_wtxDevice.output3", "Output 3", "0", "28", "Minimum start weight", "S32", ".0-31",  "", "0");                   // row 25 ; dataStr[25]
                dataGridView1.Rows.Add("7", "Digital outputs", "Bit", ".3", "_wtxDevice.output4", "Output 4", "0", "30", "Empty weight", "S32", ".0-31",  "", "0");                           // row 26 ; dataStr[26] 
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".0", "_wtxDevice.coarseFlow", "Coarse flow", "0", "32", "Tare", "U16", ".0-35",  "", "0");                               // row 27 ; dataStr[27]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".1", "_wtxDevice.fineFlow", "Fine flow", "0", "33", "Coarse flow monitoring time", "U16", ".0-15",  "", "0");            // row 28 ; dataStr[28]

                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".2", "_wtxDevice.ready", "Ready", "0", "34", "Coarse flow monitoring", "U32", ".0-31",  "", "0");                            // row 29 ; dataStr[29]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".3", "_wtxDevice.reDosing", "Re-dosing", "0", "36", "Fine flow monitoring", "U32", ".0-31",  "", "0");                       // row 30 ; dataStr[30]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".4", "_wtxDevice.emptying", "Emptying", "0", "38", "Fine flow monitoring time", "U16", ".0-15",  "", "0");                   // row 31 ; dataStr[31]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".5", "_wtxDevice.flowError", "Flow error", "0", "39", "Delay time after fine flow", "U08", ".0-7",  "", "0");                // row 32 ; dataStr[32]
               
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".6", "_wtxDevice.alarm", "Alarm", "0", "40", "Activation time after fine flow", "U08", ".0-7",  "", "0");                                     // row 33 ; dataStr[33]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".7", "_wtxDevice.ADC_OverUnderload", "ADC-Overload/Underload", "0", "41", "Systematic difference", "U32", ".0-31",  "", "0");                 // row 34 ; dataStr[34]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".8", "_wtxDevice.maxDosingTime", "Max. Dosing time", "0", "42", "Downwards dosing", "U08", ".0-7",  "", "0");                                 // row 35 ; dataStr[35]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".9", "_wtxDevice.legalTradeOP", "Legal-for-trade operation", "0", "43", "Valve control", "U08", ".0-7",  "", "0");                            // row 36 ; dataStr[36]
                
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".10", "_wtxDevice.toleranceErrorPlus", "Tolerance error +", "0",  "44", "Emptying mode","U08", ".0-7" ,     "", "0");                             // row 37 ; dataStr[37]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".11", "_wtxDevice.toleranceErrorMinus", "Tolerance error -", "0", "46", "Calibration weight" , "S32"  ,".0-31",  "", "Tools calibration");      // row 38 ; dataStr[38]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".14", "_wtxDevice.statusInput1", "Status digital input 1"  , "0", "48", "Zero load"   , "S32", ".0-31", ""    , "Tools calibration");                    // row 39 ; dataStr[39]
                dataGridView1.Rows.Add("8", "Dosing status", "Bit", ".15", "_wtxDevice.generalScaleError", "General scale error", "0", "50", "Nominal load", "S32", ".0-31", ""    , "Tools calibration");               // row 40 ; dataStr[40]
               
                dataGridView1.Rows.Add("9", "Dosing process status", "U16", ".0-15", "_wtxDevice.fillingProcessStatus", "Initializing,Pre-dosing to Analysis", "0", "-", "-", "-", "-", "-", "-", "-", "-");     // row 41 ; dataStr[41]
                dataGridView1.Rows.Add("11", "Dosing count", "U16", ".0-15", "_wtxDevice.numberDosingResults",  " ", "0", "-");                                                                    // row 43 ; dataStr[42]
                dataGridView1.Rows.Add("12", "Dosing result", "S32", ".0-31", "_wtxDevice.dosingResult", " ", "0", "-", "-", "-", "-", "-", "-", "-", "-");                                       // row 44 ; dataStr[43]

                dataGridView1.Rows.Add("14", "Mean value of dosing results", "S32"    , ".0-31", "_wtxDevice.meanValueDosingResults", " ", "0", "-", "-", "-", "-", "-", "-", "-");                   // row 45 ; dataStr[44]
                dataGridView1.Rows.Add("16", "Standard deviation" , "S32"  , ".0-31"  , "_wtxDevice.standardDeviation", " ", "0", "-", "-", "-", "-", "-", "-", "-");                                  // row 46 ; dataStr[45]
                dataGridView1.Rows.Add("18", "Total weight", "S32", ".0-31", "_wtxDevice.totalWeight", " ", "0", "-", "-", "-", "-", "-", "-", "-");                                              // row 47 ; dataStr[46]
                dataGridView1.Rows.Add("20", "Fine flow cut-off point", "S32", ".0-31", "_wtxDevice.fineFlowCutOffPoint", " ", "0", "-", "-", "-", "-", "-", "-", "-");                           // row 48 ; dataStr[47]
               
                dataGridView1.Rows.Add("22", "Coarse flow cut-off point", "S32", ".0-31", "_wtxDevice.coarseFlowCutOffPoint", " ", "0", "-", "-", "-", "-", "-", "-", "-");                       // row 49 ; dataStr[48]
                dataGridView1.Rows.Add("24", "Actual dosing time"     , "U16"  , ".0-15", "_wtxDevice.actualDosingTime", " ", "0", "-", "-", "-", "-", "-", "-", "-");                                   // row 50 ; dataStr[49]
                dataGridView1.Rows.Add("25", "Actual coarse flow time", "U16"  , ".0-15", "_wtxDevice.actualCoarseFlowTime", " ", "0", "-", "-", "-", "-", "-", "-", "-");                          // row 51 ; dataStr[50]
                dataGridView1.Rows.Add("26", "Actual fine flow time"  , "U16"  , ".0-15", "_wtxDevice.actualFineFlowTime", " ", "0", "-", "-", "-", "-", "-", "-", "-");                              // row 52 ; dataStr[51]
                
                dataGridView1.Rows.Add("27", "Parameter set (product)", "U08"  , ".0-7" , "_wtxDevice.parameterSetProduct", " ", "0", "-", "-", "-", "-", "-", "-", "-");                            // row 53 ; dataStr[52]
                dataGridView1.Rows.Add("32", "Weight memory, Day"     , "Int16", ".0-15", "_wtxDevice.weightMemDay", "Stored value for day", "0", "-", "-", "-", "-", "-", "-", "-");                  // row 54 ; dataStr[53]
                dataGridView1.Rows.Add("33", "Weight memory, Month"   , "Int16", ".0-15", "_wtxDevice.weightMemMonth", "Stored value for month", "0", "-", "-", "-", "-", "-", "-", "-");            // row 55 ; dataStr[54]
                dataGridView1.Rows.Add("34", "Weight memory, Year"    , "Int16", ".0-15", "_wtxDevice.weightMemYear",  "Stored value for year", "0", "-", "-", "-", "-", "-", "-", "-");               // row 56 ; dataStr[55]
               
                dataGridView1.Rows.Add("35", "Weight memory, Seq number", "Int16", ".0-15", "_wtxDevice.weightSeqNumber", "Stored value for seq.number", "0", "-", "-", "-", "-", "-", "-", "-"); // row 57 ; dataStr[56]
                dataGridView1.Rows.Add("36", "Weight memory, gross"     , "Int16", ".0-15", "_wtxDevice.weightMemGross", "Stored gross value", "0", "-", "-", "-", "-", "-", "-", "-");                // row 58 ; dataStr[57]
                dataGridView1.Rows.Add("37", "Weight memory, net"       , "Int16", ".0-15", "_wtxDevice.weightMemDayNet", "Stored net value"  , "0", "-", "-", "-", "-", "-", "-", "-");                   // row 59 ; dataStr[58]

            }

            label1.Text = "Only for Standard application:";       // label for information : Only output words for standard application
            label2.Text = "Only for Filler application:";         // label for information : Only output words for filler application 
            toolStripStatusLabel5.Text = "38";

            if (_wtxDevice.Connection.IsConnected == true)
                toolStripStatusLabel1.Text = "Connected";
            else
                toolStripStatusLabel1.Text = "Disconnected";

            dataGridView1.Columns[4].Width = 250;                 // Width of the fourth column containing the periodically updated values.           
        }

        // This automatic property returns an instance of this class. It has usage in the class "Settings_Form".
        public BaseWtDevice GetDataviewer
        {
            get
            {
                return _wtxDevice;
            }
        }

        // This private method is called for initializing basic information for the tool menu bar on the bottom of the windows form: 
        // For the connection status, IP address, application mode and number of inputs. 
        private void GUI_Load(object sender, EventArgs e)
        {
            if (_wtxDevice.Connection.IsConnected == true)
                toolStripStatusLabel1.Text = "Connected";
            else
                toolStripStatusLabel1.Text = "Disconnected";

            toolStripStatusLabel2.Text = "IP address: " + _wtxDevice.Connection.IpAddress;
            toolStripStatusLabel3.Text = "Mode : " + this._ApplicationModeStr; // index 14 refers to application mode of the Device
        }

        // Button-Click event to close the application: 
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
        // This method sends a command to the device : Taring. Command : 0x1       
        // For standard and filler application.
        private void button4_Click(object sender, EventArgs e)
        {
            // Taring       
            _wtxDevice.Tare();
        }

        // This method sends a command to the device : Change between gross and net value. Command : 0x2 
        // For standard and filler application.
        private void button1_Click(object sender, EventArgs e)
        {
            // Gross/Net
            _wtxDevice.SetGross();

        }

        
        // This method sends a command to the device : Zeroing. Command : 0x40
        // For standard and filler application.
        private void button5_Click(object sender, EventArgs e)
        {
            // Zeroing
            _wtxDevice.Zero();
        }

        // This method sends a command to the device : Adjust zero. Command : 0x80
        // For standard and filler application.
        private void button6_Click(object sender, EventArgs e)
        {
            // Adjust zero
            _wtxDevice.adjustZero();
        }

        // This method sends a command to the device : Adjust nominal. Command : 0x100
        // For standard and filler application.
        private void button7_Click(object sender, EventArgs e)
        {
            // Adjust nominal
            _wtxDevice.adjustNominal();
        }

        // This method sends a command to the device : Activate data. Command : 0x800
        // For standard and filler application.
        // If the button 'Activate data' is clicked the output words are entered into the datagrid in column ...
        // ... 'Output:value' from word 2 to 26 (standard mode) and from word 9 to 44 are written into the WTX device. 
        private void button8_Click(object sender, EventArgs e)
        {
            // Activate data :
            int maximumIndex = 0;

            if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)     // if in standard mode: 
            {
                _startIndex = 8;
                _arrayLength = 17;
                maximumIndex = 25;
            }
            else
            {
                _startIndex  = 11;
                _arrayLength = 26;
                maximumIndex = 36;
            }

            ushort[] valueArr = new ushort[_arrayLength];

            for (int index = _startIndex; index < maximumIndex; index++)  // In Filler mode: From index 11 to the maximum row number.In Standard mode: From index 8 to the maximum row number.
            {
                _i = index - _startIndex;
                
                var input= dataGridView1.Rows[index].Cells[12].Value;
                valueArr[_i] = (ushort)Convert.ToInt16(input);

                string inputStr = input.ToString();

                // Writing values to the WTX according to the data type : S32 or U08 or U16 (given in the GUI datagrid).
                if (inputStr != "0")
                {
                    valueArr[_i] = (ushort)Convert.ToInt32(dataGridView1.Rows[index].Cells[6].Value);
                }
            }
            //_wtxDevice.UpdateOutputWords(valueArr);

            _wtxDevice.activateData(); 
        }       

        // This method sends a command to the device : Manual taring. Command : 0x1000
        // Only for standard application.
        private void button9_Click(object sender, EventArgs e)
        {
            // Manual taring
            //if (this.is_standard == true)      // Activate this if-conditon only in case, if the should be a change between standard and filler application. 
            _wtxDevice.manualTaring();
        }

        // This method sends a command to the device : Clear dosing results. Command : 0x4
        // Only for filler application.
        private void button11_Click_1(object sender, EventArgs e)
        {
            // Clear dosing results
            //if (this.is_standard == false)
            _wtxDevice.clearDosingResults();
        }

        // This method sends a command to the device : Abort dosing. Command : 0x8
        // Only for filler application.
        private void button12_Click_1(object sender, EventArgs e)
        {
            // Abort dosing
            //if (this.is_standard == false)
            _wtxDevice.abortDosing();
        }

        // This method sends a command to the device : Start dosing. Command : 0x10
        // Only for filler application.
        private void button13_Click_1(object sender, EventArgs e)
        {
            // Start dosing
            //if (this.is_standard == false)
            _wtxDevice.startDosing();
        }

        // This method sends a command to the device : Record weight. Command : 0x4000 , Bit .14
        // For standard and filler application.
        private void button2_Click_1(object sender, EventArgs e)
        {
            // Record weight: 
            //if (this.is_standard == false)
            _wtxDevice.recordWeight();

        }

        // This method sends a command to the device : Manual re-dosing. Command : 0x8000
        // Only for filler application.
        private void button14_Click_1(object sender, EventArgs e)
        {
            // Manual re-dosing
            //if (this.is_standard == false)
            _wtxDevice.manualReDosing();
        }

        // This event starts the timer and the periodical fetch of values from the device (here: WTX120_Modbus).
        // The timer interval is set in the connection specific class "ModbusTCPConnection".
        // For the application mode(standard or filler) and the printing on the GUI the WTX registers are read out first. 
        private void startToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // The ip address is actualized.
            _wtxDevice.Connection.IpAddress = this._ipAddress;

            // The connection to the device is established. 
            _wtxDevice.Connection.Connect();     // Alternative : _wtxObj.Connect();    

            // For the application mode(standard or filler) and the printing on the GUI the WTX registers are read out first.      
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            this.set_GUI_rows();
            
            //_wtxDevice.ProcessDataReceived += ValuesOnConsole;

            // New eventhandler for a change in a data grid cell : 
            dataGridView1.CellValueChanged += new DataGridViewCellEventHandler(GridValueChangedMethod);

        }

        private LimitSwitchMode _limitMode;
        private LimitSwitchSource _limitSource;

        // This method is set if the output value in column 13 has changed - For writing output words the standard and filler mode: 
        private void GridValueChangedMethod(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 12)
            {
                ushort value = 0;
                ushort index = 0;

                string valueStr = "";

                // For standard mode : 
                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)
                {
                    if (e.RowIndex >= 8 && e.RowIndex <= 24)
                    {
                        valueStr = dataGridView1[e.ColumnIndex, e.RowIndex].Value.ToString();

                        switch (valueStr)
                        {
                            case "Above Level":
                                _limitMode = LimitSwitchMode.AboveLevel;
                                value = 0; break;
                            case "Below Level":
                                _limitMode = LimitSwitchMode.BelowLevel;
                                value = 1; break;
                            case "Outside Band":
                                _limitMode = LimitSwitchMode.OutsideBand;
                                value = 2; break;
                            case "Inside Band":
                                _limitMode = LimitSwitchMode.InsideBand;
                                value = 3; break;
                            case "Net=1":
                                _limitSource = LimitSwitchSource.Net;
                                value = 1;
                                break;
                            case "Gross=2":
                                _limitSource = LimitSwitchSource.Gross;
                                value = 2;
                                break;
                            default:
                                if (valueStr.Contains("."))
                                    value = (ushort)Convert.ToInt32(valueStr.Replace(".", ""));
                                else
                                    value = (ushort)Convert.ToInt32(valueStr);
                                break;
                        }

                        // For source 1,2,3,4 :
                        if (e.RowIndex == 9 || e.RowIndex == 13 || e.RowIndex == 17 || e.RowIndex == 21)
                        {
                            switch (value)
                            {
                                case 0:
                                    _limitMode = LimitSwitchMode.AboveLevel; break;
                                case 1:
                                    _limitMode = LimitSwitchMode.BelowLevel; break;
                                case 2:
                                    _limitMode = LimitSwitchMode.OutsideBand; break;
                                case 3:
                                    _limitMode = LimitSwitchMode.InsideBand; break;

                                default: break;
                            }

                            switch (_limitMode)
                            {
                                case LimitSwitchMode.AboveLevel:  dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Above Level"; break;
                                case LimitSwitchMode.BelowLevel:  dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Below Level"; break;
                                case LimitSwitchMode.OutsideBand: dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Outside Band"; break;
                                case LimitSwitchMode.InsideBand:  dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Inside Band"; break;

                                default: dataGridView1[e.ColumnIndex, e.RowIndex].Value = "default"; break;
                            }
                        }

                        // For mode 1, 2 : 
                        if (e.RowIndex == 10 || e.RowIndex == 14 || e.RowIndex == 18 || e.RowIndex == 22)
                        {
                            if (value == 1)
                                _limitSource = LimitSwitchSource.Net;
                            else 
                                if(value == 2)
                                    _limitSource = LimitSwitchSource.Gross;

                            if (_limitSource == LimitSwitchSource.Net)
                                dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Net=1";
                            else
                                if (_limitSource == LimitSwitchSource.Gross)
                                    dataGridView1[e.ColumnIndex, e.RowIndex].Value = "Gross=2";
                        }
                    }
                    else
                        MessageBox.Show("Bitte in den vorgegebenen Feldern eingeben für standard application.");
                }              
                else // if application mode = filler
                {
                    if (e.RowIndex >= 11 && e.RowIndex <= 36)
                    {
                        if (valueStr.Contains("."))
                        {
                            value = (ushort)Convert.ToInt32(valueStr.Replace(".", ""));
                        }
                        else
                            value = (ushort)Convert.ToInt32(valueStr);
                    }
                    else
                        MessageBox.Show("Bitte in den vorgegebenen Feldern eingeben für filler application.");
                }
                
                index = (ushort)Convert.ToInt16(dataGridView1[7, e.RowIndex].Value); // For the index, the word number which should be written to the WTX device 

                // For the standard application: 
                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)
                {
                    if (e.RowIndex >= 8 && e.RowIndex <= 24)
                    {
                        // If the specific cell of the row 8,9,10 till row 24 has changed, write the value to the specific properties. 

                        switch (e.RowIndex)
                        {
                            case 8: _wtxDevice.ManualTareValue = value; break;

                            case 9: _wtxDevice.DataStandard.LimitSwitch1Source = value; break;
                            case 10: _wtxDevice.DataStandard.LimitSwitch1Mode = value; break;
                            case 11: _wtxDevice.DataStandard.LimitSwitch1Level = value; break;
                            case 12: _wtxDevice.DataStandard.LimitSwitch1Hysteresis = value; break;

                            case 13: _wtxDevice.DataStandard.LimitSwitch2Source = value; break;
                            case 14: _wtxDevice.DataStandard.LimitSwitch2Mode = value; break;
                            case 15: _wtxDevice.DataStandard.LimitSwitch2Level = value; break;
                            case 16: _wtxDevice.DataStandard.LimitSwitch2Hysteresis = value; break;

                            case 17: _wtxDevice.DataStandard.LimitSwitch3Source = value; break;
                            case 18: _wtxDevice.DataStandard.LimitSwitch3Mode = value; break;
                            case 19: _wtxDevice.DataStandard.LimitSwitch3ActivationLevelLowerBandLimit = value; break;
                            case 20: _wtxDevice.DataStandard.LimitSwitch3Hysteresis = value; break;

                            case 21: _wtxDevice.DataStandard.LimitSwitch4Source = value; break;
                            case 22: _wtxDevice.DataStandard.LimitSwitch4Mode = value; break;
                            case 23: _wtxDevice.DataStandard.LimitSwitch4Level = value; break;
                            case 24: _wtxDevice.DataStandard.LimitSwitch4Hysteresis = value; break;

                            default: break;
                        }
                    }
                }

                else
                {
                    if (e.RowIndex >= 11 && e.RowIndex <= 36)
                    {
                        switch (e.RowIndex)
                        {
                            case 11: _wtxDevice.DataFiller.ResidualFlowTime = value;           break;
                            case 12: _wtxDevice.DataFiller.TargetFillingWeight = value;        break;
                            case 13: _wtxDevice.DataFiller.CoarseFlowCutOffPointSet = value;   break;
                            case 14: _wtxDevice.DataFiller.FineFlowCutOffPointSet = value;     break;
                            case 15: _wtxDevice.DataFiller.MinimumFineFlow = value;            break;
                            case 16: _wtxDevice.DataFiller.OptimizationOfCutOffPoints = value; break;

                            case 17: _wtxDevice.DataFiller.MaximumDosingTime = value;   break;
                            case 18: _wtxDevice.DataFiller.StartWithFineFlow = value;   break;
                            case 19: _wtxDevice.DataFiller.CoarseLockoutTime = value;   break;
                            case 20: _wtxDevice.DataFiller.FineLockoutTime = value;     break;
                            case 21: _wtxDevice.DataFiller.TareMode = value;            break;
                            case 22: _wtxDevice.DataFiller.UpperToleranceLimit = value; break;
                            case 23: _wtxDevice.DataFiller.LowerToleranceLimit = value; break;
                            case 24: _wtxDevice.DataFiller.MinimumStartWeight = value;  break;

                            case 25: _wtxDevice.DataFiller.EmptyWeight = value; break;
                            case 26: _wtxDevice.DataFiller.TareDelay = value;   break;
                            case 27: _wtxDevice.DataFiller.CoarseFlowMonitoringTime = value; break;
                            case 28: _wtxDevice.DataFiller.CoarseFlowMonitoring = value;     break;
                            case 29: _wtxDevice.DataFiller.FineFlowMonitoring = value;       break;
                            case 30: _wtxDevice.DataFiller.FineFlowMonitoringTime = value;   break;

                            case 31: _wtxDevice.DataFiller.DelayTimeAfterFineFlow = value;      break;
                            case 32: _wtxDevice.DataFiller.ActivationTimeAfterFineFlow = value; break;
                            case 33: _wtxDevice.DataFiller.SystematicDifference = value;        break;
                            case 34: _wtxDevice.DataFiller.DownwardsDosing = value; break;
                            case 35: _wtxDevice.DataFiller.ValveControl = value;    break;
                            case 36: _wtxDevice.DataFiller.EmptyingMode = value;    break;
                        }
                    }
                }
                _wtxDevice.activateData();
            }
        }     

        /// <summary>
        /// This method actualizes and resets the data grid with newly calculated values of the previous iteration. 
        /// First it actualizes the tool bar menu regarding the status of the connection, afterwards it iterates the
        /// "dataStr" array to actualize every element of the data grid in the standard or filler application. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void update(object sender, ProcessDataReceivedEventArgs e)
        {
            if (_wtxDevice.ApplicationMode == ApplicationMode.Filler)
                _ApplicationModeStr = "Filler";
            else
                _ApplicationModeStr = "Standard";


            if (_wtxDevice.Connection.IsConnected == true)
                    toolStripStatusLabel1.Text = "Connected";
                if (_wtxDevice.Connection.IsConnected == false)
                    toolStripStatusLabel1.Text = "Disconnected";

                toolStripStatusLabel2.Text = "IP address: " + _wtxDevice.Connection.IpAddress;
                toolStripStatusLabel3.Text = "Mode : " + this._ApplicationModeStr;
                toolStripStatusLabel2.Text = "IP address: " + _wtxDevice.Connection.IpAddress;

                //Changing the width of a column:
                /*foreach (DataGridViewTextBoxColumn c in dataGridView1.Columns)
                    c.Width = 120;*/
                try
                {
                    dataGridView1.Rows[0].Cells[6].Value = _wtxDevice.CurrentWeight(e.ProcessData.NetValue, e.ProcessData.Decimals);
                    dataGridView1.Rows[1].Cells[6].Value = _wtxDevice.CurrentWeight(e.ProcessData.GrossValue, e.ProcessData.Decimals);
                    dataGridView1.Rows[2].Cells[6].Value = e.ProcessData.GeneralWeightError;
                    dataGridView1.Rows[3].Cells[6].Value = e.ProcessData.ScaleAlarm;
                    dataGridView1.Rows[4].Cells[6].Value = e.ProcessData.LimitStatus;
                    dataGridView1.Rows[5].Cells[6].Value = e.ProcessData.WeightMoving;
                    dataGridView1.Rows[6].Cells[6].Value = e.ProcessData.ScaleSealIsOpen;
                    dataGridView1.Rows[7].Cells[6].Value = e.ProcessData.ManualTare;
                    dataGridView1.Rows[8].Cells[6].Value = e.ProcessData.TareMode;
                    dataGridView1.Rows[9].Cells[6].Value = e.ProcessData.ScaleRange;
                    dataGridView1.Rows[10].Cells[6].Value = e.ProcessData.ZeroRequired;
                    dataGridView1.Rows[11].Cells[6].Value = e.ProcessData.CenterOfZero;
                    dataGridView1.Rows[12].Cells[6].Value = e.ProcessData.InsideZero;
                    dataGridView1.Rows[13].Cells[6].Value = _wtxDevice.ApplicationMode.ToString();
                    dataGridView1.Rows[14].Cells[6].Value = e.ProcessData.Decimals;
                    dataGridView1.Rows[15].Cells[6].Value = e.ProcessData.Unit;
                    //dataGridView1.Rows[16].Cells[6].Value = e.ProcessData.Handshake;
                    //dataGridView1.Rows[17].Cells[6].Value = e.ProcessData.Status;

                    dataGridView1.Rows[18].Cells[6].Value = _wtxDevice.DataStandard.Input1;
                    dataGridView1.Rows[19].Cells[6].Value = _wtxDevice.DataStandard.Input2;
                    dataGridView1.Rows[20].Cells[6].Value = _wtxDevice.DataStandard.Input3;
                    dataGridView1.Rows[21].Cells[6].Value = _wtxDevice.DataStandard.Input4;
                    dataGridView1.Rows[22].Cells[6].Value = _wtxDevice.DataStandard.Output1;
                    dataGridView1.Rows[23].Cells[6].Value = _wtxDevice.DataStandard.Output2;
                    dataGridView1.Rows[24].Cells[6].Value = _wtxDevice.DataStandard.Output3;
                    dataGridView1.Rows[25].Cells[6].Value = _wtxDevice.DataStandard.Output4;
            }
                catch (Exception) { }

                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)             // In the standard application: 
                {
                    try
                    {
                        // Index 27 bis 35:
                        dataGridView1.Rows[26].Cells[6].Value = _wtxDevice.DataStandard.LimitStatus1;
                        dataGridView1.Rows[27].Cells[6].Value = _wtxDevice.DataStandard.LimitStatus2;
                        dataGridView1.Rows[28].Cells[6].Value = _wtxDevice.DataStandard.LimitStatus3;
                        dataGridView1.Rows[29].Cells[6].Value = _wtxDevice.DataStandard.LimitStatus4;
                        dataGridView1.Rows[30].Cells[6].Value = _wtxDevice.DataStandard.WeightMemDay;
                        dataGridView1.Rows[31].Cells[6].Value = _wtxDevice.DataStandard.WeightMemMonth;
                        dataGridView1.Rows[32].Cells[6].Value = _wtxDevice.DataStandard.WeightMemYear;
                        dataGridView1.Rows[33].Cells[6].Value = _wtxDevice.DataStandard.WeightMemSeqNumber;
                        dataGridView1.Rows[34].Cells[6].Value = _wtxDevice.DataStandard.WeightMemGross;
                        dataGridView1.Rows[35].Cells[6].Value = _wtxDevice.DataStandard.WeightMemNet;
                }
                    catch (Exception) { }
                }
                else
                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)   // In the filler application: 
                {
                    try
                    {
                    // Index 27 bis 55: 
                    dataGridView1.Rows[26].Cells[6].Value = _wtxDevice.DataFiller.CoarseFlow;
                    dataGridView1.Rows[27].Cells[6].Value = _wtxDevice.DataFiller.FineFlow;
                    dataGridView1.Rows[28].Cells[6].Value = _wtxDevice.DataFiller.Ready;
                    dataGridView1.Rows[29].Cells[6].Value = _wtxDevice.DataFiller.ReDosing;
                    dataGridView1.Rows[30].Cells[6].Value = _wtxDevice.DataFiller.Emptying;
                    dataGridView1.Rows[31].Cells[6].Value = _wtxDevice.DataFiller.FlowError;
                    dataGridView1.Rows[32].Cells[6].Value = _wtxDevice.DataFiller.Alarm;
                    dataGridView1.Rows[33].Cells[6].Value = _wtxDevice.DataFiller.AdcOverUnderload;
                    dataGridView1.Rows[34].Cells[6].Value = _wtxDevice.DataFiller.MaximumDosingTime;
                    //dataGridView1.Rows[35].Cells[6].Value = _wtxDevice.DataFiller.LegalForTradeOperation;
                    dataGridView1.Rows[36].Cells[6].Value = _wtxDevice.DataFiller.ToleranceErrorPlus;
                    dataGridView1.Rows[37].Cells[6].Value = _wtxDevice.DataFiller.ToleranceErrorMinus;
                    dataGridView1.Rows[38].Cells[6].Value = _wtxDevice.DataFiller.StatusInput1;
                    dataGridView1.Rows[39].Cells[6].Value = _wtxDevice.DataFiller.GeneralScaleError;
                    dataGridView1.Rows[40].Cells[6].Value = _wtxDevice.DataFiller.FillingProcessStatus;
                    dataGridView1.Rows[41].Cells[6].Value = _wtxDevice.DataFiller.NumberDosingResults;
                    dataGridView1.Rows[42].Cells[6].Value = _wtxDevice.DataFiller.DosingResult;
                    dataGridView1.Rows[43].Cells[6].Value = _wtxDevice.DataFiller.MeanValueDosingResults;
                    dataGridView1.Rows[44].Cells[6].Value = _wtxDevice.DataFiller.StandardDeviation;
                    dataGridView1.Rows[45].Cells[6].Value = _wtxDevice.DataFiller.TotalWeight;
                    dataGridView1.Rows[46].Cells[6].Value = _wtxDevice.DataFiller.FineFlowCutOffPoint;
                    dataGridView1.Rows[47].Cells[6].Value = _wtxDevice.DataFiller.CoarseFlowCutOffPoint;
                    dataGridView1.Rows[48].Cells[6].Value = _wtxDevice.DataFiller.CurrentDosingTime;
                    dataGridView1.Rows[49].Cells[6].Value = _wtxDevice.DataFiller.CurrentCoarseFlowTime;
                    dataGridView1.Rows[50].Cells[6].Value = _wtxDevice.DataFiller.CurrentFineFlowTime ;
                    dataGridView1.Rows[51].Cells[6].Value = _wtxDevice.DataFiller.ParameterSetProduct;

                    dataGridView1.Rows[52].Cells[6].Value = _wtxDevice.DataStandard.WeightMemDay;
                    dataGridView1.Rows[53].Cells[6].Value = _wtxDevice.DataStandard.WeightMemMonth;
                    dataGridView1.Rows[54].Cells[6].Value = _wtxDevice.DataStandard.WeightMemYear;
                    dataGridView1.Rows[55].Cells[6].Value = _wtxDevice.DataStandard.WeightMemSeqNumber;
                    dataGridView1.Rows[56].Cells[6].Value = _wtxDevice.DataStandard.WeightMemGross;
                    dataGridView1.Rows[57].Cells[6].Value = _wtxDevice.DataStandard.WeightMemNet;
                }
                    catch (Exception) { }
                }           
        }

        // This method stops the timer after the corresponding event has been triggered during the application.
        // Afterwards the timer and the application can be restarted.
        private void stopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //_wtxDevice.ProcessDataReceived -= ValuesOnConsole;
            toolStripStatusLabel1.Text = "Disconnected";    
        }

        // This method stops the timer and exits the application after the corresponding event has been triggered during the application.
        // Afterwards the timer and the application can not be restarted.
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Disconnected";

            //_wtxDevice.ProcessDataReceived -= ValuesOnConsole;
            this.Close();
            
            Application.Exit();
        }

        // This method saves the values from the GUI in the actual iteration in an extra word file: 
        private void saveInputToolStripMenuItem_Click(object sender, EventArgs e)
        {     
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();

            saveFileDialog1.Filter = "Word Document|*.rtf";
            saveFileDialog1.Title = "Save input and output words";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {

                string fullPath = Path.Combine(System.IO.Path.GetDirectoryName(saveFileDialog1.FileName), "\\", saveFileDialog1.FileName);

                System.IO.File.Delete(fullPath);

                System.IO.File.AppendAllText(fullPath, " PLC Interface , Input words WTX120_Modbus -> SPS , Application : " + this._ApplicationModeStr + "\n\n\n");// index 14 ("this.dataStr[14]") refers to application mode of the Device
                System.IO.File.AppendAllText(fullPath, "\nWord|\n" + "Name|\n" + "Type|\n" + "Bit|\n" + "Interface Call Routine|\n" + "List Call Routine|\n" + "Content|\n" + "Value\n\n");

                for (int x=0;x < (dataGridView1.RowCount-1); x++)       // Iterating through the whole data grid: 
                    for(int y = 0; y < (dataGridView1.ColumnCount); y++)
                    {
                        dataGridView1.CurrentCell = dataGridView1.Rows[x].Cells[y];    // Writes the descriptions and values into a word file.
                        System.IO.File.AppendAllText(fullPath, "\n" + dataGridView1.CurrentCell.Value.ToString());
                    }
                
            }
        }

        // This method is used to call another form ("Settings_Form") once the corresponding event is triggerred.
        // It is used to change the connection specific attributes, like IP address, number of inputs and sending/timer interval.
        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;     // Stop the timer (Restart is in Class "Settings_Form").
            timer1.Stop();
                  
            _settings = new SettingsForm(_wtxDevice.Connection.IpAddress, this.timer1.Interval);
            _settings.ValuesChanged += UpdateSettings;
            _settings.Show();
        }

        // This method updates the values of the connection(IP address, timer/sending interval, number of inputs), set in class "Settings_Form".
        // See class "Setting_Form" in method button2_Click(sender,e).
        // After updating the values the tool bar labels on the bottom (f.e. "toolStripStatusLabel2") is rewritten with the new values. 
        public void UpdateSettings(object sender, SettingsEventArgs e)
        {
            toolStripStatusLabel2.Text = "IP address: " + e.IPAdress;

            if (this._ipAddress != e.IPAdress)
            {
                this._ipAddress = e.IPAdress;
                _wtxDevice.Connection.IpAddress = e.IPAdress; // Alternative : _wtxDevice.Connection.IpAddress=_settings.GetIpAddress;
                _wtxDevice.Connect(5000);
            }

            if (this.timer1.Interval != e.TimerInterval)
            {
                this.timer1.Interval = e.TimerInterval;
                _wtxDevice.RestartUpdate();
                //_wtxDevice.ResetTimer(timerIntervalParam);
            }
            // timer update missing
        }

        // This method changes the GUI concerning the application mode.
        // If the menu item "Standard" in the menu bar item "View appllication" is selected, the GUI shows the description
        // of the standard application. This is only for the purpose of testing. The values of the device is not changed, only
        // the GUI. 
        private void standardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            this.set_GUI_rows();
        }

        // This method changes the GUI concerning the application mode.
        // If the menu item "Filler" in the menu bar item "View appllication" is selected, the GUI shows the description
        // of the standard application. This is only for the purpose of testing. The values of the device is not changed, only
        // the GUI. 
        private void fillerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Columns.Clear();
            dataGridView1.Rows.Clear();
            this.set_GUI_rows();
        }
        
        /*
         *  This method is called once the tool item "Calculate Calibration" is clicked. It creates a windows form for
         *  the calibration with a dead load and a nominal span. 
         */
        private void calculateCalibrationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _wtxDevice.StopUpdate();

            _adjustmentCalculator = new AdjustmentCalculator(_wtxDevice);
            DialogResult res = _adjustmentCalculator.ShowDialog();

            _wtxDevice.RestartUpdate();
        }

        /*
         *  This method is called once the tool item "Calibration with weight" is clicked. It creates a windows form for
         *  the calibration with an individual weight put on the load cell or sensor. 
         */
        private void calibrationWithWeightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _wtxDevice.StopUpdate();

            _adjustmentWeigher = new AdjustmentWeigher(_wtxDevice);
            DialogResult res = _adjustmentWeigher.ShowDialog();

            _wtxDevice.RestartUpdate();
        }
        // This button event resets the calibration to the following default setting : 
        // Dead load = 0 mv/V
        // Span (Nominal load) = 2 mV/V
        private void button3_Click(object sender, EventArgs e)
        {
            _wtxDevice.StopUpdate();

            _wtxDevice.Calculate(0, 2);

            _wtxDevice.RestartUpdate();
        }

        // Refresh the GUI if the change between standard and filler have been made: 
        private void button10_Click_1(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // For the application mode(standard or filler) and the printing on the GUI the WTX registers are read out first.      
            this.set_GUI_rows();            
        }

        // Toolstrip Click Event for Digital IO : Input & Output
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            _wtxDevice.StopUpdate();

            if (_wtxDevice.Connection.ConnType == ConnectionType.Modbus)
            {
                _wtxDevice.Disconnect();

                JetBusConnection _connection = new JetBusConnection(_ipAddress);
                _wtxDevice = new WTXJet(_connection, update);

                _wtxDevice.Connect(5000);

                _functionIOForm = new WTXModbus.FunctionIO();

                _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                DialogResult res = _functionIOForm.ShowDialog();
            }
            else
                if (_wtxDevice.Connection.ConnType == ConnectionType.Jetbus)
                {
                    _functionIOForm = new WTXModbus.FunctionIO();

                    _functionIOForm.ReadButtonClicked_IOFunctions += ReadDigitalIOFunctions;
                    _functionIOForm.WriteButtonClicked_IOFunctions += WriteDigitalIOFunctions;

                    DialogResult res = _functionIOForm.ShowDialog();
                }
            _wtxDevice.RestartUpdate();
        }

        public void ReadDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            int out1 = _wtxDevice.DataStandard.Output1;
            int out2 = _wtxDevice.DataStandard.Output2;
            int out3 = _wtxDevice.DataStandard.Output3;
            int out4 = _wtxDevice.DataStandard.Output4;

            int in1 = _wtxDevice.DataStandard.Input1;
            int in2 = _wtxDevice.DataStandard.Input2;

            _wtxDevice.Disconnect();

            ModbusTcpConnection _connection = new ModbusTcpConnection(_ipAddress);
            _wtxDevice = new WtxModbus(_connection, this._timerInterval, update);
        }
        public void WriteDigitalIOFunctions(object sender, IOFunctionEventArgs e)
        {
            if((int)e.FunctionOutputIO1!=(-1))
                _wtxDevice.DataStandard.Output1 = (int)e.FunctionOutputIO1;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output2 = (int)e.FunctionOutputIO2;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output3 = (int)e.FunctionOutputIO3;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Output4 = (int)e.FunctionOutputIO4;

            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Input1 = (int)e.FunctionInputIO1;
            if ((int)e.FunctionOutputIO1 != (-1))
                _wtxDevice.DataStandard.Input2 = (int)e.FunctionInputIO2;

            _wtxDevice.Disconnect();

            ModbusTcpConnection _connection = new ModbusTcpConnection(_ipAddress);
            _wtxDevice = new WtxModbus(_connection, this._timerInterval, update);
        }

        private void jetbusToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void ModbusTCP_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }
    }
}