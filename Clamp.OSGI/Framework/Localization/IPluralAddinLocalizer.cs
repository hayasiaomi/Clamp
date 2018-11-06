using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    public interface IPluralAddinLocalizer
    {
        /// <summary>
        /// Gets a localized message which may contain plural forms.
        /// </summary>
        /// <returns>
        /// The localized message.
        /// </returns>
        /// <param name='singular'>
        /// Message identifier to use when the specified count is 1.
        /// </param>
        /// <param name='defaultPlural'>
        /// Default message identifier to use when the specified count is not 1.
        /// </param>
        /// <param name='n'>
        /// The count that determines which plural form to use.
        /// </param>
        string GetPluralString(string singular, String defaultPlural, int n);
    }
}
