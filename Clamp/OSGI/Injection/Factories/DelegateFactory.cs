using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Injection.Factories
{
    /// <summary>
    /// IObjectFactory that invokes a specified delegate to construct the object
    /// </summary>
    internal class DelegateFactory : ObjectFactoryBase
    {
        private readonly Type registerType;

        private Func<ClampObjectContainer, NamedParameterOverloads, object> _factory;

        public override bool AssumeConstruction { get { return true; } }

        public override Type CreatesType { get { return this.registerType; } }

        public override object GetObject(Type requestedType, ClampObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            try
            {
                return _factory.Invoke(container, parameters);
            }
            catch (Exception ex)
            {
                throw new ResolutionException(this.registerType, ex);
            }
        }

        public DelegateFactory(Type registerType, Func<ClampObjectContainer, NamedParameterOverloads, object> factory)
        {
            if (factory == null)
                throw new ArgumentNullException("factory");

            _factory = factory;

            this.registerType = registerType;
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get
            {
                return new WeakDelegateFactory(this.registerType, _factory);
            }
        }

        public override ObjectFactoryBase StrongReferenceVariant
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
