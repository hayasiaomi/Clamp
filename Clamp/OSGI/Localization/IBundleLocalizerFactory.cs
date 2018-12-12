using Clamp.OSGI.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Localization
{
    public interface IBundleLocalizerFactory
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
        IBundleLocalizer CreateLocalizer(RuntimeBundle addin, NodeElement element);
    }
}
