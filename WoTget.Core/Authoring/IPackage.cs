using System;
using System.Collections.Generic;

namespace WoTget.Core.Authoring
{
    public interface IPackage:IEquatable<IPackage>
    {
        string Authors { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        string Owners { get; set; }
        string ProjectUrl { get; set; }
        List<string> Tags { get; set; }
        string Version { get; set; }
    }
}