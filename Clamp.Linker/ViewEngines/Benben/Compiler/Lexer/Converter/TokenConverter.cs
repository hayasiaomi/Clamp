using System;
using System.Linq;
using System.Linq.Expressions;
using System.Collections.Generic;
using Clamp.Linker.ViewEngines.Benben.Compiler.Lexer;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal abstract class TokenConverter
    {
        public abstract IEnumerable<object> ConvertTokens(IEnumerable<object> sequence);
    }
}

