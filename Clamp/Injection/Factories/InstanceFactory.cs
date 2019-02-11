using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.Injection.Factories
{
    /// <summary>
    /// Stores an particular instance to return for a type
    /// </summary>
    internal class InstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type registerType;
        private readonly Type registerImplementation;
        private object _instance;

        public override bool AssumeConstruction { get { return true; } }

        public InstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!ClampObjectContainer.IsValidAssignment(registerType, registerImplementation))
                throw new RegistrationTypeException(registerImplementation, "InstanceFactory");

            this.registerType = registerType;
            this.registerImplementation = registerImplementation;
            _instance = instance;
        }

        public override Type CreatesType
        {
            get { return this.registerImplementation; }
        }

        public override object GetObject(Type requestedType, ClampObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            return _instance;
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get { return new MultiInstanceFactory(this.registerType, this.registerImplementation); }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get
            {
                return new WeakInstanceFactory(this.registerType, this.registerImplementation, this._instance);
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
            throw new ConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
        }

        public void Dispose()
        {
            var disposable = _instance as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }
}
