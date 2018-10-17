using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Framework.Injection.Factories
{
    /// <summary>
    /// Provides custom lifetime management for ASP.Net per-request lifetimes etc.
    /// </summary>
    public interface IObjectLifetimeProvider
    {
        /// <summary>
        /// Gets the stored object if it exists, or null if not
        /// </summary>
        /// <returns>Object instance or null</returns>
        object GetObject();

        /// <summary>
        /// Store the object
        /// </summary>
        /// <param name="value">Object to store</param>
        void SetObject(object value);

        /// <summary>
        /// Release the object
        /// </summary>
        void ReleaseObject();
    }
}
