namespace Clamp.AppCenter.Medium
{
    using System.Collections.Generic;
#pragma warning disable CS0246 // 未能找到类型或命名空间名“ModelBinding”(是否缺少 using 指令或程序集引用?)
    using ModelBinding;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“ModelBinding”(是否缺少 using 指令或程序集引用?)
    using ClampMVC.Responses;
    using ClampMVC.Routing;

#pragma warning disable CS0535 // “DiagnosticsModuleBuilder”不实现接口成员“IWebworkModuleBuilder.BuildModule(IController, ClampWebContext)”
    internal class DiagnosticsModuleBuilder : IWebworkModuleBuilder
#pragma warning restore CS0535 // “DiagnosticsModuleBuilder”不实现接口成员“IWebworkModuleBuilder.BuildModule(IController, ClampWebContext)”
    {
#pragma warning disable CS0246 // 未能找到类型或命名空间名“IRootPathProvider”(是否缺少 using 指令或程序集引用?)
        private readonly IRootPathProvider rootPathProvider;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IRootPathProvider”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“ISerializer”(是否缺少 using 指令或程序集引用?)
        private readonly IEnumerable<ISerializer> serializers;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“ISerializer”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“IModelBinderLocator”(是否缺少 using 指令或程序集引用?)
        private readonly IModelBinderLocator modelBinderLocator;
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IModelBinderLocator”(是否缺少 using 指令或程序集引用?)

#pragma warning disable CS0246 // 未能找到类型或命名空间名“IRootPathProvider”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“IModelBinderLocator”(是否缺少 using 指令或程序集引用?)
        public DiagnosticsModuleBuilder(IRootPathProvider rootPathProvider, IModelBinderLocator modelBinderLocator)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IModelBinderLocator”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IRootPathProvider”(是否缺少 using 指令或程序集引用?)
        {
            this.rootPathProvider = rootPathProvider;
            this.serializers = new[] { new DefaultJsonSerializer { RetainCasing = false } };
            this.modelBinderLocator = modelBinderLocator;
        }

#pragma warning disable CS0246 // 未能找到类型或命名空间名“ClampWebContext”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“IController”(是否缺少 using 指令或程序集引用?)
#pragma warning disable CS0246 // 未能找到类型或命名空间名“IController”(是否缺少 using 指令或程序集引用?)
        /// <summary>
        /// Builds a fully configured <see cref="IController"/> instance, based upon the provided <paramref name="module"/>.
        /// </summary>
        /// <param name="module">The <see cref="IController"/> that should be configured.</param>
        /// <param name="context">The current request context.</param>
        /// <returns>A fully configured <see cref="IController"/> instance.</returns>
        public IController BuildModule(IController module, ClampWebContext context)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IController”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“IController”(是否缺少 using 指令或程序集引用?)
#pragma warning restore CS0246 // 未能找到类型或命名空间名“ClampWebContext”(是否缺少 using 指令或程序集引用?)
        {
            // Currently we don't connect view location, binders etc.
            module.Context = context;
            module.Response = new DefaultResponseFormatter(rootPathProvider, context, serializers);
            module.ModelBinderLocator = this.modelBinderLocator;

            module.After = new AfterPipeline();
            module.Before = new BeforePipeline();
            module.OnError = new ErrorPipeline();

            return module;
        }
    }
}