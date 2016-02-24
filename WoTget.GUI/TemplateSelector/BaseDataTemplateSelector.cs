using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace WoTget.GUI.TemplateSelector
{
    public class BaseDataTemplateSelector : DataTemplateSelector
    {
        #region Member Variables
        #endregion

        #region Constructors
        /*
         * The default constructor
         */
        public BaseDataTemplateSelector()
        {
        }
        #endregion

        #region Properties
        #endregion

        #region Functions
        protected MainWindow GetMainWindow(DependencyObject inContainer)
        {
            DependencyObject c = inContainer;
            while (true)
            {
                DependencyObject p = VisualTreeHelper.GetParent(c);

                if (c is MainWindow)
                {
                    //mSectionControl = c;
                    return c as MainWindow;
                }
                else
                {
                    c = p;
                }
            }
        }
        #endregion
    }

}
