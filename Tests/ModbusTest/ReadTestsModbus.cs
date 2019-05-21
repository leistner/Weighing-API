
using HBM.Weighing.API;
using HBM.Weighing.API.WTX;
using HBM.Weighing.API.WTX.Modbus;

using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HBM.Weighing.API.WTX.Modbus
{
    [TestFixture]
    public class ReadTestsModbus
    {
        private TestModbusTCPConnection testConnection;
        private WTXModbus _wtxDevice;

        private bool connectCallbackCalled;
        private bool connectCompleted;

        private bool disconnectCallbackCalled;
        private bool disconnectCompleted;

        // Test case source for reading values from the WTX120 device. 
        public static IEnumerable ReadTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.ReadFail).Returns(0);
                yield return new TestCaseData(Behavior.ReadSuccess).Returns(16448);
            }
        }

        // Test case source for checking the transition of the handshake bit. 
        public static IEnumerable HandshakeTestCases
        {
            get
            {
               // yield return new TestCaseData(Behavior.HandshakeFail).Returns(1);
                yield return new TestCaseData(Behavior.HandshakeSuccess).Returns(false);
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

        // Test case source for checking the values of the application mode: 

        public static IEnumerable ApplicationModeTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.InStandardMode).Returns(0);
                yield return new TestCaseData(Behavior.InFillerMode).Returns(1);
            }
        }

        // Test case source for checking the values of the application mode: 

        public static IEnumerable LogEventTestCases
        {
            get
            {
                yield return new TestCaseData(Behavior.LogEvent_Fail).Returns(false);
                yield return new TestCaseData(Behavior.LogEvent_Success).Returns(true);
            }
        }

        private ushort[] _data;
        

        [SetUp]
        public void Setup()
        {
            this.connectCallbackCalled = true;
            this.connectCompleted = true;
        }

        ushort _testValue = 0;
 
        // Test for reading: 
        [Test, TestCaseSource(typeof(ReadTestsModbus), "ReadTestCases")]
        public async Task<ushort> ReadTestModbus(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxDevice = new WTXModbus(testConnection, 200, UpdateReadTest);

            _wtxDevice.Connect(this.OnConnect, 100);
          
            await Task.Run(async () =>
            {
                ushort[] _data = await testConnection.ReadAsync();
                _wtxDevice.OnData(_data);
            });

            _testValue = (ushort)_wtxDevice.ProcessData.NetValue;
            return _testValue;
        }

        private void UpdateReadTest(object sender, ProcessDataReceivedEventArgs e)
        {
            _testValue = (ushort) e.ProcessData.NetValue;
        }

        /*
        // Test for checking the handshake bit 
        [Test, TestCaseSource(typeof(ReadTestsModbus), "HandshakeTestCases")]
        public bool testHandshake(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxDevice = new WTXModbus(testConnection, 200,UpdateTestHandshake);

            _wtxDevice.Connect(this.OnConnect, 100);

            _wtxDevice.WriteSync(0, 0x1);

            return _wtxDevice.ProcessData.Handshake;
        }
        */

        private void UpdateTestHandshake(object sender, ProcessDataReceivedEventArgs e)
        {
        }

        [Test, TestCaseSource(typeof(ReadTestsModbus), "MeasureZeroTestCases")]
        public bool MeasureZeroTest(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxDevice = new WTXModbus(testConnection, 200,UpdateMeasureZeroTest);

            _wtxDevice.Connect(this.OnConnect, 100);

            _wtxDevice.AdjustZeroSignal();

            //check if : write reg 48, 0x7FFFFFFF and if Net and gross value are zero. 

            if ((testConnection.getArrElement1 == (0x7FFFFFFF & 0xffff0000) >> 16) &&
                (testConnection.getArrElement2 == (0x7FFFFFFF & 0x0000ffff)) &&
                _wtxDevice.ProcessData.NetValue == 0 && _wtxDevice.ProcessData.GrossValue == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void UpdateMeasureZeroTest(object sender, ProcessDataReceivedEventArgs e)
        {
        }
 
        [Test, TestCaseSource(typeof(ReadTestsModbus), "ApplicationModeTestCases")]
        public int ApplicationModeTest(Behavior behavior)
        {
            TestModbusTCPConnection testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");

            WTXModbus _wtxDevice = new WTXModbus(testConnection, 200,UpdateApplicationModeTest);

            _wtxDevice.Connect(this.OnConnect, 100);

            testConnection.Write(Convert.ToString(0), DataType.U08, 0);

            testConnection.ReadSingle(0);

            return testConnection.getData[5] & 0x3 >> 1;

            //return _wtxDevice.ApplicationMode;
        }

        private void UpdateApplicationModeTest(object sender, ProcessDataReceivedEventArgs e)
        {
        }
        
        [Test, TestCaseSource(typeof(ReadTestsModbus), "LogEventTestCases")]
        public async Task<bool> LogEventGetTest(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxDevice = new WTXModbus(testConnection, 200,UpdateLogEventGetTest);

            _wtxDevice.Connect(this.OnConnect, 100);
            testConnection.IsConnected = true;

            _data = await testConnection.ReadAsync();
            
            if (testConnection._logObj.Args.Equals("Read successful: Registers have been read"))
                return true;

            else
                if (testConnection._logObj.Args.Equals("Read failed : Registers have not been read"))
                return false;

            else
                return false; 
            //return _wtxDevice.ApplicationMode;
        }
        
        private void UpdateLogEventGetTest(object sender, ProcessDataReceivedEventArgs e)
        {
        }
        
        [Test, TestCaseSource(typeof(ReadTestsModbus), "LogEventTestCases")]
        public async Task<bool> LogEventSetTest(Behavior behavior)
        {
            testConnection = new TestModbusTCPConnection(behavior, "172.19.103.8");
            _wtxDevice = new WTXModbus(testConnection, 200, UpdateLogEventSetTest);

            _wtxDevice.Connect(this.OnConnect, 100);
            testConnection.IsConnected = true;

            _data = await testConnection.ReadAsync();

            if (testConnection._logObj.Args.Equals("Read successful: Registers have been read"))
                return true;

            else
                if (testConnection._logObj.Args.Equals("Read failed : Registers have not been read"))
                return false;

            else
                return false;
            //return _wtxDevice.ApplicationMode;
        }
        
        private void UpdateLogEventSetTest(object sender, ProcessDataReceivedEventArgs e)
        {
        }


        private void OnConnect(bool obj)
        {
            throw new NotImplementedException();
        }

    }
}
