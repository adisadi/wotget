using System.Collections.Generic;
using System.Linq;
using WoTget.Core.Authoring;

namespace WoTget.Core.Repositories
{
    public interface IPackageLookup
    {
        IEnumerable<IPackage> GetPackages(bool onlyLatestVersion = true);
        IEnumerable<IPackage> GetPackages(string query, bool onlyLatestVersion = true);
        IEnumerable<IPackage> GetPackages(IEnumerable<string> tags, bool onlyLatestVersion = true);
    }
}