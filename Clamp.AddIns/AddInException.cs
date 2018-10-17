using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.AddIns
{
    [Serializable]
    public class AddInException : Exception
    {
        public AddInException() : base()
        {
        }

        public AddInException(string message) : base(message)
        {

        }

        public AddInException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected AddInException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
