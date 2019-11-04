// <copyright file="JetBusConnection.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// Hbm.Weighing.Api, a library to communicate with HBM weighing technology devices  
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

namespace Hbm.Weighing.Api.DSE.Jet
{
    using System;
    using System.Collections.Generic;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Hbm.Devices.Jet;
    using Hbm.Weighing.Api.WTX.Jet;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Use this class to handle a connection via Ethernet.
    /// This class establishes the communication to your WTX device, starts/ends the connection,
    /// receives and stores new data or writes new data.
    /// </summary>
    public class DSEJetConnection : INetConnection, IDisposable
    {
        #region ==================== constants & fields ====================
        private JetPeer _peer;
        private AutoResetEvent _successEvent = new AutoResetEvent(false);
        private Exception _localException = null;
        private string _password;
        private string _user;
        private int _timeoutMs;
        private bool _disposedValue = false;
        #endregion

        #region ==================== events & delegates ====================
        public event EventHandler<LogEventArgs> CommunicationLog;

        public event EventHandler<EventArgs> UpdateData;
        #endregion

        #region =============== constructors & destructors =================
        /// <summary>
        /// Initializes a new instance of the <see cref="JetBusConnection" /> class.
        /// Use this constructor for individual authentication
        /// </summary>
        /// <param name="ipAddress">IP address of the target device</param>
        /// <param name="user">User for device authentication</param>
        /// <param name="password">Password for device authentication</param>
        public DSEJetConnection(string ipAddress)
        {
            string _uri;
            IJetConnection jetConnection;

           _uri = "ws://" + ipAddress + ":80/jet/canopen";
           jetConnection = new WebSocketJetConnection(_uri);
            
            _peer = new JetPeer(jetConnection);
            
            this.IpAddress = ipAddress;
        }
        #endregion

        #region ======================== properties ========================
        /// <inheritdoc />
        public ConnectionType ConnectionType => ConnectionType.DSEJet;

        /// <inheritdoc />
        public bool IsConnected { get; private set; }
        
        /// <inheritdoc />
        public string IpAddress { get; set; }

        /// <inheritdoc />
        public Dictionary<string, string> AllData { get; } = new Dictionary<string, string>();
        #endregion

        #region ================ public & internal methods =================
        /// <inheritdoc />
        public void Connect(int timeoutMs = 20000)
        {
            IsConnected = false;
            _timeoutMs = timeoutMs;
            ConnectPeer(timeoutMs); 
            WaitOne(3);
        }

        /// <inheritdoc />
        public void Disconnect()
        {
            if (this.IsConnected)
            {
                _peer.Disconnect();
                this.IsConnected = false;
            }
        }

        /// <inheritdoc />
        public string Read(object command)
        {
            return ReadFromBuffer(command);
        }

        /// <inheritdoc />
        public Task<string> ReadAsync(object command)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public string ReadFromBuffer(object command)
        {
            string result = "0";
            JetBusCommand jetcommand = (JetBusCommand)command;
            if (AllData.ContainsKey(jetcommand.Path))
                result = jetcommand.ToString(AllData[jetcommand.Path]);
            return result;
        }

        /// <inheritdoc />
        public int ReadIntegerFromBuffer(object command)
        {
            JetBusCommand jetcommand = (JetBusCommand)command;
            return jetcommand.ToSValue(AllData[jetcommand.Path]);
        }

        /// <inheritdoc />
        public bool WriteInteger(object command, int value)
        {
            bool result = false;
            JValue jasonValue = new JValue(value);
            JetBusCommand _command = (JetBusCommand)command;
            SetData(_command.Path, jasonValue);
            WaitOne(1);
            result = true;
            return result;
        }

        /// <inheritdoc />
        public bool Write(object command, string value)
        {
            bool result = false;
            JValue jasonValue = new JValue(value);
            JetBusCommand _command = (JetBusCommand)command;
            SetData(_command.Path, jasonValue);
            WaitOne(1);
            result = true;
            return result;
        }

        /// <inheritdoc />
        public Task<int> WriteAsync(object command, int commandParam)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);

            // uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion

        #region =============== protected & private methods ================
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _successEvent.Close();
                    _successEvent.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void ConnectPeer(int timeoutMs)
        {
            _peer.Connect(OnConnected, timeoutMs);
        }

        private void OnConnected(bool connected)
        {
            if (connected)
            {
                FetchSelective();
                CommunicationLog?.Invoke(this, new LogEventArgs("Connection successful"));
            }
            else
            {
                _localException = new Exception("Connection failed");
                _successEvent.Set();
            }
        }

        private void FetchAll()
        {
            Matcher matcher = new Matcher();
            FetchId id;
            _peer.Fetch(out id, matcher, OnFetchData, OnFetch, this._timeoutMs);
        }


        string[] DSEFetchTargets = new string[]
        {
            "6152/00",
            "6138/02",
            "6138/01",
            "6013/01",
            "60B1/02",
            "60B1/01",
            "60A2/02",
            "60A2/01",
            "60A1/02",
            "60A1/01",
            "6144/00",
            "1030/01",
            "6000/01",
            "6021/01",
            "6141/02",
            "611C/01",
            "611C/02",
            "611C/03",
            "601A/01",
            "601A/01",
            "6149/01",
            "6149/02",
            "6149/03",
            "6149/04",
            "1018/02",
            "1011/01",
            "6050/01",
            "1010/01",
            "6111/01",
            "6116/01",
            "6002/01",
            "6002/02",
            "6040/01",
            "6113/01",
            "6114/01",
            "6112/01",
            "611B/01",
            "6118/03",
            "6118/02",
            "6118/01",
            "6117/01",
            "6020/01",
            "1018/04",
            "6110/03",
            "6110/01",
            "6110/02",
            "6143/00",
            "6015/01",
            "6014/01",
            "1018/01",
            "6153/00",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6012/01",
            "6016/01",
            "6142/00"
        };

        private void FetchSelective()
        {
            Matcher matcher = new Matcher();
            FetchId id;
            foreach (string fetchtarget in DSEFetchTargets)
            {
                matcher.EqualsTo = fetchtarget;
                _peer.Fetch(out id, matcher, OnFetchData, OnFetch, this._timeoutMs);
            }
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
                _localException = new Exception(exception.ErrorCode.ToString());
            }

            _successEvent.Set();

            this.UpdateData?.Invoke(this, new EventArgs());

            CommunicationLog?.Invoke(this, new LogEventArgs("Fetch-Selective success: " + success + " - Buffer size is " + AllData.Count));
        }            

        private void WaitOne(int timeoutMultiplier = 1)
        {
            if (!_successEvent.WaitOne(_timeoutMs * timeoutMultiplier))
            {
                throw new Exception("Jet connection timeout");
            }
                     
            if (_localException != null)
            {
                CommunicationLog?.Invoke(this, new LogEventArgs(_localException.Message));
                Exception exception = _localException;
                _localException = null;
                throw exception;
            }                        
        }
        
        /// <summary>
        /// Event will be called when device sends new fetch events
        /// </summary>
        /// <param name="data">New device data from jet peer</param>
        private void OnFetchData(JToken data)
        {
            string path = data["path"].ToString();

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
         
                CommunicationLog?.Invoke(this, new LogEventArgs(data.ToString()));
            }
        }
               
        /// <summary>
        /// Sets data for a single jet path
        /// </summary>
        /// <param name="path">Jet path for the data (e.g. "6002/01)"</param>
        /// <param name="value">The new value for the path</param>
        private void SetData(string path, JValue value)
        {
            _localException = null; 
            try
            {
                JObject request = _peer.Set(path.ToString(), value, OnSet, this._timeoutMs);
            }
            catch (Exception e)
            {
                _successEvent.Set();
                _localException = e;
            }
        }

        private void OnSet(bool success, JToken token)
        {
           if (!success)
           {
                _localException = new JetBusException(token);
            }
            
           _successEvent.Set();
            
           CommunicationLog?.Invoke(this, new LogEventArgs("Set data" + success));
        }
            
        /// <summary>
        /// Callback-Method wich is called from SslStream for SSL certificate validation
        /// </summary>
        /// <param name="sender">Sender holding the SSL stream</param>
        /// <param name="certificate">The SSL certificate to be validated</param>
        /// <param name="chain">SSL certification chain</param>
        /// <param name="sslPolicyErrors">Any policy violations</param>
        /// <returns>Indicates if validation was successful or not</returns>
        private bool RemoteCertificationCheck(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {            
            try
            {
                X509Certificate2 clientCertificate = new X509Certificate2(CertificateToByteArray(), string.Empty);                
                SslStream sslStream = sender as SslStream;
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
                return true;
            }
        }

        private byte[] CertificateToByteArray()
        {
            const string CERTIFICATE =
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
                "0oQiOFy / a6bR4Vw ==";
            byte[] _byteArray = Encoding.ASCII.GetBytes(CERTIFICATE);

            return _byteArray;
        }
        #endregion
    }
}
