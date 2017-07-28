using CommandLine;
using CommandLine.Text;

namespace WoTget.CmdLineOptions
{
    class Options
    {

        [VerbOption("init", HelpText = "set the WoT Game Directory")]
        public InitSubOptions InitVerb { get; set; }

        [VerbOption("list", HelpText = "Lists all available WoT Mod Packages with Status (Installed/Update available...)")]
        public ListSubOptions ListVerb { get; set; }

        [VerbOption("install", HelpText = "Installs WoT Mod Packages (Space delemited Package Names / For all '.')")]
        public InstallSubOptions InstallVerb { get; set; }

        [VerbOption("update", HelpText = "Updates installed WoT Mod Packages (Space delemited Package Names / For all '.')")]
        public UpdateSubOptions UpdateVerb { get; set; }

        [VerbOption("uninstall", HelpText = "Uninstalls WoT Mod Packages (Space delemited Package Names / For all '.')")]
        public UninstallSubOptions UninstallVerb { get; set; }

        [VerbOption("add", HelpText = "Add a WoT Mod Packages to Repository")]
        public AddSubOptions AddVerb { get; set; }

        [VerbOption("remove", HelpText = "Removes WoT Mod Packages from Repository (Space delemited Package Names / For all '.')")]
        public RemoveSubOptions RemoveVerb { get; set; }

        [VerbOption("removeAll", HelpText = "Removes all WoT Mod Packages from Repository")]
        public object RemoveAllVerb { get; set; }

        [HelpOption]
        public string GetUsage()
        {
            return HelpText.AutoBuild(this, (HelpText current) => HelpText.DefaultParsingErrorsHandler(this, current));
        }

        [HelpVerbOption]
        public string GetUsage(string verb)
        {
            return HelpText.AutoBuild(this, verb);
        }



    }
}
