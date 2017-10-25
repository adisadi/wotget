using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using WoTget.Core.Authoring;

namespace WoTget.Core.Commands
{
    public class InstallCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Installs a Package into WoT";
            command.HelpOption("-?|-h|--help");

            var namesArgument = command.Argument("[names]",
                                                "Names of the Package (* for all)", true);

            command.OnExecute(() =>
            {
                (new InstallCommand(namesArgument.Values)).Run();
                return 0;
            });
        }

        private readonly List<string> _names;


        public InstallCommand(List<string> names)
        {
            this._names = names;
        }

        public void Run()
        {
            if (_names.Count == 0)
            {
                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, $"Names can't be empty!");
                return;
            }

            if (_names.Count==1 && _names[0] == "*")
            {
                _names.Clear();
                foreach (var p in Application.Instance.VerifiyPackageList())
                {
                    _names.Add(p.Key.Name);
                }
            }

            foreach (var name in _names)
            {
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Installing Package '{name}'...");
                try
                {
                    var package=Application.Instance.InstallPackage(name);
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, $"'{package.Version}' done");
                }
                catch (ArgumentException ex)
                {
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, ex.Message); 
                }
            }
        }
    }
}