using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomSidedef
    {
        public DoomSector Sector;
        public UpperString UpperTexture;
        public UpperString MiddleTexture;
        public UpperString LowerTexture;
        public DoomLinedef Line;

        public DoomSidedef(DoomSector sector, UpperString upper, UpperString middle, UpperString lower)
        {
            Sector = sector;
            UpperTexture = upper;
            MiddleTexture = middle;
            LowerTexture = lower;
        }
    }
}
