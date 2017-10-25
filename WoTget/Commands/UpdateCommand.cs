using Microsoft.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;


namespace WoTget.Core.Commands
{
    public class UpdateCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Updates a Package in WoT";
            command.HelpOption("-?|-h|--help");

            var namesArgument = command.Argument("[names]",
                                                "Names of the Package (* for all)", true);

            command.OnExecute(() =>
            {
                (new UpdateCommand(namesArgument.Values)).Run();
                return 0;
            });
        }

        private readonly List<string> _names;


        public UpdateCommand(List<string> names)
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
                    if (p.Value==Application.PackageVerifyFlag.update)
                        _names.Add(p.Key.Name);
                }
            }

            foreach (var name in _names)
            {
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Updating Package '{name}'...");
                try
                {
                    var package=Application.Instance.UpdatePackage(name);
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

