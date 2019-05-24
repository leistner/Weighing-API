// <copyright file="JetBusConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.API, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.API.WTX.Jet
{
    using System;
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Hbm.Devices.Jet;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishs the communication to your WTX device, starts/ends the connection,
    /// read and write the register and shows the status of the connection and closes the connection to the device.
    /// It works by subscribing a path for the data of the WTX device. By referencing the path/index in the method Read(index)
    /// it returns a JToken object containing all information about the index.
    /// Once a value changes an event is triggered and the data is send via WtxJet to the data classes(ProcessData or 
    /// DataStandard/DataFiller/DataFillerExtended), thus it can be called by the application. 
    /// </summary>
    public class JetBusConnection : INetConnection, IDisposable
    {
        #region ==================== constants & fields ====================
        const string STD_WTX_AUTHENTICATION_USER = "Administrator";
        const string STD_WTX_AUTHENTICATION_PASSWORD = "wtx";
        protected JetPeer _peer;
        private AutoResetEvent _mSuccessEvent = new AutoResetEvent(false);
        private Exception _mException = null;
        private string _password;
        private string _user;
        private int _timeoutMs;
        private int _callbackTokenValue;
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
            byte[] _byteArray = Encoding.ASCII.GetBytes(input);

            return _byteArray;
        }
        private bool _disposedValue = false;
        #endregion

        #region ==================== events & delegates ====================
        public event EventHandler CommunicationLog;
        public event EventHandler<EventArgs> UpdateData;
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="JetBusConnection" /> class.
        /// Use this constructor for individual authentication
        /// </summary>
        /// <param name="iPAddress">IP address of the target device</param>
        /// <param name="user">User for device authentication</param>
        /// <param name="password">User for device authentication</param>
        public JetBusConnection(string iPAddress, string user, string password)
        {
            string _uri = "wss://" + iPAddress + ":443/jet/canopen";

            IJetConnection jetConnection = new WebSocketJetConnection(_uri, RemoteCertificationCheck);
            _peer = new JetPeer(jetConnection);
            
            this._user = user;
            this._password = password;
            this.IpAddress = iPAddress;
            this._callbackTokenValue = 0; 
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JetBusConnection" /> class.
        /// Use this constructor for standard authentication
        /// </summary>
        /// <param name="iPAddress">IP address of the target device</param>
        public JetBusConnection(string IPAddress) : this(IPAddress, STD_WTX_AUTHENTICATION_USER, STD_WTX_AUTHENTICATION_PASSWORD)
        {
        }
        #endregion

        #region ======================== properties ========================
        public ConnectionType ConnectionType => ConnectionType.Jetbus;

        public bool IsConnected { get; private set; }

        public string IpAddress { get; set; }

        public Dictionary<string, string> AllData { get; } = new Dictionary<string, string>();        
        #endregion

        #region ================ public & internal methods =================
        public void Connect(int timeoutMs = 20000)
        {
            IsConnected = false;
            _timeoutMs = timeoutMs;
            ConnectPeer(this._user, this._password, timeoutMs);

            // We wait until all is done:
            // ConnectPeer
            //  OnConnectAuthenticate
            //      OnAuthenticateFetchAll
            //          FetchAll   
            WaitOne(3);

            if (IsConnected)
                this.UpdateData?.Invoke(this, new EventArgs());
        }

        public void Disconnect()
        {
            _peer.Disconnect();
            this.IsConnected = false;
        }
        
        public int Read(object command)
        {
            JetBusCommand _command = (JetBusCommand)command;

            try
            {
                Matcher id = new Matcher();
                id.EqualsTo = _command.PathIndex;
                JObject request = _peer.Get(id, OnGet, this._timeoutMs);
                WaitOne(2);
                return _callbackTokenValue;
            }
            catch (FormatException)
            {
                throw new FormatException("Invalid data format");
            }
        }

        public Task<ushort[]> ReadAsync(object command)
        {
            throw new NotImplementedException();
        }

        public string ReadFromBuffer(object command)
        {
            ushort _bitMask = 0;
            ushort _mask = 0;
            string _value = "";

            try
            {
                JetBusCommand jetcommand = (JetBusCommand)command;
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

                            _value = (((ushort)Convert.ToUInt16(AllData[jetcommand.PathIndex]) & _mask) >> jetcommand.BitIndex).ToString();
                            break;
                        }

                    default:
                        {
                            _value = AllData[jetcommand.PathIndex];
                            break;
                        }
                }
            }
            catch
            {
                _value = "0";
            }
            return _value;
        }

        public void Write(object command, int value)
        {
            JValue jValue = new JValue(value);
            JetBusCommand _command = (JetBusCommand)command;
            SetData(_command.PathIndex, jValue);
        }

        public Task<int> WriteAsync(object command, int commandParam)
        {
            throw new NotImplementedException();
        }

        /// This code added to correctly implement the disposable pattern.
        /// 


            
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region =============== protected & private methods ================
        private void ConnectPeer(string user, string password, int timeoutMs)
        {
            _user = user;
            _password = password;
            _peer.Connect(OnConnectAuthenticate, timeoutMs);
        }

        private void OnConnectAuthenticate(bool connected)
        {
            if (connected)
            {
                _peer.Authenticate(_user, _password, OnAuthenticateFetchAll, _timeoutMs);
            }
            else
            {
                _mException = new Exception("Connection failed");
                _mSuccessEvent.Set();
            }
        }

        private void OnAuthenticateFetchAll(bool authenticationSuccess, JToken token)
        {
            if (authenticationSuccess)
            {
                FetchAll();
            }
            else
            {
                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.Message);
                _mSuccessEvent.Set();
            }
        }

        private void FetchAll()
        {
            Matcher matcher = new Matcher();
            FetchId id;
            _peer.Fetch(out id, matcher, OnFetchData, OnFetch, this._timeoutMs);                                 

            CommunicationLog?.Invoke(this, new LogEvent("Fetch-All success: Buffersize is " + AllData.Count));
        }

        private void OnFetch(bool success, JToken token)
        {
            if (success)
            {
                IsConnected = true;
            }
            else
            {
                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.ErrorCode.ToString());
            }
            _mSuccessEvent.Set();
                        
            CommunicationLog?.Invoke(this, new LogEvent("Fetch-All success: " + success + " - buffersize is " + AllData.Count));
        }
               

        private void WaitOne(int timeoutMultiplier = 1)
        {
            if (!_mSuccessEvent.WaitOne(_timeoutMs * timeoutMultiplier))
            {
                throw new Exception("Jet interface timeout");
            }
                     
            if (_mException != null)
            {
                Exception exception = _mException;
                _mException = null;
                throw exception;
            }                        
        }
        
        /// <summary>
        /// Event with callend when raced a Fetch-Event by a other Peer.
        /// </summary>
        /// <param name="data"></param>
        private void OnFetchData(JToken data)
        {
            string path = data["path"].ToString();
            int i = 0;

            lock (AllData)
            {
                switch (data["event"].ToString())
                {
                    case "add":
                        AllData.Add(path, data["value"].ToString());
                        break;

                    case "fetch":
                        AllData[path] = data["value"].ToString();
                        break;

                    case "change":
                        AllData[path] = data["value"].ToString();
                        break;
                }

                if (IsConnected)
                {
                    this.UpdateData?.Invoke(this, new EventArgs());
                }
                CommunicationLog?.Invoke(this, new LogEvent(data.ToString()));
            }
        }

        private void OnGet(bool success, JToken token)
        {
            if (!success)
            {
                JetBusException exception = new JetBusException(token);
                _mException = new Exception(exception.ErrorCode.ToString());
            }
            else
            {
                this._callbackTokenValue = Convert.ToInt32(token["result"].Last.Value<string>("value"));
                CommunicationLog?.Invoke(this, new LogEvent("Get data" + success + "Value: " + this._callbackTokenValue.ToString()));
            }
        }
               
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        private void SetData(string path, JValue value)
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
        /// RemoteCertificationCheck:
        /// Callback-Method wich is called from SslStream. Is a customized implementation of a certification-check.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="certificate"></param>
        /// <param name="chain"></param>
        /// <param name="sslPolicyErrors"></param>
        /// <returns></returns>
        private bool RemoteCertificationCheck(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
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
                        // If one of the included status-flags is not posiv then the cerficate-check
                        // failed. Except the "untrusted root" because it is a self-signed certificate
                        foreach (X509ChainStatus status in item.ChainElementStatus)
                        {
                            if (status.Status != X509ChainStatusFlags.NoError
                                && status.Status != X509ChainStatusFlags.UntrustedRoot
                                 && status.Status != X509ChainStatusFlags.NotTimeValid)
                            {
                                return false;
                            }
                        }
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
         
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            { 
                if (disposing)
                {
                    _mSuccessEvent.Close();
                    _mSuccessEvent.Dispose();
                }
                _disposedValue = true;
            }
        }
        #endregion
    }

}
