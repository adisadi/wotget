using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTget.CmdLineOptions
{
    class InitSubOptions:BaseSubOptions
    {
        [ValueOption(0)]
        public string WotGameDirectory { get; set; }

        [Option('f', "force", Required = false, HelpText = "ReInits an existing Environment", DefaultValue = false)]
        public bool Force { get; set; }

    }
}
