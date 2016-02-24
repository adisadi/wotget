using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace WoTget.Core.Database
{
    internal class LocalDatabaseConfig
    {
        internal string WoTHome { get; set; }
        internal string WoTVersion { get; set; }

        internal List<LocalDatabasePackageInfo> Packages { get; set; }


        public void Save(string path)
        {
            var xml = CreateXDocument(this);
            xml.Save(path, SaveOptions.None);
        }

        public static LocalDatabaseConfig Load(string path)
        {
            return FillConfig(XElement.Load(path));
        }

        private static XDocument CreateXDocument(LocalDatabaseConfig config)
        {
            if (config.Packages == null) config.Packages = new List<LocalDatabasePackageInfo>();
            return new XDocument(
                new XDeclaration("1.0", "utf-8", string.Empty),
                new XElement("databaseConfig", 
                new XElement("wotHome", config.WoTHome),
                new XElement("wotVersion", config.WoTVersion),
                new XElement("packages", config.Packages.Select(p => new XElement("package", new XAttribute("name", p.Name), new XAttribute("version", p.Version)))
                 ))
                );
        }

        private static LocalDatabaseConfig FillConfig(XElement xElement)
        {
            return new LocalDatabaseConfig
            {
                WoTHome = (string)xElement.Element("wotHome"),
                WoTVersion = (string)xElement.Element("wotVersion"),
                Packages = xElement.Element("packages").Elements().Select(e => new LocalDatabasePackageInfo { Name = e.Attribute("name").Value, Version = e.Attribute("version").Value }).ToList()
            };
        }


    }

    internal class LocalDatabasePackageInfo
    {
        internal string Name { get; set; }
        internal string Version { get; set; }
    }
}
