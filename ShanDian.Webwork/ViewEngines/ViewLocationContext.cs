namespace ShanDian.Webwork.ViewEngines
{
    /// <summary>
    /// The context for which a view is being located.
    /// </summary>
    public class ViewLocationContext
    {
        /// <summary>
        /// The module path of the <see cref="IController"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the module path.</value>
        public string ModulePath { get; set; }

        /// <summary>
        /// The name of the <see cref="IController"/> that is locating a view.
        /// </summary>
        /// <value>A <see cref="string"/> containing the name of the module.</value>
        public string ModuleName { get; set; }

        public string ModuleLocation { get; set; }

        /// <summary>
        /// The request/response context
        /// </summary>
        public WebworkContext Context { get; set; }
    }
}