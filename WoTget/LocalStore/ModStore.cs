using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using WoTget.Core.Authoring;
using System.IO;
using WoTget.Core;
using System;

namespace WoTget.LocalStore
{
    public class StoreItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Version { get; set; }
        [JsonIgnore]
        public string FileName
        {
            get
            {
                return $"{this.Id}.{new SemanticVersion(this.Version).ToNormalizedString()}{Constants.PackageExtension}";
            }
        }
    }

    public class Store
    {

        private List<StoreItem> items = new List<StoreItem>();
        private string file;
        private string directory;

        public Store(string file)
        {
            this.file = file;
            this.directory = Path.GetDirectoryName(this.file);

            if (!Directory.Exists(directory))
            {
                var d = Directory.CreateDirectory(directory);
                d.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }

            Load();
        }

        public void Add(IPackage package, Stream stream)
        {
            var item = Find(package);
            if (item != null)
            {
                if (item.Version == package.SemanticVersion.ToNormalizedString()) return;
                Remove(package);
            }

            this.items.Add(new StoreItem { Id = package.Id, Name = package.Name, Version = package.SemanticVersion.ToNormalizedString() });

            stream.Seek(0, SeekOrigin.Begin);
            using (FileStream file = new FileStream(Path.Combine(this.directory, package.FileName), FileMode.Create, FileAccess.Write))
                stream.CopyTo(file);

            Save();
        }

        public void Remove(IPackage package)
        {
            var item = Find(package);
            if (item == null) return;

            this.items.Remove(item);

            if (File.Exists(Path.Combine(this.directory, item.FileName)))
                File.Delete(Path.Combine(this.directory, item.FileName));

            Save();
        }

        public MemoryStream GetPackage(IPackage package)
        {

            var item = Find(package);
            if (item == null) throw new ArgumentException("Package not found in Store Settings!");
            if (!File.Exists(Path.Combine(this.directory, item.FileName))) throw new ArgumentException("Package not found in Store!");


            MemoryStream ms = new MemoryStream();
            using (FileStream file = new FileStream(Path.Combine(this.directory, item.FileName), FileMode.Open, FileAccess.Read))
                file.CopyTo(ms);
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }

        public bool PackageExists(IPackage package)
        {
            var item = Find(package);
            if (item == null) throw new ArgumentException("Package not found in Store Settings!");

            return File.Exists(Path.Combine(this.directory, item.FileName));
        }

        private StoreItem Find(IPackage package)
        {
            return this.items.FirstOrDefault(s => s.Id == package.Id);
        }

        private void Load()
        {
            if (!File.Exists(this.file))
            {
                Save();
                return;
            }
            items = JsonConvert.DeserializeObject<List<StoreItem>>(File.ReadAllText(this.file));
        }

        private void Save()
        {
            var json = JsonConvert.SerializeObject(this.items);
            File.WriteAllText(this.file, json);
        }

        public IEnumerable<IPackage> Packages
        {
            get
            {
                return this.items.Select(i => new Package { Name = i.Name, Version = i.Version });
            }
        }

    }
}