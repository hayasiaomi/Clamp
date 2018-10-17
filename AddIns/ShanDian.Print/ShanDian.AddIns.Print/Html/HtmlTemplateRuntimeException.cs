using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlTemplateRuntimeException : HtmlTemplateException
    {
        public HtmlTemplateRuntimeException(string message)
            : base(message)
        {
        }

        public HtmlTemplateRuntimeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
