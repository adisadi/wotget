using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
using WoTget.Core.Authoring;

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

            ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $"Removing Package '{_name}'...");
            Application.Instance.RemovePackage(_name);
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, "done");
        
        }
    }
}