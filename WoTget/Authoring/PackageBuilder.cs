
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

        public static Stream Create(string archive, string wotVersion, Func<List<string>, int> multiRoot)
        {

            //Zip Temporary unpack
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            using (var zip = new ZipArchive(File.OpenRead(archive), ZipArchiveMode.Read, false))
            {
                zip.ExtractToDirectory(tempDirectory);
            }

            var root = tempDirectory;
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            var format = Analyze(files);

            if (format == PackageFormat.NoFormat) throw new ArgumentException("Could not detect format!");

            var multiRootPaths = MultiRootAnalyze(files, format);

            if (multiRootPaths.Count > 1)
            {
                var selectedRoot = multiRoot(multiRootPaths.Select(f => PackageHelper.RemoveUntilFolder(f, root)).ToList());
                root = multiRootPaths[selectedRoot];
                files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            }

            string rootAdditionalPath = "";
            switch (format)
            {
                case PackageFormat.WotHomeRoot:
                    rootAdditionalPath = "";
                    break;
                case PackageFormat.ResModRoot:
                    rootAdditionalPath = Constants.ResModsFolder;
                    break;
                case PackageFormat.VersionRoot:
                    rootAdditionalPath = Path.Combine(Constants.ResModsFolder, Constants.VersionPlaceHolder);
                    break;
                case PackageFormat.WotMod:
                    rootAdditionalPath = Path.Combine(Constants.ModsFolder, Constants.VersionPlaceHolder);
                    break;
                case PackageFormat.WotModHomeRoot:
                    rootAdditionalPath = "";
                    break;
                case PackageFormat.WotModModsRoot:
                    rootAdditionalPath = Constants.ModsFolder;
                    break;
                default:
                    throw new ArgumentException("Format:" + format + " not implemented!");
            }

            //xx/xx/res_mods/config/xxx
            //xx/xx/res_mods/0.9.xxx/xxx
            //xx/xx/scripts/0.9.xxx/xxx
            //xx/xx/mods/config/xxx
            //xx/xx/mods/0.9.xxx/xxx
            //xx/xx/0.9.xxx/xxx.wotmod
            //xx/xx/0.9.xxx/xxx

            var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var transformedFile = file;
                    var wotVersionFolder = PackageHelper.GetWotVersionFolder(transformedFile);

                    //Replace Mode Version Folder with Constant
                    if (wotVersionFolder != string.Empty)
                    {
                        transformedFile = transformedFile.Replace(wotVersionFolder, Constants.VersionPlaceHolder);
                    }

                    //Remove trailing folders
                    switch (format)
                    {
                        case PackageFormat.WotHomeRoot:
                            transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, Constants.ResModsFolder);
                            break;
                        case PackageFormat.ResModRoot:
                            if (transformedFile.Contains(Constants.VersionPlaceHolder))
                                transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, Constants.VersionPlaceHolder);
                            else
                                transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, "configs");
                            break;
                        case PackageFormat.VersionRoot: //ToDo
                            transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, "scripts");
                            break;
                        case PackageFormat.WotMod:
                            transformedFile = Path.GetFileName(transformedFile); ;
                            break;
                        case PackageFormat.WotModHomeRoot:
                            transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, Constants.ModsFolder);
                            break;
                        case PackageFormat.WotModModsRoot:
                            if (transformedFile.Contains(Constants.VersionPlaceHolder))
                                transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, Constants.VersionPlaceHolder);
                            else
                                transformedFile = PackageHelper.RemoveBeforeFolder(transformedFile, "configs");
                            break;
                        default:
                            throw new ArgumentException("Format:" + format + " not implemented!");
                    }


                    var entryName = Path.Combine(rootAdditionalPath, transformedFile);

                    

                    zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            }
            return memoryStream;
        }

    }
}
