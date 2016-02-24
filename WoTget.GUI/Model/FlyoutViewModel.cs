using MahApps.Metro;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace WoTget.GUI.Model
{
    public class FlyoutViewModel : INotifyPropertyChanged
    {
        public FlyoutViewModel()
        {
            AccentColors = ThemeManager.Accents
                                           .Select(a => new AccentColorMenuData() { Model = this, Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                                           .ToList();

            AppThemes = ThemeManager.AppThemes
                                           .Select(a => new AppThemeMenuData() { Model = this, Name = a.Name, BorderColorBrush = a.Resources["BlackColorBrush"] as Brush, ColorBrush = a.Resources["WhiteColorBrush"] as Brush })
                                           .ToList();


        }

        private string wotHome;
        private string wotVersion;
        public string WoTHome
        {
            get { return wotHome; }
            set
            {
                if (value != wotHome)
                {
                    wotHome = value;
                    OnPropertyChanged();
                }
            }
        }
        public string WoTVersion
        {
            get { return wotVersion; }
            set
            {
                if (value != wotVersion)
                {
                    wotVersion = value;
                    OnPropertyChanged();
                }
            }
        }


        public List<AccentColorMenuData> AccentColors { get; set; }
        public List<AppThemeMenuData> AppThemes { get; set; }

        public AccentColorMenuData CurrentAccent
        {
            get
            {
                return AccentColors.Single(a => a.Name == Properties.Settings.Default.Accent);
            }
            set
            {
                if (value.Name != Properties.Settings.Default.Accent)
                {
                    Properties.Settings.Default.Accent = value.Name;
                    Properties.Settings.Default.Save();
                    OnPropertyChanged();
                }
            }
        }
        public AppThemeMenuData CurrentTheme
        {
            get
            {
                return AppThemes.Single(a => a.Name == Properties.Settings.Default.Theme);
            }
            set
            {
                Properties.Settings.Default.Theme = value.Name;
                Properties.Settings.Default.Save();
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] String caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }
    }

    public class AccentColorMenuData
    {
        public FlyoutViewModel Model { get; set; }
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        private ICommand changeAccentCommand;

        public ICommand ChangeAccentCommand
        {
            get { return this.changeAccentCommand ?? (changeAccentCommand = new SimpleCommand { CanExecuteDelegate = x => true, ExecuteDelegate = x => this.DoChangeTheme(x) }); }
        }

        protected virtual void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var accent = ThemeManager.GetAccent(Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            Model.CurrentAccent = this;
        }
    }

    public class AppThemeMenuData : AccentColorMenuData
    {
        protected override void DoChangeTheme(object sender)
        {
            var theme = ThemeManager.DetectAppStyle(Application.Current);
            var appTheme = ThemeManager.GetAppTheme(Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
            Model.CurrentTheme = this;
        }
    }

    public class SimpleCommand : ICommand
    {
        public Predicate<object> CanExecuteDelegate { get; set; }
        public Action<object> ExecuteDelegate { get; set; }

        public bool CanExecute(object parameter)
        {
            if (CanExecuteDelegate != null)
                return CanExecuteDelegate(parameter);
            return true; // if there is no can execute default to true
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            if (ExecuteDelegate != null)
                ExecuteDelegate(parameter);
        }
    }
}
