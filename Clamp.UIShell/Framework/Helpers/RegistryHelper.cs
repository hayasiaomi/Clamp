using IWshRuntimeLibrary;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.UIShell.Framework.Helpers
{
    public static class RegistryHelper
    {
        
        /// <summary>
        /// 配置扫码接口的快捷方式和注册表内容
        /// </summary>
        /// <param name="FilePath">扫码接口EXE完整路径</param>
        /// <param name="project">项目中文名（例如：扫码接口.思迅）</param>
        /// <param name="RestPluginCollect">项目英文名（例如：STTPluginV5）</param>
        /// <returns></returns>
        public static bool DeployRestPluginCollectShortcutAndregedit(string FilePath, string project, string RestPluginCollect)
        {
            try
            {
                FileInfo tempexe = tempexe = new FileInfo(FilePath);
                FileInfo tempuninst = new FileInfo(tempexe.Directory.FullName + "\\uninst.exe");
                if (!System.IO.File.Exists(FilePath))
                {
                    return false;
                }
                if (!System.IO.File.Exists(tempuninst.FullName))
                {
                    return false;
                }
                WriteRegedit(FilePath, RestPluginCollect);
                CreateShortcut("启动" + project, tempexe.FullName, false, project);
                CreateShortcut("启动" + project, tempexe.FullName, true, project);
                CreateShortcut("Uninstall", tempuninst.FullName, true, project);
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLine(string.Format("[扫码接口注册表]方法：{0}，异常：{1}，参数：FilePath：{2}，project：{3}，RestPluginCollect：{4}", "配置扫码接口的快捷方式和注册表内容", ex.ToString(), project, RestPluginCollect));
                return false;
            }
            return true;
        }


        /// <summary>
        /// 在注册表添加安装目录路径和开机自启动
        /// </summary>
        /// <param name="FileFullPath">可执行文件完整路径</param>
        /// <param name="RestPluginCollect">扫码接口项目英文名</param>
        /// <returns></returns>
        public static bool WriteRegedit(string FileFullPath, string RestPluginCollect)
        {

            var file = new FileInfo(FileFullPath);
            if (!file.Exists)
            {
                return false;
            }
            try
            {
                //安装路径
                RegistryKey CurrentUser = Registry.CurrentUser;
                RegistryKey FileApp = CurrentUser.OpenSubKey("Software\\flighty app", true);

                FileApp.SetValue(RestPluginCollect, file.Directory);


                //开机自启动
                RegistryKey LocalMachine = Registry.LocalMachine;
                RegistryKey Run = LocalMachine.OpenSubKey("Software\\Microsoft\\Windows\\CurrentVersion\\Run", true);

                Run.SetValue("RestPluginCollect", FileFullPath);
                return true;
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLine(string.Format("[扫码接口注册表]方法：{0}，异常：{1},参数：FileFullPath：{2},RestPluginCollect：{3}", "在注册表添加安装目录路径和开机自启动",
                    ex.ToString(), FileFullPath, RestPluginCollect));
                return false;
            }
        }

        /// <summary>
        /// 创建快捷方式
        /// </summary>
        /// <param name="shortcutName">快捷方式名称</param>
        /// <param name="targetPath">目标路径</param>
        /// <param name="IsStartMenu">是否创建在开始菜单栏</param>
        /// <param name="project">项目名</param>
        public static void CreateShortcut(string shortcutName, string targetPath, bool IsStartMenu, string project)
        {
            try
            {
                string desktop = "";
                if (IsStartMenu)
                {
                    desktop = Environment.GetFolderPath(Environment.SpecialFolder.Programs);//获取桌面文件夹路径
                    if (!System.IO.Directory.Exists(desktop))
                    {
                        System.IO.Directory.CreateDirectory(desktop);
                    }
                    if (!System.IO.Directory.Exists(desktop + "\\" + project))
                    {
                        System.IO.Directory.CreateDirectory(desktop + "\\" + project);
                    }
                    desktop = desktop + "\\" + project;
                }
                else
                {
                    desktop = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);//获取桌面文件夹路径
                    if (!System.IO.Directory.Exists(desktop))
                    {
                        System.IO.Directory.CreateDirectory(desktop);
                    }
                }

                string shortcutPath = Path.Combine(desktop, string.Format("{0}.lnk", shortcutName));
                WshShell shell = new WshShell();
                IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
                shortcut.TargetPath = targetPath;//指定目标路径
                shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
                shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
                shortcut.Description = "扫码接口";//设置备注
                shortcut.IconLocation = targetPath;//设置图标路径
                shortcut.Save();//保存快捷方式
            }
            catch (Exception ex)
            {
                DebugHelper.WriteLine(string.Format("[扫码接口注册表]方法：{0}，异常：{1}，参数：shortcutName：{2}，targetPath：{3}，IsStartMenu：{4}，project：{5}", "创建快捷方式",
                    ex.ToString(), shortcutName, targetPath, IsStartMenu, project));
            }
        }
    }
}
