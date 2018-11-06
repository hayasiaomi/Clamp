using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    /// <summary>
	/// Base class for add-in description collections.
	/// </summary>
	public class ObjectDescriptionCollection : CollectionBase
    {
        object owner;

        internal ObjectDescriptionCollection(object owner)
        {
            this.owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ObjectDescriptionCollection"/> class.
        /// </summary>
        public ObjectDescriptionCollection()
        {
        }

        /// <summary>
        /// Add an object.
        /// </summary>
        /// <param name='ep'>
        /// The object.
        /// </param>
        public void Add(ObjectDescription ep)
        {
            List.Add(ep);
        }

        /// <summary>
        /// Adds a collection of objects.
        /// </summary>
        /// <param name='collection'>
        /// The objects to add.
        /// </param>
        public void AddRange(ObjectDescriptionCollection collection)
        {
            foreach (ObjectDescription ob in collection)
                Add(ob);
        }

        /// <summary>
        /// Insert an object.
        /// </summary>
        /// <param name='index'>
        /// Insertion index.
        /// </param>
        /// <param name='ep'>
        /// The object.
        /// </param>
        public void Insert(int index, ObjectDescription ep)
        {
            List.Insert(index, ep);
        }

        /// <summary>
        /// Removes an object.
        /// </summary>
        /// <param name='ep'>
        /// Object to remove.
        /// </param>
        public void Remove(ObjectDescription ep)
        {
            List.Remove(ep);
        }

        /// <summary>
        /// Checks if an object is present in the collection.
        /// </summary>
        /// <param name='ob'>
        /// Object to check.
        /// </param>
        public bool Contains(ObjectDescription ob)
        {
            return List.Contains(ob);
        }
    }

    /// <summary>
    /// Base class for add-in description collections.
    /// </summary>
    public class ObjectDescriptionCollection<T> : ObjectDescriptionCollection, IEnumerable<T> where T : ObjectDescription
    {
        internal ObjectDescriptionCollection()
        {
        }

        internal ObjectDescriptionCollection(object owner) : base(owner)
        {
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return Enumerable.Cast<T>(InnerList).GetEnumerator();
        }
    }
}
