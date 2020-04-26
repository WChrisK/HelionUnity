using Helion.Util;

namespace Helion.Resource.Maps.Components
{
    public class MapSector
    {
        public readonly int Index;
        public int FloorHeight;
        public int CeilingHeight;
        public UpperString FloorTexture = Constants.NoTexture;
        public UpperString CeilingTexture = Constants.NoTexture;
        public int LightLevel = 160;
        public int Special;
        public int Tag;
        public string Comment = string.Empty;

        // ZDoom specific.
        // TODO

        public MapSector(int index)
        {
            Index = index;
        }
    }
}
