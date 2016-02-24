using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core.Authoring;

namespace WoTget.Core.Repositories
{
    /// Represents a local filesystem repository. Packages in this repository are  
    /// stored in the format {id}/{version}/{package} 

    public class LocalRepository : IRepository
    {

        private object thisLock = new object();

        private readonly DirectoryInfo rootDirectory;

        public LocalRepository(string path)
        {
            if (!Directory.Exists(path)) throw new ArgumentException($"Directory {path} doesn't exists!", "path");
            rootDirectory = new DirectoryInfo(path);
        }

        #region "IRepository"
        public void RemovePackage(IPackage package)
        {
            lock (thisLock)
            {
                var path = Path.Combine(rootDirectory.FullName, GetPackagePath(package.Name, package.SemanticVersion()));
                if (!File.Exists(path)) throw new ArgumentException($"Package '{package.Name}' Version:'{package.Version}' not found");
                Directory.Delete(Path.Combine(rootDirectory.FullName, GetPackageRoot(package.Name, package.SemanticVersion())), true);
            }
        }

        public void AddPackage(IPackage package, IEnumerable<string> files)
        {
            lock (thisLock)
            {
                var filepath = Path.Combine(rootDirectory.FullName, GetPackagePath(package.Name, package.SemanticVersion()));
                var packageFolder = Path.Combine(rootDirectory.FullName, GetPackageRoot(package.Name, package.SemanticVersion()));

                if (Directory.Exists(packageFolder))
                {
                    Directory.Delete(packageFolder, true);
                }

                if (!Directory.Exists(packageFolder))
                {
                    Directory.CreateDirectory(packageFolder);
                }

                using (var stream = PackageBuilder.CreatePackage(package, files))
                {
                    using (var fileStream = File.Create(filepath))
                    {
                        stream.Seek(0, SeekOrigin.Begin);
                        stream.CopyTo(fileStream);
                    }
                }

                ManifestHelper.Save(package, Path.Combine(packageFolder, Constants.ManifestFileName));
            }
        }

        public Stream GetPackage(IPackage package)
        {
            lock (thisLock)
            {
                var path = Path.Combine(rootDirectory.FullName, GetPackagePath(package.Name, package.SemanticVersion()));
                if (!File.Exists(path)) throw new ArgumentException($"Package '{package.Name}' Version:'{package.Version}' not found");

                return File.Open(path, FileMode.Open, FileAccess.Read);
            }
        }
        #endregion


        #region "IPackageLookup"
        public IEnumerable<IPackage> GetPackages(bool onlyLatestVersion = true)
        {
            lock (thisLock)
            {
                var packageList = new List<IPackage>();
                foreach (var packageDir in rootDirectory.GetDirectories())
                {
                    foreach (var versionDir in packageDir.GetDirectories())
                    {
                        if (File.Exists(Path.Combine(versionDir.FullName, Constants.ManifestFileName)))
                            packageList.Add(ManifestHelper.Load(Path.Combine(versionDir.FullName, Constants.ManifestFileName)));
                    }
                }

                if (onlyLatestVersion)
                {
                    return packageList.Where(p => packageList.Where(p1 => p1.Name == p.Name).Select(p2 => new SemanticVersion(p2.Version)).Max().ToNormalizedString() == p.SemanticVersion().ToNormalizedString()).ToList();
                }

                return packageList;
            }
        }

        public IEnumerable<IPackage> GetPackages(string query, bool onlyLatestVersion = true)
        {

            var packages = GetPackages(onlyLatestVersion);

            return from p in packages
                   where p.Tags.Contains(query) || p.Description.Contains(query) || p.Name.Contains(query)
                   select p;

        }

        public IEnumerable<IPackage> GetPackages(IEnumerable<string> tags, bool onlyLatestVersion = true)
        {

            var packages = GetPackages(onlyLatestVersion);

            return from p in packages
                   where p.Tags.Any(t => tags.Contains(t))
                   select p;

        }
        #endregion 

        private static string GetPackageRoot(string packageId, SemanticVersion version)
        {
            return Path.Combine(packageId, version.ToNormalizedString());
        }

        private static string GetPackagePath(string packageId, SemanticVersion version)
        {
            return Path.Combine(
                GetPackageRoot(packageId, version),
                packageId + "." + version.ToNormalizedString() + Constants.PackageExtension);
        }

    }
}
