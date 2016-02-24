using System.Windows.Controls;
using WoTget.GUI.Model;

namespace WoTget.GUI.Xamls
{
    /// <summary>
    /// Interaction logic for flyoutControl.xaml
    /// </summary>
    public partial class flyoutControl : UserControl
    {
        public flyoutControl()
        {
            InitializeComponent();
            Model = new FlyoutViewModel();
            DataContext = Model;
        }

        public FlyoutViewModel Model { get; private set; }
    }
}
