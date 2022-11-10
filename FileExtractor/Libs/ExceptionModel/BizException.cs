using System;
using System.Runtime.Serialization;

namespace Common
{
    public class BizException : Exception
    {
        public BizException(string msg) : base(msg)
        {

        }
        public BizException(string msg, Exception innerExcepion) : base(msg, innerExcepion)
        {

        }
    }
}