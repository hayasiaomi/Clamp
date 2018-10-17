using System;
using System.Diagnostics;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    [DebuggerDisplay("undefined")]
    internal class UndefinedBindingResult
    {
	    public readonly string Value;
	    private readonly HtmlTemplateConfiguration _configuration;

	    public UndefinedBindingResult(string value, HtmlTemplateConfiguration configuration)
	    {
		    Value = value;
		    _configuration = configuration;
	    }

        public override string ToString()
        {
	        var formatter = _configuration.UnresolvedBindingFormatter ?? string.Empty;
	        return string.Format( formatter, Value );
        }
    }
}

