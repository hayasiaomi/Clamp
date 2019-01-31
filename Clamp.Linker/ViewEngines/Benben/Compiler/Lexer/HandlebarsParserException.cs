using System;

namespace Clamp.Linker.ViewEngines.Benben
{
    public class HandlebarsParserException : BenbenException
    {
        public HandlebarsParserException(string message)
            : base(message)
        {
        }

        public HandlebarsParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

