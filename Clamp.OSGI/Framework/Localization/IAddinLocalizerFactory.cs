using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework.Localization
{
    public interface IAddinLocalizerFactory
    {
        /// <summary>
        /// Creates a localizer for an add-in.
        /// </summary>
        /// <returns>
        /// The localizer.
        /// </returns>
        /// <param name='addin'>
        /// The add-in for which to create the localizer.
        /// </param>
        /// <param name='element'>
        /// Localizer parameters.
        /// </param>
        IAddinLocalizer CreateLocalizer(RuntimeAddin addin, NodeElement element);
    }
}
