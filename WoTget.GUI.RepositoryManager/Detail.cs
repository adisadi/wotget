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

namespace WoTget.GUI.RepositoryManager
{
    public partial class Detail : Form
    {
        public Detail()
        {
            InitializeComponent();
            button1.Enabled = false;
        }

        public Detail(IPackage package):this()
        {
            iPackageBindingSource.DataSource = new PackageModel(package);
            ((PackageModel)iPackageBindingSource.DataSource).PropertyChanged += Detail_PropertyChanged;
        }

        private void Detail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            button1.Enabled = true;
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.DataBindings[0].WriteValue();
        }

        public PackageModel PackageModel {
            get { return ((PackageModel)iPackageBindingSource.DataSource);  } }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"Delete Package {PackageModel.Name}?", "Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation) == DialogResult.Yes)
            {
                DialogResult = DialogResult.Yes;
                this.Close();
            }
        }
    }
}
