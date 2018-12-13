using Newtonsoft.Json.Linq;
using System;


namespace HBM.Weighing.API.WTX.Jet
{
    public class JetBusException : Exception
    { 
        private int errorCode;
        private string message;

        public JetBusException(JToken token)
        {
            errorCode = token["error"]["code"].ToObject<int>();
            message = token["error"]["data"].ToString();
        }


        public int ErrorCode
        {
            get
            {
                return errorCode;
            }
        }


        public override string Message
        {
            get
            {
                return message + " [ 0x" + errorCode.ToString("X") + " ]";
            }
        }
    }
}
