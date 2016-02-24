using System.Collections.Generic;
using System.IO;
using WoTget.Core.Authoring;

namespace WoTget.Core.Database
{
    public interface IDatabase
    {
        string WoTHome { get; }
        string WoTVersion { get; }

        void Init(string wotGameDirectory, string wotVersion, bool force);
        void InstallPackage(Stream packageStream);
        void UninstallPackage(IPackage package);
        List<IPackage> GetInstalledPackages();

        bool Exists { get; }
    }
}