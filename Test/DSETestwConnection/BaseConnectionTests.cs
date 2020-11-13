﻿// <copyright file="BaseConnectionTests.cs" company="Hottinger Baldwin Messtechnik GmbH">
//
// WTXGUIsimple, a demo application for HBM Weighing-API  
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

namespace Hbm.Ie.Api.Test.DSETestwConnection
{
    using Hbm.Ie.Api.Weighing.DSE;
    using Hbm.Ie.Api.Weighing.DSE.Jet;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class BaseConnectionTests
    {
        private DSEJet _dse;
        private DSEJetConnection _connection;
        private string ipaddress = "172.19.104.11";

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
