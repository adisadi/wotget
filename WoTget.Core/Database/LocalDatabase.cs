using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core.Authoring;
using WoTget.Core.Installer;
using WoTget.Core.Repositories;

namespace WoTget.Core.Database
{
    public class LocalDatabase : BaseDatabase
    {

        private static object thisLock = new object();

        private const string localDatabaseConfigName = "packages.config";

        private string localDatabasePath;

        private IRepository packageCache;

        private IPackageInstaller packageInstaller;

        public LocalDatabase(string path,IPackageInstaller packageInstaller):base(packageInstaller)
        {
            this.packageInstaller = packageInstaller;
            localDatabasePath = path;
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);

            packageCache = new LocalRepository(localDatabasePath);
        }

        private LocalDatabaseConfig GetLocalDatabaseConfig()
        {
            if (Exists)
                return LocalDatabaseConfig.Load(localDatabaseConfigFile);
            return null;
        }


        #region "IDatabase"
        public override bool Exists
        {
            get
            {
                return File.Exists(localDatabaseConfigFile);
            }
        }

        public override string WoTHome
        {
            get
            {
                return Exists ? GetLocalDatabaseConfig().WoTHome : string.Empty;
            }
        }

        public override string WoTVersion
        {
            get
            {
                return Exists ? GetLocalDatabaseConfig().WoTVersion : string.Empty;
            }
        }

        private string localDatabaseConfigFile
        {
            get
            {
                return Path.Combine(localDatabasePath, localDatabaseConfigName);
            }
        }


        public override void Init(string wotGameDirectory, string wotVersion, bool force)
        {
            lock (thisLock)
            {
                if (!Exists || force)
                {
                    DeleteDatabase();
                    InitDatabase(wotGameDirectory, wotVersion);
                }
                else
                {
                    var localDatabaseConfig = GetLocalDatabaseConfig();
                    if (localDatabaseConfig.WoTVersion != wotVersion)
                    {
                        DeleteDatabase();
                        InitDatabase(wotGameDirectory, wotVersion);
                    }
                }
            }
        }

        private void DeleteDatabase()
        {
            if (Directory.Exists(localDatabasePath))
            {
                Directory.Delete(localDatabasePath, true);
            }
            Directory.CreateDirectory(localDatabasePath);
        }

        private void InitDatabase(string wotGameDirectory, string wotVersion)
        {
            var localDatabaseConfig = new LocalDatabaseConfig
            {
                WoTHome = wotGameDirectory,
                WoTVersion = wotVersion,
                Packages = new List<LocalDatabasePackageInfo>()
            };

            localDatabaseConfig.Save(localDatabaseConfigFile);
            packageCache = new LocalRepository(localDatabasePath);
        }


        public override List<IPackage> GetInstalledPackages()
        {
            lock (thisLock)
            {
                var localDatabaseConfig = this.GetLocalDatabaseConfig();
                var packages = packageCache.GetPackages().ToList();

                return localDatabaseConfig.Packages.Select(p => packages.FindByNameAndVersion(p.Name, p.Version)).ToList();
            }
        }

        public override void InstallPackage(Stream packageStream)
        {
            lock (thisLock)
            {

                if (!Exists) throw new DatabaseNotFoundException($"Local Database not found at '{localDatabasePath}'");

                var package = PackageReader.GetManifestFromPackageStream(packageStream);

                var localDatabaseConfig = GetLocalDatabaseConfig();

                var installedPackage = localDatabaseConfig.Packages.SingleOrDefault(p => p.Name == package.Name);
                if (installedPackage != null)
                {
                    if (installedPackage.Version != package.Version)
                    {
                        var p = packageCache.GetPackages().FindByNameAndVersion(new Package { Name = installedPackage.Name, Version = installedPackage.Version });
                        if (p == null) throw new Exception($"Old Package '{installedPackage.Name}' Version:'{installedPackage.Version}' not found!");

                        UninstallPackage(p);

                    }
                }

                localDatabaseConfig.Packages.Add(new LocalDatabasePackageInfo { Name = package.Name, Version = package.Version });
                localDatabaseConfig.Save(localDatabaseConfigFile);

                var files = packageInstaller.InstallPackageStream(packageStream, WoTHome);
                packageCache.AddPackage(package, files);

            }

        }

        public override void UninstallPackage(IPackage package)
        {
            lock (thisLock)
            {

                if (!Exists) throw new DatabaseNotFoundException($"Local Database not found at '{localDatabasePath}'");

                var localDatabaseConfig = GetLocalDatabaseConfig();
                var installedPackage = localDatabaseConfig.Packages.SingleOrDefault(p => p.Name == package.Name && p.Version == package.Version);
                if (installedPackage != null)
                {
                    try
                    {
                        using (var stream = packageCache.GetPackage(package))
                        {
                            packageInstaller.UninstallPackageStream(stream, WoTHome);
                        }

                        packageCache.RemovePackage(package);
                    }
                    catch (ArgumentException) { }
                    finally
                    {
                        localDatabaseConfig.Packages.Remove(installedPackage);
                        localDatabaseConfig.Save(localDatabaseConfigFile);
                    }

                    foreach (var p in localDatabaseConfig.Packages)
                    {
                        using (var stream = packageCache.GetPackage(new Package { Name = p.Name, Version = p.Version }))
                        {
                            packageInstaller.InstallPackageStream(stream, WoTHome);
                        }
                    }
                }
            }
        }



        #endregion
    }
}
