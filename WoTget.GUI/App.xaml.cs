using MahApps.Metro;
using System.Windows;

namespace WoTget.GUI
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
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
