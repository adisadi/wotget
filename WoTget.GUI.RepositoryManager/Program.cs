using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WoTget.Core;

namespace WoTget.GUI.RepositoryManager
{
    static class Program
    {
        [STAThreadAttribute]
        static void Main()
        {
            ILBundle.RegisterAssemblyResolver();
            Main2();
        }
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main2()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Check Version
            var newVersion = GitChecker.CheckNewVersion();
            if (newVersion != null)
            {
                var result = MessageBox.Show($"There's a new Version available {newVersion.ToString()}\n Download it now?", "New Version", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation);
                if (result == DialogResult.Yes)
                {
                    System.Diagnostics.Process.Start("https://github.com/adisadi/wotget/releases/latest");
                }
                Application.Exit();
            }

            Application.Run(new Main());
        }
    }
}
