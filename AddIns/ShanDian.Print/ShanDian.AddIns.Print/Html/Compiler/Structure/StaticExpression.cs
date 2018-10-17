using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class StaticExpression : HtmlTemplateExpression
    {
        private readonly string _value;

        public StaticExpression(string value)
        {
            _value = value;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.StaticExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public string Value
        {
            get { return _value; }
        }
    }
}

