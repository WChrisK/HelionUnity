using Helion.Util;

namespace Helion.Archives.Wads
{
    /// <summary>
    /// An entry inside of the directory table of a wad.
    /// </summary>
    public class WadDirectoryEntry
    {
        /// <summary>
        /// The offset to where the entry data starts in the wad.
        /// </summary>
        public readonly int Offset;

        /// <summary>
        /// The amount of bytes in the entry.
        /// </summary>
        public readonly int Size;

        /// <summary>
        /// The name of the entry.
        /// </summary>
        public readonly UpperString Name;

        /// <summary>
        /// Creates a new wad entry directory.
        /// </summary>
        /// <param name="offset">The offset to where the entry data starts in
        /// the wad.</param>
        /// <param name="size">The amount of bytes in the entry.</param>
        /// <param name="name">The name of the entry.</param>
        public WadDirectoryEntry(int offset, int size, UpperString name)
        {
            Offset = offset;
            Size = size;
            Name = name;
        }

        public override string ToString() => $"{Name} (offset = {Offset}, size = {Size})";
    }
}
