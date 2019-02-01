using Clamp.OSGI.Data.Annotation;

[assembly: Bundle("Clamp.ShanDian", "1.0.0.0")]
[assembly: BundleDescription("安全授权")]
[assembly: BundleAuthor("Aomi")]
[assembly: BundleName("ShanDian")]
[assembly: BundleActivator("Clamp.ShanDian.ShanDianActivator")]
[assembly: BundleDependency("Clamp.Linker", "1.0.0.0")]
[assembly: BundleDependency("Clamp.MUI.Framework", "1.0.0.0")]

