using System.Collections.Generic;
using Helion.Core.Resource.Maps;

namespace Helion.Core.Archive
{
    /// <summary>
    /// An iterator for some archive that finds maps.
    /// </summary>
    /// <remarks>
    /// This is not designed to be used with an archive that mutates between
    /// iteration calls. It is both unsafe and undefined behavior to do so.
    /// It is best to use it all at once and then stop using it.
    /// </remarks>
    public interface IArchiveMapIterator : IEnumerable<MapComponents>
    {
    }
}
