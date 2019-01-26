namespace Clamp.Linker.Conventions
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Wires up the default conventions for the accept header coercion
    /// </summary>
    public class DefaultAcceptHeaderCoercionConventions : IConvention
    {
        public void Initialise(WebworkConventions conventions)
        {
            this.ConfigureDefaultConventions(conventions);
        }

        public Tuple<bool, string> Validate(WebworkConventions conventions)
        {
            if (conventions.AcceptHeaderCoercionConventions == null)
            {
                return Tuple.Create(false, "The accept header coercion conventions cannot be null.");
            }

            return Tuple.Create(true, string.Empty);
        }

        private void ConfigureDefaultConventions(WebworkConventions conventions)
        {
            conventions.AcceptHeaderCoercionConventions = new List<Func<IEnumerable<Tuple<string, decimal>>, ClampWebContext, IEnumerable<Tuple<string, decimal>>>>(2)
                                                              {
                                                                  BuiltInAcceptHeaderCoercions.BoostHtml,
                                                                  BuiltInAcceptHeaderCoercions.CoerceBlankAcceptHeader,
                                                              };
        }
    }
}