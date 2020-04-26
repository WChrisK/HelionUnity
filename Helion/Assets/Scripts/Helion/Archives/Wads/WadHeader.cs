namespace Helion.Archives.Wads
{
    /// <summary>
    /// The header for a wad archive.
    /// </summary>
    public class WadHeader
    {
        /// <summary>
        /// True if it's an iwad, false if it's a pwad.
        /// </summary>
        public readonly bool IsIwad;

        /// <summary>
        /// The number of entries in the wad.
        /// </summary>
        public readonly int EntryCount;

        /// <summary>
        /// The offset in bytes to the directory table.
        /// </summary>
        public readonly int DirectoryTableOffset;

        /// <summary>
        /// Creates a new wad header.
        /// </summary>
        /// <param name="isIwad">True if an iwad, false if a pwad.</param>
        /// <param name="entryCount">The number of entries in the wad.</param>
        /// <param name="directoryTableOffset">The byte offset to the entry
        /// directory table.</param>
        public WadHeader(bool isIwad, int entryCount, int directoryTableOffset)
        {
            IsIwad = isIwad;
            EntryCount = entryCount;
            DirectoryTableOffset = directoryTableOffset;
        }
    }
}
