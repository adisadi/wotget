using System.Collections.Generic;
using System.Linq;
using WoTget.Core;
using WoTget.Core.Authoring;
using WoTget.Core.Installer;
using WoTget.Core.Repositories;
using WoTget.Core.Repositories.GoogleDrive;

namespace WoTget.GUI.RepositoryManager
{
    public class MyApplication
    {
        #region Singleton
        private static MyApplication instance;
        public static MyApplication Instance
        {
            get
            {
                return instance;
            }
        }

        public static void InitializeInstance(string keyFile)
        {
     
            if (instance == null)
            {
                instance = new MyApplication(keyFile);
            }
        }


     

        private IRepository repository;

        private MyApplication(string keyFile)
        {           
            repository = new GoogleDriveRepository(keyFile);
        }

        internal void Save(IPackage newPackage,IPackage oldPackage)
        {
            try
            {
                using (var stream = repository.GetPackage(oldPackage))
                {
                    var files=new PackageInstaller().InstallPackageStream(stream, "temp");
                    repository.AddPackage(newPackage,files);
                }
            }
            catch { throw; }
        }

        internal void Delete(IPackage package)
        {
            repository.RemovePackage(package);
        }

        internal void New(IPackage package, List<string> files)
        {
            repository.AddPackage(package, files);
        }

        internal List<IPackage> GetPackages()
        {
            return repository.GetPackages(false).ToList();
        }
        #endregion
    }
}
