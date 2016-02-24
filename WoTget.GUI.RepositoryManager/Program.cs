using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            Application.Run(new Main());
        }
    }
}
