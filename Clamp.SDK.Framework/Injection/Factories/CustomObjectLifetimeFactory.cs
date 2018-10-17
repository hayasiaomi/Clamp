using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Injection.Factories
{

    /// <summary>
    /// A factory that offloads lifetime to an external lifetime provider
    /// </summary>
    internal class CustomObjectLifetimeFactory : ObjectFactoryBase, IDisposable
    {
        private readonly object SingletonLock = new object();
        private readonly Type registerType;
        private readonly Type registerImplementation;
        private readonly IObjectLifetimeProvider _LifetimeProvider;

        public CustomObjectLifetimeFactory(Type registerType, Type registerImplementation, IObjectLifetimeProvider lifetimeProvider, string errorMessage)
        {
            if (lifetimeProvider == null)
                throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

            if (!Container.IsValidAssignment(registerType, registerImplementation))
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");
       
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new RegistrationTypeException(registerImplementation, errorMessage);

            this.registerType = registerType;
            this.registerImplementation = registerImplementation;
            _LifetimeProvider = lifetimeProvider;
        }

        public override Type CreatesType
        {
            get { return this.registerImplementation; }
        }

        public override object GetObject(Type requestedType, Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            object current;

            lock (SingletonLock)
            {
                current = _LifetimeProvider.GetObject();
                if (current == null)
                {
                    current = container.ConstructType(requestedType, this.registerImplementation, Constructor, options);
                    _LifetimeProvider.SetObject(current);
                }
            }

            return current;
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                _LifetimeProvider.ReleaseObject();
                return new SingletonFactory(this.registerType, this.registerImplementation);
            }
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                _LifetimeProvider.ReleaseObject();
                return new MultiInstanceFactory(this.registerType, this.registerImplementation);
            }
        }

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            _LifetimeProvider.ReleaseObject();
            return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
        }

        public override ObjectFactoryBase GetFactoryForChildContainer(Type type, Container parent, Container child)
        {
            // We make sure that the singleton is constructed before the child container takes the factory.
            // Otherwise the results would vary depending on whether or not the parent container had resolved
            // the type before the child container does.
            GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
            return this;
        }

        public void Dispose()
        {
            _LifetimeProvider.ReleaseObject();
        }
    }
}
