using System;
using System.Diagnostics;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    [DebuggerDisplay("undefined")]
    internal class UndefinedBindingResult
    {
	    public readonly string Value;
	    private readonly BenbenConfiguration _configuration;

	    public UndefinedBindingResult(string value, BenbenConfiguration configuration)
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

