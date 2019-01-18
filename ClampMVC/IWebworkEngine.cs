namespace ClampMVC
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Bootstrapper;

    /// <summary>
    /// Defines the functionality of an engine that can handle Nancy <see cref="Request"/>s.
    /// </summary>
    public interface IWebworkEngine : IDisposable
    {
        /// <summary>
        /// Factory for creating an <see cref="IPipelines"/> instance for a incoming request.
        /// </summary>
        /// <value>An <see cref="IPipelines"/> instance.</value>
        Func<WebworkContext, IPipelines> RequestPipelinesFactory { get; set; }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        /// <returns>The task object representing the asynchronous operation.</returns>
        Task<WebworkContext> HandleRequest(Request request, Func<WebworkContext, WebworkContext> preRequest, CancellationToken cancellationToken);
    }
}