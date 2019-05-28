﻿
namespace Hbm.Weighing.API.WTX.Jet
{
    using System;
    using System.Collections;
    using NUnit.Framework;
    using Newtonsoft.Json.Linq;

    using Hbm.Weighing.API;
    using Hbm.Weighing.API.WTX;   
    using Hbm.Weighing.API.WTX.Modbus;   
    using Hbm.Weighing.API.WTX.Jet;

    [TestFixture]
    public class ConnectTestsJetbus
    {

        private INetConnection testConnection;

        private bool connectCallbackCalled;
        private bool connectCompleted;

        private int testGrossValue;

        public static IEnumerable Connect_TestCases_Jetbus
        {
            get
            {
                yield return new TestCaseData(Behavior.ConnectionSuccess).Returns(true);
                yield return new TestCaseData(Behavior.ConnectionFail).Returns(false);
            }
        }

        public static IEnumerable Disconnect_Testcases_Jetbus
        {
            get
            {
                yield return new TestCaseData(Behavior.DisconnectionSuccess).Returns(false);
                yield return new TestCaseData(Behavior.DisconnectionFail).Returns(true);
            }
        }

        [SetUp]
        public void Setup()
        {
            testGrossValue = 0; 

            this.connectCallbackCalled = false;
            this.connectCompleted = true;
        }

        [Test, TestCaseSource(typeof(ConnectTestsJetbus), "Connect_TestCases_Jetbus")]
        public bool TestConnectJetbus(Behavior behaviour)
        {        
            testConnection = new TestJetbusConnection(behaviour, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; },1000);

            WTXJet WTXJetObj = new WTXJet(testConnection, 500, Update);      

            this.connectCallbackCalled = false;

            WTXJetObj.Connect(this.OnConnect, 100);
            
            return WTXJetObj.IsConnected;
        }

        [Test, TestCaseSource(typeof(ConnectTestsJetbus), "Disconnect_Testcases_Jetbus")]
        public bool TestDisconnectJetbus(Behavior behaviour)
        {
            testConnection = new TestJetbusConnection(behaviour, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            WTXJet WTXJetObj = new WTXJet(testConnection, 500, Update);

            this.connectCallbackCalled = false;
            
            WTXJetObj.Connect(this.OnConnect, 100);

            WTXJetObj.Disconnect(this.OnDisconnect);

            return WTXJetObj.IsConnected;
        }

        private void Update(object sender, ProcessDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnConnect(bool completed)
        {
            this.connectCallbackCalled = true; 

            this.connectCompleted = completed;
        }


        private void OnDisconnect(bool completed)
        {
            this.connectCallbackCalled = false;

            this.connectCompleted = completed;
        }

    }
}
