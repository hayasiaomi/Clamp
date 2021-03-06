﻿namespace Clamp.Linker.Bootstrapper
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Class for locating an INancyBootstrapper implementation.
    ///
    /// Will search the app domain for a non-abstract one, and if it can't find one
    /// it will use the default nancy one that uses TinyIoC.
    /// </summary>
    public static class LinkerBootstrapperLocator
    {
        private static ILinkerBootstrapper instance;

        /// <summary>
        /// Gets the located bootstrapper
        /// </summary>
        public static ILinkerBootstrapper Bootstrapper
        {
            get { return instance ?? (instance = LocateBootstrapper()); }
            set { instance = value; }
        }

        private static ILinkerBootstrapper LocateBootstrapper()
        {
            var bootstrapperType = GetBootstrapperType();

            try
            {
                return Activator.CreateInstance(bootstrapperType) as ILinkerBootstrapper;
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Could not initialize bootstrapper of type '{0}'.", bootstrapperType.FullName);
                throw new BootstrapperException(errorMessage, ex);
            }
        }

        private static Type GetBootstrapperType()
        {
            var customBootstrappers = AppDomainAssemblyTypeScanner
                .TypesOf<ILinkerBootstrapper>(ScanMode.ExcludeWebwork)
                .ToList();

            if (!customBootstrappers.Any())
            {
                return typeof(DefaultLinkerBootstrapper);
            }

            if (customBootstrappers.Count == 1)
            {
                return customBootstrappers.Single();
            }

            Type bootstrapper;
            if (TryFindMostDerivedType(customBootstrappers, out bootstrapper))
            {
                return bootstrapper;
            }

            var errorMessage = GetMultipleBootstrappersMessage(customBootstrappers);

            throw new BootstrapperException(errorMessage);
        }

        internal static bool TryFindMostDerivedType(List<Type> customBootstrappers, out Type bootstrapper)
        {
            var set = new HashSet<Type>();
            bootstrapper = null;

            if (customBootstrappers.All(b => set.Add(b.BaseType)))
            {
                var except = customBootstrappers.Except(set).ToList();
                bootstrapper = except.Count == 1 ? except[0] : null;
            }

            return bootstrapper != null;
        }

        private static string GetMultipleBootstrappersMessage(IEnumerable<Type> customBootstrappers)
        {
            var bootstrapperNames = customBootstrappers.Select(x => string.Concat(" - ", x.FullName));

            var bootstrapperList = string.Join(Environment.NewLine, bootstrapperNames);

            return string.Join(Environment.NewLine, new[]
            {
                "Located multiple bootstrappers:",
                bootstrapperList,
                string.Empty,
                "Either remove unused bootstrapper types or specify which type to use."
            });
        }
    }
}