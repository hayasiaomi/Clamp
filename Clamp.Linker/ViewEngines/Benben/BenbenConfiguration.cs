namespace Clamp.Linker.ViewEngines.Benben
{
    using Clamp.Linker.ViewEngines.Benben.Compiler.Resolvers;
    using System;
    using System.Collections.Generic;
    using System.IO;

    public class BenbenConfiguration
    {
        public IDictionary<string, BenbenHelper> Helpers { get; private set; }

        public IDictionary<string, BenbenBlockHelper> BlockHelpers { get; private set; }

        public IDictionary<string, Action<TextWriter, object>> RegisteredTemplates { get; private set; }

        public IExpressionNameResolver ExpressionNameResolver { get; set; }

        public ITextEncoder TextEncoder { get; set; }

        public ViewEngineFileSystem FileSystem { get; set; }

	    public string UnresolvedBindingFormatter { get; set; }

	    public bool ThrowOnUnresolvedBindingExpression { get; set; }

        /// <summary>
        /// The resolver used for unregistered partials. Defaults
        /// to the <see cref="FileSystemPartialTemplateResolver"/>.
        /// </summary>
        public IPartialTemplateResolver PartialTemplateResolver { get; set; }

        /// <summary>
        /// The handler called when a partial template cannot be found.
        /// </summary>
        public IMissingPartialTemplateHandler MissingPartialTemplateHandler { get; set; }

        public BenbenConfiguration()
        {
            this.Helpers = new Dictionary<string, BenbenHelper>(StringComparer.OrdinalIgnoreCase);
            this.BlockHelpers = new Dictionary<string, BenbenBlockHelper>(StringComparer.OrdinalIgnoreCase);
            this.PartialTemplateResolver = new FileSystemPartialTemplateResolver();
            this.RegisteredTemplates = new Dictionary<string, Action<TextWriter, object>>(StringComparer.OrdinalIgnoreCase);
            this.TextEncoder = new HtmlEncoder();
	        this.ThrowOnUnresolvedBindingExpression = false;
        }
    }
}

