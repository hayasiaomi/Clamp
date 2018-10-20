using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Injection.Factories
{
    /// <summary>
    /// Stores an particular instance to return for a type
    /// 
    /// Stores the instance with a weak reference
    /// </summary>
    internal class WeakInstanceFactory : ObjectFactoryBase, IDisposable
    {
        private readonly Type registerType;
        private readonly Type registerImplementation;
        private readonly WeakReference _instance;

        public WeakInstanceFactory(Type registerType, Type registerImplementation, object instance)
        {
            if (!ObjectContainer.IsValidAssignment(registerType, registerImplementation))
                throw new RegistrationTypeException(registerImplementation, "WeakInstanceFactory");

            this.registerType = registerType;
            this.registerImplementation = registerImplementation;
            _instance = new WeakReference(instance);
        }

        public override Type CreatesType
        {
            get { return this.registerImplementation; }
        }

        public override object GetObject(Type requestedType, ObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options)
        {
            var instance = _instance.Target;

            if (instance == null)
                throw new WeakReferenceException(this.registerType);

            return instance;
        }

        public override ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                return new MultiInstanceFactory(this.registerType, this.registerImplementation);
            }
        }

        public override ObjectFactoryBase WeakReferenceVariant
        {
            get
            {
                return this;
            }
        }

        public override ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                var instance = _instance.Target;

                if (instance == null)
                    throw new WeakReferenceException(this.registerType);

                return new InstanceFactory(this.registerType, this.registerImplementation, instance);
            }
        }

        public override void SetConstructor(ConstructorInfo constructor)
        {
            throw new ConstructorResolutionException("Constructor selection is not possible for instance factory registrations");
        }

        public void Dispose()
        {
            var disposable = _instance.Target as IDisposable;

            if (disposable != null)
                disposable.Dispose();
        }
    }
}
