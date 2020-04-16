using System;
using Helion.Core.Archives.PK3s;
using Helion.Core.Archives.Wads;
using Helion.Core.Util;

namespace Helion.Core.Archives
{
    /// <summary>
    /// A helper for reading archives.
    /// </summary>
    public static class ArchiveReader
    {
        /// <summary>
        /// Reads an archive from the path provided.
        /// </summary>
        /// <param name="path">The path to read from.</param>
        /// <returns>The archive, if it can be read.</returns>
        public static Optional<IArchive> ReadFile(string path)
        {
            if (path.EndsWith(".wad", StringComparison.OrdinalIgnoreCase))
            {
                Optional<Wad> wad = Wad.From(path);
                return wad ? wad.Value : Optional<IArchive>.Empty();
            }

            // Note that by not searching for PK3 only, we indirectly support
            // reading anything that is a zip archive.
            Optional<PK3> pk3 = PK3.From(path);
            return pk3 ? pk3.Value : Optional<IArchive>.Empty();
        }
    }
}
