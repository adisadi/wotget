
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using static WoTget.Core.Format.FormatDetect;

namespace WoTget.Core.Authoring
{
    public class PackageBuilder
    {
        public static Stream CreatePackage(IPackage package, IEnumerable<string> files, string rootFolder)
        {
            //Check if Id,Name & Version are set
            if (string.IsNullOrEmpty(package.Name) || string.IsNullOrEmpty(package.Version))
                throw new ArgumentException("Package Name and Version must be set!");

            if (package.Tags == null) package.Tags = new List<string>();

            //detect format
            var format = Analize(files);

            //Zip Temporary unpack
            if (files.Count() == 1)
            {
                var file = files.First();
                if (file.ToLower().EndsWith(".zip"))
                {
                    string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                    Directory.CreateDirectory(tempDirectory);
                    using (var zip = new ZipArchive(File.OpenRead(file), ZipArchiveMode.Read, false))
                    {
                        zip.ExtractToDirectory(tempDirectory);
                    }

                    var root = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(file));
                    var retStream = CreatePackage(package, Directory.GetFiles(root, "*", SearchOption.AllDirectories), root);
                    Directory.Delete(tempDirectory, true);
                    return retStream;
                }
            }


            if (format == PackageFormat.NoFormat) throw new ArgumentException("Could not detect format!");

            switch (format)
            {
                case PackageFormat.WotHomeRoot:
                case PackageFormat.ResModRoot:
                case PackageFormat.VersionRoot:
                    return CreatePackageResMods(package, files, rootFolder, format);
                case PackageFormat.WotMod:
                    return CreatePackageWotMod(package, files.First());
                case PackageFormat.WotModHomeRoot:
                case PackageFormat.WotModModsRoot:
                    return CreatePackageWotModWithFiles(package, files, rootFolder);
                default:
                    throw new ArgumentException("Format:" + format + " not implemented!");
            }
        }

        private static Stream CreatePackageWotModWithFiles(IPackage package, IEnumerable<string> files, string rootFolder)
        {
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    if (string.IsNullOrEmpty(file)) continue;
                    if (!File.Exists(file)) continue;

                    if (file.EndsWith(Constants.WotModExtension))
                    {
                        using (var wotModStream = WotModPackageBuilder.CreateWotModPackage(package, file))
                        {
                            var entry = archive.CreateEntry("wotget." + Path.GetFileName(file));
                            using (var entryStream = entry.Open())
                            {
                                wotModStream.Seek(0, SeekOrigin.Begin);
                                wotModStream.CopyTo(entryStream);
                            }
                        }
                        
                        continue;
                    }

                    //Other files configs...
                    var entryName = PackageHelper.RemoveUntilFolder(file, rootFolder);
                    entryName = PackageHelper.RemoveUntilFolder(entryName, "mods");
                    archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            }
            return memoryStream;
        }

        private static Stream CreatePackageWotMod(IPackage package, string file)
        {
            var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                using (var wotModStream = WotModPackageBuilder.CreateWotModPackage(package, file))
                {
                    var entry = archive.CreateEntry("wotget." + Path.GetFileName(file));
                    using (var entryStream = entry.Open())
                    {
                        wotModStream.Seek(0, SeekOrigin.Begin);
                        wotModStream.CopyTo(entryStream);
                    }
                }
            }
            return memoryStream;
        }

        private static Stream CreatePackageResMods(IPackage package, IEnumerable<string> files, string rootFolder, PackageFormat format)
        {
            var memoryStream = new MemoryStream();
            List<string> wotModFiles = new List<string>();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {

                if (format == PackageFormat.WotHomeRoot || format == PackageFormat.ResModRoot)
                {
                    foreach (var file in files)
                    {
                        if (string.IsNullOrEmpty(file)) continue;
                        if (!File.Exists(file)) continue;


                        var wotVersion = PackageHelper.GetWotVersionFolder(file);
                        if (wotVersion != string.Empty)
                        {
                            wotModFiles.Add(file);
                            continue;
                        }

                        //Other files configs...
                        var entryName = PackageHelper.RemoveUntilFolder(file, rootFolder);
                        entryName = PackageHelper.RemoveUntilFolder(entryName, "res_mods");
                        archive.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                    }
                }
                else if (format == PackageFormat.VersionRoot)
                {
                    wotModFiles.AddRange(files);
                }
                else
                    throw new ArgumentException("Format:" + format + " not implemented!");


                //Create wotmod zip
                if (wotModFiles.Count > 0)
                {
                    var wotVersion = PackageHelper.GetWotVersionFolder(wotModFiles[0]);

                    var wotModEntry = archive.CreateEntry("wotget." + package.Id.ToLower() + Constants.WotModExtension);
                    using (var wotModStream = wotModEntry.Open())
                    {
                        var stream = WotModPackageBuilder.CreateWotModPackage(package, wotModFiles, format == PackageFormat.VersionRoot ? rootFolder : wotVersion);
                        stream.Position = 0;
                        stream.CopyTo(wotModStream);
                    }
                }

            }

            return memoryStream;
        }


       
    }
}
