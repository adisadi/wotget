using System;
using System.IO;
using System.Security.Cryptography;
using WoTget.Core;
using WoTget.Core.Repositories;

namespace WoTget.Client
{
    internal class PackageCache
    {
        public const string PackageCacheDirectory = "package-cache";


        #region Singleton
        private static PackageCache instance;
        public static PackageCache Instance
        {
            get
            {
                if (instance == null) instance = new PackageCache();
                return instance;
            }
        }
        #endregion

        private IRepository repository;

        public PackageCache()
        {
            if (!Directory.Exists(PackageCacheDirectory))
            {
                Directory.CreateDirectory(PackageCacheDirectory);
            }
            repository = new LocalRepository(PackageCacheDirectory);
        }
      
    }
}
