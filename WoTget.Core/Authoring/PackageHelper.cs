using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WoTget.Core.Authoring
{
    public static class PackageHelper
    {
        public static string FileName(this IPackage package)
        {
            return $"{package.Id}.{package.SemanticVersion().ToNormalizedString()}{Constants.PackageExtension}";
        }

        public static SemanticVersion SemanticVersion(this IPackage package)
        {
            return new SemanticVersion(package.Version);
        }


        public static IEnumerable<IPackage> FindByName(this IEnumerable<IPackage> packages, string name)
        {
            return packages.Where(p => p.Name.ToLower() == name.ToLower());
        }

        public static IPackage FindByNameAndVersion(this IEnumerable<IPackage> packages, IPackage packageToSearch)
        {
            return FindByNameAndVersion(packages,packageToSearch.Name,packageToSearch.Version);
        }

        public static IPackage FindByNameAndVersion(this IEnumerable<IPackage> packages, string name, string version)
        {
            return packages.SingleOrDefault(p => p.Name.ToLower() == name.ToLower() && p.SemanticVersion() == new SemanticVersion(version));
        }

        public static bool ExistsByName(this IEnumerable<IPackage> packages, string name)
        {
            return packages.Any(p => p.Name.ToLower() == name.ToLower());
        }

        public static bool ExistsByName(this IEnumerable<IPackage> packages, IPackage packageToSearch)
        {
            return ExistsByName(packages,packageToSearch.Name);
        }

        public static bool ExistsByNameAndVersion(this IEnumerable<IPackage> packages, string name,string version)
        {
            return packages.Any(p => p.Name.ToLower() == name.ToLower() && p.SemanticVersion()==new SemanticVersion(version) );
        }

        public static bool ExistsByNameAndVersion(this IEnumerable<IPackage> packages, IPackage packageToSearch)
        {
            return ExistsByNameAndVersion(packages,packageToSearch.Name,packageToSearch.Version);
        }

        public static IEnumerable<IPackage> OnlyLatestVersion(this IEnumerable<IPackage> packages)
        {
            return packages.Where(p => packages.Where(p1 => p1.Name == p.Name).Select(p2 => new SemanticVersion(p2.Version)).Max().ToNormalizedString() == p.SemanticVersion().ToNormalizedString());
        }

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
