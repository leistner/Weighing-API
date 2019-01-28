using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HBM.Weighing.API
{
    public class DataEventArgs
    {
        private ushort[] _dataArray;
        private Dictionary<string, int> _dataDict;

        public DataEventArgs(ushort[] _dataArrayParam, Dictionary<string,int> _dataDictParam)
        {
            this._dataArray = _dataArrayParam;
            this._dataDict = _dataDictParam; 
        }

        public ushort[] Data
        {
            get
            {
                return this._dataArray;
            }
            set
            {
                this._dataArray = value;
            }
        }

        public Dictionary<string,int> DataDictionary
        {
            get
            {
                return this._dataDict;
            }
            set
            {
                this._dataDict = value;
            }
        }
    }
}

