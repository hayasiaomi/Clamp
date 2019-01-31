using System;

namespace Clamp.Linker.ViewEngines.Benben
{
    public class BenbenException : Exception
    {
        public BenbenException(string message)
            : base(message)
        {
        }

        public BenbenException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

