using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTget.GUI
{
    class Program
    {
        [STAThread]
        public static void Main()
        {
            
            ILBundle.RegisterAssemblyResolver();
#if DEBUG
            System.Diagnostics.Debugger.Launch();

#endif
            App.Main();
        }
    }
}
