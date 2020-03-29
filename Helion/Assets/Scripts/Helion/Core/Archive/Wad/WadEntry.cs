using Helion.Core.Resource;
using Helion.Core.Util;

namespace Helion.Core.Archive.Wad
{
    /// <summary>
    /// An entry in a wad file.
    /// </summary>
    public class WadEntry : IEntry
    {
        public EntryPath Path { get; }
        public ResourceNamespace Namespace { get; }
        public byte[] Data { get; }

        /// <summary>
        /// The name of the entry. This will be identical to the upper case
        /// name of the entry path.
        /// </summary>
        public UpperString Name;

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
            Namespace = resourceNamespace;
            Data = data;
            Name = Path.Name;
        }

        public override string ToString() => Path.ToString();
    }
}
