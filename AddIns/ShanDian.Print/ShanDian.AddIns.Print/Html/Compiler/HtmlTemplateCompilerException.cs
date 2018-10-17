using System;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlTemplateCompilerException : HtmlTemplateException
    {
        public HtmlTemplateCompilerException(string message)
            : base(message)
        {
        }

        public HtmlTemplateCompilerException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

