using System;
using System.IO;

namespace ShanDian.AddIns.Print.Html.Compiler.Lexer
{
    internal abstract class Parser
    {
        public abstract Token Parse(TextReader reader);
    }
}

