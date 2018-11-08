using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleDescriptionAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundleDescriptionAttribute"/> class.
        /// </summary>
        /// <param name='description'>
        /// Description of the add-in
        /// </param>
        public BundleDescriptionAttribute(string description)
        {
            Description = description;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundleDescriptionAttribute"/> class.
        /// </summary>
        /// <param name='description'>
        /// Description of the add-in
        /// </param>
        /// <param name='locale'>
        /// Locale of the description (for example, 'en-US', or 'en')
        /// </param>
        public BundleDescriptionAttribute(string description, string locale)
        {
            Description = description;
            Locale = locale;
        }

        /// <value>
        /// Description of the add-in
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Locale of the description (for example, 'en-US', or 'en')
        /// </summary>
        public string Locale { get; set; }
    }
}
