using System;
using System.IO;

namespace ShanDian.AddIns.Print.Html
{
    public delegate void HandlebarsHelper(TextWriter output, dynamic context, params object[] arguments);
    public delegate void HandlebarsBlockHelper(TextWriter output, HelperOptions options, dynamic context, params object[] arguments);

    public sealed partial class HtmlTemplate
    {
        private static readonly Lazy<IHtmlTemplate> lazy = new Lazy<IHtmlTemplate>(() => new HtmlTemplateEnvironment(new HtmlTemplateConfiguration()));

        private static IHtmlTemplate Instance { get { return lazy.Value; } }

        public static IHtmlTemplate Create(HtmlTemplateConfiguration configuration = null)
        {
            configuration = configuration ?? new HtmlTemplateConfiguration();
            return new HtmlTemplateEnvironment(configuration);
        }

        public static Action<TextWriter, object> Compile(TextReader template)
        {
            return Instance.Compile(template);
        }

        public static Func<object, string> Compile(string template)
        {
            return Instance.Compile(template);
        }

        public static void RegisterTemplate(string templateName, Action<TextWriter, object> template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterTemplate(string templateName, string template)
        {
            Instance.RegisterTemplate(templateName, template);
        }

        public static void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        public static void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
        {
            Instance.RegisterHelper(helperName, helperFunction);
        }

        /// <summary>
        /// Expose the configuration on order to have access in all Helpers and Templates.
        /// </summary>
        public static HtmlTemplateConfiguration Configuration
        {
            get { return Instance.Configuration; }
        }
    }
}