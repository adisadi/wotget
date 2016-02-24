using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WoTget.GUI.Model;

namespace WoTget.GUI.Xamls
{
    /// <summary>
    /// Interaction logic for dataGridControl.xaml
    /// </summary>
    public partial class dataGridControl : UserControl
    {
        public event EventHandler<MenuItemEventArgs> MenuItemClicked;

        public dataGridControl()
        {
            InitializeComponent();
        }

        public new IList<PackageModel> DataContext
        {
            get { return (IList<PackageModel>)dataGrid.DataContext; }
            set
            {
                dataGrid.DataContext = value;
            }
        }

        private IEnumerable<PackageModel> GetSelectedItems()
        {
            return dataGrid.SelectedItems.OfType<Model.PackageModel>();
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {

            if (!(sender is DataGridRow))
            {
                e.Handled = true;
                return;
            }

            BuildContextMenu((Model.PackageModel)((DataGridRow)sender).DataContext, ((DataGridRow)sender).ContextMenu);

        }

        private void menuItemInstall_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClicked != null)
                MenuItemClicked(sender, new MenuItemEventArgs { Packages = GetSelectedItems(), Action = MenuItemAction.Install });
        }

        private void menuItemUninstall_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClicked != null)
                MenuItemClicked(sender, new MenuItemEventArgs { Packages = GetSelectedItems(), Action = MenuItemAction.Uninstall });   
        }

        private void menuItemUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (MenuItemClicked != null)
                MenuItemClicked(sender, new MenuItemEventArgs { Packages = GetSelectedItems(), Action = MenuItemAction.Update });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (!(sender is Button)) return;

            var package = (PackageModel)((Button)sender).Tag;

            MenuItemAction action = MenuItemAction.Install;

            if (package.PackageState == State.Installed)
                action = MenuItemAction.Uninstall;
            else if (package.PackageState == State.NeedsUpdate)
                action = MenuItemAction.Update;
            else
                action = MenuItemAction.Install;

            if (MenuItemClicked != null)
                MenuItemClicked(sender, new MenuItemEventArgs { Packages = new List<PackageModel>() { package }, Action = action });
        }

        private void BuildContextMenu(Model.PackageModel package, ContextMenu contextMenu)
        {
            if (package.PackageState == State.Installed)
            {
                ((MenuItem)contextMenu.Items[0]).Visibility = Visibility.Collapsed;
                ((MenuItem)contextMenu.Items[1]).Visibility = Visibility.Visible;
                ((MenuItem)contextMenu.Items[2]).Visibility = Visibility.Collapsed;

            }
            else if (package.PackageState == State.NeedsUpdate)
            {

                ((MenuItem)contextMenu.Items[0]).Visibility = Visibility.Collapsed;
                ((MenuItem)contextMenu.Items[1]).Visibility = Visibility.Visible;
                ((MenuItem)contextMenu.Items[2]).Visibility = Visibility.Visible;

            }
            else
            {
                ((MenuItem)contextMenu.Items[0]).Visibility = Visibility.Visible;
                ((MenuItem)contextMenu.Items[1]).Visibility = Visibility.Collapsed;
                ((MenuItem)contextMenu.Items[2]).Visibility = Visibility.Collapsed;
            }

            foreach (MenuItem item in contextMenu.Items)
            {
                item.DataContext = package;
            }
        }
    }

    public class MenuItemEventArgs : EventArgs
    {
        public IEnumerable<PackageModel> Packages { get; set; }
        public MenuItemAction Action { get; set; }
    }

    public enum MenuItemAction { Install, Update, Uninstall };
}
