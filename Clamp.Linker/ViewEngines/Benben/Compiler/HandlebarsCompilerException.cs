using System;

namespace Clamp.Linker.ViewEngines.Benben
{
    public class HandlebarsCompilerException : BenbenException
    {
        public HandlebarsCompilerException(string message)
            : base(message)
        {
        }

        public HandlebarsCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

