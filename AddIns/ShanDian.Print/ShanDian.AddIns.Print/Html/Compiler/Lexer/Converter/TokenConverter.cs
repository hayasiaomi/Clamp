using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using ShanDian.AddIns.Print.Html.Compiler.Lexer;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal abstract class TokenConverter
    {
        public abstract IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

