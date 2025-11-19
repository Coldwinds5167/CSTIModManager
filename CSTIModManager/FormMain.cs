using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using CSTIModManager.Internals;
using CSTIModManager.Internals.SimpleJSON;
using Mono.Cecil;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSTIModManager
{
    public partial class FormMain : Form
    {

        private const string BaseEndpoint = "https://gitee.com/api/v5/repos/";
        private const Int16 CurrentVersion = 12;
        private List<ReleaseInfo> releasesCSTI;
        private List<ReleaseInfo> releasesCSFF;
        Dictionary<string, int> groupsCSTI = new Dictionary<string, int>();
        Dictionary<string, int> groupsCSFF = new Dictionary<string, int>();
        private string InstallDirectoryCSTI = @"";
        private string InstallDirectoryCSFF = @"";
        public bool isSteam = true;
        public bool platformDetected = false;
        public Dictionary<string, ListViewItem> modlistCSTI = new Dictionary<string, ListViewItem>();
        public Dictionary<string, ListViewItem> modlistCSFF = new Dictionary<string, ListViewItem>();
        private readonly HttpClient _httpClient;

        public enum Game
        {
            CSTI,
            CSFF
        }

        public Game GameVersion = Game.CSTI;

        public FormMain()
        {
            InitializeComponent();
        }

        private void FormMain_Load(object sender, EventArgs e)
        {
            LocationHandler();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            releasesCSTI = new List<ReleaseInfo>();
            releasesCSFF = new List<ReleaseInfo>();
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            labelVersion.Text = "CS Mod Manager v" + version.Substring(0, version.Length - 2);
            new Thread(() =>
            {
                LoadRequiredPlugins();
            }).Start();
        }

        #region ReleaseHandling
        
        private void LoadReleasesCSTI()
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
                releasesCSTI.Add(release);
            }


            allGroups.Linq.OrderBy(x => x.Value["rank"]);
            for (int i = 0; i < allGroups.Count; i++)
            {
                JSONNode current = allGroups[i];
                if (releasesCSTI.Any(x => x.Group == current["name"]))
                {
                    groupsCSTI.Add(current["name"], groupsCSTI.Count());
                }
            }
            groupsCSTI.Add("Uncategorized", groupsCSTI.Count());

            foreach (ReleaseInfo release in releasesCSTI)
            {
                foreach (string dep in release.Dependencies)
                {
                    releasesCSTI.Where(x => x.Name == dep).FirstOrDefault()?.Dependents.Add(release.Name);
                }
            }
            //WriteReleasesToDisk();
        }
        
        private void LoadReleasesCSFF()
        {
#if !DEBUG
            var decodedMods = JSON.Parse(DownloadSite("https://gitee.com/Cold_winds/csffmod-info/raw/master/mods.json"));
            var decodedGroups = JSON.Parse(DownloadSite("https://gitee.com/Cold_winds/csffmod-info/raw/master/groupinfo.json"));
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
                releasesCSFF.Add(release);
            }


            allGroups.Linq.OrderBy(x => x.Value["rank"]);
            for (int i = 0; i < allGroups.Count; i++)
            {
                JSONNode current = allGroups[i];
                if (releasesCSFF.Any(x => x.Group == current["name"]))
                {
                    groupsCSFF.Add(current["name"], groupsCSFF.Count());
                }
            }
            groupsCSFF.Add("Uncategorized", groupsCSFF.Count());

            foreach (ReleaseInfo release in releasesCSFF)
            {
                foreach (string dep in release.Dependencies)
                {
                    releasesCSFF.Where(x => x.Name == dep).FirstOrDefault()?.Dependents.Add(release.Name);
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
        
        private void WriteJson(string dir, JSONNode json)
        {
            using (var file = File.Open(dir, FileMode.Create))
            {
                Byte[] bytes = Encoding.UTF8.GetBytes(json.ToString());
                file.Write(bytes, 0, bytes.Length);
                file.Flush();
            }
        }

        private void LoadLocalDllsCSTI()
        {
                string dir = Path.Combine(InstallDirectoryCSTI, @"BepInEx\plugins");
                var files = Directory.GetFiles(dir, "*.dll*", SearchOption.AllDirectories);
                foreach (var dllPath in files)
                {
                    try
                    {
                        var readPar = new ReaderParameters { ReadSymbols = false };
                        using (AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(dllPath, readPar))
                        {
                            string fullName = ad.Name.Name;
                            foreach (var release in releasesCSTI)
                            {
                                if (fullName == release.DllName)
                                {
                                    release.isInstalled = true;
                                    if (release.OnlyDll)
                                    {
                                        release.InstallLocation = Path.GetDirectoryName(dllPath);
                                        release.LocalVersion = ad.Name.Version.ToString();
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
        
        private void LoadLocalDllsCSFF()
        {
            string dir = Path.Combine(InstallDirectoryCSFF, @"BepInEx\plugins");
            var files = Directory.GetFiles(dir, "*.dll*", SearchOption.AllDirectories);
            foreach (var dllPath in files)
            {
                try
                {
                    var readPar = new ReaderParameters { ReadSymbols = false };
                    using (AssemblyDefinition ad = AssemblyDefinition.ReadAssembly(dllPath, readPar))
                    {
                        string fullName = ad.Name.Name;
                        foreach (var release in releasesCSFF)
                        {
                            if (fullName == release.DllName)
                            {
                                release.isInstalled = true;
                                if (release.OnlyDll)
                                {
                                    release.InstallLocation = Path.GetDirectoryName(dllPath);
                                    release.LocalVersion = ad.Name.Version.ToString();
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

        private void LoadLocalModsCSTI()
        {
            string dir = Path.Combine(InstallDirectoryCSTI, @"BepInEx\plugins");
            var files = Directory.GetFiles(dir, "ModInfo.*", SearchOption.AllDirectories);
            foreach (var modinfo in files)
            {
                var json = JSON.Parse(ReadJson(modinfo));
                foreach (var release in releasesCSTI)
                {
                    if (json["Name"] == release.ModName)
                    {
                        release.isInstalled = true;
                        release.InstallLocation = Path.GetDirectoryName(modinfo);
                        if (Path.GetFileName(modinfo) == "ModInfo.disable")
                        {
                            release.isable = false;
                        }

                        if (json["Version"] != "1.0.0")
                        {
                            release.LocalVersion = json["Version"];
                        }
                    }
                }
            }
        }
        
        private void LoadLocalModsCSFF()
        {
            string dir = Path.Combine(InstallDirectoryCSFF, @"BepInEx\plugins");
            var files = Directory.GetFiles(dir, "ModInfo.*", SearchOption.AllDirectories);
            foreach (var modinfo in files)
            {
                var json = JSON.Parse(ReadJson(modinfo));
                foreach (var release in releasesCSFF)
                {
                    if (json["Name"] == release.ModName)
                    {
                        release.isInstalled = true;
                        release.InstallLocation = Path.GetDirectoryName(modinfo);
                        if (Path.GetFileName(modinfo) == "ModInfo.disable")
                        {
                            release.isable = false;
                        }

                        if (json["Version"] != "1.0.0")
                        {
                            release.LocalVersion = json["Version"];
                        }
                    }
                }
            }
        }

        private void LoadRequiredPlugins()
        {
            CheckVersion();
            UpdateStatus("获取Mod信息...");
            LoadReleasesCSTI();
            LoadReleasesCSFF();

            if (InstallDirectoryCSTI != "")
            {
                if (Directory.Exists(Path.Combine(InstallDirectoryCSTI, @"BepInEx")))
                {
                    LoadLocalModsCSTI();
                    LoadLocalDllsCSTI();
                }
            }

            this.Invoke((MethodInvoker)(() =>
            {//Invoke so we can call from current thread
             //Update checkbox's text
                Dictionary<string, int> includedGroups = new Dictionary<string, int>();

                for (int i = 0; i < groupsCSTI.Count(); i++)
                {
                    var key = groupsCSTI.First(x => x.Value == i).Key;
                    var value = listViewModsCSTI.Groups.Add(new ListViewGroup(key, HorizontalAlignment.Left));
                    groupsCSTI[key] = value;
                }

                foreach (ReleaseInfo release in releasesCSTI)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = release.Name;
                    if (!String.IsNullOrEmpty(release.Version))
                        item.Text = $"{release.Name}";
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
                    item.SubItems.Add(release.Version);
                    item.SubItems.Add(release.LocalVersion);
                    item.Tag = release;
                    if (release.Install)
                    {
                        listViewModsCSTI.Items.Add(item);
                    }
                    CheckDefaultMod(release, item);

                    if (release.Group == null || !groupsCSTI.ContainsKey(release.Group))
                    {
                        item.Group = listViewModsCSTI.Groups[groupsCSTI["Uncategorized"]];
                    }
                    else if (groupsCSTI.ContainsKey(release.Group))
                    {
                        int index = groupsCSTI[release.Group];
                        item.Group = listViewModsCSTI.Groups[index];
                    }
                    else
                    {
                        //int index = listViewMods.Groups.Add(new ListViewGroup(release.Group, HorizontalAlignment.Left));
                        //item.Group = listViewMods.Groups[index];
                    }
                    
                    modlistCSTI.Add(release.Name, item);
                }

                tabControlMain.Enabled = true;
                buttonInstall.Enabled = true;

            }));

            if (InstallDirectoryCSFF != "")
            {
                if (Directory.Exists(Path.Combine(InstallDirectoryCSFF, @"BepInEx")))
                {
                    LoadLocalModsCSFF();
                    LoadLocalDllsCSFF();
                }
            }

            this.Invoke((MethodInvoker)(() =>
            {//Invoke so we can call from current thread
             //Update checkbox's text
                Dictionary<string, int> includedGroups = new Dictionary<string, int>();
                
                for (int i = 0; i < groupsCSFF.Count(); i++)
                {
                    var key = groupsCSFF.First(x => x.Value == i).Key;
                    var value = listViewModsCSFF.Groups.Add(new ListViewGroup(key, HorizontalAlignment.Left));
                    groupsCSFF[key] = value;
                }
                foreach (ReleaseInfo release in releasesCSFF)
                {
                    ListViewItem item = new ListViewItem();
                    item.Text = release.Name;
                    if (!String.IsNullOrEmpty(release.Version))
                        item.Text = $"{release.Name}";
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
                    item.SubItems.Add(release.Version);
                    item.SubItems.Add(release.LocalVersion);
                    item.Tag = release;
                    if (release.Install)
                    {
                        listViewModsCSFF.Items.Add(item);
                    }
                    CheckDefaultMod(release, item);

                    if (release.Group == null || !groupsCSFF.ContainsKey(release.Group))
                    {
                        item.Group = listViewModsCSFF.Groups[groupsCSFF["Uncategorized"]];
                    }
                    else if (groupsCSFF.ContainsKey(release.Group))
                    {
                        int index = groupsCSFF[release.Group];
                        item.Group = listViewModsCSFF.Groups[index];
                    }
                    else
                    {
                        //int index = listViewMods.Groups.Add(new ListViewGroup(release.Group, HorizontalAlignment.Left));
                        //item.Group = listViewMods.Groups[index];
                    }
                    
                    modlistCSFF.Add(release.Name, item);
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

        private async void Install()
        {
            string gameversion;
            
            try
            {
                CheckBepinex();
            }
            catch (Exception ex)
            {
                return;
            }
            
            Dictionary<string, List<ReleaseInfo>> gameReleases = new Dictionary<string, List<ReleaseInfo>>
            {
                ["CSTI"] = releasesCSTI,
                ["CSFF"] = releasesCSFF
            };

            Dictionary<string, Dictionary<string, ListViewItem>> gameModlists = new Dictionary<string, Dictionary<string, ListViewItem>>
            {
                ["CSTI"] = modlistCSTI,
                ["CSFF"] = modlistCSFF
            };

            Dictionary<string, string> gameInstallDirectories = new Dictionary<string, string>
            {
                ["CSTI"] = InstallDirectoryCSTI,
                ["CSFF"] = InstallDirectoryCSFF
            };

            if (GameVersion == Game.CSTI)
            {
                gameversion = "CSTI";
                if (InstallDirectoryCSTI == "")
                {
                    throw new Exception("未指定游戏路径");
                }
            }
            else
            {
                gameversion = "CSFF";
                if (InstallDirectoryCSFF == "")
                {
                    throw new Exception("未指定游戏路径");
                }
            }

            List<ReleaseInfo> currentReleases = gameReleases[gameversion];
            Dictionary<string, ListViewItem> currentModlist = gameModlists[gameversion];
            string installDirectory = gameInstallDirectories[gameversion];
            
            ChangeInstallButtonState(false);
            UpdateStatus("开始安装队列...");
            foreach (ReleaseInfo release in currentReleases)
            {
                if (release.Name == "BepInEx")
                {
                    continue;
                }
                
                if (release.Name == "Modloader")
                {
                    if (Directory.Exists(Path.Combine(installDirectory, @"BepInEx\plugins\CSTI-Modloader"))||Directory.Exists(Path.Combine(installDirectory, @"BepInEx\plugins\Modloader")))
                    {
                        continue;
                    }
                }

                if (release.Install)
                {
                    if (release.isInstalled)
                    {
                        try
                        {
                            if (release.OnlyDll)
                            {
                                continue;
                            }

                            Version version1 = new Version(release.Version);
                            Version version2 = new Version(release.LocalVersion);
                            if (version1 <= version2)
                            {
                                continue;
                            }


                            Directory.Delete(release.InstallLocation, true);
                            release.isInstalled = false;
                            release.InstallLocation = null;
                        }
                        catch (Exception ex)
                        {
                            continue;
                        }
                    }
                    
                    UpdateStatus(string.Format("正在下载...{0}", release.Name));
                    byte[] file = await DownloadFile(release.Link, release.Name);
                    UpdateStatus(string.Format("正在安装...{0}", release.Name));
                    string dir;
                    if (release.InstallLocation == null)
                    {
                        dir = Path.Combine(installDirectory, @"BepInEx\plugins");
                    }
                    else
                    {
                        dir = Path.Combine(installDirectory, release.InstallLocation);
                    }

                    UnzipFile(file, dir);
                    currentModlist[release.Name].Text += " (已安装)";
                    currentModlist[release.Name].SubItems.RemoveAt(currentModlist[release.Name].SubItems.Count - 1);
                    currentModlist[release.Name].SubItems.Add(release.Version);
                    release.isInstalled = true;
                    if (!release.OnlyDll)
                    {
                        WriteVersion(release, installDirectory);
                    }
                    UpdateStatus(string.Format("安装 {0}!", release.Name));
                }

            }

            if (gameversion == "CSFF")
            {
                LoadLocalModsCSFF();
                LoadLocalDllsCSFF();
            }

            if (gameversion == "CSTI")
            {
                LoadLocalModsCSTI();
                LoadLocalDllsCSTI();
            }
            UpdateStatus("安装完成!");
            ChangeInstallButtonState(true);
            clearModCheck();
            this.Invoke((MethodInvoker)(() =>
            { //Invoke so we can call from any thread
                buttonToggleMods.Enabled = true;
            }));
        }

        private void WriteVersion(ReleaseInfo release, string InstallDirectory)
        {
            JSONNode tmpjson = string.Empty;
            string dir = Path.Combine(InstallDirectory, @"BepInEx\plugins");
            var files = Directory.GetFiles(dir, "ModInfo.*", SearchOption.AllDirectories);
            foreach (var modinfo in files)
            {
                var json = JSON.Parse(ReadJson(modinfo));
                if (json["Name"] == release.ModName)
                {
                    tmpjson = json;
                    dir = modinfo;
                    tmpjson["Version"] = release.Version;
                    release.LocalVersion = release.Version;
                    WriteJson(dir, tmpjson);
                }
            }
        }

        #endregion // Installation

        #region UIEvents

        private void buttonInstall_Click(object sender, EventArgs e)
        {
            new Thread(() =>
            {
                try
                {
                    Install();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }).Start();
        }

        private void buttonFolderBrowserCSTI_Click(object sender, EventArgs e)
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
                        InstallDirectoryCSTI = Path.GetDirectoryName(path);
                        textBoxDirectory.Text = InstallDirectoryCSTI;
                        Properties.Settings.Default.GamePathCSTI = InstallDirectoryCSTI;
                        Properties.Settings.Default.FindGamePathCSTI = false;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("这不是 Card Survival - Tropical Island.exe! 请重试!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
        }
        
        private void buttonFolderBrowserCSFF_Click(object sender, EventArgs e)
        {
            using (var fileDialog = new OpenFileDialog())
            {
                fileDialog.FileName = "Card Survival - Fantasy Forest.exe";
                fileDialog.Filter = "Exe Files (.exe)|*.exe|All Files (*.*)|*.*";
                fileDialog.FilterIndex = 1;
                if (fileDialog.ShowDialog() == DialogResult.OK)
                {
                    string path = fileDialog.FileName;
                    if (Path.GetFileName(path).Equals("Card Survival - Fantasy Forest.exe"))
                    {
                        InstallDirectoryCSFF = Path.GetDirectoryName(path);
                        textBoxDirectory2.Text = InstallDirectoryCSFF;
                        Properties.Settings.Default.GamePathCSFF = InstallDirectoryCSFF;
                        Properties.Settings.Default.FindGamePathCSFF = false;
                        Properties.Settings.Default.Save();
                    }
                    else
                    {
                        MessageBox.Show("这不是 Card Survival - Fantasy Forest.exe! 请重试!", "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                }

            }
        }
        
        public static string FindGameInstallPath(string version, string data)
        {
            // 构建 Player.log 文件路径
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string logFilePath = Path.Combine(appDataPath, $@"..\LocalLow\WinterSpring Games\{version}\Player.log");
        
            // 确保文件存在
            if (!File.Exists(logFilePath))
            {
                throw new FileNotFoundException("Player.log 文件未找到", logFilePath);
            }
        
            // 读取文件第一行
            string firstLine;
            using (StreamReader reader = new StreamReader(logFilePath))
            {
                firstLine = reader.ReadLine();
            }
        
            if (string.IsNullOrEmpty(firstLine))
            {
                throw new InvalidDataException("Player.log 文件第一行为空");
            }
        
            // 使用正则表达式提取路径
            var match = Regex.Match(firstLine, $@"Mono path\[0\] = '(.+?)\\{data}\\Managed'");
            if (!match.Success)
            {
                // 尝试其他可能的路径格式
                match = Regex.Match(firstLine, $@"Mono path\[0\] = '(.+?)/{data}/Managed'");
            }
        
            if (!match.Success)
            {
                throw new FormatException("无法从 Player.log 中解析游戏路径");
            }
        
            // 获取提取的路径
            string extractedPath = match.Groups[1].Value;
        
            // 标准化路径格式
            string normalizedPath = extractedPath
                .Replace('/', Path.DirectorySeparatorChar)
                .Replace('\\', Path.DirectorySeparatorChar);
        
            // 确保路径以目录分隔符结尾
            if (!normalizedPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                normalizedPath += Path.DirectorySeparatorChar;
            }

            if (!Directory.Exists(normalizedPath))
            {
                throw new FormatException("游戏路径不存在");
            } 
            
            return normalizedPath;
        }
        
        
        private void tabControlMain_SelectedIndexChanged(object sender, EventArgs e)
        {
            TabControl tabControl = sender as TabControl;
            if (tabControl != null)
            {
                TabPage currentTab = tabControl.SelectedTab;
                // 根据当前标签页执行不同操作
                if (currentTab == CSTI)
                {
                    GameVersion = Game.CSTI;
                    foreach (var item in modlistCSFF)
                    {
                        item.Value.Checked = false;
                    }
                }
                else if (currentTab == CSFF)  // 假设这是你新增的标签页
                {
                    GameVersion = Game.CSFF;
                    foreach (var item in modlistCSTI)
                    {
                        item.Value.Checked = false;
                    }
                }
            }
        }

        private void listViewMods_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            ReleaseInfo release = (ReleaseInfo)e.Item.Tag;
            try
            {
                Version version1 = new Version(release.Version);
                Version version2 = new Version(release.LocalVersion);
                if (release.isInstalled && e.Item.Checked)
                {
                    if (version1 > version2)
                    {
                        buttonModInfo.Enabled = true;
                        buttonInstall.Enabled = true;
                    }
                    else
                    {
                        buttonModInfo.Enabled = true;
                        buttonInstall.Enabled = false;   
                    }
                }
                else
                {
                    buttonModInfo.Enabled = false;
                    buttonInstall.Enabled = true;
                }
            }
            catch (Exception ex)
            {
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
            }

            if (release.Name.Contains("BepInEx"))
            {
                e.Item.Checked = true;
            }
            release.Install = e.Item.Checked;
            
            if (release.Name.Contains("Modloader"))
            {
                e.Item.Checked = true;
            }
            release.Install = e.Item.Checked;

            if (release.isInstalled)
            {
                buttonToggleMods.Enabled = true;
            }
            else
            {
                buttonToggleMods.Enabled = false;
            }
        }

        private void listViewMods_DoubleClick(object sender, EventArgs e)
        {
            OpenLinkFromRelease();
        }

        private void buttonModInfo_Click(object sender, EventArgs e)
        {
            string gameversion;
            
            var confirmResult = MessageBox.Show(
                "你正在尝试删除所选Mods文件. 该操作不可撤销!\n\n你确定要继续吗?",
                "确认删除",
                MessageBoxButtons.YesNo);

            Dictionary<string, List<ReleaseInfo>> gameReleases = new Dictionary<string, List<ReleaseInfo>>
            {
                ["CSTI"] = releasesCSTI,
                ["CSFF"] = releasesCSFF
            };

            Dictionary<string, Dictionary<string, ListViewItem>> gameModlists = new Dictionary<string, Dictionary<string, ListViewItem>>
            {
                ["CSTI"] = modlistCSTI,
                ["CSFF"] = modlistCSFF
            };

            Dictionary<string, string> gameInstallDirectories = new Dictionary<string, string>
            {
                ["CSTI"] = InstallDirectoryCSTI,
                ["CSFF"] = InstallDirectoryCSFF
            };

            if (GameVersion == Game.CSTI)
            {
                gameversion = "CSTI";
            }
            else
            {
                gameversion = "CSFF";
            }
            
            List<ReleaseInfo> currentReleases = gameReleases[gameversion];
            Dictionary<string, ListViewItem> currentModlist = gameModlists[gameversion];
            string installDirectory = gameInstallDirectories[gameversion];

            if (confirmResult == DialogResult.Yes)
            {
                UpdateStatus("删除mod!");
                foreach (var release in currentReleases)
                {
                    if (release.isInstalled)
                    {
                        if (release.Name.Contains("BepInEx") || release.Name.Contains("Modloader"))
                        {
                            continue;
                        }
                        else if (release.InstallLocation == Path.Combine(installDirectory, @"BepInEx\plugins"))
                        {
                            continue;
                        }
                        else
                        {
                            try
                            {
                                Directory.Delete(release.InstallLocation, true);
                                currentModlist[release.Name].Text = currentModlist[release.Name].Text.Replace(" (已安装)", "");
                                currentModlist[release.Name].Text = currentModlist[release.Name].Text.Replace(" (已禁用)", "");
                                release.isInstalled = false;
                                release.InstallLocation = null;
                                currentModlist[release.Name].SubItems.RemoveAt(currentModlist[release.Name].SubItems.Count - 1);
                                currentModlist[release.Name].SubItems.Add("0.0.0");
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.ToString()+release.InstallLocation, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                }
                clearModCheck();
            }
            UpdateStatus("删除完成!");
        }

        private void viewInfoToolStripMenuItem_Click(object sender, EventArgs e)
         {
             OpenLinkFromRelease();
         }
        

        private void clearModCheck()
        {
            foreach (var item in modlistCSTI)
            {
                item.Value.Checked = false;
            }
            
            foreach (var item in modlistCSFF)
            {
                item.Value.Checked = false;
            }
        }

        #region Folders

        private void buttonOpenLogFolderCSTI_Click(object sender, EventArgs e)
        {
            var configDirectory = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"..\LocalLow\WinterSpring Games\Card Survival - Tropical Island\");
            if (Directory.Exists(configDirectory))
                Process.Start(configDirectory);
        }

        private void buttonOpenLogFolderCSFF_Click(object sender, EventArgs e)
        {
            var configDirectory = Path.Combine(Environment.GetFolderPath(
                Environment.SpecialFolder.ApplicationData), @"..\LocalLow\WinterSpring Games\Card Survival - Fantasy Forest\");
            if (Directory.Exists(configDirectory))
                Process.Start(configDirectory);
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
        
        public async Task<byte[]> DownloadFile(string url, string modname)
        {
            using (WebClient client = new WebClient())
            {
                client.Proxy = null;
        
                // 创建进度报告对象，现在包含下载速度
                var progress = new Progress<(long BytesReceived, long TotalBytes, int Percentage, string Speed)>(report =>
                {
                    var (bytesReceived, totalBytes, percentage, speed) = report;
                    if (totalBytes > 0)
                    {
                        UpdateStatus($"下载{modname}中: \n{percentage}% ({FormatBytes(bytesReceived)}/{FormatBytes(totalBytes)}) {speed}");
                    }
                    else
                    {
                        UpdateStatus($"已下载: {FormatBytes(bytesReceived)}");
                    }
                });

                return await DownloadDataWithProgressAsync(client, url, progress);
            }
        }

        private async Task<byte[]> DownloadDataWithProgressAsync(WebClient client, string url,
            IProgress<(long, long, int, string)> progress)
        {
            try
            {
                UpdateStatus("正在连接服务器...");

                // 先获取文件大小
                long totalBytes = -1;
                try
                {
                    client.OpenRead(url);
                    totalBytes = Convert.ToInt64(client.ResponseHeaders["Content-Length"]);
                    client.CancelAsync();
                    UpdateStatus($"文件大小: {FormatBytes(totalBytes)}");
                }
                catch
                {
                    UpdateStatus("无法获取文件大小信息");
                }

                UpdateStatus("开始下载...");

                var downloadTask = client.DownloadDataTaskAsync(url);

                // 监控下载进度和计算速度
                long lastBytesReceived = 0;
                DateTime lastUpdateTime = DateTime.Now;
                string lastSpeed = "0 KB/s";

                client.DownloadProgressChanged += (sender, e) =>
                {
                    var currentTime = DateTime.Now;
                    var timeElapsed = (currentTime - lastUpdateTime).TotalSeconds;

                    // 计算下载速度（基于上次更新的数据）
                    if (timeElapsed > 0.1) // 至少0.1秒才更新速度，避免抖动
                    {
                        var bytesDiff = e.BytesReceived - lastBytesReceived;
                        var speedBytesPerSecond = bytesDiff / timeElapsed;
                        lastSpeed = $"{FormatBytes((long)speedBytesPerSecond)}/s";

                        // 限制更新频率，避免UI卡顿
                        if (currentTime - lastUpdateTime > TimeSpan.FromMilliseconds(200) ||
                            e.BytesReceived == e.TotalBytesToReceive)
                        {
                            progress?.Report((e.BytesReceived, e.TotalBytesToReceive, e.ProgressPercentage, lastSpeed));
                            lastUpdateTime = currentTime;
                            lastBytesReceived = e.BytesReceived;
                        }
                    }
                };

                byte[] result = await downloadTask;

                // 下载完成时显示最终状态
                progress?.Report((totalBytes, totalBytes, 100, "完成"));
                UpdateStatus("下载完成！");

                return result;
            }
            catch (Exception ex)
            {
                UpdateStatus($"下载失败: {ex.Message}");
                throw;
            }
        }

        private string FormatBytes(long bytes)
        {
            if (bytes == 0) return "0 B";
            string[] sizes = { "B", "KB", "MB", "GB" };
            int order = 0;
            double len = bytes;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            // 根据大小选择合适的显示精度
            string format = order == 0 ? "0" : (len < 10 ? "0.0" : "0");
            return $"{len.ToString(format)} {sizes[order]}";
        }

        private void UpdateStatus(string status)
        {
            string formattedText = string.Format("{0}", status);
            this.Invoke((MethodInvoker)(() =>
            { //Invoke so we can call from any thread
                labelStatus.Text = formattedText;
            }));
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

        private async void CheckBepinex()
        {
            string download_url =
                "https://gitee.com/Cold_winds/BepInEx/releases/download/5.4.22.0/BepInEx_x64_5.4.22.0%EF%BC%88%E8%A7%A3%E5%8E%8B%E5%88%B0%E6%B8%B8%E6%88%8F%E6%A0%B9%E7%9B%AE%E5%BD%95%EF%BC%89.zip";
            
            if (GameVersion == Game.CSTI)
            {
                if (Directory.Exists(Path.Combine(InstallDirectoryCSTI, @"BepInEx")))
                {
                    return;
                }
            }
            else
            {
                if (Directory.Exists(Path.Combine(InstallDirectoryCSFF, @"BepInEx")))
                {
                    return;
                }
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
                    byte[] file = await DownloadFile(download_url, "BepInEx");
                    UpdateStatus(string.Format("正在安装...BepInEx"));
                    if (GameVersion == Game.CSTI)
                    {
                        UnzipFile(file, InstallDirectoryCSTI);
                        Process.Start(Path.Combine(InstallDirectoryCSTI, @"Card Survival - Tropical Island.exe"));
                    }
                    else
                    {
                        UnzipFile(file, InstallDirectoryCSFF);
                        Process.Start(Path.Combine(InstallDirectoryCSFF, @"Card Survival - Fantasy Forest.exe"));
                    }
                }
                else
                {
                    throw new Exception("用户取消操作");
                }
            }
            else
            {
                throw new Exception("用户取消操作");
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
            if (listViewModsCSTI.SelectedItems.Count > 0)
            {
                ReleaseInfo release = (ReleaseInfo)listViewModsCSTI.SelectedItems[0].Tag;
                UpdateStatus($"打开gitee页面 {release.Name}");
                Process.Start(string.Format("https://gitee.com/{0}", release.GitPath));
            }
            
        }

#endregion // Helpers

#region Registry

        private void LocationHandler()
        {
            try
            {
                string gamePath = FindGameInstallPath("Card Survival - Tropical Island", "Card Survival - Tropical Island_Data");
                textBoxDirectory.Text = gamePath;
                InstallDirectoryCSTI = gamePath;
            }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.FindGamePathCSTI)
                {
                    MessageBox.Show($"自动检测热带岛屿游戏路径失败: {ex.Message}，请稍后手动添加。");
                }
                else
                {
                    string gamePath = Properties.Settings.Default.GamePathCSTI;
                    textBoxDirectory.Text = gamePath;
                    InstallDirectoryCSTI = gamePath;
                }
            }
            try
            {
                string gamePath = FindGameInstallPath("Card Survival - Fantasy Forest", "Card Survival - Fantasy Forest_Data");
                textBoxDirectory2.Text = gamePath;
                InstallDirectoryCSFF = gamePath;
            }
            catch (Exception ex)
            {
                if (Properties.Settings.Default.FindGamePathCSFF)
                {
                    MessageBox.Show($"自动检测奇幻森林路径失败: {ex.Message}，请稍后手动添加。");
                }
                else
                {
                    string gamePath = Properties.Settings.Default.GamePathCSFF;
                    textBoxDirectory2.Text = gamePath;
                    InstallDirectoryCSFF = gamePath;
                }
            }
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
            string gameversion;
            
            Dictionary<string, List<ReleaseInfo>> gameReleases = new Dictionary<string, List<ReleaseInfo>>
            {
                ["CSTI"] = releasesCSTI,
                ["CSFF"] = releasesCSFF
            };

            Dictionary<string, Dictionary<string, ListViewItem>> gameModlists = new Dictionary<string, Dictionary<string, ListViewItem>>
            {
                ["CSTI"] = modlistCSTI,
                ["CSFF"] = modlistCSFF
            };

            if (GameVersion == Game.CSTI)
            {
                gameversion = "CSTI";
            }
            else
            {
                gameversion = "CSFF";
            }

            List<ReleaseInfo> currentReleases = gameReleases[gameversion];
            Dictionary<string, ListViewItem> currentModlist = gameModlists[gameversion];
            
            try
            {
                foreach (var release in currentReleases)
                {
                    if (release.Install)
                    {
                        if (release.Name.Contains("BepInEx") || release.Name.Contains("Modloader"))
                        {
                            continue;
                        }
                        else if(release.isInstalled)
                        {
                            if (release.isable)
                            {
                                if (!release.OnlyDll)
                                {
                                    File.Move(Path.Combine(release.InstallLocation, "ModInfo.json"),
                                        Path.Combine(release.InstallLocation, "ModInfo.disable"));
                                    release.isable = false;
                                    if (currentModlist.ContainsKey(release.Name))
                                    {
                                        currentModlist[release.Name].Text += " (已禁用)";
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
                                    if (currentModlist.ContainsKey(release.Name))
                                    {
                                        currentModlist[release.Name].Text += " (已禁用)";
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
                                    if (currentModlist.ContainsKey(release.Name))
                                    {
                                        currentModlist[release.Name].Text = currentModlist[release.Name].Text.Replace(" (已禁用)", "");
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
                                    if (currentModlist.ContainsKey(release.Name))
                                    {
                                        currentModlist[release.Name].Text = currentModlist[release.Name].Text.Replace(" (已禁用)", "");
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
                clearModCheck();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "错误!", MessageBoxButtons.OK, MessageBoxIcon.Error);;
            }
        }
    }

}
