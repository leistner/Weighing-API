// <copyright file="ModbusTCPConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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
using NModbus.Device;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using NModbus;
using HBM.Weighing.API.WTX.Jet;

namespace HBM.Weighing.API.WTX.Modbus
{
    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishs the communication to your WTX device, starts/ends the connection,
    /// read and write the register and shows the status of the connection and closes the connection to the device.
    /// 
    /// It works by reading registers via Modbus to get the data of the WTX device. By referencing the index in the method Read(index)
    /// it returns a ushort array containing all information about the index.
    /// Once the read method is called, the data is read from the WTX device, put into registers and loaded into a Dictionary containing
    /// pairs of values and keys. The values are shifted and masked. The keys are the indexes(data word number) given by ModbusCommands.
    /// </summary>
    public class ModbusTcpConnection : INetConnection
    {
        #region consts

        const int MODBUS_TCP_DEFAULT_PORT = 502;
        const int WTX_DEFAULT_START_ADDRESS = 0;
        const int WTX_DEFAULT_DATAWORD_COUNT = 38;

        #endregion

        #region privates

        private IModbusMaster _master;
        private TcpClient _client;

        private bool _connected;
        private string ipAddress;
        private ushort _numOfPoints;
        private int _port;
        private ushort _startAdress;

        private ushort[] _data;
        private ushort[] _dataToWrite;

        private Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();
        private Dictionary<ModbusCommand, int> _dataCommandsBuffer = new Dictionary<ModbusCommand, int>();

        private int _dataCommand;

        private ModbusCommands _commands;

        #endregion

        #region Events
        public event EventHandler BusActivityDetection;
        public event EventHandler<DataEventArgs> UpdateDataClasses;
        public event EventHandler<DataEventArgs> IncomingDataReceived;
        #endregion

        #region Constructor

        public ModbusTcpConnection(string IpAddress)
        {
            _connected = false;
            _port = MODBUS_TCP_DEFAULT_PORT;
            ipAddress = IpAddress; //IP-address to establish a successful connection to the device

            _commands = new ModbusCommands();

            this.CreateDictionary();

            _dataToWrite = new ushort[2] { 0, 0 };

            _numOfPoints = WTX_DEFAULT_DATAWORD_COUNT;
            _startAdress = WTX_DEFAULT_START_ADDRESS;
        }

        #endregion

        #region Get/Set-Properties

        // Getter/Setter for the IP_Adress, StartAdress, NumofPoints, Sending_interval, Port, Is_connected()
        public string IpAddress
        {
            get { return ipAddress; }
            set { ipAddress = value; }
        }

        public ushort StartAdress
        {
            get { return _startAdress; }
            set { _startAdress = value; }
        }

        public ushort NumOfPoints
        {
            get { return _numOfPoints; }
            set { _numOfPoints = value; }
        }

        public int Port
        {
            get { return _port; }
            set { _port = value; }
        }

        public bool IsConnected
        {
            get
            {
                return this._connected;
            }
        }
        public int getCommand
        {
            get { return this._dataCommand; }
        }

        public int NumofPoints
        {
            get
            {
                return this._numOfPoints;
            }
            set
            {
                this._numOfPoints = (ushort)value;
            }
        }
        #endregion

        #region Connect/Disconnect methods

        // This method establishs a connection to the device. Therefore an IP address and the port number
        // for the TcpClient is needed. The client itself is used for the implementation of the ModbusIpMaster. 
        public void Connect()
        {
            try
            {
                _client = new TcpClient(ipAddress, _port);

                var factory = new ModbusFactory();
                _master = factory.CreateMaster(_client);

                _connected = true;

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has been established successfully"));
            }
            catch (Exception)
            {
                _connected = false; // If the connection establishment has not been successful - connected=false. 

                BusActivityDetection?.Invoke(this, new LogEvent("Connection has NOT been established successfully"));
            }
        }
        public ConnectionType ConnType
        {
            get { return ConnectionType.Modbus; }
        }
        // This method closes the connection to the device.
        public void Disconnect()
        {
            _client.Close();
            _connected = false;
            IncomingDataReceived = null;
        }
        #endregion

        #region Read methods

        /// <summary>
        /// This method is called from the device class "WTX120" to read the register of the device. 
        /// </summary>
        /// <param name="index"></param>
        /// <returns>dataword of the wtx device</returns>
        public int Read(object index)
        {
            try
            {
                _data = _master.ReadHoldingRegisters(0, _startAdress, _numOfPoints);

                BusActivityDetection?.Invoke(this, new LogEvent("Read successful: Registers have been read"));

                this.UpdateDictionary();
                // Updata data in data classes : 
                this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

                return _data[Convert.ToInt16(index)];
            }
            catch (ArgumentException)
            {
                Console.WriteLine("\nNumber of points has to be between 1 and 125.\n");
            }

            return _data[Convert.ToInt16(index)];
        }

        public async Task<ushort[]> ReadAsync()
        {
            _data = new ushort[100];

            _data = await _master.ReadHoldingRegistersAsync(0, _startAdress, _numOfPoints);

            this.UpdateDictionary();

            // Update data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));

            return _data;
        }

        #endregion

        #region Write methods


        public void Write(ModbusCommand ModbusFrame, int value)
        {
            switch(ModbusFrame.DataType)
            {

                case DataType.U08:
                    _master.WriteSingleRegister(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), (ushort)value);
                    break;

                case DataType.Int16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), _dataToWrite);
                    break;
                case DataType.U16:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), _dataToWrite);
                    break;

                case DataType.U32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), _dataToWrite);
                    break;
                case DataType.Int32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), _dataToWrite);
                    break;
                case DataType.S32:
                    _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
                    _dataToWrite[1] = (ushort)(value & 0x0000ffff);

                    _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(ModbusFrame.PathIndex), _dataToWrite);
                    break;
            }
        }



        public void Write(string index, int data)
        {
            this._dataCommand = data;

            _master.WriteSingleRegister(0, (ushort)Convert.ToInt32(index), (ushort)data);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));
        }

        public async Task<int> WriteAsync(ushort index, ushort commandParam)
        {
            this._dataCommand = commandParam;

            await _master.WriteSingleRegisterAsync(0, index, (ushort)_dataCommand);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort) have been written successfully to the register"));

            return this._dataCommand;
        }

        public void WriteArray(string index, int value)
        {
            _dataToWrite[0] = (ushort)((value & 0xffff0000) >> 16);
            _dataToWrite[1] = (ushort)(value & 0x0000ffff);

            _master.WriteMultipleRegisters(0, (ushort)Convert.ToInt32(index), _dataToWrite);

            BusActivityDetection?.Invoke(this, new LogEvent("Data(ushort array) have been written successfully to multiple registers"));
        }

        #endregion

        #region Update dictionary methods, properties

        private void CreateDictionary()
        {
            _dataIntegerBuffer.Add(_commands.Net_value.PathIndex+ Convert.ToInt32(_commands.Net_value.IO).ToString() + _commands.Net_value.BitLength.ToString()+_commands.Net_value.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Gross_value.PathIndex + Convert.ToInt32(_commands.Gross_value.IO).ToString() + _commands.Gross_value.BitLength.ToString() + _commands.Gross_value.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Weighing_device_1_weight_status.PathIndex + Convert.ToInt32(_commands.Weighing_device_1_weight_status.IO).ToString() + _commands.Weighing_device_1_weight_status.BitLength.ToString() + _commands.Weighing_device_1_weight_status.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Unit_prefix_fixed_parameter.PathIndex + Convert.ToInt32(_commands.Unit_prefix_fixed_parameter.IO).ToString() + _commands.Unit_prefix_fixed_parameter.BitLength.ToString() + _commands.Unit_prefix_fixed_parameter.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Fine_flow_cut_off_point.PathIndex + Convert.ToInt32(_commands.Fine_flow_cut_off_point.IO).ToString() + _commands.Fine_flow_cut_off_point.BitLength.ToString() + _commands.Fine_flow_cut_off_point.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_cut_off_point.PathIndex + Convert.ToInt32(_commands.Coarse_flow_cut_off_point.IO).ToString() + _commands.Coarse_flow_cut_off_point.BitLength.ToString() + _commands.Coarse_flow_cut_off_point.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Decimals.PathIndex + Convert.ToInt32(_commands.Decimals.IO).ToString() + _commands.Decimals.BitLength.ToString() + _commands.Decimals.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Application_mode.PathIndex + Convert.ToInt32(_commands.Application_mode.IO).ToString() + _commands.Application_mode.BitLength.ToString() + _commands.Application_mode.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Scale_command_status.PathIndex + Convert.ToInt32(_commands.Scale_command_status.IO).ToString() + _commands.Scale_command_status.BitLength.ToString() + _commands.Scale_command_status.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Status_digital_input_1.PathIndex + Convert.ToInt32(_commands.Status_digital_input_1.IO).ToString() + _commands.Status_digital_input_1.BitLength.ToString() + _commands.Status_digital_input_1.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_input_2.PathIndex + Convert.ToInt32(_commands.Status_digital_input_2.IO).ToString() + _commands.Status_digital_input_2.BitLength.ToString() + _commands.Status_digital_input_2.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_input_3.PathIndex + Convert.ToInt32(_commands.Status_digital_input_3.IO).ToString() + _commands.Status_digital_input_3.BitLength.ToString() + _commands.Status_digital_input_3.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_input_4.PathIndex + Convert.ToInt32(_commands.Status_digital_input_4.IO).ToString() + _commands.Status_digital_input_4.BitLength.ToString() + _commands.Status_digital_input_4.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Status_digital_output_1.PathIndex + Convert.ToInt32(_commands.Status_digital_output_1.IO).ToString() + _commands.Status_digital_output_1.BitLength.ToString() + _commands.Status_digital_output_1.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_output_2.PathIndex + Convert.ToInt32(_commands.Status_digital_output_2.IO).ToString() + _commands.Status_digital_output_2.BitLength.ToString() + _commands.Status_digital_output_2.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_output_3.PathIndex + Convert.ToInt32(_commands.Status_digital_output_3.IO).ToString() + _commands.Status_digital_output_3.BitLength.ToString() + _commands.Status_digital_output_3.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Status_digital_output_4.PathIndex + Convert.ToInt32(_commands.Status_digital_output_4.IO).ToString() + _commands.Status_digital_output_4.BitLength.ToString() + _commands.Status_digital_output_4.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Limit_value.PathIndex + Convert.ToInt32(_commands.Limit_value.IO).ToString() + _commands.Limit_value.BitLength.ToString() + _commands.Limit_value.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv11.PathIndex + Convert.ToInt32(_commands.Limit_value_monitoring_liv11.IO).ToString() + _commands.Limit_value_monitoring_liv11.BitLength.ToString() + _commands.Limit_value_monitoring_liv11.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Signal_source_liv12.PathIndex + Convert.ToInt32(_commands.Signal_source_liv12.IO).ToString() + _commands.Signal_source_liv12.BitLength.ToString() + _commands.Signal_source_liv12.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv13.PathIndex + Convert.ToInt32(_commands.Switch_on_level_liv13.IO).ToString() + _commands.Switch_on_level_liv13.BitLength.ToString() + _commands.Switch_on_level_liv13.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv14.PathIndex + Convert.ToInt32(_commands.Switch_off_level_liv14.IO).ToString() + _commands.Switch_off_level_liv14.BitLength.ToString() + _commands.Switch_off_level_liv14.BitIndex.ToString(), 0);
            
            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv21.PathIndex + Convert.ToInt32(_commands.Limit_value_monitoring_liv21.IO).ToString() + _commands.Limit_value_monitoring_liv21.BitLength.ToString() + _commands.Limit_value_monitoring_liv21.BitIndex.ToString(), 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv22.PathIndex + Convert.ToInt32(_commands.Signal_source_liv22.IO).ToString() + _commands.Signal_source_liv22.BitLength.ToString() + _commands.Signal_source_liv22.BitIndex.ToString(), 0);
            /*
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv23.PathIndex + Convert.ToInt32(_commands.Switch_on_level_liv23.IO).ToString() + _commands.Switch_on_level_liv23.BitLength.ToString() + _commands.Switch_on_level_liv23.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv24.PathIndex + Convert.ToInt32(_commands.Switch_off_level_liv24.IO).ToString() + _commands.Switch_off_level_liv24.BitLength.ToString() + _commands.Switch_off_level_liv24.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv31.PathIndex + Convert.ToInt32(_commands.Limit_value_monitoring_liv31.IO).ToString() + _commands.Limit_value_monitoring_liv31.BitLength.ToString() + _commands.Limit_value_monitoring_liv31.BitIndex.ToString(), 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv32.PathIndex + Convert.ToInt32(_commands.Signal_source_liv32.IO).ToString() + _commands.Signal_source_liv32.BitLength.ToString() + _commands.Signal_source_liv32.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv33.PathIndex + Convert.ToInt32(_commands.Switch_on_level_liv33.IO).ToString() + _commands.Switch_on_level_liv33.BitLength.ToString() + _commands.Switch_on_level_liv33.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv34.PathIndex + Convert.ToInt32(_commands.Switch_off_level_liv34.IO).ToString() + _commands.Switch_off_level_liv34.BitLength.ToString() + _commands.Switch_off_level_liv34.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv41.PathIndex + Convert.ToInt32(_commands.Limit_value_monitoring_liv41.IO).ToString() + _commands.Limit_value_monitoring_liv41.BitLength.ToString() + _commands.Limit_value_monitoring_liv41.BitIndex.ToString(), 0); ;
            _dataIntegerBuffer.Add(_commands.Signal_source_liv42.PathIndex + Convert.ToInt32(_commands.Signal_source_liv42.IO).ToString() + _commands.Signal_source_liv42.BitLength.ToString() + _commands.Signal_source_liv42.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_on_level_liv43.PathIndex + Convert.ToInt32(_commands.Switch_on_level_liv43.IO).ToString() + _commands.Switch_on_level_liv43.BitLength.ToString() + _commands.Switch_on_level_liv43.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Switch_off_level_liv44.PathIndex + Convert.ToInt32(_commands.Switch_off_level_liv44.IO).ToString() + _commands.Switch_off_level_liv44.BitLength.ToString() + _commands.Switch_off_level_liv44.BitIndex.ToString(), 0);
            */
            _dataIntegerBuffer.Add(_commands.ReadWeightMemDay_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemDay_ID.IO).ToString() + _commands.ReadWeightMemDay_ID.BitLength.ToString() + _commands.ReadWeightMemDay_ID.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReadWeightMemMonth_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemMonth_ID.IO).ToString() + _commands.ReadWeightMemMonth_ID.BitLength.ToString() + _commands.ReadWeightMemMonth_ID.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReadWeightMemYear_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemYear_ID.IO).ToString() + _commands.ReadWeightMemYear_ID.BitLength.ToString() + _commands.ReadWeightMemYear_ID.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReadWeightMemSeqNumber_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemSeqNumber_ID.IO).ToString() + _commands.ReadWeightMemSeqNumber_ID.BitLength.ToString() + _commands.ReadWeightMemSeqNumber_ID.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReadWeightMemGross_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemGross_ID.IO).ToString() + _commands.ReadWeightMemGross_ID.BitLength.ToString() + _commands.ReadWeightMemGross_ID.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReadWeightMemNet_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemNet_ID.IO).ToString() + _commands.ReadWeightMemNet_ID.BitLength.ToString() + _commands.ReadWeightMemNet_ID.BitIndex.ToString(), 0);

            //_dataIntegerBuffer.Add(_commands.Residual_flow_time.PathIndex + Convert.ToInt32(_commands.Residual_flow_time.IO).ToString() + _commands.Residual_flow_time.BitLength.ToString() + _commands.Residual_flow_time.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Minimum_fine_flow.PathIndex + Convert.ToInt32(_commands.Minimum_fine_flow.IO).ToString() + _commands.Minimum_fine_flow.BitLength.ToString() + _commands.Minimum_fine_flow.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Optimization.PathIndex + Convert.ToInt32(_commands.Optimization.IO).ToString() + _commands.Optimization.BitLength.ToString() + _commands.Optimization.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Tare_mode.PathIndex + Convert.ToInt32(_commands.Tare_mode.IO).ToString() + _commands.Tare_mode.BitLength.ToString() + _commands.Tare_mode.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Minimum_start_weight.PathIndex + Convert.ToInt32(_commands.Minimum_start_weight.IO).ToString() + _commands.Minimum_start_weight.BitLength.ToString() + _commands.Minimum_start_weight.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Tare_delay.PathIndex + Convert.ToInt32(_commands.Tare_delay.IO).ToString() + _commands.Tare_delay.BitLength.ToString() + _commands.Tare_delay.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring_time.PathIndex + Convert.ToInt32(_commands.Coarse_flow_monitoring_time.IO).ToString() + _commands.Coarse_flow_monitoring_time.BitLength.ToString() + _commands.Coarse_flow_monitoring_time.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring_time.PathIndex + Convert.ToInt32(_commands.Fine_flow_monitoring_time.IO).ToString() + _commands.Fine_flow_monitoring_time.BitLength.ToString() + _commands.Fine_flow_monitoring_time.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Systematic_difference.PathIndex + Convert.ToInt32(_commands.Systematic_difference.IO).ToString() + _commands.Systematic_difference.BitLength.ToString() + _commands.Systematic_difference.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Valve_control.PathIndex + Convert.ToInt32(_commands.Valve_control.IO).ToString() + _commands.Valve_control.BitLength.ToString() + _commands.Valve_control.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.AdcOverUnderload.PathIndex + Convert.ToInt32(_commands.AdcOverUnderload.IO).ToString() + _commands.AdcOverUnderload.BitLength.ToString() + _commands.AdcOverUnderload.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.LegalForTradeOperation.PathIndex + Convert.ToInt32(_commands.LegalForTradeOperation.IO).ToString() + _commands.LegalForTradeOperation.BitLength.ToString() + _commands.LegalForTradeOperation.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.StatusInput1.PathIndex + Convert.ToInt32(_commands.StatusInput1.IO).ToString() + _commands.StatusInput1.BitLength.ToString() + _commands.StatusInput1.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.GeneralScaleError.PathIndex + Convert.ToInt32(_commands.GeneralScaleError.IO).ToString() + _commands.GeneralScaleError.BitLength.ToString() + _commands.GeneralScaleError.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.CoarseFlow.PathIndex + Convert.ToInt32(_commands.CoarseFlow.IO).ToString() + _commands.CoarseFlow.BitLength.ToString() + _commands.CoarseFlow.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.FineFlow.PathIndex + Convert.ToInt32(_commands.FineFlow.IO).ToString() + _commands.FineFlow.BitLength.ToString() + _commands.FineFlow.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Ready.PathIndex + Convert.ToInt32(_commands.Ready.IO).ToString() + _commands.Ready.BitLength.ToString() + _commands.Ready.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ReDosing.PathIndex + Convert.ToInt32(_commands.ReDosing.IO).ToString() + _commands.ReDosing.BitLength.ToString() + _commands.ReDosing.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Emptying.PathIndex + Convert.ToInt32(_commands.Emptying.IO).ToString() + _commands.Emptying.BitLength.ToString() + _commands.Emptying.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.FlowError.PathIndex + Convert.ToInt32(_commands.FlowError.IO).ToString() + _commands.FlowError.BitLength.ToString() + _commands.FlowError.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Alarm.PathIndex + Convert.ToInt32(_commands.Alarm.IO).ToString() + _commands.Alarm.BitLength.ToString() + _commands.Alarm.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ToleranceErrorPlus.PathIndex + Convert.ToInt32(_commands.ToleranceErrorPlus.IO).ToString() + _commands.ToleranceErrorPlus.BitLength.ToString() + _commands.ToleranceErrorPlus.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ToleranceErrorMinus.PathIndex + Convert.ToInt32(_commands.ToleranceErrorMinus.IO).ToString() + _commands.ToleranceErrorMinus.BitLength.ToString() + _commands.ToleranceErrorMinus.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Dosing_time.PathIndex + Convert.ToInt32(_commands.Dosing_time.IO).ToString() + _commands.Dosing_time.BitLength.ToString() + _commands.Dosing_time.BitIndex.ToString() , 0);
            _dataIntegerBuffer.Add(_commands.Coarse_flow_time.PathIndex + Convert.ToInt32(_commands.Coarse_flow_time.IO).ToString() + _commands.Coarse_flow_time.BitLength.ToString() + _commands.Coarse_flow_time.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.CurrentFineFlowTime.PathIndex + Convert.ToInt32(_commands.CurrentFineFlowTime.IO).ToString() + _commands.CurrentFineFlowTime.BitLength.ToString() + _commands.CurrentFineFlowTime.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.ParameterSetProduct.PathIndex + Convert.ToInt32(_commands.ParameterSetProduct.IO).ToString() + _commands.ParameterSetProduct.BitLength.ToString() + _commands.ParameterSetProduct.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.DownwardsDosing.PathIndex + Convert.ToInt32(_commands.DownwardsDosing.IO).ToString() + _commands.DownwardsDosing.BitLength.ToString() + _commands.DownwardsDosing.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.TotalWeight.PathIndex + Convert.ToInt32(_commands.TotalWeight.IO).ToString() + _commands.TotalWeight.BitLength.ToString() + _commands.TotalWeight.BitIndex.ToString(), 0);
            //_dataIntegerBuffer.Add(_commands.TargetFillingWeight.PathIndex + Convert.ToInt32(_commands.TargetFillingWeight.IO).ToString() + _commands.TargetFillingWeight.BitLength.ToString() + _commands.TargetFillingWeight.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Run_start_dosing.PathIndex + Convert.ToInt32(_commands.Run_start_dosing.IO).ToString() + _commands.Run_start_dosing.BitLength.ToString() + _commands.Run_start_dosing.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring.PathIndex + Convert.ToInt32(_commands.Coarse_flow_monitoring.IO).ToString() + _commands.Coarse_flow_monitoring.BitLength.ToString() + _commands.Coarse_flow_monitoring.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring.PathIndex + Convert.ToInt32(_commands.Fine_flow_monitoring.IO).ToString() + _commands.Fine_flow_monitoring.BitLength.ToString() + _commands.Fine_flow_monitoring.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Emptying_mode.PathIndex + Convert.ToInt32(_commands.Emptying_mode.IO).ToString() + _commands.Emptying_mode.BitLength.ToString() + _commands.Emptying_mode.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Maximal_dosing_time.PathIndex + Convert.ToInt32(_commands.Maximal_dosing_time.IO).ToString() + _commands.Maximal_dosing_time.BitLength.ToString() + _commands.Maximal_dosing_time.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Upper_tolerance_limit.PathIndex + Convert.ToInt32(_commands.Upper_tolerance_limit.IO).ToString() + _commands.Upper_tolerance_limit.BitLength.ToString() + _commands.Upper_tolerance_limit.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Lower_tolerance_limit.PathIndex + Convert.ToInt32(_commands.Lower_tolerance_limit.IO).ToString() + _commands.Lower_tolerance_limit.BitLength.ToString() + _commands.Lower_tolerance_limit.BitIndex.ToString(), 0);

            _dataIntegerBuffer.Add(_commands.Delay_time_after_fine_flow.PathIndex + Convert.ToInt32(_commands.Delay_time_after_fine_flow.IO).ToString() + _commands.Delay_time_after_fine_flow.BitLength.ToString() + _commands.Delay_time_after_fine_flow.BitIndex.ToString(), 0);
            _dataIntegerBuffer.Add(_commands.Activation_time_after_fine_flow.PathIndex + Convert.ToInt32(_commands.Activation_time_after_fine_flow.IO).ToString() + _commands.Activation_time_after_fine_flow.BitLength.ToString() + _commands.Activation_time_after_fine_flow.BitIndex.ToString(), 0);

            // Undefined IDs : 
            /*
            _dataIntegerBuffer.Add(_commands.Range_selection_parameter, 0);
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
            _dataIntegerBuffer[_commands.Net_value.PathIndex + Convert.ToInt32(_commands.Net_value.IO).ToString() + _commands.Net_value.BitLength.ToString() + _commands.Net_value.BitIndex.ToString()] = _data[1] + (_data[0] << 16);
            _dataIntegerBuffer[_commands.Gross_value.PathIndex + Convert.ToInt32(_commands.Gross_value.IO).ToString() + _commands.Gross_value.BitLength.ToString() + _commands.Gross_value.BitIndex.ToString()] =  _data[3] + (_data[2] << 16);
            _dataIntegerBuffer[_commands.Weighing_device_1_weight_status.PathIndex + Convert.ToInt32(_commands.Weighing_device_1_weight_status.IO).ToString() + _commands.Weighing_device_1_weight_status.BitLength.ToString() + _commands.Weighing_device_1_weight_status.BitIndex.ToString()] = _data[4];
            _dataIntegerBuffer[_commands.Scale_command_status.PathIndex + Convert.ToInt32(_commands.Scale_command_status.IO).ToString() + _commands.Scale_command_status.BitLength.ToString() + _commands.Scale_command_status.BitIndex.ToString()] = _data[5];                  // status -> Measured value status
            _dataIntegerBuffer[_commands.Status_digital_input_1.PathIndex + Convert.ToInt32(_commands.Status_digital_input_1.IO).ToString() + _commands.Status_digital_input_1.BitLength.ToString() + _commands.Status_digital_input_1.BitIndex.ToString()] = _data[6];
            _dataIntegerBuffer[_commands.Status_digital_output_1.PathIndex + Convert.ToInt32(_commands.Status_digital_output_1.IO).ToString() + _commands.Status_digital_output_1.BitLength.ToString() + _commands.Status_digital_output_1.BitIndex.ToString()] = _data[7];
            _dataIntegerBuffer[_commands.Limit_value.PathIndex + Convert.ToInt32(_commands.Limit_value.IO).ToString() + _commands.Limit_value.BitLength.ToString() + _commands.Limit_value.BitIndex.ToString()] = _data[8];
            _dataIntegerBuffer[_commands.Fine_flow_cut_off_point.PathIndex + Convert.ToInt32(_commands.Fine_flow_cut_off_point.IO).ToString() + _commands.Fine_flow_cut_off_point.BitLength.ToString() + _commands.Fine_flow_cut_off_point.BitIndex.ToString()] = _data[20];
            _dataIntegerBuffer[_commands.Coarse_flow_cut_off_point.PathIndex + Convert.ToInt32(_commands.Coarse_flow_cut_off_point.IO).ToString() + _commands.Coarse_flow_cut_off_point.BitLength.ToString() + _commands.Coarse_flow_cut_off_point.BitIndex.ToString()] = _data[22];

            _dataIntegerBuffer[_commands.Application_mode.PathIndex + Convert.ToInt32(_commands.Application_mode.IO).ToString() + _commands.Application_mode.BitLength.ToString() + _commands.Application_mode.BitIndex.ToString()] = _data[5] & 0x1;             // application mode 
            _dataIntegerBuffer[_commands.Decimals.PathIndex + Convert.ToInt32(_commands.Decimals.IO).ToString() + _commands.Decimals.BitLength.ToString() + _commands.Decimals.BitIndex.ToString()] = (_data[5] & 0x70) >> 4;             // decimals
            _dataIntegerBuffer[_commands.Unit_prefix_fixed_parameter.PathIndex + Convert.ToInt32(_commands.Unit_prefix_fixed_parameter.IO).ToString() + _commands.Unit_prefix_fixed_parameter.BitLength.ToString() + _commands.Unit_prefix_fixed_parameter.BitIndex.ToString()] = (_data[5] & 0x180) >> 7;    // unit

            
            _dataIntegerBuffer[_commands.Coarse_flow_monitoring.PathIndex + Convert.ToInt32(_commands.Coarse_flow_monitoring.IO).ToString() + _commands.Coarse_flow_monitoring.BitLength.ToString() + _commands.Coarse_flow_monitoring.BitIndex.ToString()] = _data[8] & 0x1;           //_coarseFlow
            _dataIntegerBuffer[_commands.Fine_flow_monitoring.PathIndex + Convert.ToInt32(_commands.Fine_flow_monitoring.IO).ToString() + _commands.Fine_flow_monitoring.BitLength.ToString() + _commands.Fine_flow_monitoring.BitIndex.ToString()] = ((_data[8] & 0x2) >> 1);  // _fineFlow

            _dataIntegerBuffer[_commands.Ready.PathIndex + Convert.ToInt32(_commands.Ready.IO).ToString() + _commands.Ready.BitLength.ToString() + _commands.Ready.BitIndex.ToString()] = ((_data[8] & 0x4) >> 2);
            _dataIntegerBuffer[_commands.ReDosing.PathIndex + Convert.ToInt32(_commands.ReDosing.IO).ToString() + _commands.ReDosing.BitLength.ToString() + _commands.ReDosing.BitIndex.ToString()] = ((_data[8] & 0x8) >> 3);
            _dataIntegerBuffer[_commands.Emptying_mode.PathIndex + Convert.ToInt32(_commands.Emptying_mode.IO).ToString() + _commands.Emptying_mode.BitLength.ToString() + _commands.Emptying_mode.BitIndex.ToString()] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.Maximal_dosing_time.PathIndex + Convert.ToInt32(_commands.Maximal_dosing_time.IO).ToString() + _commands.Maximal_dosing_time.BitLength.ToString() + _commands.Maximal_dosing_time.BitIndex.ToString()] = ((_data[8] & 0x100) >> 8);
            _dataIntegerBuffer[_commands.Upper_tolerance_limit.PathIndex + Convert.ToInt32(_commands.Upper_tolerance_limit.IO).ToString() + _commands.Upper_tolerance_limit.BitLength.ToString() + _commands.Upper_tolerance_limit.BitIndex.ToString()] = ((_data[8] & 0x400) >> 10);
            _dataIntegerBuffer[_commands.Lower_tolerance_limit.PathIndex + Convert.ToInt32(_commands.Lower_tolerance_limit.IO).ToString() + _commands.Lower_tolerance_limit.BitLength.ToString() + _commands.Lower_tolerance_limit.BitIndex.ToString()] = ((_data[8] & 0x800) >> 11);
            _dataIntegerBuffer[_commands.StatusInput1.PathIndex + Convert.ToInt32(_commands.StatusInput1.IO).ToString() + _commands.StatusInput1.BitLength.ToString() + _commands.StatusInput1.BitIndex.ToString()] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.LegalForTradeOperation.PathIndex + Convert.ToInt32(_commands.LegalForTradeOperation.IO).ToString() + _commands.LegalForTradeOperation.BitLength.ToString() + _commands.LegalForTradeOperation.BitIndex.ToString()] = ((_data[8] & 0x200) >> 9);
            
            _dataIntegerBuffer[_commands.ReadWeightMemDay_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemDay_ID.IO).ToString() + _commands.ReadWeightMemDay_ID.BitLength.ToString() + _commands.ReadWeightMemDay_ID.BitIndex.ToString()]         = (_data[9]);
            _dataIntegerBuffer[_commands.ReadWeightMemMonth_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemMonth_ID.IO).ToString() + _commands.ReadWeightMemMonth_ID.BitLength.ToString() + _commands.ReadWeightMemMonth_ID.BitIndex.ToString()] = (_data[10]);
            _dataIntegerBuffer[_commands.ReadWeightMemYear_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemYear_ID.IO).ToString() + _commands.ReadWeightMemYear_ID.BitLength.ToString() + _commands.ReadWeightMemYear_ID.BitIndex.ToString()]     = (_data[11]);
            _dataIntegerBuffer[_commands.ReadWeightMemSeqNumber_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemSeqNumber_ID.IO).ToString() + _commands.ReadWeightMemSeqNumber_ID.BitLength.ToString() + _commands.ReadWeightMemSeqNumber_ID.BitIndex.ToString()] = (_data[12]);
            _dataIntegerBuffer[_commands.ReadWeightMemGross_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemGross_ID.IO).ToString() + _commands.ReadWeightMemGross_ID.BitLength.ToString() + _commands.ReadWeightMemGross_ID.BitIndex.ToString()] = (_data[13]);
            _dataIntegerBuffer[_commands.ReadWeightMemNet_ID.PathIndex + Convert.ToInt32(_commands.ReadWeightMemNet_ID.IO).ToString() + _commands.ReadWeightMemNet_ID.BitLength.ToString() + _commands.ReadWeightMemNet_ID.BitIndex.ToString()]         = (_data[14]);

            _dataIntegerBuffer[_commands.Emptying.PathIndex + Convert.ToInt32(_commands.Emptying.IO).ToString() + _commands.Emptying.BitLength.ToString() + _commands.Emptying.BitIndex.ToString()] = ((_data[8] & 0x10) >> 4);
            _dataIntegerBuffer[_commands.FlowError.PathIndex + Convert.ToInt32(_commands.FlowError.IO).ToString() + _commands.FlowError.BitLength.ToString() + _commands.FlowError.BitIndex.ToString()] = ((_data[8] & 0x20) >> 5);
            _dataIntegerBuffer[_commands.Alarm.PathIndex + Convert.ToInt32(_commands.Alarm.IO).ToString() + _commands.Alarm.BitLength.ToString() + _commands.Alarm.BitIndex.ToString()] = ((_data[8] & 0x40) >> 6);
            _dataIntegerBuffer[_commands.AdcOverUnderload.PathIndex + Convert.ToInt32(_commands.AdcOverUnderload.IO).ToString() + _commands.AdcOverUnderload.BitLength.ToString() + _commands.AdcOverUnderload.BitIndex.ToString()] = ((_data[8] & 0x80) >> 7);

            _dataIntegerBuffer[_commands.StatusInput1.PathIndex + Convert.ToInt32(_commands.StatusInput1.IO).ToString() + _commands.StatusInput1.BitLength.ToString() + _commands.StatusInput1.BitIndex.ToString()] = ((_data[8] & 0x4000) >> 14);
            _dataIntegerBuffer[_commands.GeneralScaleError.PathIndex + Convert.ToInt32(_commands.GeneralScaleError.IO).ToString() + _commands.GeneralScaleError.BitLength.ToString() + _commands.GeneralScaleError.BitIndex.ToString()] = ((_data[8] & 0x8000) >> 15);
            _dataIntegerBuffer[_commands.TotalWeight.PathIndex + Convert.ToInt32(_commands.TotalWeight.IO).ToString() + _commands.TotalWeight.BitLength.ToString() + _commands.TotalWeight.BitIndex.ToString()] = _data[18];
            // Filler data: Missing ID's
            /*
            _dataIntegerBuffer[IDCommands.] = _fillingProcessStatus = _data[9];  // Undefined
            _dataIntegerBuffer[IDCommands.] = _numberDosingResults = _data[11];          
            */

            // Undefined IDs:
            /*
            _dataIntegerBuffer[IDCommands.DOSING_RESULT]      = _data[12];
            _dataIntegerBuffer[IDCommands.MEAN_VALUE_DOSING_RESULTS] = _data[14];
            _dataIntegerBuffer[IDCommands.STANDARD_DEVIATION] = _data[16];
            _dataIntegerBuffer[IDCommands.CURRENT_DOSING_TIME]        = _data[24];    // _currentDosingTime = _data[24];

            _dataIntegerBuffer[IDCommands.CURRENT_COARSE_FLOW_TIME] = _data[25];      // _currentCoarseFlowTime
            _dataIntegerBuffer[IDCommands.CURRENT_FINE_FLOW_TIME]   = _data[26];      // _currentFineFlowTime
            _dataIntegerBuffer[IDCommands.RANGE_SELECTION_PARAMETER] = _data[27];     // _parameterSetProduct
            */
        }

        public ModbusCommands Commands
        {
            get
            {
                return _commands;
            }
        }

        public Dictionary<string, int> AllData
        {
            get
            {
                return _dataIntegerBuffer;
            }
        }

        public Dictionary<JetBusCommand, int> JetBusData
        {
            get
            {
                return new Dictionary<JetBusCommand, int>();
            }
        }

        #endregion

    }
}


//CreateDictionary():
/*

public void CreateDictionary()
{
    _dataIntegerBuffer.Add(_commands.Net_value.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Gross_value.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Weighing_device_1_weight_status.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Unit_prefix_fixed_parameter.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Fine_flow_cut_off_point.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Coarse_flow_cut_off_point.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Decimals.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Application_mode.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Scale_command_status.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Status_digital_input_1.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_input_2.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_input_3.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_input_4.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Status_digital_output_1.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_output_2.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_output_3.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Status_digital_output_4.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Limit_value.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv11.PathIndex, 0); ;
    _dataIntegerBuffer.Add(_commands.Signal_source_liv12.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_on_level_liv13.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_off_level_liv14.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv21.PathIndex, 0); ;
    _dataIntegerBuffer.Add(_commands.Signal_source_liv22.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_on_level_liv23.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_off_level_liv24.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv31.PathIndex, 0); ;
    _dataIntegerBuffer.Add(_commands.Signal_source_liv32.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_on_level_liv33.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_off_level_liv34.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Limit_value_monitoring_liv41.PathIndex, 0); ;
    _dataIntegerBuffer.Add(_commands.Signal_source_liv42.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_on_level_liv43.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Switch_off_level_liv44.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.ReadWeightMemDay_ID.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ReadWeightMemMonth_ID.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ReadWeightMemYear_ID.PathIndex, 0);   
    _dataIntegerBuffer.Add(_commands.ReadWeightMemSeqNumber_ID.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ReadWeightMemGross_ID.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ReadWeightMemNet_ID.PathIndex, 0);

    //_dataIntegerBuffer.Add(_commands.Residual_flow_time.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Minimum_fine_flow.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Optimization.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Tare_mode.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Minimum_start_weight.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Tare_delay.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring_time.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring_time.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Systematic_difference.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Valve_control.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.AdcOverUnderload.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.LegalForTradeOperation.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.StatusInput1.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.GeneralScaleError.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.CoarseFlow.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.FineFlow.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Ready.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ReDosing.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Emptying.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.FlowError.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Alarm.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ToleranceErrorPlus.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ToleranceErrorMinus.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Dosing_time.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Coarse_flow_time.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.CurrentFineFlowTime.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.ParameterSetProduct.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.DownwardsDosing.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.TotalWeight.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.TargetFillingWeight.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Run_start_dosing.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Coarse_flow_monitoring.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Fine_flow_monitoring.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Emptying_mode.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Maximal_dosing_time.PathIndex, 0);

    _dataIntegerBuffer.Add(_commands.Upper_tolerance_limit.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Lower_tolerance_limit.PathIndex, 0);

    //_dataIntegerBuffer.Add(_commands.Range_selection_parameter, 0);

    _dataIntegerBuffer.Add(_commands.Delay_time_after_fine_flow.PathIndex, 0);
    _dataIntegerBuffer.Add(_commands.Activation_time_after_fine_flow.PathIndex, 0);     
} 
*/


/*
 *             _dataIntegerBuffer.Add(_commands.Net_value.PathIndex+ Convert.ToInt32(_commands.Net_value.IO).ToString() + _commands.Net_value.BitLength.ToString()+_commands.Net_value.BitIndex.ToString(), 0);
        _dataCommandsBuffer.Add(_commands.Gross_value, 0);
        _dataCommandsBuffer.Add(_commands.Weighing_device_1_weight_status, 0);
        _dataCommandsBuffer.Add(_commands.Unit_prefix_fixed_parameter, 0);

        _dataCommandsBuffer.Add(_commands.Fine_flow_cut_off_point, 0);
        _dataCommandsBuffer.Add(_commands.Coarse_flow_cut_off_point, 0);
        _dataCommandsBuffer.Add(_commands.Decimals, 0);
        _dataCommandsBuffer.Add(_commands.Application_mode, 0);
        _dataCommandsBuffer.Add(_commands.Scale_command_status, 0);

        _dataCommandsBuffer.Add(_commands.Status_digital_input_1, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_input_2, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_input_3, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_input_4, 0);

        _dataCommandsBuffer.Add(_commands.Status_digital_output_1, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_output_2, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_output_3, 0);
        _dataCommandsBuffer.Add(_commands.Status_digital_output_4, 0);

        _dataCommandsBuffer.Add(_commands.Limit_value, 0);
        _dataCommandsBuffer.Add(_commands.Limit_value_monitoring_liv11, 0); ;
        _dataCommandsBuffer.Add(_commands.Signal_source_liv12, 0);
        _dataCommandsBuffer.Add(_commands.Switch_on_level_liv13, 0);
        _dataCommandsBuffer.Add(_commands.Switch_off_level_liv14, 0);

        _dataCommandsBuffer.Add(_commands.Limit_value_monitoring_liv21, 0); ;
        _dataCommandsBuffer.Add(_commands.Signal_source_liv22, 0);
        _dataCommandsBuffer.Add(_commands.Switch_on_level_liv23, 0);
        _dataCommandsBuffer.Add(_commands.Switch_off_level_liv24, 0);

        _dataCommandsBuffer.Add(_commands.Limit_value_monitoring_liv31, 0); ;
        _dataCommandsBuffer.Add(_commands.Signal_source_liv32, 0);
        _dataCommandsBuffer.Add(_commands.Switch_on_level_liv33, 0);
        _dataCommandsBuffer.Add(_commands.Switch_off_level_liv34, 0);

        _dataCommandsBuffer.Add(_commands.Limit_value_monitoring_liv41, 0); ;
        _dataCommandsBuffer.Add(_commands.Signal_source_liv42, 0);
        _dataCommandsBuffer.Add(_commands.Switch_on_level_liv43, 0);
        _dataCommandsBuffer.Add(_commands.Switch_off_level_liv44, 0);

        _dataCommandsBuffer.Add(_commands.ReadWeightMemDay_ID, 0);
        _dataCommandsBuffer.Add(_commands.ReadWeightMemMonth_ID, 0);
        _dataCommandsBuffer.Add(_commands.ReadWeightMemYear_ID, 0);
        _dataCommandsBuffer.Add(_commands.ReadWeightMemSeqNumber_ID, 0);
        _dataCommandsBuffer.Add(_commands.ReadWeightMemGross_ID, 0);
        _dataCommandsBuffer.Add(_commands.ReadWeightMemNet_ID, 0);

        //_dataCommandsBuffer.Add(_commands.Residual_flow_time, 0);
        _dataCommandsBuffer.Add(_commands.Minimum_fine_flow, 0);
        _dataCommandsBuffer.Add(_commands.Optimization, 0);
        _dataCommandsBuffer.Add(_commands.Tare_mode, 0);
        _dataCommandsBuffer.Add(_commands.Minimum_start_weight, 0);
        _dataCommandsBuffer.Add(_commands.Tare_delay, 0);
        _dataCommandsBuffer.Add(_commands.Coarse_flow_monitoring_time, 0);
        _dataCommandsBuffer.Add(_commands.Fine_flow_monitoring_time, 0);
        _dataCommandsBuffer.Add(_commands.Systematic_difference, 0);
        _dataCommandsBuffer.Add(_commands.Valve_control, 0);
        _dataCommandsBuffer.Add(_commands.AdcOverUnderload, 0);
        _dataCommandsBuffer.Add(_commands.LegalForTradeOperation, 0);

        _dataCommandsBuffer.Add(_commands.StatusInput1, 0);
        _dataCommandsBuffer.Add(_commands.GeneralScaleError, 0);
        _dataCommandsBuffer.Add(_commands.CoarseFlow, 0);
        _dataCommandsBuffer.Add(_commands.FineFlow, 0);
        _dataCommandsBuffer.Add(_commands.Ready, 0);
        _dataCommandsBuffer.Add(_commands.ReDosing, 0);
        _dataCommandsBuffer.Add(_commands.Emptying, 0);
        _dataCommandsBuffer.Add(_commands.FlowError, 0);

        _dataCommandsBuffer.Add(_commands.Alarm, 0);
        _dataCommandsBuffer.Add(_commands.ToleranceErrorPlus, 0);
        _dataCommandsBuffer.Add(_commands.ToleranceErrorMinus, 0);
        _dataCommandsBuffer.Add(_commands.Dosing_time, 0);
        _dataCommandsBuffer.Add(_commands.Coarse_flow_time, 0);
        _dataCommandsBuffer.Add(_commands.CurrentFineFlowTime, 0);
        _dataCommandsBuffer.Add(_commands.ParameterSetProduct, 0);
        _dataCommandsBuffer.Add(_commands.DownwardsDosing, 0);

        _dataCommandsBuffer.Add(_commands.TotalWeight, 0);
        _dataCommandsBuffer.Add(_commands.TargetFillingWeight, 0);
        _dataCommandsBuffer.Add(_commands.Run_start_dosing, 0);

        _dataCommandsBuffer.Add(_commands.Coarse_flow_monitoring, 0);
        _dataCommandsBuffer.Add(_commands.Fine_flow_monitoring, 0);
        _dataCommandsBuffer.Add(_commands.Emptying_mode, 0);
        _dataCommandsBuffer.Add(_commands.Maximal_dosing_time, 0);

        _dataCommandsBuffer.Add(_commands.Upper_tolerance_limit, 0);
        _dataCommandsBuffer.Add(_commands.Lower_tolerance_limit, 0);

        //_dataCommandsBuffer.Add(_commands.Range_selection_parameter, 0);

        _dataCommandsBuffer.Add(_commands.Delay_time_after_fine_flow, 0);
        _dataCommandsBuffer.Add(_commands.Activation_time_after_fine_flow, 0);
        */
        // Undefined IDs : 
        /*
        _dataCommandsBuffer.Add(_commands.DOSING_STATE, 0);
        _dataCommandsBuffer.Add(_commands.DOSING_RESULT, 0);
        _dataCommandsBuffer.Add(IDCommands.DELAY1_DOSING, 0);
        _dataCommandsBuffer.Add(IDCommands.STANDARD_DEVIATION, 0);
        _dataCommandsBuffer.Add(IDCommands.EMPTY_WEIGHT_TOLERANCE, 0);
        _dataCommandsBuffer.Add(IDCommands.MEAN_VALUE_DOSING_RESULTS, 0);
        _dataCommandsBuffer.Add(IDCommands.FINE_FLOW_PHASE_BEFORE_COARSE_FLOW, 0);
        */


   