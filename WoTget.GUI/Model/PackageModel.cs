using MahApps.Metro;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using WoTget.Core.Authoring;

namespace WoTget.GUI.Model
{
    public enum State
    {
        None = 0,
        Installed,
        NeedsUpdate
    }
    public class PackageModel : Package, INotifyPropertyChanged
    {

        public PackageModel(IPackage package)
        {
            Name = package.Name;  
            Tags = package.Tags;
            Version = package.Version;
            Description = package.Description;
        }

        private State packageState;
        public State PackageState { get { return packageState; } set { if (value != packageState) { packageState = value; OnPropertyChanged(); OnPropertyChanged("Canvas"); OnPropertyChanged("Brush"); } } }

        public Canvas Canvas
        {
            get
            {
                if (PackageState == State.Installed)
                {
                    return (Canvas)App.Current.Resources["appbar_page_check"];
                }
                else if (PackageState == State.NeedsUpdate)
                {
                    return (Canvas)App.Current.Resources["appbar_page_upload"];
                }
                else
                {
                    return (Canvas)App.Current.Resources["appbar_page_add"];
                }


            }
        }

        public Brush Brush
        {
            get
            {
                if (PackageState == State.Installed)
                {
                    //return new SolidColorBrush(Colors.LightGreen);
                    return (Brush)ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush"];


                }
                else if (PackageState == State.NeedsUpdate)
                {
                    return (Brush)ThemeManager.DetectAppStyle().Item2.Resources["AccentColorBrush4"];
                }
                else
                {
                    //return new SolidColorBrush(Colors.White);
                    return (Brush)ThemeManager.DetectAppStyle().Item2.Resources["IdealForegroundColorBrush"];
                }
            }
        }

        public string Tooltip
        {
            get
            {
                if (PackageState == State.Installed)
                {
                    return "Click to Uninstall";
                }
                else if (PackageState == State.NeedsUpdate)
                {
                    return "Click to Update";
                }
                else
                {
                    return "Click to Install"; ;
                }
            }
        }

        public string TagsString
        {
            get
            {
                if (Tags == null || Tags.Count == 0) return null;
                return "Tags:" + string.Join("/", Tags);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] String caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

    }
}
