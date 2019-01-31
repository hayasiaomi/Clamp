using System;
using System.IO;

namespace Clamp.Linker.ViewEngines.Benben.Compiler.Lexer
{
    internal abstract class Parser
    {
        public abstract Token Parse(TextReader reader);
    }
}

