using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Clamp.Linker.ViewEngines.Benben.Compiler
{
    internal class CompilationContext
    {
        private readonly BenbenConfiguration _configuration;
        private readonly ParameterExpression _bindingContext;

        public CompilationContext(BenbenConfiguration configuration)
        {
            _configuration = configuration;
            _bindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public virtual BenbenConfiguration Configuration
        {
            get { return _configuration; }
        }

        public virtual ParameterExpression BindingContext
        {
            get { return _bindingContext; }
        }
    }
}
