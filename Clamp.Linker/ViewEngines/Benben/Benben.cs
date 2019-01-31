namespace Clamp.Linker.ViewEngines.Benben
{
    using System;
    using System.IO;

    public delegate void BenbenHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void BenbenBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public sealed partial class Benben
    {
        // Lazy-load Handlebars environment to ensure thread safety.  See Jon Skeet's excellent article on this for more info. http://csharpindepth.com/Articles/General/Singleton.aspx
        private static readonly Lazy<IBenbenEnvironment> lazy = new Lazy<IBenbenEnvironment>(() => new BenbenEnvironment(new BenbenConfiguration()));

        private static IBenbenEnvironment Instance { get { return lazy.Value; } }

        public static IBenbenEnvironment Create(BenbenConfiguration configuration = null)
        {
            configuration = configuration ?? new BenbenConfiguration();
            return new BenbenEnvironment(configuration);
        }

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return Instance.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            return Instance.Compile(template);
        }
        
        public static Func<object, string> CompileView(string templatePath)
        {
            return Instance.CompileView(templatePath);
        }

        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterTemplate(string templateName, string template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterHelper(string helperName, BenbenHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, BenbenBlockHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration on order to have access in all Helpers and Templates.
        /// </summary>
        public static BenbenConfiguration Configuration
        {
            get { return Instance.Configuration; }
        }
    }
}