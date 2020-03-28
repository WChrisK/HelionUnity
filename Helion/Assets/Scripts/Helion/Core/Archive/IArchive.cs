using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Archive
{
    public interface IArchive : IEnumerable<IEntry>
    {
        /// <summary>
        /// Finds an entry by name. Does not consider extensions or namespaces.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <returns>The entry if it exists, or an empty optional.</returns>
        Optional<IEntry> Find(UpperString name);

        /// <summary>
        /// Finds all of the entries with the matching name. Does not consider
        /// extensions or namespaces.
        /// </summary>
        /// <param name="name">The name to search.</param>
        /// <returns>A list of entries, or an empty list if none are found.
        /// </returns>
        IEnumerable<IEntry> FindAll(UpperString name);
    }
}
