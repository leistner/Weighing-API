using Hbm.Weighing.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUIsimple
{
    /// <summary>
    /// Event argument for the selected input and output function. Used by the event ''.
    /// </summary>
    public class IOFunctionEventArgs : EventArgs
    {
        private OutputFunction _out1;
        private OutputFunction _out2;
        private OutputFunction _out3;
        private OutputFunction _out4;
        private InputFunction _in1;
        private InputFunction _in2;

        public IOFunctionEventArgs(OutputFunction Out1, OutputFunction Out2, OutputFunction Out3, OutputFunction Out4, InputFunction In1, InputFunction In2)
        {
            _out1 = Out1;
            _out2 = Out2;
            _out3 = Out3;
            _out4 = Out4;

            _in1 = In1;
            _in2 = In2;
        }
        public OutputFunction FunctionOutputIO1
        {
            get
            {
                return _out1;
            }
            set
            {
                _out1 = value;
            }
        }
        public OutputFunction FunctionOutputIO2
        {
            get
            {
                return _out2;
            }
            set
            {
                _out2 = value;
            }
        }
        public OutputFunction FunctionOutputIO3
        {
            get
            {
                return _out3;
            }
            set
            {
                _out3 = value;
            }
        }
        public OutputFunction FunctionOutputIO4
        {
            get
            {
                return _out4;
            }
            set
            {
                _out4 = value;
            }
        }
        public InputFunction FunctionInputIO1
        {
            get
            {
                return _in1;
            }
            set
            {
                _in1 = value;
            }
        }
        public InputFunction FunctionInputIO2
        {
            get
            {
                return _in2;
            }
            set
            {
                _in2 = value;
            }
        }

    }
}
