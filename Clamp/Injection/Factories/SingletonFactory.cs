using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Injection.Factories
{
    /// <summary>
    /// A factory that lazy instantiates a type and always returns the same instance
    /// </summary>
    internal class SingletonFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type registerType;
        private readonly Type registerImplementation;
        private readonly object SingletonLock = new object();
        private object _Current;

        public SingletonFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");

            if (!ClampObjectContainer.IsValidAssignment(registerType, registerImplementation))
                throw new RegistrationTypeException(registerImplementation, "SingletonFactory");

            this.registerType = registerType;
            this.registerImplementation = registerImplementation;
        }

        public override Type CreatesType
        {
            get { return this.registerImplementation; }
        }

        public override object GetObject(Type requestedType, ClampObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            if (parameters.Count != 0)
                throw new ArgumentException("Cannot specify parameters for singleton types");

            lock (SingletonLock)
                if (_Current == null)
                    _Current = container.ConstructType(requestedType, this.registerImplementation, Constructor, options);

            return _Current;
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                return this;
            }
        }

        public override ObjectFactoryBase GetCustomObjectLifetimeVariant(IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            return new CustomObjectLifetimeFactory(this.registerType, this.registerImplementation, lifetimeProvider, errorString);
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                return new MultiInstanceFactory(this.registerType, this.registerImplementation);
            }
        }

        public override ObjectFactoryBase GetFactoryForChildContainer(Type type, ClampObjectContainer parent, ClampObjectContainer child)
        {
            // We make sure that the singleton is constructed before the child container takes the factory.
            // Otherwise the results would vary depending on whether or not the parent container had resolved
            // the type before the child container does.
            GetObject(type, parent, NamedParameterOverloads.Default, ResolveOptions.Default);
            return this;
        }

        public void Dispose()
        {
            if (this._Current == null)
                return;

            var disposable = this._Current as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }

}
