using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionNodeSetCollection : ObjectDescriptionCollection<ExtensionNodeSet>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ExtensionNodeSetCollection"/> class.
        /// </summary>
        public ExtensionNodeSetCollection()
        {
        }

        internal ExtensionNodeSetCollection(object owner) : base(owner)
        {
        }
    }
