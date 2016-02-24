using CommandLine;
using System.Collections.Generic;

namespace WoTget.CmdLineOptions
{
    public class AddSubOptions:BaseSubOptions
    {
        [Option('n', "name", Required = true, HelpText = "Name of the Package (must be unique)")]
        public string Name { get; set; }

        [Option('s', "summary", Required = true, HelpText = "Summary of the ModPackage")]
        public string Description { get; set; }

        [Option('a', "authors", Required = false, HelpText = "List of authors")]
        public string Authors { get; set; }

        [Option('o', "owners", Required = false, HelpText = "List of owners")]
        public string Owners { get; set; }

        [Option('p', "projecturl", Required = false, HelpText = "ProjectUrl")]
        public string ProjectUrl { get; set; }

        [Option('e', "version", Required = true, HelpText = "Package Version")]
        public string Version { get; set; }

        [OptionList('t',"tags", Separator = ':')]
        public IEnumerable<string> Tags { get; set; }



        [Option('d', "dir", Required = true, HelpText = "Location of the ModPackage Directory")]
        public string Directory { get; set; }
    }
}
