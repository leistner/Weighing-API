# HBM Weighing API


[![Build status](https://hbmdevelopment.visualstudio.com/HBM%20Weighing/_apis/build/status/HBM%20Weighing%20API%20CI)](https://hbmdevelopment.visualstudio.com/HBM%20Weighing/_build/latest?definitionId=47)

Connect your own application to weighing terminals WTX110-A and WTX120 from HBM.


Contains API and 3 templates (Console application, Simple GUI, PLC view). 



**File status**

| File                               | Status                 |
| ---------------------------------- | ---------------------- |
| BaseWTDevice.cs                    | :heavy_check_mark:     |
| DataEventArgs.cs                   | :heavy_check_mark:     |
| Enums.cs                           | :heavy_check_mark:     |
| INetConnection.cs                  | :heavy_check_mark:     |
| LogEventArgs.cs                    | :heavy_check_mark:     |
| ProcessDataReceivedEventArgs.cs    | :heavy_check_mark:     | 
| WTXJet.cs                          | :heavy_check_mark:     |
| WTXModbus.cs                       | :heavy_check_mark:     | 
| ModbusCommand.cs                   | :heavy_check_mark:     |
| ModbusCommands.cs                  | :heavy_check_mark:     |
| ModbusTCPConnection.cs             | :heavy_check_mark:     |
| JetBusCommand.cs                   | :heavy_check_mark:     |
| JetBusCommands.cs                  | :heavy_check_mark:     |
| JetBusConnection.cs                | :heavy_check_mark:     |
| JetBusException.cs                 | :heavy_check_mark:     |
| MeasurementUtils.cs                | :large_orange_diamond: |
| AssemblyInfo.cs                    | :heavy_check_mark:     |
| DataFillerExtendedJet.cs           | :red_circle:           |
| DataFillerJet.cs                   | :red_circle:           |
| DataFillerModbus.cs                | :red_circle:           |
| DataStandardJet.cs                 | :heavy_check_mark:     |
| DataStandardModbus.cs              | :heavy_check_mark:     |
| IDataFiller.cs                     | :red_circle:           |
| IDataFillerExtended.cs             | :red_circle:           |
| IDataStandard.cs                   | :heavy_check_mark:     |
| PrintableWeightType.cs             | :large_orange_diamond: |
| ProcessDataJet.cs                  | :heavy_check_mark:     |
| ProcessDataModbus.cs               | :heavy_check_mark:     |
| ProcessDataModbus.cs               | :heavy_check_mark:     |
| WeightMemory.cs                    | :heavy_check_mark:     |


:heavy_check_mark: Well done!   
:large_orange_diamond: Pay some attention to coding style
:red_circle: Review missing        


## License



Copyright (c) 2019 HBM. See the [LICENSE](LICENSE) file for license rights and
limitations (MIT).