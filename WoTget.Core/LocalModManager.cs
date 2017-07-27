using System.Collections.Generic;
using System.IO;
using WoTget.Core.Authoring;
using WoTget.Core.Installer;

namespace WoTget.Core
{
    public class LocalModManager
    {
        private string wotGameDirectory;
        public LocalModManager()
        {

        }

        public void InstallPackage(Stream packageStream)
        {
            new PackageInstaller().InstallPackageStream(packageStream, wotGameDirectory);
        }
        public void UninstallPackage(Stream packageStream)
        {
            new PackageInstaller().UninstallPackageStream(packageStream, wotGameDirectory);
        }
        /// <summary>
        /// Gets all wotmod Packages in WotHome with meta.xml 
        /// </summary>
        /// <returns></returns>
        public List<IPackage> GetInstalledPackages()
        {
            var packageFolder = Path.Combine(Path.Combine(wotGameDirectory, Constants.ModsFolder), WoTHelper.GetWoTVersion(wotGameDirectory));
            if (!Directory.Exists(packageFolder)) return new List<IPackage>();
            var mods = Directory.GetFiles(packageFolder, "*" + Constants.WotModExtension, SearchOption.TopDirectoryOnly);

            List<IPackage> packages = new List<IPackage>();
            foreach (var mod in mods)
            {
                using (var stream = File.OpenRead(mod))
                {
                    var package = PackageReader.GetMetaFromWotModPackageStream(stream);
                    if (package != null)
                    {
                        packages.Add(package);
                    }
                }
            }

            return packages;
        }

        public void Init(string wotGameDirectory)
        {
            this.wotGameDirectory = wotGameDirectory;
        }

        public string WotHome
        {
            get
            {
                return wotGameDirectory;
            }
        }
    }
}
