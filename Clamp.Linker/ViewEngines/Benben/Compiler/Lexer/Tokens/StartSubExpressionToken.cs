using System;

namespace Clamp.Linker.ViewEngines.Benben.Compiler.Lexer
{
    internal class StartSubExpressionToken : ExpressionScopeToken
    {
        public StartSubExpressionToken()
        {
        }

        public override string Value
        {
            get { return "("; }
        }

        public override TokenType Type
        {
            get { return TokenType.StartSubExpression; }
        }

        public override string ToString()
        {
            return this.Value;
        }
    }
}

