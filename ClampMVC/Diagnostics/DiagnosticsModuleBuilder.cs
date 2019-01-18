namespace ClampMVC.Diagnostics
{
    using System.Collections.Generic;
    using ModelBinding;
    using ClampMVC.Responses;
    using ClampMVC.Routing;

    internal class DiagnosticsModuleBuilder : IWebworkModuleBuilder
    {
        private readonly IRootPathProvider rootPathProvider;

        private readonly IEnumerable<ISerializer> serializers;
        private readonly IModelBinderLocator modelBinderLocator;

        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IModelBinderLocator modelBinderLocator)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializers = new[] { new DefaultJsonSerializer { RetainCasing = false } };
            this.modelBinderLocator = modelBinderLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="IController"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="IController"/> instance.</returns>
        public IController BuildModule(IController module, WebworkContext context)
        {
            // Currently we don't connect view location, binders etc.
            module.Context = context;
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, serializers);
            module.ModelBinderLocator = this.modelBinderLocator;

            module.After = new AfterPipeline();
            module.Before = new BeforePipeline();
            module.OnError = new ErrorPipeline();

            return module;
        }
    }
}