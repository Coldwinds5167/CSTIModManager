using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSTIModManager.Internals.SimpleJSON;

namespace CSTIModManager.Internals
{
    public class ReleaseInfo
    {
        public string Version;
        public string Link;
        public string Name;
        public string ModName;
        public string Author;
        public string GitPath;
        public string Tag;
        public string Group;
        public string InstallLocation;
        public int ReleaseId;
        public bool Install = true;
        public bool isInstalled = false;
        public bool isable = true;
        public List<string> Dependencies = new List<string>();
        public List<string> Dependents = new List<string>();
        public ReleaseInfo(string _name, string _modname, string _author, string _version, string _group, string _link, string _installLocation, string _gitPath, JSONArray dependencies)
        {
            Name = _name;
            ModName = _modname;
            Author = _author;
            Version = _version;
            Group = _group;
            Link = _link;
            GitPath = _gitPath;
            InstallLocation = _installLocation;
            Group = _group;

            if (dependencies == null) return;
            for (int i = 0; i < dependencies.Count; i++)
            {
                Dependencies.Add(dependencies[i]);
            }
        }
    }
}
