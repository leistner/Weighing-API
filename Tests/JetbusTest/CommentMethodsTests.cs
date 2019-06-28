﻿// <copyright file="CommentMethodsTests.cs" company="Hottinger Baldwin Messtechnik GmbH">
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

namespace JetbusTest
{
    using System;
    using System.Collections;
    using Hbm.Weighing.API.Data;
    using Hbm.Weighing.API.WTX;
    using Hbm.Weighing.API.WTX.Jet;
    using NUnit.Framework;
    [TestFixture]
    public class CommentMethodsTests
    {
        private TestJetbusConnection _jetTestConnection;
        private WTXJet _wtxObj;

        private string value;
        
        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable T_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.t_UnitValue_Fail).Returns(false);
                yield return new TestCaseData(Behavior.t_UnitValue_Success).Returns(true);

            }
        }
     
        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable KG_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.kg_UnitValue_Fail).Returns(false);
                yield return new TestCaseData(Behavior.kg_UnitValue_Success).Returns(true);
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable G_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.g_UnitValue_Fail).Returns(false);
                yield return new TestCaseData(Behavior.g_UnitValue_Success).Returns(true);

            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable LB_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.lb_UnitValue_Fail).Returns(false);
                yield return new TestCaseData(Behavior.lb_UnitValue_Success).Returns(true);

            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable N_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.N_UnitValue_Fail).Returns(false);
                yield return new TestCaseData(Behavior.N_UnitValue_Success).Returns(true);

            }
        }


        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable NetGrossValueStringComment_1D_TestCase
        {
            get
            {
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_1D_Fail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_1D_Success).ExpectedResult = 1;               
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable NetGrossValueStringComment_2D_TestCase
        {
            get
            {
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_2D_Fail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_2D_Success).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable NetGrossValueStringComment_3D_TestCase
        {
            get
            {
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_3D_Fail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_3D_Success).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable NetGrossValueStringComment_4D_TestCase
        {
            get
            {
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_4D_Fail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.NetGrossValueStringComment_4D_Success).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable LimitValues_TestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.LimitValues_Underload).Returns("Underload");
                yield return new TestCaseData(Behavior.LimitValues_Overload).Returns("Overload");
                yield return new TestCaseData(Behavior.LimitValues_WeightWithinLimits).Returns("WeightWithinLimits");
                yield return new TestCaseData(Behavior.LimitValues_HigherSafeLoadLimit).Returns("HigherSafeLoadLimit");
            }
        }

        [SetUp]
        public void Setup()
        {
            value = "0";
        }

       
        [Test, TestCaseSource(typeof(CommentMethodsTests), "LimitValues_TestCases")]
        public string test_LimitValues(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            if (_wtxObj.ProcessData.Underload == false && _wtxObj.ProcessData.Overload == false && _wtxObj.ProcessData.HigherSafeLoadLimit == false)
                return "WeightWithinLimits";
                else
                    if (_wtxObj.ProcessData.Underload == true && _wtxObj.ProcessData.Overload == false && _wtxObj.ProcessData.HigherSafeLoadLimit == false)
                    return "Underload";
                    else
                        if (_wtxObj.ProcessData.Underload == false && _wtxObj.ProcessData.Overload == true && _wtxObj.ProcessData.HigherSafeLoadLimit == false)
                            return "Overload";
                        else
                            if (_wtxObj.ProcessData.Underload == false && _wtxObj.ProcessData.Overload == false && _wtxObj.ProcessData.HigherSafeLoadLimit == true)
                                return "HigherSafeLoadLimit";
                                else
                                    return "";
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_4D_TestCase")]
        public void test_NetGrossValueStringComment_4Decimals(Behavior behavior)
        {
             TestJetbusConnection _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            WTXJet _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0, 0, 4);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 4);

            Assert.AreEqual(dValue.ToString("0.0000"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_3D_TestCase")]
        public void test_NetGrossValueStringComment_3Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0, 0, 3);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 3);

            Assert.AreEqual(dValue.ToString("0.000"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_2D_TestCase")]
        public void test_NetGrossValueStringComment_2Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0, 0, 2);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 2);

            Assert.AreEqual(dValue.ToString("0.00"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_1Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0, 0, 1);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 1);

            Assert.AreEqual(dValue.ToString("0.0"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_5Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0, 0, 5);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 5);

            Assert.AreEqual(dValue.ToString("0.00000"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_6Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.PrintableWeight.Update(0,0, 6);

            double dValue = _wtxObj.ProcessData.Weight.Net / Math.Pow(10, 6);

            Assert.AreEqual(dValue.ToString("0.000000"), _wtxObj.PrintableWeight.Net.Replace(".", ","));
        }
 
        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_Default(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 7);

            Assert.AreEqual(dValue.ToString(), _wtxObj.PrintableWeight.Net);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_0Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 0);

            Assert.AreEqual(dValue.ToString(), _wtxObj.PrintableWeight.Net);
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "T_UnitValueTestCases")]
        public bool testUnit_t(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            if (value.Equals("t"))
                return true;
            else
                return false;
        }
    
        [Test, TestCaseSource(typeof(CommentMethodsTests), "KG_UnitValueTestCases")]
        public bool testUnit_kg(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            if (value.Equals("kg"))
                return true;
            else
                return false;
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "G_UnitValueTestCases")]
        public bool testUnit_g(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            if (value.Equals("g"))
                return true;
            else
                return false;
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "LB_UnitValueTestCases")]
        public bool testUnit_lb(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            if (value.Equals("lb"))
                return true;
            else
                return false;
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "N_UnitValueTestCases")]
        public bool testUnit_N(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            if (value.Equals("N"))
                return true;
            else
                return false;
        }
       
        private void update(object sender, ProcessDataReceivedEventArgs e)
        {
        }

        private void OnConnect(bool obj)
        {
        }
    }
}
