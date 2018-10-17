﻿using System;
using System.Collections.Generic;
using System.Linq;
using ShanDian.AddIns.Print.Html.Compiler.Lexer;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class ExpressionScopeConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new ExpressionScopeConverter().ConvertTokens(sequence).ToList();
        }

        private ExpressionScopeConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;
                var startExpression = item as StartExpressionToken;

                if (startExpression == null)
                {
                    yield return item;
                    continue;
                }

                var possibleBody = GetNext(enumerator);
                if (!(possibleBody is Expression))
                {
                    throw new HtmlTemplateCompilerException(String.Format("Token '{0}' could not be converted to an expression", possibleBody));
                }

                var endExpression = GetNext(enumerator) as EndExpressionToken;
                if (endExpression == null)
                {
                    throw new HtmlTemplateCompilerException("Handlebars statement was not reduced to a single expression");
                }

                if (endExpression.IsEscaped != startExpression.IsEscaped)
                {
                    throw new HtmlTemplateCompilerException("Starting and ending handlebars do not match");
                }

                yield return HtmlTemplateExpression.Statement((Expression)possibleBody, startExpression.IsEscaped, startExpression.TrimPreceedingWhitespace, endExpression.TrimTrailingWhitespace);
            }
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();
            return enumerator.Current;
        }
    }
}