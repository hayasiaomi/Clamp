using Clamp.OSGI.Framework.Data.Annotation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

[assembly: Bundle("Aomi.Hello")]
[assembly: BundleDependency("Aomi.Main", "1.0")]
