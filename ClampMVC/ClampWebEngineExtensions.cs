

namespace ClampMVC
{
    using System;
    using System.Threading;
    using ClampMVC.Helpers;

    public static class WebClampEngineExtensions
    {
        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="IClampWebEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <returns>A <see cref="ClampWebContext"/> instance containing the request/response context.</returns>
        public static ClampWebContext HandleRequest(this IClampWebEngine nancyEngine, Request request)
        {
            return HandleRequest(nancyEngine, request, context => context);
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/>.
        /// </summary>
        /// <param name="nancyEngine">The <see cref="IClampWebEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <returns>A <see cref="ClampWebContext"/> instance containing the request/response context.</returns>
        public static ClampWebContext HandleRequest(this IClampWebEngine nancyEngine, Request request, Func<ClampWebContext, ClampWebContext> preRequest)
        {
            var task = nancyEngine.HandleRequest(request, preRequest, CancellationToken.None);

            try
            {
                task.Wait();
            }
            catch (Exception ex)
            {
                throw ex.FlattenInnerExceptions();
            }

            return task.Result;
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="webworkEngine">The <see cref="IClampWebEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="preRequest">Delegate to call before the request is processed</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Delegate to call when any errors occur</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
        public static void HandleRequest(
            this IClampWebEngine webworkEngine,
            Request request,
            Func<ClampWebContext, ClampWebContext> preRequest,
            Action<ClampWebContext> onComplete,
            Action<Exception> onError,
            CancellationToken cancellationToken)
        {
            if (webworkEngine == null)
            {
                throw new ArgumentNullException("nancyEngine");
            }

            webworkEngine
                .HandleRequest(request, preRequest, cancellationToken)
                .WhenCompleted(t => onComplete(t.Result), t => onError(t.Exception));
        }

        /// <summary>
        /// Handles an incoming <see cref="Request"/> async.
        /// </summary>
        /// <param name="webworkEngine">The <see cref="IClampWebEngine"/> instance.</param>
        /// <param name="request">An <see cref="Request"/> instance, containing the information about the current request.</param>
        /// <param name="onComplete">Delegate to call when the request is complete</param>
        /// <param name="onError">Delegate to call when any errors occur</param>
        public static void HandleRequest(
            this IClampWebEngine webworkEngine,
            Request request,
            Action<ClampWebContext> onComplete,
            Action<Exception> onError)
        {
            HandleRequest(webworkEngine, request, null, onComplete, onError, CancellationToken.None);
        }
    }
}