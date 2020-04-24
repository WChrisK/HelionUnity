using System.Collections.Generic;
using Helion.Core.Resource.MapsNew.Components;

namespace Helion.Core.Worlds.Geometry
{
    public class Sector
    {
        public readonly int Index;
        public readonly SectorPlane Floor;
        public readonly SectorPlane Ceiling;
        public readonly List<Side> Sides = new List<Side>();

        public Sector(int index, MapSector mapSector, SectorPlane floor, SectorPlane ceiling)
        {
            Index = index;
            Floor = floor;
            Ceiling = ceiling;
        }
    }
}
