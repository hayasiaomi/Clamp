﻿using System;
using System.Collections.Generic;
using System.Linq;
using ShanDian.AddIns.Print.Html.Compiler.Lexer;
using System.Linq.Expressions;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class HelperConverter : TokenConverter
    {
        private static readonly string[] builtInHelpers = new[] { "else", "each" };

        public static IEnumerable<object> Convert(IEnumerable<object> sequence, HtmlTemplateConfiguration configuration)
        {
            return new HelperConverter(configuration).ConvertTokens(sequence).ToList();
        }

        private readonly HtmlTemplateConfiguration _configuration;

        private HelperConverter(HtmlTemplateConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            var enumerator = sequence.GetEnumerator();

            while (enumerator.MoveNext())
            {
                var item = enumerator.Current;

                if (item is StartExpressionToken)
                {
                    yield return item;

                    item = GetNext(enumerator);

                    if (item is Expression)
                    {
                        yield return item;
                        continue;
                    }

                    var word = item as WordExpressionToken;

                    if (word != null && IsRegisteredHelperName(word.Value))
                    {
                        yield return HtmlTemplateExpression.Helper(word.Value);
                    }
                    else
                    {
                        yield return item;
                    }
                }
                else
                {
                    yield return item;
                }
            }
        }


        private bool IsRegisteredHelperName(string name)
        {
            name = name.Replace("#", "");

            return _configuration.Helpers.ContainsKey(name) || _configuration.BlockHelpers.ContainsKey(name) || builtInHelpers.Contains(name);
        }

        private static object GetNext(IEnumerator<object> enumerator)
        {
            enumerator.MoveNext();

            return enumerator.Current;
        }
    }
}

