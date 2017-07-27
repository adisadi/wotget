using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace WoTget.Core.Authoring
{
    class WotModPackageBuilder
    {

        internal static Stream CreateWotModPackage(IPackage package, IEnumerable<string> files, string rootFolder)
        {
            var memoryStream = new MemoryStream();
            using (var archive = ZipStorer.Create(memoryStream, string.Empty, true))
            {

                using (var manifestStream = new MemoryStream())
                {
                    MetaHelper.Save(package, manifestStream);
                    manifestStream.Seek(0, SeekOrigin.Begin);
                    archive.AddStream(ZipStorer.Compression.Store, MetaHelper.MetaFileName, manifestStream, DateTime.Now, "");
                }

                //Save Files
                foreach (var file in files)
                {
                    var entryName = PackageHelper.RemoveUntilFolder(file, rootFolder);
                    entryName = Path.Combine("res\\", entryName.TrimStart(Path.DirectorySeparatorChar));
                    CreateDirectoryEntry(archive, file, rootFolder);
                    archive.AddFile(ZipStorer.Compression.Store, file, entryName, "");
                }
            }
            return memoryStream;
        }

        private static void CreateDirectoryEntry(ZipStorer archive, string path, string rootFolder)
        {
            var originalPath = Path.GetDirectoryName(path);
            var entryName = PackageHelper.RemoveUntilFolder(originalPath, rootFolder);
            if (String.IsNullOrEmpty(entryName))
            {
                archive.AddStream(ZipStorer.Compression.Store, "res/", null, DateTime.Now, "");
                return;
            }
            entryName = Path.Combine("res\\", entryName.TrimStart(Path.DirectorySeparatorChar));
            var dirInfo = new DirectoryInfo(originalPath);
            archive.AddStream(ZipStorer.Compression.Store, entryName.Replace("\\", "/").Trim('/') + "/", null, DateTime.Now, "");

            var parentPath = originalPath.Replace(dirInfo.Name, "");
            if (!string.IsNullOrEmpty(parentPath))
            {
                CreateDirectoryEntry(archive, parentPath, rootFolder);
            }
        }

        internal static Stream CreateWotModPackage(IPackage package, string wotmodFile)
        {
            if (!wotmodFile.EndsWith(Constants.WotModExtension)) throw new ArgumentException("no wotmod fileending!");

            var memoryStream = new MemoryStream();
            using (var archive = ZipStorer.Create(memoryStream, string.Empty, true))
            {

                using (var zip = ZipStorer.Open(wotmodFile, FileAccess.Read))
                {
                    // Read the central directory collection
                    List<ZipStorer.ZipFileEntry> dir = zip.ReadCentralDir();

                    foreach (var e in dir)
                    {
                        if (e.FilenameInZip == MetaHelper.MetaFileName) continue;
                        using (var fileStream = new MemoryStream())
                        {
                            zip.ExtractFile(e, fileStream);
                            fileStream.Seek(0, SeekOrigin.Begin);
                            archive.AddStream(ZipStorer.Compression.Store, e.FilenameInZip, fileStream, DateTime.Now, "");
                        }
                    }
                }

                using (var manifestStream = new MemoryStream())
                {
                    MetaHelper.Save(package, manifestStream);
                    manifestStream.Seek(0, SeekOrigin.Begin);
                    archive.AddStream(ZipStorer.Compression.Store, MetaHelper.MetaFileName, manifestStream, DateTime.Now, "");
                }
            }
            return memoryStream;

        }
    }
}
