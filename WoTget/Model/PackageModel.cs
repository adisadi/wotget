using System;
using WoTget.Core.Authoring;

namespace WoTget.Model
{
    [Flags]
    public enum VerifyPackageFlags
    {
        Unknown = 0x0,
        ExistsOnServer = 0x1,
        IsOutDated = 0x2,
        IsInstalled = 0x4
    }

    public class PackageModel:Package
    {
        public PackageModel(IPackage package)
        {
            Name = package.Name;
            Tags = package.Tags;
            Version = package.Version;
            Description = package.Description;
        }
        
        public VerifyPackageFlags PackageFlags { get; set; }
    }
}
