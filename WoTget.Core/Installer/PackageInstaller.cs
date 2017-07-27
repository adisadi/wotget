using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WoTget.Core.Installer
{
    public class PackageInstaller : IPackageInstaller
    {
        public PackageInstaller() { }

        public IEnumerable<string> InstallPackageStream(Stream packageStream, string wotHome)
        {
            //WotHome\mods
            var destinationPath = Path.Combine(wotHome, Constants.ModsFolder);
            if (!Directory.Exists(destinationPath)) Directory.CreateDirectory(destinationPath);

            //WotHome\mods\0.9.xxxx
            var modsVersionPath = Path.Combine(destinationPath, WoTHelper.GetWoTVersion(wotHome));
            if (!Directory.Exists(modsVersionPath)) Directory.CreateDirectory(modsVersionPath);

            packageStream.Seek(0, SeekOrigin.Begin);
            List<string> files = new List<string>();
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.Where(en => !string.IsNullOrEmpty(en.Name)))
                {
                    if (entry.FullName.EndsWith(Constants.WotModExtension))
                    {
                        entry.ExtractToFile(Path.Combine(modsVersionPath, entry.FullName), true);
                        continue;
                    }

                    var entryFullName = Path.Combine(destinationPath, entry.FullName);

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
            var destinationPath = Path.Combine(wotHome, Constants.ModsFolder);
            if (!Directory.Exists(destinationPath)) return;

            packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.Where(en => !string.IsNullOrEmpty(en.Name)))
                {

                    if (entry.FullName.EndsWith(Constants.WotModExtension))
                    {
                        var modsVersionPath = Path.Combine(destinationPath, WoTHelper.GetWoTVersion(wotHome));
                        if (!Directory.Exists(modsVersionPath)) continue;
                        if (File.Exists(Path.Combine(modsVersionPath, entry.FullName))) File.Delete(Path.Combine(modsVersionPath, entry.FullName));

                        continue;
                    }

                    var entryFullName = Path.Combine(destinationPath, entry.FullName);

                    if (File.Exists(entryFullName)) File.Delete(entryFullName);

                    if (Directory.Exists(Path.GetDirectoryName(entryFullName)))
                    {
                        if (Directory.GetFiles(Path.GetDirectoryName(entryFullName), "*", SearchOption.AllDirectories).Length == 0)
                        {
                            Directory.Delete(Path.GetDirectoryName(entryFullName), true);
                        }
                    }
                }
            }
        }


    }
}
