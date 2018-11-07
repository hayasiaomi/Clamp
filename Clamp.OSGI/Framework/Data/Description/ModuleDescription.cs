using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Data.Description
{
    public class ModuleDescription : ObjectDescription
    {
        private StringCollection ignorePaths;
        private List<string> dataFiles;
        private List<string> assemblies;
        private DependencyCollection dependencies;
        private ExtensionCollection extensions;
        private object parent;

        internal RuntimeBundle RuntimeAddin;

        public DependencyCollection Dependencies
        {
            get
            {
                if (dependencies == null)
                {
                    dependencies = new DependencyCollection(this);
                }
                return dependencies;
            }
        }

        public List<string> Assemblies
        {
            get
            {
                if (assemblies == null)
                {
                    assemblies = new List<string>();
                }
                return assemblies;
            }
        }

        public List<string> DataFiles
        {
            get
            {
                if (dataFiles == null)
                {
                    dataFiles = new List<string>();
                }
                return dataFiles;
            }
        }

        public List<string> AllFiles
        {
            get
            {
                List<string> col = new List<string>();
                foreach (string s in Assemblies)
                    col.Add(s);

                foreach (string d in DataFiles)
                    col.Add(d);

                return col;
            }
        }

        public ExtensionCollection Extensions
        {
            get
            {
                if (extensions == null)
                {
                    extensions = new ExtensionCollection(this);
                }
                return extensions;
            }
        }

        public StringCollection IgnorePaths
        {
            get
            {
                if (ignorePaths == null)
                    ignorePaths = new StringCollection();
                return ignorePaths;
            }
        }

        internal void SetParent(object ob)
        {
            parent = ob;
        }



    }
}
