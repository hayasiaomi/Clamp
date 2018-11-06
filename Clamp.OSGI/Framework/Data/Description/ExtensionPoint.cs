using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ExtensionPoint : ObjectDescription
    {
        private ExtensionNodeSet nodeSet;

        string path;
        string name;
        string description;

        public ExtensionNodeSet NodeSet
        {
            get
            {
                if (nodeSet == null)
                {
                    nodeSet = new ExtensionNodeSet();
                    nodeSet.SetParent(this);
                }
                return nodeSet;
            }
        }

        internal void SetExtensionsAddinId(string addinId)
        {
            //NodeSet.SetExtensionsAddinId(addinId);
            //foreach (ConditionTypeDescription cond in Conditions)
            //    cond.AddinId = addinId;
            //Addins.Add(addinId);
        }

        public string Path
        {
            get { return path != null ? path : string.Empty; }
            set { path = value; }
        }
    }
}
