using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTget.Core.Installer
{
    public interface IPackageInstaller
    {
        IEnumerable<string> InstallPackageStream(Stream packageStream, string destinationPath);
        void UninstallPackageStream(Stream packageStream, string destinationPath);
    }
}
