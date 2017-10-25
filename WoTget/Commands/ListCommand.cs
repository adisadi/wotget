using System;
using System.Linq;
using Microsoft.Extensions.CommandLineUtils;
namespace WoTget.Core.Commands
{
    public class ListCommand : ICommand
    {
        public static void Configure(CommandLineApplication command)
        {

            command.Description = "Lists all available WoT Mod Packages with Status (Installed/Update available...)";
            command.HelpOption("-?|-h|--help");

            command.OnExecute(() =>
            {
                (new ListCommand()).Run();
                return 0;
            });
        }



        public ListCommand()
        {
        }

        public void Run()
        {
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.White, "WoT Mod Packages:");
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.White, "(u=Installed and Update available/i=Installed/*=not Installed)");
            Console.WriteLine("");

            var result = Application.Instance.VerifiyPackageList();
            var maxLenght = Convert.ToInt32(result.Keys.Select(p => p.Name).Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length) * -1;
            foreach (var item in result)
            {
                var sign = item.Value== Application.PackageVerifyFlag.update ? "u" : item.Value==Application.PackageVerifyFlag.installed ? "i" : "*";

                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.Yellow, $" [{sign}] ");
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.Green, string.Format("{0," + maxLenght + "}", item.Key.Name));
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.DarkGreen, string.Format(" [{0}]", item.Key.SemanticVersion.ToNormalizedString()));
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $" --> {item.Key.Description}");
                
                Console.WriteLine("");


                
            }
        }
    }
}