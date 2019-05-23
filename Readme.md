# HBM Weighing API


[![Build status](https://hbmdevelopment.visualstudio.com/HBM%20Weighing/_apis/build/status/HBM%20Weighing%20API%20CI)](https://hbmdevelopment.visualstudio.com/HBM%20Weighing/_build/latest?definitionId=47)

Connect your own application to weighing terminals WTX110-A and WTX120 from HBM.


Contains API and 3 templates (Console application, Simple GUI, PLC view). 



**File status**

| File                               | Status               |
| ---------------------------------- | -------------------- |
| BaseWTDevice.cs                    | :white_check_mark:   |
| DataEventArgs.cs                   | :large_blue_circle:  |
| Enums.cs                           | :large_blue_circle:  |
| INetConnection.cs                  | :large_blue_circle:  |
| LogEvent.cs                        | :large_blue_circle:  |
| ProcessDataReceivedEventArgs.cs    | :large_blue_circle:  |
| WTXJet.cs                          | :large_blue_circle:  |
| WTXModbus.cs                       | :large_blue_circle:  | 
| ModbusCommand.cs                   | :large_blue_circle:  |
| ModbusCommands.cs                  | :large_blue_circle:  |
| ModbusTCPConnection.cs             | :red_circle:         |
| JetBusCommand.cs                   | :large_blue_circle:  |
| JetBusCommands.cs                  | :large_blue_circle:  |
| JetBusConnection.cs                | :red_circle:         |
| JetBusException.cs                 | :red_circle:         |
| MeasurementUtils.cs                | :large_blue_circle:  |
| AssemblyInfo.cs                    | :white_check_mark:   |
| DataFillerExtendedJet.cs           | :red_circle:         |
| DataFillerJet.cs                   | :red_circle:         |
| DataFillerModbus.cs                | :red_circle:         |
| DataStandardJet.cs                 | :red_circle:         |
| DataStandardModbus.cs              | :red_circle:         |
| IDataFiller.cs                     | :red_circle:         |
| IDataFillerExtended.cs             | :red_circle:         |
| IDataStandard.cs                   | :red_circle:         |
| PrintableWeightType.cs             | :large_blue_circle:  |
| ProcessDataJet.cs                  | :white_check_mark:   |
| ProcessDataModbus.cs               | :white_check_mark:   |
| ProcessDataModbus.cs               | :white_check_mark:   |
| 

:white_check_mark: Well done!   
:large_blue_circle: Pay some attention to coding style 
:red_circle: Review missing        


## License



Copyright (c) 2019 HBM. See the [LICENSE](LICENSE) file for license rights and
limitations (MIT).