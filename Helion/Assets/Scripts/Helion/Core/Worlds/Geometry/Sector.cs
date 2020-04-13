using System.Collections.Generic;
using Helion.Core.Util;

namespace Helion.Core.Worlds.Geometry
{
    public class Sector
    {
        public readonly int Index;
        public readonly SectorPlane FloorPlane;
        public readonly SectorPlane CeilingPlane;
        public readonly List<Subsector> Subsectors = new List<Subsector>();
        public int LightLevel;

        public float LightLevelNormalized => LightLevel * Constants.InverseLightLevel;

        public Sector(int index, SectorPlane floorPlane, SectorPlane ceilingPlane, int lightLevel)
        {
            Index = index;
            FloorPlane = floorPlane;
            CeilingPlane = ceilingPlane;
            LightLevel = lightLevel;
        }
    }
}
