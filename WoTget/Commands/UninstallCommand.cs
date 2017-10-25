using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;


namespace WoTget.Core.Commands
{
    public class UninstallCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Uninstalls a Package from WoT";
            command.HelpOption("-?|-h|--help");

            var namesArgument = command.Argument("[names]",
                                                "Names of the Package (* for all)", true);

            command.OnExecute(() =>
            {
                (new UninstallCommand(namesArgument.Values)).Run();
                return 0;
            });
        }

        private readonly List<string> _names;


        public UninstallCommand(List<string> names)
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

            if (_names.Count == 1 && _names[0] == "*")
            {
                _names.Clear();
                foreach (var p in Application.Instance.VerifiyPackageList())
                {
                    if (p.Value!=Application.PackageVerifyFlag.notinstalled)
                        _names.Add(p.Key.Name);
                }
            }

            foreach (var name in _names)
            {
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Uninstalling Package '{name}'...");
                try
                {
                    Application.Instance.UninstallPackage(name);
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, "done");
                }
                catch (ArgumentException ex)
                {
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, ex.Message);
                }
            }
        }
    }
}

