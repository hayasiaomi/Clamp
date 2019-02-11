using Clamp.OSGI.Injection.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Injection
{
    internal sealed class MultiRegisterOptions
    {
        private IEnumerable<RegisterOptions> _RegisterOptions;

        /// <summary>
        /// Initializes a new instance of the MultiRegisterOptions class.
        /// </summary>
        /// <param name="registerOptions">Registration options</param>
        public MultiRegisterOptions(IEnumerable<RegisterOptions> registerOptions)
        {
            _RegisterOptions = registerOptions;
        }

        /// <summary>
        /// Make registration a singleton (single instance) if possible
        /// </summary>
        /// <returns>RegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public MultiRegisterOptions AsSingleton()
        {
            _RegisterOptions = ExecuteOnAllRegisterOptions(ro => ro.AsSingleton());
            return this;
        }

        /// <summary>
        /// Make registration multi-instance if possible
        /// </summary>
        /// <returns>MultiRegisterOptions</returns>
        /// <exception cref="RegistrationException"></exception>
        public MultiRegisterOptions AsMultiInstance()
        {
            _RegisterOptions = ExecuteOnAllRegisterOptions(ro => ro.AsMultiInstance());
            return this;
        }

        /// <summary>
        /// Switches to a custom lifetime manager factory if possible.
        /// 
        /// Usually used for RegisterOptions "To*" extension methods such as the ASP.Net per-request one.
        /// </summary>
        /// <param name="instance">MultiRegisterOptions instance</param>
        /// <param name="lifetimeProvider">Custom lifetime manager</param>
        /// <param name="errorString">Error string to display if switch fails</param>
        /// <returns>MultiRegisterOptions</returns>
        public static MultiRegisterOptions ToCustomLifetimeManager(
            MultiRegisterOptions instance,
            IObjectLifetimeProvider lifetimeProvider,
            string errorString)
        {
            if (instance == null)
                throw new ArgumentNullException("instance", "instance is null.");

            if (lifetimeProvider == null)
                throw new ArgumentNullException("lifetimeProvider", "lifetimeProvider is null.");

            if (string.IsNullOrEmpty(errorString))
                throw new ArgumentException("errorString is null or empty.", "errorString");

            instance._RegisterOptions = instance.ExecuteOnAllRegisterOptions(ro => RegisterOptions.ToCustomLifetimeManager(ro, lifetimeProvider, errorString));

            return instance;
        }

        private IEnumerable<RegisterOptions> ExecuteOnAllRegisterOptions(Func<RegisterOptions, RegisterOptions> action)
        {
            var newRegisterOptions = new List<RegisterOptions>();

            foreach (var registerOption in _RegisterOptions)
            {
                newRegisterOptions.Add(action(registerOption));
            }

            return newRegisterOptions;
        }
    }
}
