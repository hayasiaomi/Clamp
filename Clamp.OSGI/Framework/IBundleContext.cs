using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IBundleContext
    {
        IBundle GetBundle(long id);
        IBundle[] GetBundles();

        void AddServiceListener(IServiceListener listener);

        void RemoveServiceListener(IServiceListener listener);

        void RegisterService(string clazz )
    }
}
