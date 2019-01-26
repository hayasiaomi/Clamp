namespace Clamp.Linker.Conventions
{
    using System;
    using System.Collections.Generic;
    using ViewEngines;

    /// <summary>
    /// Defines the default static contents conventions.
    /// </summary>
    public class DefaultViewLocationConventions : IConvention
    {
        /// <summary>
        /// Initialise any conventions this class "owns".
        /// </summary>
        /// <param name="conventions">Convention object instance.</param>
        public void Initialise(WebworkConventions conventions)
        {
            ConfigureViewLocationConventions(conventions);
        }

        /// <summary>
        /// Asserts that the conventions that this class "owns" are valid.
        /// </summary>
        /// <param name="conventions">Conventions object instance.</param>
        /// <returns>Tuple containing true/false for valid/invalid, and any error messages.</returns>
        public Tuple<bool, string> Validate(WebworkConventions conventions)
        {
            if (conventions.ViewLocationConventions == null)
            {
                return Tuple.Create(false, "The view conventions cannot be null.");
            }

            return (conventions.ViewLocationConventions.Count > 0) ?
                Tuple.Create(true, string.Empty) :
                Tuple.Create(false, "The view conventions cannot be empty.");
        }

        private static void ConfigureViewLocationConventions(WebworkConventions conventions)
        {
            conventions.ViewLocationConventions = new List<Func<string, object, ViewLocationContext, string>>
            {
                // 1 Handles: views / *modulename* / *viewname*
                (viewName, model, viewLocationContext) => {
                    return string.Concat("pages/", viewLocationContext.ModuleName, "/", viewName);
                },

                (viewName, model, viewLocationContext) => {
                    return string.Concat("pages/", viewLocationContext.ModuleName, "/", viewName, "-", viewLocationContext.Context.Culture);
                },

                // 2 Handles: views / *viewname*
                (viewName, model, viewLocationContext) => {
                    return string.Concat("pages/", viewName);
                },

                (viewName, model, viewLocationContext) => {
                    return string.Concat("pages/", viewName, "-", viewLocationContext.Context.Culture);
                },
            };
        }
    }
}