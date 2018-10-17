using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ShanDian.AddIns.Print.Html.Compiler
{
    internal class CompilationContext
    {
        private readonly HtmlTemplateConfiguration _configuration;
        private readonly ParameterExpression _bindingContext;

        public CompilationContext(HtmlTemplateConfiguration configuration)
        {
            _configuration = configuration;
            _bindingContext = Expression.Variable(typeof(BindingContext), "context");
        }

        public virtual HtmlTemplateConfiguration Configuration
        {
            get { return _configuration; }
        }

        public virtual ParameterExpression BindingContext
        {
            get { return _bindingContext; }
        }
    }
}
