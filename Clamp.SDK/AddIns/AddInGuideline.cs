using Clamp.AddIns;
using Clamp.Webwork;
using Clamp.Webwork.Bootstrapper;
using Clamp.Webwork.Culture;
using Clamp.Webwork.Diagnostics;
using Clamp.Webwork.Localization;
using Clamp.Webwork.ModelBinding;
using Clamp.Webwork.Responses.Negotiation;
using Clamp.Webwork.Routing;
using Clamp.Webwork.Routing.Constraints;
using Clamp.Webwork.ViewEngines;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.AddIns
{
    public class AddInGuideline : IAddInGuideline
    {
        private readonly Func<IRouteCache> routeCacheFactory;
        private readonly Func<IRouteResolver> routeResolverFactory;
        private readonly Func<IViewLocator> viewLocatorFactory;
        private bool IsInitialized = false;
        public AddInGuideline(Func<IRouteResolver> routeResolverFactory, Func<IRouteCache> routeCacheFactory, Func<IViewLocator> viewLocatorFactory)
        {
            this.routeResolverFactory = routeResolverFactory;
            this.routeCacheFactory = routeCacheFactory;
            this.viewLocatorFactory = viewLocatorFactory;
        }

        public IRouteCache GetCache()
        {
            return this.routeCacheFactory();
        }

        public IRouteResolver GetResolver()
        {
            return this.routeResolverFactory();
        }
        public IViewLocator GetViewLocator()
        {
            return this.viewLocatorFactory();
        }

        /// <summary>
        /// Initialise diagnostics
        /// </summary>
        /// <param name="pipelines">Application pipelines</param>
        public void Initialize()
        {
            if (!this.IsInitialized)
            {
                IRouteCache routeCache = this.GetCache();
                IRouteResolver routeResolver = this.GetResolver();
                IViewLocator viewLocator = this.GetViewLocator();

                List<IController> controllers = AddInManager.GetInstance<IController>("/ShanDain/Webworks");

                if (controllers != null && controllers.Count > 0)
                {
                    foreach (IController controller in controllers)
                    {
                        SD.Log.Info($"增加通信模块{controller.GetType().Name}");

                        routeCache.BuildModuleCache(controller);
                    }
                }

                routeResolver.BuildTrie();
                viewLocator.Refresh();

                this.IsInitialized = true;
            }
        }

    }
}
