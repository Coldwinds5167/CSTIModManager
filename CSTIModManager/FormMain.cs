using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CSTIModManager.Internals;
using CSTIModManager.Internals.SimpleJSON;
using Microsoft.Win32;
using Mono.Cecil;

namespace CSTIModManager
{
    public partial class FormMain : Form
    {

        private const string BaseEndpoint = "https://gitee.com/api/v5/repos/";
        private const Int16 CurrentVersion = 5;
        private List<ReleaseInfo> releases;
        Dictionary<string, int> groups = new Dictionary<string, int>();
        private string InstallDirectory = @"";
        public bool isSteam = true;
        public bool platformDetected = false;
        public Dictionary<string, ListViewItem> modlist = new Dictionary<string, ListViewItem>();

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LocationHandler();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            releases = new List<ReleaseInfo>();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Text = "CSTI Mod Manager v" + version.Substring(0, version.Length - 2);
            if (!File.Exists(Path.Combine(InstallDirectory, "winhttp.dll")))
            {
                if (File.Exists(Path.Combine(InstallDirectory, "mods.disable")))
                {
                    buttonToggleMods.Text = "启用Mods";
                    buttonToggleMods.BackColor = System.Drawing.Color.IndianRed;
                    buttonToggleMods.Enabled = true;
                }
                else
                {
                    buttonToggleMods.Enabled = false;
                }
            }
            else
            {
                buttonToggleMods.Enabled = true;
            }
            new Thread(() =>
            {
                LoadRequiredPlugins();
            }).Start();
        }

        #region ReleaseHandling

        private void LoadReleases()
        {
#if !DEBUG
            var decodedMods = JSON.Parse(DownloadSite("https://gitee.com/Cold_winds/cstimodinfo/raw/master/mods.json"));
            var decodedGroups = JSON.Parse(DownloadSite("https://gitee.com/Cold_winds/cstimodinfo/raw/master/groupinfo.json"));
#else
            var decoded = JSON.Parse(File.ReadAllText("C:/Users/Steven/Desktop/testmods.json"));
#endif
            var allMods = decodedMods.AsArray;
            var allGroups = decodedGroups.AsArray;

            for (int i = 0; i < allMods.Count; i++)
            {
                JSONNode current = allMods[i];
                ReleaseInfo release = new ReleaseInfo(current["name"], current["modname"], current["author"], current["version"], current["group"], current["download_url"], current["install_location"], current["git_path"], current["dependencies"].AsArray, current["contain_dll"], current["only_dll"]);
                if (release.ContainDll)
                {
                    release.DllName = current["dll_name"];
                }
                //UpdateReleaseInfo(ref release);
                releases.Add(release);
            }


            allGroups.Linq.OrderBy(x => x.Value["rank"]);
            for (int i = 0; i < allGroups.Count; i++)
            {
                JSONNode current = allGroups[i];
                if (releases.Any(x => x.Group == current["name"]))
                {
                    groups.Add(current["name"], groups.Count());
                }
            }
            groups.Add("Uncategorized", groups.Count());

            foreach (ReleaseInfo release in releases)
            {
                foreach (string dep in release.Dependencies)
                {
                    releases.Where(x => x.Name == dep).FirstOrDefault()?.Dependents.Add(release.Name);
                }
            }
            //WriteReleasesToDisk();
        }

        private string ReadJson(string dir)
        {
            string json = string.Empty;
            using (FileStream fs = new FileStream(dir, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    json = sr.ReadToEnd().ToString();
                    return json;
                }
            }
        }

        private void LoadLocalDlls()
        {
                string dir = Path.Combine(InstallDirectory, @"BepInEx\plugins");
                var files = Directory.GetFiles(dir, "*.dll*", SearchOption.AllDirectories);
                foreach (var dllPath in files)
                {
                    try
                    {
                        var readPar = new ReaderParameters { ReadSymbols = false };
                        using (AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(dllPath, readPar))
                        {
                            string fullName = ad.Name.Name;
                            foreach (var release in releases)
                            {
                                if (fullName == release.DllName)
                                {
                                    release.isInstalled = true;
                                    if (release.OnlyDll)
                                    {
                                        release.InstallLocation = Path.GetDirectoryName(dllPath);

                                        if (Path.GetFileName(dllPath).Contains(".dll.disable"))
                                        {
                                            release.isable = false;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    { 
                        ;
                    }
                }
        }

        private void LoadLocalMods()
        {
            string dir = Path.Combine(InstallDirectory, @"BepInEx\plugins");
            var files = Directory.GetFiles(dir, "ModInfo.*", SearchOption.AllDirectories);
            foreach (var modinfo in files)
            {
                var json = JSON.Parse(ReadJson(modinfo));
                foreach (var release in releases)
                {
                    if (json["Name"] == release.ModName)
                    {
                        release.isInstalled = true;
                        release.InstallLocation = Path.GetDirectoryName(modinfo);
                        if (Path.GetFileName(modinfo) == "ModInfo.disable")
                        {
                            release.isable = false;
                        }
                    }
                }
            }
        }

        private void LoadRequiredPlugins()
        {
            CheckVersion();
            CheckBepinex();
            UpdateStatus("获取Mod信息...");
            LoadReleases();
            LoadLocalMods();
            LoadLocalDlls();
            this.Invoke((MethodInvoker)(() =>
            {//Invoke so we can call from current thread
             //Update checkbox's text
                Dictionary<string, int> includedGroups = new Dictionary<string, int>();

                for (int i = 0; i < groups.Count(); i++)
                {
                    var key = groups.First(x => x.Value == i).Key;
                    var value = listViewMods.Groups.Add(new ListViewGroup(key, HorizontalAlignment.Left));
                    groups[key] = value;
                }

                foreach (ReleaseInfo release in releases)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = release.Name;
                    if (!String.IsNullOrEmpty(release.Version))
                        item.Text = $"{release.Name} - {release.Version}";
                    if (release.isInstalled)
                    {
                        item.Text += " (已安装)";
                    }

                    if (!release.isable)
                    {
                        item.Text += " (已禁用)";
                    }

                    if (!String.IsNullOrEmpty(release.Tag)) { item.Text = string.Format("{0} - ({1})",release.Name, release.Tag); };
                    item.SubItems.Add(release.Author);
                    item.Tag = release;
                    if (release.Install)
                    {
                        listViewMods.Items.Add(item);
                    }
                    CheckDefaultMod(release, item);

                    if (release.Group == null || !groups.ContainsKey(release.Group))
                    {
                        item.Group = listViewMods.Groups[groups["Uncategorized"]];
                    }
                    else if (groups.ContainsKey(release.Group))
                    {
                        int index = groups[release.Group];
                        item.Group = listViewMods.Groups[index];
                    }
                    else
                    {
                        //int index = listViewMods.Groups.Add(new ListViewGroup(release.Group, HorizontalAlignment.Left));
                        //item.Group = listViewMods.Groups[index];
                    }
                    
                    modlist.Add(release.Name, item);
                }

                tabControlMain.Enabled = true;
                buttonInstall.Enabled = true;

            }));
           
            UpdateStatus("Mod信息获取成功!");

        }

        private void UpdateReleaseInfo(ref ReleaseInfo release)
        {
            Thread.Sleep(100); //So we don't get rate limited by github

            string releaseFormatted = BaseEndpoint + release.GitPath + "/releases";
            var rootNode = JSON.Parse(DownloadSite(releaseFormatted))[0];
            
            release.Version = rootNode["tag_name"];
            
            var assetsNode = rootNode["assets"];
            var downloadReleaseNode = assetsNode[release.ReleaseId];
            release.Link = downloadReleaseNode["browser_download_url"];
            
            var uploaderNode = downloadReleaseNode["uploader"];
            if (release.Author.Equals(String.Empty)) release.Author = uploaderNode["login"];
        }

        #endregion // ReleaseHandling

        #region Installation

        private void Install()
        {
            ChangeInstallButtonState(false);
            UpdateStatus("开始安装队列...");
            foreach (ReleaseInfo release in releases)
            {
                if (release.Name == "BepInEx" || release.Name == "Modloader")
                {
                    if (Directory.Exists(Path.Combine(InstallDirectory, @"BepInEx")) || Directory.Exists(Path.Combine(InstallDirectory, @"Modloader")))
                    {
                        continue;
                    }
                }

                if (release.isInstalled)
                {
                    continue;
                }

                if (release.Install)
                {
                    UpdateStatus(string.Format("正在下载...{0}", release.Name));
                    byte[] file = DownloadFile(release.Link);
                    UpdateStatus(string.Format("正在安装...{0}", release.Name));
                    string fileName = Path.GetFileName(release.Link);
                    string dir;
                    if (release.InstallLocation == null)
                    {
                        dir = Path.Combine(InstallDirectory, @"BepInEx\plugins");
                    }
                    else
                    {
                        dir = Path.Combine(InstallDirectory, release.InstallLocation);
                    }

                    UnzipFile(file, dir);
                    modlist[release.Name].Text += " (已安装)";
                    release.isInstalled = true;
                    UpdateStatus(string.Format("安装 {0}!", release.Name));
                }

            }
            UpdateStatus("安装完成!");
            ChangeInstallButtonState(true);

            this.Invoke((MethodInvoker)(() =>
            { //Invoke so we can call from any thread
                buttonToggleMods.Enabled = true;
            }));
        }

        #endregion // Installation

        #region UIEvents

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                Install();
            }).Start();
        }

        private void buttonFolderBrowser_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.FileName = "Card Survival - Tropical Island.exe";
                fileDialog.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = fileDialog.FileName;
                    if (Path.GetFileName(path).Equals("Card Survival - Tropical Island.exe"))
                    {
                        InstallDirectory = Path.GetDirectoryName(path);
                        textBoxDirectory.Text = InstallDirectory;
                    }
                    else
                    {
                        MessageBox.Show("这不是 Card Survival - Tropical Island.exe! 请重试!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
        }

        private void listViewMods_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ReleaseInfo release = (ReleaseInfo)e.Item.Tag;

            if (release.isInstalled && e.Item.Checked)
            {
                buttonModInfo.Enabled = true;
                buttonInstall.Enabled = false;
            }
            else
            {
                buttonModInfo.Enabled = false;
                buttonInstall.Enabled = true;
            }

            if (release.Dependencies.Count > 0)
            {
                foreach (ListViewItem item in listViewMods.Items)
                {
                    var plugin = (ReleaseInfo)item.Tag;

                    if (plugin.Name == release.Name) continue;

                    // if this depends on plugin
                    if (release.Dependencies.Contains(plugin.Name))
                    {
                        if (e.Item.Checked)
                        {
                            item.Checked = true;
                            item.ForeColor = System.Drawing.Color.DimGray;
                        }
                        else
                        {
                            release.Install = false;
                            if (releases.Count(x => plugin.Dependents.Contains(x.Name) && x.Install) <= 1)
                            {
                                item.Checked = false;
                                item.ForeColor = System.Drawing.Color.Black;
                            }
                        }
                    }
                }
            }

            // don't allow user to uncheck if a dependent is checked
            if (release.Dependents.Count > 0)
            {
                if (releases.Count(x => release.Dependents.Contains(x.Name) && x.Install) > 0)
                {
                    e.Item.Checked = true;
                }
            }

            if (release.Name.Contains("BepInEx")) { e.Item.Checked = true; };
            release.Install = e.Item.Checked;
            if (release.Name.Contains("Modloader")) { e.Item.Checked = true; };
            release.Install = e.Item.Checked;
        }

         private void listViewMods_DoubleClick(object sender, EventArgs e)
        {
            OpenLinkFromRelease();
        }

        private void buttonModInfo_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "你正在尝试删除所选Mods文件. 该操作不可撤销!\n\n你确定要继续吗?",
                "确认删除",
                MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                UpdateStatus("删除mod!");
                foreach (var release in releases)
                {
                    if (release.Install)
                    {
                        if (release.Name.Contains("BepInEx") || release.Name.Contains("Modloader"))
                        {
                            continue;
                        }
                        else
                        {
                            try
                            {
                                Directory.Delete(release.InstallLocation, true);
                                modlist[release.Name].Text = modlist[release.Name].Text.Replace(" (已安装)", "");
                                modlist[release.Name].Text = modlist[release.Name].Text.Replace(" (已禁用)", "");
                                release.isInstalled = false;
                                release.InstallLocation = string.Empty;
                            }
                            catch (Exception ex)
                            {
                                ;
                            }
                        }
                    }
                }
            }
            UpdateStatus("删除完成!");
        }

        private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e)
         {
             OpenLinkFromRelease();
         }

        private void buttonUninstallAll_Click(object sender, EventArgs e)
        {
            var confirmResult = MessageBox.Show(
                "你正在尝试删除所有Mods文件 (包括前置文件). 该操作不可撤销!\n\n你确定要继续吗?",
                "确认删除",
                MessageBoxButtons.YesNo);

            if (confirmResult == DialogResult.Yes)
            {
                UpdateStatus("正在删除所有Mods.");

                var pluginsPath = Path.Combine(InstallDirectory, @"BepInEx\plugins");

                try
                {
                    foreach (var d in Directory.GetDirectories(pluginsPath))
                    {
                        Directory.Delete(d, true);
                    }

                    foreach (var f in Directory.GetFiles(pluginsPath))
                    {
                        File.Delete(f);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发生未知错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("删除Mods失败.");
                    return;
                }

                UpdateStatus("所有Mods删除成功!");
            }
        }

        private void buttonBackupMods_Click(object sender, EventArgs e)
        {
            var pluginsPath = Path.Combine(InstallDirectory, @"BepInEx\plugins");

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                InitialDirectory = InstallDirectory,
                FileName = $"Mod Backup",
                Filter = "ZIP Folder (.zip)|*.zip",
                Title = "备份Mods文件"
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName != "")
            {
                UpdateStatus("备份Mods文件中...");
                try
                {
                    if (File.Exists(saveFileDialog.FileName)) File.Delete(saveFileDialog.FileName);
                    ZipFile.CreateFromDirectory(pluginsPath, saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("发生未知错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    UpdateStatus("备份Mods文件失败.");
                    return;
                }
                UpdateStatus("备份Mods文件成功!");
            }


        }

        private void buttonBackupCosmetics_Click(object sender, EventArgs e)
        {
            return;
        }

        private void buttonRestoreMods_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.InitialDirectory = InstallDirectory;
                fileDialog.FileName = "Mod Backup.zip";
                fileDialog.Filter = "ZIP Folder (.zip)|*.zip";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    if (!Path.GetExtension(fileDialog.FileName).Equals(".zip", StringComparison.InvariantCultureIgnoreCase))
                    {
                        MessageBox.Show("包含无效文件!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus("恢复Mods文件失败.");
                        return;
                    }
                    var pluginsPath = Path.Combine(InstallDirectory, @"BepInEx\plugins");
                    try
                    {
                        UpdateStatus("恢复Mods中...");
                        using (var archive = ZipFile.OpenRead(fileDialog.FileName))
                        {
                            foreach (var entry in archive.Entries)
                            {
                                var directory = Path.Combine(InstallDirectory, @"BepInEx\plugins", Path.GetDirectoryName(entry.FullName));
                                if (!Directory.Exists(directory))
                                {
                                    Directory.CreateDirectory(directory);
                                }

                                entry.ExtractToFile(Path.Combine(pluginsPath, entry.FullName), true);
                            }
                        }
                        UpdateStatus("成功恢复Mods文件!");
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("发生未知错误!", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        UpdateStatus("恢复Mods文件失败.");
                    }
                }
            }
        }

        private void buttonRestoreCosmetics_Click(object sender, EventArgs e)
        {
            return;
        }

        #region Folders

        private void buttonOpenGameFolder_Click(object sender, EventArgs e)
        {
            if (Directory.Exists(InstallDirectory))
                Process.Start(InstallDirectory);
        }

        private void buttonOpenConfigFolder_Click(object sender, EventArgs e)
        {
            var configDirectory = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"..\LocalLow\WinterSpring Games\Card Survival - Tropical Island\");
            if (Directory.Exists(configDirectory))
                Process.Start(configDirectory);
        }

        private void buttonOpenBepInExFolder_Click(object sender, EventArgs e)
        {
            var BepInExDirectory = Path.Combine(InstallDirectory, "BepInEx");
            if (Directory.Exists(BepInExDirectory))
                Process.Start(BepInExDirectory);
        }

        #endregion // Folders

        private void buttonOpenWiki_Click(object sender, EventArgs e)
        {
            Process.Start("https://cswiki.uuppi.com/wiki/#!index.md");
        }

        private void buttonDiscordLink_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.com/invite/rwCtgKRZ");
        }

        #endregion // UIEvents

        #region Helpers

        private CookieContainer PermCookie;
        private string DownloadSite(string URL)
        {
            try
            {
                if (PermCookie == null) { PermCookie = new CookieContainer(); }
                HttpWebRequest RQuest = (HttpWebRequest)HttpWebRequest.Create(URL);
                RQuest.Method = "GET";
                RQuest.KeepAlive = true;
                RQuest.CookieContainer = PermCookie;
                RQuest.ContentType = "application/x-www-form-urlencoded";
                RQuest.Referer = "";
                RQuest.UserAgent = "CSTI-Mod-Manager";
                RQuest.Proxy = null;
#if DEBUG
                RQuest.Headers.Add("Authorization", $"Token {File.ReadAllText("../../token.txt")}");
#endif
                HttpWebResponse Response = (HttpWebResponse)RQuest.GetResponse();
                StreamReader Sr = new StreamReader(Response.GetResponseStream());
                string Code = Sr.ReadToEnd();
                Sr.Close();
                return Code;
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("403"))
                {
                    MessageBox.Show("获取更新信息失败，请稍后重试", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    MessageBox.Show("获取更新信息失败，请检查您的网络连接", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                Process.GetCurrentProcess().Kill();
                return null;
            }
        }

        private void UnzipFile(byte[] data, string directory)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                using (var unzip = new Unzip(ms))
                {
                    unzip.ExtractToDirectory(directory);
                }
            }
        }

        private byte[] DownloadFile(string url)
        {
            WebClient client = new WebClient();
            client.Proxy = null;
            return client.DownloadData(url);
        }

        private void UpdateStatus(string status)
        {
            string formattedText = string.Format("状态: {0}", status);
            this.Invoke((MethodInvoker)(() =>
            { //Invoke so we can call from any thread
                labelStatus.Text = formattedText;
            }));
        }
  
        private void NotFoundHandler()
        {
            bool found = false;
            while (found == false)
            {
                using (var fileDialog = new OpenFileDialog())
                {
                    fileDialog.FileName = "Card Survival - Tropical Island.exe";
                    fileDialog.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
                    fileDialog.FilterIndex = 1;
                    if (fileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string path = fileDialog.FileName;
                        if (Path.GetFileName(path).Equals("Card Survival - Tropical Island.exe"))
                        {
                            InstallDirectory = Path.GetDirectoryName(path);
                            textBoxDirectory.Text = InstallDirectory;
                            found = true;
                        }
                        else
                        {
                            MessageBox.Show("这不是Card Survival - Tropical Island.exe!请重新添加！", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                    else
                    {
                        Process.GetCurrentProcess().Kill();
                    }
                }
            }
        }

        private void CheckVersion()
        {
            UpdateStatus("检查更新中...");
            Int16 version = Convert.ToInt16(DownloadSite("https://gitee.com/Cold_winds/cstimod-manager/raw/master/update.json"));
            if (version > CurrentVersion)
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    MessageBox.Show("检测到有版本更新，请使用新版本", "有新的更新可用!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    Process.Start("https://gitee.com/Cold_winds/cstimod-manager/releases/latest");
                    Process.GetCurrentProcess().Kill();
                    Environment.Exit(0);
                }));
            }
        }

        private void CheckBepinex()
        {
            string download_url =
                "https://gitee.com/Cold_winds/BepInEx/releases/download/5.4.22.0/BepInEx_x64_5.4.22.0%EF%BC%88%E8%A7%A3%E5%8E%8B%E5%88%B0%E6%B8%B8%E6%88%8F%E6%A0%B9%E7%9B%AE%E5%BD%95%EF%BC%89.zip";
            string dir = InstallDirectory;
            if (Directory.Exists(Path.Combine(InstallDirectory, @"BepInEx")))
            {
                return;
            }
            var confirmResult1 = MessageBox.Show(
                "未检测到BepInEx!\n\n是否安装？",
                "未检测到BepInEx",
                MessageBoxButtons.YesNo);

            if (confirmResult1 == DialogResult.Yes)
            {
                var confirmResult2 = MessageBox.Show(
                    "安装过程中会启动一次游戏，\n\n请等待游戏加载完成后再关闭!\n\n是否继续？",
                    "提示",
                    MessageBoxButtons.YesNo);
                if (confirmResult2 == DialogResult.Yes)
                {
                    byte[] file = DownloadFile(download_url);
                    UpdateStatus(string.Format("正在安装...BepInEx"));
                    string fileName = Path.GetFileName(download_url);
                    UnzipFile(file, dir);
                    Process.Start(Path.Combine(InstallDirectory, @"Card Survival - Tropical Island.exe"));
                }
                else
                {
                    Process.GetCurrentProcess().Kill();
                }
            }
            else
            {
                Process.GetCurrentProcess().Kill();
            }
        }

        private void ChangeInstallButtonState(bool enabled)
        {
            this.Invoke((MethodInvoker)(() =>
                {
                    buttonInstall.Enabled = enabled;
                }));
        }

        private void OpenLinkFromRelease()
        {
            if (listViewMods.SelectedItems.Count > 0)
            {
                ReleaseInfo release = (ReleaseInfo)listViewMods.SelectedItems[0].Tag;
                UpdateStatus($"打开gitee页面 {release.Name}");
                Process.Start(string.Format("https://gitee.com/{0}", release.GitPath));
            }
            
        }

#endregion // Helpers

#region Registry

        private void LocationHandler()
        {
            string steam = GetSteamLocation();
            if (steam != null)
            {
                if (Directory.Exists(steam))
                {
                    if (File.Exists(steam + @"\Card Survival - Tropical Island.exe"))
                    {
                        textBoxDirectory.Text = steam;
                        InstallDirectory = steam;
                        platformDetected = true;
                        return;
                    }
                }
            }
            ShowErrorFindingDirectoryMessage();
        }
        private void ShowErrorFindingDirectoryMessage()
        {
            MessageBox.Show("无法找到您的游戏安装目录, 请点击 \"OK\" 然后指定一个目录", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            NotFoundHandler();
            this.TopMost = true;
        }
        private string GetSteamLocation()
        {
            string path = RegistryWOW6432.GetRegKey64(RegHive.HKEY_LOCAL_MACHINE, @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Steam App 1694420", @"InstallLocation");
            if (path != null)
            {
                path = path + @"\";
            }
            return path;
        }
        private void CheckDefaultMod(ReleaseInfo release, ListViewItem item)
        {
            if (release.Name.Contains("BepInEx") || release.Name.Contains("Modloader"))
            {
                item.Checked = true;
                item.ForeColor = System.Drawing.Color.DimGray;
            }
            else
            {
                release.Install = false;
            }
        }
#endregion // Registry

#region RegHelper
        enum RegSAM
        {
            QueryValue = 0x0001,
            SetValue = 0x0002,
            CreateSubKey = 0x0004,
            EnumerateSubKeys = 0x0008,
            Notify = 0x0010,
            CreateLink = 0x0020,
            WOW64_32Key = 0x0200,
            WOW64_64Key = 0x0100,
            WOW64_Res = 0x0300,
            Read = 0x00020019,
            Write = 0x00020006,
            Execute = 0x00020019,
            AllAccess = 0x000f003f
        }

        static class RegHive
        {
            public static UIntPtr HKEY_LOCAL_MACHINE = new UIntPtr(0x80000002u);
            public static UIntPtr HKEY_CURRENT_USER = new UIntPtr(0x80000001u);
        }

        static class RegistryWOW6432
        {
            [DllImport("Advapi32.dll")]
            static extern uint RegOpenKeyEx(UIntPtr hKey, string lpSubKey, uint ulOptions, int samDesired, out int phkResult);

            [DllImport("Advapi32.dll")]
            static extern uint RegCloseKey(int hKey);

            [DllImport("advapi32.dll", EntryPoint = "RegQueryValueEx")]
            public static extern int RegQueryValueEx(int hKey, string lpValueName, int lpReserved, ref uint lpType, System.Text.StringBuilder lpData, ref uint lpcbData);

            static public string GetRegKey64(UIntPtr inHive, String inKeyName, string inPropertyName)
            {
                return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_64Key, inPropertyName);
            }

            static public string GetRegKey32(UIntPtr inHive, String inKeyName, string inPropertyName)
            {
                return GetRegKey64(inHive, inKeyName, RegSAM.WOW64_32Key, inPropertyName);
            }

            static public string GetRegKey64(UIntPtr inHive, String inKeyName, RegSAM in32or64key, string inPropertyName)
            {
                //UIntPtr HKEY_LOCAL_MACHINE = (UIntPtr)0x80000002;
                int hkey = 0;

                try
                {
                    uint lResult = RegOpenKeyEx(RegHive.HKEY_LOCAL_MACHINE, inKeyName, 0, (int)RegSAM.QueryValue | (int)in32or64key, out hkey);
                    if (0 != lResult) return null;
                    uint lpType = 0;
                    uint lpcbData = 1024;
                    StringBuilder AgeBuffer = new StringBuilder(1024);
                    RegQueryValueEx(hkey, inPropertyName, 0, ref lpType, AgeBuffer, ref lpcbData);
                    string Age = AgeBuffer.ToString();
                    return Age;
                }
                finally
                {
                    if (0 != hkey) RegCloseKey(hkey);
                }
            }
        }

#endregion // RegHelper

        private void buttonToggleMods_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (var release in releases)
                {
                    if (release.Install)
                    {
                        if (release.Name.Contains("BepInEx") || release.Name.Contains("Modloader"))
                        {
                            continue;
                        }
                        else
                        {
                            if (release.isable)
                            {
                                if (!release.OnlyDll)
                                {
                                    File.Move(Path.Combine(release.InstallLocation, "ModInfo.json"),
                                        Path.Combine(release.InstallLocation, "ModInfo.disable"));
                                    release.isable = false;
                                    if (modlist.ContainsKey(release.Name))
                                    {
                                        modlist[release.Name].Text += " (已禁用)";
                                    }
                                }
                                else
                                {
                                    var files = Directory.GetFiles(release.InstallLocation, "*.dll",
                                        SearchOption.AllDirectories);
                                    foreach (var dll in files)
                                    {
                                        File.Move(dll,
                                            dll.Replace(".dll", ".dll.disable"));
                                    }

                                    release.isable = false;
                                    if (modlist.ContainsKey(release.Name))
                                    {
                                        modlist[release.Name].Text += " (已禁用)";
                                    }
                                }
                                if (release.ContainDll && !release.OnlyDll)
                                {
                                    var files = Directory.GetFiles(release.InstallLocation, "*.dll",
                                        SearchOption.AllDirectories);
                                    foreach (var dll in files)
                                    {
                                        File.Move(dll,
                                            dll.Replace(".dll", ".dll.disable"));
                                    }
                                }

                                UpdateStatus("禁用mod!");
                            }
                            else
                            {
                                if (!release.OnlyDll)
                                {
                                    File.Move(Path.Combine(release.InstallLocation, "ModInfo.disable"),
                                        Path.Combine(release.InstallLocation, "ModInfo.json"));
                                    release.isable = true;
                                    if (modlist.ContainsKey(release.Name))
                                    {
                                        modlist[release.Name].Text = modlist[release.Name].Text.Replace(" (已禁用)", "");
                                    }
                                }
                                else
                                {
                                    var files = Directory.GetFiles(release.InstallLocation, "*.dll*",
                                        SearchOption.AllDirectories);
                                    foreach (var dll in files)
                                    {
                                        File.Move(dll,
                                            dll.Replace(".dll.disable", ".dll"));
                                    }

                                    release.isable = true;
                                    if (modlist.ContainsKey(release.Name))
                                    {
                                        modlist[release.Name].Text = modlist[release.Name].Text.Replace(" (已禁用)", "");
                                    }
                                }
                                if (release.ContainDll)
                                {
                                    var files = Directory.GetFiles(release.InstallLocation, "*.dll*",
                                        SearchOption.AllDirectories);
                                    foreach (var dll in files)
                                    {
                                        File.Move(dll,
                                            dll.Replace(".dll.disable", ".dll"));
                                    }
                                }

                                UpdateStatus("启用mod!");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);;
            }
        }
    }

}
