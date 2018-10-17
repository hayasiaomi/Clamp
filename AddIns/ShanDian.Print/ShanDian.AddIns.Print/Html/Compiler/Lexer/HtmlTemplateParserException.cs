using System;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlTemplateParserException : HtmlTemplateException
    {
        public HtmlTemplateParserException(string message)
            : base(message)
        {
        }

        public HtmlTemplateParserException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

