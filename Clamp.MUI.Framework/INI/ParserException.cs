using System;

namespace Clamp.MUI.Framework.INI
{

    [Serializable]
    public sealed class ParserException : Exception
    {
        internal ParserException(string message, int line) : base(string.Format("Line {0}: {1}", line, message))
        {
            Line = line;
        }

        public int Line { get; private set; }
    }
}