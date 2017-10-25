using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WoTget.Core.Installer
{
    public class PackageInstaller 
    {
        public PackageInstaller() { }

        public IEnumerable<string> InstallPackageStream(Stream packageStream, string wotHome)
        {

            var wotVersion = WoTHelper.GetWoTVersion(wotHome);
            //WotHome\mods
            var modsPath = Path.Combine(wotHome, Constants.ModsFolder);
            if (!Directory.Exists(modsPath)) Directory.CreateDirectory(modsPath);

            //WotHome\mods\0.9.xxxx
            var modsVersionPath = Path.Combine(modsPath, wotVersion);
            if (!Directory.Exists(modsVersionPath)) Directory.CreateDirectory(modsVersionPath);

            //WotHome\res_mods
            var resmodsPath = Path.Combine(wotHome, Constants.ResModsFolder);
            if (!Directory.Exists(resmodsPath)) Directory.CreateDirectory(resmodsPath);

            //WotHome\res_mods\0.9.xxxx
            var resmodsVersionPath = Path.Combine(resmodsPath, wotVersion);
            if (!Directory.Exists(resmodsVersionPath)) Directory.CreateDirectory(resmodsVersionPath);


            packageStream.Seek(0, SeekOrigin.Begin);
            List<string> files = new List<string>();
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.Where(en => !string.IsNullOrEmpty(en.Name)))
                {
                    var entryFullName = Path.Combine(wotHome, entry.FullName);
                    entryFullName = entryFullName.Replace(Constants.VersionPlaceHolder, wotVersion);

                    if (!Directory.Exists(Path.GetDirectoryName(entryFullName)))
                        Directory.CreateDirectory(Path.GetDirectoryName(entryFullName));

                    entry.ExtractToFile(entryFullName, true);
                    files.Add(entryFullName);
                }
            }

            return files;
        }

        public void UninstallPackageStream(Stream packageStream, string wotHome)
        {
            var wotVersion = WoTHelper.GetWoTVersion(wotHome);

            packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.OrderBy(en=> string.IsNullOrEmpty(en.Name)))
                {
                    var entryFullName = Path.Combine(wotHome, entry.FullName);
                    entryFullName = entryFullName.Replace("_version_", wotVersion);

                    if (File.Exists(entryFullName)) File.Delete(entryFullName);

                    rekDeleteDirectory(Path.GetDirectoryName(entryFullName));
                }
            }
        }

        private void rekDeleteDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                if (Directory.GetFiles(path, "*", SearchOption.AllDirectories).Length == 0)
                {
                    Directory.Delete(path, true);
                    rekDeleteDirectory(Directory.GetParent(path).FullName);
                }
            }
        }

    }
}