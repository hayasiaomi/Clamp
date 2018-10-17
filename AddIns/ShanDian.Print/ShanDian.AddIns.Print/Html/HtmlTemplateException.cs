using System;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlTemplateException : Exception
    {
        public HtmlTemplateException(string message)
            : base(message)
        {
        }

        public HtmlTemplateException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}

