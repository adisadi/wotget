using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WoTget.GUI.Model;

namespace WoTget.GUI.Xamls
{
    /// <summary>
    /// Interaction logic for tagTreeView.xaml
    /// </summary>
    public partial class tagTreeView : UserControl
    {
        public event EventHandler<MenuItemEventArgs> MenuItemClicked;

        public tagTreeView()
        {
            InitializeComponent();
        }

        public new IList<PackageModel> DataContext
        {
            set
            {
                data = value;
                treeView.ItemsSource = CreateTreeNodes(value);
            }
        }

        public IList<PackageModel> data;
        public IList<PackageModel> Data
        {
            get
            {
                return data;
            }
        }


        private List<TreeViewNode> CreateTreeNodes(IList<PackageModel> packages)
        {
            List<TreeViewNode> list = new List<TreeViewNode>();
   
            foreach (var p in packages)
            {
                if (p.Tags==null || p.Tags.Count() == 0)
                {
                    var defaultNode= list.SingleOrDefault(t => t.Name == "Misc");
                    if (defaultNode == null)
                    {
                        defaultNode = new TreeViewNode { Name = "Misc" };
                        list.Add(defaultNode);
                    }
                    defaultNode.Packages.Add(p);
                }
                else
                {
                    var node= RekTagNodeCreate(list, 0, p.Tags);
                    node.Packages.Add(p);
                }
            }

            return list;
        }

        private TreeViewNode RekTagNodeCreate(List<TreeViewNode> parents, int index, List<string> tags)
        {
         
            TreeViewNode node = parents.SingleOrDefault(t => t.Name == tags[index]);
            if (node == null)
            {
                node = new TreeViewNode() { Name = tags[index] };
                parents.Add(node);
            }

            if (index+1 < tags.Count)
                return RekTagNodeCreate(node.Nodes, index + 1, tags);
            else
                return node;
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

    }

   

   

}
