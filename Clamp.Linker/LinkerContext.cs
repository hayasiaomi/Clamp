
namespace Clamp.Linker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Clamp.Linker.Diagnostics;
    using Clamp.Linker.Responses.Negotiation;
    using Clamp.Linker.Routing;
    using Clamp.Linker.Security;
    using Clamp.Linker.Validation;
    using System.Globalization;
    using Clamp;

    /// <summary>
    /// 连接器上下文
    /// </summary>
    public sealed class LinkerContext : IDisposable
    {
        private Request request;

        private ModelValidationResult modelValidationResult;

        public LinkerContext()
        {
            this.Items = new Dictionary<string, object>();
            this.Trace = new DefaultRequestTrace();
            this.ViewBag = new DynamicDictionary();
            this.NegotiationContext = new NegotiationContext();

            this.ControlPanelEnabled = true;
        }

        /// <summary>
        /// Gets the dictionary for storage of per-request items. Disposable items will be disposed when the context is.
        /// </summary>
        public IDictionary<string, object> Items { get; private set; }

        /// <summary>
        /// Gets or sets the resolved route
        /// </summary>
        public Route ResolvedRoute { get; set; }

        /// <summary>
        /// Gets or sets the parameters for the resolved route
        /// </summary>
        public dynamic Parameters { get; set; }

        /// <summary>
        /// 当前请求
        /// </summary>
        public Request Request
        {
            get
            {
                return this.request;
            }

            set
            {
                this.request = value;
                this.Trace.RequestData = value;
            }
        }

        /// <summary>
        /// 当前请求的Bundle名称
        /// </summary>
        public string BundleName { set; get; }

        /// <summary>
        /// 当前请求所在的Bundle
        /// </summary>
        public RuntimeBundle RuntimeBundle { set; get; }

        /// <summary>
        /// Gets or sets the outgoing response
        /// </summary>
        public Response Response { get; set; }

        /// <summary>
        /// Gets or sets the current user
        /// </summary>
        public IUserIdentity CurrentUser { get; set; }

        /// <summary>
        /// Diagnostic request tracing
        /// </summary>
        public IRequestTrace Trace { get; set; }

        /// <summary>
        /// Gets a value indicating whether control panel access is enabled for this request
        /// </summary>
        public bool ControlPanelEnabled { get; private set; }

        /// <summary>
        /// Non-model specific data for rendering in the response
        /// </summary>
        public dynamic ViewBag { get; private set; }

        /// <summary>
        /// Gets or sets the model validation result.
        /// </summary>
        public ModelValidationResult ModelValidationResult
        {
            get { return this.modelValidationResult ?? (this.modelValidationResult = new ModelValidationResult()); }
            set { this.modelValidationResult = value; }
        }

        /// <summary>
        /// Gets or sets the context's culture
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Context of content negotiation (if relevant)
        /// </summary>
        public NegotiationContext NegotiationContext { get; set; }

        /// <summary>
        /// Gets or sets the dynamic object used to locate text resources.
        /// </summary>
        public dynamic Text { get; set; }

        /// <summary>
        /// Disposes any disposable items in the <see cref="Items"/> dictionary.
        /// </summary>
        public void Dispose()
        {
            foreach (var disposableItem in this.Items.Values.OfType<IDisposable>())
            {
                disposableItem.Dispose();
            }

            this.Items.Clear();

            if (this.request != null)
            {
                ((IDisposable)this.request).Dispose();
            }

            if (this.Response != null)
            {
                this.Response.Dispose();
            }
        }
    }
}
