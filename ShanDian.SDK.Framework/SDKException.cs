using ShanDian.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ShanDian.SDK.Framework
{
    [Serializable()]
    public class SDKException : SDException
    {
        public SDKException() : base()
        {

        }

        public SDKException(string message) : base(message)
        {
        }

        public SDKException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SDKException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
