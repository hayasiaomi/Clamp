namespace Clamp.Linker.ViewEngines.Benben
{
    using Clamp.Linker.Responses;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using SuperSimpleViewEngine;
    using System.IO;

    public class BenbenViewEngine : IViewEngine
    {
        private readonly string[] extensions = new[] { "html", "htm" };

        public IEnumerable<string> Extensions
        {
            get { return extensions; }
        }

        public void Initialize(ViewEngineStartupContext viewEngineStartupContext)
        {

        }

        public Response RenderView(ViewLocationResult viewLocationResult, dynamic model, IRenderContext renderContext)
        {
            Func<object, string> template = renderContext.ViewCache.GetOrAdd(viewLocationResult, x =>
                {
                    using (var reader = viewLocationResult.Contents.Invoke())
                        return Benben.Compile(reader.ReadToEnd());
                });

            Dictionary<string, object> context = new Dictionary<string, object>();

            context.Add("Model", model);
            context.Add("ViewBag", renderContext.Context.ViewBag);

            return new HtmlResponse(statusCode: HttpStatusCode.OK, contents: stream =>
            {
                string html = template(context);

                byte[] buffers = Encoding.UTF8.GetBytes(html);

                stream.Write(buffers, 0, buffers.Length);
            });
        }
    }
}
