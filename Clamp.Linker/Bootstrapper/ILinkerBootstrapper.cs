namespace Clamp.Linker.Bootstrapper
{
    using Clamp.Data.Annotation;
    using System;

    public interface ILinkerBootstrapper : IDisposable
    {
        /// <summary>
        /// Initialise the bootstrapper. Must be called prior to GetEngine.
        /// </summary>
        void Initialise();

        /// <summary>
        /// Gets the configured INancyEngine
        /// </summary>
        /// <returns>Configured INancyEngine</returns>
        ILinkerEngine GetEngine();
    }
}