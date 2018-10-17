using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class HelperExpression : HtmlTemplateExpression
    {
        private readonly IEnumerable<Expression> _arguments;
        private readonly string _helperName;

        public HelperExpression(string helperName, IEnumerable<Expression> arguments)
            : this(helperName)
        {
            _arguments = arguments;
        }

        public HelperExpression(string helperName)
        {
            this._helperName = helperName;
            this._arguments = Enumerable.Empty<Expression>();
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.HelperExpression; }
        }

        public override Type Type
        {
            get { return typeof(void); }
        }

        public string HelperName
        {
            get { return _helperName; }
        }

        public IEnumerable<Expression> Arguments
        {
            get { return _arguments; }
        }
    }
}

