using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.SDK.Framework.Injection
{
    [Serializable]
    public class WeakReferenceException : Exception
    {
        private const string ERROR_TEXT = "Unable to instantiate {0} - referenced object has been reclaimed";

        public WeakReferenceException(Type type)
            : base(String.Format(ERROR_TEXT, type.FullName))
        {
        }

        public WeakReferenceException(Type type, Exception innerException)
            : base(String.Format(ERROR_TEXT, type.FullName), innerException)
        {
        }
        protected WeakReferenceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
