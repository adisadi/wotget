using Google.Apis.Drive.v2;
using Google.Apis.Drive.v2.Data;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using WoTget.Core.Authoring;

namespace WoTget.Core.Repositories.GoogleDrive
{
    public class GoogleDriveRepository : IRepository
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);


        private DriveService driveService;
        private Google.Apis.Drive.v2.Data.File googleModRoot;

        public const string PropertyModPackName = "WoTgetRepositoryV2";
        public const string PropertyReposityoryRoot = "WoTget";
        public const string PropertyVersion = "Version";
        public const string PropertyAuthors = "Authors";
        public const string PropertyOwners = "Owners";
        public const string PropertyProjectUrl = "ProjectUrl";




        public GoogleDriveRepository(string keyFile)
        {

            driveService = Helper.Authenticate(keyFile);
            GetOrCreateModPackDirectory();
        }

        #region "IRepository"
        public void RemovePackage(IPackage package)
        {
            var file = GetFile(package);

            if (file == null)
            {
                log.Error($"Package  '{package.Name}' Version:'{package.Version}' not found in '{PropertyReposityoryRoot}'");
                return;
            }

            log.Debug($"Delete Google Drive Package '{package.Name}' Version:'{package.Version}' in '{PropertyReposityoryRoot}'");
            Helper.DeleteFile(driveService, file.Id);
        }

        public void AddPackage(IPackage package, IEnumerable<string> files)
        {
            log.Debug($"Upload Google Drive Package '{package.Name}' Version:'{package.Version}' in '{PropertyReposityoryRoot}'");

            if (GetFile(package) != null)
                RemovePackage(package);


            var properties = new List<Property>() {
                new Property() { Key = PropertyModPackName, Value = PropertyReposityoryRoot, Visibility = "PRIVATE" },
                new Property() { Key = PropertyVersion, Value = package.Version, Visibility = "PRIVATE" },
                new Property() { Key = PropertyAuthors, Value = package.Authors, Visibility = "PRIVATE" },
                new Property() { Key = PropertyOwners, Value = package.Owners, Visibility = "PRIVATE" },
                new Property() { Key = PropertyProjectUrl, Value = package.ProjectUrl, Visibility = "PRIVATE" }
            };

            if (package.Tags != null)
            {
                foreach (var t in package.Tags)
                {
                    properties.Add(new Property() { Key = t, Value = t, Visibility = "PRIVATE" });
                }
            }

            using (var stream = PackageBuilder.CreatePackage(package, files))
            {
                Helper.UploadFile(driveService, stream, package.FileName(), package.Description, googleModRoot.Id, properties.ToArray());
            }
        }

        public Stream GetPackage(IPackage package)
        {
            var file = GetFile(package);

            if (file == null)
            {
                log.Error($"Package  '{package.Name}' Version:'{package.Version}' not found in '{PropertyReposityoryRoot}'");
                return null;
            }

            log.Debug($"Download Google Drive Package '{package.Name}'  Version:'{package.Version}' in '{PropertyReposityoryRoot}'");
            return Helper.DownloadFile(driveService, file.DownloadUrl);
        }
        #endregion

        #region "IPackageLookup"
        public IEnumerable<IPackage> GetPackages(bool onlyLatestVersion = true)
        {
            log.Debug($"Get Google Drive Packages in '{PropertyReposityoryRoot}'");
            var propertiesSearch = "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' and visibility='PRIVATE' } and mimeType != 'application/vnd.google-apps.folder'";
            return FilterLatestVersion(Helper.GetFiles(driveService, propertiesSearch).Select(f => GetPackageFromFile(f)),onlyLatestVersion); 
        }

        public IEnumerable<IPackage> GetPackages(string query, bool onlyLatestVersion = true)
        {
            log.Debug($"Get Google Drive Packages in '{PropertyReposityoryRoot}' and Query:'{query}'");
            var propertiesSearch = "properties has { key = '" + query + "' and value = '" + query + "' and visibility = 'PRIVATE' } or fullText contains '\"" + query + "\"' and ";
            propertiesSearch = propertiesSearch + "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' and visibility='PRIVATE' } and mimeType != 'application/vnd.google-apps.folder'";

            return FilterLatestVersion(Helper.GetFiles(driveService, propertiesSearch).Select(f => GetPackageFromFile(f)),onlyLatestVersion);
        }

        public IEnumerable<IPackage> GetPackages(IEnumerable<string> tags, bool onlyLatestVersion = true)
        {
            log.Debug($"Get Google Drive Packages in '{PropertyReposityoryRoot}' and Tags:'{string.Join(",", tags)}'");

            var propertiesSearch = string.Empty;
            foreach (var t in tags)
            {
                propertiesSearch += "properties has { key = '" + t + "' and value = '" + t + "' and visibility = 'PRIVATE' } or ";
            }

            if (!string.IsNullOrEmpty(propertiesSearch))
            {
                int Place = propertiesSearch.LastIndexOf(("or "));
                propertiesSearch = propertiesSearch.Remove(Place, "or ".Length).Insert(Place, "and ");
            }

            propertiesSearch = propertiesSearch + "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' and visibility='PRIVATE' } and mimeType != 'application/vnd.google-apps.folder'";

            return FilterLatestVersion(Helper.GetFiles(driveService, propertiesSearch).Select(f => GetPackageFromFile(f)),onlyLatestVersion);
        }
        #endregion

        private IEnumerable<IPackage> FilterLatestVersion(IEnumerable<IPackage> packages,bool filter)
        {
            if (!filter) return packages;

            return packages.OnlyLatestVersion();
        }


        private void GetOrCreateModPackDirectory()
        {
            log.Debug($"Get Google Drive Directoy '{PropertyReposityoryRoot}'");
            var list = Helper.GetFiles(driveService, "mimeType = 'application/vnd.google-apps.folder' and 'root' in parents and trashed=false and properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' and visibility='PRIVATE' }");

            if (list.Count() == 0)
            {
                log.Debug($"Creating Google Drive Directoy '{PropertyReposityoryRoot}'");
                googleModRoot = Helper.CreateDirectory(driveService, PropertyReposityoryRoot, "root", new Property[] { new Property() { Key = PropertyModPackName, Value = PropertyReposityoryRoot, Visibility = "PRIVATE" } });
            }
            else
            {
                googleModRoot = list[0];
            }
        }

        private Package GetPackageFromFile(Google.Apis.Drive.v2.Data.File f)
        {
            return new Package()
            {
                Name = Path.GetFileNameWithoutExtension(f.Title).Replace(new SemanticVersion(GetFilePropertySafe(f, PropertyVersion)).ToNormalizedString(), "").TrimEnd('.'),
                Authors = GetFilePropertySafe(f, PropertyAuthors),
                Description = f.Description,
                Owners = GetFilePropertySafe(f, PropertyOwners),
                ProjectUrl = GetFilePropertySafe(f, PropertyProjectUrl),
                Version = GetFilePropertySafe(f, PropertyVersion),
                Tags = f.Properties.Where(p => p.Key != PropertyModPackName && p.Key == p.Value).Select(p => p.Key).ToList()
            };
        }

        private string GetFilePropertySafe(Google.Apis.Drive.v2.Data.File file, string key)
        {
            var prop = file.Properties.FirstOrDefault(p => p.Key == key);
            return prop == null ? string.Empty : prop.Value;
        }

        private Google.Apis.Drive.v2.Data.File GetFile(IPackage package)
        {
            var files = Helper.GetFiles(driveService, "properties has { key='" + PropertyModPackName + "' and value='" + PropertyReposityoryRoot + "' and visibility='PRIVATE' } and properties has { key='" + PropertyVersion + "' and value='" + package.Version + "' and visibility='PRIVATE' } and title='" + package.FileName() + "'");
            if (files.Count > 0)
                return files[0];
            return null;
        }
    }
}
