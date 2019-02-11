using Clamp.Nodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    public class CustomExtensionAttribute : Attribute
    {
        private string id;
        private string insertBefore;
        private string insertAfter;
        private string path;

        internal const string PathFieldKey = "__path";

        /// <summary>
        /// Identifier of the node
        /// </summary>
        [NodeAttributeAttribute("id")]
        public string Id
        {
            get { return id; }
            set { id = value; }
        }

        /// <summary>
        /// Identifier of the node before which this node has to be placed
        /// </summary>
        [NodeAttributeAttribute("insertbefore")]
        public string InsertBefore
        {
            get { return insertBefore; }
            set { insertBefore = value; }
        }

        /// <summary>
        /// Identifier of the node after which this node has to be placed
        /// </summary>
        [NodeAttributeAttribute("insertafter")]
        public string InsertAfter
        {
            get { return insertAfter; }
            set { insertAfter = value; }
        }

        /// <summary>
        /// Path of the extension point being extended.
        /// </summary>
        /// <remarks>
        /// This property is optional and useful only when there are several extension points which allow
        /// using this custom attribute to define extensions.
        /// </remarks>
        [NodeAttributeAttribute("__path")]
        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        /// <summary>
        /// The extension node bound to this attribute
        /// </summary>
        public ExtensionNode ExtensionNode { get; internal set; }


        /// <summary>
        /// The add-in that registered this extension node.
        /// </summary>
        /// <remarks>
        /// This property provides access to the resources and types of the add-in that created this extension node.
        /// </remarks>
        public RuntimeBundle Bundle
        {
            get { return ExtensionNode?.Bundle; }
        }
    }
}
