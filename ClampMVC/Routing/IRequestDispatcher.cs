namespace Clamp.Linker.Routing
{
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Functionality for processing an incoming request.
    /// </summary>
    public interface IRequestDispatcher
    {
        /// <summary>
        /// Dispatches a requests.
        /// </summary>
        /// <param name="context">The <see cref="LinkerContext"/> for the current request.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        Task<Response> Dispatch(LinkerContext context, CancellationToken cancellationToken);
    }
}