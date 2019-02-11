namespace Clamp.Linker.Routing
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Clamp;
    using Clamp.Linker.Annotation;
    using Clamp.Linker;
    using Clamp.Linker.Helpers;
    using Trie;

    using MatchResult = Trie.MatchResult;

    /// <summary>
    /// Default implementation of the <see cref="IRouteResolver"/> interface.
    /// </summary>
    public class DefaultRouteResolver : IRouteResolver
    {
        private readonly IControllerCatalog catalog;
        private readonly IWebworkModuleBuilder moduleBuilder;
        private readonly IRouteCache routeCache;
        private readonly IRouteResolverTrie trie;
        private readonly IRouteSegmentExtractor routeSegmentExtractor;

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultRouteResolver"/> class, using
        /// the provided <paramref name="catalog"/>, <paramref name="moduleBuilder"/>,
        /// <paramref name="routeCache"/> and <paramref name="trie"/>.
        /// </summary>
        /// <param name="catalog">A <see cref="IControllerCatalog"/> instance.</param>
        /// <param name="moduleBuilder">A <see cref="IWebworkModuleBuilder"/> instance.</param>
        /// <param name="routeCache">A <see cref="IRouteCache"/> instance.</param>
        /// <param name="trie">A <see cref="IRouteResolverTrie"/> instance.</param>
        public DefaultRouteResolver(IControllerCatalog catalog, IWebworkModuleBuilder moduleBuilder, IRouteSegmentExtractor routeSegmentExtractor, IRouteCache routeCache, IRouteResolverTrie trie)
        {
            this.catalog = catalog;
            this.moduleBuilder = moduleBuilder;
            this.routeCache = routeCache;
            this.trie = trie;
            this.routeSegmentExtractor = routeSegmentExtractor;

            //this.BuildTrie();
        }

        /// <summary>
        /// Gets the route, and the corresponding parameter dictionary from the URL
        /// </summary>
        /// <param name="context">Current context</param>
        /// <returns>A <see cref="ResolveResult"/> containing the resolved route information.</returns>
        public ResolveResult Resolve(LinkerContext context)
        {
            var pathDecoded = HttpUtility.UrlDecode(context.Request.Path);

            MatchResult[] results = this.trie.GetMatches(GetMethod(context), pathDecoded, context);

            if (!results.Any())
            {
                string[] segments = this.routeSegmentExtractor.Extract(context.Request.Path).ToArray();

                if (segments != null && segments.Length > 0)
                {
                    if (context.RuntimeBundle != null)
                    {
                        IController[] controllers = context.RuntimeBundle.GetExtensionObjects<IController>();

                        if (controllers != null && controllers.Length > 0)
                        {
                            IController controller = controllers.FirstOrDefault(ctl => this.MatchControllerName(ctl, segments[1]));

                            if (controller != null)
                            {
                                KeyValuePair<Type, List<Tuple<int, RouteDescription>>> moduleCache = routeCache.GetBuildModuleCache(controller);

                                this.trie.BuildTrie(moduleCache.Key, moduleCache.Value);

                                results = this.trie.GetMatches(GetMethod(context), pathDecoded, context);
                            }
                        }

                    }


                }

                if (!results.Any())
                {
                    var allowedMethods = this.trie.GetOptions(pathDecoded, context).ToArray();

                    if (IsOptionsRequest(context))
                    {
                        return BuildOptionsResult(allowedMethods, context);
                    }

                    return IsMethodNotAllowed(allowedMethods) ? BuildMethodNotAllowedResult(context, allowedMethods) : GetNotFoundResult(context);
                }
            }

            // Sort in descending order
            Array.Sort(results, (m1, m2) => -m1.CompareTo(m2));

            for (var index = 0; index < results.Length; index++)
            {
                var matchResult = results[index];
                if (matchResult.Condition == null || matchResult.Condition.Invoke(context))
                {
                    return this.BuildResult(context, matchResult);
                }
            }

            return GetNotFoundResult(context);
        }


        public void BuildTrie()
        {
            this.trie.BuildTrie(this.routeCache);
        }

        private static ResolveResult BuildOptionsResult(IEnumerable<string> allowedMethods, LinkerContext context)
        {
            var path = context.Request.Path;

            var optionsResult = new OptionsRoute(path, allowedMethods);

            return new ResolveResult(optionsResult, new DynamicDictionary(), null, null, null);
        }

        private ResolveResult BuildResult(LinkerContext context, MatchResult result)
        {
            var associatedModule = this.GetModuleFromMatchResult(context, result);

            context.NegotiationContext.SetModule(associatedModule);

            var route = associatedModule.Routes.ElementAt(result.RouteIndex);
            var parameters = DynamicDictionary.Create(result.Parameters);

            return new ResolveResult
            {
                Route = route,
                Parameters = parameters,
                Before = associatedModule.Before,
                After = associatedModule.After,
                OnError = associatedModule.OnError
            };
        }

        private IController GetModuleFromMatchResult(LinkerContext context, MatchResult result)
        {
            var module = this.catalog.GetModule(result.ModuleType, context);

            return this.moduleBuilder.BuildModule(module, context);
        }

        private bool MatchControllerName(IController controller, string controllerName)
        {
            if (controller == null)
                return false;

            Type controllerType = controller.GetType();

            object[] attributes = controllerType.GetCustomAttributes(typeof(ControllerAttribute), true);

            if (attributes == null || attributes.Length <= 0)
                return false;

            ControllerAttribute controllerAttribute = attributes[0] as ControllerAttribute;

            if (controllerAttribute == null)
                return false;

            string ctlName = controllerAttribute.ControllerName;

            if (string.IsNullOrWhiteSpace(ctlName))
            {
                int ctlIndex = controllerType.Name.LastIndexOf("Controller", StringComparison.CurrentCultureIgnoreCase);

                if (ctlIndex > 0)
                {
                    ctlName = controllerType.Name.Substring(0, ctlIndex);
                }
                else
                {
                    ctlName = controllerType.Name;
                }
            }

            return string.Equals(ctlName, controllerName, StringComparison.CurrentCultureIgnoreCase);
        }


        private static ResolveResult GetNotFoundResult(LinkerContext context)
        {
            return new ResolveResult
            {
                Route = new NotFoundRoute(context.Request.Method, context.Request.Path),
                Parameters = DynamicDictionary.Empty,
                Before = null,
                After = null,
                OnError = null
            };
        }

        private static string GetMethod(LinkerContext context)
        {
            var requestedMethod = context.Request.Method;

            if (!StaticConfiguration.EnableHeadRouting)
            {
                return requestedMethod.Equals("HEAD", StringComparison.Ordinal) ? "GET" : requestedMethod;
            }

            return requestedMethod;
        }


        private static ResolveResult BuildMethodNotAllowedResult(LinkerContext context, IEnumerable<string> allowedMethods)
        {
            var route = new MethodNotAllowedRoute(context.Request.Path, context.Request.Method, allowedMethods);

            return new ResolveResult(route, new DynamicDictionary(), null, null, null);
        }

        private static bool IsMethodNotAllowed(IEnumerable<string> allowedMethods)
        {
            return allowedMethods.Any() && !StaticConfiguration.DisableMethodNotAllowedResponses;
        }

        private static bool IsOptionsRequest(LinkerContext context)
        {
            return context.Request.Method.Equals("OPTIONS", StringComparison.Ordinal);
        }


    }
}
