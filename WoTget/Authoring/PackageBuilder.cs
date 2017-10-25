
using System;
using System.IO;
using System.IO.Compression;
using static WoTget.Core.Format.FormatDetect;

namespace WoTget.Core.Authoring
{
    public class PackageBuilder
    {

        public static Stream Create(string archive, string wotVersion)
        {

            //Zip Temporary unpack
            string tempDirectory = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Directory.CreateDirectory(tempDirectory);
            using (var zip = new ZipArchive(File.OpenRead(archive), ZipArchiveMode.Read, false))
            {
                zip.ExtractToDirectory(tempDirectory);
            }

            var root = Path.Combine(tempDirectory, Path.GetFileNameWithoutExtension(archive));
            var files = Directory.GetFiles(root, "*", SearchOption.AllDirectories);
            var format = Analize(files);

            if (format == PackageFormat.NoFormat) throw new ArgumentException("Could not detect format!");

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

            var memoryStream = new MemoryStream();
            using (var zip = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var wotVersionFolder = PackageHelper.GetWotVersionFolder(file);

                    var entryName = rootAdditionalPath + PackageHelper.RemoveUntilFolder(
                        wotVersionFolder == string.Empty ? file : file.Replace(wotVersionFolder, Constants.VersionPlaceHolder), root);
                    zip.CreateEntryFromFile(file, entryName, CompressionLevel.Optimal);
                }
            }
            return memoryStream;
        }

    }
}
