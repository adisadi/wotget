using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using WoTget.Core.Authoring;
using WoTget.GUI.Model;

namespace WoTget.GUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private Client Client;

        private string lastSearch = string.Empty;

        public MainWindow()
        {
            InitializeComponent();

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

        }

        private async void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            await this.ShowMessageAsync("Exception", ((Exception)e.ExceptionObject).Message);
        }

        private Task ExecuteLongTask(Action action)
        {
            progressRing.IsActive = true;
            progressRing.Visibility = Visibility.Visible;

            return Task.Run(() =>
            {
                action();
            }).ContinueWith((taskOne) =>
            {
                Dispatcher.Invoke((Action)(() => progressRing.IsActive = false));
                Dispatcher.Invoke((Action)(() => progressRing.Visibility = Visibility.Collapsed));
            });
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Client = Client.Instance;

            if (!Client.IsDatabaseInitialized())
            {
                flyoutControl.Model.WoTHome = Client.GetWotHome();
                flyoutControl.Model.WoTVersion = Client.GetWotVersion();

                ReloadGrid();
            }
            else
            {
                InitWotGameDirectory();
            }
        }

        private void ReloadGrid()
        {
            ReloadGrid(Client.GetPackagesFromRepository());
        }


        private void ReloadGrid(IEnumerable<IPackage> packagesNames)
        {
            var packages = Client.VerifiyPackageList(packagesNames);
           
            dataGridAll.DataContext = packages;

            if (tabInstalled.IsSelected)
            {
                ReloadInstalled();
            }

            if (tabUpdates.IsSelected)
            {
                ReloadUpdates();
            }
           
        }

        private async void menuItemWotFolder_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteLongTask(() =>
            {
                Dispatcher.Invoke((Action)(() => InitWotGameDirectory()));
            });
        }

        private async void menuItemSearch_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Search", "Search in Name,Description and Tags", new MetroDialogSettings()
            {
                AnimateHide = true,
                AnimateShow = true,
                DefaultText = lastSearch
            });

            if (!string.IsNullOrEmpty(result))
            {
                Dispatcher.Invoke((Action)(() =>
                {
                    lastSearch = result;
                    ReloadGrid(Client.GetPackagesFromRepository(result));
                }));

            }
        }

        private async void menuItemReload_Click(object sender, RoutedEventArgs e)
        {
            await ExecuteLongTask(() =>
            {
                Dispatcher.Invoke((Action)(() => ReloadGrid()));
            });
        }


        private async void InitWotGameDirectory()
        {
            var result = await this.ShowInputAsync("WOT", "WOT Game Directory", new MetroDialogSettings()
            {
                AnimateHide = true,
                AnimateShow = true,
                DefaultText = Client.GetWotHome()!=string.Empty ? Client.GetWotHome() : @"C:\Games\World_of_Tanks"
            });
            if (!string.IsNullOrEmpty(result))
            {
                try
                {
                    Client.Init(result, Client.IsDatabaseInitialized());
                    flyoutControl.Model.WoTHome = Client.GetWotHome();
                    flyoutControl.Model.WoTVersion = Client.GetWotVersion();
                    ReloadGrid();
                }
                catch (FileNotFoundException)
                {
                    await this.ShowMessageAsync("Exception", "Not a valid Wot Game Directory.");
                }
                catch (Exception ex)
                {
                    await this.ShowMessageAsync("Exception", ex.ToString());
                }

            }
        }

     
        private async void dataGrid_MenuItemClicked(object sender, Xamls.MenuItemEventArgs e)
        {
            switch (e.Action)
            {
                case Xamls.MenuItemAction.Install:
                    await ExecuteLongTask(() =>
                    {
                        Client.InstallPackages(e.Packages);
                        e.Packages.ToList().ForEach(p => p.PackageState = State.Installed);
                    });
                    break;
                case Xamls.MenuItemAction.Uninstall:
                    await ExecuteLongTask(() =>
                    {
                        Client.UninstallPackages(e.Packages);
                        e.Packages.ToList().ForEach(p => p.PackageState = State.None);
                    });
                    break;
                case Xamls.MenuItemAction.Update:
                    await ExecuteLongTask(() =>
                    {
                        Client.UpdatePackages(e.Packages);
                        e.Packages.ToList().ForEach(p => p.PackageState = State.Installed);
                    });
                    break;
            }
        }

        private void tabControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            MetroAnimatedTabControl control = sender as MetroAnimatedTabControl; //The sender is a type of TabItem...

            if (control.SelectedItem != null)
            {
                if (((TabItem)control.SelectedItem).Name== "tabInstalled")
                {
                    ReloadInstalled();
                }

                if (((TabItem)control.SelectedItem).Name == "tabUpdates")
                {
                    ReloadUpdates();
                }
            }
        }

        private void ReloadInstalled()
        {
            dataGridInstalled.DataContext = dataGridAll.Data.Where(p => p.PackageState == State.Installed).ToList();
        }

        private void ReloadUpdates()
        {
            dataGridUpdates.DataContext = dataGridAll.Data.Where(p => p.PackageState == State.NeedsUpdate).ToList();
        }
    }
}
