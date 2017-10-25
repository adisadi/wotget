using Microsoft.Extensions.CommandLineUtils;
namespace WoTget.Core.Commands
{
    public class RootCommand : ICommand
    {

        public static void Configure(CommandLineApplication app)
        {
            app.Name = "WoTget";
            app.Description = "WoT Mod Manager";
            app.HelpOption("-?|-h|--help");

            // Register commands
            app.Command("list", ListCommand.Configure);
            app.Command("add", AddCommand.Configure);
            app.Command("remove", RemoveCommand.Configure);
            app.Command("install", InstallCommand.Configure);
            app.Command("uninstall", UninstallCommand.Configure);
            app.Command("update", UpdateCommand.Configure);

            app.OnExecute(() =>
            {
                (new RootCommand(app)).Run();
                return 0;
            });
        }

        private readonly CommandLineApplication _app;

        public RootCommand(CommandLineApplication app)
        {
            _app = app;
        }

        public void Run()
        {
            _app.ShowHelp();
        }

    }
}