using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WoTget.Core.Authoring
{
    public static class ManifestHelper
    {
        public static IPackage Load(string path)
        {
            return FillPackage(XElement.Load(path));
        }

        public static IPackage LoadFromString(string xml)
        {
            return FillPackage(XElement.Parse(xml));
        }

        private static IPackage FillPackage(XElement xElement)
        {
            return new Package
            {
                Name = (string)xElement.Element("name"),
                Description = (string)xElement.Element("description"),
                Version = (string)xElement.Element("version"),
                Authors = (string)xElement.Element("authors"),
                Owners = (string)xElement.Element("owners"),
                ProjectUrl = (string)xElement.Element("projectUrl"),
                Tags = xElement.Element("tags").Elements().Select(e => e.Value).ToList()
            };
        }

        public static void Save(this IPackage package, string path)
        {
            var xml = CreateXDocument(package);
            xml.Save(path, SaveOptions.None);
        }

        public static void Save(this IPackage package, Stream stream)
        {
            var xml = CreateXDocument(package);
            xml.Save(stream, SaveOptions.None);
        }

        private static XDocument CreateXDocument(IPackage package)
        {
            if (package.Tags == null) package.Tags = new List<string>();
            return new XDocument(
                new XDeclaration("1.0", "utf-8", string.Empty),
                new XElement("package",
                    new XElement("name", package.Name),
                    new XElement("description", package.Description),
                    new XElement("version", package.Version),
                    new XElement("authors", package.Authors),
                    new XElement("owners", package.Owners),
                    new XElement("projectUrl", package.ProjectUrl),
                    new XElement("tags", package.Tags.Select(t => new XElement("tag", t)).ToArray())
                 )
                );
        }

       
    }
}
