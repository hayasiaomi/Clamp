using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.Injection
{
    [Serializable]
    public class ResolutionException : Exception
    {
        private const string ERROR_TEXT = "Unable to resolve type: {0}";

        public ResolutionException(Type type)
            : base(String.Format(ERROR_TEXT, type.FullName))
        {
        }

        public ResolutionException(Type type, Exception innerException)
            : base(String.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }
        protected ResolutionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
