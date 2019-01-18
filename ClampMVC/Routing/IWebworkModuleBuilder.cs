namespace ClampMVC.Routing
{
    /// <summary>
    /// Defines the functionality to build a fully configured NancyModule instance.
    /// </summary>
    public interface IWebworkModuleBuilder
    {
        /// <summary>
        /// Builds a fully configured <see cref="IController"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="IController"/> instance.</returns>
        IController BuildModule(IController module, WebworkContext context);
    }
}