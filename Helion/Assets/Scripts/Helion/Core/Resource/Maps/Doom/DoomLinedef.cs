using Helion.Core.Resource.Maps.Shared;
using Helion.Core.Util;

namespace Helion.Core.Resource.Maps.Doom
{
    /// <summary>
    /// A line in a Doom map.
    /// </summary>
    public class DoomLinedef
    {
        public readonly int Index;
        public readonly MapVertex Start;
        public readonly MapVertex End;
        public readonly DoomSidedef Front;
        public readonly Optional<DoomSidedef> Back;

        public bool OneSided => !TwoSided;
        public bool TwoSided => Back.HasValue;
        public float Length => Start.Distance(End);

        public DoomLinedef(int index, MapVertex start, MapVertex end, DoomSidedef front, DoomSidedef back,
            ushort type, ushort flags, ushort sectorTag)
        {
            Index = index;
            Start = start;
            End = end;
            Front = front;
            Back = back;
        }
    }
}
