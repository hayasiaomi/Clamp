﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace Clamp.Injection
{
    public class RegistrationException : Exception
    {
        private const string CONVERT_ERROR_TEXT = "Cannot convert current registration of {0} to {1}";
        private const string GENERIC_CONSTRAINT_ERROR_TEXT = "Type {1} is not valid for a registration of type {0}";

        public RegistrationException(Type type, string method)
            : base(String.Format(CONVERT_ERROR_TEXT, type.FullName, method))
        {
        }

        public RegistrationException(Type type, string method, Exception innerException)
            : base(String.Format(CONVERT_ERROR_TEXT, type.FullName, method), innerException)
        {
        }

        public RegistrationException(Type registerType, Type implementationType)
            : base(String.Format(GENERIC_CONSTRAINT_ERROR_TEXT, registerType.FullName, implementationType.FullName))
        {
        }

        public RegistrationException(Type registerType, Type implementationType, Exception innerException)
            : base(String.Format(GENERIC_CONSTRAINT_ERROR_TEXT, registerType.FullName, implementationType.FullName), innerException)
        {
        }
        protected RegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
