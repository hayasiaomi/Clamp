using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Localization
{
    public interface IBundleLocalizer
    {
        /// <summary>
        /// Gets a localized message.
        /// </summary>
        /// <returns>
        /// The localized message.
        /// </returns>
        /// <param name='msgid'>
        /// The message identifier. 
        /// </param>
        string GetString(string msgid);
    }
}
