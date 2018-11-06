using Clamp.OSGI.Framework.Conditions;
using Clamp.OSGI.Framework.Description;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    internal class Codon
    {
        private Bundle bundle;
        private string name;
        private AddInProperties properties;
        private ReadOnlyCollection<ICondition> conditions;
        private string path;

        public string Path { get { return path; } }

        public string Name
        {
            get { return name; }
        }

        public Bundle Bundle
        {
            get { return bundle; }
        }

        public string Id
        {
            get { return properties["id"]; }
        }

        public string InsertAfter
        {
            get { return properties["insertafter"]; }
        }

        public string InsertBefore
        {
            get { return properties["insertbefore"]; }
        }

        public string this[string key]
        {
            get { return properties[key]; }
        }

        public AddInProperties Properties
        {
            get { return properties; }
        }

        public ReadOnlyCollection<ICondition> Conditions
        {
            get { return conditions; }
        }

        public Codon(Bundle bundle, string name, string path, AddInProperties properties, ReadOnlyCollection<ICondition> conditions)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            if (properties == null)
                throw new ArgumentNullException("properties");
            this.bundle = bundle;
            this.name = name;
            this.path = path;
            this.properties = properties;
            this.conditions = conditions;
        }

        internal object BuildItem(BuildItemArgs args)
        {
            ExtensionNode extensionNode = this.Bundle.ClampBundle.GetExtensionNode(this.path, this.name);

            if (extensionNode == null)
                throw new FrameworkException("Doozer " + Name + " not found! " + ToString());

            if (!extensionNode.HandleConditions)
            {
                ConditionFailedAction action = AddInCondition.GetFailedAction(args.Conditions, args.Parameter);
                if (action != ConditionFailedAction.Nothing)
                {
                    return null;
                }
            }

            return extensionNode.GetInstance(args.Parameter);
        }

        public override string ToString()
        {
            return String.Format("[Codon: name = {0}, id = {1}, addIn={2}]", name, Id, bundle.FileName);
        }
    }
}
