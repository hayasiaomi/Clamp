using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp
{
    [Serializable]
    internal class MissingDependencyException : Exception
    {
        public MissingDependencyException(SerializationInfo inf, StreamingContext ctx) : base(inf, ctx)
        {
        }

        public MissingDependencyException(string message) : base(message)
        {
        }
    }
}
