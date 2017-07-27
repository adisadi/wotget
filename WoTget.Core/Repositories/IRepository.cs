using System.Collections.Generic;
using System.IO;
using WoTget.Core.Authoring;

namespace WoTget.Core.Repositories
{
    public interface IRepository: IPackageLookup
    {
        void RemovePackage(IPackage package);

        void AddPackage(IPackage package, Stream stream);

        Stream GetPackage(IPackage package);
      
    }
}