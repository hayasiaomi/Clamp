using Clamp.OSGI.Data;
using Clamp.OSGI.Data.Description;
using Clamp.OSGI.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;

namespace Clamp.OSGI
{
    /// <summary>
    /// 运行时候的Bundle
    /// </summary>
    public class RuntimeBundle
    {
        private string id;
        private string baseDirectory;
        private string privatePath;
        private Bundle bundle;
        private RuntimeBundle parentBundle;
        private Assembly[] assemblies;
        private RuntimeBundle[] depBundles;
        private ResourceManager[] resourceManagers;
        private BundleLocalizer localizer;
        private ModuleDescription module;
        private ClampBundle clampBundle;

        internal RuntimeBundle(ClampBundle clampBundle)
        {
            this.clampBundle = clampBundle;
        }

        internal RuntimeBundle(ClampBundle clampBundle, RuntimeBundle parentBundle, ModuleDescription module)
        {
            this.clampBundle = clampBundle;
            this.parentBundle = parentBundle;
            this.module = module;
            id = parentBundle.id;
            baseDirectory = parentBundle.baseDirectory;
            privatePath = parentBundle.privatePath;
            bundle = parentBundle.bundle;
            localizer = parentBundle.localizer;
            module.RuntimeBundle = this;
        }

        /// <summary>
        /// Identifier of the add-in.
        /// </summary>
        public string Id
        {
            get { return Bundle.GetIdName(id); }
        }

        /// <summary>
        /// Version of the add-in.
        /// </summary>
        public string Version
        {
            get { return Bundle.GetIdVersion(id); }
        }


        /// <summary>
        /// Path to a directory where add-ins can store private configuration or status data
        /// </summary>
        public string PrivateDataPath
        {
            get
            {
                if (privatePath == null)
                {
                    privatePath = bundle.PrivateDataPath;
                    if (!Directory.Exists(privatePath))
                        Directory.CreateDirectory(privatePath);
                }
                return privatePath;
            }
        }

        /// <summary>
        /// Localizer which can be used to localize strings defined in this add-in
        /// </summary>
        public BundleLocalizer Localizer
        {
            get
            {
                if (localizer != null)
                    return localizer;
                else
                    return clampBundle.DefaultLocalizer;
            }
        }

        internal Bundle Bundle
        {
            get { return bundle; }
        }

        internal bool AssembliesLoaded
        {
            get { return assemblies != null; }
        }

        internal ModuleDescription Module
        {
            get { return module; }
        }

        internal Assembly[] Assemblies
        {
            get
            {
                EnsureAssembliesLoaded();
                return assemblies;
            }
        }


        #region public method
        /// <summary>
        /// Returns a string that represents the current RuntimeBundle.
        /// </summary>
        /// <returns>
        /// A string that represents the current RuntimeBundle.
        /// </returns>
        public override string ToString()
        {
            return bundle.ToString();
        }



        /// <summary>
        /// Gets a resource string
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <returns>
        /// The value of the resource string, or null if the resource can't be found.
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public string GetResourceString(string name)
        {
            return (string)GetResourceObject(name, true, null);
        }

        /// <summary>
        /// Gets a resource string
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <param name="throwIfNotFound">
        /// When set to true, an exception will be thrown if the resource is not found.
        /// </param>
        /// <returns>
        /// The value of the resource string
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public string GetResourceString(string name, bool throwIfNotFound)
        {
            return (string)GetResourceObject(name, throwIfNotFound, null);
        }

        /// <summary>
        /// Gets a resource string
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <param name="throwIfNotFound">
        /// When set to true, an exception will be thrown if the resource is not found.
        /// </param>
        /// <param name="culture">
        /// Culture of the resource
        /// </param>
        /// <returns>
        /// The value of the resource string
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public string GetResourceString(string name, bool throwIfNotFound, CultureInfo culture)
        {
            return (string)GetResourceObject(name, throwIfNotFound, culture);
        }

        /// <summary>
        /// Gets a resource object
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <returns>
        /// Value of the resource
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public object GetResourceObject(string name)
        {
            return GetResourceObject(name, true, null);
        }

        /// <summary>
        /// Gets a resource object
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <param name="throwIfNotFound">
        /// When set to true, an exception will be thrown if the resource is not found.
        /// </param>
        /// <returns>
        /// Value of the resource
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public object GetResourceObject(string name, bool throwIfNotFound)
        {
            return GetResourceObject(name, throwIfNotFound, null);
        }

        /// <summary>
        /// Gets a resource object
        /// </summary>
        /// <param name="name">
        /// Name of the resource
        /// </param>
        /// <param name="throwIfNotFound">
        /// When set to true, an exception will be thrown if the resource is not found.
        /// </param>
        /// <param name="culture">
        /// Culture of the resource
        /// </param>
        /// <returns>
        /// Value of the resource
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public object GetResourceObject(string name, bool throwIfNotFound, CultureInfo culture)
        {
            // Look in resources of this add-in
            foreach (ResourceManager manager in GetAllResourceManagers())
            {
                object t = manager.GetObject(name, culture);
                if (t != null)
                    return t;
            }

            // Look in resources of dependent add-ins
            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                object t = addin.GetResourceObject(name, false, culture);
                if (t != null)
                    return t;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Resource object '" + name + "' not found in add-in '" + id + "'");

            return null;
        }

        /// <summary>
        /// Gets a type defined in the add-in
        /// </summary>
        /// <param name="typeName">
        /// Full name of the type
        /// </param>
        /// <returns>
        /// A type.
        /// </returns>
        /// <remarks>
        /// The type will be looked up in the assemblies that implement the add-in,
        /// and recursively in all add-ins on which it depends.
        /// 
        /// This method throws an InvalidOperationException if the type can't be found.
        /// </remarks>
        public Type GetType(string typeName)
        {
            return GetType(typeName, true);
        }

        /// <summary>
        /// Gets a type defined in the add-in
        /// </summary>
        /// <param name="typeName">
        /// Full name of the type
        /// </param>
        /// <param name="throwIfNotFound">
        /// Indicates whether the method should throw an exception if the type can't be found.
        /// </param>
        /// <returns>
        /// A <see cref="Type"/>
        /// </returns>
        /// <remarks>
        /// The type will be looked up in the assemblies that implement the add-in,
        /// and recursively in all add-ins on which it depends.
        /// 
        /// If the type can't be found, this method throw a InvalidOperationException if
        /// 'throwIfNotFound' is 'true', or 'null' otherwise.
        /// </remarks>
        public Type GetType(string typeName, bool throwIfNotFound)
        {
            EnsureAssembliesLoaded();

            // Look in the addin assemblies

            Type at = Type.GetType(typeName, false);
            if (at != null)
                return at;

            foreach (Assembly asm in GetAllAssemblies())
            {
                Type t = asm.GetType(typeName, false);
                if (t != null)
                    return t;
            }

            // Look in the dependent add-ins
            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                Type t = addin.GetType(typeName, false);
                if (t != null)
                    return t;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Type '" + typeName + "' not found in add-in '" + id + "'");
            return null;
        }


        /// <summary>
        /// Creates an instance of a type defined in the add-in
        /// </summary>
        /// <param name="typeName">
        /// Name of the type.
        /// </param>
        /// <returns>
        /// A new instance of the type
        /// </returns>
        /// <remarks>
        /// The type will be looked up in the assemblies that implement the add-in,
        /// and recursively in all add-ins on which it depends.
        /// 
        /// This method throws an InvalidOperationException if the type can't be found.
        /// 
        /// The specified type must have a default constructor.
        /// </remarks>
        public object CreateInstance(string typeName)
        {
            return CreateInstance(typeName, true);
        }

        /// <summary>
        /// Creates an instance of a type defined in the add-in
        /// </summary>
        /// <param name="typeName">
        /// Name of the type.
        /// </param>
        /// <param name="throwIfNotFound">
        /// Indicates whether the method should throw an exception if the type can't be found.
        /// </param>
        /// <returns>
        /// A new instance of the type
        /// </returns>
        /// <remarks>
        /// The type will be looked up in the assemblies that implement the add-in,
        /// and recursively in all add-ins on which it depends.
        /// 
        /// If the type can't be found, this method throw a InvalidOperationException if
        /// 'throwIfNotFound' is 'true', or 'null' otherwise.
        /// 
        /// The specified type must have a default constructor.
        /// </remarks>
        public object CreateInstance(string typeName, bool throwIfNotFound)
        {
            Type type = GetType(typeName, throwIfNotFound);
            if (type == null)
                return null;
            else
                return Activator.CreateInstance(type, true);
        }

        /// <summary>
        /// Gets the path of an add-in file
        /// </summary>
        /// <param name="fileName">
        /// Relative path of the file
        /// </param>
        /// <returns>
        /// Full path of the file
        /// </returns>
        /// <remarks>
        /// This method can be used to get the full path of a data file deployed together with the add-in.
        /// </remarks>
        public string GetFilePath(string fileName)
        {
            return Path.Combine(baseDirectory, fileName);
        }

        /// <summary>
        /// Gets the path of an add-in file
        /// </summary>
        /// <param name="filePath">
        /// Components of the file path
        /// </param>
        /// <returns>
        /// Full path of the file
        /// </returns>
        /// <remarks>
        /// This method can be used to get the full path of a data file deployed together with the add-in.
        /// </remarks>
        public string GetFilePath(params string[] filePath)
        {
            return Path.Combine(baseDirectory, string.Join("" + Path.DirectorySeparatorChar, filePath));
        }



        /// <summary>
        /// Gets the content of a resource
        /// </summary>
        /// <param name="resourceName">
        /// Name of the resource
        /// </param>
        /// <returns>
        /// Content of the resource, or null if not found
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public Stream GetResource(string resourceName)
        {
            return GetResource(resourceName, false);
        }

        /// <summary>
        /// Gets the content of a resource
        /// </summary>
        /// <param name="resourceName">
        /// Name of the resource
        /// </param>
        /// <param name="throwIfNotFound">
        /// When set to true, an exception will be thrown if the resource is not found.
        /// </param>
        /// <returns>
        /// Content of the resource.
        /// </returns>
        /// <remarks>
        /// The add-in engine will look for resources in the main add-in assembly and in all included add-in assemblies.
        /// </remarks>
        public Stream GetResource(string resourceName, bool throwIfNotFound)
        {
            EnsureAssembliesLoaded();

            // Look in the addin assemblies

            foreach (Assembly asm in GetAllAssemblies())
            {
                Stream res = asm.GetManifestResourceStream(resourceName);
                if (res != null)
                    return res;
            }

            // Look in the dependent add-ins
            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                Stream res = addin.GetResource(resourceName);
                if (res != null)
                    return res;
            }

            if (throwIfNotFound)
                throw new InvalidOperationException("Resource '" + resourceName + "' not found in add-in '" + id + "'");

            return null;
        }

        /// <summary>
        /// Returns information about how the given resource has been persisted
        /// </summary>
        /// <param name="resourceName">
        /// Name of the resource
        /// </param>
        /// <returns>
        /// Resource information, or null if the resource doesn't exist
        /// </returns>
        public ManifestResourceInfo GetResourceInfo(string resourceName)
        {
            EnsureAssembliesLoaded();

            // Look in the addin assemblies

            foreach (Assembly asm in GetAllAssemblies())
            {
                var res = asm.GetManifestResourceInfo(resourceName);
                if (res != null)
                {
                    // Mono doesn't set the referenced assembly
                    if (res.ReferencedAssembly == null)
                        return new ManifestResourceInfo(asm, res.FileName, res.ResourceLocation);
                    return res;
                }
            }

            // Look in the dependent add-ins
            foreach (RuntimeBundle addin in GetAllDependencies())
            {
                var res = addin.GetResourceInfo(resourceName);
                if (res != null)
                    return res;
            }

            return null;
        }

        #endregion

        #region internal Method
        internal RuntimeBundle GetModule(ModuleDescription module)
        {
            // If requesting the root module, return this
            if (module == module.ParentBundleDescription.MainModule)
                return this;

            if (module.RuntimeBundle != null)
                return module.RuntimeBundle;

            RuntimeBundle addin = new RuntimeBundle(clampBundle, this, module);
            return addin;
        }

        internal BundleDescription Load(Bundle iad)
        {
            bundle = iad;

            BundleDescription description = iad.Description;
            id = description.BundleId;
            baseDirectory = description.BasePath;
            module = description.MainModule;
            module.RuntimeBundle = this;

            if (description.Localizer != null)
            {
                string cls = description.Localizer.GetAttribute("type");

                // First try getting one of the stock localizers. If none of found try getting the type.
                object fob = null;
                Type t = Type.GetType("Mono.Bundles.Localization." + cls + "Localizer, " + GetType().Assembly.FullName, false);
                if (t != null)
                    fob = Activator.CreateInstance(t);

                if (fob == null)
                    fob = CreateInstance(cls, true);

                IBundleLocalizerFactory factory = fob as IBundleLocalizerFactory;

                if (factory == null)
                    throw new InvalidOperationException("Localizer factory type '" + cls + "' must implement IBundleLocalizerFactory");

                localizer = new BundleLocalizer(factory.CreateLocalizer(this, description.Localizer));
            }

            return description;
        }

        internal void UnloadExtensions()
        {
            clampBundle.UnregisterBundleNodeSets(id);
        }

        internal void EnsureAssembliesLoaded()
        {
            if (assemblies != null)
                return;

            ArrayList asmList = new ArrayList();

            // Load the assemblies of the module
            CheckBundleDependencies(module, true);
            LoadModule(module, asmList);

            assemblies = (Assembly[])asmList.ToArray(typeof(Assembly));
            clampBundle.RegisterAssemblies(this);
        }

        #endregion

        #region private method


        private IEnumerable<ResourceManager> GetAllResourceManagers()
        {
            foreach (ResourceManager rm in GetResourceManagers())
                yield return rm;

            if (parentBundle != null)
            {
                foreach (ResourceManager rm in parentBundle.GetResourceManagers())
                    yield return rm;
            }
        }

        private IEnumerable<Assembly> GetAllAssemblies()
        {
            foreach (Assembly asm in Assemblies)
                yield return asm;

            // Look in the parent addin assemblies

            if (parentBundle != null)
            {
                foreach (Assembly asm in parentBundle.Assemblies)
                    yield return asm;
            }
        }

        private IEnumerable<RuntimeBundle> GetAllDependencies()
        {
            // Look in the dependent add-ins
            foreach (RuntimeBundle addin in GetDepBundles())
                yield return addin;

            if (parentBundle != null)
            {
                // Look in the parent dependent add-ins
                foreach (RuntimeBundle addin in parentBundle.GetDepBundles())
                    yield return addin;
            }
        }

        private ResourceManager[] GetResourceManagers()
        {
            if (resourceManagers != null)
                return resourceManagers;

            EnsureAssembliesLoaded();
            ArrayList managersList = new ArrayList();

            // Search for embedded resource files
            foreach (Assembly asm in assemblies)
            {
                foreach (string res in asm.GetManifestResourceNames())
                {
                    if (res.EndsWith(".resources"))
                        managersList.Add(new ResourceManager(res.Substring(0, res.Length - ".resources".Length), asm));
                }
            }

            return resourceManagers = (ResourceManager[])managersList.ToArray(typeof(ResourceManager));
        }

        private RuntimeBundle[] GetDepBundles()
        {
            if (depBundles != null)
                return depBundles;

            ArrayList plugList = new ArrayList();
            string ns = bundle.Description.Namespace;

            // Collect dependent ids
            foreach (Dependency dep in module.Dependencies)
            {
                BundleDependency pdep = dep as BundleDependency;
                if (pdep != null)
                {
                    RuntimeBundle adn = clampBundle.GetBundle(Bundle.GetFullId(ns, pdep.BundleId, pdep.Version));
                    if (adn != null)
                        plugList.Add(adn);
                    else
                        clampBundle.ReportError("Add-in dependency not loaded: " + pdep.FullBundleId, module.ParentBundleDescription.BundleId, null, false);
                }
            }
            return depBundles = (RuntimeBundle[])plugList.ToArray(typeof(RuntimeBundle));
        }

        private void LoadModule(ModuleDescription module, ArrayList asmList)
        {
            // Load the assemblies
            foreach (string s in module.Assemblies)
            {
                Assembly asm = null;

                // don't load the assembly if it's already loaded
                string asmPath = Path.Combine(baseDirectory, s);
                foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    // Sorry, you can't load addins from
                    // dynamic assemblies as get_Location
                    // throws a NotSupportedException
                    if (a is System.Reflection.Emit.AssemblyBuilder || a.IsDynamic)
                    {
                        continue;
                    }

                    try
                    {
                        if (a.Location == asmPath)
                        {
                            asm = a;
                            break;
                        }
                    }
                    catch (NotSupportedException)
                    {
                        // Some assemblies don't have a location
                    }
                }

                if (asm == null)
                {
                    asm = Assembly.LoadFrom(asmPath);
                }

                asmList.Add(asm);
            }
        }

        private bool CheckBundleDependencies(ModuleDescription module, bool forceLoadAssemblies)
        {
            foreach (Dependency dep in module.Dependencies)
            {
                BundleDependency pdep = dep as BundleDependency;
                if (pdep == null)
                    continue;
                if (!clampBundle.IsBundleLoaded(pdep.FullBundleId))
                    return false;
                if (forceLoadAssemblies)
                    clampBundle.GetBundle(pdep.FullBundleId).EnsureAssembliesLoaded();
            }
            return true;
        }

        #endregion

    }
}
