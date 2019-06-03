﻿
using Hbm.Weighing.API;
using Hbm.Weighing.API.WTX;
using Hbm.Weighing.API.WTX.Jet;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace JetbusTest
{
    [TestFixture]
    public class CommentMethodsTests
    {
        private TestJetbusConnection _jetTestConnection;
        private WTXJet _wtxObj;

        //private int value;

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable T_UnitValueTestCases
        {
            get
            {
                //yield return new TestCaseData(Behavior.t_UnitValue_Fail).ExpectedResult = "";
                yield return new TestCaseData(Behavior.t_UnitValue_Success).ExpectedResult = "t";

            }
        }
     
        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable KG_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.kg_UnitValue_Fail).ExpectedResult = "";
                yield return new TestCaseData(Behavior.kg_UnitValue_Success).ExpectedResult = "kg";

            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable G_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.g_UnitValue_Fail).ExpectedResult = "";
                yield return new TestCaseData(Behavior.g_UnitValue_Success).ExpectedResult = "g";

            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable LB_UnitValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.lb_UnitValue_Fail).ExpectedResult = "";
                yield return new TestCaseData(Behavior.lb_UnitValue_Success).ExpectedResult = "lb";

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
        public static IEnumerable StatusStringComment_TestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.StatusStringComment_Fail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.StatusStringComment_Success).ExpectedResult = 1;
            }
        }

        [SetUp]
        public void Setup()
        {
            //value = 0;
        }

       /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_OK(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            value = _wtxObj.ProcessData.Status;

            string Strvalue = _wtxObj.StatusStringComment();
            
            Assert.AreEqual("Execution OK!", Strvalue);
        }
        */
        /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_ONGO(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            value = 1634168417;

            string Strvalue = _wtxObj.StatusStringComment(value);

            Assert.AreEqual("Execution on go!", Strvalue);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_ErrorE1(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            value = 826629983;

            string Strvalue = _wtxObj.StatusStringComment(value);

            Assert.AreEqual("Error 1, E1", Strvalue);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_ErrorE2(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            value = 843407199;

            string Strvalue = _wtxObj.StatusStringComment(value);

            Assert.AreEqual("Error 2, E2", Strvalue);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_ErrorE3(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            value = 860184415;

            string Strvalue = _wtxObj.ProcessData.StatusStringComment(value);

            Assert.AreEqual("Error 3, E3", Strvalue);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "StatusStringComment_TestCases")]
        public void test_StatusStringComment_Default(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            value = 111199990;

            string Strvalue = _wtxObj.StatusStringComment(value);

            Assert.AreEqual("Invalid status", Strvalue);
        }
        */

        private double _grossValue = 0.0;
        private double _decimals = 0.0;

        /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_4D_TestCase")]
        public void test_NetGrossValueStringComment_4Decimals(Behavior behavior)
        {
             TestJetbusConnection _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

             WtxJet _wtxObj = new WtxJet(_jetTestConnection, update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            string strValue=_wtxObj.CurrentWeight(_grossValue, _decimals);

            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, _wtxObj.ProcessData.Decimals);

            Assert.AreEqual(dValue.ToString("0.0000"), strValue);
        }
        */
        /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_3D_TestCase")]
        public void test_NetGrossValueStringComment_3Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 3);

            Assert.AreEqual(dValue.ToString("0.000"), _wtxObj.PrintableWeight.Net);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_2D_TestCase")]
        public void test_NetGrossValueStringComment_2Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 2);

            Assert.AreEqual(dValue.ToString("0.00"), _wtxObj.PrintableWeight.Net);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_1Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 1);

            Assert.AreEqual(dValue.ToString("0.0"), _wtxObj.PrintableWeight.Net);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_5Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 5);

            Assert.AreEqual(dValue.ToString("0.00000"), _wtxObj.PrintableWeight.Net);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "NetGrossValueStringComment_1D_TestCase")]
        public void test_NetGrossValueStringComment_6Decimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            double dValue = _wtxObj.ProcessData.Weight.Gross / Math.Pow(10, 6);

            Assert.AreEqual(dValue.ToString("0.000000"), _wtxObj.PrintableWeight.Net);
        }
        */
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

        /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "T_UnitValueTestCases")]
        public void testUnit_t(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);
            
            _wtxObj.ProcessData.UpdateData(this, new EventArgs());

            value = _wtxObj.ProcessData.Unit;

            Assert.AreEqual("t", _wtxObj.Unit);
        }
        */

        private void update(object sender, ProcessDataReceivedEventArgs e)
        {
            _grossValue = e.ProcessData.Weight.Gross;
            _decimals =   e.ProcessData.Decimals;
        }
        /*
        [Test, TestCaseSource(typeof(CommentMethodsTests), "KG_UnitValueTestCases")]
        public void testUnit_kg(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateProcessData(this, new DataEventArgs(_jetTestConnection.AllData));

            value = _wtxObj.ProcessData.Unit;

            Assert.AreEqual("kg", _wtxObj.Unit);
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "G_UnitValueTestCases")]
        public void testUnit_g(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateProcessData(this, new DataEventArgs(_jetTestConnection.AllData));

            value = _wtxObj.ProcessData.Unit;

            Assert.AreEqual("g", _wtxObj.Unit);
        }

        [Test, TestCaseSource(typeof(CommentMethodsTests), "LB_UnitValueTestCases")]
        public void testUnit_lb(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateProcessData(this, new DataEventArgs(_jetTestConnection.AllData));

            value = _wtxObj.ProcessData.Unit;

            Assert.AreEqual("lb", _wtxObj.Unit);
        }
        
        [Test, TestCaseSource(typeof(CommentMethodsTests), "LB_UnitValueTestCases")]
        public void testUnit_default(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WtxJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ProcessData.UpdateProcessData(this, new DataEventArgs(_jetTestConnection.AllData));

            value = _wtxObj.ProcessData.Unit;

            Assert.AreEqual("error", _wtxObj.Unit);
        }
        */
        private void OnConnect(bool obj)
        {
            throw new NotImplementedException();
        }
    }
}
