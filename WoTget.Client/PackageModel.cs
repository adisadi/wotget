using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WoTget.Core.Authoring;

namespace WoTget.Client
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
        public VerifyPackageFlags PackageFlags { get; set; }
    }
}
