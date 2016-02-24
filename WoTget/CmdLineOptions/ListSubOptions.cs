using CommandLine;
using System.Collections.Generic;

namespace WoTget.CmdLineOptions
{
    public class ListSubOptions:BaseSubOptions
    {
        [OptionList('t', "tags", Separator = ':',HelpText = "Search for Tags delemited with ':' (ex: -t Tag1:Tag2). Only one Tag must match ")]
        public IEnumerable<string> Tags { get; set; }

        [Option('q', "query", HelpText = "Search contained in Description, Title or Tag")]
        public string Query { get; set; }

        [Option('a', "allversions", Required = false, HelpText = "Verbose Output", DefaultValue = false)]
        public bool AllVersion { get; set; }
    }
}
