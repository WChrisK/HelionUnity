using System.Collections;
using System.Collections.Generic;
using Helion.Core.Archives.Wads;
using Helion.Core.Resource.Maps;
using Helion.Core.Util;

namespace Helion.Core.Archives.PK3s
{
    /// <summary>
    /// Finds all of the maps in a PK3 archive.
    /// </summary>
    public class PK3ArchiveMapIterator : IArchiveMapIterator
    {
        private static readonly UpperString MapsFolderName = "MAPS";

        private readonly PK3 pk3;

        public PK3ArchiveMapIterator(PK3 pk3)
        {
            this.pk3 = pk3;
        }

        public IEnumerator<MapComponents> GetEnumerator()
        {
            foreach (IEntry entry in pk3.TopLevelFolderEntries(MapsFolderName))
            {
                Optional<Wad> wad = Wad.From(entry.Path.ToString(), entry.Data);
                if (!wad)
                    continue;

                foreach (MapComponents mapComponents in wad.Value.GetMaps())
                {
                    // The name of the wad archive is to take place of the map
                    // name for compatibility reasons.
                    mapComponents.Name = entry.Path.Name;
                    yield return mapComponents;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
