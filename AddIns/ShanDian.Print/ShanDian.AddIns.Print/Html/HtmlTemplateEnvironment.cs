using System;
using System.IO;
using System.Text;
using ShanDian.AddIns.Print.Html.Compiler;

namespace ShanDian.AddIns.Print.Html
{
    public partial class HtmlTemplate
    {
        private class HtmlTemplateEnvironment : IHtmlTemplate
        {
            private readonly HtmlTemplateConfiguration configuration;
            private readonly HtmlTemplateCompiler compiler;

            public HtmlTemplateEnvironment(HtmlTemplateConfiguration configuration)
            {
                if (configuration == null)
                {
                    throw new ArgumentNullException("configuration");
                }

                this.configuration = configuration;
                this.compiler = new HtmlTemplateCompiler(this.configuration);

                this.RegisterBuiltinHelpers();
            }

            public Func<object, string> CompileView(string templatePath)
            {
                var compiledView = this.compiler.CompileView(templatePath);

                return (vm) =>
                {
                    var sb = new StringBuilder();
                    using (var tw = new StringWriter(sb))
                    {
                        compiledView(tw, vm);
                    }
                    return sb.ToString();
                };
            }

            public HtmlTemplateConfiguration Configuration
            {
                get
                {
                    return this.configuration;
                }
            }

            public Action<TextWriter, object> Compile(TextReader template)
            {
                return this.compiler.Compile(template);
            }

            public Func<object, string> Compile(string template)
            {
                using (var reader = new StringReader(template))
                {
                    var compiledTemplate = Compile(reader);

                    return context =>
                    {
                        var builder = new StringBuilder();

                        using (var writer = new StringWriter(builder))
                        {
                            compiledTemplate(writer, context);
                        }

                        return builder.ToString();
                    };
                }
            }

            public void RegisterTemplate(string templateName, Action<TextWriter, object> template)
            {
                lock (this.configuration)
                {
                    this.configuration.RegisteredTemplates.AddOrUpdate(templateName, template);
                }
            }

            public void RegisterTemplate(string templateName, string template)
            {
                using (var reader = new StringReader(template))
                {
                    RegisterTemplate(templateName, Compile(reader));
                }
            }

            public void RegisterHelper(string helperName, HandlebarsHelper helperFunction)
            {
                lock (this.configuration)
                {
                    this.configuration.Helpers.AddOrUpdate(helperName, helperFunction);
                }
            }

            public void RegisterHelper(string helperName, HandlebarsBlockHelper helperFunction)
            {
                lock (this.configuration)
                {
                    this.configuration.BlockHelpers.AddOrUpdate(helperName, helperFunction);
                }
            }

            private void RegisterBuiltinHelpers()
            {
                foreach (var helperDefinition in BuiltinHelpers.Helpers)
                {
                    RegisterHelper(helperDefinition.Key, helperDefinition.Value);
                }
                foreach (var helperDefinition in BuiltinHelpers.BlockHelpers)
                {
                    RegisterHelper(helperDefinition.Key, helperDefinition.Value);
                }
            }
        }
    }
}
