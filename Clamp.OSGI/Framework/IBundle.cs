using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.OSGI.Framework
{
    public interface IBundle
    {
        Guid BundleId { get; }

        string Name { get; }

        Version Version { get; }

        void Start();

        void Stop();
    }
}
