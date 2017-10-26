using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using WoTget.Core.Authoring;
using System.Collections.Generic;

namespace WoTget.Core.Commands
{
    public class RemoveCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Removes a Package from the online store";
            command.HelpOption("-?|-h|--help");

            var nameArgument = command.Argument("[name]",
                                                "Name of the Package");

          
            command.OnExecute(() =>
            {
                (new RemoveCommand(nameArgument.Value)).Run();
                return 0;
            });
        }

        private readonly string _name;
       

        public RemoveCommand(string name)
        {
            this._name = name;
          
        }

        public void Run()
        {
            if (string.IsNullOrEmpty(_name))
            {
                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Red, $"Name can't be empty!");
                return;
            }

            var names = new List<string>() { _name };
            if (_name == "*")
            {
                names = Application.Instance.VerifiyPackageList().Select(p => p.Key.Name).ToList();
            }
            foreach (var name in names)
            {
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Removing Package '{name}'...");
                Application.Instance.RemovePackage(name);
                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, "done");
            }
        
        }
    }
}