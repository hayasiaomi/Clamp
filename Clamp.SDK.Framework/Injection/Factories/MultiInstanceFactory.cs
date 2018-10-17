using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK.Framework.Injection.Factories
{
    /// <summary>
    /// IObjectFactory that creates new instances of types for each resolution
    /// </summary>
    internal class MultiInstanceFactory : ObjectFactoryBase
    {
        private readonly Type registerType;
        private readonly Type registerImplementation;
        public override Type CreatesType { get { return this.registerImplementation; } }

        public MultiInstanceFactory(Type registerType, Type registerImplementation)
        {
            if (registerImplementation.IsAbstract() || registerImplementation.IsInterface())
                throw new RegistrationTypeException(registerImplementation, "MultiInstanceFactory");

            if (!Container.IsValidAssignment(registerType, registerImplementation))
                throw new RegistrationTypeException(registerImplementation, "MultiInstanceFactory");

            this.registerType = registerType;
            this.registerImplementation = registerImplementation;
        }

        public override object GetObject(Type requestedType, Container container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return container.ConstructType(requestedType, this.registerImplementation, Constructor, parameters, options);
            }
            catch (ResolutionException ex)
            {
                throw new ResolutionException(this.registerType, ex);
            }
        }

        public override ObjectFactoryBase SingletonVariant
        {
            get
            {
                return new SingletonFactory(this.registerType, this.registerImplementation);
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
                return this;
            }
        }
    }
}
