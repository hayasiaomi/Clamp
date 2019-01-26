using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Clamp.Linker
{
    using Bootstrapper;
    using Cookies;
    using Diagnostics;
    using ErrorHandling;
    using Routing;
    using Helpers;
    using Responses.Negotiation;

    /// <summary>
    /// Default engine for handling Nancy <see cref="Request"/>s.
    /// </summary>
    public class ClampWebEngine : ILinkerEngine
    {
        public const string ERROR_KEY = "ERROR_TRACE";
        public const string ERROR_EXCEPTION = "ERROR_EXCEPTION";

        private readonly IRequestDispatcher dispatcher;
        private readonly IClampWebContextFactory contextFactory;
        private readonly IRequestTracing requestTracing;
        private readonly IEnumerable<IStatusCodeHandler> statusCodeHandlers;
        private readonly IStaticContentProvider staticContentProvider;
        private readonly IResponseNegotiator negotiator;
        private readonly CancellationTokenSource engineDisposedCts;

        /// <summary>
        /// Initializes a new instance of the <see cref="ClampWebEngine"/> class.
        /// </summary>
        /// <param name="dispatcher">An <see cref="IRouteResolver"/> instance that will be used to resolve a route, from the modules, that matches the incoming <see cref="Request"/>.</param>
        /// <param name="contextFactory">A factory for creating contexts</param>
        /// <param name="statusCodeHandlers">Error handlers</param>
        /// <param name="requestTracing">The request tracing instance.</param>
        /// <param name="staticContentProvider">The provider to use for serving static content</param>
        /// <param name="negotiator">The response negotiator.</param>
        public ClampWebEngine(IRequestDispatcher dispatcher,
            IClampWebContextFactory contextFactory,
            IEnumerable<IStatusCodeHandler> statusCodeHandlers,
            IRequestTracing requestTracing,
            IStaticContentProvider staticContentProvider,
            IResponseNegotiator negotiator)
        {
            if (dispatcher == null)
            {
                throw new ArgumentNullException("dispatcher", "The resolver parameter cannot be null.");
            }

            if (contextFactory == null)
            {
                throw new ArgumentNullException("contextFactory");
            }

            if (statusCodeHandlers == null)
            {
                throw new ArgumentNullException("statusCodeHandlers");
            }

            if (requestTracing == null)
            {
                throw new ArgumentNullException("requestTracing");
            }

            if (staticContentProvider == null)
            {
                throw new ArgumentNullException("staticContentProvider");
            }

            if (negotiator == null)
            {
                throw new ArgumentNullException("negotiator");
            }

            this.dispatcher = dispatcher;
            this.contextFactory = contextFactory;
            this.statusCodeHandlers = statusCodeHandlers;
            this.requestTracing = requestTracing;
            this.staticContentProvider = staticContentProvider;
            this.negotiator = negotiator;
            this.engineDisposedCts = new CancellationTokenSource();
        }

        /// <summary>
        /// Factory for creating an <see cref="IPipelines"/> instance for a incoming request.
        /// </summary>
        /// <value>An <see cref="IPipelines"/> instance.</value>
        public Func<ClampWebContext, IPipelines> RequestPipelinesFactory { get; set; }

        public Task<ClampWebContext> HandleRequest(Request request, Func<ClampWebContext, ClampWebContext> preRequest, CancellationToken cancellationToken)
        {
	        var cts = CancellationTokenSource.CreateLinkedTokenSource(this.engineDisposedCts.Token, cancellationToken);
            cts.Token.ThrowIfCancellationRequested();

            var tcs = new TaskCompletionSource<ClampWebContext>();

            if (request == null)
            {
                throw new ArgumentNullException("request", "The request parameter cannot be null.");
            }

            var context = this.contextFactory.Create(request);

            if (preRequest != null)
            {
                context = preRequest(context);
            }

            var staticContentResponse = this.staticContentProvider.GetContent(context);
            if (staticContentResponse != null)
            {
                context.Response = staticContentResponse;
                tcs.SetResult(context);
                return tcs.Task;
            }

            var pipelines = this.RequestPipelinesFactory.Invoke(context);

            var lifeCycleTask = this.InvokeRequestLifeCycle(context, cts.Token, pipelines);

            lifeCycleTask.WhenCompleted(
                completeTask =>
                {
	                try
	                {
		                this.CheckStatusCodeHandler(completeTask.Result);

		                this.SaveTraceInformation(completeTask.Result);
	                }
	                catch (Exception ex)
	                {
		                tcs.SetException(ex);
		                return;
	                }
	                finally
	                {
		                cts.Dispose();
	                }

                    tcs.SetResult(completeTask.Result);
                },
                errorTask =>
                {
		            tcs.SetException(errorTask.Exception);
                },
                true);

            return tcs.Task;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.engineDisposedCts.Cancel();
        }

        private void SaveTraceInformation(ClampWebContext ctx)
        {
            if (!this.EnableTracing(ctx))
            {
                return;
            }

            if (ctx.Request == null || ctx.Response == null)
            {
                return;
            }

            var sessionGuid = this.GetDiagnosticsSessionGuid(ctx);

            ctx.Trace.RequestData = ctx.Request;
            ctx.Trace.ResponseData = ctx.Response;

            this.requestTracing.AddRequestDiagnosticToSession(sessionGuid, ctx);

            this.UpdateTraceCookie(ctx, sessionGuid);
        }

        private bool EnableTracing(ClampWebContext ctx)
        {
            return StaticConfiguration.EnableRequestTracing &&
                   !ctx.Items.ContainsKey(DiagnosticsHook.ItemsKey);
        }

        private Guid GetDiagnosticsSessionGuid(ClampWebContext ctx)
        {
            string sessionId;
            if (!ctx.Request.Cookies.TryGetValue("__NCTRACE", out sessionId))
            {
                return this.requestTracing.CreateSession();
            }

            Guid sessionGuid;
            if (!Guid.TryParse(sessionId, out sessionGuid))
            {
                return this.requestTracing.CreateSession();
            }

            if (!this.requestTracing.IsValidSessionId(sessionGuid))
            {
                return this.requestTracing.CreateSession();
            }

            return sessionGuid;
        }

        private void UpdateTraceCookie(ClampWebContext ctx, Guid sessionGuid)
        {
            var cookie = new WebworkCookie("__NCTRACE", sessionGuid.ToString(), true)
            {
                Expires = DateTime.Now.AddMinutes(30)
            };

            ctx.Response = ctx.Response.WithCookie(cookie);
        }

        private void CheckStatusCodeHandler(ClampWebContext context)
        {
            if (context.Response == null)
            {
                return;
            }

            var handlers = this.statusCodeHandlers
                .Where(x => x.HandlesStatusCode(context.Response.StatusCode, context))
                .ToList();

            var defaultHandler = handlers
                .FirstOrDefault(x => x is DefaultStatusCodeHandler);

            var customHandler = handlers
                .FirstOrDefault(x => !(x is DefaultStatusCodeHandler));

            var handler = customHandler ?? defaultHandler;
            if (handler == null)
            {
                return;
            }

            handler.Handle(context.Response.StatusCode, context);
        }

        private Task<ClampWebContext> InvokeRequestLifeCycle(ClampWebContext context, CancellationToken cancellationToken, IPipelines pipelines)
        {
            var tcs = new TaskCompletionSource<ClampWebContext>();

            var preHookTask = InvokePreRequestHook(context, cancellationToken, pipelines.BeforeRequest);

            preHookTask.WhenCompleted(t =>
                {
                    var dispatchTask = t.Result != null ? TaskHelpers.GetCompletedTask(t.Result) : this.dispatcher.Dispatch(context, cancellationToken);

                    dispatchTask.WhenCompleted(
                        completedTask =>
                        {
                            context.Response = completedTask.Result;

                            var postHookTask = this.InvokePostRequestHook(context, cancellationToken, pipelines.AfterRequest);

                            postHookTask.WhenCompleted(this.PreExecute(context, pipelines, tcs), this.HandleFaultedTask(context, pipelines, tcs));
                        },
                        this.HandleFaultedTask(context, pipelines, tcs));
                },
                this.HandleFaultedTask(context, pipelines, tcs));

            return tcs.Task;
        }

        private Action<Task> PreExecute(ClampWebContext context, IPipelines pipelines, TaskCompletionSource<ClampWebContext> tcs)
        {
            return postHookTask =>
            {
                var preExecuteTask = context.Response.PreExecute(context);

                preExecuteTask.WhenCompleted(
                    completedPostHookTask => tcs.SetResult(context),
                    this.HandleFaultedTask(context, pipelines, tcs));
            };
        }

        private Action<Task> HandleFaultedTask(ClampWebContext context, IPipelines pipelines, TaskCompletionSource<ClampWebContext> tcs)
        {
            return t =>
                {
                    try
                    {
                        var flattenedException = t.Exception.FlattenInnerExceptions();

                        this.InvokeOnErrorHook(context, pipelines.OnError, flattenedException);

                        tcs.SetResult(context);
                    }
                    catch (Exception e)
                    {
                        tcs.SetException(e);
                    }
                };
        }

        private static Task<Response> InvokePreRequestHook(ClampWebContext context, CancellationToken cancellationToken, BeforePipeline pipeline)
        {
            if (pipeline == null)
            {
                return TaskHelpers.GetCompletedTask<Response>(null);
            }

            return pipeline.Invoke(context, cancellationToken);
        }

        private Task InvokePostRequestHook(ClampWebContext context, CancellationToken cancellationToken, AfterPipeline pipeline)
        {
            return pipeline == null ? TaskHelpers.GetCompletedTask() : pipeline.Invoke(context, cancellationToken);
        }

        private void InvokeOnErrorHook(ClampWebContext context, ErrorPipeline pipeline, Exception ex)
        {
            try
            {
                if (pipeline == null)
                {
                    throw new RequestExecutionException(ex);
                }

                var onErrorResult = pipeline.Invoke(context, ex);

                if (onErrorResult == null)
                {
                    throw new RequestExecutionException(ex);
                }

                context.Response = this.negotiator.NegotiateResponse(onErrorResult, context);
            }
            catch (Exception e)
            {
                context.Response = new Response { StatusCode = HttpStatusCode.InternalServerError };
                context.Items[ERROR_KEY] = e.ToString();
                context.Items[ERROR_EXCEPTION] = e;
            }
        }
    }
}
