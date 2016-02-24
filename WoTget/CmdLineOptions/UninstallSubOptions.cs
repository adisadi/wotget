using CommandLine;
using System.Collections.Generic;

namespace WoTget.CmdLineOptions
{
    public class UninstallSubOptions:BaseSubOptions
    {
        [ValueList(typeof(List<string>), MaximumElements = -1)]
        public IList<string> Packages { get; set; }
    }
}
