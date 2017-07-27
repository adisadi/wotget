using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core;
using WoTget.Core.Authoring;
using WoTget.Core.Database;
using WoTget.Core.Database.Git;
using WoTget.Core.Repositories;

namespace WoTget.Client
{
    public class Application
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #region Singleton
        private static Application instance;
        public static Application Instance
        {
            get
            {
                return instance;
            }
        }

        public static void InitializeInstance(string serviceAccount, string P12File, string P12FileSecret)
        {
            log.Debug($"InitializeInstance serviceAccount:'{serviceAccount}' P12File:'{P12File}' P12FileSecret:'{P12FileSecret}'");
            if (instance == null)
            {
                instance = new Application(serviceAccount, P12File, P12FileSecret);
            }
        }
        #endregion

        public string ServiceAccount { get; internal set; }
        public string P12File { get; internal set; }
        public string P12FileSecret { get; internal set; }

        private IDatabase database;
        private IRepository repository;

        private Application(string serviceAccount, string p12File, string p12FileSecret)
        {
            ServiceAccount = serviceAccount;
            P12File = p12File;
            P12FileSecret = p12FileSecret;
            database = new GitDatabase();
            repository = new GoogleDriveRepository(serviceAccount, p12File, p12FileSecret);
        }

        public bool IsDatabaseInitialized()
        {
            return database.Exists;
        }

        public List<PackageModel> VerifiyPackageList(IList<string> packageNames)
        {
            var dictionary = new List<PackageModel>();

            if (packageNames == null || packageNames.Count == 0) return dictionary;


            var installedPackages = database.GetInstalledPackages();
            var packages = repository.GetPackages();

            if (packageNames.Count() == 1 && packageNames[0].Trim() == ".")
                packageNames = packages.Select(s => s.Name).ToList();

            foreach (var packageName in packageNames)
            {
                VerifyPackageFlags flag = VerifyPackageFlags.Unknown;

                var package = packages.FirstOrDefault(t => t.Name.ToLower() == packageName.ToLower());

                if (package != null) flag |= VerifyPackageFlags.ExistsOnServer;

                if (installedPackages.Any(t => t.Name.ToLower() == packageName.ToLower()))
                {
                    flag |= VerifyPackageFlags.IsInstalled;
                }

                if (flag.HasFlag(VerifyPackageFlags.ExistsOnServer) && flag.HasFlag(VerifyPackageFlags.IsInstalled))
                {
                    if (PackageCache.Instance.IsCacheOutdated(package)) flag |= VerifyPackageFlags.IsOutDated;
                }

                dictionary.Add(new PackageModel { Name = package != null ? package.Name : packageName, PackageFlags = flag, Description = package != null ? package.Description : "", Tags = package != null ? package.Tags : null });
            }
            return dictionary;
        }

        public void InstallPackages(IList<string> packageNames)
        {
            log.Debug($"InstallPackages: '{string.Join(" ", packageNames)}'");

            log.Info("Verifing Package(s):");

            if (packageNames == null || packageNames.Count == 0) throw new ArgumentException("No Packages spezified!");

            List<Package> packagesToInstall = new List<Package>();

            if (packageNames.Count == 1 && packageNames[0].Trim() == ".")
            {
                packagesToInstall = repository.GetPackages();
                foreach (var package in packagesToInstall)
                {
                    log.Info($"Package: '{package.PackageName}' OK.");
                }
            }
            else
            {
                var packages = repository.GetPackages();
                foreach (var packageName in packageNames)
                {
                    if (packages.Any(t => t.PackageName.ToLower() == packageName.ToLower()))
                    {
                        packagesToInstall.AddRange(packages.Where(t => t.PackageName.ToLower() == packageName.ToLower()));
                        log.Info($"Package: '{packageName}' OK.");
                    }
                    else
                    {
                        log.Error($"Package: '{packageName}' not found!");
                    }
                }
            }

            log.Info("");

            var installedPackages = database.GetInstalledPackages();

            List<string> outdatedPackages = new List<string>();

            foreach (var package in packagesToInstall)
            {
                if (PackageCache.Instance.IsCacheOutdated(package))
                {
                    if (installedPackages.Any(i => i.ToLower() == package.PackageName.ToLower()))
                    {
                        log.Info($"Remove installed Package: '{package.PackageName}'");
                        database.RemovePackage(package.PackageName);
                    }
                    log.Info($"Downloading Package: '{package.PackageName}'");
                    package.Download(Path.Combine(PackageCache.PackageCacheDirectory, package.FileName));

                    //Install
                    log.Info($"Installing Package: '{package.PackageName}'");
                    var unzippedFiles = new ZipPackage(Path.Combine(PackageCache.PackageCacheDirectory, package.FileName)).Extract();
                    database.AddPackage(package.PackageName, unzippedFiles);
                }
                else
                {
                    if (!installedPackages.Any(i => i.ToLower() == package.PackageName.ToLower()))
                    {
                        //Install
                        log.Info($"Installing Package: '{package.PackageName}'");
                        var unzippedFiles = new ZipPackage(Path.Combine(PackageCache.PackageCacheDirectory, package.FileName)).Extract();
                        database.AddPackage(package.PackageName, unzippedFiles);
                    }
                    else
                    {
                        log.Info($"Already installed Package: '{package.PackageName}'");
                    }
                }
            }
        }

        public static void Init(string wotGameDirectory, bool force)
        {
            new GitDatabase().Init(wotGameDirectory, WoTHelper.GetWoTVersion(wotGameDirectory), force);
        }

        public void UninstallPackages(IList<string> packageNames)
        {
            if (packageNames == null || packageNames.Count == 0) throw new ArgumentException("No Packages spezified!");

            var packages = database.GetInstalledPackages();
            if (!(packageNames.Count == 1 && packageNames[0].Trim() == "."))
            {
                packages = packages.Where(p => packageNames.Contains(p.Name)).ToList();
            }

            foreach (var package in packages)
            {
                try
                {
                    log.Info($"Uninstall Package: '{package.Name}'");
                    database.UninstallPackage(package);
                }
                catch (ArgumentException ex)
                {
                    log.Error(ex.Message);
                }
            }
        }

        public void UpdatePackages(IList<string> packageNames)
        {
            if (packageNames == null || packageNames.Count == 0) throw new ArgumentException("No Packages spezified!");

            log.Info("Verifing Package(s):");

            List<string> outdatedPackages = GetOutdatedPackages();
            var packages = repository.GetPackages();

            if (packageNames.Count == 1 && packageNames[0].Trim() == ".")
                packageNames = outdatedPackages;

            if (packageNames == null || packageNames.Count == 0)
            {
                log.Info($"Nothing to Update!");
                return;
            }


            foreach (var packageName in packageNames)
            {
                if (packages.Any(t => t.PackageName.ToLower() == packageName.ToLower()))
                {


                    var package = packages.Single(t => t.PackageName.ToLower() == packageName.ToLower());

                    if (outdatedPackages.Any(s => s.ToLower() == package.PackageName.ToLower()))
                    {
                        log.Info($"Updating Package: 'packageName'");
                        log.Info($"Remove installed Package: '{package.PackageName}'");
                        database.RemovePackage(package.PackageName);

                        log.Info($"Downloading Package: '{package.PackageName}'");
                        package.Download(Path.Combine(PackageCache.PackageCacheDirectory, package.FileName));
                        log.Info($"Installing Package: '{package.PackageName}'");
                        var unzippedFiles = new ZipPackage(Path.Combine(PackageCache.PackageCacheDirectory, package.FileName)).Extract();
                        database.AddPackage(package.PackageName, unzippedFiles);
                    }
                    else
                    {
                        log.Info($"Package: '{packageName}' up to date.");
                    }
                }
                else
                {
                    log.Error($"Package: {packageName} not found!");
                }
            }
        }



        public IEnumerable<IPackage> GetInstalledPackages()
        {
            return database.GetInstalledPackages();
        }

        public IEnumerable<IPackage> GetPackagesFromRepository()
        {
            return repository.GetPackages();
        }

        public IEnumerable<IPackage> GetPackagesFromRepository(IEnumerable<string> tags)
        {
            return repository.GetPackages(tags);
        }

        public IEnumerable<IPackage> GetPackagesFromRepository(string query)
        {
            return repository.GetPackages(query);
        }

        public static string GetWotVersion(string wotHome)
        {
            return WoTHelper.GetWoTVersion(wotHome);
        }

        public string GetWotVersion()
        {
            return WoTHelper.GetWoTVersion(database.WoTHome);
        }

        public string GetWotHome()
        {
            return database.WoTHome;
        }


        #region "Repository Functions"
        public void RemovePackages(string packageName, string version = null)
        {
            if (string.IsNullOrEmpty(packageName)) throw new ArgumentException("No Packages spezified!");

            List<IPackage> packageToDelete = new List<IPackage>();
            var packages = repository.GetPackages();

            try
            {
                if (string.IsNullOrEmpty(version))
                {
                    packageToDelete.AddRange(packages.Where(t => t.Name.ToLower() == packageName.ToLower()));
                }
                else
                {
                    packageToDelete.AddRange(packages.Where(t => t.Name.ToLower() == packageName.ToLower() && t.Version == version));
                }
            }
            catch (ArgumentNullException)
            {
                log.Info($"Package '{packageName}' with Version:'{version}' doesn't exist!");
            }

            foreach (var package in packageToDelete)
            {
                log.Info($"Delete Package '{package.Name}'.");
                repository.RemovePackage(package);
            }
        }

        public void AddPackages(string name, string directory, string description, IEnumerable<string> tags, string authors, string owners, string projectUrl, string version)
        {

            if (name.Contains(" "))
            {
                log.Error($"Package Name '{name}' contains spaces!");
                return;
            }


            if (!new DirectoryInfo(directory).GetDirectories().Any(d => d.Name == "res_mods"))
            {
                log.Error($"Directory '{directory}' doesn't contains res_mods Directory!");
                return;
            }

            var semanticVersion = new SemanticVersion("1.0.0.0");
            if (!SemanticVersion.TryParse(version,out semanticVersion))
            {
                log.Error($"Version '{version}' not valid!");
                return;
            }


            log.Info($"Upload Package '{name}' Version:'{version}'.");

            repository.AddPackage(new Package
            {
                Name = name,
                Authors = authors,
                Description = description,
                Owners = owners,
                Version = version,
                ProjectUrl = projectUrl,
                Tags = tags.ToList()
            },
                new DirectoryInfo(directory).GetFiles("*", SearchOption.AllDirectories).Select(f => f.FullName)
            );
        }

        #endregion

        private List<string> GetOutdatedPackages()
        {
            var packages = repository.GetPackages();
            var installedPackages = database.GetInstalledPackages();

            List<string> outdatedPackages = new List<string>();
            foreach (var packageName in installedPackages)
            {
                if (packages.Any(t => t.PackageName.ToLower() == packageName.ToLower()))
                {
                    var package = packages.Single(t => t.PackageName.ToLower() == packageName.ToLower());
                    if (PackageCache.Instance.IsCacheOutdated(package))
                    {
                        outdatedPackages.Add(package.PackageName);
                    }
                }
            }
            return outdatedPackages;
        }

    }
}
