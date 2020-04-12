using Helion.Core.Util;
using UnityEngine;

namespace Helion.Core.Resource.Maps.Doom
{
    public class DoomSidedef
    {
        public readonly int Index;
        public Vector2 Offset;
        public UpperString UpperTexture;
        public UpperString MiddleTexture;
        public UpperString LowerTexture;
        public DoomSector Sector;
        public DoomLinedef Line;

        public bool IsFront => ReferenceEquals(Line.Front, this);
        public bool IsBack => !IsFront;

        public DoomSidedef(int index, Vector2 offset, UpperString upper, UpperString middle, UpperString lower,
                           DoomSector sector)
        {
            Index = index;
            Offset = offset;
            UpperTexture = upper;
            MiddleTexture = middle;
            LowerTexture = lower;
            Sector = sector;
        }
    }
}
