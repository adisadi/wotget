using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTget.CmdLineOptions
{
    public class UpdateSubOptions:BaseSubOptions
    {
        [ValueList(typeof(List<string>), MaximumElements = -1)]
        public IList<string> Packages { get; set; }
    }
}
