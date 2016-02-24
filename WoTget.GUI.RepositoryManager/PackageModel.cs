using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using WoTget.Core.Authoring;

namespace WoTget.GUI.RepositoryManager
{
    public class PackageModel : IPackage, INotifyPropertyChanged
    {

        public PackageModel(IPackage package)
        {
            Name = package.Name;
            Owners = package.Owners;
            ProjectUrl = package.ProjectUrl;
            Tags = package.Tags;
            Version = package.Version;
            Authors = package.Authors;
            Description = package.Description;
        }

        public PackageModel()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;


        private string name;
        public string Name { get { return name; } set { if (value != name) { name = value; OnPropertyChanged(); } } }

        private string description;
        public string Description { get { return description; } set { if (value != description) { description = value; OnPropertyChanged(); } } }

        private string version;
        public string Version { get { return version; } set { if (value != version) { version = value; OnPropertyChanged(); } } }

        private string authors;
        public string Authors { get { return authors; } set { if (value != authors) { authors = value; OnPropertyChanged(); } } }

        private string owners;
        public string Owners { get { return owners; } set { if (value != owners) { owners = value; OnPropertyChanged(); } } }

        private string projectUrl;
        public string ProjectUrl { get { return projectUrl; } set { if (value != projectUrl) { projectUrl = value; OnPropertyChanged(); } } }

        private List<string> tags;
        public List<string> Tags { get { return tags; } set { if (value != tags) { tags = value; OnPropertyChanged(); } } }

        public string TagsString { get { return string.Join(" ", tags); } set { if (value != string.Join(" ", tags)) { Tags = value.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).ToList(); } } }

        private void OnPropertyChanged([CallerMemberName] String caller = null)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(caller));
            }
        }

        public static bool operator ==(PackageModel a, PackageModel b)
        {
            if (((object)a) == null || ((object)b) == null)
                return Object.Equals(a, b);

            return a.Equals(b);
        }

        public static bool operator !=(PackageModel a, PackageModel b)
        {
            return !(a == b);
        }

        public bool Equals(IPackage other)
        {
            if (other == null) return false;
            return this.Name == other.Name && this.Version == other.Version;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            IPackage package = obj as IPackage;
            if (package == null)
                return false;
            else
                return Equals(package);
        }

        public override int GetHashCode()
        {
            return this.FileName().GetHashCode();
        }

    }
}
