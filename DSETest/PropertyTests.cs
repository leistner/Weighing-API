using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hbm.Weighing.Api.DSE;
using Hbm.Weighing.Api.DSE.Jet;
using Hbm.Weighing.Api.WTX.Jet;

namespace DSETest
{
    [TestClass]
    public class PropertyTests
    {
        private DSEJet _dse;
        private DSEJetConnection _connection;
        private string ipaddress = "192.168.178.46";

        [TestMethod]
        public void TestSerial()
        {
            _connection = new DSEJetConnection(ipaddress);
            _dse = new DSEJet(_connection, 500, null);
            _dse.Connect(200);
            Assert.AreEqual(true, _dse.SerialNumber.Length > 0);
            _dse.Disconnect();
        }

        [TestMethod]
        public void TestPropertySwitch()
        {
            _connection = new DSEJetConnection(ipaddress);
            _dse = new DSEJet(_connection, 500, null);
            _dse.Connect(200);
            System.Threading.Thread.Sleep(500);
            double tare = _dse.ManualTareValue;
            _dse.ManualTareValue = 0.040;
            System.Threading.Thread.Sleep(500);
            Assert.AreNotEqual(tare, _dse.ManualTareValue);
            _dse.ManualTareValue = tare;
            _dse.Disconnect();
        }

        [TestMethod]
        public void readProperties()
        {
            _connection = new DSEJetConnection(ipaddress);
            _dse = new DSEJet(_connection, 500, null);
            _dse.Connect(200);
            System.Threading.Thread.Sleep(500);
            foreach(var prop in _dse.GetType().GetProperties())
            {
                if (prop.ToString().Equals("Int32 LowPasCutOffFrequency") || prop.ToString().Equals("Int32 LowPassFilterOrder")) continue;
                Assert.AreNotEqual(null, prop.GetValue(_dse));
            }
            _dse.Disconnect();
        }
    }
}
