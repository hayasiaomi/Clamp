using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ObjectDescription : PersistentObject
    {
        private object parent;

        internal void SetParent(object ob)
        {
            parent = ob;
        }

        public BundleDescription ParentAddinDescription
        {
            get
            {
                if (parent is BundleDescription)
                    return (BundleDescription)parent;
                else if (parent is ObjectDescription)
                    return ((ObjectDescription)parent).ParentAddinDescription;
                else
                    return null;
            }
        }

        internal string ParseString(string s)
        {
            var desc = ParentAddinDescription;
            if (desc != null)
                return desc.ParseString(s);
            else
                return s;
        }

    }
}
