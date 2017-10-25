using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WoTget.Core.Authoring
{
    public static class PackageHelper
    {
        public static string GetWotVersionFolder(string path)
        {
            var match = Regex.Match(path, "(?<=\\\\)([0-9]{1,2}\\.?)+(?=\\\\)");
            if (match.Success)
                return match.Value;
            return string.Empty;
        }

        public static string RemoveUntilFolder(string folder, string untilFolder)
        {
            if (untilFolder.Length>folder.Length) return "";
            return folder.Substring(folder.IndexOf(untilFolder) + untilFolder.Length).Trim(Path.DirectorySeparatorChar);
        }
    }
}
