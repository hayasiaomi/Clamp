using ClampMVC;
using System;
using System.IO;
using System.Reflection;

namespace Clamp.AppCenter
{


    public class FileSystemRootPathProvider : IRootPathProvider
    {
        public string GetRootPath()
        {
            var assembly = Assembly.GetEntryAssembly();

            return assembly != null ? 
                Path.GetDirectoryName(assembly.Location) :
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        }
    }
}
