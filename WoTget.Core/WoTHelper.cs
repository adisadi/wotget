using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace WoTget.Core
{
    public static class WoTHelper
    {
        public static string GetResModsFolder(string WoTHome)
        {
            var xdoc = XDocument.Load(Path.Combine(WoTHome, "paths.xml"));
            var node = xdoc.Descendants().SingleOrDefault(p => p.Name == "Path" && p.Value.Contains("res_mods"));

            return Path.Combine(WoTHome, node.Value.Trim().TrimStart('.'));

        }

        public static string GetWoTVersion(string WoTHome)
        {
            return new DirectoryInfo(GetResModsFolder(WoTHome)).Name;
        }
    }


   
   
}
