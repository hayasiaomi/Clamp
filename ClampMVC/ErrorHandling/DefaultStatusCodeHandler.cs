namespace Clamp.Linker.ErrorHandling
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using IO;
    using Clamp.Linker.Extensions;
    using Clamp.Linker.Responses.Negotiation;
    using Clamp.Linker.ViewEngines;

    /// <summary>
    /// Default error handler
    /// </summary>
    public class DefaultStatusCodeHandler : IStatusCodeHandler
    {
        private const string DisableErrorTracesTrueMessage = "Error details are currently disabled. Please set <code>StaticConfiguration.DisableErrorTraces = false;</code> to enable.";

        private readonly IDictionary<HttpStatusCode, string> errorMessages;
        private readonly IDictionary<HttpStatusCode, string> errorPages;
        private readonly IResponseNegotiator responseNegotiator;
        private readonly HttpStatusCode[] supportedStatusCodes = { HttpStatusCode.NotFound, HttpStatusCode.InternalServerError };

        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultStatusCodeHandler"/> type.
        /// </summary>
        /// <param name="responseNegotiator">The response negotiator.</param>
        public DefaultStatusCodeHandler(IResponseNegotiator responseNegotiator)
        {
            this.errorMessages = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound, "The resource you have requested cannot be found." },
                { HttpStatusCode.InternalServerError, "Something went horribly, horribly wrong while servicing your request." }
            };

            this.errorPages = new Dictionary<HttpStatusCode, string>
            {
                { HttpStatusCode.NotFound, LoadResource("404.html") },
                { HttpStatusCode.InternalServerError, LoadResource("500.html") }
            };

            this.responseNegotiator = responseNegotiator;
        }

        /// <summary>
        /// Whether the status code is handled
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="LinkerContext"/> instance of the current request.</param>
        /// <returns>True if handled, false otherwise</returns>
        public bool HandlesStatusCode(HttpStatusCode statusCode, LinkerContext context)
        {
            return this.supportedStatusCodes.Any(s => s == statusCode);
        }

        /// <summary>
        /// Handle the error code
        /// </summary>
        /// <param name="statusCode">Status code</param>
        /// <param name="context">The <see cref="LinkerContext"/> instance of the current request.</param>
        /// <returns>Nancy Response</returns>
        public void Handle(HttpStatusCode statusCode, LinkerContext context)
        {
            if (context.Response != null && context.Response.Contents != null && !ReferenceEquals(context.Response.Contents, Response.NoBody))
            {
                return;
            }

            if (!this.errorMessages.ContainsKey(statusCode) || !this.errorPages.ContainsKey(statusCode))
            {
                return;
            }

            Response existingResponse = null;

            if (context.Response != null)
            {
                existingResponse = context.Response;
            }

            // Reset negotiation context to avoid any downstream cast exceptions
            // from swapping a view model with a `DefaultStatusCodeHandlerResult`
            context.NegotiationContext = new NegotiationContext();

            var result = new DefaultStatusCodeHandlerResult(statusCode, this.errorMessages[statusCode], StaticConfiguration.DisableErrorTraces ? DisableErrorTracesTrueMessage : context.GetExceptionDetails());
            try
            {
                context.Response = this.responseNegotiator.NegotiateResponse(result, context);
                context.Response.StatusCode = statusCode;

                if (existingResponse != null)
                {
                    context.Response.ReasonPhrase = existingResponse.ReasonPhrase;
                }
                return;
            }
            catch (ViewNotFoundException)
            {
                // No view will be found for `DefaultStatusCodeHandlerResult`
                // because it is rendered from embedded resources below
            }

            this.ModifyResponse(statusCode, context, result);
        }

        private void ModifyResponse(HttpStatusCode statusCode, LinkerContext context, DefaultStatusCodeHandlerResult result)
        {
            if (context.Response == null)
            {
                context.Response = new Response { StatusCode = statusCode };
            }

            var contents = this.errorPages[statusCode];

            if (!string.IsNullOrEmpty(contents))
            {
                contents = contents.Replace("[DETAILS]", result.Details);
            }   
                
            context.Response.ContentType = "text/html";
            context.Response.Contents = s =>
            {
                using (var writer = new StreamWriter(new UnclosableStreamWrapper(s), Encoding.UTF8))
                {
                    writer.Write(contents);
                }
            };
        }

        private static string LoadResource(string filename)
        {
            var resourceStream = typeof(ILinkerEngine).Assembly.GetManifestResourceStream(string.Format("Nancy.ErrorHandling.Resources.{0}", filename));

            if (resourceStream == null)
            {
                return string.Empty;
            }

            using (var reader = new StreamReader(resourceStream))
            {
                return reader.ReadToEnd();
            }
        }

        internal class DefaultStatusCodeHandlerResult
        {
            public DefaultStatusCodeHandlerResult(HttpStatusCode statusCode, string message, string details)
            {
                this.StatusCode = statusCode;
                this.Message = message;
                this.Details = details;
            }

            public HttpStatusCode StatusCode { get; private set; }

            public string Message { get; private set; }

            public string Details { get; private set; }
        }
    }    
}