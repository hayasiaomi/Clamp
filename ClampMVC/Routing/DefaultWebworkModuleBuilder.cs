namespace Clamp.Linker.Routing
{
    using ModelBinding;
    using Clamp.Linker.Extensions;
    using Clamp.Linker.Responses.Negotiation;
    using Clamp.Linker.ViewEngines;
    using Clamp.Linker.Validation;

    /// <summary>
    /// Default implementation for building a full configured <see cref="IController"/> instance.
    /// </summary>
    public class DefaultWebworkModuleBuilder : IWebworkModuleBuilder
    {
        private readonly IViewFactory viewFactory;
        private readonly IResponseFormatterFactory responseFormatterFactory;
        private readonly IModelBinderLocator modelBinderLocator;
        private readonly IModelValidatorLocator validatorLocator;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultWebworkModuleBuilder"/> class.
        /// </summary>
        /// <param name="viewFactory">The <see cref="IViewFactory"/> instance that should be assigned to the module.</param>
        /// <param name="responseFormatterFactory">An <see cref="IResponseFormatterFactory"/> instance that should be used to create a response formatter for the module.</param>
        /// <param name="modelBinderLocator">A <see cref="IModelBinderLocator"/> instance that should be assigned to the module.</param>
        /// <param name="validatorLocator">A <see cref="IModelValidatorLocator"/> instance that should be assigned to the module.</param>
        public DefaultWebworkModuleBuilder(IViewFactory viewFactory, IResponseFormatterFactory responseFormatterFactory, IModelBinderLocator modelBinderLocator, IModelValidatorLocator validatorLocator)
        {
            this.viewFactory = viewFactory;
            this.responseFormatterFactory = responseFormatterFactory;
            this.modelBinderLocator = modelBinderLocator;
            this.validatorLocator = validatorLocator;
        }

        /// <summary>
        /// Builds a fully configured <see cref="IController"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="IController"/> instance.</returns>
        public IController BuildModule(IController module, ClampWebContext context)
        {
            module.Context = context;
            module.Response = this.responseFormatterFactory.Create(context);
            module.ViewFactory = this.viewFactory;
            module.ModelBinderLocator = this.modelBinderLocator;
            module.ValidatorLocator = this.validatorLocator;

            return module;
        }
    }
}