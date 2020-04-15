using Helion.Core.Resource;
using Helion.Core.Util;

namespace Helion.Core.Archives.Wads
{
    /// <summary>
    /// An entry in a wad file.
    /// </summary>
    public class WadEntry : IEntry
    {
        public EntryPath Path { get; }
        public UpperString Name { get; }
        public ResourceNamespace Namespace { get; }
        public byte[] Data { get; }

        /// <summary>
        /// Creates a new wad entry.
        /// </summary>
        /// <param name="name">The wad entry name.</param>
        /// <param name="resourceNamespace">The resource namespace of this
        /// entry.</param>
        /// <param name="data">The data for this entry.</param>
        public WadEntry(UpperString name, ResourceNamespace resourceNamespace, byte[] data)
        {
            Path = new EntryPath(name);
            Name = Path.Name;
            Namespace = resourceNamespace;
            Data = data;
        }

        public override string ToString() => Path.ToString();
    }
}
