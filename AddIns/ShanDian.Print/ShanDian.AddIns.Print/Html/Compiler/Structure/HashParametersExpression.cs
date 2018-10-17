using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class HashParametersExpression : HtmlTemplateExpression
    {
        public Dictionary<string, object> Parameters { get; set; }

        public HashParametersExpression(Dictionary<string, object> parameters)
        {
            Parameters = parameters;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HtmlTemplateExpressionType.HashParametersExpression; }
        }

        public override Type Type
        {
            get { return typeof(object); }
        }
    }
}

