namespace Clamp.Linker.ViewEngines
{
    using Clamp.Linker;

    /// <summary>
    /// Interface for manually rendering views to a Response object, rather
    /// than going through content negotiation.
    /// </summary>
    public interface IViewRenderer
    {
        /// <summary>
        /// Renders a view to a response object, bypassing content negotiation.
        /// </summary>
        /// <param name="context">Current Nancy context</param>
        /// <param name="viewName">View name</param>
        /// <param name="model">Model object (or null)</param>
        /// <returns>Response object containing the rendered view (if found)</returns>
        Response RenderView(LinkerContext context, string viewName, object model = null);
    }
}