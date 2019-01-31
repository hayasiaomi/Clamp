﻿using System.IO;

namespace Clamp.Linker.ViewEngines.Benben.Compiler.Lexer
{
    internal class PartialParser : Parser
    {
        public override Token Parse(TextReader reader)
        {
            PartialToken token = null;
            if ((char)reader.Peek() == '>')
            {
                token = Token.Partial();
            }
            return token;
        }
    }
}

