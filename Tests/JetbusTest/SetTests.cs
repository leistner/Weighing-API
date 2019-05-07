
using HBM.Weighing.API;
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

            _wtxObj.DataFillerExtended.ActivationTimeAfterFineFlow = 1;
            _wtxObj.DataFillerExtended.CoarseFlowMonitoring = 10;
            _wtxObj.DataFillerExtended.CoarseFlowMonitoringTime = 101;
            _wtxObj.DataFillerExtended.CoarseLockoutTime = 110;
            _wtxObj.DataFillerExtended.DelayTimeAfterFineFlow = 111;
            _wtxObj.DataFillerExtended.EmptyingMode = 1000;
            _wtxObj.DataFillerExtended.EmptyWeight = 1001;
            _wtxObj.DataFillerExtended.FineFlowMonitoring = 1011;
            _wtxObj.DataFillerExtended.FineFlowMonitoringTime = 1100;
            _wtxObj.DataFillerExtended.FineLockoutTime = 1111;

            _wtxObj.DataFillerExtended.LowerToleranceLimit = 11000;
            _wtxObj.DataFillerExtended.MaximumDosingTime = 11001;
            _wtxObj.DataFillerExtended.MinimumFineFlow = 11001;
            _wtxObj.DataFillerExtended.MinimumStartWeight = 11010;
            _wtxObj.DataFillerExtended.OptimizationOfCutOffPoints = 11100;
            _wtxObj.DataFillerExtended.ResidualFlowTime = 11101;
            _wtxObj.DataFillerExtended.SystematicDifference = 11110;
            _wtxObj.DataFillerExtended.TareDelay = 11111;
            _wtxObj.DataFillerExtended.TareMode = 100000;
            _wtxObj.DataFillerExtended.UpperToleranceLimit = 100001;
            _wtxObj.DataFillerExtended.ValveControl = 100010;           

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