
using Hbm.Weighing.API;
using Hbm.Weighing.API.Data;
using Hbm.Weighing.API.WTX;
using Hbm.Weighing.API.WTX.Jet;

using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JetbusTest
{
    // Class for testing read functions of JetBusConnection, like 'OnFetchData(JToken data)' and 
    // 'JToken ReadObj(object index)'.
    // In class JetBusConnection at #region read-functions:
    [TestFixture]
    public class ReadTests
    {
        private TestJetbusConnection _jetTestConnection;
        private WTXJet _wtxObj;
        private int _testInteger;
        private bool _testBoolean;
        private string _testString;


        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable ReadGrossValueTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ReadGrossValueFail).Returns(0);
                yield return new TestCaseData(Behavior.ReadGrossValueSuccess).Returns(1);
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable ReadNetValueTestCases
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadNetValueFail).Returns(0);
                yield return new TestCaseData(Behavior.ReadNetValueSuccess).Returns(1);
            }
        }

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_WEIGHING_DEVICE_1_WEIGHT_STATUS).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_WEIGHING_DEVICE_1_WEIGHT_STATUS).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device : Decimals 
        public static IEnumerable ReadTestCases_Decimals
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_Decimals).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_Decimals).ExpectedResult = 1;
            }
        }


        // Test case source for reading values from the WTX120 device : Filling process status 
        public static IEnumerable ReadTestCases_FillingProcessSatus
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_FillingProcessSatus).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_FillingProcessSatus).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device : Dosing result 
        public static IEnumerable ReadTestCases_DosingResult
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_DosingResult).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_DosingResult).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device : Number of dosing results 
        public static IEnumerable ReadTestCases_NumberDosingResults
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_NumberDosingResults).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_NumberDosingResults).ExpectedResult = 1;
            }
        }


        // Test case source for reading values from the WTX120 device : Unit or prefix or fixed parameters 
        public static IEnumerable ReadTestCases_Unit
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_Unit).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_Unit).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device : Unit or prefix or fixed parameters 
        public static IEnumerable ReadTestCases_Attributes
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_Attributes).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.ReadSuccess_Attributes).ExpectedResult = 1;
            }
        }

        // Test case source for reading values from the WTX120 device : Have data been reveiceid? (Property 'isDataReceived') 
        public static IEnumerable ReadTestCases_DataReceived
        {
            get
            {

                yield return new TestCaseData(Behavior.ReadFail_DataReceived).Returns(false);
                yield return new TestCaseData(Behavior.ReadSuccess_DataReceived).Returns(true);
            }
        }

        [SetUp]
        public void Setup()
        {
            _testInteger = 0;
            _testBoolean = false;
        }
      
        /*
        [Test, TestCaseSource(typeof(ReadTests), "ReadGrossValueTestCases")]
        public bool testReadGrossValue(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);
            
            _wtxObj.Connect(this.OnConnect, 100);        

            _testInteger = _wtxObj.GrossValue;

            //Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6144/00"));

            
            if (_jetTestConnection.getDataBuffer.ContainsKey("6144/00"))
                return true;
            else
                if (_jetTestConnection.getDataBuffer.ContainsKey(""))
                return false;
            return false;
           

        }
        */
        /*
        [Test, TestCaseSource(typeof(ReadTests), "ReadNetValueTestCases")]
        public void testReadNetValue(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.NetValue;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("601A/01"));

            //Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("601A/01"));
        }
        */

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testWeightMovingValue(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.WeightStable;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        
        private void OnConnect(bool obj)
        {
            throw new NotImplementedException();
        }
        
        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testGeneralWeightError(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.GeneralWeightError;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testScaleAlarmTriggered(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.ScaleAlarm;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testOverload(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.Overload;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testScaleSealIsOpen(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.LegalForTrade;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testWeightType(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.TareMode!=TareMode.None;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testScaleRange(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100,  update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.ProcessData.ScaleRange;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testZeroRequired(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.ZeroRequired;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testWeightWithinTheCenterOfZero(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.CenterOfZero;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_WEIGHING_DEVICE_1_WEIGHT_STATUS")]
        public void testWeightInZeroRange(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testBoolean = _wtxObj.ProcessData.InsideZero;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6012/01"));
        }



        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Decimals")]
        public void testDecimals(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.ProcessData.Decimals;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6013/01"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_FillingProcessSatus")]
        public void testFillingProcessStatus(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).FillingProcessStatus;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("SDO"));
        }
        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_DosingResult")]
        public void testDosingResult(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).DosingResult;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FRS1"));
        }


        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_NumberDosingResults")]
        public void testNumberDosingResults(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).NumberDosingResults;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("NDS"));
        }

        /*
        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Unit")]
        public void testUnit(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testString = _wtxObj.ProcessData.Unit;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("6014/01"));
        }
        */

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testInput1(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Input1;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("IM1"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testInput2(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Input2;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("IM2"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testInput3(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Input3;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("IM3"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testInput4(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Input4;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("IM4"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testOutput1(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Output1;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OM1"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testOutput2(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Output2;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OM2"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testOutput3(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Output3;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OM3"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testOutput4(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.Output4;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OM4"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitStatus1(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.LimitStatus1;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OS1"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitStatus2(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.LimitStatus2;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OS2"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitStatus3(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.LimitStatus3;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OS3"));
        }


        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitStatus4(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = _wtxObj.DataStandard.LimitStatus4;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OS4"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testMaxDosingTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).MaxDosingTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("MDT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testMeanValueDosingResults(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).MeanValueDosingResults;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("SDM"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testStandardDeviation(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).StandardDeviation;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("SDS"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testFineFlowCutOffPoint(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).FineFlowCutOffPoint;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FFD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testCoarseFlowCutOffPoint(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).CoarseFlowCutOffPoint;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("CFD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testResidualFlowTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).ResidualFlowTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("RFT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testMinimumFineFlow(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).MinimumFineFlow;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FFM"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testOptimizationOfCutOffPoints(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).OptimizationOfCutOffPoints;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("OSN"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testMaximumDosingTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).MaximumDosingTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("MDT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testCoarseLockoutTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).CoarseLockoutTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("CFT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testFineLockoutTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).FineLockoutTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FFT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testTareMode(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).TareMode;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("TMD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testUpperToleranceLimit(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).UpperToleranceLimit;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("UTL"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLowerToleranceLimit(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).LowerToleranceLimit;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("LTL"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testMinimumStartWeight(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).MinimumStartWeight;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("MSW"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testEmptyWeight(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).EmptyWeight;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("EWT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testTareDelay(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).TareDelay;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("TAD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testCoarseFlowMonitoringTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).CoarseFlowMonitoringTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("CBT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testCoarseFlowMonitoring(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).CoarseFlowMonitoring;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("CBK"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testFineFlowMonitoring(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).FineFlowMonitoring;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FBK"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testFineFlowMonitoringTime(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).FineFlowMonitoringTime;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FBT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testSystematicDifference(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).SystematicDifference;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("SYD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testValveControl(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).ValveControl;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("VCT"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testEmptyingMode(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).EmptyingMode;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("EMD"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testDelayTimeAfterFineFlow(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).DelayTimeAfterFineFlow;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("DL1"));
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testActivationTimeAfterFineFlow(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            _testInteger = ((IDataFillerExtended)_wtxObj.DataFiller).ActivationTimeAfterFineFlow;

            Assert.IsTrue(_jetTestConnection.getDataBuffer.ContainsKey("FFL"));
        }
     
        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitSwitchStatusLVS1(Behavior behavior)
        {
            bool testVar = false;

            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            int LimitSwitch1 = _wtxObj.DataStandard.LimitStatus1;

            if (_jetTestConnection.getDataBuffer.ContainsKey("2020/25") == true && LimitSwitch1 == 0)
                testVar = true;
            else
                testVar = false;

            Assert.IsTrue(testVar);
        }

        /*
        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitSwitchStatusLVS2(Behavior behavior)
        {
            bool testVar = false;

            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);
        
            int LimitSwitch2 = _wtxObj.DataStandard.LimitSwitch2Source;

            if (_jetTestConnection.getDataBuffer.ContainsKey("2020/25") == true && LimitSwitch2 == 1)
                testVar = true;
            else
                testVar = false;

            Assert.IsTrue(testVar);
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitSwitchStatusLVS3(Behavior behavior)
        {
            bool testVar = false;

            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            int LimitSwitch3 = _wtxObj.DataStandard.LimitSwitch3Source;

            if (_jetTestConnection.getDataBuffer.ContainsKey("2020/25") == true && LimitSwitch3 == 0)
                testVar = true;
            else
                testVar = false;

            Assert.IsTrue(testVar);
        }

        [Test, TestCaseSource(typeof(ReadTests), "ReadTestCases_Attributes")]
        public void testLimitSwitchStatusLVS4(Behavior behavior)
        {
            bool testVar = false;

            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection, 100, update);

            _wtxObj.Connect(this.OnConnect, 100);

            int LimitSwitch4 = _wtxObj.DataStandard.LimitSwitch4Source;

            if (_jetTestConnection.getDataBuffer.ContainsKey("2020/25") == true && LimitSwitch4 == 1)
                testVar = true;
            else
                testVar = false;

            Assert.IsTrue(testVar);
        }
        */
        private void update(object sender, ProcessDataReceivedEventArgs e)
        {
        }
    }
}
