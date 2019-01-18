using System;

namespace ClampMVC.Bootstrapper
{
    /// <summary>
    /// Bootstrapper for the Nancy Engine
    /// </summary>
    public interface IWebworkBootstrapper : IDisposable
    {
        /// <summary>
        /// Initialise the bootstrapper. Must be called prior to GetEngine.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        IWebworkEngine GetEngine();
    }
}