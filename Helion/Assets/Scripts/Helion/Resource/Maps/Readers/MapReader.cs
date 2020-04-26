using Helion.Resource.Maps;
using UnityEngine;

namespace Helion.Resource.Maps.Readers
{
    /// <summary>
    /// Handles reading maps.
    /// </summary>
    public static class MapReader
    {
        /// <summary>
        /// Reads the map components and tries to generate the map.
        /// </summary>
        /// <param name="components">The map components.</param>
        /// <param name="map">The map that was generated, or null if this
        /// returns false.</param>
        /// <returns>True on success, false on failure.</returns>
        public static bool TryRead(MapComponents components, out MapData map)
        {
            switch (components.MapType)
            {
            case MapType.Doom:
                if (DoomMapReader.TryRead(components, out map))
                    return true;
                break;
            case MapType.Hexen:
                // TODO
                break;
            case MapType.UDMF:
                // TODO
                break;
            default:
                Debug.Assert(false, $"Unknown map type: {components.MapType}");
                break;
            }

            map = null;
            return false;
        }
    }
}
