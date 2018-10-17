using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class CommentExpression : HtmlTemplateExpression
    {
        public string Value { get; private set; }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType) HtmlTemplateExpressionType.CommentExpression; }
        }

        public override Type Type
        {
            get { return typeof (void); }
        }

        public CommentExpression(string value)
        {
            Value = value;
        }
    }
}