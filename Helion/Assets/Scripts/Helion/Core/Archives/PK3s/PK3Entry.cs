using Helion.Core.Resource;
using Helion.Core.Util;

namespace Helion.Core.Archives.PK3s
{
    /// <summary>
    /// An entry in a PK3 file.
    /// </summary>
    public class PK3Entry : IEntry
    {
        public EntryPath Path { get; }
        public UpperString Name { get; }
        public ResourceNamespace Namespace { get; }
        public byte[] Data { get; }

        public PK3Entry(EntryPath path, ResourceNamespace resourceNamespace, byte[] data)
        {
            Path = path;
            Name = path.Name;
            Namespace = resourceNamespace;
            Data = data;
        }
    }
}
