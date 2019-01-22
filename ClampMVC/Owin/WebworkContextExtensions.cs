﻿namespace ClampMVC.Owin
{
    using System.Collections.Generic;

    /// <summary>
    /// OWIN extensions for the NancyContext.
    /// </summary>
    public static class WebworkContextExtensions
    {
        /// <summary>
        /// Gets the OWIN environment dictionary.
        /// </summary>
        /// <param name="context">The Nancy context.</param>
        /// <returns>The OWIN environment dictionary.</returns>
        public static IDictionary<string, object> GetOwinEnvironment(this ClampWebContext context)
        {
            object environment;
            if (context.Items.TryGetValue(WebworkMiddleware.RequestEnvironmentKey, out environment))
            {
                return environment as IDictionary<string, object>;
            }

            return null;
        }
    }
}