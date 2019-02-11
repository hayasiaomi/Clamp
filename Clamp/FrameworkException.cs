using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp
{
    [Serializable]
    public class FrameworkException : Exception
    {
        public FrameworkException() : base()
        {
        }

        public FrameworkException(string message) : base(message)
        {

        }

        public FrameworkException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected FrameworkException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
