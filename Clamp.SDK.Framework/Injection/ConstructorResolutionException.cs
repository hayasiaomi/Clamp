using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.SDK.Framework.Injection
{
    [Serializable]
    public class ConstructorResolutionException : Exception
    {
        private const string ERROR_TEXT = "Unable to resolve constructor for {0} using provided Expression.";

        public ConstructorResolutionException(Type type)
            : base(String.Format(ERROR_TEXT, type.FullName))
        {
        }

        public ConstructorResolutionException(Type type, Exception innerException)
            : base(String.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }

        public ConstructorResolutionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ConstructorResolutionException(string message)
            : base(message)
        {
        }
        protected ConstructorResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
