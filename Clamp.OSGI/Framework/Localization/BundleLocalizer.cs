using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    public class BundleLocalizer
    {
        IBundleLocalizer localizer;
        IPluralAddinLocalizer pluralLocalizer;

        internal BundleLocalizer(IBundleLocalizer localizer)
        {
            this.localizer = localizer;
            pluralLocalizer = localizer as IPluralAddinLocalizer;
        }

        /// <summary>
        /// Gets a localized message
        /// </summary>
        /// <param name="msgid">
        /// Message identifier
        /// </param>
        /// <returns>
        /// The localized message
        /// </returns>
        public string GetString(string msgid)
        {
            return localizer.GetString(msgid);
        }

        /// <summary>
        /// Gets a formatted and localized message
        /// </summary>
        /// <param name="msgid">
        /// Message identifier (can contain string format placeholders)
        /// </param>
        /// <param name="args">
        /// Arguments for the string format operation
        /// </param>
        /// <returns>
        /// The formatted and localized string
        /// </returns>
        public string GetString(string msgid, params string[] args)
        {
            return string.Format(localizer.GetString(msgid), args);
        }

        /// <summary>
        /// Gets a formatted and localized message
        /// </summary>
        /// <param name="msgid">
        /// Message identifier (can contain string format placeholders)
        /// </param>
        /// <param name="args">
        /// Arguments for the string format operation
        /// </param>
        /// <returns>
        /// The formatted and localized string
        /// </returns>
        public string GetString(string msgid, params object[] args)
        {
            return string.Format(localizer.GetString(msgid), args);
        }

        /// <summary>
        /// Gets a localized plural form for a message identifier
        /// </summary>
        /// <param name="msgid">
        /// Message identifier for the singular form
        /// </param>
        /// <param name="defaultPlural">
        /// Default result message for the plural form
        /// </param>
        /// <param name="n">
        /// Value count. Determines whether to use singular or plural form.
        /// </param>
        /// <returns>
        /// The localized message
        /// </returns>
        public string GetPluralString(string msgid, string defaultPlural, int n)
        {
            // If the localizer does not support plural forms, just use GetString to
            // get a translation. It is not correct to check 'n' in this case because
            // there is no guarantee that 'defaultPlural' will be translated.

            if (pluralLocalizer != null)
                return pluralLocalizer.GetPluralString(msgid, defaultPlural, n);
            else
                return GetString(msgid);
        }

        /// <summary>
        /// Gets a localized and formatted plural form for a message identifier
        /// </summary>
        /// <param name="singular">
        /// Message identifier for the singular form (can contain string format placeholders)
        /// </param>
        /// <param name="defaultPlural">
        /// Default result message for the plural form (can contain string format placeholders)
        /// </param>
        /// <param name="n">
        /// Value count. Determines whether to use singular or plural form.
        /// </param>
        /// <param name="args">
        /// Arguments for the string format operation
        /// </param>
        /// <returns>
        /// The localized message
        /// </returns>
        public string GetPluralString(string singular, string defaultPlural, int n, params string[] args)
        {
            return string.Format(GetPluralString(singular, defaultPlural, n), args);
        }

        /// <summary>
        /// Gets a localized and formatted plural form for a message identifier
        /// </summary>
        /// <param name="singular">
        /// Message identifier for the singular form (can contain string format placeholders)
        /// </param>
        /// <param name="defaultPlural">
        /// Default result message for the plural form (can contain string format placeholders)
        /// </param>
        /// <param name="n">
        /// Value count. Determines whether to use singular or plural form.
        /// </param>
        /// <param name="args">
        /// Arguments for the string format operation
        /// </param>
        /// <returns>
        /// The localized message
        /// </returns>
        public string GetPluralString(string singular, string defaultPlural, int n, params object[] args)
        {
            return string.Format(GetPluralString(singular, defaultPlural, n), args);
        }
    }
}
