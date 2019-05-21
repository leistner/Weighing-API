// <copyright file="JetBusConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

using Hbm.Devices.Jet;
using HBM.Weighing.API.WTX.Modbus;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Jet
{
    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishs the communication to your WTX device, starts/ends the connection,
    /// read and write the register and shows the status of the connection and closes the connection to the device.
    /// 
    /// It works by subscribing a path for the data of the WTX device. By referencing the path/index in the method Read(index)
    /// it returns a JToken object containing all information about the index.
    /// Once a value changes an event is triggered and the data is send via WtxJet to the data classes(ProcessData or 
    /// DataStandard/DataFiller/DataFillerExtended), thus it can be called by the application. 
    /// </summary>
    public class JetBusConnection : INetConnection, IDisposable
    {
        #region member
        protected JetPeer _peer;

        private Dictionary<string, JToken> _dataJTokenBuffer = new Dictionary<string, JToken>();
        Dictionary<string, int> _dataIntegerBuffer = new Dictionary<string, int>();

        Dictionary<JetBusCommand, int> _dataCommandsBuffer = new Dictionary<JetBusCommand, int>();

        private AutoResetEvent _mSuccessEvent = new AutoResetEvent(false);
        private Exception _mException = null;

        #region Events
        public event EventHandler CommunicationLog;
        public event EventHandler<DataEventArgs> IncomingDataReceived;
        public event EventHandler<EventArgs> UpdateDataClasses;
        #endregion

        private bool _connected;

        private string _ipaddress;
        private int interval;

        private JToken[] JTokenArray;
        private ushort[] DataUshortArray;
        private string[] DataStrArray;

        private string _password;
        private string _user;
        private int _timeoutMs;

        #endregion

        #region constructors

        // Constructor without ssh certification. 
        public JetBusConnection(string IPAddress, string User, string Password, int TimeoutMs = 20000)
        {
            string _uri = "wss://" + IPAddress + ":443/jet/canopen";

            IJetConnection jetConnection = new WebSocketJetConnection(_uri, RemoteCertificationCheck);
            _peer = new JetPeer(jetConnection);
            
            this._user = User;
            this._password = Password;
            this._timeoutMs = TimeoutMs;
            this._ipaddress = IPAddress;
        }

        // Constructor with ssh certification
        public JetBusConnection(string IPAddress, int TimeoutMs = 20000) : this(IPAddress, "Administrator", "wtx", TimeoutMs)
        {
        }
        #endregion

        #region support functions

        public void Connect()
        {
            ConnectPeer(this._user, this._password, this._timeoutMs);
            FetchAll();          
        }
        public void DisconnectDevice()
        {
            _peer.Disconnect();
            this._connected = false;
            this.IncomingDataReceived = null;
        }
        private void ConnectPeer(string User, string Password, int TimeoutMs)
        {
            this._user = User;
            this._password = Password;

            _peer.Connect(OnConnectAuthenticate, TimeoutMs);
            WaitOne(2);
        }

        public ConnectionType ConnType
        {
            get { return ConnectionType.Jetbus; }
        }

        public bool IsConnected
        {
            get
            {
                return _connected;
            } 
        }
                     
        private void OnConnectAuthenticate(bool connected)
        {
            if (connected)
            {
                this._connected = true;

                _peer.Authenticate(this._user, this._password, OnAuthenticate, this._timeoutMs);
            }
            else
            {
                this._connected = false;
                _mException = new Exception("Connection failed");
                _mSuccessEvent.Set();
            }
        }


        private void OnAuthenticate(bool success, JToken token)
        {           
            if (!success)
            {

                this._connected = false;
                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.Message);
            }
            _mSuccessEvent.Set();
        }


        private void OnConnect(bool connected)
        {
            if (!connected)
            {
                _mException = new Exception("Connection failed.");
            }

            this._connected = true;
            _mSuccessEvent.Set();
        }


        private void OnFetch(bool success, JToken token)
        {
            if (!success)
            {

                this._connected = false;

                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.ErrorCode.ToString());
            }
            //
            // Wake up the waiting thread where call the construktor to connect the session
            //
            dataArrived = true;
            this._connected = true;
            _mSuccessEvent.Set();
            
            CommunicationLog?.Invoke(this, new LogEvent("Fetch-All success: " + success + " - buffersize is " + _dataJTokenBuffer.Count));
        }


        private bool dataArrived;

        public virtual void FetchAll()
        {
            dataArrived = false;

            Matcher matcher = new Matcher();
            FetchId id;

            _peer.Fetch(out id, matcher, OnFetchData, OnFetch, this._timeoutMs);
            WaitOne(3);

            dataArrived = true;

            IncomingDataReceived?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));  // For getting data already in the FetchAll() 

            // Update data in data classes : 
            this.UpdateDataClasses?.Invoke(this, new EventArgs());

            CommunicationLog?.Invoke(this, new LogEvent("Fetch-All success: " + dataArrived + " - buffersize is " + _dataJTokenBuffer.Count));
        }

        protected virtual void WaitOne(int timeoutMultiplier = 1)
        {
            if (!_mSuccessEvent.WaitOne(_timeoutMs * timeoutMultiplier))
            {
                this._connected = false;

                // Timeout-Exception
                throw new Exception("Jet interface Timeout");
            }

            this._connected = true; 
                     
            if (_mException != null)
            {
                Exception exception = _mException;
                _mException = null;
                throw exception;
            }                        
        }

        public int GetDataFromDictionary(object command)
        {
            ushort _bitMask = 0;
            ushort _mask = 0;
            int _value = 0;

            try
            {
                JetBusCommand jetcommand = (JetBusCommand)command;

                Console.WriteLine(jetcommand.PathIndex); //DDD

                switch (jetcommand.DataType)
                {
                    case DataType.BIT:
                        {
                            switch (jetcommand.BitLength)
                            {
                                case 0: _bitMask = 0xFFFF; break;
                                case 1: _bitMask = 1; break; // = 001
                                case 2: _bitMask = 3; break; // = 011
                                case 3: _bitMask = 7; break; // = 111

                                default: _bitMask = 1; break;
                            }
                            _mask = (ushort)(_bitMask << jetcommand.BitIndex);

                            _value = (_dataIntegerBuffer[jetcommand.PathIndex] & _mask) >> jetcommand.BitIndex;
                            break;
                        }

                    default:
                        {
                            _value = _dataIntegerBuffer[jetcommand.PathIndex];
                            break;
                        }
                }
            }
            catch
            {
                _value = 0;
            }

            return _value;
        }


        #endregion

        #region read-functions

        /// <summary>
        /// Event with callend when raced a Fetch-Event by a other Peer.
        /// </summary>
        /// <param name="data"></param>
        protected virtual void OnFetchData(JToken data)
        {
            string path = data["path"].ToString();
            int i = 0;

            lock (_dataJTokenBuffer)
            {

                switch (data["event"].ToString())
                {
                    case "add":

                        _dataJTokenBuffer.Add(path, data["value"]);

                        if (int.TryParse(data["value"].ToString(), out i))      // checks if the data is a number, which can be converted to an integer at the path. 
                        {
                            _dataIntegerBuffer.Add(path, Convert.ToInt32(data["value"].ToString()));
                        }
                        break;

                    case "fetch":

                        _dataJTokenBuffer[path] = data["value"];

                        if (int.TryParse(data["value"].ToString(), out i))      // checks if the data is a number, which can be converted to an integer at the path. 
                        {
                            _dataIntegerBuffer[path] = Convert.ToInt32(data["value"].ToString());
                        }
                        break;

                    case "change":

                        _dataJTokenBuffer[path] = data["value"];

                        if (int.TryParse(data["value"].ToString(), out i))      // checks if the data is a number, which can be converted to an integer at the path. 
                        {
                            _dataIntegerBuffer[path] = Convert.ToInt32(data["value"].ToString());
                        }
                        
                        break;
                }

                if (dataArrived == true)
                {
                    IncomingDataReceived?.Invoke(this, new DataEventArgs(this._dataIntegerBuffer));
                    // Update data in data classes : 
                    this.UpdateDataClasses?.Invoke(this, new EventArgs());
                }
                CommunicationLog?.Invoke(this, new LogEvent(data.ToString()));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        protected virtual JToken ReadObj(object index)
        {
            lock (_dataJTokenBuffer)
                if (_dataJTokenBuffer.ContainsKey(index.ToString()))
                {
                    return _dataJTokenBuffer[index.ToString()];
                }
                else
                {
                    throw new Exception("Object does not exist in the object dictionary");
                }
        }
      
        public int ReadSingle(object command)
        {
            JetBusCommand _command = (JetBusCommand)command;

            try
            {
                return Convert.ToInt32(ReadObj(_command.PathIndex));
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid data format");
            }
        }

        public void Write(object command, int value)
        {
            JValue jValue = new JValue(value);

            JetBusCommand _command = (JetBusCommand)command;

            SetData(_command.PathIndex, jValue);
        }

        public int ReadInt(object index) {
            try {
                return Convert.ToInt32(ReadObj(index));
            }
            catch (FormatException) {
                throw new FormatException("Invalid data format");
            }
        }

        public long ReadDint(object index) {
            try {
                return Convert.ToInt64(ReadObj(index));
            }
            catch (FormatException) {
                throw new FormatException("Invalid data format");
            }
        }

        public string ReadAsc(object index) {
            return ReadObj(index).ToString();
        }

        #endregion

        #region Write functions
        private void OnSet(bool success, JToken token)
        {
           if (!success)
           {
                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.ErrorCode.ToString());
           }
            
           _mSuccessEvent.Set();
            
           CommunicationLog?.Invoke(this, new LogEvent("Set data" + success ));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        protected virtual void SetData(string path, JValue value)
        {
            try
            {
                JObject request = _peer.Set(path.ToString(), value, OnSet, this._timeoutMs);
            }
            catch (Exception e)
            {
                throw new Exception(e.ToString());
            }
        }


        public void WriteInt(string index, int data)
        {
            JValue value = new JValue(data);
            SetData(index, value);
        }


        public void WriteDint(string index, long data)
        {
            JValue value = new JValue(data);
            SetData(index, value);
        }


        public void WriteAsc(string index, string data)
        {
            JValue value = new JValue(data);
            SetData(index, value);
        }
        #endregion

        
        private string BufferToString()
        {
            StringBuilder sb = new StringBuilder();
            lock (_dataJTokenBuffer) {
                int i = 0;
                foreach (var item in _dataJTokenBuffer) {
                    sb.Append(i.ToString("D3")).Append(" # ").Append(item).Append("\r\n");
                    i++;
                }
            }
            return sb.ToString();
        }
        private void ConvertJTokenToStringArray()
        {
            JTokenArray = _dataJTokenBuffer.Values.ToArray();
            DataUshortArray = new ushort[JTokenArray.Length];
            DataStrArray = new string[JTokenArray.Length];

            for (int i = 0; i < JTokenArray.Length; i++)
            {
                JToken JTokenElement = JTokenArray[i];

                DataStrArray[i] = JTokenElement.ToString();
            }
        }

        public Task<int> WriteAsync(object command, int commandParam)
        {
            throw new NotImplementedException();
        }

        public Task<ushort[]> ReadAsync()
        {
            throw new NotImplementedException();
        }



        public void Disconnect()
        {
            _peer.Disconnect();
            this._connected = false;
            this.IncomingDataReceived = null;
        }


        /// <summary>
        /// RemoteCertificationCheck:
        /// Callback-Method wich is called from SslStream. Is a customized implementation of a certification-check.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        public bool RemoteCertificationCheck(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            
            try
            {
                X509Certificate2 clientCertificate = new X509Certificate2(CertificateToByteArray(),"");
                
                SslStream sslStream = (sender as SslStream);

                if (sslPolicyErrors == SslPolicyErrors.None || sslPolicyErrors == SslPolicyErrors.RemoteCertificateChainErrors)
                {
                    foreach (X509ChainElement item in chain.ChainElements)
                    {
                        item.Certificate.Export(X509ContentType.Cert);
                        //
                        // If one of the included status-flags is not posiv then the cerficate-check
                        // failed. Except the "untrusted root" because it is a self-signed certificate
                        //
                        foreach (X509ChainStatus status in item.ChainElementStatus)
                        {
                            if (status.Status != X509ChainStatusFlags.NoError
                                && status.Status != X509ChainStatusFlags.UntrustedRoot
                                 && status.Status != X509ChainStatusFlags.NotTimeValid)
                            {

                                return false;
                            }
                        }
                        //
                        // compare the certificate in the chain-collection. If on of the certificate at
                        // the path to root equal, are the check ist positive
                        //
                        if (clientCertificate.Equals(item.Certificate))
                        {
                            return true;
                        }
                    }
                }
                // TODO: to reactivate the hostename-check returning false.
                return true;
            }
            catch (Exception)
            {
                // If thrown any exception then is the certification-check failed
                return false;
            }
        }

        private byte[] CertificateToByteArray()
        {
            const string input = 
                
                "MIIECzCCAvOgAwIBAgIJAPTJN5RGpzbRMA0GCSqGSIb3DQEBBQUAMIGbMQswCQYDVQQGEwJERTELMAkGA1UECAw" +
                "CSEUxEjAQBgNVBAcMCURhcm1zdGFkdDErMCkGA1UECgwiSG90dGluZ2VyIEJhbGR3aW4gTWVzc3RlY2huaWsgR21iSDELMAkGA1UECwwCV" +
                "1QxFDASBgNVBAMMC3d3dy5oYm0uY29tMRswGQYJKoZIhvcNAQkBFgxpbmZvQGhibS5jb20wHhcNMTcwNDA2MTU1NzQzWhcNMjcwNDA0MTU1N" +
                "zQzWjCBmzELMAkGA1UEBhMCREUxCzAJBgNVBAgMAkhFMRIwEAYDVQQHDAlEYXJtc3RhZHQxKzApBgNVBAoMIkhvdHRpbmdlciBCYWxkd2luIE1" +
                "lc3N0ZWNobmlrIEdtYkgxCzAJBgNVBAsMAldUMRQwEgYDVQQDDAt3d3cuaGJtLmNvbTEbMBkGCSqGSIb3DQEJARYMaW5mb0BoYm0uY29tMIIBIjA" +
                "NBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAr + 51SnSoQX1M3aOUwaD8dcFIoac9peaiRsIOUqwJGn58g + n53aevw54sFyvffcJnzZVAFEP" +
                "Ech1oQCNowsNDnNr4UL / NaO4C4GsJX5bmdia6nGj7IWLMeQzs + 0gfqPWmO / OsJVnwrp9h6 / SxuIz5n04l7ESupSBnXfifb9RGA0encHt" +
                "ZK0LxHRD9sxyhNYKDKW76hgDK / EZ5HU2YjKS0y58 + PU15AV + vQ5srJFMC + KNHveWF4xgj528r3C25FWpVtW5Dqd937OrSAS5truGxPBz" +
                "enWx3PHw6zRPvBbOApTNWLfbcp90mF8 / 9wFi94PG + EokaYSoF0xyT2G6fAVs3qQIDAQABo1AwTjAdBgNVHQ4EFgQUZC39SAkffhRee1x / 7" +
                "TbnrQJQ / jMwHwYDVR0jBBgwFoAUZC39SAkffhRee1x / 7TbnrQJQ / jMwDAYDVR0TBAUwAwEB / zANBgkqhkiG9w0BAQUFAAOCAQEACRI28" +
                "UaB6KNtDVee + waz + SfNd3tm / RspwRarJBlf9dcbpcxolvp9UxCoWkyf9hkmJPEyJzJltuan08DkqmschD36saOJcVRh6BfLNBD8DkFTavP" +
                "0Q2BKb8FvJpdacNTRK542sJHSk5gr6imwnD5EAj + OT24UpH5rwq5Esu5TYFLdSuYfRXw6puTION / fqqTKVK9Au0TdFPgZ4Fppym4fInQ0jJQ" +
                "hcGSWMs3yomPqftwitRwv5 / p8hLtf3yNIkk9OnBwPpT7QxXxw4Zs0Jvl / VBFuNwbeD12ur3RKbMyCn9W0RjaMrYpKnAjik3IlSqDYZ0XDMwZ" +
                "0oQiOFy / a6bR4Vw =="     
                ;

            // Invoke GetBytes method.
            byte[] _byteArray = Encoding.ASCII.GetBytes(input);

            return _byteArray;
        }


        #region IDisposable Support
        private bool _disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing) {
            if (!_disposedValue) { 
                if (disposing) {
                    // dispose managed state (managed objects).
                    _mSuccessEvent.Close();
                    _mSuccessEvent.Dispose();
                }
                _disposedValue = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.

            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        public void WriteSync(ushort wordNumber, ushort commandParam)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, JToken> getDataBuffer
        {
            get
            {
                return _dataJTokenBuffer;
            }
        }

        public string IpAddress
        {
            get
            {
                return this._ipaddress;
            }
            set
            {
                this._ipaddress = value;
            }
        }
        public Dictionary<string, int> AllData
        {
            get
            {
                return _dataIntegerBuffer;
            }
        }
        public Dictionary<ModbusCommand, int> ModbusData
        {
            get
            {
                return new Dictionary<ModbusCommand, int>();
            }
        }

        #endregion
    }

}
