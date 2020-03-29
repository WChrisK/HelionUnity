using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomSidedef
    {
        public Vector2 Offset;
        public UpperString UpperTexture;
        public UpperString MiddleTexture;
        public UpperString LowerTexture;
        public DoomSector Sector;
        public DoomLinedef Line;

        public DoomSidedef(Vector2 offset, UpperString upper, UpperString middle, UpperString lower, DoomSector sector)
        {
            Offset = offset;
            UpperTexture = upper;
            MiddleTexture = middle;
            LowerTexture = lower;
            Sector = sector;
        }
    }
}
