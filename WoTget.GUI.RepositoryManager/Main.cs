using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoTget.Core.Authoring;
using WoTget.Core.Repositories.GoogleDrive;

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

            var package = new PackageModel() { Tags = new List<string>() };

            var newD = new New(package);
            if (newD.ShowDialog() == DialogResult.OK)
            {
                MyApplication.Instance.New(newD.PackageModel, newD.Files);
               LoadData();
            }
        }
    }
}
