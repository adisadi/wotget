using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WoTget.Core;
using WoTget.Core.Authoring;
using WoTget.Core.Installer;
using WoTget.Core.Repository;
using WoTget.LocalStore;

namespace WoTget
{
    public class Application
    {

        #region Singleton
        private static Application instance;
        public static Application Instance
        {
            get
            {
                return instance;
            }
        }

        public static void InitializeInstance(string keyFile,string wotGameDirectory)
        {
            if (instance == null)
            {
                instance = new Application(keyFile,wotGameDirectory);
            }
        }
        #endregion

        private Store store;
        private DriveRepository driveRepository;
        private string wotGameDirectory;
        private Application(string keyFile,string wotGameDirectory)
        {
            driveRepository = new DriveRepository(keyFile);
            store=new Store(".store\\settings.json");
            this.wotGameDirectory=wotGameDirectory;
        }

        public enum PackageVerifyFlag
        {
            notinstalled,
            installed,
            update

        }
        public Dictionary<IPackage, PackageVerifyFlag> VerifiyPackageList()
        {
            var dict = new Dictionary<IPackage, PackageVerifyFlag>();

            var packages = driveRepository.GetPackages().ToList();
            var installedPackages = store.Packages;

            foreach (var package in packages)
            {

                var installedPackage = installedPackages.SingleOrDefault(p => p.Id == package.Id);

                if (installedPackage == null)
                {
                    dict.Add(package, PackageVerifyFlag.notinstalled);
                    continue;
                }

                if (package.SemanticVersion > installedPackage.SemanticVersion)
                {
                    dict.Add(package, PackageVerifyFlag.update);
                    continue;
                }

                dict.Add(package, PackageVerifyFlag.installed);
            }

            return dict;
        }

        public void AddPackage(string name, string description, string version, string archive, bool force)
        {

            var p = new Package
            {
                Name = name,
                Description = description,
                Version = version
            };

            var package = driveRepository.GetPackages().SingleOrDefault(pa => pa.Id == p.Id);
            if (package != null)
            {
                if (package.SemanticVersion >= p.SemanticVersion && !force)
                {
                    throw new ArgumentException($"Package already exists with Version '{package.SemanticVersion.ToNormalizedString()}'!");
                }

                RemovePackage(name);
            }

            var allowedArchives = new List<string> { ".zip", ".rar" };
            var ext = Path.GetExtension(archive);

            if (!allowedArchives.Contains(ext))
                throw new ArgumentException($"Only '{string.Join(",", allowedArchives)}' Archives implemented!");


            driveRepository.AddPackage(p, PackageBuilder.Create(archive,WoTHelper.GetWoTVersion(wotGameDirectory)));
        }

        public void RemovePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) throw new ArgumentException("No Packages spezified!");

            var ptemp = new Package { Name = packageName };
            var packageToDelete = driveRepository.GetPackages().SingleOrDefault(p => p.Id == ptemp.Id);

            if (packageToDelete == null) throw new ArgumentException($"not found!");

            driveRepository.RemovePackage(packageToDelete);
        }

        public IPackage InstallPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) throw new ArgumentException("No Packages spezified!");

            var ptemp = new Package { Name = packageName };
            var packageToInstall = VerifiyPackageList().SingleOrDefault(p => p.Key.Id == ptemp.Id);
            if (packageToInstall.Key == null) throw new ArgumentException($"Package '{packageName}' not found!");

            if (packageToInstall.Value == PackageVerifyFlag.update)
            {
                //UnInstall Package
                UninstallPackage(packageToInstall.Key.Name);
            }

            using (var stream = driveRepository.GetPackage(packageToInstall.Key))
            {
                new PackageInstaller().InstallPackageStream(stream, wotGameDirectory);
                store.Add(packageToInstall.Key, stream);
            }

            return packageToInstall.Key;
        }

        public void UninstallPackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) throw new ArgumentException("No Packages spezified!");

            var ptemp = new Package { Name = packageName };
            var packageToUninstall = VerifiyPackageList().SingleOrDefault(p => p.Key.Id == ptemp.Id);
            if (packageToUninstall.Key == null) throw new ArgumentException($"not found!");
            if (packageToUninstall.Value == PackageVerifyFlag.notinstalled) throw new ArgumentException($"not installed!");

            if (store.PackageExists(packageToUninstall.Key))
            {
                using (var stream = store.GetPackage(packageToUninstall.Key))
                {
                    new PackageInstaller().UninstallPackageStream(stream, wotGameDirectory);
                    store.Remove(packageToUninstall.Key);
                }

                //Reinstall Others from Store
                foreach (var p in VerifiyPackageList().Where(e => e.Value == PackageVerifyFlag.installed || e.Value == PackageVerifyFlag.update))
                {
                    using (var stream = store.GetPackage(p.Key))
                    {
                        new PackageInstaller().InstallPackageStream(stream, wotGameDirectory);
                    }
                }
            }
        }

        public IPackage UpdatePackage(string packageName)
        {
            if (string.IsNullOrEmpty(packageName)) throw new ArgumentException("No Packages spezified!");

            var ptemp = new Package { Name = packageName };
            var packageToUpdate = VerifiyPackageList().SingleOrDefault(p => p.Key.Id == ptemp.Id);
            if (packageToUpdate.Key == null) throw new ArgumentException($"Package '{packageName}' not found!");
            if (packageToUpdate.Value == PackageVerifyFlag.notinstalled) throw new ArgumentException($"not installed!");
            if (packageToUpdate.Value == PackageVerifyFlag.installed) throw new ArgumentException($"nothing to update!");

            if (store.PackageExists(packageToUpdate.Key))
            {
                using (var stream = store.GetPackage(packageToUpdate.Key))
                {
                    new PackageInstaller().UninstallPackageStream(stream, wotGameDirectory);
                    store.Remove(packageToUpdate.Key);
                }

                //Reinstall Others from Store
                foreach(var p in VerifiyPackageList().Where(e=>e.Value==PackageVerifyFlag.installed || e.Value == PackageVerifyFlag.update))
                {
                    using (var stream = store.GetPackage(p.Key))
                    {
                        new PackageInstaller().InstallPackageStream(stream, wotGameDirectory);
                    }
                }
            }

            return InstallPackage(packageName);
        }
    }
}
