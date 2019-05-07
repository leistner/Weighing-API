using System;
using System.Collections.Generic;
using System.Text;

namespace HBM.Weighing.API.WTX.Jet
{
    public class JetBusCommand
    {
        public JetBusCommand(int DataType, string Path, int BitIndex, int BitLength);

        int DataType { get; }

        string Path { get; }

        int BitIndex { get; }

        int BitLength { get; }

    }
}
