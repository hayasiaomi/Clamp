namespace Clamp.Linker.Responses.Negotiation
{
    using System;

    public class Negotiator : IHideObjectMembers
    {
        // TODO - this perhaps should be an interface, along with the view thing above
        // that would then wrap this to give more granular extension point for things like
        // AsNegotiated

        /// <summary>
        /// Initializes a new instance of the <see cref="Negotiator"/> class,
        /// with the provided <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context that should be negotiated.</param>
        public Negotiator(ClampWebContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            this.NegotiationContext = context.NegotiationContext;
        }

        /// <summary>
        /// Gets the <see cref="NegotiationContext"/> used by the negotiator.
        /// </summary>
        /// <value>A <see cref="NegotiationContext"/> instance.</value>
        public NegotiationContext NegotiationContext { get; private set; }
    }
}