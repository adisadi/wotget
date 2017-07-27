using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core;
using WoTget.Core.Authoring;

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


        private LocalModManager localModManager;
        private IRepository repository;

        private Client(IRepository repository)
        {

            localModManager = new LocalModManager();
            this.repository = repository;
        }

     

        public List<PackageModel> VerifiyPackageList(IEnumerable<IPackage> packageNames)
        {
            var dictionary = new List<PackageModel>();

            if (packageNames == null || packageNames.Count() == 0) return dictionary;


            var installedPackages = localModManager.GetInstalledPackages();
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

        internal bool HasWotHome()
        {
            if (string.IsNullOrEmpty(localModManager.WotHome)) return false;
            if (!Directory.Exists(localModManager.WotHome)) return false;
            return true;
        }

        public void InstallPackages(IEnumerable<PackageModel> packages)
        {

            var installedPackages = localModManager.GetInstalledPackages();

            foreach (var package in packages)
            {
                using (var stream = repository.GetPackage(package))
                {
                    if (installedPackages.ExistsByName(package))
                    {
                        localModManager.UninstallPackage(stream);
                    }
                    localModManager.InstallPackage(stream);
                }
            }
        }

        public void Init(string wotGameDirectory)
        {
            localModManager.Init(wotGameDirectory);
        }

        public void UninstallPackages(IEnumerable<PackageModel> packages)
        {

            var installedPackages = localModManager.GetInstalledPackages();

            foreach (var package in packages)
            {
                if (installedPackages.ExistsByName(package))
                {
                    using (var stream = repository.GetPackage(package))
                    {
                        localModManager.UninstallPackage(stream);
                    }
                }
                    
            }
        }

        public void UpdatePackages(IEnumerable<PackageModel> packages)
        {

            List<IPackage> outdatedPackages = GetOutdatedPackages();

            foreach (var package in packages)
            {      
                if (outdatedPackages.ExistsByName(package))
                {
                    var packageToDeinstall = outdatedPackages.FindByName(package.Name).SingleOrDefault();
                    using (var stream = repository.GetPackage(package))
                    {
                        localModManager.UninstallPackage(stream);
                    }
                      
                    using (var stream = repository.GetPackage(package))
                    {
                        localModManager.InstallPackage(stream);
                    }
                }
            }
        }

        public IEnumerable<IPackage> GetInstalledPackages()
        {
            return localModManager.GetInstalledPackages();
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

       
        public string GetWotVersion()
        {
            return WoTHelper.GetWoTVersion(localModManager.WotHome);
        }

        public string GetWotHome()
        {
            return localModManager.WotHome;
        }

        private List<IPackage> GetOutdatedPackages()
        {
            var packages = repository.GetPackages();
            var installedPackages = localModManager.GetInstalledPackages();

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
