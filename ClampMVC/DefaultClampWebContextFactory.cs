using Clamp.Linker.Culture;
using Clamp.Linker.Diagnostics;
using Clamp.Linker.Localization;
using System.Collections.Generic;
using System.Linq;

namespace Clamp.Linker
{

    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultClampWebContextFactory : IClampWebContextFactory
    {
        private readonly ICultureService cultureService;
        private readonly IRequestTraceFactory requestTraceFactory;
        private readonly ITextResource textResource;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultClampWebContextFactory"/> class.
        /// </summary>
        /// <param name="cultureService">An <see cref="ICultureService"/> instance.</param>
        /// <param name="requestTraceFactory">An <see cref="IRequestTraceFactory"/> instance.</param>
        /// <param name="textResource">An <see cref="ITextResource"/> instance.</param>
        public DefaultClampWebContextFactory(ICultureService cultureService, IRequestTraceFactory requestTraceFactory, ITextResource textResource)
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

            context.BundleName = GetUrlSegments(request.Path).FirstOrDefault();

            // Move this to DefaultRequestTrace.
            context.Trace.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return context;
        }

        public List<string> GetUrlSegments(string path)
        {
            List<string> segments = new List<string>();

            var currentSegment = string.Empty;
            var openingParenthesesCount = 0;

            for (var index = 0; index < path.Length; index++)
            {
                var token = path[index];

                if (token.Equals('('))
                {
                    openingParenthesesCount++;
                }

                if (token.Equals(')'))
                {
                    openingParenthesesCount--;
                }

                if (!token.Equals('/') || openingParenthesesCount > 0)
                {
                    currentSegment += token;
                }

                if ((token.Equals('/') || index == path.Length - 1) && currentSegment.Length > 0 && openingParenthesesCount == 0)
                {
                    segments.Add(currentSegment);

                    currentSegment = string.Empty;
                }
            }

            return segments;
        }
    }
}