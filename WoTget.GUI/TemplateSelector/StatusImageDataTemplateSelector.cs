using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WoTget.GUI.Model;

namespace WoTget.GUI.TemplateSelector
{
    public class StatusImageDataTemplateSelector : BaseDataTemplateSelector
    {
        #region Constructors

        /*
         * The default constructor
         */
        public StatusImageDataTemplateSelector()
        {
        }
        #endregion

        #region Functions
        public override DataTemplate SelectTemplate(object inItem, DependencyObject inContainer)
        {
            DataRowView row = inItem as DataRowView;

            if (row != null)
            {
                if (row.DataView.Table.Columns.Contains("PackageState"))
                {
                    MainWindow w = GetMainWindow(inContainer);
                    State status = (State)row["PackageState"];
                    if (status == State.None)
                    {
                        return (DataTemplate)w.FindResource("appbar_cloud_download");
                    }
                    if (status == State.Installed)
                    {
                        return (DataTemplate)w.FindResource("appbar_cloud_download");
                    }
                    if (status == State.NeedsUpdate)
                    {
                        return (DataTemplate)w.FindResource("appbar_cloud_download");
                    }
                }
            }
            return null;
        }
        #endregion
    }

}
