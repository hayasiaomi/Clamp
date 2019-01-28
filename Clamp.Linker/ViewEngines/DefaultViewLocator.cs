namespace Clamp.Linker.ViewEngines
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading;

    /// <summary>
    /// The default implementation for how views are located by Nancy.
    /// </summary>
    public class DefaultViewLocator : IViewLocator
    {
        private readonly List<ViewLocationResult> viewLocationResultCaches = new List<ViewLocationResult>();

        private readonly IViewLocationProvider viewLocationProvider;

        private readonly IEnumerable<IViewEngine> viewEngines;

        private readonly ReaderWriterLockSlim padlock = new ReaderWriterLockSlim();

        private readonly char[] invalidCharacters;

        public DefaultViewLocator(IViewLocationProvider viewLocationProvider, IEnumerable<IViewEngine> viewEngines)
        {
            this.viewLocationProvider = viewLocationProvider;
            this.viewEngines = viewEngines;

            this.invalidCharacters = Path.GetInvalidFileNameChars().Where(c => c != '/').ToArray();
        }


        public void Refresh()
        {
            this.viewLocationResultCaches.Clear();
        }

        /// <summary>
        /// Gets the location of the view defined by the <paramref name="viewName"/> parameter.
        /// </summary>
        /// <param name="viewName">Name of the view to locate.</param>
        /// <param name="context">The <see cref="LinkerContext"/> instance for the current request.</param>
        /// <returns>A <see cref="ViewLocationResult"/> instance if the view could be located; otherwise <see langword="null"/>.</returns>
        public ViewLocationResult LocateView(string viewName, ViewLocationContext viewLocationContext)
        {
            if (string.IsNullOrEmpty(viewName))
            {
                return null;
            }

            if (!this.IsValidViewName(viewName))
            {
                return null;
            }

            this.padlock.EnterUpgradeableReadLock();

            try
            {
                var cachedResults = this.GetCachedMatchingViews(viewName, viewLocationContext);

                if (cachedResults.Count == 1)
                {
                    return cachedResults.Single();
                }

                if (cachedResults.Count > 1)
                {
                    throw new AmbiguousViewsException(GetAmgiguousViewExceptionMessage(cachedResults.Count, cachedResults));
                }

                return null;
            }
            finally
            {
                this.padlock.ExitUpgradeableReadLock();
            }
        }

        /// <summary>
        /// Gets all the views that are currently discovered
        /// Note: this is *not* the recommended way to deal with the view locator
        /// as it doesn't allow for runtime discovery of views with the
        /// <see cref="StaticConfiguration.Caching"/> settings.
        /// </summary>
        /// <returns>A collection of <see cref="ViewLocationResult"/> instances</returns>
        public IEnumerable<ViewLocationResult> GetAllCurrentlyDiscoveredViews()
        {
            this.padlock.EnterReadLock();

            try
            {
                // Make a copy to avoid any modification issues
                var newList = new List<ViewLocationResult>(this.viewLocationResultCaches.Count);
                this.viewLocationResultCaches.ForEach(newList.Add);
                return newList;
            }
            finally
            {
                this.padlock.ExitReadLock();
            }
        }


        private ViewLocationResult[] GetUncachedMatchingViews(string viewName, string viewLocation)
        {
            var viewExtension = GetExtensionFromViewName(viewName);

            var supportedViewExtensions = String.IsNullOrEmpty(viewExtension) ? GetSupportedViewExtensions() : new[] { viewExtension };

            var location = GetLocationFromViewName(viewName, viewLocation);
            var nameWithoutExtension = GetFilenameWithoutExtensionFromViewName(viewName);

            return this.viewLocationProvider.GetLocatedViews(supportedViewExtensions, location, nameWithoutExtension).ToArray();
        }

        private List<ViewLocationResult> GetCachedMatchingViews(string viewName, ViewLocationContext viewLocationContext)
        {
            List<ViewLocationResult> viewLocationResults = new List<ViewLocationResult>();

            string bundleName = viewLocationContext.Context.RuntimeBundle.Name;

            viewLocationResults.AddRange(this.viewLocationResultCaches
                       .Where(x => BundleNameMatchesView(bundleName, x))
                       .Where(x => NameMatchesView(viewName, x))
                       .Where(x => ExtensionMatchesView(viewName, x))
                       .Where(x => LocationMatchesView(viewName, x, viewLocationContext.ModuleLocation))
                       .ToList());

            if (!viewLocationResults.Any())
            {
                ViewLocationResult viewLocationResult = this.viewLocationProvider.GetLocatedViewLocation(viewName, GetSupportedViewExtensions(), viewLocationContext);

                if (viewLocationResult != null)
                {
                    this.viewLocationResultCaches.Add(viewLocationResult);
                    viewLocationResults.Add(viewLocationResult);
                }
            }

            return viewLocationResults;
        }

        private IEnumerable<string> GetSupportedViewExtensions()
        {
            return this.viewEngines.SelectMany(engine => engine.Extensions).Distinct();
        }

        private static string GetAmgiguousViewExceptionMessage(int count, IEnumerable<ViewLocationResult> viewsThatMatchesCritera)
        {
            return string.Format("This exception was thrown because multiple views were found. {0} view(s):\r\n\t{1}", count, string.Join("\r\n\t", viewsThatMatchesCritera.Select(GetFullLocationOfView).ToArray()));
        }

        private static string GetFullLocationOfView(ViewLocationResult viewLocationResult)
        {
            return string.Concat(viewLocationResult.Location, "/", viewLocationResult.Name, ".", viewLocationResult.Extension);
        }

        private static bool ExtensionMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var extension = GetExtensionFromViewName(viewName);

            return string.IsNullOrEmpty(extension) || viewLocationResult.Extension.Equals(extension, StringComparison.OrdinalIgnoreCase);
        }

        private static bool LocationMatchesView(string viewName, ViewLocationResult viewLocationResult, string viewLocation)
        {
            var location = GetLocationFromViewName(viewName, viewLocation);

            return viewLocationResult.Location.Equals(location, StringComparison.OrdinalIgnoreCase);
        }

        private static bool BundleNameMatchesView(string bundleName, ViewLocationResult viewLocationResult)
        {
            return (!string.IsNullOrEmpty(bundleName)) && viewLocationResult.BundleName.Equals(bundleName, StringComparison.OrdinalIgnoreCase);
        }

        private static bool NameMatchesView(string viewName, ViewLocationResult viewLocationResult)
        {
            var name = GetFilenameWithoutExtensionFromViewName(viewName);

            return (!string.IsNullOrEmpty(name)) && viewLocationResult.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }

        private static string GetFilenameWithoutExtensionFromViewName(string viewName)
        {
            return Path.GetFileNameWithoutExtension(viewName);
        }

        private static string GetLocationFromViewName(string viewName, string viewLocation)
        {
            var filename = Path.GetFileName(viewName);
            var index = viewName.LastIndexOf(filename, StringComparison.OrdinalIgnoreCase);
            var location = index >= 0 ? viewName.Remove(index, filename.Length) : viewName;
            location = location.TrimEnd(new[] { '/' });

            return string.Concat(viewLocation, "/", location);
        }

        private static string GetExtensionFromViewName(string viewName)
        {
            var extension = Path.GetExtension(viewName);

            return !String.IsNullOrEmpty(extension) ? extension.Substring(1) : extension;
        }

        private bool IsValidViewName(string viewName)
        {
            return !this.invalidCharacters.Any(viewName.Contains);
        }
    }
}