using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WoTget.Core
{
    public static class GitChecker
    {
        public static Version CurrentVersion = Assembly.GetEntryAssembly().GetName().Version;
        public static Version CheckNewVersion()
        {
            try
            {
                var match =
                    new Regex(
                        @"\[assembly\: AssemblyVersion\(""(\d{1,})\.(\d{1,})\.(\d{1,})\.(\d{1,})""\)\]")
                        .Match(DownloadServerVersion(Assembly.GetEntryAssembly().GetName().Name));

                if (!match.Success) return null;
                var gitVersion =
                    new Version(
                        $"{match.Groups[1]}.{match.Groups[2]}.{match.Groups[3]}.{match.Groups[4]}");

                if (gitVersion <= CurrentVersion)
                {
                    return null;
                }
                return gitVersion;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static string DownloadServerVersion(string dllName)
        {
            //test
            using (var wC = new WebClient())
                return
                    wC.DownloadString(
                        "https://raw.githubusercontent.com/adisadi/wotget/master/" + dllName + "/Properties/AssemblyInfo.cs");
        }

        public static void DownloadFile(Version version, string downloadPath)
        {
            var fileName = Path.GetFileName(Assembly.GetEntryAssembly().CodeBase);
            using (var webClient = new WebClient())
                webClient.DownloadFile("https://github.com/adisadi/wotget/releases/download/V" + version.ToString() + "/" + fileName, Path.Combine(downloadPath));
        }
    }

}
