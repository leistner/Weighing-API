
using HBM.Weighing.API;
using HBM.Weighing.API.Data;
using HBM.Weighing.API.WTX;
using HBM.Weighing.API.WTX.Jet;

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JetbusTest
{
    // Class for testing the set properties the class WTXJet:
    [TestFixture]
    public class SetTests
    {
        private TestJetbusConnection _jetTestConnection;
        private WTXJet _wtxObj;
        
        // Test case source for writing values to the WTX120 device : Zeroing
        public static IEnumerable setTests
        {
            get
            {
                yield return new TestCaseData(Behavior.setTestsFail).Returns(false);
                yield return new TestCaseData(Behavior.setTestsSuccess).Returns(true);
            }
        }

        [SetUp]
        public void Setup()
        {
        }


        [Test, TestCaseSource(typeof(SetTests), "setTests")]
        public bool SetActivationTimeTest(Behavior behavior)
        {
            _jetTestConnection = new TestJetbusConnection(behavior, "wss://172.19.103.8:443/jet/canopen", "Administrator", "wtx", delegate { return true; });

            _wtxObj = new WTXJet(_jetTestConnection,update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.DataStandard.LimitSwitch1Source = 10000;
            _wtxObj.DataStandard.LimitSwitch2Source = 10001;
            _wtxObj.DataStandard.LimitSwitch3Source = 10010;
            _wtxObj.DataStandard.LimitSwitch4Source = 10100;

            ((IDataFillerExtended)_wtxObj.DataFiller).ActivationTimeAfterFineFlow = 1;
            ((IDataFillerExtended)_wtxObj.DataFiller).CoarseFlowMonitoring = 10;
            ((IDataFillerExtended)_wtxObj.DataFiller).CoarseFlowMonitoringTime = 101;
            ((IDataFillerExtended)_wtxObj.DataFiller).CoarseLockoutTime = 110;
            ((IDataFillerExtended)_wtxObj.DataFiller).DelayTimeAfterFineFlow = 111;
            ((IDataFillerExtended)_wtxObj.DataFiller).EmptyingMode = 1000;
            ((IDataFillerExtended)_wtxObj.DataFiller).EmptyWeight = 1001;
            ((IDataFillerExtended)_wtxObj.DataFiller).FineFlowMonitoring = 1011;
            ((IDataFillerExtended)_wtxObj.DataFiller).FineFlowMonitoringTime = 1100;
            ((IDataFillerExtended)_wtxObj.DataFiller).FineLockoutTime = 1111;

            ((IDataFillerExtended)_wtxObj.DataFiller).LowerToleranceLimit = 11000;
            ((IDataFillerExtended)_wtxObj.DataFiller).MaximumDosingTime = 11001;
            ((IDataFillerExtended)_wtxObj.DataFiller).MinimumFineFlow = 11001;
            ((IDataFillerExtended)_wtxObj.DataFiller).MinimumStartWeight = 11010;
            ((IDataFillerExtended)_wtxObj.DataFiller).OptimizationOfCutOffPoints = 11100;
            ((IDataFillerExtended)_wtxObj.DataFiller).ResidualFlowTime = 11101;
            ((IDataFillerExtended)_wtxObj.DataFiller).SystematicDifference = 11110;
            ((IDataFillerExtended)_wtxObj.DataFiller).TareDelay = 11111;
            ((IDataFillerExtended)_wtxObj.DataFiller).TareMode = 100000;
            ((IDataFillerExtended)_wtxObj.DataFiller).UpperToleranceLimit = 100001;
            ((IDataFillerExtended)_wtxObj.DataFiller).ValveControl = 100010;           

            if (_jetTestConnection.getDataBuffer.ContainsKey("VCT") && _jetTestConnection.getDataBuffer.ContainsValue(100010))           
                return true;
            else
                if (_jetTestConnection.getDataBuffer.ContainsKey("6002/02") && _jetTestConnection.getDataBuffer.ContainsValue(1230))
                    return false;

            return false;
        }

        private void update(object sender, ProcessDataReceivedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void OnConnect(bool obj)
        {
            throw new NotImplementedException();
        }
    }
}