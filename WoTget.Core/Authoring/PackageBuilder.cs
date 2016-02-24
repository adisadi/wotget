using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WoTget.Core.Authoring
{
    public class PackageBuilder
    {
        public static Stream CreatePackage(IPackage package, IEnumerable<string> files)
        {

            if (!files.All(f => f.Contains(Constants.PackageRootFolder)))
                throw new ArgumentException($"All files must be inside '{Constants.PackageRootFolder}' Folder");

            var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file)) continue;
                    if (!File.Exists(file)) continue;
                  
              
                    var entryName = file.Substring(file.IndexOf(Constants.PackageRootFolder));

                    if (entryName == Constants.ManifestFileName) continue; //Don't write Manifestfile will be generated and added later

                    archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }

                var manifestEntry = archive.CreateEntry(Constants.ManifestFileName);
                using (var manifestStream = manifestEntry.Open())
                {
                    ManifestHelper.Save(package, manifestStream);
                }

            }

            return memoryStream;
        }
    }
}
