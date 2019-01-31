using System;

namespace Clamp.Linker.ViewEngines.Benben.Compiler.Lexer
{
    internal class AssignmentToken : Token
    {
        public override TokenType Type
        {
            get { return TokenType.Assignment; }
        }

        public override string Value
        {
            get { return "="; }
        }
    }
}

