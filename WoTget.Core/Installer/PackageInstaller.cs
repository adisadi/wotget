using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WoTget.Core.Installer
{
    public class PackageInstaller:IPackageInstaller
    {
        public PackageInstaller() { }

        public  IEnumerable<string> InstallPackageStream(Stream packageStream,string destinationPath)
        {
            packageStream.Seek(0, SeekOrigin.Begin);
            List<string> files = new List<string>();
            using (var archive = new ZipArchive(packageStream,ZipArchiveMode.Read, true))
            {        
                foreach (ZipArchiveEntry entry in archive.Entries.Where(en => !string.IsNullOrEmpty(en.Name) && en.Name != Constants.ManifestFileName))
                {
                    var entryFullName = Path.Combine(destinationPath, entry.FullName);

                    if (!Directory.Exists(Path.GetDirectoryName(entryFullName)))
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(entryFullName));
                    }

                    entry.ExtractToFile(entryFullName, true);
                    files.Add(entryFullName);
                }
            }

            return files;
        }

        public  void UninstallPackageStream(Stream packageStream, string destinationPath)
        {
            packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream,ZipArchiveMode.Read,true))
            {
                foreach (ZipArchiveEntry entry in archive.Entries.Where(en => !string.IsNullOrEmpty(en.Name) && en.Name != Constants.ManifestFileName))
                {
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
