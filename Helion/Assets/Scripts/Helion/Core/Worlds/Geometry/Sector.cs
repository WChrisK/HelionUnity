using System.Collections.Generic;

namespace Helion.Core.Worlds.Geometry
{
    public class Sector
    {
        public readonly int Index;
        public readonly SectorPlane FloorPlane;
        public readonly SectorPlane CeilingPlane;
        public readonly List<Subsector> Subsectors = new List<Subsector>();

        public Sector(int index, SectorPlane floorPlane, SectorPlane ceilingPlane)
        {
            Index = index;
            FloorPlane = floorPlane;
            CeilingPlane = ceilingPlane;
        }
    }
}
