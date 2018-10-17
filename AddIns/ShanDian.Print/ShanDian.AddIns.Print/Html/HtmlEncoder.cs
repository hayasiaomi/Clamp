using System;
using System.Globalization;
using System.Text;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlEncoder : ITextEncoder
    {
        public string Encode(string text)
        {
            //if (text == null)
            //    return String.Empty;

            //var sb = new StringBuilder(text.Length);

            //for (var i = 0; i < text.Length; i++)
            //{
            //    switch (text[i])
            //    {
            //        case '"':
            //            sb.Append("&quot;");
            //            break;
            //        case '&':
            //            sb.Append("&amp;");
            //            break;
            //        case '<':
            //            sb.Append("&lt;");
            //            break;
            //        case '>':
            //            sb.Append("&gt;");
            //            break;

            //        default:
            //                sb.Append(text[i]);
            //            break;
            //    }
            //}
            //return sb.ToString();

            return text;
        }
    }
}