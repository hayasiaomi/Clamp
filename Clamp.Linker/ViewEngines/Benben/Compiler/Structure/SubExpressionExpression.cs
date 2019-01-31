using System;
using Clamp.Linker.ViewEngines.Benben.Compiler;
using System.Linq.Expressions;

namespace Clamp.Linker.ViewEngines.Benben
{
    internal class SubExpressionExpression : HandlebarsExpression
    {
        private readonly Expression _expression;

        public SubExpressionExpression(Expression expression)
        {
            _expression = expression;
        }

        public override Type Type
        {
            get { return typeof(object); }
        }

        public Expression Expression
        {
            get { return _expression; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.SubExpression; }
        }
    }
}

