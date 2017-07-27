using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace WoTget.Core.Authoring
{
    public static class MetaHelper
    {

        public const string MetaFileName = "meta.xml";

        public static IPackage Load(string path)
        {
            return FillPackage(XElement.Load(path));
        }

        public static IPackage LoadFromString(string xml)
        {
            return FillPackage(XElement.Parse(xml));
        }

        public static void Save(this IPackage package, Stream stream)
        {
            var xml = CreateXDocument(package);
            xml.Save(stream, SaveOptions.None);
        }

        private static XDocument CreateXDocument(IPackage package)
        {
            return new XDocument(
                    new XDeclaration("1.0", "utf-8", string.Empty),
                    new XElement("root",
                        new XElement("id", package.Id.Replace(" ", "_")),
                        new XElement("name", package.Name),
                        new XElement("description", package.Description),
                        new XElement("version", package.Version),
                        new XElement("tags", package.Tags.Select(t => new XElement("tag", t)).ToArray())
                     )
                    );
        }

        private static IPackage FillPackage(XElement xElement)
        {
            //If theres no tag element its not a package from us!!
            if (xElement.Element("tags") == null) return null;

            return new Package
            {
                Name = (string)xElement.Element("name"),
                Description = (string)xElement.Element("description"),
                Version = (string)xElement.Element("version"),
                Tags = xElement.Element("tags").Elements().Select(e => e.Value).ToList()
            };
        }
    }
}
