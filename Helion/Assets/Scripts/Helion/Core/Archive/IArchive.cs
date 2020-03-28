using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Archive
{
    /// <summary>
    /// An archive that contains entries.
    /// </summary>
    public interface IArchive : IEnumerable<IEntry>
    {
        /// <summary>
        /// Finds an entry by name. Does not consider extensions or namespaces.
        /// This will return the most recent entry, so if there are multiple
        /// entries with the same name, the latest entry is returned.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <returns>The entry if it exists, or an empty optional.</returns>
        Optional<IEntry> Find(UpperString name);

        /// <summary>
        /// Finds all of the entries with the matching name. Does not consider
        /// extensions or namespaces. The entries will be ordered so the first
        /// elements encountered are the ones found later on in the archive.
        /// </summary>
        /// <remarks>
        /// Because of the ordering, this means any entry that should be loaded
        /// over other ones will be at the front of the enumerable.
        /// </remarks>
        /// <param name="name">The name to search.</param>
        /// <returns>A list of entries, or an empty list if none are found.
        /// </returns>
        IEnumerable<IEntry> FindAll(UpperString name);

        /// <summary>
        /// Gets an iterator over the maps in the archive.
        /// </summary>
        /// <returns>An iterator for the maps in the archive.</returns>
        IArchiveMapIterator GetMaps();
    }
}
