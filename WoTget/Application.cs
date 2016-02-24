using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core;
using WoTget.Core.Authoring;
using WoTget.Core.Database;
using WoTget.Core.Installer;
using WoTget.Core.Repositories;
using WoTget.Core.Repositories.GoogleDrive;
using WoTget.Model;

namespace WoTget
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

        public static void InitializeInstance(string keyFile)
        {
            log.Debug($"InitializeInstance KeyFile:'{keyFile}'");
            if (instance == null)
            {
                instance = new Application(keyFile);
            }
        }
        #endregion


        private IDatabase database;
        private IRepository repository;

        private Application(string keyFile)
        {
         
            database = new LocalDatabase(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "database"),new PackageInstaller());
            repository = new GoogleDriveRepository(keyFile);
        }

        public bool IsDatabaseInitialized()
        {
            return database.Exists;
        }

        public List<PackageModel> VerifiyPackageList(IEnumerable<string> tags, string query, bool allVersion)
        {
            var packagemodelList = new List<PackageModel>();
            var packages = new List<IPackage>();
            if (tags != null)
            {
                packages = GetPackagesFromRepository(tags, !allVersion).ToList();
            }
            else if (query != null)
            {
                packages = GetPackagesFromRepository(query, !allVersion).ToList();
            }
            else
            {
                packages = GetPackagesFromRepository(!allVersion).ToList();
            }

            var installedPackages = database.GetInstalledPackages();
            var repositoryPackages = repository.GetPackages(false);


            foreach (var packageName in packages)
            {
                VerifyPackageFlags flag = VerifyPackageFlags.Unknown;

                var installed = installedPackages.FindByNameAndVersion(packageName);

                flag |= VerifyPackageFlags.ExistsOnServer;

                if (installed != null) flag |= VerifyPackageFlags.IsInstalled;

                if (allVersion)
                {
                    var repPackages = repositoryPackages.FindByName(packageName.Name);
                    if (repPackages.Any(p => p.SemanticVersion() > packageName.SemanticVersion()) && flag.HasFlag(VerifyPackageFlags.IsInstalled))
                    {
                        flag |= VerifyPackageFlags.IsOutDated;
                    }
                }
                else
                {
                    var i = installedPackages.FindByName(packageName.Name).SingleOrDefault();
                    if (i != null && i.SemanticVersion() < packageName.SemanticVersion())
                    {
                        flag |= VerifyPackageFlags.IsInstalled;
                        flag |= VerifyPackageFlags.IsOutDated;
                    }
                }

                packagemodelList.Add(new PackageModel(packageName) { PackageFlags = flag });
            }

            return packagemodelList;
        }

        public void InstallPackages(IList<string> packageNames)
        {
            log.Debug($"InstallPackages: '{string.Join(" ", packageNames)}'");

            log.Info("Verifing Package(s):");

            if (packageNames == null || packageNames.Count == 0) throw new ArgumentException("No Packages spezified!");

            List<IPackage> packagesToInstall = new List<IPackage>();

            if (packageNames.Count == 1 && packageNames[0].Trim() == ".")
            {
                packagesToInstall = repository.GetPackages().ToList();
                foreach (var package in packagesToInstall)
                {
                    log.Info($"Package: '{package.Name}' OK.");
                }
            }
            else
            {
                var packages = repository.GetPackages();
                foreach (var packageName in packageNames)
                {
                    if (packages.ExistsByName(packageName))
                    {
                        packagesToInstall.AddRange(packages.FindByName(packageName));
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

            foreach (var package in packagesToInstall)
            {
                if (installedPackages.ExistsByName(package))
                {
                    log.Info($"Remove installed Package: '{package.Name}'");
                    database.UninstallPackage(package);
                }

                log.Info($"Downloading Package: '{package.Name}'");
                using (var stream = repository.GetPackage(package))
                {
                    log.Info($"Installing Package: '{package.Name}'");
                    database.InstallPackage(stream);
                }

            }
        }

        public void Init(string wotGameDirectory, bool force)
        {
            database.Init(wotGameDirectory, WoTHelper.GetWoTVersion(wotGameDirectory), force);
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

            List<IPackage> outdatedPackages = GetOutdatedPackages();

            if (packageNames.Count == 1 && packageNames[0].Trim() == ".")
                packageNames = outdatedPackages.Select(p => p.Name).ToList();

            if (packageNames == null || packageNames.Count == 0)
            {
                log.Info($"Nothing to Update!");
                return;
            }

            var repositoryPackages = repository.GetPackages();

            foreach (var packageName in packageNames)
            {
                if (outdatedPackages.ExistsByName(packageName))
                {

                    var package = outdatedPackages.FindByName(packageName).SingleOrDefault();

                    log.Info($"Updating Package: 'packageName'");
                    log.Info($"Remove installed Package: '{package.Name}' Version: '{package.Version}'");
                    database.UninstallPackage(package);

                    var packageToInstall = repositoryPackages.FindByName(packageName).SingleOrDefault();

                    log.Info($"Downloading Package: '{packageToInstall.Name}' Version: '{packageToInstall.Version}'");
                    using (var stream = repository.GetPackage(packageToInstall))
                    {
                        log.Info($"Installing Package: '{packageToInstall.Name}' Version: '{packageToInstall.Version}'");
                        database.InstallPackage(stream);
                    }
                }
                else
                {
                    log.Info($"Package: '{packageName}' up to date or doesn't exist");
                }
            }
        }



        public IEnumerable<IPackage> GetInstalledPackages()
        {
            return database.GetInstalledPackages();
        }

        public IEnumerable<IPackage> GetPackagesFromRepository(bool onlyLatestVersion = true)
        {
            return repository.GetPackages(onlyLatestVersion);
        }

        public IEnumerable<IPackage> GetPackagesFromRepository(IEnumerable<string> tags, bool onlyLatestVersion = true)
        {
            return repository.GetPackages(tags, onlyLatestVersion);
        }

        public IEnumerable<IPackage> GetPackagesFromRepository(string query, bool onlyLatestVersion = true)
        {
            return repository.GetPackages(query, onlyLatestVersion);
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
            if (!SemanticVersion.TryParse(version, out semanticVersion))
            {
                log.Error($"Version '{version}' not valid!");
                return;
            }


            log.Info($"Upload Package '{name}' Version:'{version}'.");

            if (tags == null) tags = new List<string>();

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

        private List<IPackage> GetOutdatedPackages()
        {
            var packages = repository.GetPackages();
            var installedPackages = database.GetInstalledPackages();

            List<IPackage> outdatedPackages = new List<IPackage>();
            foreach (var installed in installedPackages)
            {
                if (packages.ExistsByName(installed))
                {
                    var package = packages.FindByName(installed.Name).SingleOrDefault();
                    if (package.SemanticVersion() > installed.SemanticVersion())
                    {
                        outdatedPackages.Add(installed);
                    }
                }
            }
            return outdatedPackages;
        }

    }
}
