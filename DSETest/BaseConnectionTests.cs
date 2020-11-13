using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Hbm.Weighing.Api.DSE;
using Hbm.Weighing.Api.DSE.Jet;
using Hbm.Weighing.Api.WTX.Jet;

namespace DSETest
{
    [TestClass]
    public class BaseConnectionTests
    {
        private DSEJet _dse;
        private DSEJetConnection _connection;
        private string ipaddress = "192.168.178.46";

        [TestMethod]
        public void TestConnecting()
        {
            _connection = new DSEJetConnection(ipaddress);
            _dse = new DSEJet(_connection, 500, null);
            _dse.Connect(200);
            Assert.AreEqual(true, _dse.IsConnected);
            _dse.Disconnect();
        }

        [TestMethod]
        public void TestDisconnecting()
        {
            _connection = new DSEJetConnection(ipaddress);
            _dse = new DSEJet(_connection, 500, null);
            _dse.Connect(200);
            _dse.Disconnect();
            Assert.AreEqual(false, _dse.IsConnected);
        }

    }
}
