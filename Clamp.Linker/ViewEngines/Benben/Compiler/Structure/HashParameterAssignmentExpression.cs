using System;
using System.Linq.Expressions;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class HashParameterAssignmentExpression : HandlebarsExpression
    {
        public string Name { get; set; }

        public HashParameterAssignmentExpression(string name)
        {
            Name = name;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.HashParameterAssignmentExpression; }
        }

        public override Type Type
        {
            get { return typeof(object); }
        }
    }
}

