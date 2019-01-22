namespace ClampMVC.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using ClampMVC.Culture;

    /// <summary>
    /// Caches information about all the available routes that was discovered by the bootstrapper.
    /// </summary>
    public class RouteCache : Dictionary<Type, List<Tuple<int, RouteDescription>>>, IRouteCache
    {
        private readonly IRouteSegmentExtractor routeSegmentExtractor;
        private readonly IRouteDescriptionProvider routeDescriptionProvider;
        private readonly IEnumerable<IRouteMetadataProvider> routeMetadataProviders;

        /// <summary>
        /// Initializes a new instance of the <see cref="RouteCache"/> class.
        /// </summary>
        /// <param name="moduleCatalog">The <see cref="IControllerCatalog"/> that should be used by the cache.</param>
        /// <param name="contextFactory">The <see cref="IClampWebContextFactory"/> that should be used to create a context instance.</param>
        /// <param name="routeSegmentExtractor"> </param>
        /// <param name="cultureService"></param>
        /// <param name="routeMetadataProviders"></param>
        /// <param name="routeDescriptionProvider"></param>
        public RouteCache(
            IControllerCatalog moduleCatalog,
            IClampWebContextFactory contextFactory,
            IRouteSegmentExtractor routeSegmentExtractor,
            IRouteDescriptionProvider routeDescriptionProvider,
            ICultureService cultureService,
            IEnumerable<IRouteMetadataProvider> routeMetadataProviders)
        {
            this.routeSegmentExtractor = routeSegmentExtractor;
            this.routeDescriptionProvider = routeDescriptionProvider;
            this.routeMetadataProviders = routeMetadataProviders;

            var request = new Request("GET", "/", "http");

            using (var context = contextFactory.Create(request))
            {
                this.BuildCache(moduleCatalog.GetAllModules(context));
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates of the cache is empty or not.
        /// </summary>
        /// <returns><see langword="true"/> if the cache is empty, otherwise <see langword="false"/>.</returns>
        public bool IsEmpty()
        {
            return !this.Values.SelectMany(r => r).Any();
        }

        public void BuildModuleCache(IController controller)
        {
            var moduleType = controller.GetType();

            var routes = controller.Routes.Select(r => r.Description).ToArray();

            foreach (var routeDescription in routes)
            {
                routeDescription.Description = this.routeDescriptionProvider.GetDescription(controller, routeDescription.Path);
                routeDescription.Segments = this.routeSegmentExtractor.Extract(routeDescription.Path).ToArray();
                routeDescription.Metadata = this.GetRouteMetadata(controller, routeDescription);
            }

            this.AddRoutesToCache(routes, moduleType);
        }

        public void BuildCache(IEnumerable<IController> modules)
        {
            foreach (var module in modules)
            {
                this.BuildModuleCache(module);
            }
        }


        private RouteMetadata GetRouteMetadata(IController module, RouteDescription routeDescription)
        {
            var data = new Dictionary<Type, object>();

            foreach (var provider in this.routeMetadataProviders)
            {
                var type = provider.GetMetadataType(module, routeDescription);
                var metadata = provider.GetMetadata(module, routeDescription);

                if (type != null && metadata != null)
                {
                    data.Add(type, metadata);
                }
            }

            return new RouteMetadata(data);
        }

        private void AddRoutesToCache(IEnumerable<RouteDescription> routes, Type moduleType)
        {
            if (this.ContainsKey(moduleType))
            {
                throw new InvalidOperationException(string.Format("the module [{0}] is already existed. please check it", moduleType.FullName));
            }

            this[moduleType] = new List<Tuple<int, RouteDescription>>();

            this[moduleType].AddRange(routes.Select((r, i) => new Tuple<int, RouteDescription>(i, r)));
        }
    }
}