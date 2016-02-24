using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WoTget.Core.Authoring
{
    public class Package : IPackage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Version { get; set; }
        public string Authors { get; set; }
        public string Owners { get; set; }
        public string ProjectUrl { get; set; }
        public List<string> Tags { get; set; }

        public static bool operator ==(Package a, Package b)
        {
            if (((object)a) == null || ((object)b) == null)
                return Object.Equals(a, b);

            return a.Equals(b);
        }

        public static bool operator !=(Package a, Package b)
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
