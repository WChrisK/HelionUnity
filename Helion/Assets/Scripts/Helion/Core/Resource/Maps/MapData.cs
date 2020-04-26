using System.Collections.Generic;
using Helion.Core.Resource.Maps.Components;
using Helion.Core.Util;

namespace Helion.Core.Resource.Maps
{
    /// <summary>
    /// Data for a map that is verified to be well-formed.
    /// </summary>
    public class MapData
    {
        public readonly UpperString Name;
        public readonly List<MapLinedef> Linedefs = new List<MapLinedef>();
        public readonly List<MapSector> Sectors = new List<MapSector>();
        public readonly List<MapSidedef> Sidedefs = new List<MapSidedef>();
        public readonly List<MapThing> Things = new List<MapThing>();
        public readonly List<MapVertex> Vertices = new List<MapVertex>();

        // Intended only to be created by internal components so we know that
        // all the indices for the lookup values (ex: lines point to the right
        // sides) are correct.
        internal MapData(UpperString name)
        {
            Name = name;
        }
    }
}
