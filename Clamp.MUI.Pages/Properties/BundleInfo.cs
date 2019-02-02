using Clamp.OSGI.Data.Annotation;

[assembly: Bundle("Clamp.MUI.Pages", "1.0.0.0")]
[assembly: BundleName("ddd")]
[assembly: BundleActivator("Clamp.MUI.Pages.MUIPagesActivator")]
[assembly: BundleDependency("Clamp.Linker", "1.0.0.0")]
[assembly: BundleDependency("Clamp.MUI.Framework", "1.0.0.0")]

