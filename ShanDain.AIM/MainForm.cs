using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using ShanDain.AIM.DTO;
using ICSharpCode.SharpZipLib.Zip;
using ShanDain.AIM.DownloadUnit;
using System.Diagnostics;
using ShanDain.AIM.Helper;
using ShanDain.AIM.Service;

namespace ShanDain.AIM
{
    public partial class MainForm : Form
    {
        private HttpHelper httpHelper = null;
        private Dictionary<string, AddInInfo> CurrentAddinDic = new Dictionary<string, AddInInfo>();
        /// <summary>
        /// 正在下载的列表
        /// </summary>
        private List<string> WaitDownloading = new List<string>();
        /// <summary>
        /// 已经下载并且装载好的列表
        /// </summary>
        private Dictionary<string, AddInInfo> LoadedAddinDic = new Dictionary<string, AddInInfo>();
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true; //总是接受  
        }
        public MainForm()
        {
            MLog.GetInstance().SendDebug("主程序启动");
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
            InitializeComponent();
            var colHeader1 = new ColumnHeader() {Text = "插件名", Width = 160};
            this.listView_searchResult.Columns.Add(colHeader1);
            this.listView_loaded.Columns.Add((ColumnHeader)colHeader1.Clone());
            var colHeader2 = new ColumnHeader() { Text = "版本号", Width = 100 };
            this.listView_searchResult.Columns.Add((ColumnHeader)colHeader2.Clone());
            this.listView_loaded.Columns.Add(colHeader2);
            var colHeader3 = new ColumnHeader() { Text = "描述信息", Width = 140 };
            this.listView_searchResult.Columns.Add((ColumnHeader)colHeader3.Clone());
            this.listView_loaded.Columns.Add(colHeader3);
            var colHeader4 = new ColumnHeader() { Text = "装载状态", Width = 100 };
            this.listView_searchResult.Columns.Add((ColumnHeader)colHeader4.Clone());
            this.listView_loaded.Columns.Add(colHeader4);
            var colHeader5 = new ColumnHeader() {Text = "是否可用", Width = 80};
            //this.listView_searchResult.Columns.Add((ColumnHeader) colHeader5.Clone());
            this.listView_loaded.Columns.Add(colHeader5);



            this.listView_version.Columns.Add(new ColumnHeader() {Text = "Version", Width = 100});
            httpHelper = new HttpHelper(this);
            if (!Directory.Exists(ConfigHelper.LocalBasePath))
            {
                Directory.CreateDirectory(ConfigHelper.LocalBasePath);
                MLog.GetInstance().SendDebug("创建目录:" + ConfigHelper.LocalBasePath);
            }
            //检测本地已经下载好的包
            LoadDownloaded();
            //装载本地已经下载好的包
            LoadLocalPackages();
            //装载正在下载中的包
            LoadDownloading();
            MLog.Notice += UpdateLog;
        }

        public void EnableButtonSearch()
        {
            this.Invoke((Action) (() => { this.button_search.Enabled = true; }));
        }

        public void UpdateServerStatus(string message)
        {
            this.Invoke((Action) (() => { this.label_serverstatus.Text = message; }));
        }

        public void UpdateLog(string message)
        {
            this.Invoke((Action) (() =>
            {
                var temp = this.textBox_showlog.Text;
                if (temp.Length > 5000)
                {
                    temp = temp.Substring(0, 5000);
                }

                temp = $"{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")}:{message}\r\n{temp}";
                this.textBox_showlog.Text = temp;
            }));
        }

        /// <summary>
        /// 装载本地已经下载好的包
        /// </summary>
        private void LoadLocalPackages()
        {
            var addinNamesPath = Directory.GetDirectories(ConfigHelper.LocalBasePath);
            foreach (var path in addinNamesPath)
            {
                MLog.GetInstance().SendDebug("加载本地配置,路径" + path);
                LoadLocalPackage(path, false);
            }

            UpdateLoadedPackagesView();
        }

        private void LoadLocalPackage(string path, bool flush = true)
        {
            AddInInfo addinInfo = null;
            try
            {
                addinInfo = PackManagerService.GetAddInfo(path, false);
            }
            catch (Exception e)
            {
                MLog.GetInstance().SendDebug(e.ToString());
                MessageBox.Show("找不到插件描述文件!:" + e.Message + "\\.AddIn.xml");
                //TODO 删除插件目录?
                return;
            }

            if (!LoadedAddinDic.TryGetValue(addinInfo.AddInId, out var otherAddinfo))
            {
                LoadedAddinDic.Add(addinInfo.AddInId, addinInfo);
            }
            else
            {
                if (addinInfo.Version == otherAddinfo.Version)
                {
                    MLog.GetInstance().SendDebug($"重复装载{addinInfo.AddInId}");
                    throw new Exception($"重复装载{addinInfo.AddInId}");
                }
                else
                {
                    LoadedAddinDic[addinInfo.AddInId] = addinInfo;
                }
            }
            if(flush)
                UpdateLoadedPackagesView();

            foreach (ListViewItem item in this.listView_searchResult.Items)
            {
                var searchresultAppinId = item.Text;
                if(!LoadedAddinDic.TryGetValue(searchresultAppinId, out var tempversions))
                    continue;
                var verDic = LoadedAddinDic[searchresultAppinId];
                var remoteAddInInfo = item.Tag as Dictionary<string, AddInInfo>;
                foreach (var searchresultitem in remoteAddInInfo)
                {
                    if (verDic.Version == searchresultitem.Value.Version)
                    {
                        item.SubItems[3].Text = "已装载";
                    }
                    else
                    {
                        item.SubItems[3].Text = "未装载";
                    }
                }
            }
        }

        private void UpdateLoadedPackagesView()
        {
            this.listView_loaded.Items.Clear();
            foreach (var temp in LoadedAddinDic)
            {
                var item = new ListViewItem();
                item.Text = temp.Key;
                var version = temp.Value.Version;
                var info = temp.Value;
                string misspack = null;
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = version.ToString() });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = info.Description });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() { Text = "已装载" });
                item.SubItems.Add(new ListViewItem.ListViewSubItem() {Text = FindDeep(info, ref misspack) ? "可用" : "不可用"});
                item.Tag = info;
                this.listView_loaded.Items.Add(item);
            }
        }
        //查询依赖,判断插件是否可用
        private bool FindDeep(AddInInfo addininfo, ref string misspackage)
        {
            foreach (var deepappin in addininfo.Dependency)
            {
                if (LoadedAddinDic.TryGetValue(deepappin.AddInId, out var val))
                {
                    if (val.Version != deepappin.Version)
                    {
                        misspackage = deepappin.AddInId + ":" + deepappin.Version.ToString();
                        return false;
                    }
                }
                else
                {
                    misspackage = deepappin.AddInId + ":" + deepappin.Version.ToString();
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 检测本地已经下载好的包,如果有下载好但是未解压的执行解压并且删除,有解压好的,但是未删除的删除
        /// </summary>
        private void LoadDownloaded()
        {
            if (!Directory.Exists(ConfigHelper.DownloadPath))
            {
                Directory.CreateDirectory(ConfigHelper.DownloadPath);
                MLog.GetInstance().SendDebug("创建下载目录:" + ConfigHelper.DownloadPath);
            }
            var files = Directory.GetFiles(ConfigHelper.DownloadPath);
            foreach (var file in files)
            {
                File.Delete(file);
                MLog.GetInstance().SendDebug("删除下载文件:" + file);
            }
            /*
            var waitToUnzip = new List<string>();
            foreach (var file in files)
            {
                var filename = Path.GetFileNameWithoutExtension(file);
                var temp = filename.Split('_');
                var addinid = temp[0];
                var version = temp[1];
                //已经装载成功,但是下载包还未删除,删掉
                if (LoadedAddinDic.TryGetValue(addinid, out var tempdic) && tempdic != null)
                {
                    if (tempdic.Version.ToString() == version)
                    {
                        File.Delete(file);
                        continue;
                    }
                }
                waitToUnzip.Add(file);
            }
            */
            //TODO 解压,覆盖
            //foreach (var filename in waitToUnzip)
            //{
            //    var path = $"./{LocalBasePath}/{Path.GetFileNameWithoutExtension(filename)}/";
            //    if (Directory.Exists(path))
            //    {
            //        Directory.Delete(path, true);
            //    }
            //    new FastZip().ExtractZip(filename, path, "");
            //    File.Delete(filename);
            //}
        }
        
        /// <summary>
        /// 装载正在下载中的包,启动继续下载
        /// </summary>
        private void LoadDownloading()
        {
            if (!Directory.Exists(ConfigHelper.DownloadTempPath))
            {
                Directory.CreateDirectory(ConfigHelper.DownloadTempPath);
                MLog.GetInstance().SendDebug("创建临时下载目录!" + ConfigHelper.DownloadTempPath);
            }

            var files = Directory.GetFiles(ConfigHelper.DownloadTempPath);
            foreach (var filename in files)
            {
                //var temp = Path.GetFileName(filename).Replace(".zip.tmp", "");
                //WaitDownloading.Add(temp);
                //删除未下载完成的任务
                File.Delete(filename);
                MLog.GetInstance().SendDebug("删除下载临时文件:" + filename);
            }
        }
        
        /// <summary>
        /// 启动查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_search_Click(object sender, EventArgs e)
        {
            var keyword = this.textBox_search.Text?.Trim();
            this.button_search.Enabled = false;
            this.listView_searchResult.Items.Clear();
            httpHelper.Search(keyword);
        }
        /// <summary>
        /// 更新查询结果
        /// </summary>
        /// <param name="count"></param>
        /// <param name="searchResult"></param>
        public void UpdateSearchResult(int count, List<Tuple<string, Dictionary<string, AddInInfo>>> searchResult)
        {
            this.listView_searchResult.Items.Clear();
            if (count == 0)
                MessageBox.Show("未找到结果!");
            foreach (var item in searchResult)
            {
                var version = item.Item2.Keys.First();
                var addininfo = item.Item2[version];
                var listitem = new ListViewItem();
                listitem.Text = addininfo.AddInId;
                listitem.SubItems.Add(addininfo.Version.ToString());
                listitem.SubItems.Add(addininfo.Description);
                var t = CheckIsLoadedOrDownloading(addininfo.AddInId, addininfo.Version.ToString());
                var state = "未下载";
                if (t.Item1)
                    state = "已装载";
                if (t.Item2)
                    state = "正在下载";
                listitem.SubItems.Add(state);
                listitem.Tag = item.Item2;
                listView_searchResult.Items.Add(listitem);
            }
        }
        /// <summary>
        /// 查询结果中的选中处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_searchResult_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var target = sender as ListView;
            var selectedItem = e.Item;
            CurrentAddinDic = (Dictionary<string, AddInInfo>)selectedItem.Tag;
            ShowList(CurrentAddinDic);
        }

        private void LocalView_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var target = sender as ListView;
            var selectedItem = e.Item;
            var CurrentAddinDic = (AddInInfo)selectedItem.Tag;
            ShowList(CurrentAddinDic);
        }

        private void ShowList(Dictionary<string, AddInInfo> dic)
        {
            this.listView_version.Items.Clear();
            var curitemkey = dic.Keys.First();
            var curItem = dic[curitemkey];
            foreach (var item in dic)
            {
                this.listView_version.Items.Add(new ListViewItem() {Text = item.Key, Tag = item.Value});
            }

            var str = FormatAddinInfo(curItem);
            var t = CheckIsLoadedOrDownloading(curItem.AddInId, curItem.Version.ToString());
            this.button_unload.Enabled = t.Item1;
            this.label_selected.Text = str;
        }

        private void ShowList(AddInInfo addin)
        {

            this.listView_version.Items.Clear();
            this.listView_version.Items.Add(new ListViewItem() { Text = addin.Version.ToString(), Tag = addin });

            var str = FormatAddinInfo(addin);
            var t = CheckIsLoadedOrDownloading(addin.AddInId, addin.Version.ToString());
            this.button_unload.Enabled = t.Item1;
            this.label_selected.Text = str;
        }
        /// <summary>
        /// 格式化显示插件信息
        /// </summary>
        /// <param name="addInInfo"></param>
        /// <returns></returns>
        private string FormatAddinInfo(AddInInfo addInInfo)
        {
            string state = "未下载";
            var t = CheckIsLoadedOrDownloading(addInInfo.AddInId, addInInfo.Version.ToString());
            if (t.Item1)
                state = "已装载";
            if (t.Item2)
                state = "正在下载";
            string missPackage = null;
            bool canuse = FindDeep(addInInfo, ref missPackage);
            var strtemp = canuse ? "可用" : $"不可用,缺少:\r\n{missPackage}";
            var deepstr = string.Join("  \r\n", addInInfo.Dependency.Select(x => x.AddInId + ":" + x.Version.ToString()));
            return $"ID:{addInInfo.AddInId}\r\nAuthor:{addInInfo.Auther}\r\nVersion:{addInInfo.Version.ToString()}\r\n插件状态:{state}\r\n插件:{strtemp}\r\n插件依赖项:\r\n{deepstr}";
        }
        /// <summary>
        /// 版本号选择框选中后的处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listView_version_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            var addinInfo = e.Item.Tag as AddInInfo;
            this.label_selected.Text = FormatAddinInfo(addinInfo);
            var t = CheckIsLoadedOrDownloading(addinInfo.AddInId, addinInfo.Version.ToString());
            this.button_unload.Enabled = t.Item1;
        }
        /// <summary>
        /// 启动下载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button_satrt_Click(object sender, EventArgs e)
        {
            var selectedInfo = GetSelectedItem();
            if(selectedInfo == null)
            {
                MessageBox.Show("请先选中要下载的项目");
                return;
            }
            var addinid = selectedInfo.Item1;
            var version = selectedInfo.Item2;
            var checkresult = CheckIsLoadedOrDownloading(addinid, version);
            if (checkresult.Item1)
            {
                MessageBox.Show("已经在本地装载,无需再次下载");
                return;
            }

            if (checkresult.Item2)
            {
                MessageBox.Show("正在下载,无需再次下载");
                return;
            }

            //检测是否有其他包依赖当前包
            LoadedAddinDic.TryGetValue(addinid, out var localpack);
            MLog.GetInstance().SendDebug($"开始检测:{addinid} {version}的依赖/冲突项");
            List<AddInInfo> deepList = new List<AddInInfo>();
            if (localpack != null)
            {
                foreach (var pack in LoadedAddinDic)
                {
                    if (pack.Key == addinid)
                        continue;
                    foreach (var deep in pack.Value.Dependency)
                    {
                        if (deep.AddInId == localpack.AddInId && deep.Version == localpack.Version)
                        {
                            deepList.Add(pack.Value);
                        }
                    }
                }
            }

            if (deepList.Count > 0)
            {
                var resu = MessageBox.Show("该包被以下包依赖,如果卸载,以下组件可能无法使用!,是否继续升级?\r\n" + string.Join("\r\n", deepList.Select(x => x.AddInId +" "+ x.Version.ToString())), "确认升级?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
                MLog.GetInstance()
                    .SendDebug(
                        $"{string.Join("--", deepList.Select(x => x.AddInId + " " + x.Version.ToString()))} 依赖: {addinid}:{version}");
                MLog.GetInstance().SendDebug($"选择结果:{resu.ToString()}");
                if (resu != DialogResult.OK)
                {
                    return;
                }
            }

            SetControlState(false);

            var t = new Thread(new ThreadStart((Action) (() =>
            {
                try
                {
                    Get(new Tuple<string, string, IManagerGetter>(addinid, version, new NetGetter()));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                    this.Invoke((Action) (() => { SetControlState(true); }));
                }
            }))) {IsBackground = true};
            t.Start();
            //var t = new Thread(new ParameterizedThreadStart(Get)){ IsBackground = true};
            //t.Start(new Tuple<string, string, IManagerGetter>(addinid, version, new NetGetter()));
            //Get(addinid, version, new NetGetter());
        }

        private Tuple<string, string> GetSelectedItem()
        {
            var tt = this.tabControl.SelectedTab.Controls[0] as ListView;
            var selectedItem = tt.SelectedItems;
            if (selectedItem.Count == 0)
            {
                return null;
            }

            var addinid = selectedItem[0].Text;
            string version = null;
            var selectedVersion = this.listView_version.SelectedItems;
            if (selectedVersion.Count > 0)
                version = selectedVersion[0].Text;
            if (string.IsNullOrEmpty(version))
                version = selectedItem[0].SubItems[1].Text;
            return new Tuple<string, string>(addinid, version);
        }

        /// <summary>
        /// 字典结构,name:version:checksum
        /// </summary>
        /// <param name="result"></param>
        private void ParsedDepency(Tuple<string, string, string> target, Tuple<Dictionary<string, List<Tuple<string, string>>>, List<string>> result)
        {

            if (result.Item2 != null && result.Item2.Count > 0)
            {
                MessageBox.Show("以下包与要下载的包存在冲突!无法下载!" + string.Join(",", result.Item2));
                MLog.GetInstance().SendDebug("以下包与要下载的包存在冲突!无法下载!" + string.Join(",", result.Item2));
                SetControlState(true);
                return;
            }

            var downloadNameList = new List<Tuple<string, string>>(){ new Tuple<string, string>($"{target.Item1}_{target.Item2}", target.Item3) };
            foreach (var item in result.Item1)
            {
                if (item.Key == target.Item1)
                {
                    continue;
                }

                if (LoadedAddinDic.ContainsKey(item.Key))
                {
                    continue;
                }
                //只有本地没装载过的AppIn才会去下载
                var tversion = item.Value.First();
                downloadNameList.Add(new Tuple<string, string>($"{item.Key}_{tversion.Item1}", tversion.Item2));
            }
            //foreach (var item in result.Item1)
            //{
            //    foreach (var item2 in item.Value)
            //    {
            //        var check = CheckIsLoadedOrDownloading(item.Key, item2.Item1);
            //        if(check.Item1 || check.Item2)
            //            continue;
            //        downloadNameList.Add(new Tuple<string, string>($"{item.Key}_{item2.Item1}", item2.Item2));
            //    }
            //}
            var task = new TaskList();
            task.WorkResult += Task_WorkResult;
            foreach (var name in downloadNameList)
            {
                task.AddTask(name.Item1);
            }
            foreach (var item in downloadNameList)
            {
                var donwloadTask = new DownloadTask(item.Item1, item.Item2, task);
                donwloadTask.UpdateDownload += DonwloadTask_UpdateDownload;
                donwloadTask.DownloadResult += DonwloadTask_DownloadResult;
                donwloadTask.Start();
            }
        }

        private void Task_WorkResult(Dictionary<string, int> obj)
        {
            bool allsuccess = true;
            List<string> tasknamelist = new List<string>();
            lock (obj)
            {
                foreach (var temp in obj)
                {
                    if (temp.Value == 2)
                    {
                        allsuccess = false;
                        break;
                    }
                    tasknamelist.Add(temp.Key);
                }
            }

            try
            {
                ///全部成功以后开始解压
                if (allsuccess)
                {
                    MLog.GetInstance().SendDebug("全部下载完成,等待装载!");
                    var result = ServiceHelper.StopService(ConfigHelper.ServiceName);
                    if (result == ServiceOPResult.Timeout || result == ServiceOPResult.Unknow)
                    {
                        foreach (var process in Process.GetProcesses())
                        {
                            if (process.ProcessName == ConfigHelper.ProcessName)
                            {
                                try
                                {
                                    process.Kill();
                                    MLog.GetInstance().SendDebug("停止服务成功");
                                    this.UpdateServerStatus("停止服务成功");
                                    break;
                                }
                                catch (Exception e)
                                {
                                    MLog.GetInstance().SendDebug("无法停止服务进程");
                                    this.UpdateServerStatus("无法停止服务进程!");
                                    this.Invoke((Action)(() => {
                                        SetControlState(true);
                                    }));
                                    return;
                                }
                            }
                        }
                    }
                    else if(result == ServiceOPResult.Success)
                    {
                        MLog.GetInstance().SendDebug("停止服务成功");
                        this.UpdateServerStatus("停止服务成功");
                    }

                    foreach (var addinName in tasknamelist)
                    {
                        var y = addinName.Split('_');
                        var sourceZipFile = $"./{ConfigHelper.DownloadPath}/{addinName}.zip";
                        new FastZip().ExtractZip(sourceZipFile, $"./{ConfigHelper.LocalBasePath}/{y[0]}/", FastZip.Overwrite.Always,
                            new FastZip.ConfirmOverwriteDelegate((x) => true), "", "", false);
                        File.Delete(sourceZipFile);
                        var temp = addinName.Split('_');
                        this.Invoke((Action)(() => { this.LoadLocalPackage($"{ConfigHelper.LocalBasePath}\\{temp[0]}"); }));
                    }
                    MLog.GetInstance().SendDebug("装载完毕");
                    Exception ex = null;
                    var startResult = ServiceHelper.StartService(ConfigHelper.ServiceName, ref ex);
                    if (!startResult)
                    {
                        MLog.GetInstance().SendDebug(ex.ToString());
                        this.UpdateServerStatus("重启服务失败!");
                    }
                    else
                    {
                        MLog.GetInstance().SendDebug("重启服务成功!");
                        this.UpdateServerStatus("重启服务成功!");
                    }
                }
                else
                {
                    //不是全部成功的情况,删除已下载的文件
                    foreach (var file in Directory.GetFiles(ConfigHelper.DownloadPath))
                    {
                        File.Delete(file);
                    }
                }
            }
            catch (Exception e)
            {
                MLog.GetInstance().SendDebug(e.ToString());
            }
            this.Invoke((Action)(() => {
                SetControlState(true);
            }));
        }

        private void DonwloadTask_DownloadResult(DownloadTask downloadTask, bool success)
        {
            Console.WriteLine("下载结果:" + success);
            MLog.GetInstance().SendDebug($"下载任务:{downloadTask.downloadurl} 结果:{success}");
            /*
            if (success)
            {
                var y = downloadTask.AddInName.Split('_');
                var sourceZipFile = $"./{DownloadPath}/{downloadTask.AddInName}.zip";
                new FastZip().ExtractZip(sourceZipFile, $"./{LocalBasePath}/{y[0]}/", FastZip.Overwrite.Always,
                    new FastZip.ConfirmOverwriteDelegate((x) => true), "", "", false);
                File.Delete(sourceZipFile);
                var temp = downloadTask.AddInName.Split('_');
                this.Invoke((Action) (() => { this.LoadLocalPackage($"{LocalBasePath}\\{temp[0]}"); }));
            }
            */
        }

        private void DonwloadTask_UpdateDownload(DownloadTask downloadTask, int percentage)
        {
            Console.WriteLine("下载进度:" + percentage);
        }

        private void SetControlState(bool state)
        {
            this.listView_searchResult.Enabled = state;
            this.listView_version.Enabled = state;
            this.button_satrt.Enabled = state;
            this.button_search.Enabled = state;
            this.textBox_search.Enabled = state;
        }
        

        /// <summary>
        /// 检测插件是否已经装载或者正在下载
        /// </summary>
        /// <param name="addin"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        private Tuple<bool, bool> CheckIsLoadedOrDownloading(string addin, string version)
        {
            if (LoadedAddinDic.TryGetValue(addin, out var val) && val != null)
            {
                //if(val.TryGetValue(version, out var addininfo) && addininfo != null)
                    //return new Tuple<bool, bool>(true, false);
                if(val.Version.ToString() == version)
                    return new Tuple<bool, bool>(true, false);
            }

            if (WaitDownloading.Contains($"{addin}_{version}"))
                return new Tuple<bool, bool>(false, true);
            return new Tuple<bool, bool>(false, false);
        }
        /// <summary>
        /// 解析依赖,回掉ParsedDepency
        /// </summary>
        /// <param name="obj"></param>
        private void Get(object obj)
        {
            var tuple = obj as Tuple<string, string, IManagerGetter>;
            string addinid = tuple.Item1;
            string version = tuple.Item2;
            IManagerGetter manager = tuple.Item3;
            MLog.GetInstance().SendDebug($"开始启动下载:{addinid} {version}");
            Dictionary<string, List<Tuple<string, string>>> needDownloadList = new Dictionary<string, List<Tuple<string, string>>>();
            Stack<RefAddInInfo> depStack = new Stack<RefAddInInfo>();
            List<string> conList = new List<string>();

            var addin = manager.GetAddInInfo(addinid, version);
            needDownloadList.Add(addin.AddInId, new List<Tuple<string, string>>() { new Tuple<string, string>(addin.Version.ToString(), addin.CheckSum) });
            foreach (var addinref in addin.Dependency)
            {
                depStack.Push(addinref);
            }

            conList.AddRange(addin.Conflict.Select(x => x.AddInId));
            conList = conList.Distinct().ToList();

            while (depStack.Count > 0)
            {
                var addinref = depStack.Pop();
                var temp = manager.GetAddInInfo(addinref.AddInId, addinref.Version.ToString());
                if (temp == null)
                    throw new Exception($"遇到无法解析包:{addinref.AddInId}, {addinref.Version.ToString()}");
                #region 将当前添加至待下载列表
                if (!needDownloadList.TryGetValue(temp.AddInId, out var versionList))
                {
                    needDownloadList.Add(temp.AddInId, new List<Tuple<string, string>>() { new Tuple<string, string>(temp.Version.ToString(), temp.CheckSum) });
                }
                else
                {
                    if (versionList.FirstOrDefault(x=>x.Item1 == temp.Version.ToString()) == null)
                    {
                        versionList.Add(new Tuple<string, string>(temp.Version.ToString(), temp.CheckSum));
                    }
                }
                #endregion
                //添加冲突列表
                foreach (var id in temp.Conflict.Select(x => x.AddInId))
                {
                    if (!conList.Contains(id))
                        conList.Add(id);
                }
                //添加待解析栈
                temp.Dependency.ForEach((x) =>
                {
                    if (needDownloadList.TryGetValue(x.AddInId, out var versions) &&
                        versions.FirstOrDefault(y => y.Item1 == x.Version.ToString()) != null)
                        return;
                    depStack.Push(x);
                });
            }
            List<string> conedList = new List<string>();
            //最后检测冲突列表
            foreach (var id in conList)
            {
                if (needDownloadList.ContainsKey(id))
                    conedList.Add(id);
            }

            MLog.GetInstance().SendDebug($"检测到冲突项:{string.Join(",", conedList)}");
            foreach (var item in needDownloadList)
            {
                foreach (var item2 in item.Value)
                {
                    MLog.GetInstance().SendDebug($"检测到依赖项:{item.Key} {item2.Item1}");
                }
            }
            var resultvalue = new Tuple<Dictionary<string, List<Tuple<string, string>>>, List<string>>(needDownloadList, conedList);
            this.Invoke((Action) (() =>
            {
                this.ParsedDepency(new Tuple<string, string, string>(addinid, version, addin.CheckSum), resultvalue);
            }));
        }

        private void button_unload_Click(object sender, EventArgs e)
        {
            var selectedInfo = GetSelectedItem();
            if (selectedInfo == null)
            {
                MessageBox.Show("请先选中要卸载的项目");
                return;
            }
            List<Tuple<string, string>> list = new List<Tuple<string, string>>();
            foreach (var temp in LoadedAddinDic)
            {
                foreach (var val in temp.Value.Dependency)
                {
                    if (val.AddInId == selectedInfo.Item1)
                    {
                        list.Add(new Tuple<string, string>(temp.Key, temp.Value.Version.ToString()));
                    }
                }
            }

            DialogResult resu = DialogResult.OK;
            MLog.GetInstance().SendDebug($"检测到卸载的包依赖:{selectedInfo.Item1} {selectedInfo.Item2}"+ string.Join("\r\n", list.Select(x => x.Item1 + x.Item2)));
            if (list.Count > 0)
            {
                resu = MessageBox.Show("该包被以下包依赖,如果卸载,以下组件可能无法使用!,是否继续卸载?\r\n" + string.Join("\r\n", list.Select(x=>x.Item1+x.Item2)), "确认卸载?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
            }

            if (resu == DialogResult.OK)
            {
                try
                {
                    Directory.Delete(ConfigHelper.LocalBasePath + "/" + selectedInfo.Item1, true);
                    LoadedAddinDic.Remove(selectedInfo.Item1);
                    MLog.GetInstance().SendDebug($"卸载:{ConfigHelper.LocalBasePath + "/" + selectedInfo.Item1} 成功");
                }
                catch (Exception ex)
                {
                    MLog.GetInstance().SendDebug("卸载失败" + ex.ToString());
                    MLog.GetInstance().SendDebug("卸载失败!");
                }
            }
            else
            {
                return;
            }
            UpdateLoadedPackagesView();
            MessageBox.Show("卸载完毕");
        }

        private void tabPage_loaded_Click(object sender, EventArgs e)
        {

        }
    }

    

    
}
