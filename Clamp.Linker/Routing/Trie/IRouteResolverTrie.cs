namespace Clamp.Linker.Routing.Trie
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Trie structure for resolving routes
    /// </summary>
    public interface IRouteResolverTrie
    {
        /// <summary>
        /// Build the trie from the route cache
        /// </summary>
        /// <param name="cache">The route cache</param>
        void BuildTrie(IRouteCache cache);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="moduleKey"></param>
        /// <param name="routeDefinitions"></param>
        void BuildTrie(Type moduleKey, List<Tuple<int, RouteDescription>> routeDefinitions);

        /// <summary>
        /// Get all matches for the given method and path
        /// </summary>
        /// <param name="method">HTTP method</param>
        /// <param name="path">Requested path</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>An array of <see cref="MatchResult"/> elements</returns>
        MatchResult[] GetMatches(string method, string path, LinkerContext context);

        /// <summary>
        /// Get all method options for the given path
        /// </summary>
        /// <param name="path">Requested path</param>
        /// <param name="context">Current Nancy context</param>
        /// <returns>A collection of strings, each representing an allowed method</returns>
        IEnumerable<string> GetOptions(string path, LinkerContext context);
    }
}