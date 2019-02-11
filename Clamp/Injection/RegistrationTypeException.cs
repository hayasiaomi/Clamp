using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.Injection
{
    [Serializable]
    public class RegistrationTypeException : Exception
    {
        private const string REGISTER_ERROR_TEXT = "Cannot register type {0} - abstract classes or interfaces are not valid implementation types for {1}.";

        public RegistrationTypeException(Type type, string factory)
            : base(String.Format(REGISTER_ERROR_TEXT, type.FullName, factory))
        {
        }

        public RegistrationTypeException(Type type, string factory, Exception innerException)
            : base(String.Format(REGISTER_ERROR_TEXT, type.FullName, factory), innerException)
        {
        }
        protected RegistrationTypeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
