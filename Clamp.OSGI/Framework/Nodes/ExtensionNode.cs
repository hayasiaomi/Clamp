using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public class ExtensionNode
    {
        public string Id { set; get; }

        public bool HandleConditions { get; }

        public virtual object GetInstance(object parameter)
        {
            return null;
        }
    }
}
