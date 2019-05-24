// <copyright file="Program.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Example application for Hbm.Weighing.API, a library to communicate with HBM weighing technology devices  
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
using System.Threading;
using System.Globalization;

using Hbm.Weighing.API;
using Hbm.Weighing.API.WTX;
using Hbm.Weighing.API.WTX.Jet;
using Hbm.Weighing.API.WTX.Modbus;

namespace WTXModbus
{
    /// <summary>
    /// This class implements a console application. An Object of the class 'ModbusTcpConnection' or 'JetBusConnection' and 'BaseWTDevice'('WTXJet' or WTXModbus') 
    /// are initialized as a publisher and subscriber. Afterwards a connection to the device is established and the timer/sending interval is set. 
    /// A timer with for example 500ms is created. After 500ms an event is triggered, which executes the method "Update" reading the register of the device
    /// by an asynchronous call in the method 'e.ProcessData.Async_Call'. As soon as the reading is finisihed, the callback method "Read_DataReceived" takes over the
    /// new data , which have already been interpreted in class 'WTX120_Modbus', so the data is given as a string array. 
    /// The data is also printed on the console in the callback method "Read_DataReceived". 
    /// Being in the while-loop it is possible to select commands to the device. For example taring, change from gross to net value, stop dosing, zeroing and so on. 
    /// 
    /// This is overall just a simple application as an example. A significantly broad presentation is given by the windows form, but for starters it is recommended. 
    /// </summary>

    static class Program
    {

        #region Locales
        private const string DEFAULT_IP_ADDRESS = "192.168.100.88";

        private static string mode = "";
        private const string MESSAGE_CONNECTION_FAILED = "Connection failed!";
        private const string MESSAGE_CONNECTING = "Connecting...";

        private const int WAIT_DISCONNECT = 2000; 
        
        private static BaseWTDevice _wtxDevice;
        
        private static string _ipAddress = DEFAULT_IP_ADDRESS ;     // IP-adress, set as the first argument in the VS project properties menu as an argument or in the console application as an input(which is commented out in the following) 
        private static int _timerInterval = 200;                    // timer interval, set as the second argument in the VS project properties menu as an argument or in the console application as an input(which is commented out in the following) 
        private static ushort _inputMode;                           // inputMode (number of input bytes), set as the third argument in the VS project properties menu as an argument or in the console application as an input(which is commented out in the following) 

        private static ConsoleKeyInfo _valueOutputwords;
        private static ConsoleKeyInfo _valueExitapplication;

        private static string _calibrationWeight = "0";
        
        private static double _preload, _capacity;
        private static IFormatProvider _provider;

        private const double MULTIPLIER_MV2_D = 500000;      //   2 / 1000000; // 2mV/V correspond 1 million digits (d)
        private static string _strCommaDot ="";

        //private static bool _isCalibrating = false;        // For checking if the WTX120_Modbus device is calculating at a moment after the command has been send. If 'isCalibrating' is true, the values are not printed on the console. 
        private static bool _showAllInputWords  = false;
        private static bool _showAllOutputWords = false;

        #endregion

        static void Main(string[] args)
        {
            // Input for the ip adress, the timer interval and the input mode: 

            _inputMode = 6;
            _timerInterval = 200;

            DefineInputs(args);

            // Initialize:

            _provider = CultureInfo.InvariantCulture;

            Console.WriteLine("\nTCPModbus Interface for weighting terminals of HBM\nEnter e to exit the application");

            do // do-while loop for the connection establishment. If the connection is established successfully, the do-while loop is left/exit. 
            {
                InitializeConnection();

            } while (_wtxDevice.Connection.IsConnected==false);


            MenuCases();

            //Get a single value from the WTX device : 
            double x = _wtxDevice.ProcessData.Weight.Net; 

        } // end main  



        private static void DefineInputs(string[] args)
        {
            if (args.Length > 0)
                mode = args[0];
            else
                mode = "Jetbus";

            if (args.Length > 1)
                _ipAddress = args[1];
            else
            {
                Console.WriteLine(" WTX - For connection establishment : No IP adress is given. \n please enter the IP adress to the WTX device, format : aaa.bbb.ccc.dd");
                _ipAddress = Console.ReadLine();
            }
            if (args.Length > 2)
                _timerInterval = Convert.ToInt32(args[2]);
            else
                _timerInterval = 200;

        }

        #region Connection
        // This method connects to the given IP address
        private static void InitializeConnection()
        {
            if (mode == "Modbus" || mode == "modbus")    // If 'Modbus/Tcp' is selected: 
            {
                // Creating objects of ModbusTcpConnection and WTXModbus: 
                ModbusTCPConnection _modbusConection = new ModbusTCPConnection(_ipAddress);

                _wtxDevice = new Hbm.Weighing.API.WTX.WTXModbus(_modbusConection, _timerInterval, Update);

                ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).ReadBufferLength = 38;
            }
            else
            {
                if (mode == "Jet" || mode == "jet" || mode == "Jetbus" || mode == "jetbus")  // If 'JetBus' is selected: 
                {
                    // Creating objects of JetBusConnection and WTXJet: 
                    JetBusConnection _jetConnection = new JetBusConnection(_ipAddress, "Administrator", "wtx");

                    _wtxDevice = new WTXJet(_jetConnection,Update);
                }
            }

            // Connection establishment via Modbus or Jetbus :  
            try
            {
                _wtxDevice.Connect(5000);
            }
            catch (Exception)
            {
                Console.WriteLine(MESSAGE_CONNECTION_FAILED);
            }


            if (_wtxDevice.Connection.IsConnected == true)
            {
                Update(null, null);  // call of callback method to print values on console, even though the measured values did not change. 

                Console.WriteLine("\nThe connection has been established successfully.\nThe values of the WTX device are printed on the console ... :");

                CommandLine.Properties.Settings.Default.IPaddress = _ipAddress;
                CommandLine.Properties.Settings.Default.Save();
            }
            else
            {
                Console.WriteLine(MESSAGE_CONNECTION_FAILED);
                Console.WriteLine("\nFailure : The connection has not been established successfully.\nPlease enter a correct IP Adress for the connection establishment...");               
                _ipAddress = Console.ReadLine();
            }

        } // End method Connect() 


        #region menu cases
        private static void MenuCases()
        {
            // This while loop is repeated till the user enters 'e' (=e meaning exit). After the timer interval the register of the device is read out. 
            // In the while-loop the user can select commands, which are send immediately to the device. 
            while (_valueExitapplication.KeyChar != 'e')
            {
                //_isCalibrating = false;
                _valueOutputwords = Console.ReadKey();
                int valueOutput = Convert.ToInt32(_valueOutputwords.KeyChar);

                if (mode == "Modbus" || mode == "modbus")
                {
                    switch (_valueOutputwords.KeyChar)
                    {

                        case '0': _wtxDevice.Tare(); break;                  // Taring 
                        case '1': _wtxDevice.SetGross(); break;                   // Gross/Net
                        case '2': _wtxDevice.Zero(); break;                 // Zeroing
                        case '3': _wtxDevice.AdjustZeroSignal(); break;              // Adjust zero 
                        case '4': _wtxDevice.AdjustNominalSignal(); break;           // Adjust nominal
                        //case '5': _wtxDevice.ActivateData(); break;            // Activate data
                        //case '6': _wtxDevice.ManualTaring(); break;            // Manual taring
                        case '7': _wtxDevice.RecordWeight(); break;            // Record Weight


                        // 'c' for writing on multiple registers, which is necessary for the calibration. 
                        case 'c':       // Calculate Calibration
                            CalculateCalibration();
                            break;
                        case 'w':       // Calculation with weight 
                            CalibrationWithWeight();
                            break;
                        case 'a':  // Show all input words in the filler application. 
                            if (_showAllInputWords == false)
                            {
                                _showAllInputWords = true;
                                //_wtxDevice.Refreshed = true;
                            }
                            else
                                if (_showAllInputWords == true)
                            {
                                _showAllInputWords = false;
                                //_wtxDevice.Refreshed = true;
                            }
                            break;

                        case 'o': // Writing of the output words

                            if (_showAllOutputWords == false)
                            {
                                _showAllOutputWords = true;
                                //_wtxDevice.Refreshed = true;
                            }
                            else
                            if (_showAllOutputWords == true)
                            {
                                _showAllOutputWords = false;
                                //_wtxDevice.Refreshed = true;
                            }

                            break;

                        // Change connection from Modbus to Jetbus: 
                        case 'j':
                            _wtxDevice.ProcessDataReceived -= Update;   // Delete Callback method 'Update' from the Eventhandler 'DataUpdateEvent'.

                            mode = "Jetbus";

                            if (_wtxDevice != null)    // Necessary to check if the object of BaseWtDevice have been created and a connection exists. 
                            {
                                _wtxDevice.Connection.Disconnect();
                                _wtxDevice = null;
                            }

                            Thread.Sleep(WAIT_DISCONNECT);     // Wait for 2 seconds till the disconnection request is finished. 

                            InitializeConnection();
                            _wtxDevice.ProcessDataReceived += Update;   // To get updated values from the WTX, use method Update(..). 
                            break;


                        default: break;

                    }   // end switch-case
                }

                else
                    if(mode=="Jet" || mode=="Jetbus" || mode =="jet" || mode =="jetbus")
                    {
                    switch (_valueOutputwords.KeyChar)
                    {
                        case '0': _wtxDevice.Tare(); break;                  // Taring 
                        case '1': _wtxDevice.SetGross(); break;                   // Gross/Net
                        case '2': _wtxDevice.Zero(); break;                 // Zeroing
                        case '3': _wtxDevice.AdjustZeroSignal(); break;              // Adjust zero 
                        case '4': _wtxDevice.AdjustNominalSignal(); break;           // Adjust nominal
                        //case '5': _wtxDevice.ActivateData(); break;            // Activate data
                        //case '6': _wtxDevice.ManualTaring(); break;            // Manual taring
                        case '7': _wtxDevice.RecordWeight(); break;            // Record Weight

                        // 'c' for writing on multiple registers, which is necessary for the calibration. 
                        case 'c':       // Calculate Calibration
                            CalculateCalibration();
                            break;
                        case 'w':       // Calculation with weight 
                            CalibrationWithWeight();
                            break;
                        // Change connection from Jetbus to Modbus: 
                        case 'j':
                           
                            _wtxDevice.ProcessDataReceived -= Update;   // Delete Callback method 'Update' from the Eventhandler 'DataUpdateEvent'.

                            mode = "Modbus";

                            if (_wtxDevice != null)    // Necessary to check if the object of BaseWtDevice have been created and a connection exists. 
                            {
                                _wtxDevice.Connection.Disconnect();
                                _wtxDevice = null;
                            }

                            Thread.Sleep(WAIT_DISCONNECT);     // Wait for 2 seconds till the disconnection request is finished. 

                            InitializeConnection();
                            _wtxDevice.ProcessDataReceived += Update;   // To get updated values from the WTX, use method Update(..). 
                            break;

                        default: break;

                    }   // end switch-case

                } // end if

                //int valueOutput = Convert.ToInt32(value_outputwords.KeyChar);
                if (valueOutput >= 9)
                {// switch-case for writing the additional output words of the filler application: 
                    //InputWriteOutputWordsFiller((ushort)valueOutput, value);
                }

                _valueExitapplication = Console.ReadKey();
                if (_valueExitapplication.KeyChar == 'e')
                    break;

            }// end while
        } // end method MenuCases() 


        // This method sets the number of read bytes (words) in the register of the device. 
        // You can set '1' for the net value. '2','3' or '4' for the net and gross value. '5' for the net,gross value and the weight status(word[4]) for the bits representing the weight status like weight moving, weight type, scale range and so ... 
        // You can set '6' for reading the previous bytes(gross/net values, weight status) and for enabling to write on the register. With '6', bit "application mode", "decimals", "unit", "handshake" and "status" is read. 
        // Especially the handshake and status bit is used for writing. 
        private static void set_number_inputs()
        {

            Console.WriteLine("Please enter how many words(bytes) you want to read from the register\nof the device. See the following table for choosing:");

            Console.WriteLine("\nEnter '1'     : Enables reading of ... \n\t\t  word[0]- netto value.\n");
            Console.WriteLine("Enter '2','3',4': Enables reading of ... \n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");
            Console.WriteLine("Enter '5'       : Enables reading of ... \n\t\t  word[4]- weight moving,weight type,scale range,..(see manual)\n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");
            Console.WriteLine("Enter '6'       : Enables writing to the register and reading of ... \n\t\t  word[5]- application mode,decimals,unit,handshake,status bit\n\t\t  word[4]- weight moving,weight type,scale range,..(see manual)\n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");

            Console.WriteLine("It is recommended to use at least '6' for writing and reading. \nDefault setting for the full application in filler mode : '38'\nPlease tip the button 'Enter' after you typed in the number '1' or '2' or...'6'");

            _inputMode = (ushort)Convert.ToInt32(Console.ReadLine());

        }
        /*
         * This method calcutes the values for a dead load and a nominal load(span) in a ratio in mV/V and write in into the WTX registers. 
         */
         
        private static void CalculateCalibration()
        {
            //_isCalibrating = true;

            _wtxDevice.Stop();

            zero_load_nominal_load_input();
           
            _wtxDevice.CalculateAdjustment(_preload,_capacity);
            
            //_isCalibrating = false;           
        }

        /*
         * This method does a calibration with an individual weight to the WTX.  
         * First you tip the value for the calibration weight, then you set the value for the dead load (method ‚MeasureZero‘), 
         * finally you set the value for the nominal weight in the WTX (method ‚Calibrate(calibrationValue)‘).
         */
        private static void CalibrationWithWeight()
        {
            _wtxDevice.Stop();

            Console.Clear();
            Console.WriteLine("\nPlease tip the value for the calibration weight and tip enter to confirm : ");
            _calibrationWeight = Console.ReadLine();

            Console.WriteLine("\nTo start : Set zero load and press any key for measuring zero and wait.");
            string another = Console.ReadLine();

            _wtxDevice.AdjustZeroSignal();
            Console.WriteLine("\nZero load measured.Put weight on scale, press any key and wait.");

            string another2 = Console.ReadLine();

            _wtxDevice.AdjustNominalSignalWithCalibrationWeight(CalibrationWeight());

            _wtxDevice.Restart();
        }  

        /*
         * This method potentiate the number of the values decimals and multiply it with the calibration weight(input) to get
         * an integer which is in written into the WTX registers by the method Calibrate(potencyCalibrationWeight()). 
         */
        private static double CalibrationWeight()
        {
            _strCommaDot = _calibrationWeight.Replace(".", ","); // Transformation into a floating-point number.Thereby commas and dots can be used as input for the calibration weight.                         
            return double.Parse(_strCommaDot);                                                                                                                
        }
        #endregion

        #region callback
        // This method prints the values of the device (as a integer and the interpreted string) as well as the description of each bit. 
        private static void Update(object sender, ProcessDataReceivedEventArgs e)
        {
            // The description and the value of the WTX are only printed on the console if the Interface, containing all auto-properties of the values is 
            // not null (respectively empty) and if no calibration is done at that moment.

            if (_wtxDevice != null && e != null)
            {
                Console.Clear();               

                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)  // If the WTX device is in standard application/mode.
                {
                    Console.WriteLine("0-Taring | 1-Gross/net   | 2-Zeroing  | 3- Adjust zero    | 4-Adjust nominal |\n5-Activate Data          | 6-Manual taring                | 7-Weight storage | \nj-Connection to Jetbus   | a-Show all input words 0 to 37 |\no-Show output words 9-44 | b-Bytes read from the register |\nc-Calculate Calibration  | w-Calibration with weight      | e-Exit\n");
                }
                else
                {

                    if(_showAllInputWords==false && mode=="Modbus")
                    Console.WriteLine("\n0-Taring  | 1-Gross/net  | 2-Clear dosing  | 3- Abort dosing | 4-Start dosing| \n5-Zeroing | 6-Adjust zero| 7-Adjust nominal| 8-Activate data | 9-Weight storage|m-Manual redosing | j-Connection to Jetbus | a-Show all input words 0 to 37 |\no-Show output words 9-44 | b-Bytes read from the register |\nc-Calculate Calibration | w-Calibration with weight       | e-Exit\n");
                    else
                    if (_showAllInputWords == true && mode == "Modbus")
                        Console.WriteLine("\n0-Taring  | 1-Gross/net  | 2-Clear dosing  | 3- Abort dosing | 4-Start dosing| \n5-Zeroing | 6-Adjust zero| 7-Adjust nominal| 8-Activate data | 9-Weight storage|m-Manual redosing | j-Connection to Jetbus | a-Show only input word 0 to 5 |\nb-Bytes read from the register |\nc-Calculate Calibration | w-Calibration with weight       | e-Exit\n");

                    if (mode == "Jet" || mode == "Jetbus" || mode == "jet" || mode == "jetbus")
                        Console.WriteLine("\n0-Taring  | 1-Gross/net  | 2-Clear dosing  | 3- Abort dosing | 4-Start dosing| \n5-Zeroing | 6-Adjust zero| 7-Adjust nominal| 8-Activate data | 9-Weight storage|m-Manual redosing | j-Connection to Modbus | \nc-Calculate Calibration | w-Calibration with weight  | e-Exit the application\n");

                }

                if (_wtxDevice.ApplicationMode == ApplicationMode.Standard)   // If the device is in the standard mode (standard=0 or standard=1; filler=1 or filler=2) 
                {

                    // The values are printed on the console according to the input - "numInputs": 

                    if (_inputMode == 1)
                    {
                        Console.WriteLine("Net value:                     " + _wtxDevice.Weight.Net + "\t  As an int/bool:  " + e.ProcessData.Weight.Net);
                    }
                    else
                        if (_inputMode == 2 || _inputMode == 3 || _inputMode == 4)
                    {
                        Console.WriteLine("Net value:                     " + _wtxDevice.Weight.Net +   "\t  As an int/bool:  " + e.ProcessData.Weight.Net);
                        Console.WriteLine("Gross value:                   " + _wtxDevice.Weight.Gross + "\t  As an int/bool:  " + e.ProcessData.Weight.Gross);
                    }
                    else
                       if (_inputMode == 5)
                    {
                        Console.WriteLine("Net value:                     " + _wtxDevice.Weight.Net +   "\t  As an int/bool:  " + e.ProcessData.Weight.Net);
                        Console.WriteLine("Gross value:                   " + _wtxDevice.Weight.Gross + "\t  As an int/bool:  " + e.ProcessData.Weight.Gross);
                        Console.WriteLine("General weight error:          " + e.ProcessData.GeneralWeightError.ToString() + "\t  As an int/bool:  " + e.ProcessData.GeneralWeightError);
                        Console.WriteLine("Scale alarm triggered:         " + e.ProcessData.ScaleAlarm.ToString() +        "\t  As an int/bool:  " + e.ProcessData.ScaleAlarm);
                        Console.WriteLine("Scale seal is open:            " + e.ProcessData.LegalForTrade.ToString() +    "\t  As an int/bool:  " + e.ProcessData.LegalForTrade);
                        Console.WriteLine("Weight type:                   " + e.ProcessData.TareMode + "\t  As an int/bool:  " + e.ProcessData.TareMode);
                        Console.WriteLine("Scale range:                   " + e.ProcessData.ScaleRange + "\t  As an int/bool:  " + e.ProcessData.ScaleRange);
                        Console.WriteLine("Zero required/True zero:       " + e.ProcessData.ZeroRequired.ToString() +       "\t  As an int/bool:  " + e.ProcessData.ZeroRequired);
                        Console.WriteLine("Weight within center of zero:  " + e.ProcessData.CenterOfZero.ToString() + "\t  As an int/bool:  " + e.ProcessData.CenterOfZero);
                        Console.WriteLine("Weight in zero range:          " + e.ProcessData.InsideZero.ToString() +  "\t  As an int/bool:  " + e.ProcessData.InsideZero);
                        Console.WriteLine("Overload:                      " + e.ProcessData.Overload.ToString() + "        As an int/bool:  " + e.ProcessData.Overload);
                        Console.WriteLine("Underload:                      " + e.ProcessData.Underload.ToString() + "        As an int/bool:  " + e.ProcessData.Underload);
                        Console.WriteLine("Weight stable:                 " + e.ProcessData.WeightStable.ToString() + "          As an int/bool: " + e.ProcessData.WeightStable);
                    }
                    else
                    if (_inputMode == 6 || _inputMode == 38)
                    { 
                        Console.WriteLine("Net value:                     " + _wtxDevice.Weight.Net +  "\t  As an int/bool:  " + e.ProcessData.Weight.Net);
                        Console.WriteLine("Gross value:                   " + _wtxDevice.Weight.Gross + "\t  As an int/bool:  " + e.ProcessData.Weight.Gross);
                        Console.WriteLine("General weight error:          " + e.ProcessData.GeneralWeightError.ToString() +                  "\t  As an int/bool:  " + e.ProcessData.GeneralWeightError);
                        Console.WriteLine("Scale alarm triggered:         " + e.ProcessData.ScaleAlarm.ToString() +                         "\t  As an int/bool:  " + e.ProcessData.ScaleAlarm);
                        Console.WriteLine("Scale seal is open:            " + e.ProcessData.LegalForTrade.ToString() +                     "\t  As an int/bool:  " + e.ProcessData.LegalForTrade);
                        Console.WriteLine("Weight type:                   " + e.ProcessData.TareMode + "\t  As an int/bool:  " + e.ProcessData.TareMode);
                        Console.WriteLine("Scale range:                   " + e.ProcessData.ScaleRange + "\t  As an int/bool:  " + e.ProcessData.ScaleRange);
                        Console.WriteLine("Zero required/True zero:       " + e.ProcessData.ZeroRequired.ToString() +      "\t  As an int/bool:  " + e.ProcessData.ZeroRequired);
                        Console.WriteLine("Weight within center of zero:  " + e.ProcessData.CenterOfZero.ToString() + "\t  As an int/bool:  " + e.ProcessData.CenterOfZero);
                        Console.WriteLine("Weight in zero range:          " + e.ProcessData.InsideZero.ToString() + "\t  As an int/bool:  " + e.ProcessData.InsideZero);
                        Console.WriteLine("Application mode:              " + _wtxDevice.ApplicationMode.ToString() + "\t  As an int/bool:  " + _wtxDevice.ApplicationMode.ToString());
                        Console.WriteLine("Decimal places:                " + e.ProcessData.Decimals.ToString() +          "\t  As an int/bool:  " + e.ProcessData.Decimals);
                        Console.WriteLine("Unit:                          " + _wtxDevice.Unit +                       "\t  As an int/bool:  " + e.ProcessData.Unit);
                        
                        Console.WriteLine("Limit status:                  " + limitCommentMethod() + "       As an int/bool:  " + e.ProcessData.Overload);
                        Console.WriteLine("Weight moving:                 " + e.ProcessData.WeightStable.ToString() + "         As an int/bool: " + e.ProcessData.WeightStable);

                    }
                    else
                        Console.WriteLine("\nWrong input for the number of bytes, which should be read from the register!\nPlease enter 'b' to choose again.");
                }
                else
                {
                    Console.WriteLine("Net value:                     " + _wtxDevice.PrintableWeight.Net +   "\t  As an int/bool:  " + e.ProcessData.Weight.Net);
                    Console.WriteLine("Gross value:                   " + _wtxDevice.PrintableWeight.Gross + "\t  As an int/bool:  " + e.ProcessData.Weight.Gross);
                    Console.WriteLine("General weight error:          " + e.ProcessData.GeneralWeightError.ToString() +                   "\t  As an int/bool:  " + e.ProcessData.GeneralWeightError);
                    Console.WriteLine("Scale alarm triggered:         " + e.ProcessData.ScaleAlarm.ToString() +     "\t  As an int/bool:  " + e.ProcessData.ScaleAlarm);
                    Console.WriteLine("Scale seal is open:            " + e.ProcessData.LegalForTrade.ToString() + "\t  As an int/bool:  " + e.ProcessData.LegalForTrade);
                    Console.WriteLine("Weight type:                   " + e.ProcessData.TareMode +               "\t  As an int/bool:  " + e.ProcessData.TareMode);
                    Console.WriteLine("Scale range:                   " + e.ProcessData.ScaleRange +               "\t  As an int/bool:  " + e.ProcessData.ScaleRange);
                    Console.WriteLine("Zero required/True zero:       " + e.ProcessData.ZeroRequired.ToString() +    "\t  As an int/bool:  " + e.ProcessData.ZeroRequired);
                    Console.WriteLine("Weight within center of zero:  " + e.ProcessData.CenterOfZero.ToString() + "\t  As an int/bool:  " + e.ProcessData.CenterOfZero);
                    Console.WriteLine("Weight in zero range:          " + e.ProcessData.InsideZero.ToString() +           "\t  As an int/bool:  " + e.ProcessData.InsideZero);
                    Console.WriteLine("Application mode:              " + _wtxDevice.ApplicationMode.ToString() +           "\t  As an int/bool:  " + _wtxDevice.ApplicationMode);
                    Console.WriteLine("Decimal places:                " + e.ProcessData.Decimals.ToString() +         "\t  As an int/bool:  " + e.ProcessData.Decimals);
                    Console.WriteLine("Unit:                          " + _wtxDevice.Unit +                      "\t  As an int/bool:  " + e.ProcessData.Unit);
                    //Console.WriteLine("Handshake:                     " + e.ProcessData.Handshake.ToString() +        "\t  As an int/bool:  " + e.ProcessData.Handshake);
                    //Console.WriteLine("Status:                        " + statusCommentMethod() +                    "\t  As an int/bool:  " + e.ProcessData.Status);

                    Console.WriteLine("Limit status:                  " + limitCommentMethod() +  "     As an int/bool:  " + e.ProcessData.Overload);
                    Console.WriteLine("Weight moving:                 " + e.ProcessData.WeightStable.ToString() + "          As an int/bool:  " + e.ProcessData.WeightStable);

                    if (_showAllInputWords == true)
                    {

                        Console.WriteLine("Digital input  1:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input1.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input1);
                        Console.WriteLine("Digital input  2:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input2.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input2);
                        Console.WriteLine("Digital input  3:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input3.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input3);
                        Console.WriteLine("Digital input  4:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input4.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Input4);

                        Console.WriteLine("Digital output 1:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output1.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output1);
                        Console.WriteLine("Digital output 2:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output2.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output2);
                        Console.WriteLine("Digital output 3:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output3.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output3);
                        Console.WriteLine("Digital output 4:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output4.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.Output4);

                        Console.WriteLine("Coarse flow:                   " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlow.ToString() +  "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlow);
                        Console.WriteLine("Fine flow:                     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlow.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlow);
                        Console.WriteLine("Ready:                         " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Ready.ToString() +       "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Ready);
                        Console.WriteLine("Re-dosing:                     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ReDosing.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ReDosing);

                        Console.WriteLine("Emptying:                      " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Emptying.ToString() +          "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Emptying);
                        Console.WriteLine("Flow error:                    " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FlowError.ToString() +         "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FlowError);
                        Console.WriteLine("Alarm:                         " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Alarm.ToString() +             "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.Alarm);
                        Console.WriteLine("ADC Overload/Unterload:        " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.AdcOverUnderload.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.AdcOverUnderload);

                        Console.WriteLine("Max.Dosing time:               " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MaxDosingTime.ToString() +          "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MaxDosingTime);
                        Console.WriteLine("Tolerance error+:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ToleranceErrorPlus.ToString() +     "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ToleranceErrorPlus);
                        Console.WriteLine("Tolerance error-:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ToleranceErrorMinus.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ToleranceErrorMinus);

                        Console.WriteLine("Status digital input 1:        " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.StatusInput1.ToString() +           "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.StatusInput1);
                        Console.WriteLine("General scale error:           " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.GeneralScaleError.ToString() +      "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.GeneralScaleError);
                        Console.WriteLine("Filling process status:        " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FillingProcessStatus.ToString() +   "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FillingProcessStatus);
                        Console.WriteLine("Number of dosing results:      " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.NumberDosingResults.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.NumberDosingResults);

                        Console.WriteLine("Dosing result:                 " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.DosingResult.ToString() +           "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.DosingResult);
                        Console.WriteLine("Mean value of dosing results:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MeanValueDosingResults.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MeanValueDosingResults);
                        Console.WriteLine("Standard deviation:            " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.StandardDeviation.ToString() +      "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.StandardDeviation);
                        Console.WriteLine("Total weight:                  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.TotalWeight.ToString() +            "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.TotalWeight);

                        Console.WriteLine("Fine flow cut-off point:       " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlowCutOffPoint.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlowCutOffPoint);
                        Console.WriteLine("Coarse flow cut-off point:     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlowCutOffPoint.ToString() +  "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlowCutOffPoint);
                        Console.WriteLine("Current dosing time:           " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentDosingTime.ToString() +      "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentDosingTime);
                        Console.WriteLine("Current coarse flow time:      " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentCoarseFlowTime.ToString() +  "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentCoarseFlowTime);
                        Console.WriteLine("Current fine flow time:        " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentFineFlowTime.ToString() +    "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CurrentFineFlowTime);

                        Console.WriteLine("Parameter set (product):       " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ParameterSetProduct.ToString() + "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ParameterSetProduct);
                        Console.WriteLine("Weight memory, Day:            " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemDay.ToString() +        "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemDay);
                        Console.WriteLine("Weight memory, Month:          " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemMonth.ToString() +      "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemMonth);
                        Console.WriteLine("Weight memory, Year:           " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemYear.ToString() +       "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemYear);
                        Console.WriteLine("Weight memory, Seq.Number:     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemSeqNumber.ToString() +  "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemSeqNumber);
                        Console.WriteLine("Weight memory, gross:          " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemGross.ToString() +      "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemGross);
                        Console.WriteLine("Weight memory, net:            " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemNet.ToString() +        "\t  As an int/bool:  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataStandard.WeightMemNet);

                        Console.WriteLine("\nPress 'a' again to hide the input words.");
                    }
                    
                    if(_showAllOutputWords==true)
                    {
                        Console.WriteLine("\nOutput words:\n");

                        Console.WriteLine(" 9) Residual flow time:            " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ResidualFlowTime      + " Press '9' and a value to write");
                        Console.WriteLine("10) Target filling weight:         " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.TargetFillingWeight   + " Press '10' and a value to write");
                        Console.WriteLine("12) Coarse flow cut-off point:     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlowCutOffPoint + " Press '12' and a value to write");
                        Console.WriteLine("14) Fine flow cut-off point:       " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlowCutOffPoint   + " Press '14' and a value to write");

                        Console.WriteLine("16) Minimum fine flow:             " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MinimumFineFlow   + " Press '16' and a value to write");
                        Console.WriteLine("18) Optimization of cut-off points:" + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.OptimizationOfCutOffPoints + " Press '18' and a value to write");
                        Console.WriteLine("19) Maximum dosing time:           " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MaxDosingTime     + " Press '19' and a value to write");
                        Console.WriteLine("20) Start with fine flow:          " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.StartWithFineFlow + " Press '20' and a value to write");

                        Console.WriteLine("21) Coarse lockout time:           " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseLockoutTime + " Press '21' and a value to write");
                        Console.WriteLine("22) Fine lockout time:             " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineLockoutTime   + " Press '22' and a value to write");
                        Console.WriteLine("23) Tare mode:                     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.TareMode + " Press '23' and a value to write");
                        Console.WriteLine("24) Upper tolerance limit + :      " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.UpperToleranceLimit + " Press '24' and a value to write");

                        Console.WriteLine("26) Lower tolerance limit -:       " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.LowerToleranceLimit + " Press '26' and a value to write");
                        Console.WriteLine("28) Minimum start weight:          " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.MinimumStartWeight  + " Press '28' and a value to write");
                        Console.WriteLine("30) Empty weight:                  " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.EmptyWeight + " Press '30' and a value to write");
                        Console.WriteLine("32) Tare delay:                    " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.TareDelay   + " Press '32' and a value to write");

                        Console.WriteLine("33) Coarse flow monitoring time:   " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlowMonitoringTime + " Press '33' and a value to write");
                        Console.WriteLine("34) Coarse flow monitoring:        " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.CoarseFlowMonitoring   + " Press '34' and a value to write");
                        Console.WriteLine("36) Fine flow monitoring:          " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlowMonitoring     + " Press '36' and a value to write");
                        Console.WriteLine("38) Fine flow monitoring time:     " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.FineFlowMonitoringTime + " Press '38' and a value to write");

                        Console.WriteLine("40) Delay time after fine flow:    " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.DelayTimeAfterFineFlow + " Press '40' and a value to write");
                        Console.WriteLine("41) Systematic difference:         " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.SystematicDifference + " Press '41' and a value to write");
                        Console.WriteLine("42) Downwards dosing:              " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.DownwardsDosing + " Press '42' and a value to write");
                        Console.WriteLine("43) Valve control:                 " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.ValveControl   + " Press '43' and a value to write");
                        Console.WriteLine("44) Emptying mode:                 " + ((Hbm.Weighing.API.WTX.WTXModbus)_wtxDevice).DataFiller.EmptyingMode   + " Press '44' and a value to write");

                        Console.WriteLine("\nPress 'o' again to hide the output words.");

                    }
                    
                }
            }
        }
        #endregion

        #region comment methods
        private static string statusCommentMethod()
        {
            /*
            if (mode == "Jetbus" || mode == "Jet" || mode == "jet" || mode == "jetbus")
            {
                if (_wtxDevice.ProcessData.Status == 0) // 1634168417 = false
                    return "Command on go";

                if (_wtxDevice.ProcessData.Status == 1) // 1801543519 = true
                    return "Command ok";
            }
            else
                if (mode == "Modbus" || mode == "modbus")
                {
                    if (_wtxDevice.ProcessData.Status == 0)
                        return "Command on go";

                    if (_wtxDevice.ProcessData.Status == 1)
                        return "Command ok";
                 }
            */
            return "Command on go";
        }

        private static string limitCommentMethod()
        {
            if (_wtxDevice.ProcessData.Overload)
                return "Higher than maximum capacity";
            else if (_wtxDevice.ProcessData.Underload)
                return "Lower than minimum";
            else if (_wtxDevice.ProcessData.HigherSafeLoadLimit)
                return "Higher than safe load limit";
            else
                return "Weight within limits";
        }


        private static void zero_load_nominal_load_input()
        {

            Console.Clear();

            Console.WriteLine("\nPlease tip the value for the zero load/dead load and tip enter to confirm : ");

            string _preloadStr = Console.ReadLine();
            _strCommaDot = _preloadStr.Replace(".", ",");           // For converting into a floating-point number.
            _preload = double.Parse(_strCommaDot);                  // By using the method 'double.Parse(..)' you can use dots and commas.


            Console.WriteLine("\nPlease tip the value for the span/nominal load and tip enter to confirm : ");

            string _capacityStr = Console.ReadLine();
            _strCommaDot = _capacityStr.Replace(".", ",");           // For converting into a floating-point number.
            _capacity = double.Parse(_strCommaDot);                  // By using the method 'double.Parse(..)' you can use dots and commas.

        }
        #endregion

        #region print method
        // This method prints the table to choose how many byte of the register are read. 
        // The parameter "beginning" stands for the moment of the program, in which is this program is called Either on the beginning(=true) or during the execution while the timer is running (=false).
        private static void print_table_for_register_words(bool beginning)
        {
            Console.Clear();

            Console.WriteLine("TCPModbus Interface for weighting terminals of HBM\n");

            if (beginning == true)
                Console.WriteLine("Timer/Sending interval received.\nPlease enter how many words(bytes) you want to read from the register\nof the device. See the following table for choosing:");

            if (beginning == false)
                Console.WriteLine("Executing was interrupted to choose another number of bytes, which will be read from the register.\nPlease enter how many words(bytes) you want to read from the register\nof the device. See the following table for choosing:");

            Console.WriteLine("\nEnter '1'     : Enables reading of ... \n\t\t  word[0]- netto value.\n");
            Console.WriteLine("Enter '2','3',4': Enables reading of ... \n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");
            Console.WriteLine("Enter '5'       : Enables reading of ... \n\t\t  word[4]- weight moving,weight type,scale range,..(see manual)\n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");
            Console.WriteLine("Enter '6'       : Enables writing to the register and reading of ... \n\t\t  word[5]- application mode,decimals,unit,handshake,status bit\n\t\t  word[4]- weight moving,weight type,scale range,..(see manual)\n\t\t  word[2]- gross value. \n\t\t  Word[0]- netto value.\n");

            Console.WriteLine("It is recommended to use at least '6' for writing and reading. \nDefault setting for the full application in filler mode : '38'\nPlease tip the button 'Enter' after you typed in the number '1' or '2' or...'6'");
        }
        #endregion
    }
}


#endregion