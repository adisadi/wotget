using CommandLine;

namespace WoTget.CmdLineOptions
{
    public class BaseSubOptions
    {
       
        [Option('v', "verbose", Required = false, HelpText = "Verbose Output",DefaultValue = false)]
        public bool Verbose { get; set; }
    }
}
