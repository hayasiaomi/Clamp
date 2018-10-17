using Clamp.UIShell.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;

namespace Clamp.UIShell.Localization
{
    [ContentProperty("ResourceName")]
    [MarkupExtensionReturnType(typeof(object))]
    public class LocalizeExtension : MarkupExtension
    {
        public string DefaultValue { set; get; }

        public string ResourceName { set; get; }

        public LocalizeExtension()
        {
        }

        public LocalizeExtension(string resourceName)
            : this(resourceName, null)
        {

        }

        public LocalizeExtension(string resourceName, string defaultValue)
        {
            this.ResourceName = resourceName;
            this.DefaultValue = defaultValue;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            string dataText = SDResources.ResourceManager.GetString(this.ResourceName);

            if (string.IsNullOrWhiteSpace(dataText))
                return this.DefaultValue;
            return dataText;
        }
    }
}
