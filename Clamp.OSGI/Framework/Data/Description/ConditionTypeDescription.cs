using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public sealed class ConditionTypeDescription : ObjectDescription
    {
        string id;
        string typeName;
        string addinId;
        string description;

        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ConditionTypeDescription"/> class.
        /// </summary>
        public ConditionTypeDescription()
        {
        }

        /// <summary>
        /// Copies data from another condition type definition
        /// </summary>
        /// <param name='cond'>
        /// Condition from which to copy
        /// </param>
        public void CopyFrom(ConditionTypeDescription cond)
        {
            id = cond.id;
            typeName = cond.typeName;
            addinId = cond.AddinId;
            description = cond.description;
        }


        /// <summary>
        /// Gets or sets the identifier of the condition type
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public string Id
        {
            get { return id != null ? id : string.Empty; }
            set { id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the type that implements the condition
        /// </summary>
        /// <value>
        /// The name of the type.
        /// </value>
        public string TypeName
        {
            get { return typeName != null ? typeName : string.Empty; }
            set { typeName = value; }
        }

        /// <summary>
        /// Gets or sets the description of the condition.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description
        {
            get { return description != null ? description : string.Empty; }
            set { description = value; }
        }

        internal string AddinId
        {
            get { return addinId; }
            set { addinId = value; }
        }
    }
}
