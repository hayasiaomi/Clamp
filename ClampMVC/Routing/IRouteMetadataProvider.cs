namespace Clamp.Linker.Routing
{
    using System;

    /// <summary>
    /// Defines the functionality for retrieving metadata for routes.
    /// </summary>
    public interface IRouteMetadataProvider
    {
        /// <summary>
        /// Gets the <see cref="Type"/> of the metadata that is created by the provider.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>A <see cref="Type"/> instance, or <see langword="null" /> if nothing is found.</returns>
        Type GetMetadataType(IController module, RouteDescription routeDescription);

        /// <summary>
        /// Gets the metadata for the provided route.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> instance that the route is declared in.</param>
        /// <param name="routeDescription">A <see cref="RouteDescription"/> for the route.</param>
        /// <returns>An object representing the metadata for the given route, or <see langword="null" /> if nothing is found.</returns>
        object GetMetadata(IController module, RouteDescription routeDescription);
    }
}