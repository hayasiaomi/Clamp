using Clamp.Data.Annotation;

[assembly: Bundle("Clamp.MUI.WF", "1.0.0.0")]
[assembly: BundleName("mui")]
[assembly: BundleDependency("Clamp.AppCenter", "1.0.0.0")]
[assembly: BundleDependency("Clamp.MUI.Framework", "1.0.0.0")]
[assembly: BundleActivator("Clamp.MUI.WF.WFActivator")]