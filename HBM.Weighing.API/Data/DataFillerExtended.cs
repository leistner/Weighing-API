using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API.Data
{
    public class DataFillerExtended : DataFiller, IDataFillerExtended
    {

        private ushort[] _data;

        private int _attributeExample;

        public DataFillerExtended()
        {
            _attributeExample = 0;
        }

        public void UpdateFillerExtendedData(ushort[] _dataParam)
        {
            _data = _dataParam;

            _attributeExample = _data[0];

        }
    }
}
