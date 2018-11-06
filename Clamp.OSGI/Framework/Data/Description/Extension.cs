using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class Extension : ObjectDescription, IComparable
    {
        private string path;

        public string Path
        {
            get { return path; }
            set { path = value; }
        }

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }
    }
}
