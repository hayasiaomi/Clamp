using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Injection.Factories
{
    internal abstract class ObjectFactoryBase
    {
        /// <summary>
        /// Whether to assume this factory successfully constructs its objects
        /// 
        /// Generally set to true for delegate style factories as CanResolve cannot delve
        /// into the delegates they contain.
        /// </summary>
        public virtual bool AssumeConstruction { get { return false; } }

        /// <summary>
        /// The type the factory instantiates
        /// </summary>
        public abstract Type CreatesType { get; }

        /// <summary>
        /// Constructor to use, if specified
        /// </summary>
        public ConstructorInfo Constructor { get; protected set; }

        /// <summary>
        /// Create the type
        /// </summary>
        /// <param name="requestedType">Type user requested to be resolved</param>
        /// <param name="container">Container that requested the creation</param>
        /// <param name="parameters">Any user parameters passed</param>
        /// <param name="options"></param>
        /// <returns></returns>
        public abstract object GetObject(Type requestedType, ObjectContainer container, NamedParameterOverloads parameters, ResolveOptions options);

        public virtual ObjectFactoryBase SingletonVariant
        {
            get
            {
                throw new RegistrationException(this.GetType(), "singleton");
            }
        }

        public virtual ObjectFactoryBase MultiInstanceVariant
        {
            get
            {
                throw new RegistrationException(this.GetType(), "multi-instance");
            }
        }

        public virtual ObjectFactoryBase StrongReferenceVariant
        {
            get
            {
                throw new RegistrationException(this.GetType(), "strong reference");
            }
        }

        public virtual ObjectFactoryBase WeakReferenceVariant
        {
            get
            {
                throw new RegistrationException(this.GetType(), "weak reference");
            }
        }

        public virtual ObjectFactoryBase GetCustomObjectLifetimeVariant(IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            throw new RegistrationException(this.GetType(), errorString);
        }

        public virtual void SetConstructor(ConstructorInfo constructor)
        {
            Constructor = constructor;
        }

        public virtual ObjectFactoryBase GetFactoryForChildContainer(Type type, ObjectContainer parent, ObjectContainer child)
        {
            return this;
        }
    }
}
