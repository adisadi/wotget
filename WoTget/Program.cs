using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Layout;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using WoTget.Core;
using WoTget.CmdLineOptions;
using WoTget.Model;
using WoTget.Core.Authoring;
using WoTget.Core.Repositories.GoogleDrive;

namespace WoTget
{
    class Program
    {

        private static Application Application;

        static int Main(string[] args)
        {
            ILBundle.RegisterAssemblyResolver();
            return Main2(args);
        }

        static int Main2(string[] args)
        {


            try
            {

#if DEBUG
                System.Diagnostics.Debugger.Launch();

#endif

                string invokedVerb = "";
                object invokedVerbInstance = "";

                var options = new Options();
                if (!CommandLine.Parser.Default.ParseArguments(args, options,
                  (verb, subOptions) =>
                  {
                      // if parsing succeeds the verb name and correct instance
                      // will be passed to onVerbCommand delegate (string,object)
                      invokedVerb = verb.ToLower();
                      invokedVerbInstance = subOptions;
                  }))
                {
                    Environment.Exit(CommandLine.Parser.DefaultExitCodeFail);
                }
                var version = Assembly.GetExecutingAssembly().GetName().Version;

                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Yellow, $"WoTget Version: {version.ToString()}");

                AddConsoleAppender(invokedVerbInstance is BaseSubOptions && ((BaseSubOptions)invokedVerbInstance).Verbose ? Level.Debug : Level.Info);

                Application.InitializeInstance(JsonConfig.Config.Global.GoogleKeyFile);
                Application = Application.Instance;


                if (invokedVerb == "init")
                {
                    return InitCommand((InitSubOptions)invokedVerbInstance);
                }

                if (!Application.IsDatabaseInitialized() && invokedVerb != "init")
                {

                    Console.WriteLine();
                    ConsoleHelper.ColoredConsoleWrite(ConsoleColor.Yellow, $"What's your WoT Game Directory? (Default:C:\\Games\\World_of_Tanks):");
                    string wotDirectory = Console.ReadLine();
                    if (string.IsNullOrEmpty(wotDirectory))
                    {
                        wotDirectory = "C:\\Games\\World_of_Tanks";
                    }

                    var returnValue = InitCommand(new InitSubOptions { WotGameDirectory = wotDirectory });
                    if (returnValue == 1)
                        return 1;
                }



                ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.Green, $"WoT Version: '{Application.GetWotVersion()}' Game Directory: '{Application.GetWotHome()}'");
                Console.WriteLine("");

                if (invokedVerb == "list")
                {
                    return ListCommand((ListSubOptions)invokedVerbInstance);
                }

                if (invokedVerb == "install")
                {
                    return InstallCommand((InstallSubOptions)invokedVerbInstance);
                }

                if (invokedVerb == "update")
                {
                    return UpdateCommand((UpdateSubOptions)invokedVerbInstance);
                }

                if (invokedVerb == "uninstall")
                {
                    return UninstallCommand((UninstallSubOptions)invokedVerbInstance);
                }

                //Package Managment Commands
                if (invokedVerb == "add")
                {
                    return AddCommand((AddSubOptions)invokedVerbInstance);
                }

                if (invokedVerb == "remove")
                {
                    return RemoveCommand((RemoveSubOptions)invokedVerbInstance);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.ToString());
                return 1;
            }

            return 0;
        }


        private static void AddConsoleAppender(Level level)
        {

            ILog log = LogManager.GetLogger("WoTget");

            ColoredConsoleAppender appender = new ColoredConsoleAppender();
            appender.Name = "ConsoleAppender";
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { ForeColor = ColoredConsoleAppender.Colors.White | ColoredConsoleAppender.Colors.HighIntensity, Level = Level.Info });
            appender.AddMapping(new ColoredConsoleAppender.LevelColors() { ForeColor = ColoredConsoleAppender.Colors.Red | ColoredConsoleAppender.Colors.HighIntensity, Level = Level.Error });

            PatternLayout layout = new PatternLayout();
            layout.ConversionPattern = "%message%newline";
            layout.ActivateOptions();

            appender.Layout = layout;
            appender.ActivateOptions();

            ((Logger)log.Logger).Level = level;
            ((Logger)log.Logger).AddAppender(appender);
            ((Logger)log.Logger).Repository.Configured = true;





        }


        private static int InitCommand(InitSubOptions initSubOptions)
        {

            if (string.IsNullOrEmpty(initSubOptions.WotGameDirectory))
            {
                ConsoleHelper.WriteError("WoT Game Directory not set!\n Usgae Example:\nwotget init <wotgamedirectory>");
                return 1;
            }

            if (!Directory.Exists(initSubOptions.WotGameDirectory))
            {
                ConsoleHelper.WriteError($"WoT Game Directory '{initSubOptions.WotGameDirectory}' doesn't exists!\nMake shure you enter a valid WoT Game Directory (ex:C:\\Games\\World_of_Tanks)");
                return 1;
            }

            try
            {
                string wotVersion = Application.GetWotVersion(initSubOptions.WotGameDirectory);
            }
            catch
            {
                ConsoleHelper.WriteError($"Could not find Path.xml in WoT Game Directory '{initSubOptions.WotGameDirectory}'\nMake shure you enter a valid WoT Game Directory (ex:C:\\Games\\World_of_Tanks)");
                return 1;
            }

            Console.WriteLine("");
            Application.Init(initSubOptions.WotGameDirectory, initSubOptions.Force);

            return 0;
        }



        private static int UninstallCommand(UninstallSubOptions uninstallSubOptions)
        {
            if (uninstallSubOptions.Packages == null || uninstallSubOptions.Packages.Count == 0)
            {
                ConsoleHelper.WriteError("No Packages specified");
                return 1;
            }

            Application.UninstallPackages(uninstallSubOptions.Packages);

            return 0;
        }

        private static int RemoveCommand(RemoveSubOptions removeSubOptions)
        {
            Application.RemovePackages(removeSubOptions.Name);
            return 0;
        }

        private static int AddCommand(AddSubOptions uploadSubOptions)
        {
            Application.AddPackages(uploadSubOptions.Name,uploadSubOptions.Directory, uploadSubOptions.Description, uploadSubOptions.Tags, uploadSubOptions.Authors, uploadSubOptions.Owners, uploadSubOptions.ProjectUrl, uploadSubOptions.Version);
            return 0;
        }

        private static int ListCommand(ListSubOptions listSubOptions)
        {
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.White, "WoT Mod Packages:");
            ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.White, "(u=Installed and Update available/i=Installed/*=not Installed)");
            Console.WriteLine("");

            var result = Application.VerifiyPackageList(listSubOptions.Tags,listSubOptions.Query,listSubOptions.AllVersion);
            var maxLenght = Convert.ToInt32(result.Select(p => p.Name).Aggregate("", (max, cur) => max.Length > cur.Length ? max : cur).Length) * -1;
            foreach (var package in result)
            {
                var sign = package.PackageFlags.HasFlag(VerifyPackageFlags.IsOutDated) ? "u" : package.PackageFlags.HasFlag(VerifyPackageFlags.IsInstalled) ? "i" : "*";

                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.Yellow, $" [{sign}] ");
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.Green, string.Format("{0," + maxLenght + "}", package.Name));
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.DarkGreen, string.Format(" [{0}]", package.SemanticVersion().ToNormalizedString()));
                ConsoleHelper.ColoredConsoleWrite(ConsoleColor.White, $" --> {package.Description}");
                if (package.Tags != null && package.Tags.Count > 0)
                    ConsoleHelper.ColoredConsoleWriteLine(ConsoleColor.DarkRed, $" [Tags:'{string.Join(":", package.Tags)}']");
                else { Console.WriteLine(""); }


                
            }
            return 0;
        }

        private static int InstallCommand(InstallSubOptions installSubOptions)
        {
            if (installSubOptions.Packages == null || installSubOptions.Packages.Count == 0)
            {
                ConsoleHelper.WriteError("No Packages specified");
                return 1;
            }

            Application.InstallPackages(installSubOptions.Packages);

            return 0;
        }

        private static int UpdateCommand(UpdateSubOptions updateSubOptions)
        {
            if (updateSubOptions.Packages == null || updateSubOptions.Packages.Count == 0)
            {
                ConsoleHelper.WriteError("No Packages specified");
                return 1;
            }

            Application.UpdatePackages(updateSubOptions.Packages);

            return 0;
        }

    }
}
