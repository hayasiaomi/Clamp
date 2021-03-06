﻿using System;

namespace Clamp.Linker.ViewEngines.Benben.Compiler.Lexer
{
    internal class EndSubExpressionToken : ExpressionScopeToken
    {
        public EndSubExpressionToken()
        {
        }

        public override string Value
        {
            get { return ")"; }
        }

        public override TokenType Type
        {
            get { return TokenType.EndSubExpression; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

