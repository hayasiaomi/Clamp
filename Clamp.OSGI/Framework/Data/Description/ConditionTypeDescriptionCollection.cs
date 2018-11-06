using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ConditionTypeDescriptionCollection : ObjectDescriptionCollection<ConditionTypeDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Mono.Addins.Description.ConditionTypeDescriptionCollection"/> class.
        /// </summary>
        public ConditionTypeDescriptionCollection()
        {
        }

        internal ConditionTypeDescriptionCollection(object owner) : base(owner)
        {
        }

        /// <summary>
        /// Gets the <see cref="Mono.Addins.Description.ConditionTypeDescription"/> at the specified index.
        /// </summary>
        /// <param name='n'>
        /// Index.
        /// </param>
        /// <returns>
        /// The condition.
        /// </returns>
        public ConditionTypeDescription this[int n]
        {
            get { return (ConditionTypeDescription)List[n]; }
        }
    }
}
