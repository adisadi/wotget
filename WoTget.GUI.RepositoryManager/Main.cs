using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using WoTget.Core.Authoring;

namespace WoTget.GUI.RepositoryManager
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();

            MyApplication.InitializeInstance(JsonConfig.Config.Global.GoogleKeyFile);
        }

        private void Main_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        private void LoadData()
        {
            iPackageBindingSource.DataSource = MyApplication.Instance.GetPackages().OrderBy(p => p.Name).ThenByDescending(p => p.SemanticVersion());
            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridView1.AutoResizeColumns();
        }

        private void dataGridView1_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var package = (IPackage)dataGridView1.CurrentRow.DataBoundItem;
            var detail = new Detail(package);

            var result = detail.ShowDialog();

            if (result == DialogResult.OK)
            {
                MyApplication.Instance.Save(detail.PackageModel,package);
                LoadData();
            }
            else if (result==DialogResult.Yes)
            {
                MyApplication.Instance.Delete(package);
                LoadData();
            }
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {

            var package = new PackageModel() { Tags = new List<string>(),Version="1.0" };

            var newD = new New(package);
            if (newD.ShowDialog() == DialogResult.OK)
            {
                MyApplication.Instance.New(newD.PackageModel, newD.Files);
               LoadData();
            }
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var package = (IPackage)dataGridView1.CurrentRow.DataBoundItem;

            var newD = new New(package);
            if (newD.ShowDialog() == DialogResult.OK)
            {
                MyApplication.Instance.New(newD.PackageModel, newD.Files);
                LoadData();
            }


        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var package = (IPackage)dataGridView1.CurrentRow.DataBoundItem;

            if (MessageBox.Show($"Delete Package {package.Name}?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                MyApplication.Instance.Delete(package);
                LoadData();
            }
        }
    }
}
