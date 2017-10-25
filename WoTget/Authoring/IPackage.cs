using System;
using System.Collections.Generic;

namespace WoTget.Core.Authoring
{
    public interface IPackage : IEquatable<IPackage>
    {
        string Id { get; }
        string Description { get; set; }
        string Name { get; set; }
        string Version { get; set; }
        string FileName { get; }
        SemanticVersion SemanticVersion { get; }
    }
}