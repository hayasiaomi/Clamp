using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Nodes
{
    public abstract class InstanceExtensionNode : ExtensionNode
    {
        object cachedInstance;

        /// <summary>
        /// Gets the extension object declared by this node
        /// </summary>
        /// <param name="expectedType">
        /// Expected object type. An exception will be thrown if the object is not an instance of the specified type.
        /// </param>
        /// <returns>
        /// The extension object
        /// </returns>
        /// <remarks>
        /// The extension object is cached and the same instance will be returned at every call.
        /// </remarks>
        public object GetInstance(Type expectedType)
        {
            object ob = GetInstance();
            if (!expectedType.IsInstanceOfType(ob))
                throw new InvalidOperationException(string.Format("Expected subclass of type '{0}'. Found '{1}'.", expectedType, ob.GetType()));
            return ob;
        }

        /// <summary>
        /// Gets the extension object declared by this node
        /// </summary>
        /// <returns>
        /// The extension object
        /// </returns>
        /// <remarks>
        /// The extension object is cached and the same instance will be returned at every call.
        /// </remarks>
        public object GetInstance()
        {
            if (cachedInstance == null)
                cachedInstance = CreateInstance();
            return cachedInstance;
        }

        /// <summary>
        /// Creates a new extension object
        /// </summary>
        /// <param name="expectedType">
        /// Expected object type. An exception will be thrown if the object is not an instance of the specified type.
        /// </param>
        /// <returns>
        /// The extension object
        /// </returns>
        public object CreateInstance(Type expectedType)
        {
            object ob = CreateInstance();
            if (!expectedType.IsInstanceOfType(ob))
                throw new InvalidOperationException(string.Format("Expected subclass of type '{0}'. Found '{1}'.", expectedType, ob.GetType()));
            return ob;
        }

        /// <summary>
        /// Creates a new extension object
        /// </summary>
        /// <returns>
        /// The extension object
        /// </returns>
        public abstract object CreateInstance();
    }
}
