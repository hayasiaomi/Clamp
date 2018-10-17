using System;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class PathExpression : HtmlTemplateExpression
    {
        private readonly string _path;

        public PathExpression(string path)
        {
            _path = path;
        }

        public new string Path
        {
            get { return _path; }
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.PathExpression; }
        }

        public override Type Type
        {
            get { return typeof(object); }
        }
    }
}

