namespace ShanDian.Webwork.ViewEngines
{
    using System.Collections.Generic;

    /// <summary>
    /// Defines the functionality for locating the requested view.
    /// </summary>
    public interface IViewLocator : IHideObjectMembers
    {
        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <param name="context">The <see cref="WebworkContext"/> instance for the current request.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        ViewLocationResult LocateView(string viewName, ViewLocationContext viewLocationContext);

        /// <summary>
        /// Gets all the views that are currently discovered
        /// Note: this is *not* the recommended way to deal with the view locator
        /// as it doesn't allow for runtime discovery of views with the
        /// <see cref="StaticConfiguration.Caching"/> settings.
        /// </summary>
        /// <returns>A collection of <see cref="ViewLocationResult"/> instances</returns>
        IEnumerable<ViewLocationResult> GetAllCurrentlyDiscoveredViews();

        void Refresh();
    }
}