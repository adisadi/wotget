using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using WoTget.Core.Authoring;

namespace WoTget.Core.Commands
{
    public class AddCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Adds a Package";
            command.HelpOption("-?|-h|--help");

            var nameArgument = command.Argument("[name]",
                                                "Name of the Package");

            var descArgument = command.Argument("[description]",
                                                "Description of the Package");

            var versionArgument = command.Argument("[version]",
                                                   "Version of the Package");

            var archiveArgument = command.Argument("[archive]",
                                                "Archive to add");

            var forceOption = command.Option("-f|--force", "force replace if exists", CommandOptionType.NoValue);

            command.OnExecute(() =>
            {
                (new AddCommand(nameArgument.Value, descArgument.Value, versionArgument.Value, archiveArgument.Value, forceOption.HasValue())).Run();
                return 0;
            });
        }

        private readonly string _name;
        private readonly string _description;
        private readonly string _version;
        private readonly string _archive;
        private readonly bool _force;

        public AddCommand(string name, string description, string version, string archive, bool force)
        {
            this._name = name;
            this._version = version;
            this._archive = archive;
            this._description = description;
            this._force = force;
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(_name))
            {
                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, $"Name can't be empty!");
                return;
            }

            var semanticVersion = new SemanticVersion("1.0.0.0");
            if (!string.IsNullOrEmpty(_version))
            {
                if (!SemanticVersion.TryParse(_version, out semanticVersion))
                {
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, $"Version '{_version}' not valid!");
                    return;
                }
            }

            if (!System.IO.File.Exists(_archive))
            {
                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, $"Archive '{_archive}' not found!");
                return;
            }

            ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Adding Package '{_name}'...");
            Application.Instance.AddPackage(_name, _description, _version, _archive, _force);
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, "done");
        }
    }
}