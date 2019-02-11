using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.Data.Description
{
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

#pragma warning disable 1591
        protected override void OnRemove(int index, object value)
        {
            ObjectDescription ep = (ObjectDescription)value;
            if (ep.Element != null)
            {
                ep.Element.ParentNode.RemoveChild(ep.Element);
                ep.Element = null;
            }
            if (owner != null)
                ep.SetParent(null);
        }

        protected override void OnInsertComplete(int index, object value)
        {
            if (owner != null)
                ((ObjectDescription)value).SetParent(owner);
        }

        protected override void OnSetComplete(int index, object oldValue, object newValue)
        {
            if (owner != null)
            {
                ((ObjectDescription)newValue).SetParent(owner);
                ((ObjectDescription)oldValue).SetParent(null);
            }
        }

        protected override void OnClear()
        {
            if (owner != null)
            {
                foreach (ObjectDescription ob in List)
                    ob.SetParent(null);
            }
        }
#pragma warning restore 1591


        internal void SaveXml(XmlElement parent)
        {
            foreach (ObjectDescription ob in this)
                ob.SaveXml(parent);
        }

        internal void Verify(string location, StringCollection errors)
        {
            int n = 0;
            foreach (ObjectDescription ob in this)
            {
                ob.Verify(location + "[" + n + "]/", errors);
                n++;
            }
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
