using MahApps.Metro;
using System.Windows;
using WoTget.Core;

namespace WoTget.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {

            //Check Version
            var newVersion = GitChecker.CheckNewVersion();
            if (newVersion != null)
            {
                var result=MessageBox.Show($"There's a new Version available {newVersion.ToString()}\n Download it now?", "New Version", MessageBoxButton.YesNo, MessageBoxImage.Exclamation);
                if (result == MessageBoxResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/adisadi/wotget/releases/latest");
                }
                Application.Current.Shutdown();
            }

            // get the theme from the current application
            var theme = ThemeManager.DetectAppStyle(Application.Current);

            // now set the Green accent and dark theme
            ThemeManager.ChangeAppStyle(Application.Current,
                                        ThemeManager.GetAccent(GUI.Properties.Settings.Default.Accent),
                                        ThemeManager.GetAppTheme(GUI.Properties.Settings.Default.Theme));

            base.OnStartup(e);
        }

    }
}
