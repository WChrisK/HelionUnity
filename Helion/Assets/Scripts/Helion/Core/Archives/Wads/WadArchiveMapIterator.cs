using System.Collections;
using System.Collections.Generic;
using Helion.Core.Resource.MapsNew;
using Helion.Core.Util;

namespace Helion.Core.Archives.Wads
{
    /// <summary>
    /// A map finding iterator for a wad archive.
    /// </summary>
    public class WadArchiveMapIterator : IArchiveMapIterator
    {
        private static readonly HashSet<UpperString> MapEntryNames = new HashSet<UpperString>
        {
            "THINGS", "LINEDEFS", "SIDEDEFS", "VERTEXES", "SEGS", "SSECTORS", "NODES", "SECTORS",
            "REJECT", "BLOCKMAP", "BEHAVIOR", "SCRIPTS", "TEXTMAP", "ZNODES", "DIALOGUE", "ENDMAP",
            "GL_LEVEL", "GL_VERT", "GL_SEGS", "GL_SSECT", "GL_NODES"
        };

        private readonly Wad wad;
        private readonly UpperString mapName;
        private UpperString glLevelName = "";

        /// <summary>
        /// Wraps around a wad archive and will search maps from it.
        /// </summary>
        /// <param name="wad">The wad to use.</param>
        /// <param name="mapName">An external name to the map, which should be
        /// left null if this is not being set forcefully by a PK3.</param>
        public WadArchiveMapIterator(Wad wad, UpperString mapName = null)
        {
            this.wad = wad;
            this.mapName = mapName ?? "";
        }

        public IEnumerator<MapComponents> GetEnumerator()
        {
            IEntry lastEntry = null;
            bool makingMap = false;
            MapComponents components = new MapComponents(mapName);

            foreach (IEntry entry in wad)
            {
                bool isMapEntry = IsMapEntry(entry.Path.Name);

                if (makingMap)
                {
                    if (isMapEntry)
                        components.Track(entry);
                    else
                    {
                        makingMap = false;

                        if (components.IsValid())
                            yield return components;

                        components = new MapComponents(mapName);
                        glLevelName = "";
                    }
                }
                else if (isMapEntry)
                {
                    if (lastEntry != null)
                    {
                        glLevelName = $"GL_{lastEntry.Path.Name}";

                        // This is a way of checking if our 'hacky override' to
                        // support external map naming should be done or not. If
                        // there is no level name to override (empty string for the
                        // name) then we know we should use the marker name.
                        if (mapName.Empty)
                            components = new MapComponents(lastEntry.Path.Name);
                    }

                    // It is the case that we could have a map with no marker
                    // entry at the beginning. Such a case is a malformed map
                    // and the builder will know what to do here if the last
                    // entry is null.
                    components.TrackMapMarker(lastEntry);
                    components.Track(entry);
                    makingMap = true;
                }

                lastEntry = entry;
            }

            // If we run out of entries but have a map that is potentially done
            // yet not processed, return it.
            if (components.IsValid())
                yield return components;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private bool IsMapEntry(UpperString name)
        {
            return MapEntryNames.Contains(name) || glLevelName == name;
        }
    }
}
