using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTget.Core.Authoring;
using WoTget.Core.Installer;

namespace WoTget.Core.Database
{
    public abstract class BaseDatabase:IDatabase
    {
        protected IPackageInstaller installer;
        public BaseDatabase(IPackageInstaller installer)
        {
            this.installer = installer;
        }

        public abstract bool Exists { get; }
        public abstract string WoTHome { get; }
        public abstract string WoTVersion { get; }

        public abstract List<IPackage> GetInstalledPackages();
        public abstract void Init(string wotGameDirectory, string wotVersion, bool force);
        public abstract void InstallPackage(Stream packageStream);
        public abstract void UninstallPackage(IPackage package);
    }
}
