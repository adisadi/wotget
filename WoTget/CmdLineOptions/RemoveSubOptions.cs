using CommandLine;
using System.Collections.Generic;

namespace WoTget.CmdLineOptions
{
    public class RemoveSubOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the Package to remove")]
        public string Name { get; set; }
    }
}
