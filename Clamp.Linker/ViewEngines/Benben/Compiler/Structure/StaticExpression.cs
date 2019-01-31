﻿using System;
using System.Linq.Expressions;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class StaticExpression : HandlebarsExpression
    {
        private readonly string _value;

        public StaticExpression(string value)
        {
            _value = value;
        }

        public override ExpressionType NodeType
        {
            get { return (ExpressionType)HandlebarsExpressionType.StaticExpression; }
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

