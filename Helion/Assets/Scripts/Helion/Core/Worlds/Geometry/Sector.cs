using System.Collections.Generic;
using Helion.Core.Resource.MapsNew.Components;
using Helion.Core.Util;
using Helion.Core.Util.Extensions;

namespace Helion.Core.Worlds.Geometry
{
    public class Sector
    {
        public readonly int Index;
        public readonly SectorPlane Floor;
        public readonly SectorPlane Ceiling;
        public readonly List<Side> Sides = new List<Side>();
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
            LightLevelNormalized = (lightLevel * Constants.InverseLightLevel).Clamp(0, 1);
        }
    }
}
