using Clamp.OSGI.Data.Annotation;

[assembly: Bundle("Clamp.MUI.EasyUI", "1.0.0.0")]
[assembly: BundleName("admin")]
[assembly: BundleActivator("Clamp.MUI.EasyUI.EasyUIActivator")]
[assembly: BundleDependency("Clamp.Linker", "1.0.0.0")]
[assembly: BundleDependency("Clamp.MUI.Framework", "1.0.0.0")]

