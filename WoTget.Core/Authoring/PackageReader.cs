using System.IO;
using System.IO.Compression;
using System.Linq;

namespace WoTget.Core.Authoring
{
    public class PackageReader
    {
        public static IPackage GetMetaFromWotModPackageStream(Stream packageStream)
        {
            if (packageStream.CanSeek)
                packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                var entry = archive.GetEntry(MetaHelper.MetaFileName);
                if (entry == null) return null;
                return MetaHelper.LoadFromString(new StreamReader(entry.Open()).ReadToEnd());
            }
        }

        public static IPackage GetMetaFromPackageStream(Stream packageStream)
        {
            if (packageStream.CanSeek)
                packageStream.Seek(0, SeekOrigin.Begin);
            using (var archive = new ZipArchive(packageStream, ZipArchiveMode.Read, true))
            {
                var wotMod= archive.Entries.SingleOrDefault(e => e.Name.EndsWith(Constants.WotModExtension));
                if (wotMod == null) return null;

                using (var s = wotMod.Open())
                {
                    return GetMetaFromWotModPackageStream(s);
                }
            }
        }


    }
}
