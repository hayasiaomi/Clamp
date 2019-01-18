namespace ClampMVC
{
    using ClampMVC.Culture;
    using ClampMVC.Diagnostics;
    using ClampMVC.Localization;

    /// <summary>
    /// Creates NancyContext instances
    /// </summary>
    public class DefaultWebworkContextFactory : IWebworkContextFactory
    {
        private readonly ICultureService cultureService;
        private readonly IRequestTraceFactory requestTraceFactory;
        private readonly ITextResource textResource;

        /// <summary>
        /// Creates a new instance of the <see cref="DefaultWebworkContextFactory"/> class.
        /// </summary>
        /// <param name="cultureService">An <see cref="ICultureService"/> instance.</param>
        /// <param name="requestTraceFactory">An <see cref="IRequestTraceFactory"/> instance.</param>
        /// <param name="textResource">An <see cref="ITextResource"/> instance.</param>
        public DefaultWebworkContextFactory(ICultureService cultureService, IRequestTraceFactory requestTraceFactory, ITextResource textResource)
        {
            this.cultureService = cultureService;
            this.requestTraceFactory = requestTraceFactory;
            this.textResource = textResource;
        }

        /// <summary>
        /// Create a new <see cref="WebworkContext"/> instance.
        /// </summary>
        /// <returns>A <see cref="WebworkContext"/> instance.</returns>
        public WebworkContext Create(Request request)
        {
            var context =
                new WebworkContext();

            context.Trace = this.requestTraceFactory.Create(request);
            context.Request = request;
            context.Culture = this.cultureService.DetermineCurrentCulture(context);
            context.Text = new TextResourceFinder(this.textResource, context);

            // Move this to DefaultRequestTrace.
            context.Trace.TraceLog.WriteLog(s => s.AppendLine("New Request Started"));

            return context;
        }
    }
}