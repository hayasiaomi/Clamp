using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class BoolishExpression : HtmlTemplateExpression
    {
        private readonly Expression _condition;

        public BoolishExpression(Expression condition)
        {
            _condition = condition;
        }

        public new Expression Condition
        {
            get { return _condition; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.BoolishExpression; }
        }

        public override Type Type
        {
            get { return typeof(bool); }
        }
    }
}

