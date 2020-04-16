using System.Collections.Generic;
using Helion.Core.Resource;
using Helion.Core.Util;

namespace Helion.Core.Archives
{
    /// <summary>
    /// An archive that contains entries.
    /// </summary>
    public interface IArchive : IEnumerable<IEntry>
    {
        /// <summary>
        /// The location for this archive.
        /// </summary>
        string Uri { get; }

        /// <summary>
        /// Finds an entry by name. Does not consider extensions or namespaces.
        /// This will return the most recent entry, so if there are multiple
        /// entries with the same name, the latest entry is returned.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <returns>The entry if it exists, or an empty optional.</returns>
        Optional<IEntry> Find(UpperString name);

        /// <summary>
        /// Finds an entry by name with the namespace provided. Does not look
        /// at extensions. This will return the most recent entry, so if there
        /// are multiple entries with the same name, the latest entry is
        /// returned.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <param name="type">The namespace type.</param>
        /// <returns>The entry if it exists, or an empty optional.</returns>
        Optional<IEntry> Find(UpperString name, ResourceNamespace type);

        /// <summary>
        /// Finds the latest entry by path. Everything is taken into account,
        /// meaning directory slashes, extensions, etc. It is case insensitive.
        /// Namespaces are not taken into account.
        /// </summary>
        /// <param name="path">The path to search (ex: "flats/hi.mp3").</param>
        /// <returns>The entry if it exists, or an empty optional.</returns>
        Optional<IEntry> FindPath(UpperString path);

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
