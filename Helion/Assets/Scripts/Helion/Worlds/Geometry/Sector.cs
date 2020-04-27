using System.Collections.Generic;
using Helion.Resource.Maps.Components;
using Helion.Worlds.Geometry.Subsectors;

namespace Helion.Worlds.Geometry
{
    /// <summary>
    /// A sector in a map, made up of subsectors and sector planes.
    /// </summary>
    public class Sector
    {
        public readonly int Index;
        public readonly SectorPlane Floor;
        public readonly SectorPlane Ceiling;
        public readonly List<Side> Sides = new List<Side>();
        public readonly List<Subsector> Subsectors = new List<Subsector>();
        // TODO: Setting this should update mesh colors.
        public int LightLevel { get; private set; }
        public float LightLevelNormalized { get; private set; }

        public Sector(int index, MapSector mapSector, SectorPlane floor, SectorPlane ceiling)
        {
            Index = index;
            Floor = floor;
            Ceiling = ceiling;
            SetLightLevel(mapSector.LightLevel);

            floor.Sector = this;
            ceiling.Sector = this;
        }

        public void SetLightLevel(int lightLevel)
        {
            LightLevel = lightLevel;
            LightLevelNormalized = WorldUtil.ToDoomLightLevel(lightLevel);
        }
    }
}
