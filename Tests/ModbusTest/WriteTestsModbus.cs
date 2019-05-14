using HBM.Weighing.API.WTX.Modbus;
using NUnit.Framework;
using System;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Modbus
{
    [TestFixture]
    public class WriteTestsModbus
    {

        private TestModbusTCPConnection testConnection;
        private WTXModbus _wtxObj;

        private bool connectCallbackCalled;
        private bool connectCompleted;

        private bool disconnectCallbackCalled;
        private bool disconnectCompleted;

        // Test case source for writing values to the WTX120 device. 
        public static IEnumerable WriteTestCases
        {
            get
            {
                //yield return new TestCaseData(Behavior.WriteFail).Returns(0x2);
                yield return new TestCaseData(Behavior.WriteSuccess).Returns(0);
            }
        }

        public static IEnumerable WriteArrayTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteArrayFail).Returns(false);
                yield return new TestCaseData(Behavior.WriteArraySuccess).Returns(true);
            }
        }

        // Test case source for writing values to the WTX120 device. 
        public static IEnumerable WriteSyncTestModbus
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteSyncSuccess).Returns(0x100);
                yield return new TestCaseData(Behavior.WriteSyncFail).Returns(0);
            }
        }

        public static IEnumerable MeasureZeroTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.MeasureZeroFail).Returns(false);
                yield return new TestCaseData(Behavior.MeasureZeroSuccess).Returns(true);
            }
        }

        public static IEnumerable AsyncWriteBackgroundworkerCase
        {
            get
            {
                yield return new TestCaseData(Behavior.AsyncWriteBackgroundworkerSuccess).Returns(true);
                yield return new TestCaseData(Behavior.AsyncWriteBackgroundworkerFail).Returns(true);
            }
        }

        // Test case source for writing values to the WTX120 device. 
        public static IEnumerable TareTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.TareFail).ExpectedResult = 0;
                yield return new TestCaseData(Behavior.TareSuccess).ExpectedResult = 0x1;
            }
        }

        public static IEnumerable WriteHandshakeTestModbus
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteHandshakeTestSuccess).Returns(0x1);
                yield return new TestCaseData(Behavior.WriteHandshakeTestFail).Returns(0x0);
            }
        }

        public static IEnumerable GrosMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.GrosMethodTestSuccess).ExpectedResult = (0x2);
                yield return new TestCaseData(Behavior.GrosMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable TareMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.TareMethodTestSuccess).ExpectedResult = (0x1);
                yield return new TestCaseData(Behavior.TareMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable ZeroMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ZeroMethodTestSuccess).ExpectedResult=(0x40);
                yield return new TestCaseData(Behavior.ZeroMethodTestFail).ExpectedResult=(0x0);
            }
        }

        public static IEnumerable AdjustingZeroMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.AdjustingZeroMethodSuccess).ExpectedResult = (0x80);
                yield return new TestCaseData(Behavior.AdjustingZeroMethodFail).ExpectedResult = (0x0);
            }
        }

        public static IEnumerable AdjustNominalMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.AdjustNominalMethodTestSuccess).ExpectedResult = (0x100);
                yield return new TestCaseData(Behavior.AdjustNominalMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable ActivateDataMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ActivateDataMethodTestSuccess).ExpectedResult = (0x800);
                yield return new TestCaseData(Behavior.ActivateDataMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable ManualTaringMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ManualTaringMethodTestSuccess).ExpectedResult = (0x1000);
                yield return new TestCaseData(Behavior.ManualTaringMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable ClearDosingResultsMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ClearDosingResultsMethodTestSuccess).ExpectedResult = (0x4);
                yield return new TestCaseData(Behavior.ClearDosingResultsMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable AbortDosingMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.AbortDosingMethodTestSuccess).ExpectedResult = (0x8);
                yield return new TestCaseData(Behavior.AbortDosingMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable StartDosingMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.StartDosingMethodTestSuccess).ExpectedResult = (0x10);
                yield return new TestCaseData(Behavior.StartDosingMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable RecordWeightMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.RecordWeightMethodTestSuccess).ExpectedResult = (0x4000);
                yield return new TestCaseData(Behavior.RecordWeightMethodTestFail).ExpectedResult = (0x0);
            }
        }
        public static IEnumerable ManualRedosingMethodTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ManualRedosingMethodTestSuccess).ExpectedResult = (0x8000);
                yield return new TestCaseData(Behavior.ManualRedosingMethodTestFail).ExpectedResult = (0x0);
            }
        }

        public static IEnumerable WriteS32ArrayTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteS32ArrayTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteS32ArrayTestFail).Returns(false);
            }
        }


        public static IEnumerable WriteU16ArrayTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteU16ArrayTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteU16ArrayTestFail).Returns(false);
            }
        }

        public static IEnumerable WriteU08ArrayTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteU08ArrayTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteU08ArrayTestFail).Returns(false);
            }
        }

        public static IEnumerable ResetTimerTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ResetTimerTestSuccess).Returns(500);
                //yield return new TestCaseData(Behavior.ResetTimerTestFail).Returns(200);
            }
        }

        public static IEnumerable UpdateOutputTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.UpdateOutputTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.UpdateOutputTestFail).Returns(false);
            }
        }

        public static IEnumerable WriteLimitSwitch1ModeTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteLimitSwitch1ModeTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteLimitSwitch1ModeTestFail).Returns(false);
            }
        }

        public static IEnumerable WriteLimitSwitch2ModeTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteLimitSwitch2ModeTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteLimitSwitch2ModeTestFail).Returns(false);
            }
        }
        public static IEnumerable WriteLimitSwitch3ModeTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteLimitSwitch3ModeTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteLimitSwitch3ModeTestFail).Returns(false);
            }
        }
        public static IEnumerable WriteLimitSwitch4ModeTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.WriteLimitSwitch4ModeTestSuccess).Returns(true);
                yield return new TestCaseData(Behavior.WriteLimitSwitch4ModeTestFail).Returns(false);
            }
        }


        [SetUp]
        public void Setup()
        {
            this.connectCallbackCalled = true;
            this.connectCompleted = true;

        }

        // Tests for writing limit values : 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteLimitSwitch1ModeTestCases")]
        public bool LimitSwitch1ModeWriteTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.DataStandard.LimitSwitch1Mode = 1;

            if (testConnection.getCommand == 4 && _wtxObj.DataStandard.LimitSwitch1Mode == 1)
                return true;

            else
                return false;
        }

        // Tests for writing limit values : 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteLimitSwitch2ModeTestCases")]
        public bool LimitSwitch2ModeWriteTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);
            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.DataStandard.LimitSwitch2Mode = 2;

            if (testConnection.getCommand == 11 && _wtxObj.DataStandard.LimitSwitch2Mode == 2)
                return true;

            else
                return false;
        }

        // Tests for writing limit values : 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteLimitSwitch3ModeTestCases")]
        public bool LimitSwitch3ModeWriteTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);
            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.DataStandard.LimitSwitch3Mode = 3;

            if (testConnection.getCommand == 17 && _wtxObj.DataStandard.LimitSwitch3Mode == 3)
                return true;

            else
                return false;
        }

        // Tests for writing limit values : 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteLimitSwitch4ModeTestCases")]
        public bool LimitSwitch4ModeWriteTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);
            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.DataStandard.LimitSwitch4Mode = 4;

            if (testConnection.getCommand == 23 && _wtxObj.DataStandard.LimitSwitch4Mode == 4)
                return true;
            else
                return false;
        }

        // Test for method : Zeroing
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ZeroMethodTestCases")]
        public void ZeroMethodTestModbus(Behavior behavior)
        {
            int command = 0;

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.Zero();

            command = _wtxObj.getCommand;

            if (behavior == Behavior.ZeroMethodTestSuccess)
                Assert.AreEqual(0x40, command);
            else
                if (behavior == Behavior.ZeroMethodTestFail)
                Assert.AreEqual(0x00, command);
          
        }
        /*
        // Test for synchronous writing : Tare 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteSyncTestModbus")]
        public int WriteSyncTest(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");

            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.WriteSync(0, 0x100);

            return testConnection.getCommand;
        }
        */
        /*
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteArrayTestCases")]
        public bool WriteArrayTestCasesModbus(Behavior behavior)
        {
            bool parameterEqualArrayWritten = false;

            TestModbusTCPConnection testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            WtxModbus _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.WriteOutputWordS32(0x7FFFFFFF, 50);

            if ((testConnection.getArrElement1 == (0x7FFFFFFF & 0xffff0000) >> 16) &&
                (testConnection.getArrElement2 == (0x7FFFFFFF & 0x0000ffff)))
            {
                parameterEqualArrayWritten = true;
            }
            else
            {
                parameterEqualArrayWritten = false;
            }

            //Assert.AreEqual(true ,parameterEqualArrayWritten);

            return parameterEqualArrayWritten;
        }
        */


        // Test for writing : Tare 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "TareTestCases")]
        public void TareAsyncTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.Tare();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x1, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);

        }

        private void Update(object sender, ProcessDataReceivedEventArgs e)
        {

        }

        private void OnConnect(bool obj)
        {
            throw new NotImplementedException();
        }

        // Test for method : Switch to gross value or net value
        [Test, TestCaseSource(typeof(WriteTestsModbus), "GrosMethodTestCases")]
        public void GrosMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.SetGross();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x2, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }

        // Test for method : Taring
        [Test, TestCaseSource(typeof(WriteTestsModbus), "TareMethodTestCases")]
        public void TareMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.Tare();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x1, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);

        }
        /*
        // Test for method : Adjusting zero
        [Test, TestCaseSource(typeof(WriteTestsModbus), "AdjustingZeroMethodTestCases")]
        public void AdjustingZeroMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.AdjustZeroSignal();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x80, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }
        */
        // Test for method : Adjusting nominal
        [Test, TestCaseSource(typeof(WriteTestsModbus), "AdjustNominalMethodTestCases")]
        public void AdjustingNominalMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.AdjustNominalSignal();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x100, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }

        // Test for method : Adjusting nominal
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ActivateDataMethodTestCases")]
        public void /*int*/ ActivateDataMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ActivateData();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x800, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }

        // Test for method : Adjusting nominal
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ManualTaringMethodTestCases")]
        public void ManualTaringTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.TareManually(200.0); 

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x1000, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }


        /* No filling in baseWTDevice !
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ClearDosingResultsMethodTestCases")]
        public void ClearDosingResultsMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ClearDosingResults();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x4, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }
        */
        /* No filling in baseWTDevice !
        [Test, TestCaseSource(typeof(WriteTestsModbus), "AbortDosingMethodTestCases")]
        public void AbortDosingMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.AbortDosing();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x8, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }
        */
        /*
// Test for method : Zeroing
[Test, TestCaseSource(typeof(WriteTestsModbus), "ZeroMethodTestCases")]
public async Task ZeroMethodTestModbus(Behavior behavior)
{
    int command = 0;

    testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
    _wtxObj = new WtxModbus(testConnection, 200,Update);

    _wtxObj.Connect(this.OnConnect, 100);

    command = await _wtxObj.AsyncWrite(0, 0x40);

    if (behavior == Behavior.ZeroMethodTestSuccess)
        Assert.AreEqual(0x40, command);
    else
        if (behavior == Behavior.ZeroMethodTestFail)
        Assert.AreEqual(0x00, command);
}
*/


        /* No filling in baseWTDevice !
        [Test, TestCaseSource(typeof(WriteTestsModbus), "StartDosingMethodTestCases")]
        public void StartDosingMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.StartDosing();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x10, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }
        */

        // Test for method : Record weight
        [Test, TestCaseSource(typeof(WriteTestsModbus), "RecordWeightMethodTestCases")]
        public void RecordweightMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.RecordWeight();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x4000, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }


        /* No filling in baseWTDevice !
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ManualRedosingMethodTestCases")]
        public void ManualRedosingMethodTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ManualReDosing();

            if (behavior == Behavior.TareMethodTestSuccess)
                Assert.AreEqual(0x8000, _wtxObj.getCommand);
            else
                if (behavior == Behavior.TareMethodTestFail)
                Assert.AreEqual(0x0, _wtxObj.getCommand);
        }
        */

        /*
        // Test for method : Write an Array of type signed integer 32 bit. 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteS32ArrayTestCases")]
        public bool WriteS32ArrayTestModbus(Behavior behavior)
        {
            ushort[] _data = new ushort[2];

            _data[0] = (ushort)((0x7FFFFFFF & 0xFFFF0000) >> 16);
            _data[1] = (ushort)(0x7FFFFFFF & 0x0000FFFF);

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.WriteOutputWordS32(0x7FFFFFFF, 48);

            if (testConnection.getArrElement1 == _data[0] && testConnection.getArrElement2 == _data[1] &&
                testConnection.getWordNumber == 48)
                return true;
            else
                return false;

        }
        
         /*
        // Test for method : Write an Array of type unsigned integer 16 bit. 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteU16ArrayTestCases")]
        public bool WriteU16ArrayTestModbus(Behavior behavior)
        {
            ushort[] _data = new ushort[1];

            _data[0] = (ushort)((0x7FFFFFFF & 0xFFFF0000) >> 16);

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.WriteOutputWordU16(0x7FFFFFFF, 50);

            if (testConnection.getArrElement1 == _data[0] && testConnection.getWordNumber == 50)
                return true;
            else
                return false;

        }
        */
        /*
        // Test for method : Write an Array of type unsigned integer 16 bit. 
        [Test, TestCaseSource(typeof(WriteTestsModbus), "WriteU08ArrayTestCases")]
        public bool WriteU08ArrayTestModbus(Behavior behavior)
        {
            ushort[] _data = new ushort[1];

            _data[0] = (ushort)(0xA1 & 0xFF);

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.WriteOutputWordU08(0xA1, 1);

            if (testConnection.getArrElement1 == _data[0] && testConnection.getWordNumber == 1)
                return true;
            else
                return false;

        }
        */
        [Test, TestCaseSource(typeof(WriteTestsModbus), "ResetTimerTestCases")]
        public int ResetTimerTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WTXModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.ResetTimer(500);

            return (int)_wtxObj._aTimer.Interval;
            //Assert.AreEqual(_wtxObj._aTimer.Interval, 500);
        }
        /*
        [Test, TestCaseSource(typeof(WriteTestsModbus), "UpdateOutputTestCases")]
        public bool UpdateOutputTest(Behavior behavior)
        {
            bool compareDataWritten = false;

            ushort[] _dataWritten = new ushort[2];
            ushort[] _outputData = new ushort[43];

            for (int i = 0; i < _outputData.Length; i++)
                _outputData[i] = 1;

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.UpdateOutputWords(_outputData);

            for (int i = 0; i < _outputData.Length; i++)
            {
                _wtxObj.WriteOutputWordS32(_outputData[i], (ushort)(i + 40));

                _dataWritten[0] = (ushort)((_outputData[i] & 0xffff0000) >> 16);
                _dataWritten[1] = (ushort)(_outputData[i] & 0x0000ffff);

                if (testConnection.getArrElement1 == _dataWritten[0] && testConnection.getArrElement2 == _dataWritten[1])
                    compareDataWritten = true;
                else
                    compareDataWritten = false;
            }

            _wtxObj.activateData();

            if (compareDataWritten == true)

                return true;

            else
                return false;

        }
        */
        /*
        [Test, TestCaseSource(typeof(WriteTestsModbus), "UpdateOutputTestCases")]
        public bool UpdateOutput1Test(Behavior behavior)
        {
            bool compareDataWritten = false;

            ushort[] _dataWritten = new ushort[2];
            ushort[] _outputData = new ushort[43];

            for (int i = 0; i < _outputData.Length; i++)
                _outputData[i] = 1;

            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxObj = new WtxModbus(testConnection, 200, Update);

            _wtxObj.Connect(this.OnConnect, 100);

            _wtxObj.UpdateOutputWords(_outputData);

            for (int i = 0; i < _outputData.Length; i++)
            {
                _wtxObj.WriteOutputWordS32(_outputData[i], (ushort)(i + 40));

                _dataWritten[0] = (ushort)((_outputData[i] & 0xffff0000) >> 16);
                _dataWritten[1] = (ushort)(_outputData[i] & 0x0000ffff);

                if (testConnection.getArrElement1 == _dataWritten[0] && testConnection.getArrElement2 == _dataWritten[1] && testGetOutputwords() == true)
                    compareDataWritten = true;
                else
                    compareDataWritten = false;
            }

            _wtxObj.activateData();

            if (compareDataWritten == true) // && testConnection.getCommand==0x800)

                return true;

            else
                return false;

        }
        */

        private bool testGetOutputwords()
        {
            if
             (
            _wtxObj.ManualTareValue == 1 && _wtxObj.DataStandard.LimitSwitch1Source == 1 && _wtxObj.DataStandard.LimitSwitch1Mode == 1 && _wtxObj.DataStandard.LimitSwitch1Level == 1 && _wtxObj.DataStandard.LimitSwitch1Hysteresis == 1 &&
            _wtxObj.DataStandard.LimitSwitch2Source == 1 && _wtxObj.DataStandard.LimitSwitch2Mode == 1 && _wtxObj.DataStandard.LimitSwitch2Level == 1 && _wtxObj.DataStandard.LimitSwitch2Hysteresis == 1 && _wtxObj.DataStandard.LimitSwitch3Source == 1 &&
            _wtxObj.DataStandard.LimitSwitch3Mode == 1 && _wtxObj.DataStandard.LimitSwitch3ActivationLevelLowerBandLimit == 1 && _wtxObj.DataStandard.LimitSwitch3Hysteresis == 1 && _wtxObj.DataStandard.LimitSwitch4Source == 1 && _wtxObj.DataStandard.LimitSwitch4Mode == 1 &&
            _wtxObj.DataStandard.LimitSwitch4Level == 1 && _wtxObj.DataStandard.LimitSwitch4Hysteresis == 1 && _wtxObj.DataFiller.ResidualFlowTime == 1 && _wtxObj.DataFiller.TargetFillingWeight == 1 && _wtxObj.DataFiller.EmptyingMode == 1 &&
            _wtxObj.DataFiller.CoarseFlowCutOffPointSet == 1 && _wtxObj.DataFiller.FineFlowCutOffPointSet == 1 && _wtxObj.DataFiller.MinimumFineFlow == 1 && _wtxObj.DataFiller.OptimizationOfCutOffPoints == 1 && _wtxObj.DataFiller.MaximumDosingTime == 1 && _wtxObj.DataFiller.ValveControl == 1 &&
            _wtxObj.DataFiller.StartWithFineFlow == 1 && _wtxObj.DataFiller.CoarseLockoutTime == 1 && _wtxObj.DataFiller.FineLockoutTime == 1 && _wtxObj.DataFiller.TareMode == 1 && _wtxObj.DataFiller.UpperToleranceLimit == 1 && _wtxObj.DataFiller.LowerToleranceLimit == 1 &&
            _wtxObj.DataFiller.MinimumStartWeight == 1 && _wtxObj.DataFiller.TareDelay == 1 && _wtxObj.DataFiller.CoarseFlowMonitoringTime == 1 && _wtxObj.DataFiller.CoarseFlowMonitoring == 1 && _wtxObj.DataFiller.FineFlowMonitoring == 1 && _wtxObj.DataFiller.EmptyWeight == 1 &&
            _wtxObj.DataFiller.FineFlowMonitoringTime == 1 && _wtxObj.DataFiller.DelayTimeAfterFineFlow == 1 && _wtxObj.DataFiller.ActivationTimeAfterFineFlow == 1 && _wtxObj.DataFiller.SystematicDifference == 1 && _wtxObj.DataFiller.DownwardsDosing == 1
            )
                return true;

            else
                return false;
        }

    }
}