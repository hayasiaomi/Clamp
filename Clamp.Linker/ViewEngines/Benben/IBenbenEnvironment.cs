

namespace Clamp.Linker.ViewEngines.Benben
{
    using System;
    using System.IO;
    using Clamp.Linker.ViewEngines.Benben.Compiler;
    using System.Text;

    public interface IBenbenEnvironment
    {
        Action<TextWriter, object> Compile(TextReader template);

        Func<object, string> Compile(string template);

        Func<object, string> CompileView(string templatePath);

        BenbenConfiguration Configuration { get; }

        void RegisterTemplate(string templateName, Action<TextWriter, object> template);

        void RegisterTemplate(string templateName, string template);

        void RegisterHelper(string helperName, BenbenHelper helperFunction);

        void RegisterHelper(string helperName, BenbenBlockHelper helperFunction);
    }
}

