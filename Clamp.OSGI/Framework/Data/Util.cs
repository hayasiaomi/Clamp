using Clamp.OSGI.Framework.Data.Description;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.OSGI.Framework.Data
{
    internal class Util
    {
        public static Assembly LoadAssemblyForReflection(string fileName)
        {
            return Assembly.LoadFile(fileName);
        }

        public static int GetStringHashCode(string s)
        {
            int h = 0;
            int n = 0;
            for (; n < s.Length - 1; n += 2)
            {
                h = unchecked((h << 5) - h + s[n]);
                h = unchecked((h << 5) - h + s[n + 1]);
            }
            if (n < s.Length)
                h = unchecked((h << 5) - h + s[n]);
            return h;
        }

        public static void CheckWrittableFloder(string path)
        {
            string testFile = null;
            int n = 0;
            var random = new Random();
            do
            {
                testFile = Path.Combine(path, random.Next().ToString());
                n++;
            } while (File.Exists(testFile) && n < 100);
            if (n == 100)
                throw new InvalidOperationException($"{path}目录非无法创建文件");

            StreamWriter w = new StreamWriter(testFile);
            w.Close();
            File.Delete(testFile);
        }

        public static void AddDependencies(BundleDescription desc, BundleScanResult scanResult)
        {
            // Not implemented in AddinScanResult to avoid making AddinDescription remotable
            foreach (ModuleDescription mod in desc.AllModules)
            {
                foreach (Dependency dep in mod.Dependencies)
                {
                    BundleDependency adep = dep as BundleDependency;
                    if (adep == null) continue;
                    string depid = Bundle.GetFullId(desc.Namespace, adep.AddinId, adep.Version);
                    scanResult.AddAddinToUpdateRelations(depid);
                }
            }
        }

        public static string GetGacPath(string fullName)
        {
            string[] parts = fullName.Split(',');
            if (parts.Length != 4) return null;
            string name = parts[0].Trim();

            int i = parts[1].IndexOf('=');
            string version = i != -1 ? parts[1].Substring(i + 1).Trim() : parts[1].Trim();

            i = parts[2].IndexOf('=');
            string culture = i != -1 ? parts[2].Substring(i + 1).Trim() : parts[2].Trim();
            if (culture == "neutral") culture = "";

            i = parts[3].IndexOf('=');
            string token = i != -1 ? parts[3].Substring(i + 1).Trim() : parts[3].Trim();

            string versionDirName = version + "_" + culture + "_" + token;


            // .NET 4.0 introduces a new GAC directory structure and location.
            // Assembly version directory names are now prefixed with the CLR version
            // Since there can be different assembly versions for different target CLR runtimes,
            // we now look for the best match, that is, the assembly with the higher CLR version

            var currentVersion = new Version(Environment.Version.Major, Environment.Version.Minor);

            foreach (var gacDir in GetDotNetGacDirectories())
            {
                var asmDir = Path.Combine(gacDir, name);
                if (!Directory.Exists(asmDir))
                    continue;
                Version bestVersion = new Version(0, 0);
                string bestDir = null;
                foreach (var dir in Directory.GetDirectories(asmDir, "v*_" + versionDirName))
                {
                    var dirName = Path.GetFileName(dir);
                    i = dirName.IndexOf('_');
                    Version av;
                    if (Version.TryParse(dirName.Substring(1, i - 1), out av))
                    {
                        if (av == currentVersion)
                            return dir;
                        else if (av < currentVersion && av > bestVersion)
                        {
                            bestDir = dir;
                            bestVersion = av;
                        }
                    }
                }
                if (bestDir != null)
                    return bestDir;
            }

            // Look in the old GAC. There are no CLR prefixes here

            foreach (var gacDir in GetLegacyDotNetGacDirectories())
            {
                var asmDir = Path.Combine(gacDir, name);
                asmDir = Path.Combine(asmDir, versionDirName);
                if (Directory.Exists(asmDir))
                    return asmDir;
            }
            return null;
        }
        internal static bool IsManagedAssembly(string file)
        {
            try
            {
                AssemblyName.GetAssemblyName(file);
                return true;
            }
            catch (BadImageFormatException)
            {
                return false;
            }
        }

        static IEnumerable<string> GetDotNetGacDirectories()
        {
            var winDir = Path.GetFullPath(Environment.SystemDirectory + "\\..");

            string gacDir = winDir + "\\Microsoft.NET\\assembly\\GAC";
            if (Directory.Exists(gacDir))
                yield return gacDir;
            if (Directory.Exists(gacDir + "_32"))
                yield return gacDir + "_32";
            if (Directory.Exists(gacDir + "_64"))
                yield return gacDir + "_64";
            if (Directory.Exists(gacDir + "_MSIL"))
                yield return gacDir + "_MSIL";
        }

        static IEnumerable<string> GetLegacyDotNetGacDirectories()
        {
            var winDir = Path.GetFullPath(Environment.SystemDirectory + "\\..");

            string gacDir = winDir + "\\assembly\\GAC";
            if (Directory.Exists(gacDir))
                yield return gacDir;
            if (Directory.Exists(gacDir + "_32"))
                yield return gacDir + "_32";
            if (Directory.Exists(gacDir + "_64"))
                yield return gacDir + "_64";
            if (Directory.Exists(gacDir + "_MSIL"))
                yield return gacDir + "_MSIL";
        }

    }
}
