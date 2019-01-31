using System.Collections.Generic;
using System.Linq;
using Clamp.Linker.ViewEngines.Benben.Compiler.Lexer;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class CommentAndLayoutConverter : TokenConverter
    {
        public static IEnumerable<object> Convert(IEnumerable<object> sequence)
        {
            return new CommentAndLayoutConverter().ConvertTokens(sequence).ToList();
        }

        private CommentAndLayoutConverter()
        {
        }

        public override IEnumerable<object> ConvertTokens(IEnumerable<object> sequence)
        {
            return sequence.Select(Convert);
        }

        private static object Convert(object item)
        {
            var commentToken = item as CommentToken;
            if (commentToken != null)
            {
                return HandlebarsExpression.Comment(commentToken.Value);
            }
            else if (item is LayoutToken)
            {
                return HandlebarsExpression.Comment(null);
            }
            else
            {
                return item;
            }
        }
    }
}

