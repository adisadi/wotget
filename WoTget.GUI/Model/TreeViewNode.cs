using System.Collections.Generic;

namespace WoTget.GUI.Model
{
    public class TreeViewNode
    {
        public TreeViewNode()
        {
            Nodes = new List<TreeViewNode>();
            Packages = new List<PackageModel>();
        }

        public string Name { get; set; }

        public List<TreeViewNode> Nodes { get; set; }
        public List<PackageModel> Packages { get; set; }

        public IList<object> Items
        {
            get
            {
                IList<object> childNodes = new List<object>();
                foreach (var group in Nodes)
                    childNodes.Add(group);
                foreach (var entry in Packages)
                    childNodes.Add(entry);

                return childNodes;
            }
        }
    }
}
