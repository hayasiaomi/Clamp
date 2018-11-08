using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Annotation
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
    public class BundlePropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundlePropertyAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the property
        /// </param>
        /// <param name='value'>
        /// Value of the property
        /// </param>
        public BundlePropertyAttribute(string name, string value) : this(name, null, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.BundlePropertyAttribute"/> class.
        /// </summary>
        /// <param name='name'>
        /// Name of the property
        /// </param>
        /// <param name='locale'>
        /// Locale of the property. It can be null if the property is not bound to a locale.
        /// </param>
        /// <param name='value'>
        /// Value of the property
        /// </param>
        public BundlePropertyAttribute(string name, string locale, string value)
        {
            Name = name;
            Locale = locale;
            Value = value;
        }

        /// <summary>
        /// Name of the property
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Locale of the property. It can be null if the property is not bound to a locale.
        /// </summary>
        public string Locale { get; set; }

        /// <summary>
        /// Value of the property
        /// </summary>
        public string Value { get; set; }
    }
}
