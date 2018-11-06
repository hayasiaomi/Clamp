using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class AddinProperty 
    {
        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Locale of the property. It is null if the property is not localized.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Value of the property.
        /// </summary>
        public string Value { get; set; }
    }
}
