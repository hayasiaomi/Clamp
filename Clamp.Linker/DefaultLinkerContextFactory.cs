using Clamp.Linker.Culture;
using Clamp.Linker.Diagnostics;
using Clamp.Linker.Helpers;
using Clamp.Linker.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Clamp.Linker
{

    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultLinkerContextFactory : ILinkerContextFactory
    {
        private readonly ICultureService cultureService;
        private readonly IRequestTraceFactory requestTraceFactory;
        private readonly ITextResource textResource;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultLinkerContextFactory"/> class.
        /// </summary>
        /// <param name="cultureService">An <see cref="ICultureService"/> instance.</param>
        /// <param name="requestTraceFactory">An <see cref="IRequestTraceFactory"/> instance.</param>
        /// <param name="textResource">An <see cref="ITextResource"/> instance.</param>
        public DefaultLinkerContextFactory(ICultureService cultureService, IRequestTraceFactory requestTraceFactory, ITextResource textResource)
        {
            this.cultureService = cultureService;
            this.requestTraceFactory = requestTraceFactory;
            this.textResource = textResource;
        }

        /// <summary>
        /// Create a new <see cref="LinkerContext"/> instance.
        /// </summary>
        /// <returns>A <see cref="LinkerContext"/> instance.</returns>
        public LinkerContext Create(Request request)
        {
            var context = new LinkerContext();

            context.Trace = this.requestTraceFactory.Create(request);
            context.Request = request;
            context.Culture = this.cultureService.DetermineCurrentCulture(context);
            context.Text = new TextResourceFinder(this.textResource, context);
            context.BundleName = HttpUtility.GetUrlSegments(request.Path).FirstOrDefault();

            context.RuntimeBundle = LinkerActivator.BundleContext.GetRuntimeBundleByName(context.BundleName);

            // Move this to DefaultRequestTrace.
            context.Trace.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return context;
        }
    }
}