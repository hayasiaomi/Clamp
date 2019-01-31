using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class HashParametersExpression : HandlebarsExpression
    {
        public Dictionary<string, Expression> Parameters { get; set; }

        public HashParametersExpression(Dictionary<string, Expression> parameters)
        {
            Parameters = parameters;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.HashParametersExpression; }
        }

        public override Type Type
        {
            get { return typeof(object); }
        }
    }
}

