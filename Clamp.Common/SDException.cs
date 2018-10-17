using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.Common
{
    [Serializable()]
    public class SDException : Exception
    {
        public SDException() : base()
        {
        }

        public SDException(string message) : base(message)
        {
        }

        public SDException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SDException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
