using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    public class ModuleCollection : ObjectDescriptionCollection<ModuleDescription>
    {
        public ModuleCollection()
        {
        }

        internal ModuleCollection(object owner) : base(owner)
        {
        }


        public ModuleDescription this[int n]
        {
            get { return (ModuleDescription)List[n]; }
        }
    }
}
