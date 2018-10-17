using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class DeferredSectionExpression : HtmlTemplateExpression
    {
        public DeferredSectionExpression(
            PathExpression path,
            BlockExpression body,
            BlockExpression inversion)
        {
            Path = path;
            Body = body;
            Inversion = inversion;
        }

        public BlockExpression Body { get; private set; }

        public BlockExpression Inversion { get; private set; }

        public new PathExpression Path { get; private set; }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.DeferredSection; }
        }
    }
}

