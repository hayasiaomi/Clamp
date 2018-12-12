using Clamp.OSGI.Injection.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Clamp.OSGI.Injection
{
    internal sealed class RegisterOptions
    {
        private ObjectContainer _Container;
        private TypeRegistration _Registration;

        public RegisterOptions(ObjectContainer container, TypeRegistration registration)
        {
            _Container = container;
            _Registration = registration;
        }

        /// <summary>
        /// Make registration a singleton (single instance) if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public RegisterOptions AsSingleton()
        {
            var currentFactory = _Container.GetCurrentFactory(_Registration);

            if (currentFactory == null)
                throw new RegistrationException(_Registration.Type, "singleton");

            return _Container.AddUpdateRegistration(_Registration, currentFactory.SingletonVariant);
        }

        /// <summary>
        /// Make registration multi-instance if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public RegisterOptions AsMultiInstance()
        {
            var currentFactory = _Container.GetCurrentFactory(_Registration);

            if (currentFactory == null)
                throw new RegistrationException(_Registration.Type, "multi-instance");

            return _Container.AddUpdateRegistration(_Registration, currentFactory.MultiInstanceVariant);
        }

        /// <summary>
        /// Make registration hold a weak reference if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public RegisterOptions WithWeakReference()
        {
            var currentFactory = _Container.GetCurrentFactory(_Registration);

            if (currentFactory == null)
                throw new RegistrationException(_Registration.Type, "weak reference");

            return _Container.AddUpdateRegistration(_Registration, currentFactory.WeakReferenceVariant);
        }

        /// <summary>
        /// Make registration hold a strong reference if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public RegisterOptions WithStrongReference()
        {
            var currentFactory = _Container.GetCurrentFactory(_Registration);

            if (currentFactory == null)
                throw new RegistrationException(_Registration.Type, "strong reference");

            return _Container.AddUpdateRegistration(_Registration, currentFactory.StrongReferenceVariant);
        }

        public RegisterOptions UsingConstructor<RegisterType>(Expression<Func<RegisterType>> constructor)
        {
            if (!ObjectContainer.IsValidAssignment(_Registration.Type, typeof(RegisterType)))
                throw new ConstructorResolutionException(typeof(RegisterType));

            var lambda = constructor as LambdaExpression;
            if (lambda == null)
                throw new ConstructorResolutionException(typeof(RegisterType));

            var newExpression = lambda.Body as NewExpression;
            if (newExpression == null)
                throw new ConstructorResolutionException(typeof(RegisterType));

            var constructorInfo = newExpression.Constructor;
            if (constructorInfo == null)
                throw new ConstructorResolutionException(typeof(RegisterType));

            var currentFactory = _Container.GetCurrentFactory(_Registration);
            if (currentFactory == null)
                throw new ConstructorResolutionException(typeof(RegisterType));

            currentFactory.SetConstructor(constructorInfo);

            return this;
        }
        /// <summary>
        /// Switches to a custom lifetime manager factory if possible.
        /// 
        /// Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
        /// </summary>
        /// <param name="instance">RegisterOptions instance</param>
        /// <param name="lifetimeProvider">Custom lifetime manager</param>
        /// <param name="errorString">Error string to display if switch fails</param>
        /// <returns>RegisterOptions</returns>
        public static RegisterOptions ToCustomLifetimeManager(RegisterOptions instance, IObjectLifetimeProvider lifetimeProvider, string errorString)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "instance is null.");

            if (lifetimeProvider == null)
                throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

            if (string.IsNullOrEmpty(errorString))
                throw new ArgumentException("errorString is null or empty.", "errorString");

            var currentFactory = instance._Container.GetCurrentFactory(instance._Registration);

            if (currentFactory == null)
                throw new RegistrationException(instance._Registration.Type, errorString);

            return instance._Container.AddUpdateRegistration(instance._Registration, currentFactory.GetCustomObjectLifetimeVariant(lifetimeProvider, errorString));
        }
    }

}
