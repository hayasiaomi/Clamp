using System;
using System.Linq;
using Clamp.Linker.ViewEngines.Benben.Compiler;
using System.Linq.Expressions;
using System.Reflection;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class BoolishConverter : HandlebarsExpressionVisitor
    {
        public static Expression Convert(Expression expr, CompilationContext context)
        {
            return new BoolishConverter(context).Visit(expr);
        }

        private BoolishConverter(CompilationContext context)
            : base(context)
        {
        }

        protected override Expression VisitBoolishExpression(BoolishExpression bex)
        {
            return Expression.Call(
#if netstandard
                new Func<object, bool>(BenbensUtils.IsTruthyOrNonEmpty).GetMethodInfo(),
#else
                new Func<object, bool>(BenbensUtils.IsTruthyOrNonEmpty).Method,
#endif
                Visit(bex.Condition));
        }
    }
}

