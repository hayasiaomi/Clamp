using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Linker.Conventions
{
    public class DefaultStaticResourcesConventions : IConvention
    {
        /// <summary>
        /// Initialise any conventions this class "owns".
        /// </summary>
        /// <param name="conventions">Convention object instance.</param>
        public void Initialise(LinkerConventions conventions)
        {
            conventions.StaticContentsConventions = new List<Func<LinkerContext, string, Response>>
            {
                StaticContentConventionBuilder.AddDirectory("Content")
            };
        }

        /// <summary>
        /// Asserts that the conventions that this class "owns" are valid
        /// </summary>
        /// <param name="conventions">Conventions object instance.</param>
        /// <returns>Tuple containing true/false for valid/invalid, and any error messages.</returns>
        public Tuple<bool, string> Validate(LinkerConventions conventions)
        {
            if (conventions.StaticContentsConventions == null)
            {
                return Tuple.Create(false, "The static contents conventions cannot be null.");
            }

            return (conventions.StaticContentsConventions.Count > 0) ? Tuple.Create(true, string.Empty) : Tuple.Create(false, "The static contents conventions cannot be empty.");
        }


    }
}
