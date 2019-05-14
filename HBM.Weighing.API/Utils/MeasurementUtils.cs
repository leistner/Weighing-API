using System;
using System.Collections.Generic;
using System.Text;

namespace HBM.Weighing.API.Utils
{
    static class MeasurementUtils
    {
        public static double DigitToDouble(int value, int decimals)
        {
            return (double)value / Math.Pow(10, decimals);
        }
    }
}
