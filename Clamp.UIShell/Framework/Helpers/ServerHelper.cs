using Clamp.UIShell.Framework.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace Clamp.UIShell.Framework.Helpers
{
    public class ServerHelper
    {
        public static string GetServerVersion()
        {
            string hydraWorkAreaDll = System.IO.Path.Combine(SDShell.SDRootPath, "Hydra.WorkArea.dll");

            if (File.Exists(hydraWorkAreaDll))
            {
                return FileVersionInfo.GetVersionInfo(hydraWorkAreaDll).FileVersion;

            }
            return string.Empty;
        }


        public static string GetRestSystem()
        {
            string scanCodeConfig = GetScanCodeConfigPath();

            if (!string.IsNullOrWhiteSpace(scanCodeConfig))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(scanCodeConfig);

                XmlElement root = xmlDocument.DocumentElement;

                if (root != null)
                {
                    XmlNode mdsNode = root.SelectSingleNode("MeakerDiningSetting");

                    if (mdsNode != null)
                    {
                        XmlNode rvNode = mdsNode.SelectSingleNode("RestaurantVersion");
                        if (rvNode != null)
                            return rvNode.InnerText;
                    }
                }
            }

            return string.Empty;
        }

        public static bool GetAutoCheckout()
        {
            string scanCodeConfig = GetScanCodeConfigPath();

            if (!string.IsNullOrWhiteSpace(scanCodeConfig))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(scanCodeConfig);

                XmlElement root = xmlDocument.DocumentElement;

                if (root != null)
                {
                    XmlNode mdsNode = root.SelectSingleNode("MeakerDiningSetting");

                    if (mdsNode != null)
                    {
                        XmlNode rvNode = mdsNode.SelectSingleNode("IssuedPayBillInfo");
                        if (rvNode != null)
                        {
                            string rvValue = rvNode.InnerText;

                            if (!string.IsNullOrWhiteSpace(rvValue) && rvValue == "1")
                            {
                                return true;
                            }

                            return false;
                        }
                    }
                }
            }

            return false;
        }


        public static string GetScanCodeConfigPath()
        {
            string[] sdsDirectoryNames = Directory.GetDirectories(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "config", SearchOption.TopDirectoryOnly);

            if (sdsDirectoryNames == null || sdsDirectoryNames.Length <= 0 || sdsDirectoryNames.Length > 1)
            {
                DebugHelper.WriteLine("找不到目录或是存在多个config");

                return null;
            }

            string scanCodeConfig = System.IO.Path.Combine(sdsDirectoryNames[0], "ScanCodeConfig.xml");

            if (!File.Exists(scanCodeConfig))
            {
                DebugHelper.WriteLine("找不到ScanCodeConfig.xml文件");

                return null;
            }

            return scanCodeConfig;
        }

        public static string GetServerConfigPath()
        {
            string[] sdsDirectoryNames = Directory.GetDirectories(Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).Parent.FullName, "config", SearchOption.TopDirectoryOnly);

            if (sdsDirectoryNames == null || sdsDirectoryNames.Length <= 0 || sdsDirectoryNames.Length > 1)
            {
                DebugHelper.WriteLine("找不到目录或是存在多个config");

                return null;
            }

            string serverConfig = System.IO.Path.Combine(sdsDirectoryNames[0], "ServerConfig.xml");

            if (!File.Exists(serverConfig))
            {
                DebugHelper.WriteLine("找不到ScanCodeConfig.xml文件");

                return null;
            }

            return serverConfig;
        }


        public static bool IsNewRestId()
        {
            string serverConfig = GetServerConfigPath();

            if (!string.IsNullOrWhiteSpace(serverConfig))
            {
                XmlDocument xmlDocument = new XmlDocument();
                xmlDocument.Load(serverConfig);

                XmlElement serverSettingEle = (XmlElement)xmlDocument.SelectSingleNode("root/ServerSetting");

                if (serverSettingEle != null)
                {
                    XmlNode isNewRestIdNode = serverSettingEle.SelectSingleNode("IsNewRestId");

                    if (isNewRestIdNode != null)
                    {
                        string value = isNewRestIdNode.InnerText;
                        if (!string.IsNullOrWhiteSpace(value))
                        {
                            return Convert.ToBoolean(value);
                        }
                    }
                }
            }
            return false;
        }



    }
}
