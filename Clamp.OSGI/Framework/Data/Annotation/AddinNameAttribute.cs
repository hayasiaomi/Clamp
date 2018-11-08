using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundleNameAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundleNameAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the add-in
        /// </param>
        public BundleNameAttribute(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundleNameAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the add-in
        /// </param>
        /// <param name='locale'>
        /// Locale of the name (for example, 'en-US', or 'en')
        /// </param>
        public BundleNameAttribute(string name, string locale)
        {
            Name = name;
            Locale = locale;
        }

        /// <value>
        /// Name of the add-in
        /// </value>
        public string Name { get; set; }

        /// <summary>
        /// Locale of the name (for example, 'en-US', or 'en')
        /// </summary>
        public string Locale { get; set; }
    }
}
