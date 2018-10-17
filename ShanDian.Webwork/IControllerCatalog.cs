namespace ShanDian.Webwork
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Catalog of <see cref="IController"/> instances.
    /// </summary>
    public interface IControllerCatalog
    {
        /// <summary>
        /// Get all NancyModule implementation instances - should be per-request lifetime
        /// </summary>
        /// <param name="context">The current context</param>
        /// <returns>An <see cref="IEnumerable{T}"/> instance containing <see cref="IController"/> instances.</returns>
        IEnumerable<IController> GetAllModules(WebworkContext context);

        /// <summary>
        /// Retrieves a specific <see cref="IController"/> implementation - should be per-request lifetime
        /// </summary>
        /// <param name="moduleType">Module type</param>
        /// <param name="context">The current context</param>
        /// <returns>The <see cref="IController"/> instance</returns>
        IController GetModule(Type moduleType, WebworkContext context);
    }
}