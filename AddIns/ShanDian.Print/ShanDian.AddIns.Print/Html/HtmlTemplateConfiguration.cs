﻿using System;
using System.Collections.Generic;
using System.IO;
using ShanDian.AddIns.Print.Html.Compiler.Resolvers;

namespace ShanDian.AddIns.Print.Html
{
    public class HtmlTemplateConfiguration
    {
        public IDictionary<string, HandlebarsHelper> Helpers { get; private set; }

        public IDictionary<string, HandlebarsBlockHelper> BlockHelpers { get; private set; }

        public IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; private set; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ITextEncoder TextEncoder { get; set; }

        public ViewEngineFileSystem FileSystem { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }
	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

	    public HtmlTemplateConfiguration()
        {
            this.Helpers = new Dictionary<string, HandlebarsHelper>(StringComparer.OrdinalIgnoreCase);
            this.BlockHelpers = new Dictionary<string, HandlebarsBlockHelper>(StringComparer.OrdinalIgnoreCase);
            this.RegisteredTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            this.TextEncoder = new HtmlEncoder();
	        this.ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

