using Helion.Resource;
using Helion.Util;

namespace Helion.Archives
{
    /// <summary>
    /// An entry in an archive.
    /// </summary>
    public interface IEntry
    {
        /// <summary>
        /// The path to the entry.
        /// </summary>
        EntryPath Path { get; }

        /// <summary>
        /// The upper cased name of the entry.
        /// </summary>
        UpperString Name { get; }

        /// <summary>
        /// The namespace of the entry.
        /// </summary>
        ResourceNamespace Namespace { get; }

        /// <summary>
        /// The data for the entry.
        /// </summary>
        byte[] Data { get; }
    }
}
