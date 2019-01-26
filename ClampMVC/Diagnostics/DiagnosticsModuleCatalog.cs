namespace Clamp.Linker.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using ModelBinding;
    using Clamp.Linker.Bootstrapper;
    using Clamp.Linker.TinyIoc;
    using Responses;

    internal class DiagnosticsModuleCatalog : IControllerCatalog
    {
        private readonly TinyIoCContainer container;

        public DiagnosticsModuleCatalog(IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, ClampWebInternalConfiguration configuration, DiagnosticsConfiguration diagnosticsConfiguration)
        {
            this.container = ConfigureContainer(providers, rootPathProvider, requestTracing, configuration, diagnosticsConfiguration);
        }

        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IController"/> instances.</returns>
        public IEnumerable<IController> GetAllModules(ClampWebContext context)
        {
            return this.container.ResolveAll<IController>(false);
        }

        /// <summary>
        /// Retrieves a specific <see cref="IController"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="IController"/> instance</returns>
        public IController GetModule(Type moduleType, ClampWebContext context)
        {
            return this.container.Resolve<IController>(moduleType.FullName);
        }

        private static TinyIoCContainer ConfigureContainer(IEnumerable<IDiagnosticsProvider> providers, IRootPathProvider rootPathProvider, IRequestTracing requestTracing, ClampWebInternalConfiguration configuration, DiagnosticsConfiguration diagnosticsConfiguration)
        {
            var diagContainer = new TinyIoCContainer();

            diagContainer.Register<IInteractiveDiagnostics, InteractiveDiagnostics>();
            diagContainer.Register<IRequestTracing>(requestTracing);
            diagContainer.Register<IRootPathProvider>(rootPathProvider);
            diagContainer.Register<ClampWebInternalConfiguration>(configuration);
            diagContainer.Register<IModelBinderLocator, DefaultModelBinderLocator>();
            diagContainer.Register<IBinder, DefaultBinder>();
            diagContainer.Register<IFieldNameConverter, DefaultFieldNameConverter>();
            diagContainer.Register<BindingDefaults, BindingDefaults>();
            diagContainer.Register<ISerializer>(new DefaultJsonSerializer { RetainCasing = false });
            diagContainer.Register<DiagnosticsConfiguration>(diagnosticsConfiguration);

            foreach (var diagnosticsProvider in providers)
            {
                var key = string.Concat(
                    diagnosticsProvider.GetType().FullName,
                    "_",
                    diagnosticsProvider.DiagnosticObject.GetType().FullName);
                
                diagContainer.Register<IDiagnosticsProvider>(diagnosticsProvider, key);
            }

            foreach (var moduleType in AppDomainAssemblyTypeScanner.TypesOf<DiagnosticController>().ToArray())
            {
                diagContainer.Register(typeof(IController), moduleType, moduleType.FullName).AsMultiInstance();
            }

            return diagContainer;
        }
    }
}