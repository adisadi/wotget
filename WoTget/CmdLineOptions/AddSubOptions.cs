using CommandLine;
using System.Collections.Generic;

namespace WoTget.CmdLineOptions
{
    public class AddSubOptions:BaseSubOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the Package (must be unique)")]
        public string Name { get; set; }

        [Option('s', "summary", Required = false, HelpText = "Summary of the ModPackage")]
        public string Description { get; set; }

        [Option('e', "version", Required = false, HelpText = "Package Version")]
        public string Version { get; set; }

        [OptionList('t',"tags", Separator = ':')]
        public IEnumerable<string> Tags { get; set; }

        [Option('d', "dir", Required = true, HelpText = "Location of the ModPackage Directory or Zip File")]
        public string Directory { get; set; }
    }
}
