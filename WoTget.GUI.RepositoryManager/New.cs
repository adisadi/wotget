using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoTget.Core;
using WoTget.Core.Authoring;

namespace WoTget.GUI.RepositoryManager
{
    public partial class New : Detail
    {
        public New():base()
        {
            InitializeComponent();
        }

        public New(IPackage package):base(package)
        {
            InitializeComponent();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                var folder = folderBrowserDialog.SelectedPath;

                if (string.IsNullOrEmpty(nameTextBox.Text))
                {
                    nameTextBox.Text = new DirectoryInfo(folder).Name;
                }

                Files =  Directory.GetFiles(folder, "*", SearchOption.AllDirectories).ToList();

                if (!files.All(f => f.Contains(Constants.PackageRootFolder)))
                {
                    MessageBox.Show($"All files must be inside '{Constants.PackageRootFolder}' Folder", "Wrong Package Format");
                    files.Clear();
                    Files = files;
                }
            }
        }

        private List<string> files;
        public List<string> Files
        {
            get { return files; }
            set
            {
                files = value;
                listView1.Items.Clear();
                listView1.Items.AddRange(files.Select(s => new ListViewItem(s)).ToArray());
                listView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
            }
        }
    }
}
