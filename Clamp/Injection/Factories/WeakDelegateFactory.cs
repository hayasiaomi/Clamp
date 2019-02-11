using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Injection.Factories
{
    /// <summary>
    /// IObjectFactory that invokes a specified delegate to construct the object
    /// Holds the delegate using a weak reference
    /// </summary>
    internal class WeakDelegateFactory : ObjectFactoryBase
    {
        private readonly Type registerType;

        private WeakReference _factory;

        public override bool AssumeConstruction { get { return true; } }

        public override Type CreatesType { get { return this.registerType; } }

        public override object GetObject(Type requestedType, ClampObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            var factory = _factory.Target as Func<ClampObjectContainer, NamedParameterOverloads, object>;

            if (factory == null)
                throw new WeakReferenceException(this.registerType);

            try
            {
                return factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(this.registerType, ex);
            }
        }

        public WeakDelegateFactory(Type registerType, Func<ClampObjectContainer, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            _factory = new WeakReference(factory);

            this.registerType = registerType;
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                var factory = _factory.Target as Func<ClampObjectContainer, NamedParameterOverloads, object>;

                if (factory == null)
                    throw new WeakReferenceException(this.registerType);

                return new DelegateFactory(this.registerType, factory);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get
            {
                return this;
            }
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException("Constructor selection is not possible for delegate factory registrations");
        }
    }

}
