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
using WoTget.GUI.Model;

namespace WoTget.GUI
{
    public class Client
    {


        #region Singleton
        private static Client instance;
        public static Client Instance
        {
            get
            {
                if (instance == null)
                {
                    InitializeInstance();
                }
                return instance;
            }
        }

        public static void InitializeInstance()
        {
            if (instance == null)
            {
                instance = new Client(new GoogleDriveRepository(JsonConfig.Config.Global.GoogleKeyFile));
            }
        }
        #endregion


        private IDatabase database;
        private IRepository repository;

        private Client(IRepository repository)
        {
        
            database = new LocalDatabase(Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "database"), new PackageInstaller());
            this.repository = repository;
        }

        public bool IsDatabaseInitialized()
        {
            return database.Exists;
        }

        public List<PackageModel> VerifiyPackageList(IEnumerable<IPackage> packageNames)
        {
            var dictionary = new List<PackageModel>();

            if (packageNames == null || packageNames.Count() == 0) return dictionary;


            var installedPackages = database.GetInstalledPackages();
            var packages = repository.GetPackages();

            foreach (var packageName in packageNames)
            {
                State flag = State.None;

                var package = packages.FindByNameAndVersion(packageName);
                var installed = installedPackages.FindByName(packageName.Name).SingleOrDefault();

                if (installed != null) flag = State.Installed;

                if (flag == State.Installed)
                {
                    if (package.SemanticVersion() > installed.SemanticVersion()) flag = State.NeedsUpdate;
                }

                dictionary.Add(new PackageModel(packageName) { PackageState = flag });
            }
            return dictionary;
        }

        public void InstallPackages(IEnumerable<PackageModel> packages)
        {

            var installedPackages = database.GetInstalledPackages();

            foreach (var package in packages)
            {
                if (installedPackages.ExistsByName(package))
                {
                    database.UninstallPackage(package);
                }

                using (var stream = repository.GetPackage(package))
                {
                    database.InstallPackage(stream);
                }
            }
        }

        public void Init(string wotGameDirectory, bool force)
        {
            database.Init(wotGameDirectory, WoTHelper.GetWoTVersion(wotGameDirectory), force);
        }

        public void UninstallPackages(IEnumerable<PackageModel> packages)
        {

            var installedPackages = database.GetInstalledPackages();

            foreach (var package in packages)
            {
                if (installedPackages.ExistsByName(package))
                    database.UninstallPackage(package);
            }
        }

        public void UpdatePackages(IEnumerable<PackageModel> packages)
        {

            List<IPackage> outdatedPackages = GetOutdatedPackages();

            foreach (var package in packages)
            {      
                if (outdatedPackages.ExistsByName(package))
                {
                    database.UninstallPackage(package);
                    using (var stream = repository.GetPackage(package))
                    {
                        database.InstallPackage(stream);
                    }
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

        private List<IPackage> GetOutdatedPackages()
        {
            var packages = repository.GetPackages();
            var installedPackages = database.GetInstalledPackages();

            List<IPackage> outdatedPackages = new List<IPackage>();
            foreach (var installed in installedPackages)
            {
                if (packages.ExistsByName(installed))
                {
                    var package = packages.FindByNameAndVersion(installed);
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
