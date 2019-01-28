namespace Clamp.Linker.Conventions
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class DefaultCultureConventions : IConvention
    {
        /// <summary>
        /// Initialise culture conventions
        /// </summary>
        /// <param name="conventions"></param>
        public void Initialise(LinkerConventions conventions)
        {
            ConfigureDefaultConventions(conventions);
        }

        /// <summary>
        /// Determine if culture conventions are valid
        /// </summary>
        /// <param name="conventions"></param>
        /// <returns></returns>
        public Tuple<bool, string> Validate(LinkerConventions conventions)
        {
            if (conventions.CultureConventions == null)
            {
                return Tuple.Create(false, "The culture conventions cannot be null.");
            }

            return (conventions.CultureConventions.Count > 0) ?
               Tuple.Create(true, string.Empty) :
               Tuple.Create(false, "The culture conventions cannot be empty.");
        }

        /// <summary>
        /// Setup default conventions
        /// </summary>
        /// <param name="conventions"></param>
        private static void ConfigureDefaultConventions(LinkerConventions conventions)
        {
            conventions.CultureConventions = new List<Func<LinkerContext, CultureInfo>>
            {
                BuiltInCultureConventions.FormCulture,
                BuiltInCultureConventions.HeaderCulture,
                BuiltInCultureConventions.SessionCulture,
                BuiltInCultureConventions.CookieCulture,
                BuiltInCultureConventions.ThreadCulture
            };
        }
    }
}
