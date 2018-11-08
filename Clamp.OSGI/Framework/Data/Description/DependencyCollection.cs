using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class DependencyCollection : ObjectDescriptionCollection<Dependency>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Bundles.Description.DependencyCollection"/> class.
        /// </summary>
        public DependencyCollection()
        {
        }

        internal DependencyCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Bundles.Description.Dependency"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// The idnex.
        /// </param>
        public Dependency this[int n]
        {
            get { return (Dependency)List[n]; }
        }

        /// <summary>
        /// Adds a dependency to the collection
        /// </summary>
        /// <param name='dep'>
        /// The dependency to add.
        /// </param>
        public void Add(Dependency dep)
        {
            List.Add(dep);
        }

        /// <summary>
        /// Remove the specified dependency.
        /// </summary>
        /// <param name='dep'>
        /// Dependency to remove.
        /// </param>
        public void Remove(Dependency dep)
        {
            List.Remove(dep);
        }
    }
}
