using System;
using ShanDian.AddIns.Print.Html.Compiler;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html
{
    internal class SubExpressionExpression : HtmlTemplateExpression
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
            get { return (ExpressionType)HtmlTemplateExpressionType.SubExpression; }
        }
    }
}

