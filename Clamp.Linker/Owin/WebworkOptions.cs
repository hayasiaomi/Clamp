namespace Clamp.Linker.Owin
{
    using System;
    using Clamp.Linker.Bootstrapper;

    /// <summary>
    /// Options for hosting Nancy with OWIN.
    /// </summary>
    public class WebworkOptions
    {
        private ILinkerBootstrapper bootstrapper;
        private Func<LinkerContext, bool> performPassThrough;

        /// <summary>
        /// Gets or sets the bootstrapper. If none is set, NancyBootstrapperLocator.Bootstrapper is used.
        /// </summary>
        public ILinkerBootstrapper Bootstrapper
        {
            get { return this.bootstrapper ?? LinkerBootstrapperLocator.Bootstrapper; }
            set { this.bootstrapper = value; }
        }

        /// <summary>
        /// Gets or sets the delegate that determines if NancyMiddleware performs pass through.
        /// </summary>
        public Func<LinkerContext, bool> PerformPassThrough
        {
            get { return this.performPassThrough ?? (context => false); }
            set { this.performPassThrough = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to request a client certificate or not.
        /// Defaults to false.
        /// </summary>
        public bool EnableClientCertificates { get; set; }
    }
}