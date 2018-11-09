using Clamp.OSGI.Framework.Data.Annotation;
using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: BundleRoot("Clamp", "1.0")]
[assembly: BundleDependency("Clamp.Webwork", "1.0")]
[assembly: BundleDependency("Aomi.Main", "1.0")]
