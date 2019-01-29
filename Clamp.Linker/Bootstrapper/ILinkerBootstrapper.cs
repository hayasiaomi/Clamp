﻿namespace Clamp.Linker.Bootstrapper
{
    using Clamp.OSGI.Data.Annotation;
    using System;

    [TypeExtensionPoint]
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