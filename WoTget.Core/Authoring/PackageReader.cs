using System.IO;
using System.IO.Compression;

namespace WoTget.Core.Authoring
{
    public class PackageReader
    {
        public static IPackage GetManifestFromPackageStream(Stream packageStream)
        {
            packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                var entry = archive.GetEntry(Constants.ManifestFileName);
                return ManifestHelper.LoadFromString(new StreamReader(entry.Open()).ReadToEnd());
            }
        }


    }
}
