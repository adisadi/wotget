using System.Collections.Generic;
using System.Linq;
using System.IO;
using WoTget.Core.Authoring;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;

namespace WoTget.Core.Repository
{
    public class DriveRepository
    {
        private DriveService driveService;
        private Google.Apis.Drive.v3.Data.File googleModRoot;

        public const string PropertyModPackName = "WoTgetRepositoryV3";
        public const string PropertyReposityoryRoot = "WoTget";
        public const string PropertyVersion = "Version";


        public DriveRepository(string keyFile)
        {

            driveService = DriveHelper.Authenticate(keyFile);
            GetOrCreateModPackDirectory();
        }


        public void RemovePackage(IPackage package)
        {
            var file = GetFile(package);

            if (file == null)
            {
                return;
            }

            DriveHelper.DeleteFile(driveService, file.Id);
        }

        public void AddPackage(IPackage package, Stream packageStream)
        {

            if (GetFile(package) != null)
                RemovePackage(package);

            var properties = new Dictionary<string, string>() {
                 {  PropertyModPackName,  PropertyReposityoryRoot },
                 { PropertyVersion,  package.Version}
            };


            using (packageStream)
            {
                DriveHelper.UploadFile(driveService, packageStream, package.Id, package.Description, googleModRoot.Id, properties);
            }
        }

        public Stream GetPackage(IPackage package)
        {
            var file = GetFile(package);

            if (file == null)
            {
                return null;
            }

            return DriveHelper.DownloadFile(driveService, file.Id);
        }

        public IEnumerable<IPackage> GetPackages()
        {

            var propertiesSearch = "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "'} and mimeType != 'application/vnd.google-apps.folder'";
            return DriveHelper.GetFiles(driveService, propertiesSearch).Select(f => GetPackageFromFile(f));
        }


        private void GetOrCreateModPackDirectory()
        {

            var list = DriveHelper.GetFiles(driveService, "mimeType = 'application/vnd.google-apps.folder' and 'root' in parents and trashed=false and properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "'}");

            if (list.Count() == 0)
            {
                googleModRoot = DriveHelper.CreateDirectory(driveService, PropertyReposityoryRoot, "root", new Dictionary<string, string> { { PropertyModPackName, PropertyReposityoryRoot } });
            }
            else
            {
                googleModRoot = list[0];
            }
        }

        private Package GetPackageFromFile(Google.Apis.Drive.v3.Data.File f)
        {
            return new Package()
            {
                Name = Path.GetFileNameWithoutExtension(f.Name).Replace(new SemanticVersion(GetFilePropertySafe(f, PropertyVersion)).ToNormalizedString(), "").TrimEnd('.').Replace("_", " "),
                Description = f.Description,
                Version = GetFilePropertySafe(f, PropertyVersion),
            };
        }

        private string GetFilePropertySafe(Google.Apis.Drive.v3.Data.File file, string key)
        {
            var prop = file.Properties.FirstOrDefault(p => p.Key == key);
            return prop.Key == null ? string.Empty : prop.Value;
        }

        private Google.Apis.Drive.v3.Data.File GetFile(IPackage package)
        {
            var files = DriveHelper.GetFiles(driveService, "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' } and name='" + package.Id + "'");
            if (files.Count > 0)
                return files[0];
            return null;
        }
    }
}