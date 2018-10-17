using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace ShanDian.SDK.Framework
{
    [Serializable()]
    public class ObjectNotFoundException : SDKException
    {
        public ObjectNotFoundException() : base()
        {
        }

        public ObjectNotFoundException(Type serviceType) : base("Required object not found: " + serviceType.FullName)
        {
        }

        public ObjectNotFoundException(string message) : base(message)
        {
        }

        public ObjectNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ObjectNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
