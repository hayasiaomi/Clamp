using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using ShanDain.AIM.DTO;
using System.Security.Cryptography;
using ICSharpCode.SharpZipLib.Zip;

namespace ShanDain.AIM.Service
{
    public class PackManagerService
    {
        protected string PackagePath;
        public const string ADDININFOFILE = "AddIn.xml";
        /// <summary>
        /// 缓存
        /// </summary>
        protected static ConcurrentDictionary<string, ConcurrentDictionary<string, AddInInfo>> PackageInfoCache = new ConcurrentDictionary<string, ConcurrentDictionary<string, AddInInfo>>();
        /// <summary>
        /// 排序缓存
        /// </summary>
        protected List<string> OrderedCache = new List<string>();

        public PackManagerService(string packagePath)
        {
            if(string.IsNullOrEmpty(packagePath))
                throw new ArgumentNullException(nameof(packagePath));
            this.PackagePath = packagePath;
            if (!Directory.Exists(this.PackagePath))
            {
                Directory.CreateDirectory(this.PackagePath);
            }
        }
        /// <summary>
        /// 更新目录列表
        /// </summary>
        /// <returns>第一级:id, 第二级:version</returns>
        public ConcurrentDictionary<string, ConcurrentDictionary<string,  AddInInfo>> UpDatePackage()
        {
            var result = new ConcurrentDictionary<string, ConcurrentDictionary<string, AddInInfo>>();
            var orderedCache = new List<string>();
            var packages = Directory.GetDirectories(this.PackagePath);
            foreach (var package in packages)
            {
                var dic = GetPack(package);
                if (dic.Count > 0)
                {
                    var key = dic.First().Value.AddInId;
                    result[key] = dic;
                    orderedCache.Add(key);
                }
            }
            //更新缓存,排序缓存
            OrderedCache = orderedCache.OrderBy(x => x).ToList();
            PackageInfoCache = result;
            return result;
        }

        private ConcurrentDictionary<string, AddInInfo> GetPack(string path)
        {
            var result = new ConcurrentDictionary<string, AddInInfo>();
            var packages = Directory.GetDirectories(path);
            foreach (var package in packages)
            {
                var addInInfo = GetAddInfo(package, true);
                result[addInInfo.Version.ToString()] = addInInfo;
            }

            return result;
        }

        public static AddInInfo GetAddInfo(string path, bool needCheckFile)
        {
            var addinFile = Directory.GetFiles(path).FirstOrDefault(x => x.Contains(ADDININFOFILE));
            if (string.IsNullOrEmpty(addinFile))
                throw new FileNotFoundException(path);
            return GetAddInfoFromFile(addinFile, path, needCheckFile);
        }

        private static AddInInfo GetAddInfoFromFile(string addinFile, string path, bool needCheckFile)
        {
            var document = new XmlDocument();
            document.Load(addinFile);
            return GetAddInfoFromDocument(document, path, needCheckFile);
        }

        private static AddInInfo GetAddInfoFromDocument(XmlDocument document, string path, bool needCheckFile)
        {
            var result = new AddInInfo();
            var root = document.DocumentElement;
            if (root.Name != "AddIn")
            {
                throw new Exception("root node TagName error:" + root.Name);
            }

            var name = root.GetAttribute("name");
            result.Name = name;
            var author = root.GetAttribute("author");
            result.Auther = author;
            var copyright = root.GetAttribute("copyright");
            result.Copyright = copyright;
            var description = root.GetAttribute("description");
            result.Description = description;

            var manifest = root.GetElementsByTagName("Manifest")[0];

            var info = GetAddInRefInfo(manifest);

            result.AddInId = info.AddInId;
            result.ArtifactId = info.ArtifactId;
            result.GroupId = info.GroupId;
            result.Version = info.Version;

            var dependency = GetDependency(manifest);
            var conflic = GetConflict(manifest);
            result.Dependency.AddRange(dependency);
            result.Conflict.AddRange(conflic);
            if (needCheckFile)
            {
                var fn = $"{path}\\{result.AddInId}_{result.Version}.zip";
                if (!File.Exists(fn))
                {
                    throw new FileNotFoundException(
                        $"packages not found: $Packages/{result.AddInId}_{result.Version}.zip");
                }

                result.CheckSum = CalMd5(fn);
            }

            return result;
        }

        static string CalMd5(string file)
        {
            var md5 = new MD5CryptoServiceProvider();
            var fs = new FileStream(file, FileMode.Open);
            var retval = md5.ComputeHash(fs);
            StringBuilder sc = new StringBuilder();
            for (int i = 0; i < retval.Length; i++)
            {
                sc.Append(retval[i].ToString("x2"));
            }
            Console.WriteLine(sc.ToString());
            md5.Dispose();
            fs.Close();
            return sc.ToString();
        }

        private static RefAddInInfo GetAddInRefInfo(XmlNode node)
        {
            string ArtifactId = null;
            string GroupId = null;
            string AddInId = null;
            VersionInfo Version = null;
            foreach (XmlNode cnode in node.ChildNodes)
            {
                if (cnode.Name == nameof(ArtifactId))
                    ArtifactId = cnode.InnerText?.Trim();
                if (cnode.Name == nameof(GroupId))
                    GroupId = cnode.InnerText?.Trim();
                if (cnode.Name == nameof(AddInId))
                    AddInId = cnode.InnerText?.Trim();
                if (cnode.Name == nameof(Version))
                    Version = VersionInfo.ParseVersion(cnode.InnerText?.Trim());
            }

            if (string.IsNullOrEmpty(ArtifactId))
            {
                throw new Exception(nameof(ArtifactId)+" is empty");
            }
            if (string.IsNullOrEmpty(GroupId))
            {
                throw new Exception(nameof(GroupId) + " is empty");
            }
            if (string.IsNullOrEmpty(AddInId))
            {
                throw new Exception(nameof(AddInId) + " is empty");
            }
            if (Version == null)
            {
                throw new Exception(nameof(Version) + " is empty");
            }

            return new RefAddInInfo(ArtifactId, GroupId, AddInId, Version);
        }

        private static List<RefAddInInfo> GetDependency(XmlNode node)
        {
            return GetAddInRefList(node, "Dependency");
        }
        private static List<RefAddInInfo> GetConflict(XmlNode node)
        {
            return GetAddInRefList(node, "Conflict");
        }

        private static List<RefAddInInfo> GetAddInRefList(XmlNode parentNode, string nodeName)
        {
            XmlNode TargetNode = null;
            foreach (XmlNode childNode in parentNode.ChildNodes)
            {
                if (childNode.Name == nodeName)
                {
                    TargetNode = childNode;
                    break;
                }
            }
            var result = new List<RefAddInInfo>();
            if (TargetNode == null)
                return result;
            foreach (XmlNode childNode in TargetNode.ChildNodes)
            {
                if (childNode.Name == "AddInReference")
                {
                    var addinref = GetAddInRefInfo(childNode);
                    result.Add(addinref);
                }
            }
            return result;
        }
        
        /// <summary>
        /// 获取插件详情
        /// </summary>
        /// <param name="addinId"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public AddInInfo GetAddInInfo(string addinId, string version)
        {
            var dic = GetAddInInfo(addinId);
            if (!dic.TryGetValue(version, out var addin))
                return null;
            return addin;
        }
        /// <summary>
        /// 获取插件信息(包含全版本信息)
        /// </summary>
        /// <param name="addinId"></param>
        /// <returns></returns>
        public ConcurrentDictionary<string, AddInInfo> GetAddInInfo(string addinId)
        {
            var cache = PackageInfoCache;
            cache.TryGetValue(addinId, out var resdult);
            return resdult ?? new ConcurrentDictionary<string, AddInInfo>();
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="start"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public Tuple<List<Tuple<string, Dictionary<string, AddInInfo>>>, int> GetAddInList(string filter, int start, int size)
        {
            var list = OrderedCache;
            List<string> resultKeys = new List<string>();
            int count = 0;
            lock (list)
            {
                var temp = string.IsNullOrEmpty(filter) ? list : list.Where(x => x.Contains(filter)).ToList();
                count = temp.Count;
                resultKeys.AddRange(temp.Skip(start).Take(size));
            }
            var resultValue = new List<Tuple<string, Dictionary<string, AddInInfo>>>();
            foreach (var addinid in resultKeys)
            {
                PackageInfoCache.TryGetValue(addinid, out var addinwithversion);
                if (addinwithversion != null)
                {
                    var versionDic = new Dictionary<string, AddInInfo>();
                    foreach (var item in addinwithversion)
                    {
                        versionDic[item.Key] = item.Value;
                    }
                    resultValue.Add(new Tuple<string, Dictionary<string, AddInInfo>>(addinid, versionDic));
                }
            }

            return new Tuple<List<Tuple<string, Dictionary<string, AddInInfo>>>, int>(resultValue, count);
        }
        /// <summary>
        /// 上传包
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public Tuple<bool, string> CheckAndUnzipFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(Path.GetFileName(filename));
            MemoryStream memstream = null;
            using (var zipStream = new ZipInputStream(new FileStream(filename, FileMode.Open)))
            {
                ZipEntry zipEntry = null;
                while ((zipEntry = zipStream.GetNextEntry()) != null)
                {
                    byte[] buffer = new byte[4096];
                    if (zipEntry.Name == "AddIn.xml")
                    {
                        memstream = new MemoryStream();
                        while (true)
                        {
                            var realRead = zipStream.Read(buffer, 0, buffer.Length);
                            if(realRead == 0)
                                break;
                            memstream.Write(buffer, 0, realRead);
                        }
                        break;
                    }
                }
            }

            if (memstream == null)
                throw new Exception("not found AppIn.xml in " + Path.GetFileName(filename));
            XmlDocument document = new XmlDocument();
            memstream.Seek(0, SeekOrigin.Begin);
            document.Load(memstream);
            var addininfo = GetAddInfoFromDocument(document, "", false);
            var addinid = addininfo.AddInId;
            var version = addininfo.Version.ToString();
            if(Directory.Exists($"{PackagePath}\\{addinid}\\{version}"))
                throw new Exception($"Package:{addinid}:{version} is existed!");

            if (Directory.Exists($"{PackagePath}\\{addinid}"))
            {
                var tempdirs = Directory.GetDirectories($"{PackagePath}\\{addinid}");
                foreach (var path in tempdirs)
                {
                    var lastpath = Path.GetFileName(path);
                    var thisversion = VersionInfo.ParseVersion(lastpath);
                    if (thisversion.CompareTo(addininfo.Version) > 0)
                    {
                        return new Tuple<bool, string>(false, $"{addininfo.AddInId}:{thisversion.ToString()} is bigger then {addininfo.Version}");
                    }
                }

            }

            Directory.CreateDirectory($"{PackagePath}\\{addinid}\\{version}");
            File.Copy(filename, $"{PackagePath}\\{addinid}\\{version}\\{addinid}_{version}.zip");
            using (var fs = new FileStream($"{PackagePath}\\{addinid}\\{version}\\AddIn.xml", FileMode.CreateNew))
            {
                memstream.WriteTo(fs);
            }
            memstream.Close();
            return new Tuple<bool, string>(true, null);
        }
    }
}
