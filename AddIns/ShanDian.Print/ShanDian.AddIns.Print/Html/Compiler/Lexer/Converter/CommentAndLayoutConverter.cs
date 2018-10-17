using System.Collections.Generic;
using System.Linq;
using ShanDian.AddIns.Print.Html.Compiler.Lexer;

namespace ShanDian.AddIns.Print.Html.Compiler
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
                return HtmlTemplateExpression.Comment(commentToken.Value);
            }
            else if (item is LayoutToken)
            {
                return HtmlTemplateExpression.Comment(null);
            }
            else
            {
                return item;
            }
        }
    }
}

